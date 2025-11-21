using System.Collections.Generic;
using Components;
using UnityEngine;

namespace Utils
{
    public static class ChecklistParser
    {
        // 문항의 최대 점수 (5점 만점)
        private const float MAX_QUESTION_SCORE = 5f; 
        // 능력치 최대값 (100점 만점)
        private const float TARGET_STAT_MAX = 100f;

        public static ChildStats ParseJsonToStats(string jsonString)
        {
            ChildStats newStats = new ChildStats();

            try
            {
                Dictionary<string, int> responses = ParseSimpleJson(jsonString);

                if (responses == null || responses.Count == 0)
                {
                    Debug.LogError("[ChecklistParser] 'responses' 데이터를 찾을 수 없거나 비어있습니다.");
                    return newStats;
                }

                // 점수 집계 (1~7번 능력치)
                float[] sums = new float[8]; 
                int[] counts = new int[8];

                foreach (var kvp in responses)
                {
                    string key = kvp.Key; // 예: "1-1-1"
                    int score = kvp.Value; // 예: 3

                    // "1-1-1"에서 맨 앞 글자('1')만 따와서 카테고리로 사용
                    string[] parts = key.Split('-');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int categoryIdx))
                    {
                        if (categoryIdx >= 1 && categoryIdx <= 7)
                        {
                            sums[categoryIdx] += score;
                            counts[categoryIdx]++;
                        }
                    }
                }

                // 평균 계산 및 변환
                newStats.Language = CalculateStat(sums[1], counts[1]);
                newStats.Logic = CalculateStat(sums[2], counts[2]);
                newStats.Spatial = CalculateStat(sums[3], counts[3]);
                newStats.Body = CalculateStat(sums[4], counts[4]);
                newStats.Musical = CalculateStat(sums[5], counts[5]);
                newStats.Interpersonal = CalculateStat(sums[6], counts[6]);
                newStats.Nature = CalculateStat(sums[7], counts[7]);

                Debug.Log("[ChecklistParser] 능력치 변환 완료!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ChecklistParser] 파싱 오류: {e.Message}");
            }

            return newStats;
        }
        private static Dictionary<string, int> ParseSimpleJson(string json)
        {
            var result = new Dictionary<string, int>();

            // 1. "responses" 블록 찾기
            int startKeyIndex = json.IndexOf("\"responses\"");
            if (startKeyIndex == -1) return result;

            int openBraceIndex = json.IndexOf('{', startKeyIndex);
            if (openBraceIndex == -1) return result;

            int closeBraceIndex = json.IndexOf('}', openBraceIndex);
            if (closeBraceIndex == -1) return result;

            // 2. 내용물만 추출 ("1-1-1": 3, "1-1-2": 3 ...)
            string content = json.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);

            // 3. 콤마(,)로 나누고 키:값 파싱
            string[] pairs = content.Split(',');
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    // 따옴표(")와 공백 제거
                    string key = parts[0].Trim().Replace("\"", "");
                    string valueStr = parts[1].Trim();

                    if (int.TryParse(valueStr, out int value))
                    {
                        result[key] = value;
                    }
                }
            }

            return result;
        }

        private static float CalculateStat(float sum, int count)
        {
            if (count == 0) return 0;
            return (sum / count / MAX_QUESTION_SCORE) * TARGET_STAT_MAX;
        }
    }
}