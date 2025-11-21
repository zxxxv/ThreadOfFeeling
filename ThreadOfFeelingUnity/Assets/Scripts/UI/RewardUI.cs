using System;
using Components;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardUI : MonoBehaviour {

        [Header("보상 UI 컴포넌트")]
        [Tooltip("보상 정보를 보여줄 팝업 패널")]
        [SerializeField] private GameObject rewardPanel;
        [Tooltip("보상 아이템 아이콘을 표시할 Image")]
        [SerializeField] private Image rewardImage;
        [Tooltip("보상 아이템 이름을 표시할 Text")]
        [SerializeField] private TextMeshProUGUI rewardNameText;

        // 팝업이 닫힐 때 실행할 콜백
        private Action onCloseCallback;
        private bool isActive = false;

        // 팝업이 열린 시간을 저장할 변수
        private float openTime;
        // 팝업이 열리고 입력 입력을 막을 최소 시간 (0.5초)
        private const float INPUT_DELAY = 0.5f;

        // 초기화
        public void Init() {
            if (rewardPanel != null) rewardPanel.SetActive(false);
            isActive = false;
        }

        // 보상 팝업 띄우기
        public void Show(Item rewardItem, Action onClose) {
            SoundManager.Instance.ClearSound();
            onCloseCallback = onClose;

            // 보상 지급 로직 실행
            GiveReward(rewardItem);

            // UI 표시
            if (rewardPanel != null) {
                rewardPanel.SetActive(true);
                isActive = true;
                openTime = Time.unscaledTime;

                // UI 업데이트
                if (rewardItem != null) {
                    if (rewardImage != null) rewardImage.sprite = rewardItem.itemIcon;
                    if (rewardNameText != null) rewardNameText.text = rewardItem.itemName;
                }
            }
        }

        private void GiveReward(Item item) {
            if (item != null) {
                Debug.Log($"[RewardPopupUI] 보상 획득: {item.itemName}");
                
                // 인벤토리 추가
                DataManager.Instance.AddRewardItem(item);
            }
        }

        private void Update() {
            // 패널이 켜져 있을 때만 입력 감지
            if (!isActive) return;

            if (Time.unscaledTime - openTime < INPUT_DELAY) return;

            if (InputManager.Instance.GetSpaceKeyDown()) {
                Close();
            }
        }

        // 팝업 닫기 및 콜백 실행
        private void Close() {
            SoundManager.Instance.SelectSound();
            isActive = false;
            if (rewardPanel != null) rewardPanel.SetActive(false);
            
            // 등록된 콜백(씬 이동) 실행
            onCloseCallback?.Invoke();
        }
    }
}