using System;
using System.Collections.Generic;

namespace Components {
    
    [System.Serializable]
    public class ChildProfile {
        public int ChildId;
        public string Nickname;
        public AgeBand AgeBand;
        public Gender Gender;
        public bool IsTtsUsed; 
        public int RoomId;
        
        public string CreatedAt; 
        public string UpdatedAt;
        
        public Inventory Inventory;
        public List<int> ClearedStoryIds;

        public ChildStats Stats;

        public ChildProfile() {
            this.Inventory = new Inventory();
            this.ClearedStoryIds = new List<int>();
            this.CreatedAt = DateTime.Now.ToString();
            this.UpdatedAt = DateTime.Now.ToString();
        }

        public ChildProfile(string nickname, AgeBand ageBand, Gender gender) {
            this.Nickname = nickname;
            this.AgeBand = ageBand;
            this.Gender = gender;
            this.IsTtsUsed = false;
            
            // 날짜를 문자열로 변환해서 저장
            this.CreatedAt = DateTime.Now.ToString(); 
            this.UpdatedAt = DateTime.Now.ToString();

            this.Inventory = new Inventory();
            this.ClearedStoryIds = new List<int>();
            this.Stats = new ChildStats();
        }

        // 문자열로 저장된 날짜를 DateTime으로 변환해서 가져오는 함수
        public DateTime GetCreatedDate() {
            if (DateTime.TryParse(CreatedAt, out DateTime date)) return date;
            return DateTime.MinValue;
        }
        
        public void UpdateTimestamp() {
            this.UpdatedAt = DateTime.Now.ToString();
        }

        // 나이대 한글 반환 프로퍼티
        public string AgeBandText {
            get {
                switch (AgeBand) {
                    case AgeBand.Kindergarten:      return "유치원";
                    case AgeBand.ElementaryLower:   return "초등학교 저학년";
                    case AgeBand.ElementaryUpper:   return "초등학교 고학년";
                    default:                        return "알 수 없음";
                }
            }
        }

        // 성별 한글 반환 프로퍼티
        public string GenderText {
            get {
                switch (Gender) {
                    case Gender.Male:   return "남자";
                    case Gender.Female: return "여자";
                    default:            return "-";
                }
            }
        }
    }
}