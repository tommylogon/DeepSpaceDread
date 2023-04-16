using System.IO;
using UnityEditor;
using UnityEngine;
using static PrefabSelectionWindow;

[System.Serializable]
public class PrefabSpawner2DEditor : EditorWindow
{
    [System.Serializable]
    class Spawner
    {
        public float size = 0.1f;
        public float fillRate = 0.25f;
        public float timer = 0.0f;
        public bool continuous = false;
        public Vector3 position;
        public Vector3 direction;
        public Vector3 surface = Vector3.forward;
    }

    class SinglePrefab
    {
        public Transform transform;
        public Vector3 scale;
        public Vector3 normal;
        public Vector3 origin;
    }

    const string defaultSaveFilePath = "Assets\\Plugins\\PrefabSpawner2D\\Editor\\Data\\SaveData.txt";
    const string slotsSaveFilePath = "Assets\\Plugins\\PrefabSpawner2D\\Editor\\Data\\";
    readonly string undoAction = "Instantiate Prefab";

    bool altDown = false;
    bool ctrlDown = false;

    [SerializeField] int editorHash;
    [SerializeField] readonly Spawner spawner = new Spawner();
    [SerializeField] readonly SinglePrefab singlePrefab = new SinglePrefab();
    [SerializeField] readonly PrefabSelectionWindow prefabWindow = new PrefabSelectionWindow();

    static bool enabled = false;

    [MenuItem("Tools/Prefab Spawner 2D/Prefabs Window")]
    public static void ShowWindow()
    {
        PrefabSpawner2DEditor editor = GetWindow<PrefabSpawner2DEditor>(false, "Prefab Spawner 2D", true);
        editor.minSize = new Vector2(160, 300);

        if (File.Exists(defaultSaveFilePath))
        {
            editor.LoadFromJson(new StreamReader(defaultSaveFilePath));
        }
    }

    [MenuItem("Tools/Prefab Spawner 2D/Toggle Spawner")]
    public static void ToggleSpawner()
    {
        enabled = !enabled;
    }

    // Called when the window gets keyboard focus
    void OnFocus()
    {
        // Remove if already present and register the function
        SceneView.duringSceneGui -= OnSceneGUI;

        // This allows us to register an update function for the scene view port
        SceneView.duringSceneGui += OnSceneGUI;

        // Storing our editors hash ID for control ID purposes
        editorHash = GetHashCode();
    }

    // Called when the window is closed
    void OnDestroy()
    {
        // Unregister our scene update function
        SceneView.duringSceneGui -= OnSceneGUI;

        Directory.CreateDirectory(slotsSaveFilePath);

        if (!File.Exists(defaultSaveFilePath))
        {
            File.Create(defaultSaveFilePath).Dispose();
        }

        File.WriteAllText(defaultSaveFilePath, this.ToJson());
    }

    void OnGUI()
    {
        #region Settings
        EditorGUILayout.Separator();

        // Continuous mode toggle
        spawner.continuous = EditorGUILayout.BeginToggleGroup("Continuous", spawner.continuous);

        // Prefab's rate for placing objects
        EditorGUILayout.LabelField("Fill Rate");
        spawner.fillRate = EditorGUILayout.Slider(spawner.fillRate, 0.01f, 0.5f);
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Separator();
        #endregion

        #region Prefabs Selection Menu
        int gridWidth = (int)position.width / 140;
        prefabWindow.DrawGUI(gridWidth);

        EditorGUILayout.Separator();
        #endregion

        #region Save/Load Buttons
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Slots..."))
        {
            SaveSlotsWindow.ShowWindow();
        }

        if (GUILayout.Button("Load Slots..."))
        {
            string file = EditorUtility.OpenFilePanel("Load Slots File", slotsSaveFilePath, "json");
            if (file.Length > 0)
            {
                LoadFromJson(new StreamReader(file));
            }
        }

        EditorGUILayout.EndHorizontal();
        #endregion
    }

    // Updates whenever the scene is interacted with
    void OnSceneGUI(SceneView view)
    {
        // Convert to world space
        Vector2 mousePosition = MouseHelper.Position;
        if (OutsideSceneView(view, mousePosition))
        {
            return;
        }

        Vector3 offset = new Vector3((mousePosition.x - view.position.x) / view.position.width, ((view.position.height + view.position.y) - mousePosition.y) / view.position.height, 0);
        spawner.position = view.camera.ViewportToWorldPoint(offset);
        spawner.position.z = 0;

        // Display the spawner
        if (enabled)
        {
            DisplaySpawner();
        }

        // Handle the spawner input
        HandleSpawnerInput();
    }

    bool OutsideSceneView(SceneView view, Vector2 position)
    {
        if (position.x < view.position.x || position.x > view.position.x + view.position.width)
        {
            return true;
        }

        if (position.y < view.position.y || position.y > view.position.y + view.position.height)
        {
            return true;
        }

        return false;
    }

    // Draw the spawner circle and surface normal
    void DisplaySpawner()
    {
        // Number of line segments in the circle
        byte circleSegments = 30;

        // Draw the spawner circle
        Handles.BeginGUI();

        // Finding the direction of the spawner on the surface
        spawner.direction = Vector3.Normalize(Vector3.Cross(spawner.position, spawner.position + spawner.surface));
        Vector3 startPoint = spawner.direction * spawner.size;
        Vector3 previousPoint = spawner.position + startPoint;
        Vector3 nextPoint;

        // Draw the spawner line segments
        for (float i = 0.0f; i < 365; i += 360.0f / circleSegments)
        {
            // Calculate the new point on the circle
            nextPoint = spawner.position + Quaternion.AngleAxis(i, spawner.surface) * startPoint;

            // Draw a line from the old to the new
            Handles.DrawLine(HandleUtility.WorldToGUIPoint(previousPoint),
                             HandleUtility.WorldToGUIPoint(nextPoint));

            previousPoint = nextPoint;
        }

        // Draw the surface normal
        Handles.DrawLine(HandleUtility.WorldToGUIPoint(spawner.position),
                         HandleUtility.WorldToGUIPoint(spawner.position + spawner.surface));
        HandleUtility.Repaint();
        Handles.EndGUI();
    }

    void HandleSpawnerInput()
    {
        // This logic bypass the Unity GUI key event handling with a custom logic
        if (Event.current.keyCode == KeyCode.LeftAlt || Event.current.keyCode == KeyCode.RightAlt)
        {
            altDown = (Event.current.type != EventType.KeyUp);
        }

        if (Event.current.keyCode == KeyCode.LeftControl || Event.current.keyCode == KeyCode.RightControl)
        {
            ctrlDown = Event.current.type != EventType.KeyUp;
        }

        // Ctrl + E == Toggle enabled
        if (Event.current.keyCode == KeyCode.E && Event.current.type == EventType.KeyUp && ctrlDown)
        {
            enabled = !enabled;
            Repaint();
        }

        // Check if the tool is in use
        if (enabled)
        {
            if (Event.current.button == 0)
            {
                Selection.activeGameObject = null;

                if (!altDown && !ctrlDown)
                {
                    switch (Event.current.type)
                    {
                        case EventType.MouseDown:
                            {
                                if (spawner.continuous)
                                {
                                    PlacePrefab();
                                }

                                break;
                            }
                        case EventType.MouseDrag:
                            {
                                if (spawner.continuous && Time.realtimeSinceStartup - spawner.timer >= spawner.fillRate)
                                {
                                    PlacePrefab();
                                }

                                break;
                            }
                        case EventType.MouseUp:
                            {
                                if (!spawner.continuous)
                                {
                                    PlacePrefab();
                                    Event.current.Use();
                                }
                                break;
                            }
                    }
                }
                else if (Event.current.button == 0 && ctrlDown)
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        PlaceSinglePrefab();
                    }
                }
            }

            // Get tool control ID
            int controlID = GUIUtility.GetControlID(editorHash, FocusType.Passive);

            // This intercepts all input events
            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(controlID);
            }
        }

        Vector2 mousePos = Event.current.mousePosition;

        if (singlePrefab.transform != null)
        {
            if (Event.current.type == EventType.MouseUp ||
                mousePos.x > Screen.width + 50 || mousePos.x < -50 ||
                mousePos.y > Screen.height + 50 || mousePos.y < -50)
            {
                ReleaseSinglePrefab();
            }
            else if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Space)
            {
                IterateSinglePrefab();
            }
            else
            {
                UpdateSinglePrefab();
            }
        }
    }

    #region Place Prefabs
    // Place randomly selected prefabs into the scene
    void PlacePrefab()
    {
        // Check if we have active prefabs to place
        if (prefabWindow.HasPrefab())
        {
            var prefab = prefabWindow.GetPrefab();
            Vector2 position = prefab.randomPositionToggle ? GetRandomPosition(spawner.position) : spawner.position;
            Quaternion rotation = prefab.GetRotation();

            GameObject obj = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            obj.transform.position = new Vector3(position.x, position.y, obj.transform.position.z);

            if (prefab.randomRotationToggle)
            {
                obj.transform.rotation = rotation;
            }

            if (prefab.randomScaleToggle)
            {
                obj.transform.localScale *= Random.Range(prefab.randomScaleMin, prefab.randomScaleMax);
            }

            Undo.RegisterCreatedObjectUndo(obj, undoAction);

            obj.transform.parent = SetParent(prefabWindow);

            spawner.timer = Time.realtimeSinceStartup;
        }
    }

    void PlaceSinglePrefab()
    {
        // Check if there are active prefabs to place
        if (prefabWindow.HasPrefab())
        {
            var prefab = prefabWindow.GetPrefab();
            Quaternion rotation = prefab.GetRotation();
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            obj.transform.position = new Vector3(spawner.position.x, spawner.position.y, obj.transform.position.z);
            obj.transform.rotation = Quaternion.identity;

            if (prefab.randomRotationToggle)
            {
                obj.transform.rotation = rotation;
            }

            Undo.RegisterCreatedObjectUndo(obj, undoAction);

            obj.transform.parent = SetParent(prefabWindow);

            singlePrefab.transform = obj.transform;
            singlePrefab.scale = obj.transform.localScale;
            singlePrefab.normal = spawner.surface;
            singlePrefab.origin = Event.current.mousePosition;
        }
    }

    // Update the scale and rotation of a prefab
    void UpdateSinglePrefab()
    {
        if (singlePrefab.transform)
        {
            float x = (Event.current.mousePosition.x - singlePrefab.origin.x) / Screen.width;
            float y = ((Event.current.mousePosition.y - singlePrefab.origin.y) / Screen.height) * -2 + 1;

            singlePrefab.transform.rotation = Quaternion.AngleAxis(720f * x, singlePrefab.normal);
            singlePrefab.transform.localScale = singlePrefab.scale * y;
        }
    }

    void ReleaseSinglePrefab()
    {
        singlePrefab.transform = null;
    }

    void IterateSinglePrefab()
    {
        if (prefabWindow.HasPrefab())
        {
            var prefab = prefabWindow.IteratePrefabs();

            GameObject obj = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            obj.transform.position = new Vector3(singlePrefab.transform.position.x, singlePrefab.transform.position.y, obj.transform.position.z);
            obj.transform.rotation = singlePrefab.transform.rotation;

            Undo.RegisterCreatedObjectUndo(obj, undoAction);

            DestroyImmediate(singlePrefab.transform.gameObject);

            obj.transform.parent = SetParent(prefabWindow);

            singlePrefab.transform = obj.transform;
            singlePrefab.scale = obj.transform.localScale;
        }
    }

    // Returns a random valid placement position
    Vector3 GetRandomPosition(Vector2 position)
    {
        return new Vector3(position.x + Random.Range(-spawner.size, spawner.size), position.y + Random.Range(-spawner.size, spawner.size), 0);
    }

    public Transform SetParent(PrefabSelectionWindow prefabWindow)
    {
        if (prefabWindow.prefabsTab[prefabWindow.selectedTab].parent)
        {
            GameObject parent = prefabWindow.prefabsTab[prefabWindow.selectedTab].parent as GameObject;
            return parent.transform;
        }

        return null;
    }
    #endregion

    #region Handle Prefabs Data
    public string ToJson()
    {
        return prefabWindow.ToJson(spawner.continuous, spawner.fillRate, prefabWindow.numberOfTabs);
    }

    void LoadFromJson(StreamReader data)
    {
        var json = JsonHelper.FromJson<Prefab>(data.ReadToEnd());
        data.Close();

        spawner.continuous = (bool)json.Item2[0];
        spawner.fillRate = (float)json.Item2[1];
        prefabWindow.numberOfTabs = (byte)json.Item2[2];

        prefabWindow.LoadFromJson(json.Item1);
    }
    #endregion
}
