using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SceneUI : MonoBehaviour {

        [Header("Menu UI (From SceneUI)")]
        [Tooltip("모든 씬에서 공통으로 사용할 메뉴 UI 게임 오브젝트")]
        [SerializeField] private GameObject menuSet;
        [Tooltip("메뉴 창에 표시될 유저 닉네임 Text")]
        [SerializeField] private TextMeshProUGUI menuUserName;
        
        protected virtual void Start() {
            Init();
            FindUserName();
        }
        
        protected virtual void Init()
        {
            // 초기화
        }

        protected virtual void Update() {
            if (InputManager.Instance == null) {
                Debug.LogWarning("InputManager가 없습니다!");
                return;
            }
            if (InputManager.Instance.GetEscapeKeyDown()) {
                if (menuSet != null) {
                    MenuSet(!menuSet.activeSelf);
                }
                else {
                Debug.LogWarning("menuSet이 연결되지 않았습니다!");
                }
            }
        }

        public void FindUserName() {
            if (menuUserName == null) {
                Debug.LogWarning("[SceneUI] menuUserName이 연결되지 않았습니다!");
                return;
            }
            
            if (DataManager.Instance == null) {
                Debug.LogWarning("[SceneUI] DataManager가 없습니다!");
                return;
            }
            
            if (DataManager.Instance.currentProfile == null) {
                Debug.LogWarning("[SceneUI] currentProfile이 없습니다!");
                return;
            }
            
            // 모두 안전하면 설정
            menuUserName.text = DataManager.Instance.currentProfile.nickname;
            Debug.Log($"[SceneUI] 사용자 이름 표시: {menuUserName.text}");
        }

        private void MenuSet(bool isActive) {
            if (menuSet == null) return;
            menuSet.SetActive(isActive);
            if (isActive) GameManager.Instance.PauseGame();
            else GameManager.Instance.ResumeGame();
        }

        // 게임 저장 - 메뉴
        public void OnClickSave() {
            // 플레이어 위치
            // 클리어 한 동화
            // 인벤토리
            // roomLayout
            // DataManager 호출해서 저장하기
            DataManager.Instance.SaveProfileData();
            MenuSet(false);
        }

        // 게임 끝내기 - build시에만 작동
        public void OnClickExit() {
            Application.Quit();
        }

        // 이어하기 - 메뉴
        public void OnClickResume() {
            MenuSet(false);
        }

        protected virtual void OnclickGoToBack() { }

        public void OnClickGoToStory() {
            GameManager.Instance.LoadStoryScene();
        }

        public void OnClickGoToHousing() {
            GameManager.Instance.LoadHousingScene();
        }

        public void OnClickGoToVillage() {
            GameManager.Instance.LoadVillageScene();
        }

        public void OnClickGoToSelection() {
            GameManager.Instance.LoadSelectionScene();
        }
    }
}
