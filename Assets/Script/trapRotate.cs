using UnityEngine;

public class trapRotate : MonoBehaviour
{
    public float rotationSpeed = 90f; // 1�b��90�x��]

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
