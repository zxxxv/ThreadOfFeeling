using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Components;

namespace UI
{
    public class SceneUI : MonoBehaviour {

        [Header("Menu UI (From SceneUI)")]
        [SerializeField] private GameObject menuSet;

        [Header("Profile Info UI")]
        [SerializeField] private TextMeshProUGUI menuUserName;
        [SerializeField] private TextMeshProUGUI profileAgeText;
        [SerializeField] private TextMeshProUGUI profileGenderText;

        [Tooltip("7각형 능력치 그래프")]
        [SerializeField] private RadarChart statChart;
        
        [Header("Settings UI")]
        [Tooltip("TTS 사용 여부를 설정할 토글 버튼")]
        [SerializeField] private Toggle ttsToggle;
        
        protected virtual void Start() {
            UpdateProfileUI();
            if (menuSet != null) {
                menuSet.SetActive(false);
            }
            else {
                Debug.LogError("[SceneUI] 'MenuSet' 게임 오브젝트가 인스펙터에 연결되지 않았습니다");
            }
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

        private void UpdateProfileUI() {
            if (DataManager.Instance == null || DataManager.Instance.currentProfile == null) {
                return;
            }

            var profile = DataManager.Instance.currentProfile;

            // 닉네임
            if (menuUserName != null) menuUserName.text = profile.Nickname;
            
            // 나이
            if (profileAgeText != null) 
                profileAgeText.text = profile.AgeBandText; 
            
            // 성별
            if (profileGenderText != null) 
                profileGenderText.text = profile.GenderText;

            // TTS 설정 상태
            if (ttsToggle != null) ttsToggle.SetIsOnWithoutNotify(profile.IsTtsUsed);

            if (statChart != null && profile.Stats != null) {
                // 최대 능력치 값을 100으로 가정하고 정규화해서 전달
                statChart.SetStats(profile.Stats.GetNormalizedValues(100f));
            }
        }

        // TTS 토글 값이 변경될 때 호출됨
        public void OnTtsToggleChanged(bool isOn) {
            if (DataManager.Instance != null && DataManager.Instance.currentProfile != null) {
                SoundManager.Instance.SelectSound();
                // 데이터 저장
                DataManager.Instance.currentProfile.IsTtsUsed = isOn;
                Debug.Log($"[SceneUI] TTS 설정 변경됨: {isOn}");
                
                // 꼬임 방지 로직
                if (isOn == false) {
                    // TTS를 껐다면, 현재 나오고 있는 목소리를 즉시 끄기
                    SoundManager.Instance.StopTTS(); 
                }
                else {
                    // TTS를 켰다면 다음 문장부터 읽음
                }
            }
        }

        // 메뉴창 활성화 or 비활성화
        private void MenuSet(bool isActive) {
            if (menuSet == null) return;
            menuSet.SetActive(isActive);
            if (isActive) {
                GameManager.Instance.PauseGame();
                SoundManager.Instance.SelectSound(true);
                // 메뉴 열 때 프로필 정보 최신화
                UpdateProfileUI();
            }
            else {
                GameManager.Instance.ResumeGame();
                SoundManager.Instance.SelectSound(false);
            }
        }

        // 게임 저장 - 메뉴
        public void OnClickSave() {
            DataManager.Instance.SaveProfileData();
            Debug.Log("[SceneUI] 게임 저장 완료");
            MenuSet(false);
        }

        // 게임 끝내기 - build시에만 작동
        public void OnClickExit() {
            SoundManager.Instance.SelectSound();
            Application.Quit();
        }

        // 이어하기 - 메뉴
        public void OnClickResume() {
            MenuSet(false);
        }

        // 씬 이동 함수 (버튼과 연결)
        public void OnClickGoToStory() {
            SoundManager.Instance.SelectSound();
            GameManager.Instance.LoadStoryScene();
        }
        public void OnClickGoToHousing() {
            SoundManager.Instance.SelectSound();
            GameManager.Instance.LoadHousingScene();
        }
        public void OnClickGoToVillage() {
            SoundManager.Instance.SelectSound();
            GameManager.Instance.LoadVillageScene();
        }
        public void OnClickGoToSelection() {
            SoundManager.Instance.SelectSound();
            GameManager.Instance.LoadSelectionScene();
        }
        public void OnClickGoToProfile() {
            SoundManager.Instance.SelectSound();
            GameManager.Instance.LoadProfileScene();
        }
        public void OnClickGoToStart() {
            SoundManager.Instance.SelectSound();
            GameManager.Instance.LoadStartScene();
        }
    }
}
