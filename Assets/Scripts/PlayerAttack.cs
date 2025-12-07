using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerTunableStats playerStats;
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
    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private PlayerTunableStats Stats
    {
        get
        {
            if (playerStats != null)
                return playerStats;
            if (movement != null)
                return movement.CurrentStats;
            return null;
        }
    }

    private void Update()
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
        float range = GetMeleeAttackRange();
        float duration = GetMeleeAttackDuration();
        Vector3 attackPos = attackPoint.position + (facingRight ? Vector3.right : Vector3.left) * range;

        Instantiate(slashEffectPrefab, attackPos, Quaternion.identity);

        GameObject sword = Instantiate(
            swordPrefab,
            attackPos,
            facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0)
        );

        Vector2 overlapSize = new Vector2(Mathf.Max(0.5f, range * 1.5f), 0.5f);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPos,
            overlapSize,
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

        Destroy(sword, duration);
    }

    void AttackRanged()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null)
            return;

        float direction = facingRight ? 1 : -1;
        float projectileSpeedValue = GetProjectileSpeed();

        GameObject proj = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0)
        );

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction * projectileSpeedValue, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            float gizmoRange = Application.isPlaying ? GetMeleeAttackRange() : attackRange;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                attackPoint.position + Vector3.right * gizmoRange,
                new Vector3(1.5f, 0.5f, 0f)
            );
        }
    }

    private float GetProjectileSpeed()
    {
        PlayerTunableStats stats = Stats;
        return stats != null ? stats.ProjectileSpeed : projectileSpeed;
    }

    private float GetMeleeAttackRange()
    {
        PlayerTunableStats stats = Stats;
        return stats != null ? stats.MeleeAttackRange : attackRange;
    }

    private float GetMeleeAttackDuration()
    {
        PlayerTunableStats stats = Stats;
        return stats != null ? stats.MeleeAttackDuration : attackDuration;
    }
}
