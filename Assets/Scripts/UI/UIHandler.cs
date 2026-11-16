using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI distanceTravelledText;

    [SerializeField]
    TextMeshProUGUI gameOverText;

    [SerializeField]
    CanvasGroup gameOverCanvasGroup;

    //Optional. If left empty a "BEST" label is created under the distance counter automatically
    [SerializeField]
    TextMeshProUGUI bestDistanceText;

    //Reference
    CarHandler playerCarHandler;

    int bestDistance = 0;
    bool hasCrashed = false;

    //Lets other UI (like the start screen) use the same font as the HUD
    public TMP_FontAsset HudFont => distanceTravelledText != null ? distanceTravelledText.font : null;

    void Awake()
    {
        playerCarHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<CarHandler>();
        playerCarHandler.OnPlayerCrashed += PlayerCarHandler_OnPlayerCrashed;
    }

    void OnDestroy()
    {
        if (playerCarHandler != null)
            playerCarHandler.OnPlayerCrashed -= PlayerCarHandler_OnPlayerCrashed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.alpha = 0;

        bestDistance = HighScore.Best;

        if (bestDistanceText == null)
            bestDistanceText = CreateBestDistanceText();

        UpdateBestDistanceText(bestDistance);
    }

    // Update is called once per frame
    void Update()
    {
        int distance = GetDistance();

        distanceTravelledText.text = distance.ToString("000000");

        //Once the player passes their best, the BEST counter climbs with them
        if (!hasCrashed && distance > bestDistance)
            UpdateBestDistanceText(distance);
    }

    int GetDistance()
    {
        return Mathf.Max(0, Mathf.RoundToInt(playerCarHandler.DistanceTravelled));
    }

    void UpdateBestDistanceText(int distance)
    {
        if (bestDistanceText != null)
            bestDistanceText.text = $"BEST {distance:000000}";
    }

    //Copies the distance counter's style and places the copy just below it
    TextMeshProUGUI CreateBestDistanceText()
    {
        TextMeshProUGUI bestText = Instantiate(distanceTravelledText, distanceTravelledText.transform.parent);
        bestText.name = "Best Distance Text (TMP)";

        RectTransform rect = bestText.rectTransform;
        rect.anchoredPosition += new Vector2(0, -distanceTravelledText.rectTransform.rect.height * 1f);

        //Smaller than the main counter
        bestText.enableAutoSizing = true;
        bestText.fontSizeMax = Mathf.Max(12, distanceTravelledText.fontSize * 1f);
        bestText.alpha = 0.75f;

        return bestText;
    }

    IEnumerator StartGameOverAnimationCO()
    {
        yield return new WaitForSecondsRealtime(3.0f);

        gameOverCanvasGroup.interactable = true;

        while (gameOverCanvasGroup.alpha < 0.8f)
        {
            gameOverCanvasGroup.alpha = Mathf.MoveTowards(gameOverCanvasGroup.alpha, 1.0f, Time.deltaTime * 2);

            yield return null;
        }
    }

    // Events
    void PlayerCarHandler_OnPlayerCrashed(CarHandler obj)
    {
        //The player can be hit by flying parts after crashing, only handle the first crash
        if (hasCrashed)
            return;

        hasCrashed = true;

        int distance = GetDistance();
        bool isNewBest = HighScore.Submit(distance);

        if (isNewBest)
        {
            bestDistance = distance;
            gameOverText.text = $"NEW BEST!\nDISTANCE {distance:000000}";
        }
        else
        {
            gameOverText.text = $"DISTANCE {distance:000000}\nBEST {bestDistance:000000}";
        }

        UpdateBestDistanceText(bestDistance);

        StartCoroutine(StartGameOverAnimationCO());
    }

    public void OnRestartClicked()
    {
        //Restore time scale
        Time.timeScale = 1.0f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
