using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDistributionEntry
{
    public ContentBase Value;
}

[CreateAssetMenu(fileName = "ItemDistribution", menuName = "Item Distribution")]
public class ItemDistribution : ScriptableObject
{
    public List<ItemDistributionEntry> commonItems;
    public List<ItemDistributionEntry> accursedItems;
    public List<ItemDistributionEntry> unholyItems;
}

