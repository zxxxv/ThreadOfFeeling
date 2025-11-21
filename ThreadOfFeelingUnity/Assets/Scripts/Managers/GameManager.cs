using System.Collections.Generic;
using Components;
using Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        // 게임 상태 관리
        public GameState CurrentState { get; private set; }
        private Stack<GameState> stateStack = new Stack<GameState>();        

        // 플레이어 관련
        [SerializeField] private GameObject maleCharacterPrefab;    // 남성 캐릭터 프리팹
        [SerializeField] private GameObject femaleCharacterPrefab;  // 여성 캐릭터 프리팹
        [SerializeField] private Vector3 spawnPosition = new Vector3(0, 0, 0); // 시작 스폰 지점
        private GameObject playerInstance;        // 생성된 플레이어 캐릭터의 인스턴스
        private PlayerController playerController; // 플레이어 컨트롤러

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        private void Start() {
            // 씬이 로드될 때마다 호출될 함수를 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
            // 현재 로드된 씬에서도 즉시 1회 실행
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        // 씬이 로드될 때마다 호출되는 함수
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            
            // BGM 변경 요청
            SoundManager.Instance.ChangeBgm(scene.name);

            // 씬에 따른 상태 및 플레이어 설정
            switch (scene.name)
            {
                case "VillageScene":
                    StartVillage(); // 플레이어 생성
                    SetState(GameState.Village);
                    break;

                case "StartScene":
                    ResetPlayerRef();
                    SetState(GameState.Start);
                    break;

                case "SelectionScene":
                    ResetPlayerRef();
                    SetState(GameState.Selection);
                    break;

                case "StoryScene":
                    ResetPlayerRef();
                    SetState(GameState.Story);
                    break;

                case "HousingScene":
                    ResetPlayerRef();
                    SetState(GameState.Housing);
                    break;

                case "ProfileScene":
                    ResetPlayerRef();
                    SetState(GameState.Profile);
                    break;

                default:
                    Debug.Log($"[GameManager] 정의되지 않은 씬: {scene.name}");
                    ResetPlayerRef();
                    SetState(GameState.Loading);
                    break;
            }

            // 스택 초기화
            stateStack.Clear();
        }
        
        // 씬이 바뀔 때 플레이어 참조 초기화 (Village가 아니면 플레이어 없음)
        private void ResetPlayerRef() {
            playerInstance = null;
            playerController = null;
        }

        // 마을 씬 시작 시 호출
        private void StartVillage() {
            if (playerInstance != null) return; 
            ChildProfile userProfile = DataManager.Instance.currentProfile;

            if (userProfile == null) {
                Debug.LogError("[GameManager] 선택된 프로필이 없습니다");
                return;
            }

            GameObject prefabToCreate = (userProfile.Gender == Gender.Male) ? maleCharacterPrefab : femaleCharacterPrefab;

            if (prefabToCreate != null) {
                playerInstance = Instantiate(prefabToCreate, spawnPosition, Quaternion.identity);
                playerInstance.tag = "Player";
                playerController = playerInstance.GetComponent<PlayerController>();
            }
            else {
                Debug.LogError("[GameManager] 성별에 맞는 캐릭터 프리팹이 GameManager에 연결되지 않았습니다");
            }
        }

        // 게임 상태 제어
        private void SetState(GameState newState) {
            if (CurrentState == newState) return;
            CurrentState = newState;
            Debug.Log("[GameManager] 게임 상태 변경: " + CurrentState);

            if (playerController == null && playerInstance != null) {
                playerController = playerInstance.GetComponent<PlayerController>();
            }

            if (playerController != null) {
                switch (CurrentState) {
                    case GameState.Village:
                        playerController.ResumeMovement();
                        break;
                    case GameState.Story:
                    case GameState.Housing:
                    case GameState.Selection:
                    case GameState.Paused:
                    case GameState.NPCTalk:
                    case GameState.Profile:
                    case GameState.Loading:
                    case GameState.Start:
                        playerController.StopMovement();
                        break;
                }
            }
            else {
                if (CurrentState == GameState.Village) {
                    Debug.LogWarning("[GameManager] " + CurrentState + " 상태로 변경하려 하나, PlayerController가 없습니다. (씬에 플레이어 없음)");
                }
            }
        
        }

        // NPC 대화 상태로 변경
        public void EnterTalkState() {
            // 이미 대화 중이라면 중복 실행 방지
            if (CurrentState == GameState.NPCTalk) return;
            // 현재 상태를 스택에 저장
            stateStack.Push(CurrentState);
            SetState(GameState.NPCTalk);
        }

        // NPC 대화 상태 끝
        public void ExitTalkState() {
            // 대화 상태가 아니면 실행하지 않음
            if (CurrentState != GameState.NPCTalk) return;
            // 스택에서 이전 상태를 꺼내서 복구
            if (stateStack.Count > 0) {
                SetState(stateStack.Pop());
            }
            else {
                // 만약 스택이 비어있다면 현재 씬의 기본 상태로 복구
                RecoverStateByScene();
            }
        }

        // 게임 일시정지 [Esc]
        public void PauseGame() {
            if (CurrentState == GameState.Paused) return;
            stateStack.Push(CurrentState);
            SetState(GameState.Paused);
        }

        // 게임 재개 [Esc]
        public void ResumeGame() {
            if (CurrentState != GameState.Paused) return;
            if (stateStack.Count > 0) {
                SetState(stateStack.Pop());
            }
            else {
                Debug.LogWarning("[GameManager] 스택에 상태가 비어있습니다");
                RecoverStateByScene(); 
            }
        }

        // 스택이 비었을때 현재 씬에 맞는 기본 상태로 복구
        private void RecoverStateByScene() {
            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName) {
                case "VillageScene": SetState(GameState.Village); break;
                case "HousingScene": SetState(GameState.Housing); break;
                case "StoryScene": SetState(GameState.Story); break;
                case "StartScene": SetState(GameState.Story); break;
                case "SelectionScene": SetState(GameState.Selection); break;
                case "ProfileScene": SetState(GameState.Profile); break;
                default: SetState(GameState.Loading); break;
            }
        }

        // 씬 로드 시 등록된 함수를 해제
        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // -- 씬 이동 함수들 --
        public void LoadStoryScene() {
            SceneManager.LoadScene("StoryScene");
        }
        public void LoadHousingScene() {
            SceneManager.LoadScene("HousingScene");
        }
        public void LoadVillageScene() {
            SceneManager.LoadScene("VillageScene");
        }
        public void LoadSelectionScene() {
            SceneManager.LoadScene("SelectionScene");
        }
        public void LoadProfileScene() {
            SceneManager.LoadScene("ProfileScene");
        }
        public void LoadStartScene() {
            SceneManager.LoadScene("StartScene");
        }
    }
}