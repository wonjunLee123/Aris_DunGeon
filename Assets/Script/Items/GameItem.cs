using UnityEngine;

public enum GameItemType { Consumable, Equipment }

public abstract class GameItem : MonoBehaviour
{
    public string itemName;
    public string description;
    public GameItemType itemType;

    public abstract void Use(GameObject player);
}
