using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManeger : MonoBehaviour
{
    public float rotationDuration = 1f;

    private Camera mainCamera;
    private GameObject cameraPivot;
    private GameObject player;

    private bool rotatingBack = false;
    private bool retryScheduled = false;
    private static bool comingFromRetry = false;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        mainCamera = Camera.main;
        player = GameObject.Find("Player");

        SetupPivot();

        if (comingFromRetry)
        {
            // カメラを180°に設定しておき、後で戻す
            cameraPivot.transform.rotation = Quaternion.Euler(0, 0, 180f);
            StartCoroutine(DelayedRotateBack());
            comingFromRetry = false;
        }
    }

    void Update()
    {
        if (!retryScheduled && player == null)
        {
            retryScheduled = true;
            StartCoroutine(RetryAfterDelay(1f));
        }
    }

    private void SetupPivot()
    {
        if (cameraPivot != null) return;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));

        cameraPivot = new GameObject("CameraPivot");
        cameraPivot.transform.position = bottomLeft;
        DontDestroyOnLoad(cameraPivot);

        mainCamera.transform.SetParent(cameraPivot.transform);
    }

    public void Retry()
    {
        StartCoroutine(RetryWithRotation());
    }

    private IEnumerator RetryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Retry();
    }

    private IEnumerator RetryWithRotation()
    {
        yield return RotateOverTime(180f, rotationDuration);

        // 次のシーンで逆回転するフラグを立てる
        comingFromRetry = true;

        // カメラとピボット削除（次シーンで新たに作るため）
        Destroy(cameraPivot);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.Find("Player");
        mainCamera = Camera.main;

        SetupPivot();

        // comingFromRetry で回転済みなのでここでは回さない
    }

    private IEnumerator DelayedRotateBack()
    {
        yield return null; // 1フレーム待機してから回転開始
        yield return RotateOverTime(-180f, rotationDuration);
    }

    private IEnumerator RotateOverTime(float angle, float duration)
    {
        Quaternion startRot = cameraPivot.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, angle);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            cameraPivot.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        cameraPivot.transform.rotation = endRot;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
