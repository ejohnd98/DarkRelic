using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class RenderedAction {

    public DR_Action originalAction;

    public bool MustBeAnimatedAlone(){
        return originalAction is StairAction;
    }

    public RenderedAction(DR_Action action){
        originalAction = action;
    }

    public override string ToString(){
        return originalAction.owner.Name + ": " + originalAction.GetType().Name;
    }

    public bool OverlapsWith(RenderedAction other){
        List<DR_Entity> relatedEntities = originalAction.GetRelatedEntities();

        foreach (DR_Entity entity in other.originalAction.GetRelatedEntities()){
            if (relatedEntities.Contains(entity)){
                return true;
            }
        }
        return false;
    }
}

public class GameRenderer : MonoBehaviour
{
    public static GameRenderer instance;

    public static float ActorDepth = -1.0f;
    public static float ItemDepth = -0.75f;
    public static float PropDepth = -0.5f;

    public Camera MainCamera;
    public Vector3 cameraOffset;
    public Transform renderedPlayer;

    public DR_Map currentRenderedMap = null;

    public Sprite WallTexture, FloorTexture, FogTexture;
    public GameObject CellObj;

    Dictionary<DR_Entity, GameObject> EntityObjects;
    Dictionary<Vector2Int, GameObject> CellObjects;

    Vector2Int selectedCellPos;

    Queue<RenderedAction> actionQueue = new();
    List<ActionAnimation> activeAnimations = new();

    //Vector2Int selectedCellPos;
    public bool currentlyUpdating = false;

    public bool lockCameraPos = false; //Messy

    public bool HasActionsQueued(){
        return actionQueue.Count > 0;
    }

    void Awake()
    {
        if (instance != null){
            Debug.LogError("Renderer already exists!");
        }
    
        instance = this;
        EntityObjects = new Dictionary<DR_Entity, GameObject>();
        CellObjects = new Dictionary<Vector2Int, GameObject>();
    }

    void Update() {
        if (DR_GameManager.instance.CurrentState == DR_GameManager.GameState.INVALID){
            return;
        }

        float debugTimeMod = 1.0f;
        if (Input.GetKey(KeyCode.LeftControl)){
            debugTimeMod = 5.0f;
        }

        if (activeAnimations.Count > 0){
            for (int i = 0; i < activeAnimations.Count; i++){
                
                var anim = activeAnimations[i];
                anim.AnimStep(Time.deltaTime * debugTimeMod);

                if (!anim.isAnimating){
                    activeAnimations.RemoveAt(i);
                    i--;
                }
            }
        }
        //AnimationSystem.UpdateAnims(Time.deltaTime);
        //UpdateEntities(Time.deltaTime);
    }

    public void AddAction(DR_Action action){
        if (action is WaitAction){
            return;    
        }

        actionQueue.Enqueue(new(action));
    }

    public void FullyUpdateRenderer(bool createTiles = false){
        if (currentlyUpdating){
            Debug.LogAssertion("Tried to update renderer mid-update");
            return;
        }
        currentlyUpdating = true;

        StartCoroutine(RenderActions(createTiles));
    }

    private IEnumerator RenderActions(bool createTiles){
        DR_GameManager gm = DR_GameManager.instance;

        List<RenderedAction> actionsToBeRendered = new();

        // Loop until all actions are taken care of
        while (actionQueue.Count > 0){

            actionsToBeRendered.Clear();

            // Determine which actions can be visualized at the same time
            while (actionQueue.Count > 0){
                RenderedAction nextAction = actionQueue.Peek();

                bool canAddActionToList = true;//actionsToBeRendered.Count == 0;

                foreach (var queuedAction in actionsToBeRendered){
                    if (nextAction.OverlapsWith(queuedAction) || nextAction.MustBeAnimatedAlone()){
                        canAddActionToList = false;
                        break;
                    }
                }

                if (canAddActionToList){

                    Vector2Int nextActionPos = nextAction.originalAction.owner.Position;
                    //Skip rendering action if entity not visible (could be made more robust as actions can affect multiple spaces)
                    if (!currentRenderedMap.IsVisible[nextActionPos.y, nextActionPos.x] 
                            && !DR_GameManager.instance.debug_disableFOV
                            && !nextAction.originalAction.owner.HasComponent<PlayerComponent>()){
                        actionQueue.Dequeue();
                        continue;
                    }

                    actionsToBeRendered.Add(actionQueue.Dequeue());
                }else{
                    break;
                }
                
            }

            // Create and start current group of animations
            if (actionsToBeRendered.Count > 0){
                //Debug.Log("Playing animations for: " + ActionListToString(actionsToBeRendered));
            }

            for (int i = 0; i < actionsToBeRendered.Count; i++)
            {
                var anim = CreateActionAnimation(actionsToBeRendered[i]);

                if (anim != null){
                    activeAnimations.Add(anim);
                    anim.StartAnim();

                    if (anim.action.originalAction is StairAction){
                        createTiles = true;
                    }

                    
                    if (anim.action.originalAction is AttackAction attack){
                        if (attack.killed && i != actionsToBeRendered.Count - 1)
                        {
                            // Briefly pause anim if killed and there are more actions to be performed
                            // This is a hacky attempt to prevent move anims from overlapping with death FX
                            yield return new WaitForSeconds(0.1f);
                        }

                        LogSystem.instance.AddDamageLog(attack.damageEvent);
                        
                    }else{
                        LogSystem.instance.AddLog(actionsToBeRendered[i].originalAction);
                    }
                }else{
                    LogSystem.instance.AddLog(actionsToBeRendered[i].originalAction);
                }
            }

            yield return new WaitUntil(() => activeAnimations.Count == 0);

            //TODO: How should fov be shown per-step (maybe it doesn't need to be if it will always be updated after a player moves?)
        }


        // Bring map up to date (this may become redundant if the action animations make the same changes?)
        currentlyUpdating = false;

        currentRenderedMap = DR_GameManager.instance.CurrentMap;
        if (createTiles){
            CreateTiles();
        }else{
            UpdateTiles();
        }
        UpdateEntities();
    }

    private ActionAnimation CreateActionAnimation(RenderedAction renderedAction){

        var action = renderedAction.originalAction;
        if (action is StairAction stairAction){
            //TODO: messy
            return new StairAnimation(renderedAction);
        }
        // With any luck as long as the anims move everything to where they're supposed to be, this will be good enough for keeping map in sync
        // Stuff going out of or coming into visibility MIGHT need special care though
        GameObject entityObj;
        EntityObjects.TryGetValue(renderedAction.originalAction.owner, out entityObj);
        if (entityObj == null){
            return null;
        }
        

        Transform entityTransform = entityObj.transform;
        Vector2Int startPos = new(Mathf.RoundToInt(entityTransform.position.x), Mathf.RoundToInt(entityTransform.position.y));

        
        if (action is MoveAction moveAction){
            Vector2Int endPos = moveAction.pos;
            return new MoveAnimation(renderedAction, entityTransform, startPos, endPos);
        }
        if (action is AttackAction attackAction){

            GameObject targetEntityObj;
            EntityObjects.TryGetValue(attackAction.target, out targetEntityObj);
            if (targetEntityObj == null){
                Debug.LogError("CreateActionAnimation: Could not get targeted entity obj for attack anim");
                return null;
            }
            Transform targetEntityTransform = targetEntityObj.transform;
            Vector2Int endPos = new(Mathf.RoundToInt(targetEntityTransform.position.x), Mathf.RoundToInt(targetEntityTransform.position.y));
            return new AttackAnimation(renderedAction, entityTransform, targetEntityTransform, startPos, endPos);
        }
        if (action is AbilityAction abilityAction){

            //TODO: more properly determine what animation to use for an ability (specify on ability itself?)
            if (abilityAction.ability is BloodBoltAbility bloodBoltAbility){
                return new ProjectileAnimation(renderedAction, entityTransform, EntityObjects[bloodBoltAbility.target].transform);
            }

            return new AbilityAnimation(renderedAction, entityTransform);
        }

        return null;
    }

    private static string ActionListToString(List<RenderedAction> list){
        string result = "[";
        foreach (RenderedAction action in list){
            result += "(" + action.ToString() + "), ";
        }
        return result + "]";
    }

    private void CreateTiles(){
        foreach(GameObject obj in CellObjects.Values){
            Destroy(obj);
        }
        CellObjects.Clear();

        // Add new visuals
        for(int y = 0; y < currentRenderedMap.MapSize.y; y++){
            for(int x = 0; x < currentRenderedMap.MapSize.x; x++){
                if (currentRenderedMap.GetCell(new Vector2Int(x,y)).neverRender){
                    continue;
                }

                GameObject NewCellObj = Instantiate(CellObj,new Vector3(x, y, 0),Quaternion.identity, transform);
                CellObjects.Add(new Vector2Int(x,y), NewCellObj);
                NewCellObj.name = "Cell (" + x + ", " + y + ")";

                // Should this be in UpdateTiles instead?
                NewCellObj.GetComponent<CellObj>().SetBlood(currentRenderedMap.GetCell(new Vector2Int(x,y)));
            }
        }

        UpdateTiles();
    }

    private void UpdateTiles(){
        foreach (var (pos, obj) in CellObjects){
            Sprite CellSprite = FogTexture;
            if (currentRenderedMap.IsVisible[pos.y, pos.x] || DR_GameManager.instance.debug_disableFOV){
                CellSprite = currentRenderedMap.Cells[pos.y, pos.x].bBlocksMovement? WallTexture : FloorTexture;
                obj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                CellObjects[pos].GetComponent<CellObj>().SetBlood(currentRenderedMap.Cells[pos.y, pos.x]);

            }else if (currentRenderedMap.IsKnown[pos.y, pos.x]){
                CellSprite = currentRenderedMap.Cells[pos.y, pos.x].bBlocksMovement? WallTexture : FloorTexture;
                obj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                CellObjects[pos].GetComponent<CellObj>().SetBlood(null);
            }else{
                CellObjects[pos].GetComponent<CellObj>().SetBlood(null);
            }
            obj.GetComponent<SpriteRenderer>().sprite = CellSprite;
        }
    }

    public void ClearAllObjects(){
        foreach(GameObject obj in CellObjects.Values){
            Destroy(obj);
        }
        CellObjects.Clear();

        foreach(GameObject obj in EntityObjects.Values){
            Destroy(obj);
        }
        EntityObjects.Clear();
    }

    private void UpdateEntities(){
        foreach(DR_Entity entity in currentRenderedMap.Entities){
            bool isVisible = currentRenderedMap.IsVisible[entity.Position.y, entity.Position.x] || DR_GameManager.instance.debug_disableFOV;
            if (isVisible && !EntityObjects.ContainsKey(entity)){
                AddEntityObj(entity);
            }
        }

        List<DR_Entity> entitiesToRemove = new List<DR_Entity>();
        foreach(DR_Entity entity in EntityObjects.Keys){
            bool isVisible = currentRenderedMap.IsVisible[entity.Position.y, entity.Position.x] || DR_GameManager.instance.debug_disableFOV;
            if (entity.noLongerValid || !entity.isOnMap || !isVisible){
                entitiesToRemove.Add(entity);
            }
        }
        foreach(DR_Entity entity in entitiesToRemove){
            Destroy(EntityObjects[entity]);
            EntityObjects.Remove(entity);
        }

        foreach(DR_Entity Entity in EntityObjects.Keys){
            bool isVisible = currentRenderedMap.IsVisible[Entity.Position.y, Entity.Position.x];
            bool isKnown = currentRenderedMap.IsKnown[Entity.Position.y, Entity.Position.x];
            if (!isVisible && !isKnown && !DR_GameManager.instance.debug_disableFOV){
                EntityObjects[Entity].SetActive(false);
                continue;
            }
            
            bool isProp = Entity.HasComponent<PropComponent>();
            if (!isVisible && (!isProp || !isKnown) && !DR_GameManager.instance.debug_disableFOV){
                EntityObjects[Entity].SetActive(false);
                continue;
            }

            SpriteComponent spriteComponent = Entity.GetComponent<SpriteComponent>();
            if (spriteComponent == null){
                EntityObjects[Entity].SetActive(false);
                continue;
            }

            EntityObjects[Entity].SetActive(true);

            // TODO: make proper system to determine z depth for each entity
            Vector3 pos = Entity.GetPosFloat(GetDepthForEntity(Entity));
            
            EntityObjects[Entity].transform.position = pos;

            // Messy way of setting animation while still letting doors reflect their sprite when it changes
            if (Entity.GetComponent<SpriteComponent>().hasAnimation){
                
            }else{
                EntityObjects[Entity].GetComponent<SpriteRenderer>().sprite = spriteComponent.Sprite;
            }
            

            if (!isVisible && isKnown && !DR_GameManager.instance.debug_disableFOV){
                EntityObjects[Entity].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }else{
                EntityObjects[Entity].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }  
    }

    public void AddEntityObj(DR_Entity entity){
        GameObject NewEntityObj = Instantiate(CellObj, Vector3.zero, Quaternion.identity, transform);
        EntityObjects[entity] = NewEntityObj;

        if (entity.GetComponent<SpriteComponent>() is SpriteComponent spriteComp && spriteComp.hasAnimation){
            NewEntityObj.GetComponent<CellObj>().SetAnim(spriteComp);
        }

        if (entity.HasComponent<PlayerComponent>()){
            renderedPlayer = NewEntityObj.transform;
        }
    }

    public void RemoveEntityObj(DR_Entity entity)
    {
        if (EntityObjects.ContainsKey(entity)){
            Destroy(EntityObjects[entity]);
            EntityObjects.Remove(entity);
        }
    }

    public void ResetSelectedCell(){
        if (CellObjects.ContainsKey(selectedCellPos)){
            GameObject selectedCell = CellObjects[selectedCellPos];
            selectedCell.GetComponent<CellObj>().SetSelected(false);
        }
    }

    public void SetSelectedCell(Vector2Int pos){
        ResetSelectedCell();
        if (CellObjects.ContainsKey(pos)){
            GameObject selectedCell = CellObjects[pos];
            selectedCell.GetComponent<CellObj>().SetSelected(true);
            selectedCellPos = pos;
        }
    }

    public void SetBlood(Vector2Int pos){
        CellObjects[pos].GetComponent<CellObj>().SetBlood(currentRenderedMap.Cells[pos.y, pos.x]);
    }

    public void UpdateCamera(bool forcePos = false)
    {
        //TODO: do not move camera when player attacks
        if (lockCameraPos){
            return;
        }

        Vector3 DesiredPos = MainCamera.transform.position;
        if (renderedPlayer != null){
            DesiredPos.x = renderedPlayer.position.x;
            DesiredPos.y = renderedPlayer.position.y;
        }
        DesiredPos += cameraOffset;
        
        if (forcePos){
            MainCamera.transform.position = DesiredPos;
            return;
        }

        float LerpAmount = Time.deltaTime * 3.0f;
        MainCamera.transform.position = Easings.QuadEaseOut(MainCamera.transform.position, DesiredPos, LerpAmount);
    }

    public static float GetDepthForEntity(DR_Entity entity){
        if (entity.HasComponent<PropComponent>()){
            return PropDepth;
        }

        if (entity.HasComponent<ItemComponent>()){
            return ItemDepth;
        }

        return ActorDepth;
    }
}
