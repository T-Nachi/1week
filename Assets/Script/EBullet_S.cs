using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBullet_S : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 baseDirection = Vector2.right; // SetDirection�Őݒ肳��郍�[�J������
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
            // �e�̉�]�𔽉f�������[���h�����ɕϊ�
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

        // �e�̉�]���g���ă��[���h�����ɕϊ��i�i�s�����j
        Vector2 worldDirection = baseDirection;
        if (transform.parent != null)
        {
            worldDirection = transform.parent.rotation * baseDirection;
        }

        // ���[���h�����Ɋ�Â��������ځi�p�x�j�ɂ���
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
