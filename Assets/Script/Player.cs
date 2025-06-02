using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 inputDir;
    public Vector2 aimDir;

    //フラグ
    bool jumpTrigger;
    bool isGround;
    bool isAim;
    bool isShoot;
    public bool isRotate;

    //パラメータ
    public float moveSpeed;
    public float jumpPower;
    public float rayLength = 0.3f;
    public float aimoffset;
    float defGravity;

    //他オブジェクト情報
    public LayerMask groundLayer;
    public GameObject AnchorPrefab;
    GameObject anchor;
    Transform stage;

    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        stage = GameObject.Find("Stage").transform;
        rb = GetComponent<Rigidbody2D>();
        defGravity = rb.gravityScale;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        GroundInfo();
        if (!isShoot)
        {
            if (isGround && inputDir == Vector2.down) inputDir = Vector2.zero;
            rb.gravityScale = defGravity;
            if (inputDir != Vector2.zero) aimDir = inputDir;
            if (isAim) Aim();
            Move();
            Jump();
        }
        else
        {
            Shoot();
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(inputDir.x * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        if (isGround && jumpTrigger)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
    }


    private void Aim()
    {

        if (!isGround)
            Time.timeScale = 0.3f;
        else
            Time.timeScale = 1f;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // 回転を設定
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);

        if (anchor == null) anchor = Instantiate(AnchorPrefab, transform.position + (Vector3)aimDir * aimoffset, rotation, stage);
        else
        {
            anchor.transform.position = transform.position + (Vector3)aimDir * aimoffset;
            anchor.transform.rotation = rotation;
        }

    }
    private void Shoot()
    {
        rb.gravityScale = 0;
        if (anchor != null)
        {
            Ancher ancherScript = anchor.GetComponent<Ancher>();
            if (ancherScript != null)
            {
                ancherScript.player = this.transform;
                ancherScript.isShoot = true;
                isRotate = ancherScript.isRotate;
            }
        }
        else
        {
            if (isRotate)
            {
            }
            isShoot = false;
        }
    }

    private void GroundInfo()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        isGround = hit.collider != null;
        Color rayColor = isGround ? Color.red : Color.green;
        Debug.DrawRay(transform.position, Vector2.down.normalized * rayLength * 10, rayColor);
    }

    public void Input_Move(InputAction.CallbackContext context)
    {

        var v = context.ReadValue<Vector2>();
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
        {
            inputDir.x = v.x > 0 ? 1 : -1;
            inputDir.y = 0;
        }
        else if (Mathf.Abs(v.y) > Mathf.Abs(inputDir.x))
        {
            inputDir.y = v.y > 0 ? 1 : -1;
            inputDir.x = 0;
        }
        else
        {
            inputDir = Vector2.zero;
        }
    }
    public void Input_Jump(InputAction.CallbackContext context)
    {
        bool Trigger = context.performed;
        jumpTrigger = Trigger;
    }
    public void Input_Fire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isAim = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            rb.velocity = Vector2.zero;
            Time.timeScale = 1f;
            isShoot = true;
            isAim = false;
        }
    }




}
