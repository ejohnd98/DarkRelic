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
        //TODO: add a monobehaviour component to the entity objects so they can update the move position theirselves?
        UpdateEntities(Time.deltaTime);
        //DR_GameManager.instance.UpdateCamera();
    }

    // TODO: only update tiles that need updating (don't delete everything everytime, reuse game objects)
    public void UpdateTiles(){
        DR_Map currentMap = DR_GameManager.instance.CurrentMap;

        foreach(GameObject obj in CellObjects.Values){
            Destroy(obj);
        }
        CellObjects.Clear();

        // Add new visuals
        for(int y = 0; y < currentMap.MapSize.y; y++){
            for(int x = 0; x < currentMap.MapSize.x; x++){
                GameObject NewCellObj = Instantiate(CellObj,new Vector3(x, y, 0),Quaternion.identity, transform);
                Sprite CellSprite = FogTexture;
                if (currentMap.IsVisible[y, x] || DR_GameManager.instance.debug_disableFOV){
                    CellSprite = currentMap.Cells[y,x].bBlocksMovement? WallTexture : FloorTexture;
                }else if (currentMap.IsKnown[y, x]){
                    CellSprite = currentMap.Cells[y,x].bBlocksMovement? WallTexture : FloorTexture;
                    NewCellObj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                }
                NewCellObj.GetComponent<SpriteRenderer>().sprite = CellSprite;
                CellObjects.Add(new Vector2Int(x,y), NewCellObj);
            }
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

    public void UpdateEntities(float deltaTime){
        DR_Map currentMap = DR_GameManager.instance.CurrentMap;

        foreach(DR_Entity entity in currentMap.Entities){
            if (!EntityObjects.ContainsKey(entity)){
                AddEntityObj(entity);
            }
        }

        List<DR_Entity> entitiesToRemove = new List<DR_Entity>();
        foreach(DR_Entity entity in EntityObjects.Keys){
            if (entity.noLongerValid || !entity.isOnMap){
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
            MoveAnimComponent moveComponent = Entity.GetComponent<MoveAnimComponent>();
            if (moveComponent != null && moveComponent.isMoving){
                moveComponent.AnimStep(deltaTime);
                pos = moveComponent.GetAnimPosition(GetDepthForEntity(Entity));
            }else{
                pos = Entity.GetPosFloat(GetDepthForEntity(Entity));
            }
            
            EntityObjects[Entity].transform.position = pos;
            EntityObjects[Entity].GetComponent<SpriteRenderer>().sprite = spriteComponent.Sprite;

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

    public static float GetDepthForEntity(DR_Entity entity){
        if (entity.HasComponent<PropComponent>()){
            return PropDepth;
        }

        if (entity is DR_Entity){
            return ItemDepth;
        }

        return ActorDepth;
    }
}
