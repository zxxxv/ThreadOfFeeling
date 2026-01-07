using System.Collections;
using UnityEngine;
using Components;

namespace PythonManagers {
    public class MotionInputManager : MonoBehaviour {
        
        // [안전한 싱글톤 패턴]
        private static MotionInputManager _instance;
        public static MotionInputManager Instance {
            get {
                if (_instance != null) return _instance;

                _instance = FindAnyObjectByType<MotionInputManager>();

                if (_instance == null) {
                    GameObject container = new GameObject("MotionInputManager");
                    _instance = container.AddComponent<MotionInputManager>();
                    Debug.Log("[MotionInputManager] Auto-created Singleton Instance.");
                }

                return _instance;
            }
        }

        public MotionInputType inputMode = MotionInputType.Hand;

        private void Awake() {
            // 중복 방지 로직 (씬 내에 2개가 생기는 것 방지)
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            
            // [삭제됨] DontDestroyOnLoad(gameObject);
            // 씬 이동 시 파괴되어야 카메라 리소스를 반환할 수 있습니다.
        }

        private void OnDestroy() {
            // 싱글톤 참조 정리
            if (_instance == this) {
                _instance = null;
            }

            // [중요] 씬이 끝나거나 객체가 파괴될 때 모든 파이썬 프로세스 종료 (카메라 Off)
            StopAllProcesses();
        }

        public int GetMotionInput() {
            switch (inputMode) {
                case MotionInputType.Emotion:
                    return GetEmotionInput();
                case MotionInputType.Hand:
                    return GetHandInput();
                default:
                    return 0;
            }
        }

        private int GetEmotionInput() {
            if (EmotionManager.Instance == null) return 0;
            return EmotionManager.Instance.GetEmotion();
        }

        private int GetHandInput() {
            if (SelectHandsManager.Instance == null) return 0;
            return SelectHandsManager.Instance.GetHandCode();
        }

        // --- 모드 전환 로직 ---

        public void SetEmotionMode() {
            StartCoroutine(SwitchToEmotionRoutine());
        }

        public void SetHandMode() {
            StartCoroutine(SwitchToHandRoutine());
        }

        private IEnumerator SwitchToEmotionRoutine() {
            Debug.Log(">>> [MotionInputManager] Switching to EMOTION Mode...");
            inputMode = MotionInputType.Emotion;

            // 1. 기존 프로세스 종료 (손)
            if (SelectHandsManager.Instance != null) {
                SelectHandsManager.Instance.StopPythonProcess();
            }
            
            // 프로세스 정리 대기
            yield return new WaitForSeconds(0.5f);

            // 2. 새 프로세스 시작 (감정)
            if (EmotionManager.Instance != null) {
                Debug.Log(">>> [MotionInputManager] Starting Emotion Process...");
                EmotionManager.Instance.StartPythonProcess();
            } else {
                Debug.LogWarning(">>> [Warning] EmotionManager Instance is NULL or not ready.");
            }
        }

        private IEnumerator SwitchToHandRoutine() {
            Debug.Log(">>> [MotionInputManager] Switching to HAND Mode...");
            inputMode = MotionInputType.Hand;

            // 1. 기존 프로세스 종료 (감정)
            if (EmotionManager.Instance != null) {
                EmotionManager.Instance.StopPythonProcess();
            }

            // 프로세스 정리 대기
            yield return new WaitForSeconds(0.5f);

            // 2. 새 프로세스 시작 (손)
            if (SelectHandsManager.Instance != null) {
                Debug.Log(">>> [MotionInputManager] Starting Hand Process...");
                SelectHandsManager.Instance.StartPythonProcess();
            } else {
                Debug.LogWarning(">>> [Warning] SelectHandsManager Instance is NULL or not ready.");
            }
        }

        // 모든 프로세스 강제 종료 함수
        public void StopAllProcesses() {
            // Debug.Log(">>> Stopping ALL Python Processes...");
            StopAllCoroutines(); // 진행 중인 전환 코루틴 중단

            if (EmotionManager.Instance != null) {
                EmotionManager.Instance.StopPythonProcess();
            }

            if (SelectHandsManager.Instance != null) {
                SelectHandsManager.Instance.StopPythonProcess();
            }
        }
    }
}