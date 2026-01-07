using UnityEngine;

namespace Components
{
    [System.Serializable]
    public class Question {
        [Tooltip("이 칸을 비워두면 퀴즈 없이 넘어갑니다.")]
        [TextArea(2, 5)]
        public string questionText;

        // 퀴즈(질문)용 TTS 클립
        public AudioClip questionVoice;

        [Header("선택지 (최대 4개)")]
        public string answer1;
        public string answer2;
        public string answer3; // 추가됨
        public string answer4; // 추가됨

        [Range(0, 3)]
        [Tooltip("정답 인덱스 (0=answer1, 1=answer2, 2=answer3, 3=answer4)")]
        public int correctAnswerIndex = 0;

        [TextArea(2, 5)]
        public string correctFeedback;
        [TextArea(2, 5)]
        public string wrongFeedback;

        [Header("스토리 타입 설정")]
        [Tooltip("이 퀴즈가 등장할 스토리 타입 (TypeA=손/2지선다, TypeB=감정/4지선다)")]
        public StoryType targetType; 
        [Tooltip("체크하면 모든 타입에서 등장합니다.")]
        public bool isCommon = false; 
    }
}