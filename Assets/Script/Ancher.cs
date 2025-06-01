using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ancher : MonoBehaviour
{
    public Transform player;
    public float speed = 10f;
    public float maxDistance = 7f;
    public float pullSpeed = 10f;
    public bool isShoot = false;
    public bool isRotate;


    bool isMove;
    private Vector2 startPosition;
    private Vector2 direction;
    private bool hitSomething = false;
    private bool returning = false;
    private Rigidbody2D rb;

    void Start()
    {
        isRotate = false;
        isShoot = false;
        isMove = false;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!isShoot)
        {

            startPosition = transform.position;
        }
        else
        {
            if (isMove == false)
            {
                float z = transform.eulerAngles.z + 90;

                float rad = z * Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                rb.velocity = direction * speed;
                isMove = true;
            }

            if (!hitSomething && !returning && Vector2.Distance(startPosition, transform.position) >= maxDistance)
            {
                // ñﬂÇÈèàóùÇ÷à⁄çs
                StartCoroutine(ReturnToPlayer());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isShoot)
        {
            if (!hitSomething && other.gameObject != player.gameObject)
            {
                hitSomething = true;
                isRotate = true;
                rb.velocity = Vector2.zero;
                StartCoroutine(PullPlayerToPoint());
            }
        }
    }

    private IEnumerator PullPlayerToPoint()
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb == null) yield break;

        while (Vector2.Distance(player.position, transform.position) > 0.5f)
        {
            Vector2 pullDir = (transform.position - player.position).normalized;
            playerRb.velocity = pullDir * pullSpeed;
            yield return null;
        }

        playerRb.velocity = Vector2.zero;
        Destroy(gameObject);
    }

    private IEnumerator ReturnToPlayer()
    {
        returning = true;
        while (Vector2.Distance(transform.position, player.position) > 0.5f)
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.velocity = dir * speed;
            yield return null;
        }

        Destroy(gameObject);
    }
}
