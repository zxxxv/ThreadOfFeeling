using System.Collections.Generic;
using System.IO;
using Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    [System.Serializable]
    public class ProfileWrapper
    {
        public List<ChildProfile> items;
    }

    public class DataManager : MonoBehaviour {
        public static DataManager Instance { get; private set; }

        public List<ChildProfile> profileList = new List<ChildProfile>();
        public ChildProfile currentProfile { get; private set; }
        public Story selectedTale { get; private set; }

        private string ProfileListPath => Path.Combine(Application.persistentDataPath, "profiles.json");

        Dictionary<int, string[]> talkData;
        Dictionary<int, Sprite> portraitData;
        public Sprite[] portraitArr; 

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAllProfiles(); 
                LoadVillageData();
            }
            else {
                Destroy(gameObject);
            }
        }

        // ==========================================
        //              프로필 목록 관리
        // ==========================================

        public void LoadAllProfiles() {
            if (File.Exists(ProfileListPath)) {
                try {
                    string json = File.ReadAllText(ProfileListPath);
                    ProfileWrapper wrapper = JsonUtility.FromJson<ProfileWrapper>(json);
                    
                    if (wrapper != null && wrapper.items != null) {
                        profileList = wrapper.items;
                        Debug.Log($"[DataManager] 프로필 {profileList.Count}개 로드 완료");
                    }
                    else {
                        profileList = new List<ChildProfile>();
                    }
                }
                catch (System.Exception e) {
                    Debug.LogError($"[DataManager] 로드 실패: {e.Message}");
                    profileList = new List<ChildProfile>();
                }
            }
            else {
                profileList = new List<ChildProfile>();
            }
        }

        public void SaveAllProfiles() {
            try {
                ProfileWrapper wrapper = new ProfileWrapper();
                wrapper.items = profileList;
                string json = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(ProfileListPath, json);
            }
            catch (System.Exception e) {
                Debug.LogError($"[DataManager] 저장 실패: {e.Message}");
            }
        }

        public void AddProfile(ChildProfile newProfile) {
            newProfile.ChildId = profileList.Count + (int)(System.DateTime.Now.Ticks % 10000);
            profileList.Add(newProfile);
            SaveAllProfiles(); 
            Debug.Log($"[DataManager] 새 프로필 추가됨: {newProfile.Nickname}");
        }

        public void UpdateProfile(ChildProfile updatedProfile) {
            int index = profileList.FindIndex(p => p.ChildId == updatedProfile.ChildId);
            if (index != -1) {
                profileList[index] = updatedProfile;
                SaveAllProfiles();
            }
        }

        public void DeleteProfile(ChildProfile profile) {
            ChildProfile target = profileList.Find(p => p.ChildId == profile.ChildId);
            if (target != null) {
                profileList.Remove(target);
                SaveAllProfiles();
            }
        }

        public void SelectProfile(ChildProfile profile) {
            currentProfile = profile;
            Debug.Log($"[DataManager] 현재 플레이어 설정: {currentProfile.Nickname}");
        }
        
        // 단일 프로필 저장 (데이터 변경 시 호출)
        public void SaveProfileData() {
            if (currentProfile != null) {
                UpdateProfile(currentProfile); 
            }
        }
        
        public bool IsTtsUsed() {
            return currentProfile != null && currentProfile.IsTtsUsed;
        }

        // ==========================================
        //              게임 데이터 관리
        // ==========================================

        public void SelectFairyTaleData(Story taleData) {
            selectedTale = taleData;
        }

        private void LoadVillageData() {
            talkData = new Dictionary<int, string[]>();
            portraitData = new Dictionary<int, Sprite>();
            GenerateObjectData();
        }

        private void GenerateObjectData() {
            talkData.Add(1000, new string[] { 
                "안녕? 난 다비드야:3", 
                "우리 뭐하고 놀까?:0:CHOICE",
            });

            if (portraitArr != null && portraitArr.Length >= 5) {
                portraitData.Add(1000 + 0, portraitArr[0]);
                portraitData.Add(1000 + 1, portraitArr[1]);
                portraitData.Add(1000 + 2, portraitArr[2]);
                portraitData.Add(1000 + 3, portraitArr[3]);
                portraitData.Add(1000 + 4, portraitArr[4]);
            }
        }

        public string GetTalkData(int id, int talkIndex) {
            if (!talkData.ContainsKey(id) || talkIndex >= talkData[id].Length)
                return null;
            return talkData[id][talkIndex];
        }

        public Sprite GetPortrait(int id, int portraitIndex) {
            int key = id + portraitIndex;
            if (portraitData.ContainsKey(key))
                return portraitData[key];
            return null;
        }

        // ==========================================
        // [추가/수정] 인벤토리 및 스토리 클리어 관리
        // ==========================================

        // 아이템 획득 (저장 기능 추가)
        public void AddRewardItem(Item item, int amount = 1) {
            if (currentProfile != null) {
                currentProfile.Inventory.AddItem(item, amount);
                SaveProfileData(); // [중요] 획득 즉시 저장
                Debug.Log($"[DataManager] 아이템 저장 완료: {item.itemName}");
            }
        }

        // 아이템 사용 (저장 기능 추가)
        public bool UseRewardItem(int itemId, int amount = 1) {
            if (currentProfile != null) {
                bool success = currentProfile.Inventory.UseItem(itemId, amount);
                if (success) {
                    SaveProfileData(); // [중요] 사용 즉시 저장
                    return true;
                }
            }
            return false;
        }

        // 스토리 클리어 처리
        public void AddClearedStory(int storyId) {
            if (currentProfile == null) return;

            if (currentProfile.ClearedStoryIds == null) 
                currentProfile.ClearedStoryIds = new List<int>();

            // 이미 깬 스토리가 아니라면 추가
            if (!currentProfile.ClearedStoryIds.Contains(storyId)) {
                currentProfile.ClearedStoryIds.Add(storyId);
                SaveProfileData(); // 클리어 기록 저장
                Debug.Log($"[DataManager] 스토리 클리어 저장 완료: ID {storyId}");
            }
        }

        // 스토리 클리어 여부 확인 (UI 표시용)
        public bool IsStoryCleared(int storyId) {
            if (currentProfile == null || currentProfile.ClearedStoryIds == null) return false;
            return currentProfile.ClearedStoryIds.Contains(storyId);
        }

        // ==========================================
        //          체크리스트 데이터 불러오기
        // ==========================================

        // 체크리스트 불러오기 함수
        public void LoadChecklistFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[DataManager] 파일을 찾을 수 없습니다: {filePath}");
                return;
            }

            string jsonString = File.ReadAllText(filePath);
            ChildStats newStats = ChecklistParser.ParseJsonToStats(jsonString);

            if (currentProfile != null)
            {
                currentProfile.Stats = newStats;
                SaveProfileData();
                Debug.Log("[DataManager] 체크리스트 데이터가 프로필에 반영되었습니다.");
            }
        }
        
        public void LoadChecklistFromResources(string fileName)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);
            if (textAsset != null)
            {
                ChildStats newStats = ChecklistParser.ParseJsonToStats(textAsset.text);
                if (currentProfile != null)
                {
                    currentProfile.Stats = newStats;
                    SaveProfileData();
                    Debug.Log("[DataManager] Resources 데이터 로드 완료");
                }
            }
            else
            {
                Debug.LogError($"Resources 폴더에서 {fileName}을 찾을 수 없습니다.");
            }
        }
    }
}