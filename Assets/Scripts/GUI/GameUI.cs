using System.Collections;
using Combat.Firearm;
using LivingEntities.Enemy;
using LivingEntities.Player;
using Potions;
using Score;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadeImage;

    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public TextMeshProUGUI newWaveCount;
    public TextMeshProUGUI newEnemyCount;

    private Spawner _spawner;

    private readonly string[] _numbers =
        { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

    public TextMeshProUGUI scoreUI;
    [SerializeField] TextMeshProUGUI potionEffectLabel;
    public RectTransform healthBar;
    private Player _player;

    public TextMeshProUGUI gameOverScoreUI;

    [SerializeField] private TextMeshProUGUI remainingAmmo;
    [SerializeField] private TextMeshProUGUI totalAmmo;

    private Gun _gun;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.Score.ToString("D6");
        // Health bar
        float healthPercent = 0;
        if (_player != null)
        {
            if (_player.Health > _player.startingHealth)
                _player.startingHealth = _player.Health;
            healthPercent = _player.Health / _player.startingHealth;
        }

        healthBar.localScale = new Vector3(healthPercent, 0.5997045f, 1);

        // Ammo
        
        if (_gun != null)
            remainingAmmo.text = _gun.projectileRemainingInMag.ToString();
    }

    

    private void Awake()
    {
        _spawner = FindObjectOfType<Spawner>();
        _spawner.OnNewWave += OnNewWave;
    }

    private void OnEnable()
    {
        Potion.OnPotionUse += PotionUse;
    }

    private void OnDisable()
    {
        Potion.OnPotionUse -= PotionUse;
    }

    private void PotionUse(string effect)
    {
        StartCoroutine(PotionUseIE(effect));
    }

    private IEnumerator PotionUseIE(string effect)
    {
        potionEffectLabel.gameObject.SetActive(true);
        potionEffectLabel.text = effect;
        yield return new WaitForSeconds(2);
        potionEffectLabel.gameObject.SetActive(false);
    }

    void OnNewWave(int waveNumber)
    {
        newWaveCount.text = "- Wave " + _numbers[waveNumber - 1] + " -";
        string enemyCount = ((_spawner.waves[waveNumber - 1]).infinite)
            ? "Infinite"
            : _spawner.waves[waveNumber - 1].enemyCount.ToString();
        newEnemyCount.text = "Enemies: " + enemyCount;


        StartCoroutine(AnimateNewWaveBanner());

        // Ammo
        StartCoroutine(GetGun());
    }
    
    IEnumerator GetGun()
    {
        yield return new WaitForSeconds(0.1f);
        _gun = FindObjectOfType<GunController>().EquippedGun;
        totalAmmo.text = "/ " + _gun.projectilePerMag;
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float animationPercent = 0;
        float speed = 3f;
        float dir = 1;
        float delayTime = 1.5f;
        float endDelayTime = Time.time + 1 / speed + delayTime;
        while (animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * speed * dir;

            if (animationPercent >= 1)
            {
                animationPercent = 1;
                if (Time.time > endDelayTime)
                    dir = -1;
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-270, 50, animationPercent);
            yield return null;
        }
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, .9f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        remainingAmmo.transform.parent.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadeImage.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}