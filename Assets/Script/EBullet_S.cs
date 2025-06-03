using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBullet_S : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 baseDirection = Vector2.right; // SetDirectionで設定されるローカル方向
    private Rigidbody2D rb;
    private Transform parent;

    bool isStop;

    private bool directionSet = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!directionSet && transform.parent != null)
        {
            parent = transform.parent;
            Vector2 worldDir = parent.rotation * baseDirection;
            SetDirection(worldDir);
        }
        else if (transform.parent != null)
        {
            parent = transform.parent;
        }
    }

    void FixedUpdate()
    {
        if (parent != null)
        {
            // 親の回転を反映したワールド方向に変換
            Vector2 worldDir = parent.rotation * baseDirection;
            if (!isStop)
            {
                rb.velocity = worldDir.normalized * speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = baseDirection.normalized * speed;
        }
    }

    public void SetDirection(Vector2 direction)
    {
        baseDirection = direction.normalized;
        directionSet = true;

        // 親の回転を使ってワールド方向に変換（進行方向）
        Vector2 worldDirection = baseDirection;
        if (transform.parent != null)
        {
            worldDirection = transform.parent.rotation * baseDirection;
        }

        // ワールド方向に基づいた見た目（角度）にする
        float angle = Mathf.Atan2(worldDirection.y, worldDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Arrow")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Arrow")
        {
            isStop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Arrow")
        {
            isStop = false;
        }
    }

}
