using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // 텍스트 사용을 위해 필수

namespace UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RadarChart : MaskableGraphic {
        [Header("Chart Settings")]
        [Tooltip("그래프의 반지름 (크기)")]
        [SerializeField] private float radius = 150f;
        [Tooltip("선 두께 (색 채우기 대신 선만 그림)")]
        [SerializeField] private float thickness = 5f; 
        [Tooltip("능력치 개수 (7각형)")]
        [SerializeField] private int statCount = 7;
        
        [Header("Label Settings")]
        [Tooltip("꼭짓점에 표시할 숫자 텍스트 프리팹 (TMP)")]
        [SerializeField] private TextMeshProUGUI labelPrefab;
        [Tooltip("숫자 라벨이 그래프 끝(100%)에서 얼마나 더 떨어져 있을지")]
        //[SerializeField] private float labelDistance = 20f;

        // 0.0 ~ 1.0 사이로 정규화된 데이터 배열
        private float[] statValues;
        private List<TextMeshProUGUI> labels = new List<TextMeshProUGUI>();

        protected override void Awake() {
            base.Awake();
        }

        // 오브젝트가 켜질 때 라벨 생성
        protected override void OnEnable() {
            base.OnEnable();
            //CreateLabels();
        }

        // 오브젝트가 꺼질 때 라벨 삭제 (에디터 찌꺼기 방지)
        protected override void OnDisable() {
            base.OnDisable();
            //ClearLabels();
        }

        public void SetStats(float[] normalizedValues) {
            if (normalizedValues == null || normalizedValues.Length != statCount) {
                this.statValues = new float[statCount];
            } else {
                this.statValues = normalizedValues;
            }

            // [테스트용] 데이터가 0일 때 최대 크기로 표시
            //bool allZero = true;
            //foreach(float v in this.statValues) if(v > 0.01f) allZero = false;
            
            //if(allZero) {
            //     for(int i=0; i<statCount; i++) this.statValues[i] = 0.99f; 
            //}

            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            if (statValues == null || statValues.Length == 0) return;

            float angleStep = 360f / statCount;
            Vector2? prevPos = null;
            Vector2 firstPos = Vector2.zero;

            for (int i = 0; i < statCount; i++) {
                float angle = (i * -angleStep + 90) * Mathf.Deg2Rad;
                float currentRadius = radius * statValues[i];

                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;
                Vector2 currentPos = new Vector2(x, y);

                if (i == 0) firstPos = currentPos;

                if (prevPos.HasValue) {
                    DrawLine(vh, prevPos.Value, currentPos, thickness);
                }
                
                prevPos = currentPos;
            }

            if (prevPos.HasValue) {
                DrawLine(vh, prevPos.Value, firstPos, thickness);
            }
        }

        private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float width) {
            Vector2 dir = (end - start).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * width * 0.5f;

            UIVertex[] verts = new UIVertex[4];
            UIVertex v = UIVertex.simpleVert;
            v.color = color;

            v.position = start - normal; verts[0] = v;
            v.position = start + normal; verts[1] = v;
            v.position = end + normal;   verts[2] = v;
            v.position = end - normal;   verts[3] = v;

            vh.AddUIVertexQuad(verts);
        }
    }
}