using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Editor script to set up the Fish Temperature Check scene.
/// Run from menu: Tools > Fish CheckTemp > Setup Scene
///
/// Sprite mapping from asset.png sheet:
///   asset_0  (23x22)   = green dot (small)
///   asset_1  (70x70)   = pause button icon
///   asset_2  (23x23)   = orange dot (small)
///   asset_4  (240x181) = X button (red cross rounded rect)
///   asset_5  (436x395) = thermometer probe instrument
///   asset_6  (47x47)   = green spot dot
///   asset_7  (47x47)   = orange spot dot
///   asset_8  (47x47)   = yellow spot dot
///   asset_9  (240x181) = check button (green check rounded rect)
///   asset_10 (78x79)   = green circle (large)
///   asset_11 (78x79)   = red circle (large)
///   asset_12 (78x78)   = gray circle (large)
/// </summary>
public class FishCheckTempSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Fish CheckTemp/Setup Scene")]
    public static void SetupScene()
    {
        // Clean up any existing root
        GameObject existingRoot = GameObject.Find("FishCheckTemp_Root");
        if (existingRoot != null)
            Undo.DestroyObjectImmediate(existingRoot);

        // === CAMERA ===
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 5.4f;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.15f, 0.15f, 0.2f, 1f);
        }

        // === LOAD SPRITES ===
        Sprite bgSprite      = Spr("Assets/Image/Fish_CheckTemp/bg.png", "bg_0");
        Sprite fishSprite     = Spr("Assets/Image/Fish_CheckTemp/fish.png", "fish_1");
        Sprite pauseIcon      = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_1");
        Sprite xBtnSprite     = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_4");
        Sprite checkBtnSprite = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_9");
        Sprite probeSprite    = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_5");
        Sprite spotOrangeSpr  = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_7");
        Sprite dotGreenLg     = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_10");
        Sprite dotRedLg       = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_11");
        Sprite dotGrayLg      = Spr("Assets/Image/Fish_CheckTemp/asset.png", "asset_12");

        // === ROOT ===
        GameObject root = new GameObject("FishCheckTemp_Root");
        Undo.RegisterCreatedObjectUndo(root, "Create FishCheckTemp");

        // === CANVAS ===
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(root.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 5f;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        // ========= UI LAYERS (bottom to top) =========

        // --- BACKGROUND ---
        GameObject bgObj = Img(canvasObj.transform, "Background", bgSprite);
        Stretch(bgObj);

        // --- FISH IMAGE ---
        GameObject fishObj = Img(canvasObj.transform, "FishImage", fishSprite);
        Rect(fishObj, new Vector2(-150, 20), new Vector2(780, 380));

        // --- FISH SPOT DROP TARGETS (orange dots on the fish) ---
        GameObject spotHeadObj   = MakeSpotTarget(canvasObj.transform, "SpotHead",   spotOrangeSpr, new Vector2(-430, 50),  55f, 0);
        GameObject spotMiddleObj = MakeSpotTarget(canvasObj.transform, "SpotMiddle", spotOrangeSpr, new Vector2(-160, 85),  55f, 1);
        GameObject spotTailObj   = MakeSpotTarget(canvasObj.transform, "SpotTail",   spotOrangeSpr, new Vector2(60, 35),    55f, 2);

        // --- THERMOMETER AREA ---
        // Thermometer dots - VERTICAL ALIGNMENT (stacked on right side)
        float dotX = 750f;
        float dotTopY = 140f;
        float dotSpacing = 70f;
        float dotSize = 55f;

        GameObject thermDotHead   = Img(canvasObj.transform, "ThermoDotHead",   dotGrayLg);
        Rect(thermDotHead,   new Vector2(dotX, dotTopY), new Vector2(dotSize, dotSize));
        GameObject thermDotMiddle = Img(canvasObj.transform, "ThermoDotMiddle", dotGrayLg);
        Rect(thermDotMiddle, new Vector2(dotX, dotTopY - dotSpacing), new Vector2(dotSize, dotSize));
        GameObject thermDotTail   = Img(canvasObj.transform, "ThermoDotTail",   dotGrayLg);
        Rect(thermDotTail,   new Vector2(dotX, dotTopY - dotSpacing * 2), new Vector2(dotSize, dotSize));

        // --- TEMPERATURE DISPLAY TEXT ---
        GameObject tempTextObj = MakeText(canvasObj.transform, "ThermometerText", "--\u00B0C",
            new Vector2(580, 100), new Vector2(200, 90), 52, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
        AddOutline(tempTextObj, new Color(0.1f, 0.1f, 0.1f, 0.8f), new Vector2(2, -2));

        // --- DRAGGABLE PEN (separate from thermometer BG, on top) ---
        GameObject penObj = new GameObject("DraggablePen");
        penObj.transform.SetParent(canvasObj.transform, false);
        Image penImg = penObj.AddComponent<Image>();
        penImg.sprite = probeSprite;
        penImg.preserveAspect = true;
        penImg.raycastTarget = true;
        penObj.AddComponent<CanvasGroup>();
        penObj.AddComponent<DraggablePen>();
        RectTransform penRect = penObj.GetComponent<RectTransform>();
        penRect.anchoredPosition = new Vector2(580, -200);
        penRect.sizeDelta = new Vector2(280, 250);

        // --- TOP BAR ---
        GameObject topBar = Img(canvasObj.transform, "TopBar", null);
        topBar.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.7f);
        RectTransform topBarRect = topBar.GetComponent<RectTransform>();
        topBarRect.anchorMin = new Vector2(0, 1);
        topBarRect.anchorMax = new Vector2(1, 1);
        topBarRect.pivot = new Vector2(0.5f, 1);
        topBarRect.anchoredPosition = Vector2.zero;
        topBarRect.sizeDelta = new Vector2(0, 90);

        // Back Button
        GameObject btnBackObj = Btn(canvasObj.transform, "BtnBack", pauseIcon, new Vector2(-890, 495), new Vector2(60, 60));

        // Timer
        GameObject timerObj = MakeText(canvasObj.transform, "TimerText", "01:00",
            new Vector2(-740, 495), new Vector2(200, 60), 44, Color.white, FontStyle.Bold, TextAnchor.MiddleLeft);
        AddOutline(timerObj, Color.black, new Vector2(1, -1));

        // Title
        GameObject titleObj = MakeText(canvasObj.transform, "TitleText",
            "\"\u0E15\u0E23\u0E27\u0E08\u0E27\u0E31\u0E14\u0E2D\u0E38\u0E13\u0E2B\u0E20\u0E39\u0E21\u0E34\u0E1B\u0E25\u0E32\"",
            new Vector2(0, 495), new Vector2(500, 60), 34, Color.white, FontStyle.Normal, TextAnchor.MiddleCenter);

        // Score
        GameObject scoreObj = MakeText(canvasObj.transform, "ScoreText", "score: 0",
            new Vector2(680, 495), new Vector2(300, 60), 36, Color.white, FontStyle.Normal, TextAnchor.MiddleRight);

        // Pause Button
        GameObject btnPauseObj = Btn(canvasObj.transform, "BtnPause", pauseIcon, new Vector2(890, 495), new Vector2(60, 60));

        // --- ANSWER BUTTONS (bottom center) ---
        GameObject btnCorrectObj = Btn(canvasObj.transform, "BtnCorrect", checkBtnSprite, new Vector2(-250, -380), new Vector2(160, 120));
        GameObject btnWrongObj   = Btn(canvasObj.transform, "BtnWrong",   xBtnSprite,     new Vector2(250, -380),  new Vector2(160, 120));

        // --- GAME OVER PANEL ---
        GameObject gameOverObj = new GameObject("GameOverPanel");
        gameOverObj.transform.SetParent(canvasObj.transform, false);
        Image goImg = gameOverObj.AddComponent<Image>();
        goImg.color = new Color(0, 0, 0, 0.78f);
        Stretch(gameOverObj);

        MakeText(gameOverObj.transform, "GameOverTitle", "GAME OVER",
            new Vector2(0, 120), new Vector2(700, 120), 80, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
        GameObject goScoreObj = MakeText(gameOverObj.transform, "GameOverScore", "Final Score: 0",
            new Vector2(0, -10), new Vector2(500, 70), 46, Color.yellow, FontStyle.Bold, TextAnchor.MiddleCenter);

        GameObject btnRestartObj = new GameObject("BtnRestart");
        btnRestartObj.transform.SetParent(gameOverObj.transform, false);
        btnRestartObj.AddComponent<Image>().color = new Color(0.15f, 0.72f, 0.33f, 1f);
        Button restartBtn = btnRestartObj.AddComponent<Button>();
        Rect(btnRestartObj, new Vector2(0, -160), new Vector2(320, 90));
        MakeText(btnRestartObj.transform, "RestartLabel", "RESTART",
            Vector2.zero, new Vector2(300, 80), 40, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
        gameOverObj.SetActive(false);

        // === GAME MANAGER ===
        GameObject managerObj = new GameObject("GameManager");
        managerObj.transform.SetParent(root.transform);
        FishCheckTempManager mgr = managerObj.AddComponent<FishCheckTempManager>();

        // Wire up spot drop targets -> manager reference
        spotHeadObj.GetComponent<FishSpotDropTarget>().manager = mgr;
        spotMiddleObj.GetComponent<FishSpotDropTarget>().manager = mgr;
        spotTailObj.GetComponent<FishSpotDropTarget>().manager = mgr;

        // Wire all serialized fields
        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("timerText").objectReferenceValue       = timerObj.GetComponent<Text>();
        so.FindProperty("scoreText").objectReferenceValue       = scoreObj.GetComponent<Text>();
        so.FindProperty("thermometerText").objectReferenceValue = tempTextObj.GetComponent<Text>();
        so.FindProperty("gameOverPanel").objectReferenceValue   = gameOverObj;
        so.FindProperty("gameOverScoreText").objectReferenceValue = goScoreObj.GetComponent<Text>();

        so.FindProperty("spotHeadImage").objectReferenceValue   = spotHeadObj.GetComponent<Image>();
        so.FindProperty("spotMiddleImage").objectReferenceValue = spotMiddleObj.GetComponent<Image>();
        so.FindProperty("spotTailImage").objectReferenceValue   = spotTailObj.GetComponent<Image>();

        so.FindProperty("btnCorrect").objectReferenceValue      = btnCorrectObj.GetComponent<Button>();
        so.FindProperty("btnWrong").objectReferenceValue        = btnWrongObj.GetComponent<Button>();

        so.FindProperty("thermoDotHead").objectReferenceValue   = thermDotHead.GetComponent<Image>();
        so.FindProperty("thermoDotMiddle").objectReferenceValue = thermDotMiddle.GetComponent<Image>();
        so.FindProperty("thermoDotTail").objectReferenceValue   = thermDotTail.GetComponent<Image>();

        // Dot sprites
        so.FindProperty("dotGraySprite").objectReferenceValue  = dotGrayLg;
        so.FindProperty("dotGreenSprite").objectReferenceValue = dotGreenLg;
        so.FindProperty("dotRedSprite").objectReferenceValue   = dotRedLg;

        so.FindProperty("btnBack").objectReferenceValue         = btnBackObj.GetComponent<Button>();
        so.FindProperty("btnPause").objectReferenceValue        = btnPauseObj.GetComponent<Button>();
        so.FindProperty("btnRestart").objectReferenceValue      = restartBtn;
        so.ApplyModifiedProperties();

        // === EVENT SYSTEM ===
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.transform.SetParent(root.transform);
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Debug.Log("[FishCheckTemp] Scene setup complete!");
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    // ============= FACTORY HELPERS =============

    private static Sprite Spr(string path, string name)
    {
        foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(path))
            if (o is Sprite s && s.name == name) return s;
        Debug.LogWarning("[FishCheckTemp] Sprite not found: " + name);
        return null;
    }

    private static GameObject Img(Transform parent, string name, Sprite sprite)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        if (sprite != null) { img.sprite = sprite; img.preserveAspect = true; }
        img.raycastTarget = false;
        return obj;
    }

    private static GameObject Btn(Transform parent, string name, Sprite sprite, Vector2 pos, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        if (sprite != null) { img.sprite = sprite; img.preserveAspect = true; }
        img.raycastTarget = true;
        Button btn = obj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        cb.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
        btn.colors = cb;
        Rect(obj, pos, size);
        return obj;
    }

    /// <summary>
    /// Creates a fish spot that is a DROP TARGET (not a button).
    /// Has Image + FishSpotDropTarget component. Raycast = true so drops work.
    /// </summary>
    private static GameObject MakeSpotTarget(Transform parent, string name, Sprite sprite, Vector2 pos, float size, int spotIndex)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        if (sprite != null) { img.sprite = sprite; img.preserveAspect = true; }
        img.raycastTarget = true; // Must be true for IDropHandler
        img.color = new Color(1f, 0.6f, 0.2f, 1f); // orange default

        FishSpotDropTarget dt = obj.AddComponent<FishSpotDropTarget>();
        dt.spotIndex = spotIndex;

        Rect(obj, pos, new Vector2(size, size));
        return obj;
    }

    private static GameObject MakeText(Transform parent, string name, string text,
        Vector2 pos, Vector2 size, int fontSize, Color color, FontStyle style, TextAnchor align)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Text t = obj.AddComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.color = color;
        t.fontStyle = style;
        t.alignment = align;
        t.raycastTarget = false;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        Rect(obj, pos, size);
        return obj;
    }

    private static void Rect(GameObject obj, Vector2 pos, Vector2 size)
    {
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchoredPosition = pos;
        r.sizeDelta = size;
    }

    private static void Stretch(GameObject obj)
    {
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }

    private static void AddOutline(GameObject obj, Color c, Vector2 dist)
    {
        Outline o = obj.AddComponent<Outline>();
        o.effectColor = c;
        o.effectDistance = dist;
    }
#endif
}
