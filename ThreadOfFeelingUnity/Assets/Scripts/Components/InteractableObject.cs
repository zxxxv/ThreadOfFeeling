using Managers;
using UI;
using UnityEngine;

namespace Components
{
    public class InteractableObject : MonoBehaviour {
        [Header("Object Info")]
        [Tooltip("DataManager에서 데이터를 찾기 위한 고유 ID")]
        public int id;
        [Tooltip("NPC일 경우 체크 (초상화 표시)")]
        public bool isNPC;

        [Header("State")]
        [Tooltip("현재 상호작용(대화) 중인지 여부")]
        public bool isAction = false;
        [Tooltip("현재 대화의 인덱스")]
        public int talkIndex = 0;

        private VillageSceneUI uiManager; 
        private static VillageSceneUI _cachedUIManager;
        //private GameState previousState;

        void Start() {
            if (_cachedUIManager == null) {
                _cachedUIManager = GameObject.FindFirstObjectByType<VillageSceneUI>();
                if (_cachedUIManager == null) {
                    Debug.LogError($"[InteractableObject: {id}] 씬에서 MainSceneUI를 찾을 수 없습니다! MainSceneUI 스크립트가 씬의 Canvas 등에 붙어있는지 확인하세요.");
                    return;
                }
            }
            uiManager = _cachedUIManager;
        }

        public void HandleInteraction() {
            if (!isAction) {
                GameManager.Instance.EnterTalkState();
                isAction = true;
                ProceedTalk();
            }
            else {
                ProceedTalk();
            }
        }

        private void ProceedTalk() {
            if (uiManager == null) {
                Debug.LogError($"[InteractableObject: {id}] uiManager가 할당되지 않았습니다. Start()에서 MainSceneUI를 찾지 못했습니다.");
                EndInteraction();
                return;
            }

            // 1. DataManager로부터 대화 데이터를 가져옴
            string talkData = DataManager.Instance.GetTalkData(id, talkIndex);

            // 2. 대화 데이터가 null이면 (대화 끝)
            if (talkData == null) {
                EndInteraction();
                return;
            }
        
            talkIndex++;

            string[] parts = talkData.Split(':');
            string textToShow = parts[0];
            Sprite portraitToShow = null;
            bool showChoices = false;

            // NPC 전용 로직
            if (isNPC) {
                // parts[1] 표정
                if (parts.Length > 1 && int.TryParse(parts[1], out int portraitIndex)) {
                    portraitToShow = DataManager.Instance.GetPortrait(id, portraitIndex);
                }
            
                // parts[2] 선택지 - 동화, 하우징
                if (parts.Length > 2 && parts[2] == "CHOICE") {
                    showChoices = true;
                }
            }
            uiManager.ShowTalkPanel(textToShow, portraitToShow, isNPC);
            uiManager.ShowChoiceButtons(showChoices);
        }

        private void EndInteraction() {
            isAction = false;
            talkIndex = 0;
            if (uiManager != null) {
                uiManager.HideTalkPanel();
                uiManager.ShowChoiceButtons(false);
            }
            GameManager.Instance.ExitTalkState();
        }
    }
}