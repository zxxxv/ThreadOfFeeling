using Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UI
{
    public class ProfileSlot : MonoBehaviour {
        [Header("UI Components")]
        [SerializeField] private Image characterImage;
        [SerializeField] private GameObject plusIcon;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private Button slotButton;

        // 캐릭터 이미지에 붙어있는 애니메이터 컴포넌트
        [SerializeField] private Animator characterAnimator; 

        [Header("Overlay UI")]
        [SerializeField] private GameObject selectedOverlay; 
        [SerializeField] private Button btnGameStart;
        [SerializeField] private Button btnSetting;

        [Header("Resources (Static)")]
        [SerializeField] private Sprite maleSprite;   // 정지 이미지 (애니메이터 없을 때 대비)
        [SerializeField] private Sprite femaleSprite;

        [Header("Resources (Animation)")]
        // 성별에 따른 애니메이터 컨트롤러 (여기에 걷는 모션 등이 들어있어야 함)
        [SerializeField] private RuntimeAnimatorController maleController;
        [SerializeField] private RuntimeAnimatorController femaleController;

        public ChildProfile ProfileData { get; private set; }
        private Action<ProfileSlot> onClickCallback;

        public void Init(Action<ProfileSlot> onClick, Action<ChildProfile> onStart, Action<ChildProfile> onSetting) {
            this.onClickCallback = onClick;

            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => onClickCallback?.Invoke(this));

            btnGameStart.onClick.RemoveAllListeners();
            btnGameStart.onClick.AddListener(() => onStart?.Invoke(ProfileData));

            btnSetting.onClick.RemoveAllListeners();
            btnSetting.onClick.AddListener(() => onSetting?.Invoke(ProfileData));

            selectedOverlay.SetActive(false);
            
            // 초기화 시 애니메이션 멈춤
            if (characterAnimator != null) characterAnimator.enabled = false;
        }

        public void SetData(ChildProfile profile) { // 매개변수 간소화 (인스펙터 변수 사용)
            this.ProfileData = profile;

            if (profile == null) {
                characterImage.gameObject.SetActive(false);
                plusIcon.SetActive(true);
                nicknameText.text = "";
                selectedOverlay.SetActive(false); 
                
                // 데이터 없으면 애니메이터 끄기
                if(characterAnimator != null) characterAnimator.enabled = false;
            }
            else {
                characterImage.gameObject.SetActive(true);
                plusIcon.SetActive(false);
                nicknameText.text = profile.Nickname;

                // 성별에 따라 컨트롤러 교체 및 기본 이미지 설정
                if (profile.Gender == Gender.Male) {
                    characterImage.sprite = maleSprite;
                    if(characterAnimator != null) characterAnimator.runtimeAnimatorController = maleController;
                }
                else {
                    characterImage.sprite = femaleSprite;
                    if(characterAnimator != null) characterAnimator.runtimeAnimatorController = femaleController;
                }

                // 평소엔 애니메이션 끄거나 Idle 상태로 둠
                if(characterAnimator != null) {
                    characterAnimator.enabled = true;
                    characterAnimator.SetBool("isSelected", false); 
                }
            }
        }

        public void SetSelected(bool isSelected) {
            if (ProfileData == null) return; 
            selectedOverlay.SetActive(isSelected);

            // 선택되었을 때만 움직이게 설정
            if (characterAnimator != null) {
                // Animator 파라미터로 제어 ("isSelected"라는 Bool 파라미터)
                characterAnimator.SetBool("isSelected", isSelected);
            }
        }
    }
}