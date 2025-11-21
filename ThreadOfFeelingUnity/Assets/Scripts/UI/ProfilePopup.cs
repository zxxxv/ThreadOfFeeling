using Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UI {
    public class ProfilePopup : MonoBehaviour {
        [Header("Input Fields")]
        [SerializeField] private TMP_InputField inputNickname;
        [SerializeField] private Toggle toggleMale;
        [SerializeField] private Toggle toggleFemale;
        [SerializeField] private TMP_Dropdown dropdownAge; // 나이대 선택

        [Header("Buttons")]
        [SerializeField] private Button btnConfirm; // 생성/수정 확인 버튼
        [SerializeField] private TextMeshProUGUI txtConfirm; // 버튼 텍스트 ("생성하기" vs "수정하기")
        [SerializeField] private Button btnDelete;  // 삭제 버튼
        [SerializeField] private Button btnClose;   // 닫기 버튼

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText; // "프로필 생성" vs "프로필 설정"

        // 현재 작업 중인 데이터 및 콜백
        private ChildProfile targetProfile;
        private Action<ChildProfile> onCompleteCallback; // 생성/수정 완료 시 호출
        private Action<ChildProfile> onDeleteCallback;   // 삭제 시 호출
        private bool isEditMode; // 생성 모드인지 수정 모드인지 구분

        private void Awake() {
            // 버튼 리스너 연결
            btnConfirm.onClick.AddListener(OnConfirmClick);
            btnDelete.onClick.AddListener(OnDeleteClick);
            btnClose.onClick.AddListener(Close);

            // 나이대 드롭다운 옵션 초기화 (Enum 순서와 일치시킴)
            if (dropdownAge.options.Count == 0) {
                dropdownAge.options.Add(new TMP_Dropdown.OptionData("유치원"));        // Index 0 -> Enum 1
                dropdownAge.options.Add(new TMP_Dropdown.OptionData("초등학교 저학년")); // Index 1 -> Enum 2
                dropdownAge.options.Add(new TMP_Dropdown.OptionData("초등학교 고학년")); // Index 2 -> Enum 3
            }

            gameObject.SetActive(false); // 시작 시 꺼둠
        }

        // [생성 모드] 팝업 열기
        public void OpenCreate(Action<ChildProfile> onComplete) {
            isEditMode = false;
            targetProfile = null;
            onCompleteCallback = onComplete;
            onDeleteCallback = null;

            // UI 초기화
            titleText.text = "새 프로필 생성";
            txtConfirm.text = "생성하기";
            inputNickname.text = "";
            toggleMale.isOn = true; // 기본값 남자
            dropdownAge.value = 0;  // 기본값 유치원

            btnDelete.gameObject.SetActive(false); // 생성 중엔 삭제 버튼 숨김
            gameObject.SetActive(true);
        }

        // [수정 모드] 팝업 열기
        public void OpenEdit(ChildProfile profile, Action<ChildProfile> onUpdate, Action<ChildProfile> onDelete) {
            isEditMode = true;
            targetProfile = profile;
            onCompleteCallback = onUpdate;
            onDeleteCallback = onDelete;

            // 기존 정보 채워넣기
            titleText.text = "프로필 설정";
            txtConfirm.text = "수정하기";
            inputNickname.text = profile.Nickname;

            if (profile.Gender == Gender.Male) toggleMale.isOn = true;
            else toggleFemale.isOn = true;

            // AgeBand(1~3)를 Dropdown Index(0~2)로 변환
            dropdownAge.value = Mathf.Clamp((int)profile.AgeBand - 1, 0, 2);

            btnDelete.gameObject.SetActive(true); // 수정 중엔 삭제 버튼 보임
            gameObject.SetActive(true);
        }

        private void OnConfirmClick() {
            string nick = inputNickname.text.Trim();
            if (string.IsNullOrEmpty(nick)) {
                Debug.LogWarning("닉네임을 입력해주세요!");
                return; // 빈 이름 방지
            }

            Gender gender = toggleMale.isOn ? Gender.Male : Gender.Female;
            AgeBand age = (AgeBand)(dropdownAge.value + 1); // Index 0 -> Enum 1

            if (isEditMode && targetProfile != null) {
                // 기존 객체의 내용만 변경 (수정)
                targetProfile.Nickname = nick;
                targetProfile.Gender = gender;
                targetProfile.AgeBand = age;
                targetProfile.UpdateTimestamp();

                onCompleteCallback?.Invoke(targetProfile);
            }
            else {
                // 새 객체 만들기 (생성)
                ChildProfile newProfile = new ChildProfile(nick, age, gender);
                onCompleteCallback?.Invoke(newProfile);
            }

            Close();
        }

        private void OnDeleteClick() {
            // 삭제 처리
            if (isEditMode && targetProfile != null) {
                onDeleteCallback?.Invoke(targetProfile);
            }
            Close();
        }

        public void Close() {
            gameObject.SetActive(false);
        }
    }
}