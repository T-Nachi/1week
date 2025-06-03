using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int fireInterval = 50;

    public enum Direction { Up, Down, Left, Right }
    public Direction shootDirection = Direction.Right;

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

        // ���[�J�������Œe�����i�X�e�[�W�̉�]���l�����Ȃ��j
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

        // ���[�J�������̂܂ܓn���i�e���Őe�̉�]���g���ď�������j
        bullet.GetComponent<EBullet_S>().SetDirection(localDirection);
    }
}
