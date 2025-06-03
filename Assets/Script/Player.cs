using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    Vector2 inputDir;
    public Vector2 aimDir;

    //フラグ
    bool jumpTrigger;
    public bool isGround;
    bool isAim;
    public bool isShoot;
    public bool isRotate;

    //パラメータ
    public float moveSpeed;
    public float jumpPower;
    public float rayLength = 0.3f;
    public float aimoffset;
    float defGravity;

    //他オブジェクト情報
    private Volume volume;
    Vignette vignetteS;
    public LayerMask groundLayer;
    public GameObject AnchorPrefab;
    GameObject anchor;
    GameObject stage;
    Stage stageS;
    private Rigidbody2D standingOnRb;


    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("Vinnet").GetComponent<Volume>();
        if (volume != null) { vignetteS = volume.GetComponent<Vignette>(); }
        stage = GameObject.Find("Stage");
        if (stage != null)
        {
            stageS = stage.GetComponent<Stage>();
        }
        rb = GetComponent<Rigidbody2D>();
        defGravity = rb.gravityScale;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (stageS != null)
        {
            if (!stageS.isRotating)
            {
                GroundInfo();

                if (standingOnRb != null)
                {
                    // 動く床（弾）の速度をプレイヤーの位置に反映（Time.fixedDeltaTimeで補正）
                    rb.position += standingOnRb.velocity * Time.fixedDeltaTime;
                }

                if (!isShoot)
                {
                    if (isGround && inputDir == Vector2.down) inputDir = Vector2.zero;
                    rb.gravityScale = defGravity;
                    if (inputDir != Vector2.zero) aimDir = inputDir;
                    if (isAim) Aim();
                    else { if (vignetteS != null) { vignetteS.triggerVignette = false; } }
                    Move();
                    Jump();
                }
                else
                {
                    Shoot();
                }
            }
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
        {
            Time.timeScale = 0.2f;
            if (vignetteS != null) { vignetteS.triggerVignette = true; }
        }
        else
        {
            Time.timeScale = 1f;
            if (vignetteS != null) { vignetteS.triggerVignette = false; }
        }
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // 回転を設定
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);

        if (anchor == null) anchor = Instantiate(AnchorPrefab, transform.position + (Vector3)aimDir * aimoffset, rotation, stage.transform);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EBullet") && collision.transform.position.y < transform.position.y)
        {
            standingOnRb = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower / 2);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EBullet") && collision.transform.position.y < transform.position.y)
        {
            standingOnRb = null;
        }
    }


}
