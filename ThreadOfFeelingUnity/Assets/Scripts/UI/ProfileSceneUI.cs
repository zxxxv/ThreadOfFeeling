using System.Collections.Generic;
using Components;
using Managers;
using UnityEngine;

namespace UI
{
    public class ProfileSceneUi : SceneUI {
        
        [Header("Profile UI Elements")]
        [Tooltip("배치된 3개의 슬롯 (Hierarchy에서 직접 연결)")]
        [SerializeField] private ProfileSlot[] slots; 
        
        [Tooltip("프로필 팝업 창")]
        [SerializeField] private ProfilePopup profilePopup;

        private const int MAX_SLOTS = 3;

        // 현재 선택된 슬롯을 기억하는 변수
        private ProfileSlot currentSelectedSlot = null;

        protected override void Start() {
            if (profilePopup != null) profilePopup.Close();

            // 슬롯 데이터 로드
            RefreshSlots();
        }

        // 화면 갱신
        private void RefreshSlots() {
            List<ChildProfile> profiles = DataManager.Instance.profileList;
            currentSelectedSlot = null; // 선택 초기화

            for (int i = 0; i < slots.Length; i++) {
                if (i < MAX_SLOTS) {
                    // 콜백 연결
                    slots[i].Init(OnSlotClick, OnGameStartClick, OnProfileSettingClick);

                    if (i < profiles.Count) {
                        // 데이터 있음: 프로필 정보 표시
                        slots[i].SetData(profiles[i]);
                    }
                    else {
                        // 데이터 없음: 빈 슬롯(+) 표시
                        slots[i].SetData(null);
                    }
                    
                    // 선택 상태 초기화
                    slots[i].SetSelected(false);
                    slots[i].gameObject.SetActive(true);
                }
                else {
                    slots[i].gameObject.SetActive(false); 
                }
            }
        }

        // --- 이벤트 핸들러 ---

        // 슬롯 클릭 시 호출
        private void OnSlotClick(ProfileSlot clickedSlot) {
            SoundManager.Instance.SelectSound();

            if (clickedSlot.ProfileData == null) {
                DeselectAllSlots();
                
                if (profilePopup != null) {
                    profilePopup.OpenCreate(OnCreateProfile);
                }
                else {
                    Debug.LogError("[UI] 오류: profilePopup이 연결되지 않았습니다!");
                }
            }
            else {
                if (currentSelectedSlot == clickedSlot) {
                    DeselectAllSlots();
                }
                else {
                    currentSelectedSlot = clickedSlot;
                    foreach (var slot in slots) {
                        slot.SetSelected(slot == clickedSlot);
                    }
                }
            }
        }

        // 모든 슬롯 선택 해제 헬퍼 함수
        private void DeselectAllSlots() {
            currentSelectedSlot = null;
            foreach (var slot in slots) {
                slot.SetSelected(false);
            }
        }

        private void OnCreateProfile(ChildProfile newProfile) {
            DataManager.Instance.AddProfile(newProfile);
            RefreshSlots(); 
            SoundManager.Instance.RightSound(); 
        }

        private void OnUpdateProfile(ChildProfile updatedProfile) {
            DataManager.Instance.UpdateProfile(updatedProfile);
            RefreshSlots();
            SoundManager.Instance.SelectSound();
        }

        private void OnDeleteProfile(ChildProfile profile) {
            DataManager.Instance.DeleteProfile(profile);
            RefreshSlots();
            SoundManager.Instance.WrongSound(); 
        }

        private void OnProfileSettingClick(ChildProfile profile) {
            SoundManager.Instance.SelectSound();
            profilePopup.OpenEdit(profile, OnUpdateProfile, OnDeleteProfile);
        }

        private void OnGameStartClick(ChildProfile profile) {
            DataManager.Instance.SelectProfile(profile);
            OnClickGoToVillage();
        }
    }
}