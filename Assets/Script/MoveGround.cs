using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{

    GameObject stage;
    Stage stageS;

    BoxCollider2D box;

    // Start is called before the first frame update
    void Start()
    {
        stage = GameObject.Find("Stage");
        if (stage != null)
        {
            stageS = stage.GetComponent<Stage>();
        }
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stageS != null)
        {
            if (stageS.isRotating)
            {
                box.isTrigger = true;
            }
            else
            {
                box.isTrigger = false;
            }
        }
    }
}
