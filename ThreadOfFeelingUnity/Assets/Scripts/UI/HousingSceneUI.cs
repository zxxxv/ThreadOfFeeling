using Components;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class HousingSceneUi : SceneUI 
    {
        [Header("Housing Specific")]
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Button saveLayoutButton;
        [SerializeField] private Button exitButton;
        
        [Header("카메라")]
        [SerializeField] private Camera mainCamera;

        private GameObject currentPicture;
        private Item currentItem;
        private bool isPlacing = false;
        private int placingFrameCount = 0;

        protected override void Start() 
        {
            base.Start();
            
            if (saveLayoutButton != null)
                saveLayoutButton.onClick.AddListener(OnSaveLayout);
            
            if (exitButton != null)
                exitButton.onClick.AddListener(OnClickGoToVillage);

            if (mainCamera == null)
                mainCamera = Camera.main;
            
            if (inventoryPanel != null)
                inventoryPanel.SetActive(true);
            //  itemsContainer 자동 찾기
            if (itemsContainer == null)
            {
                GameObject room = GameObject.Find("Room");
                if (room != null)
                {
                    Transform container = room.transform.Find("ItemsContainer");
                    if (container != null)
                    {
                        itemsContainer = container;
                        Debug.Log($"[HousingSceneUI] ItemsContainer 자동으로 찾음!");
                    }
                    else
                    {
                        Debug.LogError("[HousingSceneUI] Room/ItemsContainer를 찾을 수 없습니다!");
                    }
                }
                else
                {
                    Debug.LogError("[HousingSceneUI] Room을 찾을 수 없습니다!");
                }
            }
            else
            {
                Debug.Log($"[HousingSceneUI] ItemsContainer 설정됨: {itemsContainer.name}");
            }
            
            if (inventoryPanel != null)
                inventoryPanel.SetActive(true);
        }

        protected override void Update() 
        {   
            // InputManager 체크 제거 
            // if (InputManager.Instance == null) 
            //    return;
            
            base.Update();

            // 배치 모드
            if (isPlacing && currentPicture != null && mainCamera != null)
            {
                // 마우스 위치로 이동
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10f;
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
                currentPicture.transform.position = worldPos;
                
                placingFrameCount++;

                // 2프레임 이후 클릭 감지
                if (placingFrameCount >= 2)
                {
                    // UI 위가 아닐 때만
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        // 좌클릭: 배치
                        if (Input.GetMouseButtonDown(0))
                        {
                            PlacePicture();
                        }
                        
                        // 우클릭: 취소
                        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                        {
                            CancelPlacement();
                        }
                    }
                }
            }
        }

        public void StartPlacement(Item item) 
        {
            if (item == null || item.itemPrefab == null)
                return;
            
            if (isPlacing)
                CancelPlacement();

            currentItem = item;
            currentPicture = Instantiate(item.itemPrefab);
            
            // 즉시 마우스 위치로
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            if (mainCamera != null)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10f;
                currentPicture.transform.position = mainCamera.ScreenToWorldPoint(mousePos);
            }
            else
            {
                currentPicture.transform.position = Vector3.zero;
            }
            
            // Sorting Order 높이기
            SpriteRenderer sr = currentPicture.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 100;
            
            // 반투명
            MakeTransparent();
            
            isPlacing = true;
            placingFrameCount = 0;
        }


        // 가구 배치 후  함수 
        private void PlacePicture() {
            if (currentPicture == null || itemsContainer == null)
            {
                CancelPlacement();
                return;
            }
            
            Vector3 worldPosition = currentPicture.transform.position;
            worldPosition.z = 0;
            
            currentPicture.transform.SetParent(itemsContainer, true);
            currentPicture.transform.position = worldPosition;
            
            RestoreOpacity();
            
            SpriteRenderer sr = currentPicture.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10; //  다른 가구들과 같게
            }

            PlacedPicture placed = currentPicture.AddComponent<PlacedPicture>();
            placed.Initialize(currentItem, this);

            currentPicture = null;
            currentItem = null;
            isPlacing = false;
        }

        private void CancelPlacement()
        {
            if (currentPicture != null)
                Destroy(currentPicture);

            if (currentItem != null)
            {
                InventoryUI inventory = FindFirstObjectByType<InventoryUI>();
                if (inventory != null)
                    inventory.ReturnItem(currentItem);
            }

            currentPicture = null;
            currentItem = null;
            isPlacing = false;
        }

        public void StartMovingPicture(PlacedPicture picture)
        {
            if (isPlacing)
                CancelPlacement();

            currentItem = picture.item;
            currentPicture = picture.gameObject;
            Destroy(picture);

            SpriteRenderer sr = currentPicture.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 100;

            MakeTransparent();
            isPlacing = true;
            placingFrameCount = 0;
        }

        public void RemovePicture(PlacedPicture picture)
        {
            InventoryUI inventory = FindFirstObjectByType<InventoryUI>();
            if (inventory != null)
                inventory.ReturnItem(picture.item);
            
            Destroy(picture.gameObject);
        }

        private void MakeTransparent()
        {
            if (currentPicture == null) return;

            SpriteRenderer sr = currentPicture.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0.5f;
                sr.color = color;
            }
        }

        private void RestoreOpacity()
        {
            if (currentPicture == null) return;

            SpriteRenderer sr = currentPicture.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 1f;
                sr.color = color;
            }
        }

        private void OnSaveLayout()
        {
            Debug.Log("[HousingSceneUI] 저장");
        }
    }

    public class PlacedPicture : MonoBehaviour
    {
        public Item item;
        private HousingSceneUi housingUI;

        public void Initialize(Item pictureItem, HousingSceneUi ui)
        {
            item = pictureItem;
            housingUI = ui;

            if (GetComponent<Collider2D>() == null)
                gameObject.AddComponent<BoxCollider2D>();
        }

        private void OnMouseDown()
        {
            if (Input.GetKey(KeyCode.R))
                housingUI.RemovePicture(this);
            else
                housingUI.StartMovingPicture(this);
        }
    }
}
