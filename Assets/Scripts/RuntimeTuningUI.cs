using UnityEngine;

public class RuntimeTuningUI : MonoBehaviour
{
    [SerializeField] private PlayerTunableStats playerStats;
    [SerializeField] private Rect initialWindowRect = new Rect(80f, 80f, 1400f, 900f);
    [SerializeField] private Vector2 minimumWindowSize = new Vector2(960f, 720f);
    [SerializeField] private Vector2 desiredWindowSize = new Vector2(1400f, 900f);
    [SerializeField] private float sliderLabelWidth = 220f;
    [SerializeField] private float sliderMaxWidth = 800f;
    [SerializeField] private float buttonHeight = 60f;
    [SerializeField, Range(12, 72)] private int windowTitleFontSize = 48;
    [SerializeField, Range(12, 48)] private int headerFontSize = 50;
    [SerializeField, Range(12, 48)] private int bodyFontSize = 25;
    [SerializeField, Range(12, 48)] private int valueFontSize = 25;
    [SerializeField, Range(12, 48)] private int buttonFontSize = 25;

    private Rect windowRect;
    private bool windowVisible;
    private Vector2 scrollPosition;
    private GUIStyle boldLabelStyle;
    private GUIStyle bodyLabelStyle;
    private GUIStyle valueLabelStyle;
    private GUIStyle buttonStyle;
    private GUIStyle windowStyle;
    private static RuntimeTuningUI instance;
    private static PlayerTunableStats cachedStats;
    private static int lastToggleFrame = -1;

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
        EnforceMinimumWindowSize();
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

        EnforceMinimumWindowSize();
        windowRect = GUILayout.Window(GetInstanceID(), windowRect, DrawWindow, GUIContent.none, GetWindowStyle());
    }

    private void DrawWindow(int windowID)
    {
        GUIStyle windowTitleStyle = GetWindowStyle();
        Rect titleRect = new Rect(0f, 0f, windowRect.width, windowTitleStyle.lineHeight + 12f);
        GUI.Label(titleRect, "Runtime Tuning", windowTitleStyle);
        GUILayout.Space(windowTitleStyle.lineHeight + 8f);

        GUILayout.Label("Adjust player stats while in Play Mode.", GetBodyLabelStyle());

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
        playerStats.ProjectileSpeed = SliderWithValue("Projectile Speed", playerStats.ProjectileSpeed, 0f, 60f);
        playerStats.MeleeAttackRange = SliderWithValue("Melee Range", playerStats.MeleeAttackRange, 0.1f, 5f);
        playerStats.MeleeAttackDuration = SliderWithValue("Melee Duration", playerStats.MeleeAttackDuration, 0.05f, 2f);

        GUILayout.Space(10f);
        GUILayout.Label("Presentation", boldLabel);
        var size = playerStats.PlayerSize;
        size.x = SliderWithValue("Size X", size.x, 0.1f, 20f);
        size.y = SliderWithValue("Size Y", size.y, 0.1f, 20f);
        size.z = SliderWithValue("Size Z", size.z, 0.1f, 20f);
        playerStats.PlayerSize = size;

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUIStyle resetCaptureStyle = GetButtonStyle();
        float resolvedButtonHeight = Mathf.Max(30f, buttonHeight);
        if (GUILayout.Button("Reset To Defaults", resetCaptureStyle, GUILayout.Height(resolvedButtonHeight)))
        {
            playerStats.ResetToDefaults();
        }
        if (GUILayout.Button("Capture Defaults", resetCaptureStyle, GUILayout.Height(resolvedButtonHeight)))
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
        GUIStyle labelStyle = GetBodyLabelStyle();
        GUIStyle valueStyle = GetValueLabelStyle();
        float labelWidth = Mathf.Max(100f, sliderLabelWidth);
        GUILayout.Label(label, labelStyle, GUILayout.Width(labelWidth));
        float sliderWidth = Mathf.Max(60f, sliderMaxWidth);
        value = GUILayout.HorizontalSlider(value, min, max, GUILayout.MaxWidth(sliderWidth));
        GUILayout.Label(value.ToString("F2"), valueStyle, GUILayout.Width(80f));
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

        boldLabelStyle.fontSize = Mathf.Max(12, headerFontSize);
        return boldLabelStyle;
    }

    private GUIStyle GetBodyLabelStyle()
    {
        if (bodyLabelStyle == null)
            bodyLabelStyle = new GUIStyle(GUI.skin.label);

        bodyLabelStyle.fontSize = Mathf.Max(12, bodyFontSize);
        bodyLabelStyle.wordWrap = false;
        return bodyLabelStyle;
    }

    private GUIStyle GetValueLabelStyle()
    {
        if (valueLabelStyle == null)
        {
            valueLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight
            };
        }

        valueLabelStyle.fontSize = Mathf.Max(12, valueFontSize);
        valueLabelStyle.wordWrap = false;
        return valueLabelStyle;
    }

    private GUIStyle GetButtonStyle()
    {
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }

        buttonStyle.fontSize = Mathf.Max(12, buttonFontSize);
        return buttonStyle;
    }

    private GUIStyle GetWindowStyle()
    {
        if (windowStyle == null)
        {
            windowStyle = new GUIStyle(GUI.skin.window)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }

        windowStyle.fontSize = Mathf.Max(12, windowTitleFontSize);
        return windowStyle;
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

        if (lastToggleFrame == Time.frameCount)
            return;

        ui.playerStats = resolved;
        cachedStats = resolved;
        ui.windowVisible = !ui.windowVisible;
        lastToggleFrame = Time.frameCount;
    }

    private void EnforceMinimumWindowSize()
    {
        float minWidth = Mathf.Max(10f, minimumWindowSize.x);
        float minHeight = Mathf.Max(10f, minimumWindowSize.y);
        float desiredWidth = Mathf.Max(minWidth, desiredWindowSize.x);
        float desiredHeight = Mathf.Max(minHeight, desiredWindowSize.y);

        if (windowRect.width < minWidth)
            windowRect.width = minWidth;
        if (windowRect.height < minHeight)
            windowRect.height = minHeight;

        if (windowRect.width < desiredWidth)
            windowRect.width = desiredWidth;
        if (windowRect.height < desiredHeight)
            windowRect.height = desiredHeight;
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
