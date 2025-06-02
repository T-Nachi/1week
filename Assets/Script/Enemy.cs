using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int fireInterval = 50;

    public enum Direction { Up, Down, Left, Right }
    public Direction shootDirection = Direction.Right; // Inspector‚Å‘I‚×‚é

    public float offset; 

    private int fireCounter = 0;

    Transform stage;
    private void Start()
    {
        stage = GameObject.Find("Stage").transform;
    }

    void FixedUpdate()
    {
        fireCounter++;

        if (fireCounter >= fireInterval)
        {
            Shoot();
            fireCounter = 0;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, stage);
        Vector2 dir = Vector2.right;

        switch (shootDirection)
        {
            case Direction.Up:
                dir = Vector2.up;
                break;
            case Direction.Down:
                dir = Vector2.down;
                break;
            case Direction.Left:
                dir = Vector2.left;
                break;
            case Direction.Right:
                dir = Vector2.right;
                break;
        }

        bullet.GetComponent<EBullet_S>().SetDirection(dir);
    }
}
