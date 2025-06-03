using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManeger : MonoBehaviour
{
    public float rotationDuration = 1f;

    private GameObject cameraPivot;
    private Camera mainCamera;
    private GameObject player;

    private bool retryScheduled = false;
    private bool rotatingBack = false;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    void Initialize()
    {
        player = GameObject.Find("Player");
        mainCamera = Camera.main;

        SetupPivot();
    }

    void Update()
    {
        if (!retryScheduled && player == null)
        {
            retryScheduled = true;
            StartCoroutine(RetryAfterDelay(1f));
        }
    }

    void SetupPivot()
    {
        if (cameraPivot == null)
        {
            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));

            cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.position = bottomLeft;
            DontDestroyOnLoad(cameraPivot);
        }

        mainCamera.transform.SetParent(cameraPivot.transform);
    }

    public void Retry()
    {
        StartCoroutine(RetryWithRotation());
    }

    IEnumerator RetryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Retry();
    }

    IEnumerator RetryWithRotation()
    {
        yield return RotateOverTime(180f, rotationDuration);

        rotatingBack = true;

        // カメラ親子関係を切り離して破棄
        mainCamera.transform.SetParent(null);
        Destroy(cameraPivot);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 古いカメラが残っている場合は削除（保険）
        var cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        if (cameras.Length > 1)
        {
            foreach (var cam in cameras)
            {
                if (cam != Camera.main.gameObject)
                    Destroy(cam);
            }
        }

        player = GameObject.Find("Player");
        mainCamera = Camera.main;

        SetupPivot();

        if (rotatingBack)
        {
            cameraPivot.transform.rotation = Quaternion.Euler(0, 0, 180f);
            StartCoroutine(DelayedRotateBack());
            rotatingBack = false;
        }
    }

    IEnumerator DelayedRotateBack()
    {
        yield return null;
        yield return RotateOverTime(-180f, rotationDuration);
    }

    IEnumerator RotateOverTime(float angle, float duration)
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

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
