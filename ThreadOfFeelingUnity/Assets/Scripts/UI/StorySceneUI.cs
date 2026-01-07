using Components;
using Controller;
using Managers;
using PythonManagers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StorySceneUi : SceneUI {
        [Header("스토리 UI")]
        [SerializeField] private Image storyDisplayImage;
        [SerializeField] private GameObject dialoguePanel; 
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Header("컨트롤러 연결")]
        [SerializeField] private QuizController quizController;
        [SerializeField] private RewardUI rewardPopup;

        private Story currentTale;
        private StoryType currentType; 
        private int currentScenarioIndex = 0;
        private Scenario currentScenario; 
        private bool IsQuizMode => quizController != null && quizController.IsActive;

        protected override void Start() {
            base.Start();
            if (quizController == null) quizController = GetComponentInChildren<QuizController>();
            if (rewardPopup == null) rewardPopup = GetComponentInChildren<RewardUI>();
            if (rewardPopup != null) rewardPopup.Init();

            currentTale = DataManager.Instance.selectedTale;
            currentType = DataManager.Instance.selectedStoryType; 

            if (MotionInputManager.Instance != null) {
                if (currentType == StoryType.TypeB) MotionInputManager.Instance.SetEmotionMode();
                else MotionInputManager.Instance.SetHandMode();
            }

            if (currentTale == null || currentTale.scenarios.Count == 0) {
                GameManager.Instance.LoadSelectionScene();
                return;
            }
            currentScenarioIndex = 0;
            ShowCurrentScenario();
        }

        protected override void Update() {
            base.Update();
            if (IsQuizMode) {
                quizController.HandleInput();
                return;
            }
            if (InputManager.Instance.GetSpaceKeyDown()) {
                SoundManager.Instance.SelectSound();
                CheckAndStartQuizOrNext();
            }
        }

        private void CheckAndStartQuizOrNext() {
            List<Question> validQuestions = new List<Question>();

            if (currentScenario.quizzes != null) {
                // [수정] 타입이 맞으면서 + 퀴즈 텍스트가 실제 존재하는 것만 필터링
                validQuestions = currentScenario.quizzes
                    .Where(q => (q.isCommon || q.targetType == currentType) && !string.IsNullOrWhiteSpace(q.questionText))
                    .ToList();
            }
            
            if (validQuestions.Count > 0) StartQuizMode(validQuestions);
            else ShowNextScenario();
        }

        private void StartQuizMode(List<Question> questions) {
            quizController.StartQuizSequence(questions, OnQuizSequenceFinished);
        }

        private void OnQuizSequenceFinished() => ShowNextScenario();

        public void ShowCurrentScenario() {
            if (dialoguePanel != null) dialoguePanel.SetActive(true);
            currentScenario = currentTale.scenarios[currentScenarioIndex];
            if (storyDisplayImage != null) storyDisplayImage.sprite = currentScenario.image;
            if (dialogueText != null) dialogueText.text = currentScenario.dialogueText;

            SoundManager.Instance.StopTTS();
            if (currentScenario.ttsClip != null) SoundManager.Instance.PlayTTS(currentScenario.ttsClip);
        }

        public void ShowNextScenario() {
            currentScenarioIndex++;
            if (currentScenarioIndex < currentTale.scenarios.Count) ShowCurrentScenario();
            else HandleStoryEnd();
        }

        private void HandleStoryEnd() {
            SoundManager.Instance.StopTTS();
            DataManager.Instance.AddClearedStory(currentTale.storyId, currentType);

            System.Action onSceneExit = () => {
                if (MotionInputManager.Instance != null) MotionInputManager.Instance.StopAllProcesses();
                OnClickGoToSelection(); 
            };

            if (currentTale.storyReward != null && rewardPopup != null) {
                if (dialoguePanel != null) dialoguePanel.SetActive(false);
                rewardPopup.Show(currentTale.storyReward, onSceneExit);
            } else onSceneExit();
        }
    }
}