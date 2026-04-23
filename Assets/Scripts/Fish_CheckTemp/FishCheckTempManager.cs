using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Main game manager for the Fish Temperature Checking mini-game.
/// Players drag the thermometer pen onto spots on the fish to read temperatures,
/// then press check or X to judge if the temperature is correct.
/// On round completion, the fish slides out left and a new random fish slides in from the right.
/// </summary>
public class FishCheckTempManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameTime = 60f;
    [SerializeField] private float correctTempMin = 50f;
    [SerializeField] private float correctTempMax = 60f;
    [SerializeField] private float slideSpeed = 0.5f; // Duration of slide animation
    [SerializeField] private int[] winCondition;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI thermometerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private StarDisplay starDisplay;

    [Header("Fish Image")]
    [SerializeField] private Image fishImage;
    [SerializeField] private Sprite[] fishSprites; // All available fish sprites

    [Header("Fish Spot Images")]
    [SerializeField] private Image spotHeadImage;
    [SerializeField] private Image spotMiddleImage;
    [SerializeField] private Image spotTailImage;

    [Header("Fish Spot GameObjects (for hiding during transition)")]
    [SerializeField] private GameObject spotHeadObject;
    [SerializeField] private GameObject spotMiddleObject;
    [SerializeField] private GameObject spotTailObject;

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
    private bool isTransitioning;

    // Temperature data per spot
    private float[] spotTemperatures = new float[3];
    private bool[] spotChecked = new bool[3];

    // Fish sprite tracking
    private int currentFishIndex = -1;

    // Fish image positioning
    private RectTransform fishRectTransform;
    private Vector2 fishOnScreenPos;   // The normal resting position
    private float slideOffsetX = 2200f; // How far off-screen to slide

    // Colors
    private readonly Color colorGreen = new Color(0.2f, 0.8f, 0.2f, 1f);
    private readonly Color colorOrange = new Color(1f, 0.6f, 0.2f, 1f);
    private readonly Color colorYellow = new Color(1f, 0.9f, 0.2f, 1f);
    private readonly Color colorRed = new Color(0.9f, 0.2f, 0.2f, 1f);

    private void Start()
    {
        SetupButtons();

        // Cache fish RectTransform and its starting position
        if (fishImage != null)
        {
            fishRectTransform = fishImage.GetComponent<RectTransform>();
            fishOnScreenPos = fishRectTransform.anchoredPosition;
        }

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

    public void InitGame()
    {
        currentTime = gameTime;
        score = 0;
        isGameActive = true;
        isPaused = false;
        isTransitioning = false;
        currentSpotIndex = -1;

        RollTemperatures();
        SetAllSpotColors(colorOrange);
        ResetThermoDots();
        SetRandomFishSprite();
        ShowSpots(true);

        // Reset fish position to on-screen
        if (fishRectTransform != null)
            fishRectTransform.anchoredPosition = fishOnScreenPos;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (thermometerText != null) thermometerText.text = "--\u00B0C";

        RefreshUI();
    }

    /// <summary>
    /// Set a random fish sprite, avoiding the same one twice in a row.
    /// </summary>
    private void SetRandomFishSprite()
    {
        if (fishSprites == null || fishSprites.Length == 0 || fishImage == null) return;

        int newIndex;
        if (fishSprites.Length == 1)
        {
            newIndex = 0;
        }
        else
        {
            // Pick a different fish than current
            do
            {
                newIndex = Random.Range(0, fishSprites.Length);
            } while (newIndex == currentFishIndex);
        }

        currentFishIndex = newIndex;
        if (fishSprites[newIndex] != null)
        {
            fishImage.sprite = fishSprites[newIndex];
        }
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

    /// <summary>
    /// Show or hide the fish spot drop targets and their dot indicators.
    /// </summary>
    private void ShowSpots(bool show)
    {
        if (spotHeadObject != null) spotHeadObject.SetActive(show);
        if (spotMiddleObject != null) spotMiddleObject.SetActive(show);
        if (spotTailObject != null) spotTailObject.SetActive(show);

        // Also hide/show the thermometer dots
        if (thermoDotHead != null) thermoDotHead.gameObject.SetActive(show);
        if (thermoDotMiddle != null) thermoDotMiddle.gameObject.SetActive(show);
        if (thermoDotTail != null) thermoDotTail.gameObject.SetActive(show);
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
        if (!isGameActive || isPaused || isTransitioning || spotChecked[idx]) return;

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
        if (!isGameActive || isPaused || isTransitioning || currentSpotIndex < 0) return;

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

        // All spots done? Begin fish transition
        if (spotChecked[0] && spotChecked[1] && spotChecked[2])
            StartCoroutine(FishTransitionSequence());

        RefreshUI();
    }

    /// <summary>
    /// Full transition: hide dots, slide fish out left, swap sprite, slide in from right, show dots.
    /// </summary>
    private IEnumerator FishTransitionSequence()
    {
        isTransitioning = true;

        // Brief pause to let player see the results
        yield return new WaitForSeconds(0.6f);

        // 1) Hide the spots/dots during transition
        ShowSpots(false);

        // 2) Slide current fish out to the LEFT
        if (fishRectTransform != null)
        {
            Vector2 offScreenLeft = new Vector2(fishOnScreenPos.x - slideOffsetX, fishOnScreenPos.y);
            yield return StartCoroutine(SlideFish(fishRectTransform.anchoredPosition, offScreenLeft, slideSpeed));
        }

        // 3) Change to a new random fish sprite
        SetRandomFishSprite();

        // 4) Position fish off-screen to the RIGHT
        if (fishRectTransform != null)
        {
            Vector2 offScreenRight = new Vector2(fishOnScreenPos.x + slideOffsetX, fishOnScreenPos.y);
            fishRectTransform.anchoredPosition = offScreenRight;
        }

        // 5) Roll new temperatures and reset visuals
        RollTemperatures();
        SetAllSpotColors(colorOrange);
        ResetThermoDots();
        if (thermometerText != null) thermometerText.text = "--\u00B0C";

        // 6) Slide new fish in from the RIGHT to center
        if (fishRectTransform != null)
        {
            yield return StartCoroutine(SlideFish(fishRectTransform.anchoredPosition, fishOnScreenPos, slideSpeed));
        }

        // 7) Show the spots/dots again
        ShowSpots(true);

        isTransitioning = false;
    }

    /// <summary>
    /// Smoothly slide the fish RectTransform from one position to another.
    /// Uses an ease-in-out curve for a polished feel.
    /// </summary>
    private IEnumerator SlideFish(Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth step easing (ease-in-out)
            t = t * t * (3f - 2f * t);

            fishRectTransform.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        fishRectTransform.anchoredPosition = to;
    }

    private void EndGame()
    {
        isGameActive = false;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        StateManager.setFishCheckTemp(score);
        if (starDisplay != null) starDisplay.displayStar(StateManager.GetStarValue(score, winCondition[2], winCondition[1], winCondition[0]), score);
        StateManager.SendPacket();
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
        StopAllCoroutines(); // Stop any running transitions
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
