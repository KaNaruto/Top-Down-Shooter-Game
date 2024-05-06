using System;
using System.Collections;
using Audio;
using Map;
using UnityEngine;

namespace LivingEntities.Enemy
{
    public class Spawner : MonoBehaviour
    {
        public event Action<int> OnNewWave;

        public Wave[] waves;
        public Wave currentWave;
        private int _currentWaveNumber;
        [SerializeField] Enemy enemy;


        [SerializeField] private Boss boss;
        [SerializeField] int bossSpawnRound;
        private bool _isBossSpawned;
        private bool _isBossDead;
        public static bool IsBossAlive { get; private set; }

        private LivingEntity _playerEntity;
        private Transform _playerTransform;

        private int _enemiesRemainingToSpawn;
        private int _enemiesAlive;
        private float _nextSpawnTime;
        public float spawnDelay = 1; // In seconds
        public float tileFlashSpeed = 4; // How many time it will flash in seconds
        [SerializeField] private int elapsedTimeForMaxSpawnSpeed;
        [SerializeField] private float minTimeBetweenSpawnSpeed;

        private MapGenerator _map;

        [SerializeField] bool devMode;

        // Anti-camping measures
        private readonly float _campResetThresholdDistance = 1.5f;
        private readonly float _timeBetweenCampingChecks = 2;
        private float _nextCampCheckTime;
        private Vector3 _campingPositionOld;
        private bool _isCamping;
        private bool _disabled;

        private void Start()
        {
            // Anti-camping
            _playerEntity = FindObjectOfType<Player.Player>();
            _playerTransform = _playerEntity.transform;
            _nextCampCheckTime = _timeBetweenCampingChecks + Time.time;
            _campingPositionOld = _playerTransform.position;

            _playerEntity.OnDeath += OnPlayerDeath;

            _map = FindObjectOfType<MapGenerator>();
            NextWave();
        }


        private void Update()
        {
            // Anti-camp measures
            if (!_disabled)
            {
                if (Time.time > _nextCampCheckTime)
                {
                    _nextCampCheckTime = Time.time + _timeBetweenCampingChecks;

                    var position = _playerTransform.position;
                    _isCamping = (Vector3.Distance(position, _campingPositionOld) <
                                  _campResetThresholdDistance);
                    _campingPositionOld = position;
                }


                // Spawn boss
                if (!_isBossSpawned && _currentWaveNumber == bossSpawnRound)
                {
                    StartCoroutine(SpawnBoss());
                    _isBossSpawned = true;
                    IsBossAlive = true;
                }

                // Spawn Enemies
                if (((_enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > _nextSpawnTime) &&
                    (_isBossSpawned == _isBossDead))
                {
                    _enemiesRemainingToSpawn--;
                    _nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;

                    StartCoroutine(nameof(SpawnEnemy));
                }
            }

            if (devMode)
                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    StopCoroutine(nameof(SpawnEnemy));
                    NextWave();
                }
        }

        IEnumerator DecreaseSpawnTime()
        {
            float decreaseRate= (currentWave.timeBetweenSpawn - minTimeBetweenSpawnSpeed) / elapsedTimeForMaxSpawnSpeed;
            while (currentWave.timeBetweenSpawn-minTimeBetweenSpawnSpeed>=0.01f)
            {
                currentWave.timeBetweenSpawn -= decreaseRate;
                yield return new WaitForSeconds(1);
            }
        }

        IEnumerator SpawnBoss()
        {
            Transform spawnTile = _map.GetRandomOpenTile();
            // Flash Colors
            Material tileMat = spawnTile.GetComponent<Renderer>().material;
            Color originalColor = tileMat.color;
            Color flashColor = Color.red;
            float spawnTimer = 0;

            while (spawnTimer < spawnDelay)
            {
                tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
                spawnTimer = spawnTimer + Time.deltaTime;
                yield return null;
            }


            Enemy spawnedBoss = Instantiate(boss, spawnTile.position + Vector3.up, Quaternion.identity);
            spawnedBoss.OnDeath += OnBossDeath;
            tileMat.color = originalColor;
        }

        IEnumerator SpawnEnemy()
        {
            Transform spawnTile = _map.GetRandomOpenTile();
            if (_isCamping) // Anti-camp measurements
                spawnTile = _map.GetTileFromPosition(_playerTransform.position);
            // Flash Colors
            Material tileMat = spawnTile.GetComponent<Renderer>().material;
            Color originalColor = tileMat.color;
            Color flashColor = Color.red;
            float spawnTimer = 0;

            while (spawnTimer < spawnDelay)
            {
                tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
                spawnTimer = spawnTimer + Time.deltaTime;
                yield return null;
            }


            Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;

            spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth,
                currentWave.skinColor);
            tileMat.color = originalColor;
        }


        void OnPlayerDeath()
        {
            _disabled = true;
        }

        void OnEnemyDeath()
        {
            _enemiesAlive--;

            if (_enemiesAlive == 0)
                NextWave();
        }

        void OnBossDeath()
        {
            _isBossDead = true;
            IsBossAlive = false;
        }

        void NextWave()
        {
            foreach (Enemy enemy1 in FindObjectsOfType<Enemy>())
                GameObject.Destroy(enemy1.gameObject);

            if (_currentWaveNumber > 0)
                AudioManager.Instance.PlaySound2D("Level Complete");


            _currentWaveNumber++;
            if (_currentWaveNumber - 1 < waves.Length)
            {
                currentWave = waves[_currentWaveNumber - 1];
                StopCoroutine(DecreaseSpawnTime());
                StartCoroutine(DecreaseSpawnTime());
            
                _enemiesRemainingToSpawn = currentWave.enemyCount;
                _enemiesAlive = currentWave.enemyCount;

                if (OnNewWave != null)
                    OnNewWave(_currentWaveNumber);

            
                ResetPlayerPosition();
            }

            Debug.Log("Wave " + _currentWaveNumber + " started");
        }

        // Resets players position to center on every wave
        void ResetPlayerPosition()
        {
            _playerTransform.position = _map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
        }

        [Serializable]
        public class Wave
        {
            public bool infinite;
            public int enemyCount;
            public float timeBetweenSpawn;

            public float moveSpeed;
            public int hitsToKillPlayer;
            public float enemyHealth;
            public Color skinColor;
        }
    }
}