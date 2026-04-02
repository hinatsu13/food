using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Main game manager for the Fish Temperature Checking mini-game.
/// Players drag the thermometer pen onto spots on the fish to read temperatures,
/// then press check or X to judge if the temperature is correct.
/// </summary>
public class FishCheckTempManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameTime = 60f;
    [SerializeField] private float correctTempMin = 50f;
    [SerializeField] private float correctTempMax = 60f;

    [Header("UI References")]
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text thermometerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverScoreText;

    [Header("Fish Spot Images")]
    [SerializeField] private Image spotHeadImage;
    [SerializeField] private Image spotMiddleImage;
    [SerializeField] private Image spotTailImage;

    [Header("Answer Buttons")]
    [SerializeField] private Button btnCorrect;
    [SerializeField] private Button btnWrong;

    [Header("Thermometer Indicator Dots")]
    [SerializeField] private Image thermoDotHead;
    [SerializeField] private Image thermoDotMiddle;
    [SerializeField] private Image thermoDotTail;

    [Header("Thermometer Dot Sprites")]
    [SerializeField] private Sprite dotGraySprite;
    [SerializeField] private Sprite dotGreenSprite;
    [SerializeField] private Sprite dotRedSprite;

    [Header("Control Buttons")]
    [SerializeField] private Button btnBack;
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnRestart;

    // Game state
    private float currentTime;
    private int score;
    private bool isGameActive;
    private bool isPaused;
    private int currentSpotIndex = -1;

    // Temperature data per spot
    private float[] spotTemperatures = new float[3];
    private bool[] spotChecked = new bool[3];

    // Colors
    private readonly Color colorGreen = new Color(0.2f, 0.8f, 0.2f, 1f);
    private readonly Color colorOrange = new Color(1f, 0.6f, 0.2f, 1f);
    private readonly Color colorYellow = new Color(1f, 0.9f, 0.2f, 1f);
    private readonly Color colorRed = new Color(0.9f, 0.2f, 0.2f, 1f);

    private void Start()
    {
        SetupButtons();
        InitGame();
    }

    private void SetupButtons()
    {
        if (btnCorrect != null) btnCorrect.onClick.AddListener(() => OnAnswerSelected(true));
        if (btnWrong != null) btnWrong.onClick.AddListener(() => OnAnswerSelected(false));

        if (btnBack != null) btnBack.onClick.AddListener(OnBackPressed);
        if (btnPause != null) btnPause.onClick.AddListener(OnPausePressed);
        if (btnRestart != null) btnRestart.onClick.AddListener(OnRestartPressed);

        ToggleAnswerButtons(false);
    }

    private void InitGame()
    {
        currentTime = gameTime;
        score = 0;
        isGameActive = true;
        isPaused = false;
        currentSpotIndex = -1;

        RollTemperatures();
        SetAllSpotColors(colorOrange);
        ResetThermoDots();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (thermometerText != null) thermometerText.text = "--\u00B0C";

        RefreshUI();
    }

    private void RollTemperatures()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Random.value > 0.5f)
                spotTemperatures[i] = Random.Range(correctTempMin, correctTempMax);
            else
            {
                if (Random.value > 0.5f)
                    spotTemperatures[i] = Random.Range(30f, correctTempMin - 1f);
                else
                    spotTemperatures[i] = Random.Range(correctTempMax + 1f, 80f);
            }
            spotChecked[i] = false;
        }
    }

    private void SetAllSpotColors(Color c)
    {
        if (spotHeadImage != null) spotHeadImage.color = c;
        if (spotMiddleImage != null) spotMiddleImage.color = c;
        if (spotTailImage != null) spotTailImage.color = c;
    }

    private void ResetThermoDots()
    {
        if (thermoDotHead != null && dotGraySprite != null) thermoDotHead.sprite = dotGraySprite;
        if (thermoDotMiddle != null && dotGraySprite != null) thermoDotMiddle.sprite = dotGraySprite;
        if (thermoDotTail != null && dotGraySprite != null) thermoDotTail.sprite = dotGraySprite;

        // Also reset color to white so sprite shows correctly
        if (thermoDotHead != null) thermoDotHead.color = Color.white;
        if (thermoDotMiddle != null) thermoDotMiddle.color = Color.white;
        if (thermoDotTail != null) thermoDotTail.color = Color.white;
    }

    private void Update()
    {
        if (!isGameActive || isPaused) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame();
        }
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        if (scoreText != null)
        {
            scoreText.text = "score: " + score.ToString();
        }
    }

    /// <summary>
    /// Called by FishSpotDropTarget when the pen is dropped on a spot.
    /// </summary>
    public void OnPenDroppedOnSpot(int idx)
    {
        if (!isGameActive || isPaused || spotChecked[idx]) return;

        currentSpotIndex = idx;

        // Highlight selected spot as yellow, others stay orange
        for (int i = 0; i < 3; i++)
        {
            if (!spotChecked[i])
            {
                Image img = GetSpotImage(i);
                if (img != null) img.color = (i == idx) ? colorYellow : colorOrange;
            }
        }

        // Show temperature on thermometer
        if (thermometerText != null)
            thermometerText.text = Mathf.RoundToInt(spotTemperatures[idx]).ToString() + "\u00B0C";

        ToggleAnswerButtons(true);
    }

    private void OnAnswerSelected(bool playerSaysCorrect)
    {
        if (!isGameActive || isPaused || currentSpotIndex < 0) return;

        float temp = spotTemperatures[currentSpotIndex];
        bool actuallyCorrect = temp >= correctTempMin && temp <= correctTempMax;
        bool playerRight = (playerSaysCorrect == actuallyCorrect);

        spotChecked[currentSpotIndex] = true;

        // Color the fish spot
        Image spotImg = GetSpotImage(currentSpotIndex);
        if (spotImg != null)
            spotImg.color = playerRight ? colorGreen : colorRed;

        // Update thermometer dot sprite
        Image dotImg = GetThermoDot(currentSpotIndex);
        if (dotImg != null)
        {
            dotImg.sprite = playerRight ? dotGreenSprite : dotRedSprite;
            dotImg.color = Color.white;
        }

        if (playerRight) score += 10;

        ToggleAnswerButtons(false);
        currentSpotIndex = -1;
        if (thermometerText != null) thermometerText.text = "--\u00B0C";

        // All spots done? Next round
        if (spotChecked[0] && spotChecked[1] && spotChecked[2])
            StartCoroutine(BeginNextRound());

        RefreshUI();
    }

    private IEnumerator BeginNextRound()
    {
        yield return new WaitForSeconds(1f);
        RollTemperatures();
        SetAllSpotColors(colorOrange);
        ResetThermoDots();
        if (thermometerText != null) thermometerText.text = "--\u00B0C";
    }

    private void EndGame()
    {
        isGameActive = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverScoreText != null) gameOverScoreText.text = "Final Score: " + score.ToString();
    }

    private void OnBackPressed() { Debug.Log("Back pressed"); }

    private void OnPausePressed()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void OnRestartPressed()
    {
        Time.timeScale = 1f;
        InitGame();
    }

    private void ToggleAnswerButtons(bool on)
    {
        if (btnCorrect != null) btnCorrect.interactable = on;
        if (btnWrong != null) btnWrong.interactable = on;
    }

    private Image GetSpotImage(int idx)
    {
        if (idx == 0) return spotHeadImage;
        if (idx == 1) return spotMiddleImage;
        if (idx == 2) return spotTailImage;
        return null;
    }

    private Image GetThermoDot(int idx)
    {
        if (idx == 0) return thermoDotHead;
        if (idx == 1) return thermoDotMiddle;
        if (idx == 2) return thermoDotTail;
        return null;
    }
}
