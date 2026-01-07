using UnityEngine;
using PythonManagers; // MotionInputManager 접근용
using Components;     // MotionInputType 사용

namespace Managers
{
    public class InputManager : MonoBehaviour {
        public static InputManager Instance { get; private set; }
        private Vector3 _moveInput;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            Vector3 rawInput = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) rawInput.x = -1;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) rawInput.x = 1;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) rawInput.y = 1;
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) rawInput.y = -1;
            _moveInput = rawInput.normalized;
        }

        public Vector3 GetMoveInput() {
            return _moveInput;
        }

        public bool GetSpaceKeyDown() {
            return Input.GetButtonDown("Jump");
        }

        public bool GetNOneKeyDown() {
            return Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);
        }

        public bool GetNTwoKeyDown() {
            return Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
        }

        public bool GetEscapeKeyDown() {
            return Input.GetButtonDown("Cancel");
        }

        // --- 수정된 부분 시작 ---

        // 감정 모드 설정
        public MotionInputType SetEmotionMode() {
            // 1. MotionInputManager가 있으면 설정 메서드 호출 (반환값 없음)
            if (MotionInputManager.Instance != null)
            {
                MotionInputManager.Instance.SetEmotionMode();
            }
            
            // 2. 호출한 쪽(QuizController 등)에 현재 모드가 무엇인지 알려주기 위해 명시적으로 반환
            return MotionInputType.Emotion;
        }
        
        // 핸드 모드 설정
        public MotionInputType SetHandMode() {
            if (MotionInputManager.Instance != null)
            {
                MotionInputManager.Instance.SetHandMode();
            }

            return MotionInputType.Hand;
        }

        public int GetMotionInput() {
            // MotionInputManager가 있으면 값 가져오고, 없으면 0
            return MotionInputManager.Instance != null
                ? MotionInputManager.Instance.GetMotionInput()
                : 0;
        }
        // --- 수정된 부분 끝 ---
    }
}