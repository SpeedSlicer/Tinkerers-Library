using UnityEditor;
using UnityEngine;
using System.IO;
/**
    Taken from https://github.com/teamorbit1/UnityPrefabScreenshotter
    Made by teamorbit1
*/
public class PrefabScreenshotWindow : EditorWindow
{
    private GameObject prefab;
    private PreviewRenderUtility preview;
    private GameObject instance;

    private float yaw = -30f;
    private float pitch = 20f;
    private float distance = 2.5f;
    private Vector3 pivot = Vector3.zero;
    private Vector3 panOffset = Vector3.zero;

    private int exportResolution = 1024;

    private string saveFolder = "";

    private Vector2 lastMousePos;

    [MenuItem("Tools/Prefab Screenshot (Interactive)")]
    private static void OpenWindow()
    {
        GetWindow<PrefabScreenshotWindow>("Prefab Screenshot");
    }

    private void OnEnable()
    {
        CreatePreview();
    }

    private void OnDisable()
    {
        Cleanup();
    }

    private void CreatePreview()
    {
        preview = new PreviewRenderUtility(true);
        preview.cameraFieldOfView = 30f;

        preview.camera.clearFlags = CameraClearFlags.Color;
        preview.camera.backgroundColor = new Color(0, 0, 0, 0);

        preview.lights[0].intensity = 1.4f;
        preview.lights[0].transform.rotation = Quaternion.Euler(50, 30, 0);
        preview.lights[1].intensity = 0f;
    }

    private void Cleanup()
    {
        if (instance != null)
            DestroyImmediate(instance);

        if (preview != null)
        {
            preview.Cleanup();
            preview = null;
        }
    }

    private void SpawnInstance()
    {
        if (instance != null)
            DestroyImmediate(instance);

        if (prefab == null)
            return;

        instance = (GameObject)preview.InstantiatePrefabInScene(prefab);

        Bounds b = CalculateBounds(instance);
        pivot = b.center;

        float radius = b.extents.magnitude;
        distance = radius * 2.2f;
        panOffset = Vector3.zero;
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds b = new Bounds(rends[0].bounds.center, rends[0].bounds.size);
        foreach (var r in rends)
            b.Encapsulate(r.bounds);

        return b;
    }

    private void OnGUI()
    {

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Rotate: Left Mouse", GUILayout.Height(25), GUILayout.Width(180))) { }
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Pan: Right Mouse", GUILayout.Height(25), GUILayout.Width(180))) { }
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Reset: Middle Mouse", GUILayout.Height(25), GUILayout.Width(180)))
        {
            ResetView();
        }

        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        EditorGUI.BeginChangeCheck();
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            SpawnInstance();
            Repaint();
        }

        exportResolution = EditorGUILayout.IntPopup("Export Size",
            exportResolution,
            new[] { "512", "1024", "2048" },
            new[] { 512, 1024, 2048 });

        GUILayout.Space(10);

        Rect previewRect = GUILayoutUtility.GetRect(10, 10, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        DrawPreview(previewRect);

        GUILayout.Space(10);

        GUI.enabled = instance != null;

        if (GUILayout.Button("Capture Transparent PNG", GUILayout.Height(30)))
            CapturePNG();

        GUI.enabled = true;

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Save Folder: " + (string.IsNullOrEmpty(saveFolder) ? "<None Selected>" : saveFolder));

        if (GUILayout.Button("Select Folder", GUILayout.Width(120)))
        {
            string newPath = EditorUtility.OpenFolderPanel("Select Save Folder", "", "");
            if (!string.IsNullOrEmpty(newPath))
            {
                saveFolder = newPath;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPreview(Rect rect)
    {
        if (instance == null || preview == null)
        {
            EditorGUI.HelpBox(rect, "Assign a Prefab to preview.", MessageType.Info);
            return;
        }

        HandleInput(rect);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 camPos = pivot + panOffset + rot * (Vector3.back * distance);

        preview.camera.transform.position = camPos;
        preview.camera.transform.LookAt(pivot + panOffset);

        preview.BeginPreview(rect, GUIStyle.none);
        preview.camera.Render();
        Texture result = preview.EndPreview();
        GUI.DrawTexture(rect, result, ScaleMode.ScaleToFit, true);
    }

    private void HandleInput(Rect rect)
    {
        Event e = Event.current;

        if (!rect.Contains(e.mousePosition))
            return;

        Vector2 delta = Vector2.zero;
        if (e.type == EventType.MouseDrag)
            delta = e.delta;

        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            float rotSpeed = 0.4f;
            yaw += delta.x * rotSpeed;
            pitch -= delta.y * rotSpeed;

            pitch = Mathf.Clamp(pitch, -80f, 80f);

            e.Use();
            Repaint();
        }

        if (e.type == EventType.MouseDrag && e.button == 1)
        {
            float panSpeed = distance * 0.002f;

            Vector3 right = preview.camera.transform.right;
            Vector3 up = preview.camera.transform.up;

            panOffset += (-right * delta.x + up * delta.y) * panSpeed;

            e.Use();
            Repaint();
        }

        if (e.type == EventType.MouseDown && e.button == 2)
        {
            ResetView();
            e.Use();
            Repaint();
        }

        if (e.type == EventType.ScrollWheel)
        {
            float zoomSpeed = 0.2f;
            distance += e.delta.y * zoomSpeed;
            distance = Mathf.Max(0.2f, distance);
            e.Use();
            Repaint();
        }
    }



    private void ResetView()
    {
        yaw = -30f;
        pitch = 20f;
        panOffset = Vector3.zero;

        if (instance != null)
        {
            Bounds b = CalculateBounds(instance);
            float radius = b.extents.magnitude;
            distance = radius * 2.2f;
        }
    }

    private void CapturePNG()
    {
        if (prefab == null || preview == null) return;
        if (string.IsNullOrEmpty(saveFolder))
        {
            EditorUtility.DisplayDialog("No Folder Selected", "Please select a save folder first.", "OK");
            return;
        }

        int res = exportResolution;

        var rt = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32);
        rt.Create();

        preview.camera.targetTexture = rt;
        preview.camera.Render();
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, res, res), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        preview.camera.targetTexture = null;
        rt.Release();

        string path = Path.Combine(saveFolder, prefab.name + "_icon.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());

        AssetDatabase.Refresh();
        DestroyImmediate(tex);

        Debug.Log("Saved screenshot to: " + path);
    }
}


