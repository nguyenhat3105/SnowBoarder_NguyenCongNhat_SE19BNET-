using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteAlways]
public class MainCamera : MonoBehaviour
{
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject levelScreen;
    [SerializeField] private GameObject HighScorePanel;
    public TMP_Text[] scoreTexts;

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void Options()
    {

    }
    public void Levels()
    {
        if (mainScreen != null && levelScreen != null)
        {
            mainScreen.SetActive(false);
            levelScreen.SetActive(true);
        }
    }

    public void ShowHighScores()
    {
        HighScorePanel.SetActive(true);

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            int score = PlayerPrefs.GetInt("HighScore" + i, 0);
            scoreTexts[i].text = score.ToString();
        }
    }

    public void HideHighScores()
    {
        HighScorePanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Start()
    {
        DisableMenuSnowfall();
        StyleMainMenu();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            StyleMainMenu();
        }
    }

    private void OnEnable()
    {
        DisableMenuSnowfall();
        if (!Application.isPlaying)
        {
            StyleMainMenu();
        }
    }

    void Update()
    {

    }

    private void StyleMainMenu()
    {
        if (mainScreen == null)
        {
            return;
        }

        // Chỉ cài đặt tiêu đề game — màu sắc và kích thước button được chỉnh trong Unity Inspector
        TMP_Text gameName = FindText(mainScreen.transform, "GameName");
        if (gameName != null)
        {
            gameName.text = "SNOW BOARDING";
        }
    }

    private static void DisableMenuSnowfall()
    {
        ParticleSystem[] particles = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        GameObject cameraSnowfall = GameObject.Find("Camera Snowfall");
        if (cameraSnowfall == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(cameraSnowfall);
        }
        else
        {
            DestroyImmediate(cameraSnowfall);
        }
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

    private static void SetTextColor(TMP_Text text, Color color)
    {
        text.color = color;
        text.faceColor = color;
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

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
