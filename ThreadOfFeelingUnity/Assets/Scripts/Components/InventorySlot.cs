using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components {
    // 인벤토리의 한 칸을 의미하는 클래스
    [System.Serializable]
    public class InventorySlot {
        public int itemId;    // 아이템 고유 ID
        public int quantity;  // 보유 개수

        public InventorySlot(int id, int count) {
            this.itemId = id;
            this.quantity = count;
        }
    }
}