using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject swordPrefab;
    public Transform attackPoint;
    public float attackRange = 1f;
    public float attackDuration = 0.2f;
    public LayerMask enemyLayer;
    public GameObject slashEffectPrefab; 

    private bool facingRight = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Attack();
        }

        // Detect player direction
        float move = Input.GetAxisRaw("Horizontal");
        if (move > 0) facingRight = true;
        else if (move < 0) facingRight = false;
    }

    void Attack()
    {
        Vector3 attackPos = attackPoint.position + (facingRight ? Vector3.right : Vector3.left) * attackRange;

        
        Instantiate(slashEffectPrefab, attackPos, Quaternion.identity);

        GameObject sword = Instantiate(swordPrefab, attackPos, facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0));

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, new Vector2(1.5f, 0.5f), 0f, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy: " + enemy.name);
                Destroy(enemy.gameObject);
            }
        }

        Destroy(sword, attackDuration);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position + Vector3.right * attackRange, new Vector3(1.5f, 0.5f, 0f));
    }
}
