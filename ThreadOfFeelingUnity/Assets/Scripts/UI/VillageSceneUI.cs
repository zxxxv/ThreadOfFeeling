using Components;
using Managers;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class VillageSceneUI : SceneUI {

        [Header("Talk UI")]
        [Tooltip("대화창 패널 게임 오브젝트")]
        [SerializeField] private GameObject talkPanel;
        [Tooltip("대화 내용이 표시될 Text")]
        [SerializeField] private TextMeshProUGUI objectText;
        [Tooltip("NPC 초상화가 표시될 Image")]
        [SerializeField] private Image portraitImg;

        [Header("Choice Buttons")]
        [Tooltip("동화 선택 버튼")]
        [SerializeField] private GameObject choiceBttnStory;
        [Tooltip("하우징 선택 버튼")]
        [SerializeField] private GameObject choiceBttnHousing;

        protected override void Start() {
            base.Start();
            ShowChoiceButtons(false);
            SoundManager.Instance.PlayBgm(true);
        }

        protected override void Update() {
            base.Update();
        }

        public void ShowTalkPanel(string text, Sprite portrait, bool isNPC) {
            SoundManager.Instance.SelectSound(true);
            talkPanel.SetActive(true);
            objectText.text = text;

            if (isNPC && portrait != null) {
                // NPC이고 초상화가 있으면 표시
                portraitImg.sprite = portrait;
                portraitImg.color = new Color(1, 1, 1, 1); // 완전 불투명
            }
            else {
                // NPC가 아니거나 초상화가 없으면 숨김
                portraitImg.sprite = null;
                portraitImg.color = new Color(1, 1, 1, 0); // 완전 투명
            }
        }

        public void HideTalkPanel() {
            SoundManager.Instance.SelectSound(false);
            talkPanel.SetActive(false);
        }

        public void ShowChoiceButtons(bool show) {
            choiceBttnStory.SetActive(show);
            choiceBttnHousing.SetActive(show);
        }

        // 체크리스트 html로 열기
        public void OnClickOpenHtmlReport() {
            SoundManager.Instance.SelectSound();
            string fileName = "report.html"; 
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(path)) {
                Application.OpenURL("file://" + path);
                Debug.Log($"[ViallageSceneUI] HTML 파일 열기 시도: {path}");
            }
            else {
                Debug.LogError($"[ViallageSceneUI] 파일을 찾을 수 없습니다: {path}\n'Assets/StreamingAssets' 폴더에 '{fileName}' 파일을 넣어주세요.");
            }
        }
    }
}
