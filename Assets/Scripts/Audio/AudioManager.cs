using System;
using System.Collections;
using LivingEntities.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public enum AudioChannel
        {
            Master,
            Sfx,
            Music
        }


        public float MasterVolumePercent { get; private set; } = 1;
        public float SfxVolumePercent { get; private set; } = 1;
        public float MusicVolumePercent { get; private set; } = 1;

        private AudioSource _sfx2DSource;
        private AudioSource[] _musicSources;
        private int _activeMusicSoundIndex;

        public static AudioManager Instance;

        private Transform _audioListener;
        private Transform _playerT;

        private SoundLibrary _library;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _library = GetComponent<SoundLibrary>();

                DontDestroyOnLoad(gameObject);

                _musicSources = new AudioSource[2];
                for (int i = 0; i < 2; i++)
                {
                    GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                    _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                    newMusicSource.transform.parent = transform;
                }

                // For 2D
                GameObject newSfx2DSource = new GameObject("2D sfx source");
                _sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
                newSfx2DSource.transform.parent = transform;

                _audioListener = FindObjectOfType<AudioListener>().transform;

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _playerT = FindObjectOfType<Player>()?.transform;
        }

        private void Update()
        {
            if (_playerT != null)
                _audioListener.position = _playerT.position;
        }


        public void SetVolume(float volumePercent, AudioChannel channel)
        {
            switch (channel)
            {
                case AudioChannel.Master:
                    MasterVolumePercent = volumePercent;
                    break;
                case AudioChannel.Music:
                    MusicVolumePercent = volumePercent;
                    break;
                case AudioChannel.Sfx:
                    SfxVolumePercent = volumePercent;
                    break;
            }

            _musicSources[0].volume = MusicVolumePercent * MasterVolumePercent;
            _musicSources[1].volume = MusicVolumePercent * MasterVolumePercent;

            // Save players settings
            PlayerPrefs.SetFloat("master vol", MasterVolumePercent);
            PlayerPrefs.SetFloat("sfx vol", SfxVolumePercent);
            PlayerPrefs.SetFloat("music vol", MusicVolumePercent);
            PlayerPrefs.Save();
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1)
        {
            _activeMusicSoundIndex = 1 - _activeMusicSoundIndex;
            _musicSources[_activeMusicSoundIndex].clip = clip;
            _musicSources[_activeMusicSoundIndex].Play();

            StartCoroutine(AnimateMusicCrossfade(fadeDuration));
        }


        IEnumerator AnimateMusicCrossfade(float duration)
        {
            float percent = 0;
            float speed = 1 / duration;
            while (percent < 1)
            {
                percent += Time.deltaTime * speed;
                // For active music
                _musicSources[_activeMusicSoundIndex].volume =
                    Mathf.Lerp(0, MusicVolumePercent * MasterVolumePercent, percent);
                // For nonactive music
                _musicSources[1 - _activeMusicSoundIndex].volume =
                    Mathf.Lerp(MusicVolumePercent * MasterVolumePercent, 0, percent);

                yield return null;
            }
        }

        public void PlaySound(AudioClip clip, Vector3 pos)
        {
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, pos, SfxVolumePercent * MasterVolumePercent);
        }

        public void PlaySound(String soundName, Vector3 pos)
        {
            PlaySound(_library.GetClipFromName(soundName), pos);
        }

        public void PlaySound2D(string soundName)
        {
            _sfx2DSource.PlayOneShot(_library.GetClipFromName(soundName), SfxVolumePercent * MasterVolumePercent);
        }
    }
}