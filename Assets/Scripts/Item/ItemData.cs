// Item.cs
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Itens/Novo Item")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public ItemType type;
}

public enum ItemType
{
    Consumable,
    Weapon,
    QuestItem
}