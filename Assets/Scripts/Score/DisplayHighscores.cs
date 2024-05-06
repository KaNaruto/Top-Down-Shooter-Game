using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Score
{
    public class DisplayHighscores : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI[] highscoreText;
        private Highscores _highscoreManager;
    
        [SerializeField] TMP_InputField usernameInputField;
        public static bool UserUploaded;

        private void Start()
        {
            _highscoreManager = GetComponent<Highscores>();

            for (int i = 0; i < highscoreText.Length; i++)
            {
                highscoreText[i].text = i + 1 + ". Fetching";
            }

            _highscoreManager.DownloadHighscores();
        
        }

        private void ResetUploadStatus(Scene scene, LoadSceneMode sceneMode)
        {
            UserUploaded = false;
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += ResetUploadStatus;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= ResetUploadStatus;
        }
    


        public void OnHighscoreDownloaded(Highscore[] highscoreList)
        {
            for (int i = 0; i < highscoreText.Length; i++)
            {
                highscoreText[i].text = i + 1 + ". ";
                if (highscoreList.Length > i)
                    highscoreText[i].text += highscoreList[i].Username + " - " + highscoreList[i].Score;
            }
        }

        public void RefreshHighscores()
        {
            if(!UserUploaded)
            {
                string username = usernameInputField.text;
                int score = ScoreKeeper.PreviousScore;
                Highscores.AddNewHighscore(username,score);
            }
        }
    }
}