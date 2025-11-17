// JLMD 

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if (horizontalInput > 0.01f) // Flip player
            transform.localScale = new Vector3(6, 6, 1);
        else if (horizontalInput < -0.01f) // Flip player again
            transform.localScale = new Vector3(-6, 6, 1);

        if (Input.GetKey(KeyCode.Space) && isGrounded()) // Fixed method call
            Jump();

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded()); // Fixed method call
    }
    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, speed); // Fixed typo: linearVelocity -> velocity
        anim.SetTrigger("jump");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision logic here
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
}