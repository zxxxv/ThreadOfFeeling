using Components;
using Managers;
using PythonManagers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controller {
    public class QuizController : MonoBehaviour {
        [Header("퀴즈 UI")]
        [SerializeField] private GameObject questionPanel;
        [SerializeField] private TextMeshProUGUI questionText;
        
        [Header("선택지 버튼")]
        [SerializeField] private Button answerButton1;
        [SerializeField] private Button answerButton2;
        [SerializeField] private Button answerButton3;
        [SerializeField] private Button answerButton4;

        [Header("버튼 내부 이미지")]
        [SerializeField] private GameObject[] buttonImages; 

        [Header("피드백 UI")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Button feedbackContinueButton;

        private List<Question> currentQuizzes;
        private int currentQuizIndex = 0;
        private bool isAnswer1OnButton1; 
        private bool isWaitingForNext = false;
        private Action onAllQuizzesCompleted;

        private bool IsFourChoiceMode => currentQuizzes != null && currentQuizIndex < currentQuizzes.Count && 
                                         currentQuizzes[currentQuizIndex].targetType == StoryType.TypeB;

        public bool IsActive => questionPanel.activeInHierarchy || feedbackPanel.activeInHierarchy;

        private void Start() {
            if(answerButton1) answerButton1.onClick.AddListener(() => OnAnswerClicked(1));
            if(answerButton2) answerButton2.onClick.AddListener(() => OnAnswerClicked(2));
            if(answerButton3) answerButton3.onClick.AddListener(() => OnAnswerClicked(3));
            if(answerButton4) answerButton4.onClick.AddListener(() => OnAnswerClicked(4));
            questionPanel.SetActive(false);
            feedbackPanel.SetActive(false);
        }

        public void HandleInput() {
            int motionInput = MotionInputManager.Instance.GetMotionInput();

            if (questionPanel.activeInHierarchy) {
                if (IsFourChoiceMode) {
                    if (InputManager.Instance.GetNOneKeyDown() || motionInput == 10) OnAnswerClicked(0);
                    else if (InputManager.Instance.GetNTwoKeyDown() || motionInput == 20) OnAnswerClicked(1);
                    else if (Input.GetKeyDown(KeyCode.Alpha3) || motionInput == 30) OnAnswerClicked(2);
                    else if (Input.GetKeyDown(KeyCode.Alpha4) || motionInput == 40) OnAnswerClicked(3);
                } else {
                    if (InputManager.Instance.GetNOneKeyDown() || motionInput == 10) OnAnswerClicked(0);
                    else if (InputManager.Instance.GetNTwoKeyDown() || motionInput == 20) OnAnswerClicked(1);
                }
            } else if (feedbackPanel.activeInHierarchy) {
                if (InputManager.Instance.GetSpaceKeyDown()) {
                    SoundManager.Instance.SelectSound();
                    if (isWaitingForNext) ShowNextQuiz();
                    else RetryQuiz();
                }
            }
        }

        public void StartQuizSequence(List<Question> quizzes, Action onComplete) {
            currentQuizzes = quizzes;
            onAllQuizzesCompleted = onComplete;
            currentQuizIndex = 0;
            ShowQuiz();
        }

        private void ShowQuiz() {
            if (currentQuizzes == null || currentQuizIndex >= currentQuizzes.Count) {
                EndQuizSequence();
                return;
            }

            Question q = currentQuizzes[currentQuizIndex];

            // [추가] 텍스트가 없는 퀴즈는 데이터 오류이므로 스킵
            if (string.IsNullOrWhiteSpace(q.questionText)) {
                ShowNextQuiz();
                return;
            }

            bool showImages = (q.targetType == StoryType.TypeB);
            if (buttonImages != null) {
                foreach (var img in buttonImages) if (img != null) img.SetActive(showImages);
            }

            if (q.targetType == StoryType.TypeB) {
                MotionInputManager.Instance.SetEmotionMode();
                SetButtonActive(answerButton1, true); SetButtonActive(answerButton2, true);
                SetButtonActive(answerButton3, true); SetButtonActive(answerButton4, true);
                SetButtonText(answerButton1, q.answer1); SetButtonText(answerButton2, q.answer2);
                SetButtonText(answerButton3, q.answer3); SetButtonText(answerButton4, q.answer4);
            } else {
                MotionInputManager.Instance.SetHandMode();
                SetButtonActive(answerButton1, true); SetButtonActive(answerButton2, true);
                SetButtonActive(answerButton3, false); SetButtonActive(answerButton4, false);
                if (UnityEngine.Random.value < 0.5f) {
                    SetButtonText(answerButton1, q.answer1); SetButtonText(answerButton2, q.answer2);
                    isAnswer1OnButton1 = true;
                } else {
                    SetButtonText(answerButton1, q.answer2); SetButtonText(answerButton2, q.answer1);
                    isAnswer1OnButton1 = false;
                }
            }

            questionPanel.SetActive(true);
            feedbackPanel.SetActive(false);
            questionText.text = $"#퀴즈. {q.questionText}";
            SoundManager.Instance.StopTTS();
            if (q.questionVoice != null) SoundManager.Instance.PlayTTS(q.questionVoice);
        }

        private void SetButtonActive(Button btn, bool isActive) { if (btn != null) btn.gameObject.SetActive(isActive); }
        private void SetButtonText(Button btn, string text) { if (btn != null) btn.GetComponentInChildren<TextMeshProUGUI>().text = text; }

        private void OnAnswerClicked(int clickedButtonIndex) {
            if (!questionPanel.activeInHierarchy) return;
            SoundManager.Instance.SelectSound();
            Question q = currentQuizzes[currentQuizIndex];
            SoundManager.Instance.StopTTS();
            questionPanel.SetActive(false);
            feedbackPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            feedbackContinueButton.onClick.RemoveAllListeners();

            bool isCorrect = (q.targetType == StoryType.TypeB) ? (clickedButtonIndex == q.correctAnswerIndex) :
                ((isAnswer1OnButton1 ? clickedButtonIndex : 1 - clickedButtonIndex) == q.correctAnswerIndex);

            if (isCorrect) {
                SoundManager.Instance.RightSound();
                feedbackText.text = q.correctFeedback;
                SetupFeedbackButton("다음", ShowNextQuiz);
                isWaitingForNext = true;
            } else {
                SoundManager.Instance.WrongSound();
                feedbackText.text = q.wrongFeedback;
                SetupFeedbackButton("다시 시도", RetryQuiz);
                isWaitingForNext = false;
            }
        }

        private void SetupFeedbackButton(string text, UnityEngine.Events.UnityAction action) {
            feedbackContinueButton.gameObject.SetActive(true);
            feedbackContinueButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
            feedbackContinueButton.onClick.AddListener(action);
        }

        private void RetryQuiz() { feedbackPanel.SetActive(false); questionPanel.SetActive(true); }

        private void ShowNextQuiz() {
            currentQuizIndex++;
            ShowQuiz();
        }

        private void EndQuizSequence() {
            questionPanel.SetActive(false);
            feedbackPanel.SetActive(false);
            onAllQuizzesCompleted?.Invoke();
        }
    }
}