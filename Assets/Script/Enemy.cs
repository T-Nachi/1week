using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int fireInterval = 50;
    public GameObject particle;

    public enum Direction { Up, Down, Left, Right }
    public Direction shootDirection = Direction.Right;

    public float offset;

    bool hitAncher;

    private int fireCounter = 0;

    Transform stage;

    private void Start()
    {
        stage = GameObject.Find("Stage").transform;
    }

    void FixedUpdate()
    {
        fireCounter++;

        if (fireCounter >= fireInterval && !hitAncher)
        {
            Shoot();
            fireCounter = 0;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, 0), stage);

        // ローカル方向で弾を撃つ（ステージの回転を考慮しない）
        Vector2 localDirection = Vector2.right;
        switch (shootDirection)
        {
            case Direction.Up:
                localDirection = Vector2.up;
                break;
            case Direction.Down:
                localDirection = Vector2.down;
                break;
            case Direction.Left:
                localDirection = Vector2.left;
                break;
            case Direction.Right:
                localDirection = Vector2.right;
                break;
        }

        // ローカル方向のまま渡す（弾側で親の回転を使って処理する）
        bullet.GetComponent<EBullet_S>().SetDirection(localDirection);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Arrow")
        {
            hitAncher = true;
            gameObject.tag = "KillEnemy";
        }
        if (hitAncher && collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }


}
