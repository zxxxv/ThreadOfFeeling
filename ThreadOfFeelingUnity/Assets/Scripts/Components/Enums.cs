namespace Components
{
    public enum Gender {
        Male,
        Female
    }

    public enum AgeBand {
        Unknown = 0,            // 알 수 없음 (기본값)
        Kindergarten = 1,       // 유치원
        ElementaryLower = 2,    // 초등학교 저학년
        ElementaryUpper = 3     // 초등학교 고학년 이상
    }

    public enum StoryType {
        TypeA,
        TypeB,
        NotSelected
    }

    public enum GameState {
        Start,      // 시작 씬
        Profile,    // 프로필 설정 씬
        Village,    // 마을 씬
        Selection,  // 동화 선택 씬
        Story,      // 동화 씬
        Housing,    // 하우징 씬
        Paused,     // 일시정지 (UI 메뉴 등)
        NPCTalk,    // NPC 대화중
        Loading,    // 로딩 중
    }

    public enum Sfx { 
        Ding, 
        Wrong,
        Select,
        Clear   // 스토리 클리어
    };
}