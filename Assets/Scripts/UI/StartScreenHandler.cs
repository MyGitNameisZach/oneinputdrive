using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

//Title screen shown when the game first launches. Freezes the game until the player presses the action key or clicks.
//It builds its own UI in code, so nothing needs to be added to the scene. Restarts skip it and go straight into a run.
public class StartScreenHandler : MonoBehaviour
{
    const string GameTitle = "ONE INPUT DRIVE";
    const KeyCode StartKey = KeyCode.Space;

    CanvasGroup canvasGroup;
    TextMeshProUGUI promptText;
    bool isWaitingForStart = true;

    //Runs once, after the first scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void CreateOnLaunch()
    {
        //Only show the start screen in a scene that has a player car
        if (GameObject.FindGameObjectWithTag("Player") == null)
            return;

        new GameObject("Start Screen").AddComponent<StartScreenHandler>();
    }

    void Awake()
    {
        BuildUI();
        GameState.WaitForStart();
    }

    void Update()
    {
        if (!isWaitingForStart)
            return;

        //Pulse the prompt. Uses unscaled time because the game is frozen
        promptText.alpha = 0.55f + 0.45f * Mathf.Sin(Time.unscaledTime * 4.0f);

        if (IsStartPressed())
            StartGame();
    }

    bool IsStartPressed()
    {
        if (Input.GetKeyDown(StartKey) || Input.GetMouseButtonDown(0))
            return true;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
                return true;
        }

        return false;
    }

    void StartGame()
    {
        isWaitingForStart = false;
        GameState.Begin();

        StartCoroutine(FadeOutCO());
    }

    IEnumerator FadeOutCO()
    {
        canvasGroup.blocksRaycasts = false;

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0, Time.unscaledDeltaTime * 3.0f);
            yield return null;
        }

        Destroy(gameObject);
    }

    // UI construction
    void BuildUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        //Dim the road behind the menu
        GameObject background = new GameObject("Background", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(transform, false);
        StretchToFill(background.GetComponent<RectTransform>());
        background.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);

        //Use the same font as the in-game HUD so the menu matches
        UIHandler uiHandler = FindFirstObjectByType<UIHandler>();
        TMP_FontAsset font = uiHandler != null ? uiHandler.HudFont : null;

        CreateText("Title", GameTitle, 110, 230, font);
        CreateText("Best", $"BEST {HighScore.Best:000000}", 44, 80, font);
        CreateText("Controls", "TAP  -  CHANGE LANE\nHOLD  -  BRAKE", 36, -70, font);
        promptText = CreateText("Prompt", "PRESS SPACE TO START", 50, -270, font);
    }

    TextMeshProUGUI CreateText(string objectName, string text, float fontSize, float yPosition, TMP_FontAsset font)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
        textObject.transform.SetParent(transform, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.sizeDelta = new Vector2(-80, fontSize * 3);
        rect.anchoredPosition = new Vector2(0, yPosition);

        TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();

        if (font != null)
            tmp.font = font;

        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;

        return tmp;
    }

    static void StretchToFill(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
