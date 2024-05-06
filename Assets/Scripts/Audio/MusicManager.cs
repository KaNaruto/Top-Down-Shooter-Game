using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] AudioClip mainTheme;
        [SerializeField] AudioClip menuTheme;

        private string _sceneName;
        void Awake ()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    
        void OnSceneLoaded (Scene scene, LoadSceneMode loadSceneMode)
        {
            Debug.Log ("OnSceneLoaded");
            string newSceneName = SceneManager.GetActiveScene().name;
            if (newSceneName != _sceneName) {
                _sceneName = newSceneName;
                Invoke (nameof(PlayMusic), .2f);
            }
        }

        void PlayMusic()
        {
            AudioClip clipToPlay = null;

            if (_sceneName == "Menu")
                clipToPlay = menuTheme;
            else if (_sceneName == "MainGame")
                clipToPlay = mainTheme;
        
            if(clipToPlay !=null) {
                AudioManager.Instance.PlayMusic(clipToPlay,2);
                Invoke(nameof(PlayMusic),clipToPlay.length);
            }
        }
    }
}
