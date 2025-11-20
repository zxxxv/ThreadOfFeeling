using UnityEngine;

namespace Components
{
    [CreateAssetMenu(fileName = "Item", menuName = "GameData/Item")]
    public class Item : ScriptableObject
    {
        public enum ItemType {   
        Reward, // 시나리오 보상으로 얻은 아이템 
        Furniture // 기존 가구 
      }

        public int itemId;
        public string itemName;
        public string itemExplain;
        public Sprite itemIcon;
        public ItemType itemType;
        public GameObject itemPrefab;      }

}