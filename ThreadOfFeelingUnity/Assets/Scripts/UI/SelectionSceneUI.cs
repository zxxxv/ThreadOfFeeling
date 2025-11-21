using Components;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SelectionSceneUi : SceneUI {
        [Header("Story Details Popup")]
        [Tooltip("상세 정보 팝업의 부모 Panel 오브젝트")]
        [SerializeField] private GameObject storyDetailsPanel;
        [Tooltip("팝업에 표시할 동화 제목 Text")]
        [SerializeField] private TextMeshProUGUI detailTitle;
    
        [Tooltip("팝업에 표시할 동화 커버 Image")]
        [SerializeField] private Image detailCoverImage; 

        [Tooltip("팝업에 표시할 동화 설명 Text")]
        [SerializeField] private TextMeshProUGUI detailDescription;
        [Tooltip("팝업에 표시할 동화 태그 Text")]
        [SerializeField] private TextMeshProUGUI detailTag;
        [Tooltip("팝업의 '시작하기' 버튼")]
        [SerializeField] private Button startButton;

        private Story pendingStory;

        protected override void Start(){
            base.Start();
            storyDetailsPanel.SetActive(false);
        }

        protected override void Update() {
            base.Update();
        }

        public void ShowStoryDetails(Story data) {
            if (data == null) return;
            SoundManager.Instance.SelectSound();
            pendingStory = data;

            if (detailTitle != null)
                detailTitle.text = data.storyTitle;

            if (detailCoverImage != null) {
                detailCoverImage.sprite = data.storyCoverImage;
                detailCoverImage.gameObject.SetActive(data.storyCoverImage != null);
            }

            if (detailDescription != null)
                detailDescription.text = data.storyDescription;
        
            if (detailTag != null)
                detailTag.text = data.storyTag;

            storyDetailsPanel.SetActive(true);
        }

        public void OnClickStartStory() {
            SoundManager.Instance.SelectSound();
            if (pendingStory == null) {
                Debug.LogError("[SelectionSceneUi] 시작할 동화가 선택되지 않았습니다.");
                return;
            }
            DataManager.Instance.SelectFairyTaleData(pendingStory);
            storyDetailsPanel.SetActive(false);
            GameManager.Instance.LoadStoryScene();
        }

        public void OnClickClosePopup() {
            SoundManager.Instance.SelectSound();
            storyDetailsPanel.SetActive(false);
            pendingStory = null;
        }
    }
}