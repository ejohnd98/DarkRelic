using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Renderer : MonoBehaviour
{
    public static DR_Renderer instance;
    public static int animsActive = 0;

    public static float ActorDepth = -1.0f;
    public static float ItemDepth = -0.75f;
    public static float PropDepth = -0.5f;

    public Sprite WallTexture, FloorTexture, FogTexture;
    public GameObject CellObj;

    Dictionary<DR_Entity, GameObject> EntityObjects;
    Dictionary<Vector2Int, GameObject> CellObjects;

    Vector2Int selectedCellPos;

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
        AnimationSystem.UpdateAnims(Time.deltaTime);
        UpdateEntities(Time.deltaTime);
    }

    public void UpdateTiles(){
        DR_Map currentMap = DR_GameManager.instance.CurrentMap;

        foreach (var (pos, obj) in CellObjects){
            Sprite CellSprite = FogTexture;
            if (currentMap.IsVisible[pos.y, pos.x] || DR_GameManager.instance.debug_disableFOV){
                CellSprite = currentMap.Cells[pos.y, pos.x].bBlocksMovement? WallTexture : FloorTexture;
                obj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }else if (currentMap.IsKnown[pos.y, pos.x]){
                CellSprite = currentMap.Cells[pos.y, pos.x].bBlocksMovement? WallTexture : FloorTexture;
                obj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
            obj.GetComponent<SpriteRenderer>().sprite = CellSprite;
        }
    }

    public void CreateTiles(){
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

                NewCellObj.GetComponent<CellObj>().SetBlood(currentMap.GetCell(new Vector2Int(x,y)));
            }
        }

        UpdateTiles();
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

    public void UpdateEntities(float deltaTime){
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
            Vector3 pos;
            MoveAnimation moveAnim = Entity.GetComponent<MoveAnimation>();
            AttackAnimation attackAnim = Entity.GetComponent<AttackAnimation>();
            if (moveAnim != null){
                pos = moveAnim.GetAnimPosition(GetDepthForEntity(Entity));
            }else if (attackAnim != null){
                pos = attackAnim.GetAnimPosition(GetDepthForEntity(Entity));
            }else{
                pos = Entity.GetPosFloat(GetDepthForEntity(Entity));
            }
            
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

    //TODO: this function should not be being called from elsewhere really
    // instead perhaps each cell object can subscribe to an event on the DR_Cell?
    public void SetCellBloodState(Vector2Int pos, DR_Cell cell){
        if (CellObjects.ContainsKey(pos)){
            CellObjects[pos].GetComponent<CellObj>().SetBlood(cell);
        }
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
