using SQLite;
using UnityEngine;

// <summary>
namespace Data
{
    /// 7. RewardItem - 게임 보상 아이템의 정의
    /// </summary>
    [Table("RewardItem")]
    public class RewardItemDB {

        // item_id, INTEGER, 아이템 고유 ID
        [PrimaryKey, AutoIncrement]
        public int item_id { get; set; } // 기본키 

        // item_name, TEXT, 아이템 이름
        public string item_name { get; set; }

        // vector, VECTOR (물건 배치 정보)를 3개의 실수(REAL) 컬럼으로 분리
        public float vector_x { get; set; }
        public float vector_y { get; set; }
        public float vector_z { get; set; }

        /// <summary>
        /// [DB 저장 안 됨] C# 코드에서 편하게 Vector3로 접근하기 위한 도우미 속성.
        /// [Ignore] 속성은 이 속성이 DB의 컬럼이 아님을 SQLite-net에 알립니다.
        /// </summary>
        [Ignore]
        public Vector3 Position {
            // Position 값을 읽으려고 할 때
            get { 
                return new Vector3(vector_x, vector_y, vector_z); 
            }
            // Position 값에 Vector3를 할당할 때
            set {
                vector_x = value.x;
                vector_y = value.y;
                vector_z = value.z;
            }
        }
        // item_description, TEXT, 아이템 설명
        public string item_description { get; set; }

        [Ignore]
        public Sprite icon { get; set; }
        
        [Ignore]
        public GameObject prefab { get; set; }
        
    }
}