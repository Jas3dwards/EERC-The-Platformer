using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject swordPrefab;
    public Transform attackPoint;
    public float attackRange = 1f;
    public float attackDuration = 0.2f;
    public LayerMask enemyLayer;
    public GameObject slashEffectPrefab;

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 8f;

    private bool facingRight = true;

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if (move > 0) facingRight = true;
        else if (move < 0) facingRight = false;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            AttackMelee();

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            AttackRanged();
    }

    void AttackMelee()
    {
        Vector3 attackPos = attackPoint.position + (facingRight ? Vector3.right : Vector3.left) * attackRange;

        Instantiate(slashEffectPrefab, attackPos, Quaternion.identity);

        GameObject sword = Instantiate(
            swordPrefab,
            attackPos,
            facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0)
        );

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPos,
            new Vector2(1.5f, 0.5f),
            0f,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Destroy(enemy.gameObject);
            }
        }

        Destroy(sword, attackDuration);
    }

    void AttackRanged()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null)
            return;

        float direction = facingRight ? 1 : -1;

        GameObject proj = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0)
        );

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction * projectileSpeed, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                attackPoint.position + Vector3.right * attackRange,
                new Vector3(1.5f, 0.5f, 0f)
            );
        }
    }
}
