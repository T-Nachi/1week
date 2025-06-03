using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fead : MonoBehaviour
{
    public float duration = 1.5f;
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        gameObject.SetActive(true); // 保険
    }

    void Start()
    {
        // 開始時は comingFromRetry に応じて白か黒でフェードアウト
        if (SceneManeger.comingFromRetry)
        {
            SetColor(Color.black);
            StartCoroutine(FadeOut());
            SceneManeger.comingFromRetry = false;
        }
        else
        {
            SetColor(Color.white);
            StartCoroutine(FadeOut());
        }
    }

    public void StartFadeInAndRetry()
    {
        SetColor(new Color(0f, 0f, 0f, 0f));
        gameObject.SetActive(true);
        StartCoroutine(FadeInAndRetry());
    }

    private void SetColor(Color color)
    {
        color.a = 1f;
        fadeImage.color = color;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color startColor = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Color c = startColor;
            c.a = 1f - t;
            fadeImage.color = c;
            yield return null;
        }
    }

    private System.Collections.IEnumerator FadeInAndRetry()
    {
        float elapsed = 0f;
        Color c = new Color(0f, 0f, 0f, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            c.a = t;
            fadeImage.color = c;
            yield return null;
        }

        SceneManeger.comingFromRetry = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
