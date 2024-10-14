using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

// When given an entity, this will extract any useful information and display it to the player
// Can start off as just a string
public class DebugUI : MonoBehaviour
{
    public Button CancelButton;
    public GameObject UIParent;
    public Transform enemyGrid, propGrid, relicGrid;
    public GameObject ButtonPrefab;
    List<GameObject> Buttons;

    private bool waitingForInput = false;
    private Content contentWaitingForInput = null;

    private void Start() {
        Buttons = new List<GameObject>();

        foreach (var content in DR_GameManager.instance.enemyContentArray){
            AddButton(content, enemyGrid);
        }
        foreach (var content in DR_GameManager.instance.itemAltars){
            AddButton(content, propGrid);
        }
        AddButton(DR_GameManager.instance.healthAltarContent, propGrid);
        AddButton(DR_GameManager.instance.cursedAltar, propGrid);
        AddButton(DR_GameManager.instance.doorContent, propGrid);
        AddButton(DR_GameManager.instance.lockedDoorContent, propGrid);


        foreach (var content in LootHandler.instance.abilityContentObjects){
            AddButton(content, relicGrid);
        }
    }

    public void ToggleMenu(){
        if (!UIParent.activeSelf){
            StopPlacingContent();
        }
        UIParent.SetActive(!UIParent.activeSelf);
    }

    private void AddButton(ContentBase contentBase, Transform parent){
        GameObject itemButtonObj = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity, parent);
        UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();

        itemButton.SetSprite(contentBase.GetContentSprite());
        itemButton.onClick.AddListener(() => {SetContent(contentBase);});

        Buttons.Add(itemButtonObj);
    }

    void SetContent(ContentBase contentBase){
        if (contentBase is AbilityContent ability){
            var player = DR_GameManager.instance.GetPlayer();
            player.GetComponent<AbilityComponent>().AddAbilityFromContent(ability);
            player.GetComponent<LevelComponent>().UpdateStats();

            SoundSystem.instance.PlaySound("wizardSound");
        }else{
            CancelButton.gameObject.SetActive(true);
            waitingForInput = true;
            contentWaitingForInput = contentBase as Content;
            Cursor.SetCursor(UISystem.instance.targetingCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
            ToggleMenu();
        }
    }

    public void StopPlacingContent(){
        waitingForInput = false;
        contentWaitingForInput = null;
        CancelButton.gameObject.SetActive(false);
        Cursor.SetCursor(UISystem.instance.cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }

    // TODO: make this more like a palette + brush so multiple enemies can be easily placed at once
    // Should add a little X button beside the menu button that would clear the palette once done.
    void PlaceContent(Content content, Vector2Int pos){
        var gm = DR_GameManager.instance;
        var entity = EntityFactory.CreateEntityFromContent(content);
        var cell = gm.CurrentMap.GetCell(pos);

        if (entity.HasComponent<PropComponent>()){
            if (entity.GetComponent<AltarComponent>() is AltarComponent altar
                && altar.altarType != AltarType.HEALTH_ALTAR){
                altar.altarAbilityContent = LootHandler.instance.GetRandomAbility(altar.chestType);
            }


            if (gm.CurrentMap.AddProp(entity, pos)){
                SoundSystem.instance.PlaySound("wizardSound");
            }
        }else{
            if (entity.HasComponent<LevelComponent>()){
                entity.GetComponent<LevelComponent>().level = gm.CurrentDungeon.dungeonGenInfo.getFloorEnemyLevel(gm.CurrentDungeon.mapIndex);
                entity.GetComponent<LevelComponent>().UpdateStats();
            }
            
            if (gm.CurrentMap.AddActor(entity, pos)){
                SoundSystem.instance.PlaySound("wizardSound");
                gm.turnSystem.UpdateEntityLists(gm.CurrentMap);
            }
            
        }
        GameRenderer.instance.UpdateEntities();
    }

    void Update()
    {
        if (!waitingForInput){
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
                StopPlacingContent();
            }

        //TODO: this is still getting called when clicking the cancel button
        if (Input.GetMouseButtonDown(0) && DR_InputHandler.instance.mouseIsInWorld){
            Vector2Int MousePos = DR_InputHandler.instance.mouseWorldPosition;
            PlaceContent(contentWaitingForInput, MousePos);
        }
    }
}
