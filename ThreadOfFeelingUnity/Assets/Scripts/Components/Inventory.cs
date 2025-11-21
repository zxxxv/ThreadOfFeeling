using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components {

    [System.Serializable]
    public class Inventory {
        // 실제 아이템 데이터들이 저장될 리스트
        public List<InventorySlot> Slots { get; set; } = new List<InventorySlot>();

        // 슬롯당 최대 개수 제한
        private const int MAX_SLOT_CAPACITY = 10;

        public Inventory() {
            Slots = new List<InventorySlot>();
        }

        // -- 기능 --

        // 아이템 획득 (보상 받을 때 사용)
        public void AddItem(Item item, int amount = 1) {
            // 이미 가지고 있는 아이템인지 확인
            var slot = Slots.FirstOrDefault(s => s.itemId == item.itemId);

            if (slot != null) {
                // 이미 있는 경우: 개수만 증가
                if (slot.quantity < MAX_SLOT_CAPACITY) {
                    int spaceLeft = MAX_SLOT_CAPACITY - slot.quantity; // 남은 공간 계산
                    int amountToAdd = Mathf.Min(amount, spaceLeft);    // 실제로 더할 수 있는 양

                    slot.quantity += amountToAdd;

                    if (amount > spaceLeft) {
                        Debug.LogWarning($"[Inventory] 아이템이 가득 차서 {amount - spaceLeft}개는 버려졌습니다.");
                    }
                }
                else {
                    Debug.LogWarning($"[Inventory] {item.itemName} 아이템이 이미 가득 찼습니다. (최대 {MAX_SLOT_CAPACITY}개)");
                }
            }
            else {
                // 없는 경우: 새로 추가
                Slots.Add(new InventorySlot(item.itemId, amount));
            }
        }

        // 아이템 사용/배치 (하우징에서 가구 배치 시 사용)
        public bool UseItem(int itemId, int amount = 1) {
            var slot = Slots.FirstOrDefault(s => s.itemId == itemId);

            if (slot != null && slot.quantity >= amount) {
                slot.quantity -= amount;

                // 개수가 0이 되면 목록에서 지움
                if (slot.quantity <= 0) {
                    Slots.Remove(slot);
                }
                return true; // 사용 성공
            }
            Debug.LogWarning("[Inventory] 아이템이 부족합니다.");
            return false; // 사용 실패
        }

        // 아이템 보유 개수 확인
        public int GetItemCount(int itemId) {
            return Slots.Where(s => s.itemId == itemId).Sum(s => s.quantity);
        }
        
        // 특정 아이템 보유 여부 확인
        public bool HasItem(int itemId) {
            return GetItemCount(itemId) > 0;
        }
        
        // 인벤토리 비우기 (디버그용 등)
        public void Clear() {
            Slots.Clear();
        }
    }
}