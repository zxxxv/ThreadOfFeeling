using Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    [System.Serializable]
    public class BgmData {
        public string sceneName;
        public AudioClip clip;   // 재생할 음악
    }
    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance { get; private set; }

        [Header("#BGM")]
        public BgmData[] sceneBgms;
        public float bgmVolume;

        AudioSource bgmPlayer;
        AudioLowPassFilter bgmEffect;

        [Header("#SFX")]
        public AudioClip[] sfxClips;
        public float sfxVolume;
        public int channels;
        AudioSource[] sfxPlayers;
        int channelIndex;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else {
                Destroy(gameObject);
            }
        }

        void Init() {
            // 배경음 플레이어 초기화
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            bgmPlayer = bgmObject.AddComponent<AudioSource>();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.volume = bgmVolume;
            bgmPlayer.loop = true;

            bgmEffect = bgmObject.AddComponent<AudioLowPassFilter>();
            bgmEffect.enabled = false;

            // 효과음 플레이어 초기화
            GameObject sfxObject = new GameObject("SfxPlayer");
            sfxObject.transform.parent = transform;
            sfxPlayers = new AudioSource[channels];

            for (int index = 0; index < sfxPlayers.Length; index++) {
                sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
                sfxPlayers[index].playOnAwake = false;
                sfxPlayers[index].bypassListenerEffects = true;
                sfxPlayers[index].volume = sfxVolume;
            }

         }

        // GamgeManager에서 호출하는 형식
        public void ChangeBgm(string sceneName) {
            // 현재 씬 이름에 맞는 BGM 찾기
            AudioClip targetClip = null;
            
            foreach (BgmData data in sceneBgms) {
                if (data.sceneName == sceneName) {
                    targetClip = data.clip;
                    break;
                }
            }

            // 음악 재생
            if (targetClip != null) {
                // 이미 같은 음악이 나오고 있다면 굳이 끊지 않음
                if (bgmPlayer.clip == targetClip && bgmPlayer.isPlaying)
                    return;

                bgmPlayer.clip = targetClip;
                bgmPlayer.Play();
            }
            else {
                // 해당 씬에 설정된 음악이 없으면 멈춤
                bgmPlayer.Stop();
            }
        }

        // 배경음악 재생하기
        public void PlayBgm(bool isPlay) {
            if (isPlay) {
                bgmPlayer.Play();
            }
            else {
                bgmPlayer.Stop();
            }
        }

        // 배경음악 작게
        private void EffectBgm(bool isPlay) {
            bgmEffect.enabled = isPlay;
        }

        // 효과음 재생하기
        private void PlaySfx(Sfx sfx) {
            for (int index = 0; index < sfxPlayers.Length; index++) {
                int loopIndex = (index + channelIndex) % sfxPlayers.Length;
                if (sfxPlayers[loopIndex].isPlaying) {
                    continue;
                }
                channelIndex = loopIndex;
                sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
                sfxPlayers[loopIndex].Play();
                break;
            }
            
        }

        // TTS 재생
        public void PlayTTS() { }
        // TTS 멈춤
        public void StopTTS() { }

        // 단순 선택 효과음 재생
        public void SelectSound() {
            PlaySfx(Sfx.Select);
        }

        // pop up용 선택 효과음 재생 (배경음악 소리를 줄임)
        public void SelectSound(bool effect) {
            PlaySfx(Sfx.Select);
            EffectBgm(effect);
        }

        // 클리어 효과음 재생
        public void ClearSound() {
            PlaySfx(Sfx.Clear);
        }

        // 맞췄을 때 효과음 재생
        public void RightSound() {
            PlaySfx(Sfx.Ding);
        }
        
        // 틀렸을 때 효과음 재생
        public void WrongSound() {
            PlaySfx(Sfx.Wrong);
        }
    }
}