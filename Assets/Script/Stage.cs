using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    GameObject player;
    Player playerS;
    bool canRotate;

    public float rotationSpeed = 180f; // 回転速度（度/秒）

    TrailRenderer pTrail;

    Quaternion targetRotation;
    public bool isRotating = false;

    private void Start()
    {
        player = GameObject.Find("Player");
        if (player != null)
        {
            pTrail = player.GetComponent<TrailRenderer>();
            playerS = player.GetComponent<Player>();
        }
        targetRotation = transform.rotation; // 初期回転
    }

    void Update()
    {
        if (playerS != null)
        {
            if (!playerS.isRotate) canRotate = true;

            if (canRotate && playerS.isRotate)
            {
                StartRotation(playerS.aimDir);
                canRotate = false;
            }
        }

        if (isRotating)
        {
            if (pTrail != null) pTrail.enabled = false;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
        else
        {
            if (pTrail != null) pTrail.enabled = true;
        }
    }

    void StartRotation(Vector2 dir)
    {
        targetRotation = GetTargetRotation(dir);
        isRotating = true;
    }

    Quaternion GetTargetRotation(Vector2 dir)
    {
        float currentZ = transform.eulerAngles.z;
        float newZ = currentZ;

        if (dir == Vector2.right)
        {
            newZ -= 90f;
        }
        else if (dir == Vector2.left)
        {
            newZ += 90f;
        }
        else if (dir == Vector2.down)
        {
            newZ += 0f;
        }
        else if (dir == Vector2.up)
        {
            newZ += 180f;
        }

        // 角度を0〜360に収める（省略しても動くが安定のため）
        newZ = (newZ + 360f) % 360f;

        return Quaternion.Euler(0, 0, newZ);
    }
}
