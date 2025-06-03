using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    GameObject player;
    Player playerS;

    public string targetName = "Enemy";
    public bool allEnemiesDefeated = false;

    bool isAvtive;
    public bool isClear;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        if (player != null)
        {
            playerS = player.GetComponent<Player>();
        }
        isAvtive = false;
        isClear = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int count = 0;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.name.Contains(targetName))
            {
                count++;
            }
        }

        allEnemiesDefeated = (count == 0);

        if (playerS != null) {
            if (allEnemiesDefeated && playerS.isGround)
            {
                isAvtive = true;
            }
            else
            {
                isAvtive = false;
            }
        }

        if (isAvtive)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
        }

        Debug.Log(transform.position);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isAvtive)
        {
            isClear = true;
        }
    }

    public Vector3 GetPositionIgnoringParentRotation()
    {
        if (transform.parent == null)
            return transform.position;

        Transform parent = transform.parent;
        Vector3 localPos = transform.localPosition;
        Vector3 scaledLocalPos = Vector3.Scale(localPos, parent.localScale);
        Vector3 unrotatedLocalPos = Quaternion.Inverse(parent.rotation) * scaledLocalPos;
        return parent.position + unrotatedLocalPos;
    }
}
