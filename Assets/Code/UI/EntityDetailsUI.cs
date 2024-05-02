using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// When given an entity, this will extract any useful information and display it to the player
// Can start off as just a string
public class EntityDetailsUI : MonoBehaviour
{
    public GameObject DetailsUIParent;
    public Transform EntryParent;
    public DetailsUIEntry DetailsEntryPrefab;

    private DR_Cell selectedCell = null;
    private List<DetailsUIEntry> detailsEntries = new List<DetailsUIEntry>();

    public void SetCell(DR_Cell newCell){
        selectedCell = newCell ?? DR_GameManager.instance.TryGetPlayerCell();
        UpdateUI();
    }

    private void AddDetailsEntry(DR_Entity entity){
        DetailsUIEntry detailEntry = Instantiate(DetailsEntryPrefab, EntryParent);
        detailEntry.Init(entity);
        detailsEntries.Add(detailEntry);
    }

    // For cell specific details only
    private void AddDetailsEntry(DR_Cell cell){
        DetailsUIEntry detailEntry = Instantiate(DetailsEntryPrefab, EntryParent);
        detailEntry.Init(cell);
        detailsEntries.Add(detailEntry);
    }

    private void ClearDetails(){
        foreach (var entry in detailsEntries){
            Destroy(entry.gameObject);
        }
        detailsEntries.Clear();
    }

    private void UpdateUI(){
        ClearDetails();
        if (selectedCell == null){
            DetailsUIParent.SetActive(false);
            return;
        }

        if (selectedCell.Actor != null){
            AddDetailsEntry(selectedCell.Actor);
        }

        if (selectedCell.Item != null){
            AddDetailsEntry(selectedCell.Item);
        }

        if (selectedCell.Prop != null){
            AddDetailsEntry(selectedCell.Prop);
        }

        AddDetailsEntry(selectedCell);
        
        DetailsUIParent.SetActive(true);
    }
}
