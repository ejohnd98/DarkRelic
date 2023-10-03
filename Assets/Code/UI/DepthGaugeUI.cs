using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DepthGaugeUI : MonoBehaviour
{
    DR_Dungeon dungeon;
    public GameObject DepthGaugeUIParent;
    public TextMeshProUGUI TitleText; 
    public GameObject LevelEntryPrefab;
    public GameObject LevelEntryIconPrefab; 
    public Sprite PlayerIcon, BossIcon;
    public Transform LevelEntryParent;

    List<GameObject> LevelEntryObjects;

    private void Awake() {
        LevelEntryObjects = new List<GameObject>();
    }

    public void SetDungeon(DR_Dungeon newDungeon){
        dungeon = newDungeon;
        UpdateUI();
    }

    public void HideUI(){
        DepthGaugeUIParent.SetActive(false);
    }

    private void UpdateUI(){
        if (dungeon == null){
            HideUI();
            return;
        }
        string titleText = "Dungeon";
        TitleText.text = titleText;

        foreach (GameObject obj in LevelEntryObjects){
            Destroy(obj);
        }
        LevelEntryObjects.Clear();

        for (int i = 0; i < dungeon.maps.Count; i++){
            GameObject levelEntry = Instantiate(LevelEntryPrefab, Vector3.zero, Quaternion.identity, LevelEntryParent);
            LevelEntryObjects.Add(levelEntry);

            if (i == dungeon.mapIndex){
                GameObject playerIconObj = Instantiate(LevelEntryIconPrefab, Vector3.zero, Quaternion.identity, levelEntry.transform);
                playerIconObj.GetComponent<Image>().sprite = PlayerIcon;
            }

            //TODO: create component to track what map index an entity is on (maybe only for boss + player?)
            //temp way of figuring out where boss is:
            if (dungeon.maps[i].Entities.Contains(DR_GameManager.instance.GetBoss())){
                GameObject bossIconObj = Instantiate(LevelEntryIconPrefab, Vector3.zero, Quaternion.identity, levelEntry.transform);
                bossIconObj.GetComponent<Image>().sprite = BossIcon;
            }
        }
        
        DepthGaugeUIParent.SetActive(true);
    }
}
