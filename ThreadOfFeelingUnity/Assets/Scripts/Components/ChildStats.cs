using UnityEngine;

namespace Components {

    // 7각형 능력치 데이터
    [System.Serializable]
    public class ChildStats {
        // 7가지 다중 지능 예시
        public float Language;      // 언어
        public float Logic;         // 논리수학
        public float Spatial;       // 공간
        public float Body;          // 신체운동
        public float Musical;       // 음악
        public float Interpersonal; // 대인관계
        public float Nature;        // 자연친화

        // 차트 그리기용 배열 반환
        public float[] GetNormalizedValues(float maxStatValue = 100f) {
            return new float[] {
                Mathf.Clamp01(Language / maxStatValue),
                Mathf.Clamp01(Logic / maxStatValue),
                Mathf.Clamp01(Spatial / maxStatValue),
                Mathf.Clamp01(Body / maxStatValue),
                Mathf.Clamp01(Musical / maxStatValue),
                Mathf.Clamp01(Interpersonal / maxStatValue),
                Mathf.Clamp01(Nature / maxStatValue)
            };
        }

        public ChildStats() {
            // 초기값 (테스트용)
            Language = 20; Logic = 20; Spatial = 20;
            Body = 20; Musical = 20; Interpersonal = 20; Nature = 20;
        }
    }
}