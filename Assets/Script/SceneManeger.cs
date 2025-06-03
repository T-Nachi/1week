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
    public static bool comingFromRetry = false;
    private bool isZooming = false;
    public static bool isRetryRequested = false;

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
            Retry();
        }
    }

    private void SetupPivot()
    {
        if (cameraPivot != null) return;

        // カメラの現在位置を基準にピボット作成
        cameraPivot = new GameObject("CameraPivot");
        cameraPivot.transform.position = mainCamera.transform.position;
        //DontDestroyOnLoad(cameraPivot);

        mainCamera.transform.SetParent(cameraPivot.transform, true); // ワールド座標維持
    }

    public void Retry()
    {
        // シーン内のFeadオブジェクトを探してフェードイン開始
        Fead fade = FindObjectOfType<Fead>();
        if (fade != null)
        {
            fade.StartFadeInAndRetry();
        }
    }

    private IEnumerator RetryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Retry();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 古い cameraPivot が存在したら削除（AudioListener重複回避）
        if (cameraPivot != null)
        {
            Destroy(cameraPivot);
            cameraPivot = null;
        }

        mainCamera = Camera.main;
        player = GameObject.Find("Player");
        goal = GameObject.Find("Goal");
        if (goal != null) goalS = goal.GetComponent<Goal>();

        SetupPivot();
    }


    private IEnumerator DelayedRotateBack()
    {
        yield return null; // 1フレーム待つ
        yield return RotateOverTime(180f, rotationDuration);
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

        float moveDuration = 1.5f;
        float zoomDuration = 1.0f;
        float elapsed = 0f;

        // 1. まずゴールに注視しながらカメラを移動
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            cameraPivot.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // 2. 次に視野を狭めてズームイン（orthographicSizeを縮小）
        elapsed = 0f;
        float startSize = mainCamera.orthographicSize;
        float targetSize = 0.3f; // ← ここで視野の狭さを調整（値を小さくするほどズーム）

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / zoomDuration);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        // 3. 必要なら少し待ってから次シーンへ遷移
        yield return new WaitForSeconds(0.5f);

        // 次のシーンをロード（シーン名を適宜変更してください）
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
