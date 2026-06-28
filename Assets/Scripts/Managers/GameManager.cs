using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Score;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public TMP_Text winScoreText;
    public TMP_Text gameOverScoreText;
    private bool isPaused = false;
    private bool isGameOverPanelStyled = false;
    private ParticleSystem cameraSnowfall;
    private Camera gameplayCamera;

    void Start()
    {
        ConfigureSnowfall();
        ConfigureRockObstacles();
    }

    void LateUpdate()
    {
        FollowCameraWithSnowfall();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void GameOver()
    {
        StyleGameOverPanel();
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score: " + player.score;
            HighestScore.SaveHighScore(player.score); // Cập nhật điểm cao nhất
        }
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0;
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && winScoreText != null)
        {
            winScoreText.text = "Score: " + player.score;
            HighestScore.SaveHighScore(player.score); // Cập nhật điểm cao nhất
        }
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log($"Next level is: {nextSceneIndex}!");
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        if (cameraSnowfall != null)
        {
            Destroy(cameraSnowfall.gameObject);
        }

        SceneManager.LoadSceneAsync(0);
    }

    private void ConfigureSnowfall()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }

        ParticleSystem[] particles = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        Material snowMaterial = null;
        foreach (ParticleSystem particle in particles)
        {
            if (!particle.name.Contains("Snow Generator"))
            {
                continue;
            }

            ParticleSystemRenderer particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
            if (snowMaterial == null && particleRenderer != null)
            {
                snowMaterial = particleRenderer.sharedMaterial;
            }

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        gameplayCamera = Camera.main;
        GameObject snowfallObject = new GameObject("Camera Snowfall", typeof(ParticleSystem), typeof(ParticleSystemRenderer));
        cameraSnowfall = snowfallObject.GetComponent<ParticleSystem>();

        ParticleSystemRenderer renderer = snowfallObject.GetComponent<ParticleSystemRenderer>();
        if (renderer != null && snowMaterial != null)
        {
            renderer.sharedMaterial = snowMaterial;
        }

        SetupCameraSnowfall(cameraSnowfall);
        FollowCameraWithSnowfall();
        cameraSnowfall.Play(true);
    }

    private void SetupCameraSnowfall(ParticleSystem snowfall)
    {
        ParticleSystem.MainModule main = snowfall.main;
        main.loop = true;
        main.playOnAwake = true;
        main.prewarm = true;
        main.startLifetime = 6.5f;
        main.startSpeed = 0f;
        main.startSize = 0.18f;
        main.maxParticles = 1200;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        ParticleSystem.EmissionModule emission = snowfall.emission;
        emission.enabled = true;
        emission.rateOverTime = 150f;

        ParticleSystem.ShapeModule shape = snowfall.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
        shape.position = new Vector3(0f, 12f, 0f);
        shape.scale = new Vector3(40f, 1f, 1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = snowfall.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = -0.8f;
        velocity.y = -5.2f;

        ParticleSystemRenderer renderer = snowfall.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 120;
        }
    }

    private void FollowCameraWithSnowfall()
    {
        if (cameraSnowfall == null)
        {
            return;
        }

        if (gameplayCamera == null)
        {
            gameplayCamera = Camera.main;
        }

        if (gameplayCamera == null)
        {
            return;
        }

        cameraSnowfall.transform.position = new Vector3(
            gameplayCamera.transform.position.x,
            gameplayCamera.transform.position.y,
            0f
        );
        cameraSnowfall.transform.rotation = Quaternion.identity;
    }

    private void ConfigureRockObstacles()
    {
        // Physics material: nẩy lên khi chạm đá, không ma sát
        PhysicsMaterial2D bouncyMat = new PhysicsMaterial2D("BouncyRock");
        bouncyMat.bounciness = 0.7f;
        bouncyMat.friction = 0f;

        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            if (!spriteRenderer.name.Contains("Rock"))
            {
                continue;
            }

            // Xóa collider / effector cũ
            foreach (Collider2D old in spriteRenderer.GetComponents<Collider2D>())
                Destroy(old);
            PlatformEffector2D oldEffector = spriteRenderer.GetComponent<PlatformEffector2D>();
            if (oldEffector != null) Destroy(oldEffector);

            Vector2 spriteSize = spriteRenderer.sprite != null
                ? spriteRenderer.sprite.bounds.size
                : Vector2.one;

            // Solid collider — va chạm vật lý thật, player sẽ nẩy lên khi chạm đá
            BoxCollider2D col = spriteRenderer.gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.sharedMaterial = bouncyMat;
            col.size = new Vector2(spriteSize.x * 0.82f, spriteSize.y * 0.72f);
            col.offset = Vector2.zero;
        }
    }



    private void StyleGameOverPanel()
    {
        if (isGameOverPanelStyled || gameOverPanel == null)
        {
            return;
        }

        RectTransform panel = gameOverPanel.GetComponent<RectTransform>();
        if (panel == null)
        {
            return;
        }

        Stretch(panel);

        Image overlay = gameOverPanel.GetComponent<Image>();
        if (overlay != null)
        {
            overlay.sprite = null;
            overlay.color = new Color(0.03f, 0.08f, 0.12f, 0.74f);
        }

        RectTransform card = EnsureImage("GameOverCard", panel, new Color(0.86f, 0.96f, 1f, 0.96f));
        card.SetAsFirstSibling();
        card.anchorMin = new Vector2(0.5f, 0.5f);
        card.anchorMax = new Vector2(0.5f, 0.5f);
        card.pivot = new Vector2(0.5f, 0.5f);
        card.anchoredPosition = new Vector2(0f, 4f);
        card.sizeDelta = new Vector2(700f, 460f);

        Shadow cardShadow = card.gameObject.GetComponent<Shadow>();
        if (cardShadow == null)
        {
            cardShadow = card.gameObject.AddComponent<Shadow>();
        }
        cardShadow.effectColor = new Color(0f, 0.04f, 0.08f, 0.45f);
        cardShadow.effectDistance = new Vector2(0f, -12f);

        RectTransform topAccent = EnsureImage("TopAccent", card, new Color(0.04f, 0.34f, 0.54f, 1f));
        topAccent.anchorMin = new Vector2(0.5f, 1f);
        topAccent.anchorMax = new Vector2(0.5f, 1f);
        topAccent.pivot = new Vector2(0.5f, 1f);
        topAccent.anchoredPosition = Vector2.zero;
        topAccent.sizeDelta = new Vector2(700f, 20f);

        RectTransform snowLine = EnsureImage("SnowLine", card, new Color(1f, 1f, 1f, 0.9f));
        snowLine.anchorMin = new Vector2(0.5f, 1f);
        snowLine.anchorMax = new Vector2(0.5f, 1f);
        snowLine.pivot = new Vector2(0.5f, 1f);
        snowLine.anchoredPosition = new Vector2(0f, -20f);
        snowLine.sizeDelta = new Vector2(700f, 6f);

        TMP_Text title = FindText(gameOverPanel.transform, "TitleText");
        if (title != null)
        {
            title.transform.SetParent(card, false);
            title.text = "RUN ENDED";
            SetTextColor(title, new Color(0.04f, 0.20f, 0.30f, 1f));
            title.fontSize = 78f;
            title.fontStyle = FontStyles.Normal;
            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 42f;
            title.fontSizeMax = 78f;
            title.enableWordWrapping = false;
            ResetTextBox(title);
            SetRect(title.rectTransform, new Vector2(0f, 145f), new Vector2(600f, 86f));
        }

        TMP_Text hint = EnsureText("GameOverHint", card, "Try a cleaner landing angle.", 32f);
        SetTextColor(hint, new Color(0.18f, 0.35f, 0.42f, 1f));
        hint.fontStyle = FontStyles.Normal;
        hint.enableAutoSizing = true;
        hint.fontSizeMin = 22f;
        hint.fontSizeMax = 32f;
        ResetTextBox(hint);
        SetRect(hint.rectTransform, new Vector2(0f, 82f), new Vector2(560f, 44f));

        if (gameOverScoreText != null)
        {
            gameOverScoreText.transform.SetParent(card, false);
            SetTextColor(gameOverScoreText, new Color(1f, 1f, 1f, 1f));
            gameOverScoreText.fontSize = 54f;
            gameOverScoreText.fontStyle = FontStyles.Normal;
            gameOverScoreText.alignment = TextAlignmentOptions.Center;
            gameOverScoreText.enableAutoSizing = true;
            gameOverScoreText.fontSizeMin = 30f;
            gameOverScoreText.fontSizeMax = 54f;
            gameOverScoreText.enableWordWrapping = false;
            ResetTextBox(gameOverScoreText);
            SetRect(gameOverScoreText.rectTransform, new Vector2(0f, 8f), new Vector2(420f, 68f));

            RectTransform scorePlate = EnsureImage("ScorePlate", card, new Color(0.03f, 0.31f, 0.49f, 1f));
            scorePlate.SetSiblingIndex(gameOverScoreText.transform.GetSiblingIndex());
            SetRect(scorePlate, new Vector2(0f, 8f), new Vector2(470f, 82f));
            Shadow scoreShadow = scorePlate.gameObject.GetComponent<Shadow>();
            if (scoreShadow == null)
            {
                scoreShadow = scorePlate.gameObject.AddComponent<Shadow>();
            }
            scoreShadow.effectColor = new Color(0f, 0.08f, 0.12f, 0.35f);
            scoreShadow.effectDistance = new Vector2(0f, -5f);
            gameOverScoreText.transform.SetAsLastSibling();
        }

        Button restartButton = FindButton(gameOverPanel.transform, "Restart");
        Button menuButton = FindButton(gameOverPanel.transform, "BackToMenu");
        StyleButton(restartButton, card, new Vector2(-155f, -112f), new Color(0.96f, 0.58f, 0.06f, 1f), "RETRY");
        StyleButton(menuButton, card, new Vector2(155f, -112f), new Color(0.03f, 0.28f, 0.40f, 1f), "MENU");

        TMP_Text footer = EnsureText("GameOverFooter", card, "SPACE / CLICK TO TRY AGAIN", 24f);
        SetTextColor(footer, new Color(0.17f, 0.36f, 0.44f, 0.85f));
        footer.fontStyle = FontStyles.Normal;
        ResetTextBox(footer);
        SetRect(footer.rectTransform, new Vector2(0f, -190f), new Vector2(560f, 34f));

        isGameOverPanelStyled = true;
    }

    private static void StyleButton(Button button, RectTransform parent, Vector2 position, Color color, string label)
    {
        if (button == null)
        {
            return;
        }

        button.transform.SetParent(parent, false);
        RectTransform rect = button.GetComponent<RectTransform>();
        SetRect(rect, position, new Vector2(250f, 72f));

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }

        Shadow shadow = button.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = button.gameObject.AddComponent<Shadow>();
        }
        shadow.effectColor = new Color(0f, 0.06f, 0.10f, 0.32f);
        shadow.effectDistance = new Vector2(0f, -5f);

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
        colors.selectedColor = color;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            text.text = label;
            SetTextColor(text, new Color(1f, 1f, 1f, 1f));
            text.fontSize = 38f;
            text.fontStyle = FontStyles.Normal;
            text.alignment = TextAlignmentOptions.Center;
            text.enableAutoSizing = true;
            text.fontSizeMin = 24f;
            text.fontSizeMax = 38f;
            text.enableWordWrapping = false;
            ResetTextBox(text);
            Stretch(text.rectTransform);
        }
    }

    private static RectTransform EnsureImage(string name, RectTransform parent, Color color)
    {
        Transform existing = parent.Find(name);
        GameObject target = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        target.transform.SetParent(parent, false);

        Image image = target.GetComponent<Image>();
        if (image == null)
        {
            image = target.AddComponent<Image>();
        }
        image.sprite = null;
        image.color = color;
        image.raycastTarget = false;

        return target.GetComponent<RectTransform>();
    }

    private static TMP_Text EnsureText(string name, RectTransform parent, string text, float fontSize)
    {
        Transform existing = parent.Find(name);
        GameObject target = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        target.transform.SetParent(parent, false);

        TMP_Text label = target.GetComponent<TMP_Text>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        label.enableWordWrapping = false;
        ResetTextBox(label);

        return label;
    }

    private static void ResetTextBox(TMP_Text text)
    {
        text.margin = Vector4.zero;
        text.rectTransform.localScale = Vector3.one;
        text.rectTransform.localRotation = Quaternion.identity;
        text.rectTransform.anchoredPosition3D = new Vector3(
            text.rectTransform.anchoredPosition3D.x,
            text.rectTransform.anchoredPosition3D.y,
            0f
        );
    }

    private static void SetTextColor(TMP_Text text, Color color)
    {
        text.color = color;
        text.faceColor = color;
    }

    private static TMP_Text FindText(Transform root, string name)
    {
        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            if (text.name == name)
            {
                return text;
            }
        }

        return null;
    }

    private static Button FindButton(Transform root, string name)
    {
        Button[] buttons = root.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.name == name)
            {
                return button;
            }
        }

        return null;
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}
