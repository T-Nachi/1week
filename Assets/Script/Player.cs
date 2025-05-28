using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 inputDir;
    bool jumpTrigger;
    bool isGround;

    public float moveSpeed;

    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector2(inputDir.x * moveSpeed, rb.velocity.y);
    }


    public void Input_Move(InputAction.CallbackContext context)
    {

        var v = context.ReadValue<Vector2>();
        inputDir.x = v.x > 0 ? 1 : -1;
        inputDir.y = 0;
    }
    public void Input_Jump(InputAction.CallbackContext context)
    {
        bool Trigger = context.performed;
        jumpTrigger = Trigger;
    }
    public void Input_Fire(InputAction.CallbackContext context)
    {
        bool Trigger = context.performed;
    }



}
