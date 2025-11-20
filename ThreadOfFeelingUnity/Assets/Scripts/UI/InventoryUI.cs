using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Components;

namespace UI
{
    [System.Serializable]
    public class InventorySlotData
    {
        public Item item;
        public int quantity;
        
        public bool IsEmpty => item == null;
        
        public void Clear()
        {
            item = null;
            quantity = 0;
        }
    }

    public class InventoryUI : MonoBehaviour
    {
        [Header("슬롯 참조")]
        [SerializeField] private Transform slotsContainer;
        
        [Header("인벤토리 설정")]
        [SerializeField] private int maxSlots = 5;
        
        [Header("참조")]
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private HousingSceneUi housingSceneUI;
        
        private List<InventorySlotData> inventorySlots = new List<InventorySlotData>();
        private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();

        private void Start()
        {
            // HousingSceneUI 찾기
            if (housingSceneUI == null)
            {
                housingSceneUI = FindFirstObjectByType<HousingSceneUi>();
            }
            
            Debug.Log($"[InventoryUI] HousingSceneUI: {(housingSceneUI != null ? "찾음" : "없음")}");
            
            // ItemsContainer 자동 찾기
            if (itemsContainer == null)
            {
                GameObject room = GameObject.Find("Room");
                if (room != null)
                {
                    Transform container = room.transform.Find("ItemsContainer");
                    if (container != null)
                        itemsContainer = container;
                }
            }
            
            InitializeInventory();
            LoadAvailableItems();
        }

        private void InitializeInventory()
        {
            for (int i = 0; i < maxSlots; i++)
            {
                inventorySlots.Add(new InventorySlotData());
            }

            FindExistingSlots();
        }

        private void FindExistingSlots()
        {
            slotUIList.Clear();

            for (int i = 0; i < slotsContainer.childCount; i++)
            {
                Transform slotTransform = slotsContainer.GetChild(i);
                
                InventorySlotUI slotUI = slotTransform.GetComponent<InventorySlotUI>();
                if (slotUI == null)
                {
                    slotUI = slotTransform.gameObject.AddComponent<InventorySlotUI>();
                }
                
                slotUI.Initialize(i, this);
                slotUIList.Add(slotUI);
            }

            maxSlots = slotUIList.Count;
        }

        private void LoadAvailableItems()
        {
            Item[] allItems = Resources.LoadAll<Item>("Items");
            
            Debug.Log($"[InventoryUI] 전체 아이템: {allItems.Length}개");

            HashSet<string> placedItemNames = new HashSet<string>();
            
            if (itemsContainer != null)
            {
                foreach (Transform child in itemsContainer)
                {
                    string itemName = child.name.Replace("(Clone)", "").Replace("_0", "").Trim();
                    placedItemNames.Add(itemName);
                }
            }

            int addedCount = 0;
            foreach (Item item in allItems)
            {
                // Reward 타입만
                if (item.itemType != Item.ItemType.Reward)
                    continue;

                string prefabName = item.itemPrefab != null ? item.itemPrefab.name : "";
                
                if (!placedItemNames.Contains(prefabName) && !placedItemNames.Contains(item.itemName))
                {
                    if (AddItem(item))
                    {
                        addedCount++;
                        Debug.Log($"[InventoryUI] 인벤토리에 추가: {item.itemName}");
                    }
                }
            }
            
            Debug.Log($"[InventoryUI] 인벤토리에 {addedCount}개 아이템 추가됨");
        }

        public bool AddItem(Item item)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].IsEmpty)
                {
                    inventorySlots[i].item = item;
                    inventorySlots[i].quantity = 1;
                    
                    UpdateSlotUI(i);
                    return true;
                }
            }
            
            return false;
        }

        public bool RemoveItem(Item item)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (!inventorySlots[i].IsEmpty && inventorySlots[i].item == item)
                {
                    inventorySlots[i].Clear();
                    UpdateSlotUI(i);
                    return true;
                }
            }
            return false;
        }

        private void UpdateSlotUI(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < slotUIList.Count)
            {
                slotUIList[slotIndex].UpdateDisplay(inventorySlots[slotIndex]);
            }
        }

        public void OnSlotClicked(int slotIndex) {
            Debug.Log($"===== OnSlotClicked 호출 ===== 슬롯: {slotIndex}");
            
            if (inventorySlots[slotIndex].IsEmpty)
            {
                Debug.Log("빈 슬롯");
                return;
            }

            Item selectedItem = inventorySlots[slotIndex].item;
            Debug.Log($"선택된 아이템: {selectedItem.itemName}");
            Debug.Log($"housingSceneUI: {(housingSceneUI != null ? "있음" : "없음")}");

            if (housingSceneUI != null)
            {
                Debug.Log("StartPlacement 호출 시작!");
                housingSceneUI.StartPlacement(selectedItem);
                Debug.Log("StartPlacement 호출 완료!");
                
                RemoveItem(selectedItem);
                Debug.Log("RemoveItem 완료!");
            }
            else
            {
                Debug.LogError("housingSceneUI가 null입니다!");
            }
        }

        public void ReturnItem(Item item)
        {
            AddItem(item);
        }
    }

    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
    {
        private Image itemIcon;
        private int slotIndex;
        private InventoryUI inventoryUI;

        public void Initialize(int index, InventoryUI inventory)
        {
            slotIndex = index;
            inventoryUI = inventory;

            Transform iconTransform = transform.Find("ItemIcon");
            
            if (iconTransform != null)
            {
                itemIcon = iconTransform.GetComponent<Image>();
            }
            else
            {
                GameObject iconObj = new GameObject("ItemIcon");
                iconObj.transform.SetParent(transform, false);
                itemIcon = iconObj.AddComponent<Image>();
            }
            
            SetupIconRectTransform();
            itemIcon.raycastTarget = false;
            itemIcon.color = new Color(1, 1, 1, 0);
        }

        private void SetupIconRectTransform()
        {
            RectTransform iconRect = itemIcon.rectTransform;
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.offsetMin = new Vector2(5, 5);
            iconRect.offsetMax = new Vector2(-5, -5);
        }

        public void UpdateDisplay(InventorySlotData slotData)
        {
            if (itemIcon == null)
                return;

            if (slotData.IsEmpty)
            {
                itemIcon.sprite = null;
                itemIcon.color = new Color(1, 1, 1, 0);
            }
            else
            {
                itemIcon.sprite = slotData.item.itemIcon;
                itemIcon.color = new Color(1, 1, 1, 1);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            inventoryUI.OnSlotClicked(slotIndex);
        }
    }
}