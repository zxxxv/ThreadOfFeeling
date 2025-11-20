using UnityEngine;

namespace Components
{
    public class PlaceableItem : MonoBehaviour
    {
        [Header("Item Data")]
        public Item itemData;
        
        [Header("Placement Settings")]
        [SerializeField] private PolygonCollider2D floorBounds; // 바닥 영역
        
        private bool isDragging = false;
        private Vector3 offset;
        private Vector3 lastValidPosition; // 마지막 유효한 위치
        private Camera mainCamera;
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            mainCamera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
            // 시작 위치를 유효한 위치로 저장
            lastValidPosition = transform.position;
            
            // HousingSceneUI에서 floorBounds 자동 설정
            if (floorBounds == null)
            {
                floorBounds = GameObject.Find("FloorBounds")?.GetComponent<PolygonCollider2D>();
                
                if (floorBounds == null)
                {
                    Debug.LogWarning("[PlaceableItem] FloorBounds를 찾을 수 없습니다!");
                }
            }
        }

        void OnMouseDown()
        {
            isDragging = true;
            Vector3 mousePos = GetMouseWorldPos();
            offset = transform.position - mousePos;
        }

        void OnMouseDrag()
        {
            if (isDragging)
            {
                Vector3 mousePos = GetMouseWorldPos();
                Vector3 newPosition = mousePos + offset;
                transform.position = newPosition;
            }
        }

        void OnMouseUp()
        {
            isDragging = false;
        }

        Vector3 GetMouseWorldPos()
        {
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = 10f;
            return mainCamera.ScreenToWorldPoint(screenPos);
        }

        void Update()
        {
            if (spriteRenderer != null)
            {   
                // Y 좌표 기반 깊이감 (1000 기준, Y가 낮을수록 앞)
                int baseOrder = 1000;
                spriteRenderer.sortingOrder = baseOrder + Mathf.RoundToInt(-transform.position.y * 10);
            }
        }
    }
}