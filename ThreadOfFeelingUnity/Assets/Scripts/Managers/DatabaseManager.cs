using System.IO;
using Data;
using SQLite;
using UnityEngine;
using System.Linq; // 데이터 쉽게 검색/필터링 해주는 기능 

namespace Managers
{
    /// <summary>
    /// 데이터베이스 연결, 테이블 생성, 모든 데이터 입출력(CRUD)을 총괄하는 관리자입니다.
    /// </summary>
    public class DatabaseManager : MonoBehaviour
    {

        private SQLiteConnection _connection;
        public static DatabaseManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // app.db 저장 경로 
            // Assets/는 '읽기 전용'폴더. - 여기에 저장하면 게임 빌드시 데이터 수정/저장이 불가능 
            // 그래서 Application.persistentDataPath는 Unity가 제공하는 안전한 읽기/쓰기 전용 폴더 경로 
            string dbPath = Path.Combine(Application.persistentDataPath, "app.db");
            _connection = new SQLiteConnection(dbPath);

            // Data 폴더의 10개 테이블 생성
            _connection.CreateTable<ChildProfileDB>();
            _connection.CreateTable<GuardianDB>();
            _connection.CreateTable<ScenarioDB>();
            _connection.CreateTable<StoryDB>();
            _connection.CreateTable<QuestionDB>();
            _connection.CreateTable<AttemptDB>();
            _connection.CreateTable<RewardItemDB>();
            _connection.CreateTable<InventoryDB>();
            _connection.CreateTable<RoomLayoutDB>();
            _connection.CreateTable<WeeklyReportDB>();

            Debug.Log($"[DatabaseManager] DB 연결 성공 및 테이블 10개 준비 완료. 경로: {dbPath}");
        }

        /// <summary>
        /// 아래에 데이터 사용 함수 구현 
        /// </summary>

        #region Attempt (학습 로그) 함수
        #endregion

        #region ChildProfile & Guardian 함수
        #endregion

        #region Scenario & Stroy & Question 함수 
        #endregion

        #region Inventory & RewardItem 함수 
        #endregion

        #region RoomLayout 함수 


        /// 특정 프로필의 방 레이아웃 저장 
        public void SaveRoomLayout(int profileId, int roomId, string slotsJson) {
        // 기존 레이아웃이 있는지 확인
        var existing = _connection.Table<RoomLayoutDB>()
        .Where(r => r.profile_id == profileId && r.room_id == roomId)
        .FirstOrDefault();
    
        if (existing != null) {
        // 업데이트
        existing.slots_jason = slotsJson;
        existing.save_time = System.DateTime.Now;
        _connection.Update(existing);
        Debug.Log($"[DatabaseManager] 방 레이아웃 업데이트: roomId={roomId}");
        }
        else {
        // 새로 생성
        var newLayout = new RoomLayoutDB
        {
            profile_id = profileId,
            room_id = roomId,
            slots_jason = slotsJson,
            save_time = System.DateTime.Now
        };
        _connection.Insert(newLayout);
        Debug.Log($"[DatabaseManager] 방 레이아웃 저장: roomId={roomId}");
        }
}
        /// 특정 프로필의 방 레이아웃 로드 
        public RoomLayoutDB LoadRoomLayout(int profileId, int roomId) {
            var layout = _connection.Table<RoomLayoutDB>()
            .Where(r => r.profile_id == profileId && r.room_id == roomId)
            .FirstOrDefault();
        
            if (layout != null) {   
            Debug.Log($"[DatabaseManager] 방 레이아웃 로드 성공: roomId={roomId}"); } 
            else {
            Debug.Log($"[DatabaseManager] 저장된 레이아웃 없음: roomId={roomId}"); }
            return layout; 
        }
        
        #endregion


        #region WeeklyReport 함수 
        #endregion
    }
}
