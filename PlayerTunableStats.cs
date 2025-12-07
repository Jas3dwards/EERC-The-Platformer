using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTunableStats", menuName = "Tuning/Player Tunable Stats")]
public class PlayerTunableStats : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float flySpeed = 12f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravityScale = 3f;

    [Header("Combat")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float meleeAttackRange = 1f;
    [SerializeField] private float meleeAttackDuration = 0.2f;

    [Header("Presentation")]
    [SerializeField] private Vector3 playerSize = new Vector3(6f, 6f, 1f);

    [System.Serializable]
    private struct PlayerStatsSnapshot
    {
        public bool initialized;
        public float moveSpeed;
        public float sprintMultiplier;
        public float flySpeed;
        public float jumpForce;
        public float gravityScale;
        public float damage;
        public float projectileSpeed;
        public float meleeAttackRange;
        public float meleeAttackDuration;
        public Vector3 playerSize;
    }

    [SerializeField, HideInInspector] private PlayerStatsSnapshot defaultSnapshot;

    private void OnEnable()
    {
        if (!defaultSnapshot.initialized)
        {
            CaptureDefaults();
        }
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0f, value);
    }

    public float SprintMultiplier
    {
        get => sprintMultiplier;
        set => sprintMultiplier = Mathf.Max(1f, value);
    }

    public float FlySpeed
    {
        get => flySpeed;
        set => flySpeed = Mathf.Max(0f, value);
    }

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = Mathf.Max(0f, value);
    }

    public float GravityScale
    {
        get => gravityScale;
        set => gravityScale = Mathf.Max(0f, value);
    }

    public float Damage
    {
        get => damage;
        set => damage = Mathf.Max(0f, value);
    }

    public float ProjectileSpeed
    {
        get => projectileSpeed;
        set => projectileSpeed = Mathf.Max(0f, value);
    }

    public float MeleeAttackRange
    {
        get => meleeAttackRange;
        set => meleeAttackRange = Mathf.Max(0.1f, value);
    }

    public float MeleeAttackDuration
    {
        get => meleeAttackDuration;
        set => meleeAttackDuration = Mathf.Max(0.01f, value);
    }

    public Vector3 PlayerSize
    {
        get => playerSize;
        set => playerSize = new Vector3(Mathf.Max(0.1f, Mathf.Abs(value.x)), Mathf.Max(0.1f, Mathf.Abs(value.y)), Mathf.Max(0.1f, Mathf.Abs(value.z)));
    }

    public void ResetToDefaults()
    {
        if (!defaultSnapshot.initialized)
            return;

        moveSpeed = defaultSnapshot.moveSpeed;
        sprintMultiplier = defaultSnapshot.sprintMultiplier;
        flySpeed = defaultSnapshot.flySpeed;
        jumpForce = defaultSnapshot.jumpForce;
        gravityScale = defaultSnapshot.gravityScale;
        damage = defaultSnapshot.damage;
        projectileSpeed = defaultSnapshot.projectileSpeed;
        meleeAttackRange = defaultSnapshot.meleeAttackRange;
        meleeAttackDuration = defaultSnapshot.meleeAttackDuration;
        playerSize = defaultSnapshot.playerSize;
    }

    public void CaptureDefaults()
    {
        defaultSnapshot = new PlayerStatsSnapshot
        {
            initialized = true,
            moveSpeed = moveSpeed,
            sprintMultiplier = sprintMultiplier,
            flySpeed = flySpeed,
            jumpForce = jumpForce,
            gravityScale = gravityScale,
            damage = damage,
            projectileSpeed = projectileSpeed,
            meleeAttackRange = meleeAttackRange,
            meleeAttackDuration = meleeAttackDuration,
            playerSize = playerSize
        };
    }

    public void InitializeValues(
        float moveSpeedValue,
        float sprintMultiplierValue,
        float flySpeedValue,
        float jumpForceValue,
        float gravityScaleValue,
        float damageValue,
        float projectileSpeedValue,
        float meleeAttackRangeValue,
        float meleeAttackDurationValue,
        Vector3 sizeValue,
        bool captureAsDefaults = true)
    {
        moveSpeed = moveSpeedValue;
        sprintMultiplier = sprintMultiplierValue;
        flySpeed = flySpeedValue;
        jumpForce = jumpForceValue;
        gravityScale = gravityScaleValue;
        damage = damageValue;
        projectileSpeed = projectileSpeedValue;
        meleeAttackRange = meleeAttackRangeValue;
        meleeAttackDuration = meleeAttackDurationValue;
        playerSize = sizeValue;

        if (captureAsDefaults)
        {
            CaptureDefaults();
        }
    }

    public static PlayerTunableStats CreateRuntimeInstance(
        float moveSpeedValue,
        float sprintMultiplierValue,
        float flySpeedValue,
        float jumpForceValue,
        float gravityScaleValue,
        float damageValue,
        float projectileSpeedValue,
        float meleeAttackRangeValue,
        float meleeAttackDurationValue,
        Vector3 sizeValue)
    {
        PlayerTunableStats stats = CreateInstance<PlayerTunableStats>();
        stats.InitializeValues(
            moveSpeedValue,
            sprintMultiplierValue,
            flySpeedValue,
            jumpForceValue,
            gravityScaleValue,
            damageValue,
            projectileSpeedValue,
            meleeAttackRangeValue,
            meleeAttackDurationValue,
            sizeValue,
            true);
        return stats;
    }
}
