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
            StartCoroutine(DelayedRotateBackAndResetFlag());
        }

    }

    private IEnumerator DelayedRotateBackAndResetFlag()
    {
        yield return null;
        yield return RotateOverTime(180f, rotationDuration);
        comingFromRetry = false; // �� ��]���I��������ƂɃt���O��߂��I
    }


    void Update()
    {
        // �N���A���o�F���g���C���͖�����
        if (!isZooming && goalS != null && goalS.isClear && !comingFromRetry)
        {
            isZooming = true;
            StartCoroutine(ZoomToGoalAndLoadNextScene());
        }

        // ���g���C����
        if (!retryScheduled && player == null)
        {
            retryScheduled = true;
            StartCoroutine(RetryAfterDelay(1f));
        }
    }

    private void SetupPivot()
    {
        if (cameraPivot != null) return;

        // �����̃��[���h���W���擾�inearClipPlane �ł͂Ȃ��J�����ʒu����̋����Łj
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.transform.position.z * -1));

        // �s�{�b�g�쐬�i��������]�̎��Ɂj
        cameraPivot = new GameObject("CameraPivot");
        cameraPivot.transform.position = bottomLeft;
        DontDestroyOnLoad(cameraPivot);

        // �J�����̃��[���h�ʒu�ƃs�{�b�g�̍��������[�J���ɕϊ�
        mainCamera.transform.SetParent(cameraPivot.transform);
        mainCamera.transform.localPosition = mainCamera.transform.position - bottomLeft;
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
        // �������Ńs�{�b�g�ʒu�������Ɉړ��i��ʂ̍����j
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        cameraPivot.transform.position = new Vector3(bottomLeft.x, bottomLeft.y, cameraPivot.transform.position.z);

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
        yield return null;
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

        // �S�[���̃��[���h���W�����̂܂܎g���i�e�̉�]�������j
        Vector3 targetPos = goal.transform.position;
        targetPos.z = startPos.z; // Z���W����cameraPivot�ɍ��킹��

        float moveDuration = 1.0f;
        float zoomDuration = 1.0f;

        // �J�����ړ��i���j
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            cameraPivot.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // �Y�[���C���i��������߂�j
        float startSize = mainCamera.orthographicSize;
        float targetSize = 0.3f;
        elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / zoomDuration);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }





    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
