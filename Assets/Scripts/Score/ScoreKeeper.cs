using System.Collections;
using LivingEntities.Enemy;
using LivingEntities.Player;
using TMPro;
using UnityEngine;

namespace Score
{
    public class ScoreKeeper : MonoBehaviour
    {
        public static int Score { get; private set; }
        public static int PreviousScore { get; private set; }
        private float _lastEnemyKilledTime;
        private int _streakCount;
        private readonly float _streakExpiryTime = 1f;
        private int _scoreIncrement = 5;
        private int _streakMultiplier = 2;

        public TextMeshProUGUI scoreUI;

        private void Start()
        {
            Enemy.OnDeathStatic += OnEnemyKilled;
            FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
            FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        }


        private void OnEnemyKilled()
        {
            if (Time.time < _lastEnemyKilledTime + _streakExpiryTime)
                _streakCount++;
            else
            {
                _streakCount = 0;
            }

            _lastEnemyKilledTime = Time.time;

            Score += _scoreIncrement + _streakMultiplier * _streakCount;
        }

        private void OnNewWave(int waveCount)
        {
            if (waveCount == 10)
                StartCoroutine(IOnNewWave());
        }

        IEnumerator IOnNewWave()
        {
            yield return new WaitForSeconds(3);
            FindObjectOfType<Boss>().OnDeath += OnBossDeath;
        }

        void OnBossDeath()
        {
            Score += 1000;
            _streakMultiplier = 4;
            _scoreIncrement = 10;
        }

        // Prevent the multiple copy of event
        void OnPlayerDeath()
        {
            Enemy.OnDeathStatic -= OnEnemyKilled;
            FindObjectOfType<Spawner>().OnNewWave -= OnNewWave;
            var boss = FindObjectOfType<Boss>();
            if (boss != null)
            {
                boss.OnDeath -= OnBossDeath;
            }
        
            PreviousScore = Score;
            Score = 0;
        }
    }
}