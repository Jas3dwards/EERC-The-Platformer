// JLMD 
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement ActivePlayer { get; private set; }

    [SerializeField] private PlayerTunableStats playerStats;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float flySpeed = 12f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float triplePressWindow = 1f;
    [SerializeField] private float defaultDamage = 10f;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private bool noclipActive;
    private int pPressCount;
    private float lastPressTime;
    private int oPressCount;
    private float lastOPressTime;
    private int iPressCount;
    private float lastIPressTime;
    private float defaultGravityScale;
    private static readonly Vector3 DefaultScale = new Vector3(6f, 6f, 1f);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        defaultGravityScale = body.gravityScale;
        EnsurePlayerStats();
        ActivePlayer = this;
        RuntimeTuningUI.RegisterStats(playerStats);
    }

    private void OnEnable()
    {
        ActivePlayer = this;
        if (playerStats == null)
            EnsurePlayerStats();
        RuntimeTuningUI.RegisterStats(playerStats);
    }

    private void OnDisable()
    {
        if (ActivePlayer == this)
            ActivePlayer = null;
    }

    private void OnDestroy()
    {
        if (ActivePlayer == this)
            ActivePlayer = null;
    }

    public PlayerTunableStats CurrentStats
    {
        get
        {
            EnsurePlayerStats();
            return playerStats;
        }
    }
    private void Update()
    {
        bool canProcessDebugInput = ActivePlayer == null || ActivePlayer == this;
        if (canProcessDebugInput)
        {
            HandleNoclipToggle();
            HandleTuningToggle();
            HandleHitboxToggle();
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float facingDirection = Mathf.Sign(transform.localScale.x);
        if (Mathf.Approximately(facingDirection, 0f))
            facingDirection = 1f;
        if (horizontalInput > 0.01f)
            facingDirection = 1f;
        else if (horizontalInput < -0.01f)
            facingDirection = -1f;
        transform.localScale = GetDirectionalScale(facingDirection);

        if (noclipActive)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float flySpeedValue = GetFlySpeed();
            Vector2 flyVelocity = new Vector2(horizontalInput * flySpeedValue, verticalInput * flySpeedValue);
            body.linearVelocity = flyVelocity;
            anim.SetBool("run", flyVelocity.sqrMagnitude > 0.01f);
            anim.SetBool("grounded", false);
        }
        else
        {
            float moveSpeed = GetMoveSpeed();
            if (IsSprinting(horizontalInput))
                moveSpeed *= GetSprintMultiplier();

            body.linearVelocity = new Vector2(horizontalInput * moveSpeed, body.linearVelocity.y);
            body.gravityScale = GetGravityScale();

            if (Input.GetKey(KeyCode.Space) && isGrounded()) // Fixed method call
                Jump();

            anim.SetBool("run", horizontalInput != 0);
            anim.SetBool("grounded", isGrounded()); // Fixed method call
        }
    }
    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, GetJumpForce());
        anim.SetTrigger("jump");
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "WindRight")
        {
            body.AddForce(350f * Vector2.right);
            Debug.Log("Player Moved");
        }
        if (collision.transform.tag == "WindLeft")
        {
            body.AddForce(350f * Vector2.left);
            Debug.Log("Player Moved");
        }
        if (collision.transform.tag == "WindUp")
        {
            body.AddForce(50f * Vector2.up);
            Debug.Log("Player Moved");
        }
        if (collision.transform.tag == "WindDown")
        {
            body.AddForce(50f * Vector2.down);
            Debug.Log("Player Moved");
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D raycastHit1 = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.right, 0.1f, groundLayer);
        RaycastHit2D raycastHit2 = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.left, 0.1f, groundLayer);
        return raycastHit.collider != null; // Return true if the raycast hits something
        return raycastHit1.collider != null; // Return true if the raycast hits something
        return raycastHit2.collider != null; // Return true if the raycast hits something
    }
    private bool isWalledRight()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.right, 0.1f, groundLayer);
        return raycastHit.collider != null; // Return true if the raycast hits something
    }
    private bool isWalledLeft()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.left, 0.1f, groundLayer);
        return raycastHit.collider != null; // Return true if the raycast hits something
    }

    private void HandleNoclipToggle()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.time - lastPressTime > triplePressWindow)
                pPressCount = 0;

            lastPressTime = Time.time;
            pPressCount++;

            if (pPressCount >= 3)
            {
                ToggleNoclipMode();
                pPressCount = 0;
            }
        }
        else if (Time.time - lastPressTime > triplePressWindow)
        {
            pPressCount = 0;
        }
    }

    private void HandleTuningToggle()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (Time.time - lastOPressTime > triplePressWindow)
                oPressCount = 0;

            lastOPressTime = Time.time;
            oPressCount++;

            if (oPressCount >= 3)
            {
                RuntimeTuningUI.Toggle(playerStats);
                oPressCount = 0;
            }
        }
        else if (Time.time - lastOPressTime > triplePressWindow)
        {
            oPressCount = 0;
        }
    }

    private void HandleHitboxToggle()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Time.time - lastIPressTime > triplePressWindow)
                iPressCount = 0;

            lastIPressTime = Time.time;
            iPressCount++;

            if (iPressCount >= 3)
            {
                CollisionHitboxVisualizer.Toggle();
                iPressCount = 0;
            }
        }
        else if (Time.time - lastIPressTime > triplePressWindow)
        {
            iPressCount = 0;
        }
    }

    private void ToggleNoclipMode()
    {
        noclipActive = !noclipActive;
        if (noclipActive)
        {
            boxCollider.enabled = false;
            body.gravityScale = 0f;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            boxCollider.enabled = true;
            body.gravityScale = GetGravityScale();
        }
    }

    private float GetMoveSpeed() => playerStats != null ? playerStats.MoveSpeed : speed;

    private float GetSprintMultiplier() => playerStats != null ? playerStats.SprintMultiplier : sprintMultiplier;

    private float GetFlySpeed() => playerStats != null ? playerStats.FlySpeed : flySpeed;

    private float GetJumpForce() => playerStats != null ? playerStats.JumpForce : jumpForce;

    private float GetGravityScale() => playerStats != null ? playerStats.GravityScale : defaultGravityScale;

    private Vector3 GetDirectionalScale(float direction)
    {
        Vector3 baseScale = playerStats != null ? playerStats.PlayerSize : DefaultScale;
        float sign = Mathf.Approximately(direction, 0f) ? 1f : Mathf.Sign(direction);
        float x = Mathf.Abs(baseScale.x) * sign;
        return new Vector3(x, baseScale.y, baseScale.z);
    }

    private void EnsurePlayerStats()
    {
        if (playerStats != null)
            return;

        Vector3 size = transform != null ? transform.localScale : DefaultScale;
        if (size == Vector3.zero)
            size = DefaultScale;

        size = new Vector3(
            Mathf.Max(0.1f, Mathf.Abs(size.x)),
            Mathf.Max(0.1f, Mathf.Abs(size.y)),
            Mathf.Max(0.1f, Mathf.Abs(size.z))
        );

        float projectileSpeed = 8f;
        float meleeRange = 1f;
        float meleeDuration = 0.2f;
        PlayerAttack attackComponent = GetComponent<PlayerAttack>();
        if (attackComponent != null)
        {
            projectileSpeed = Mathf.Max(0f, attackComponent.projectileSpeed);
            meleeRange = Mathf.Max(0.1f, attackComponent.attackRange);
            meleeDuration = Mathf.Max(0.01f, attackComponent.attackDuration);
        }

        playerStats = PlayerTunableStats.CreateRuntimeInstance(
            speed,
            sprintMultiplier,
            flySpeed,
            jumpForce,
            defaultGravityScale,
            defaultDamage,
            projectileSpeed,
            meleeRange,
            meleeDuration,
            size
        );
    }

    private bool IsSprinting(float horizontalInput)
    {
        if (Mathf.Approximately(horizontalInput, 0f))
            return false;

        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}
