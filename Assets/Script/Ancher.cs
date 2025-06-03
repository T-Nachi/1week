using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ancher : MonoBehaviour
{
    public Transform player;
    public float speed = 10f;
    public float maxDistance = 7f;
    public float pullSpeed = 10f;
    public bool isShoot = false;
    public bool isRotate;
    bool isHit;

    bool isMove;
    private Vector2 startPosition;
    private Vector2 direction;
    private bool hitSomething = false;
    private bool returning = false;
    private Rigidbody2D rb;
    int count;


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
                // 戻る処理へ移行
                StartCoroutine(ReturnToPlayer());
            }
        }

        if (isHit && isShoot)
        {
            hitSomething = true;
            isRotate = true;
            rb.velocity = Vector2.zero;
            StartCoroutine(PullPlayerToPoint());
            isHit = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitSomething && other.tag != "Player")
        {
            isHit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!hitSomething && collision.tag != "Player")
        {
            isHit = false;
        }
    }

    private IEnumerator PullPlayerToPoint()
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb == null) yield break;

        int frameCount = 0;
        int maxFrames = Mathf.RoundToInt(0.5f / Time.fixedDeltaTime);

        if (player != null)
        {
            while (Vector2.Distance(player.position, transform.position) > 0.1f)
            {
                Vector2 pullDir = (transform.position - player.position).normalized;
                playerRb.velocity = pullDir * pullSpeed;

                frameCount++;
                if (frameCount > maxFrames)
                {
                    Debug.Log("時間切れでアンカーを破壊");
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }
        playerRb.velocity = Vector2.zero;
        Destroy(gameObject);

    }

    private IEnumerator ReturnToPlayer()
    {
        returning = true;
        if (player != null)
        {
            while (Vector2.Distance(transform.position, player.position) > 0.5f)
            {
                Vector2 dir = ((Vector2)player.position - rb.position).normalized;
                rb.velocity = dir * speed;
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
