using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwich : MonoBehaviour
{
    GameObject stage;
    Stage stageS;

    Rigidbody2D rb;
    float defGravity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defGravity = rb.gravityScale;
        stage = GameObject.Find("Stage");
        if (stage != null)
        {
            stageS = stageS.GetComponent<Stage>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (stageS != null)
        {
            if (stageS.isRotating)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
            }
            else rb.gravityScale = defGravity;
        }
    }
}
