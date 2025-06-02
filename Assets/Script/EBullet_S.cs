using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBullet_S : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveDirection = Vector2.right;
    private Rigidbody2D rb;

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.velocity = moveDirection * speed;
    }
}
