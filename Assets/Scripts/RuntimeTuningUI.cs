using UnityEngine;

public class RuntimeTuningUI : MonoBehaviour
{
    [SerializeField] private PlayerTunableStats playerStats;
    [SerializeField] private Rect initialWindowRect = new Rect(30f, 30f, 360f, 420f);

    private Rect windowRect;
    private bool windowVisible;
    private Vector2 scrollPosition;
    private GUIStyle boldLabelStyle;
    private static RuntimeTuningUI instance;
    private static PlayerTunableStats cachedStats;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        windowRect = initialWindowRect;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void OnGUI()
    {
        if (!windowVisible)
            return;

        if (playerStats == null)
        {
            const string warning = "Assign a PlayerTunableStats asset to RuntimeTuningUI.";
            var style = new GUIStyle(GUI.skin.box) { fontSize = 14, alignment = TextAnchor.MiddleCenter };
            GUI.Box(new Rect(30f, 30f, 320f, 60f), warning, style);
            return;
        }

        windowRect = GUILayout.Window(GetInstanceID(), windowRect, DrawWindow, "Runtime Tuning");
    }

    private void DrawWindow(int windowID)
    {
        GUILayout.Label("Adjust player stats while in Play Mode.", GUI.skin.label);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

        GUILayout.Space(5f);
        GUIStyle boldLabel = GetBoldLabel();
        GUILayout.Label("Movement", boldLabel);
        playerStats.MoveSpeed = SliderWithValue("Move Speed", playerStats.MoveSpeed, 0f, 40f);
        playerStats.SprintMultiplier = SliderWithValue("Sprint Multiplier", playerStats.SprintMultiplier, 1f, 4f);
        playerStats.FlySpeed = SliderWithValue("Fly Speed", playerStats.FlySpeed, 0f, 40f);
        playerStats.JumpForce = SliderWithValue("Jump Force", playerStats.JumpForce, 0f, 40f);
        playerStats.GravityScale = SliderWithValue("Gravity Scale", playerStats.GravityScale, 0f, 10f);

        GUILayout.Space(10f);
        GUILayout.Label("Combat", boldLabel);
        playerStats.Damage = SliderWithValue("Damage", playerStats.Damage, 0f, 100f);

        GUILayout.Space(10f);
        GUILayout.Label("Presentation", boldLabel);
        var size = playerStats.PlayerSize;
        size.x = SliderWithValue("Size X", size.x, 0.1f, 20f);
        size.y = SliderWithValue("Size Y", size.y, 0.1f, 20f);
        size.z = SliderWithValue("Size Z", size.z, 0.1f, 20f);
        playerStats.PlayerSize = size;

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset To Defaults"))
        {
            playerStats.ResetToDefaults();
        }
        if (GUILayout.Button("Capture Defaults"))
        {
            playerStats.CaptureDefaults();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    private float SliderWithValue(string label, float value, float min, float max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(130f));
        value = GUILayout.HorizontalSlider(value, min, max);
        GUILayout.Label(value.ToString("F2"), GUILayout.Width(60f));
        GUILayout.EndHorizontal();
        return Mathf.Clamp(value, min, max);
    }

    private GUIStyle GetBoldLabel()
    {
        if (boldLabelStyle == null)
        {
            boldLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
        }

        return boldLabelStyle;
    }

    public static void Toggle(PlayerTunableStats stats)
    {
        RuntimeTuningUI ui = EnsureInstance();
        PlayerTunableStats resolved = ResolveStats(stats);
        if (resolved == null)
        {
            Debug.LogWarning("RuntimeTuningUI could not find PlayerTunableStats to modify. Ensure a PlayerMovement is active.");
            ui.windowVisible = false;
            return;
        }

        ui.playerStats = resolved;
        cachedStats = resolved;
        ui.windowVisible = !ui.windowVisible;
    }

    public static void RegisterStats(PlayerTunableStats stats)
    {
        if (stats == null)
            return;

        cachedStats = stats;
        if (instance != null)
            instance.playerStats = stats;
    }

    private static RuntimeTuningUI EnsureInstance()
    {
        if (instance != null)
            return instance;

#if UNITY_2023_1_OR_NEWER
        instance = FindFirstObjectByType<RuntimeTuningUI>();
#else
        instance = FindObjectOfType<RuntimeTuningUI>();
#endif
        if (instance != null)
            return instance;

        GameObject obj = new GameObject("RuntimeTuningUI");
        instance = obj.AddComponent<RuntimeTuningUI>();
        return instance;
    }

    private static PlayerTunableStats ResolveStats(PlayerTunableStats candidate)
    {
        if (candidate != null)
            return candidate;

        if (cachedStats != null)
            return cachedStats;

        if (instance != null && instance.playerStats != null)
            return instance.playerStats;

        PlayerMovement movement = PlayerMovement.ActivePlayer;
        if (movement == null)
        {
#if UNITY_2023_1_OR_NEWER
            movement = FindFirstObjectByType<PlayerMovement>();
#else
            movement = FindObjectOfType<PlayerMovement>();
#endif
        }

        if (movement == null)
            return null;

        PlayerTunableStats stats = movement.CurrentStats;
        cachedStats = stats;
        return stats;
    }
}
