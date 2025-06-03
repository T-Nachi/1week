using UnityEngine;
using UnityEngine.UI;

public class Fead : MonoBehaviour
{
    public float duration = 1.5f;
    private Image fadeImage;


    void Start()
    {
        // リトライから来た場合はスキップ
        if (SceneManeger.comingFromRetry)
        {
            gameObject.SetActive(false);
            return;
        }

        fadeImage = GetComponent<Image>();
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Color c = fadeImage.color;
            c.a = 1f - t;
            fadeImage.color = c;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
