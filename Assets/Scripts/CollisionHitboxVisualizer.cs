using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CollisionHitboxVisualizer : MonoBehaviour
{
    [SerializeField] private Color colliderColor = new Color(0f, 0.9f, 1f, 0.9f);
    [SerializeField] private Color triggerColor = new Color(1f, 0.6f, 0.1f, 0.9f);
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    [SerializeField, Range(12, 72)] private int circleSegments = 32;

    private static CollisionHitboxVisualizer instance;
    private static readonly Vector3[] boxCornerBuffer = new Vector3[4];

    private readonly List<Vector3> pointBuffer = new List<Vector3>(128);
    private bool overlayEnabled;
    private Material lineMaterial;

    public static void Toggle()
    {
        CollisionHitboxVisualizer visualizer = EnsureInstance();
        visualizer.SetVisible(!visualizer.overlayEnabled);
    }

    private static CollisionHitboxVisualizer EnsureInstance()
    {
        if (instance != null)
            return instance;

        GameObject obj = new GameObject("CollisionHitboxVisualizer");
        instance = obj.AddComponent<CollisionHitboxVisualizer>();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Camera.onPostRender += HandleCameraPostRender;
#if UNITY_2019_1_OR_NEWER
        RenderPipelineManager.endCameraRendering += HandleEndCameraRendering;
#endif
    }

    private void OnDisable()
    {
        Camera.onPostRender -= HandleCameraPostRender;
#if UNITY_2019_1_OR_NEWER
        RenderPipelineManager.endCameraRendering -= HandleEndCameraRendering;
#endif
    }

    private void OnDestroy()
    {
        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
            lineMaterial = null;
        }

        if (instance == this)
            instance = null;
    }

#if UNITY_2019_1_OR_NEWER
    private void HandleEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        HandleCameraPostRender(camera);
    }
#endif

    private void HandleCameraPostRender(Camera camera)
    {
        if (!overlayEnabled || camera == null)
            return;

        if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.VR)
            return;

        DrawAllColliders();
    }

    private void SetVisible(bool visible)
    {
        overlayEnabled = visible;
        Debug.Log($"Collision and hitbox visualization {(visible ? "enabled" : "disabled")}.");
    }

    private void EnsureLineMaterial()
    {
        if (lineMaterial != null)
            return;

        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        lineMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }

    private void DrawAllColliders()
    {
        Collider2D[] colliders = GetColliders();
        if (colliders == null || colliders.Length == 0)
            return;

        EnsureLineMaterial();

        GL.PushMatrix();
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);

        foreach (Collider2D collider in colliders)
        {
            if (collider == null)
                continue;

            Color color = collider.enabled
                ? (collider.isTrigger ? triggerColor : colliderColor)
                : inactiveColor;

            DrawColliderShape(collider, color);
        }

        GL.End();
        GL.PopMatrix();
    }

    private static Collider2D[] GetColliders()
    {
#if UNITY_2023_1_OR_NEWER
        return FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
#else
        return FindObjectsOfType<Collider2D>();
#endif
    }

    private void DrawColliderShape(Collider2D collider, Color color)
    {
        switch (collider)
        {
            case BoxCollider2D box:
                DrawBox(box, color);
                break;
            case CircleCollider2D circle:
                DrawCircle(circle, color);
                break;
            case CapsuleCollider2D capsule:
                DrawCapsule(capsule, color);
                break;
            case PolygonCollider2D polygon:
                DrawPolygon(polygon, color);
                break;
            case EdgeCollider2D edge:
                DrawEdge(edge, color);
                break;
            case CompositeCollider2D composite:
                DrawComposite(composite, color);
                break;
            default:
                DrawBounds(collider.bounds, color);
                break;
        }
    }

    private void DrawBox(BoxCollider2D box, Color color)
    {
        Vector2 size = box.size * 0.5f;
        Vector2 offset = box.offset;

        boxCornerBuffer[0] = box.transform.TransformPoint(offset + new Vector2(-size.x, -size.y));
        boxCornerBuffer[1] = box.transform.TransformPoint(offset + new Vector2(-size.x, size.y));
        boxCornerBuffer[2] = box.transform.TransformPoint(offset + new Vector2(size.x, size.y));
        boxCornerBuffer[3] = box.transform.TransformPoint(offset + new Vector2(size.x, -size.y));

        DrawLoop(boxCornerBuffer, 4, color);
    }

    private void DrawCircle(CircleCollider2D circle, Color color)
    {
        int segments = Mathf.Clamp(circleSegments, 12, 96);
        pointBuffer.Clear();
        Vector2 offset = circle.offset;

        float step = Mathf.PI * 2f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * step;
            Vector2 local = offset + new Vector2(Mathf.Cos(angle) * circle.radius, Mathf.Sin(angle) * circle.radius);
            pointBuffer.Add(circle.transform.TransformPoint(local));
        }

        DrawLoop(pointBuffer, color);
    }

    private void DrawCapsule(CapsuleCollider2D capsule, Color color)
    {
        int segments = Mathf.Clamp(circleSegments, 12, 96);
        pointBuffer.Clear();
        Vector2 offset = capsule.offset;
        Vector2 half = capsule.size * 0.5f;
        float step = Mathf.PI * 2f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * step;
            Vector2 local = offset + new Vector2(Mathf.Cos(angle) * half.x, Mathf.Sin(angle) * half.y);
            pointBuffer.Add(capsule.transform.TransformPoint(local));
        }

        DrawLoop(pointBuffer, color);
    }

    private void DrawPolygon(PolygonCollider2D polygon, Color color)
    {
        int pathCount = polygon.pathCount;
        for (int i = 0; i < pathCount; i++)
        {
            Vector2[] path = polygon.GetPath(i);
            if (path == null || path.Length < 2)
                continue;

            pointBuffer.Clear();
            for (int j = 0; j < path.Length; j++)
            {
                pointBuffer.Add(polygon.transform.TransformPoint(path[j]));
            }

            DrawLoop(pointBuffer, color);
        }
    }

    private void DrawEdge(EdgeCollider2D edge, Color color)
    {
        Vector2[] points = edge.points;
        if (points == null || points.Length < 2)
            return;

        pointBuffer.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            pointBuffer.Add(edge.transform.TransformPoint(points[i]));
        }

        DrawPath(pointBuffer, false, color);
    }

    private void DrawComposite(CompositeCollider2D composite, Color color)
    {
        int pathCount = composite.pathCount;
        bool loop = composite.geometryType == CompositeCollider2D.GeometryType.Polygons; // outlines shouldn't loop

        for (int i = 0; i < pathCount; i++)
        {
            int pointCount = composite.GetPathPointCount(i);
            if (pointCount < 2)
                continue;

            Vector2[] path = new Vector2[pointCount];
            composite.GetPath(i, path);

            pointBuffer.Clear();
            for (int j = 0; j < pointCount; j++)
            {
                pointBuffer.Add(composite.transform.TransformPoint(path[j]));
            }

            DrawPath(pointBuffer, loop, color);
        }
    }

    private void DrawBounds(Bounds bounds, Color color)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        boxCornerBuffer[0] = new Vector3(min.x, min.y, min.z);
        boxCornerBuffer[1] = new Vector3(min.x, max.y, min.z);
        boxCornerBuffer[2] = new Vector3(max.x, max.y, min.z);
        boxCornerBuffer[3] = new Vector3(max.x, min.y, min.z);

        DrawLoop(boxCornerBuffer, 4, color);
    }

    private void DrawLoop(IList<Vector3> points, Color color)
    {
        if (points == null || points.Count < 2)
            return;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % points.Count];
            DrawLine(current, next, color);
        }
    }

    private void DrawLoop(Vector3[] points, int count, Color color)
    {
        if (count < 2)
            return;

        for (int i = 0; i < count; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % count];
            DrawLine(current, next, color);
        }
    }

    private void DrawPath(IList<Vector3> points, bool loop, Color color)
    {
        if (points == null || points.Count < 2)
            return;

        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(points[i], points[i + 1], color);
        }

        if (loop)
        {
            DrawLine(points[points.Count - 1], points[0], color);
        }
    }

    private void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        GL.Color(color);
        GL.Vertex(from);
        GL.Vertex(to);
    }
}
