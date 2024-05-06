using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Score
{
    public class Highscores : MonoBehaviour
    {
        private const string PrivateCode = "";
        private const string PublicCode = "";
        private const string WebURL = "http://dreamlo.com/lb/";
        private static Highscores _instance;

        private DisplayHighscores _highscoresDisplay;

        public Highscore[] HighscoresList;
        private void Awake()
        {
            _highscoresDisplay = GetComponent<DisplayHighscores>();
            _instance = this;
        }


        public static void AddNewHighscore(string username, int score)
        {
            _instance.StartCoroutine(_instance.UploadHighscoreToDatabase(username, score));
        }

        IEnumerator UploadHighscoreToDatabase(string username, int score)
        {
            username = Clean(username);
            string url = WebURL + PrivateCode + "/add/" + UnityWebRequest.EscapeURL(username) + "/" + score.ToString();

            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                DisplayHighscores.UserUploaded = true;
                DownloadHighscores();
            }
            else
                Debug.LogError("Error uploading: " + www.error);
        }

        public void DownloadHighscores()
        {
            StartCoroutine(nameof(DownloadHighscoresFromDatabase));
        }


        IEnumerator DownloadHighscoresFromDatabase()
        {
            string url = WebURL + PublicCode + "/pipe/";

            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                FormatHighscores(responseText);
                _highscoresDisplay.OnHighscoreDownloaded(HighscoresList);
            }
            else
            {
                Debug.LogError("Error downloading: " + www.error);
            }
        }

        void FormatHighscores(string textStream)
        {
            string[] entries = textStream.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            HighscoresList = new Highscore[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                string[] entryInfo = entries[i].Split(new[] { '|' });
                string username = entryInfo[0];
                int score = int.Parse(entryInfo[1]);

                HighscoresList[i] = new Highscore(username, score);
            }
        }

        string Clean(string s)
        {
            s = s.Replace("/", "");
            s = s.Replace("|", "");
            return s;
        }
    }

    public struct Highscore
    {
        public readonly string Username;
        public readonly int Score;

        public Highscore(string username, int score)
        {
            this.Username = username;
            this.Score = score;
        }
    }
}