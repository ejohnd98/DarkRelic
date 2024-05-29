using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderedAction {

    public DR_Action originalAction;

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

    public Sprite WallTexture, FloorTexture, FogTexture;
    public GameObject CellObj;

    Dictionary<DR_Entity, GameObject> EntityObjects;
    Dictionary<Vector2Int, GameObject> CellObjects;

    Queue<RenderedAction> actionQueue = new();

    //Vector2Int selectedCellPos;
    public bool currentlyUpdating = false;

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

    void LateUpdate() {
        if (DR_GameManager.instance.CurrentState == DR_GameManager.GameState.INVALID){
            return;
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
        DR_Map currentMap = gm.CurrentMap;

        //TODO: use this
        List<RenderedAction> actionsToBeRendered = new();

        // Loop until all actions are taken care of
        while (actionQueue.Count > 0){

            actionsToBeRendered.Clear();

            // Determine which actions can be visualized at the same time
            while (actionQueue.Count > 0){
                RenderedAction nextAction = actionQueue.Peek();

                bool canAddActionToList = true;//actionsToBeRendered.Count == 0;

                foreach (var queuedAction in actionsToBeRendered){
                    if (nextAction.OverlapsWith(queuedAction)){
                        canAddActionToList = false;
                        break;
                    }
                }

                if (canAddActionToList){

                    Vector2Int nextActionPos = nextAction.originalAction.owner.Position;
                    //Skip rendering action if entity not visible (could be made more robust as actions can affect multiple spaces)
                    if (!currentMap.IsVisible[nextActionPos.y, nextActionPos.x] && !DR_GameManager.instance.debug_disableFOV){
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
                Debug.Log("Playing animations for: " + ActionListToString(actionsToBeRendered));
                yield return new WaitForSeconds(1.0f);
            }

            //TODO: implement method in RenderedAction to check compatability with provided list of other actions
            // These are incompatabile if they use the same entities in any ways (idea for later, implement functions in actions to return a list of all entities affected)
            // Keep adding actions to actionsToBeRendered until an incompatability or queue is empty
            // Then, create animation objects to actually visualize these actions.
            // Once that is done, if queue has more, repeat the process. Do this until queue is empty.
        }


        // Bring map up to date (this may become redundant if the action animations make the same changes?)
        currentlyUpdating = false;

        if (createTiles){
            CreateTiles();
        }else{
            UpdateTiles();
        }
        UpdateEntities();
    }

    private static string ActionListToString(List<RenderedAction> list){
        string result = "[";
        foreach (RenderedAction action in list){
            result += "(" + action.ToString() + "), ";
        }
        return result + "]";
    }

    private void CreateTiles(){
        DR_Map currentMap = DR_GameManager.instance.CurrentMap;

        foreach(GameObject obj in CellObjects.Values){
            Destroy(obj);
        }
        CellObjects.Clear();

        // Add new visuals
        for(int y = 0; y < currentMap.MapSize.y; y++){
            for(int x = 0; x < currentMap.MapSize.x; x++){
                GameObject NewCellObj = Instantiate(CellObj,new Vector3(x, y, 0),Quaternion.identity, transform);
                CellObjects.Add(new Vector2Int(x,y), NewCellObj);
                NewCellObj.name = "Cell (" + x + ", " + y + ")";

                // Should this be in UpdateTiles instead?
                NewCellObj.GetComponent<CellObj>().SetBlood(currentMap.GetCell(new Vector2Int(x,y)));
            }
        }

        UpdateTiles();
    }

    private void UpdateTiles(){
        DR_Map currentMap = DR_GameManager.instance.CurrentMap;

        foreach (var (pos, obj) in CellObjects){
            Sprite CellSprite = FogTexture;
            if (currentMap.IsVisible[pos.y, pos.x] || DR_GameManager.instance.debug_disableFOV){
                CellSprite = currentMap.Cells[pos.y, pos.x].bBlocksMovement? WallTexture : FloorTexture;
                obj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                CellObjects[pos].GetComponent<CellObj>().SetBlood(currentMap.Cells[pos.y, pos.x]);

            }else if (currentMap.IsKnown[pos.y, pos.x]){
                CellSprite = currentMap.Cells[pos.y, pos.x].bBlocksMovement? WallTexture : FloorTexture;
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
        DR_Map currentMap = DR_GameManager.instance.CurrentMap;

        foreach(DR_Entity entity in currentMap.Entities){
            bool isVisible = currentMap.IsVisible[entity.Position.y, entity.Position.x] || DR_GameManager.instance.debug_disableFOV;
            if (isVisible && !EntityObjects.ContainsKey(entity)){
                AddEntityObj(entity);
            }
        }

        List<DR_Entity> entitiesToRemove = new List<DR_Entity>();
        foreach(DR_Entity entity in EntityObjects.Keys){
            bool isVisible = currentMap.IsVisible[entity.Position.y, entity.Position.x] || DR_GameManager.instance.debug_disableFOV;
            if (entity.noLongerValid || !entity.isOnMap || !isVisible){
                entitiesToRemove.Add(entity);
            }
        }
        foreach(DR_Entity entity in entitiesToRemove){
            Destroy(EntityObjects[entity]);
            EntityObjects.Remove(entity);
        }

        foreach(DR_Entity Entity in EntityObjects.Keys){
            bool isVisible = currentMap.IsVisible[Entity.Position.y, Entity.Position.x];
            bool isKnown = currentMap.IsKnown[Entity.Position.y, Entity.Position.x];
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
            Vector3 pos = Entity.GetPosFloat(GetDepthForEntity(Entity));;
            
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
    }

    // public void ResetSelectedCell(){
    //     if (CellObjects.ContainsKey(selectedCellPos)){
    //         GameObject selectedCell = CellObjects[selectedCellPos];
    //         selectedCell.GetComponent<CellObj>().SetSelected(false);
    //     }
    // }

    // public void SetSelectedCell(Vector2Int pos){
    //     ResetSelectedCell();
    //     if (CellObjects.ContainsKey(pos)){
    //         GameObject selectedCell = CellObjects[pos];
    //         selectedCell.GetComponent<CellObj>().SetSelected(true);
    //         selectedCellPos = pos;
    //     }
    // }

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
