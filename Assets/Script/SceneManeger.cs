using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManeger : MonoBehaviour
{
    public float rotationDuration = 1f;

    private Camera mainCamera;
    private GameObject cameraPivot;
    private GameObject player;
    private GameObject goal;
    private Goal goalS;

    private bool retryScheduled = false;
    private static bool comingFromRetry = false;
    private bool isZooming = false;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        mainCamera = Camera.main;
        player = GameObject.Find("Player");
        goal = GameObject.Find("Goal");
        if (goal != null) goalS = goal.GetComponent<Goal>();

        SetupPivot();

        if (comingFromRetry)
        {
            cameraPivot.transform.rotation = Quaternion.Euler(0, 0, 180f);
            StartCoroutine(DelayedRotateBack());
            comingFromRetry = false;
        }
    }

    void Update()
    {
        if (!isZooming && goalS != null && goalS.isClear)
        {
            isZooming = true;
            StartCoroutine(ZoomToGoalAndLoadNextScene());
        }

        if (!retryScheduled && player == null)
        {
            retryScheduled = true;
            StartCoroutine(RetryAfterDelay(1f));
        }
    }

    private void SetupPivot()
    {
        if (cameraPivot != null) return;

        // カメラの現在位置を基準にピボット作成
        cameraPivot = new GameObject("CameraPivot");
        cameraPivot.transform.position = mainCamera.transform.position;
        DontDestroyOnLoad(cameraPivot);

        mainCamera.transform.SetParent(cameraPivot.transform, true); // ワールド座標維持
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
        comingFromRetry = true;
        Destroy(cameraPivot);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.Find("Player");
        mainCamera = Camera.main;

        goal = GameObject.Find("Goal");
        if (goal != null) goalS = goal.GetComponent<Goal>();

        SetupPivot();
    }

    private IEnumerator DelayedRotateBack()
    {
        yield return null; // 1フレーム待つ
        yield return RotateOverTime(180f, rotationDuration); // 同じ向きに回転
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

    private IEnumerator ZoomToGoalAndLoadNextScene()
    {
        Vector3 startPos = cameraPivot.transform.position;
        Vector3 targetPos = goal.transform.position;
        targetPos.z = startPos.z; // カメラのZ値は維持

        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            cameraPivot.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // シーン遷移（必要に応じて変更）
        SceneManager.LoadScene("NextSceneName");
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
