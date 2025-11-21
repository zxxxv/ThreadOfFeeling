using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [CreateAssetMenu(fileName = "NewStory", menuName = "GameData/Story")]
    public class Story : ScriptableObject {
        [Header("스토리 기본 정보")]
        [Tooltip("동화 ID")]
        public int storyId;
        [Tooltip("동화 제목")]
        public string storyTitle;
    
        [Tooltip("동화 선택 씬의 팝업에 표시될 커버 이미지")]
        public Sprite storyCoverImage;

        [Tooltip("동화 선택 씬의 팝업에 표시될 간단한 설명")]
        [TextArea(3, 10)]
        public string storyDescription;

        [Tooltip("동화 선택 씬의 팝업에 표시될 태그 (예: #용기 #권선징악)")]
        public string storyTag;

        [Header("스토리 보상")]
        [Tooltip("이 동화를 완료했을 때 지급할 아이템 (선택 사항)")]
        public Item storyReward;

        [Header("시나리오 목록")]
        [Tooltip("이 동화를 구성하는 시나리오(페이지) 목록")]
        public List<Scenario> scenarios;
    }
}