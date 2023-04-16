using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PrefabSelectionWindow
{
    [System.Serializable]
    public class Prefab
    {
        [System.NonSerialized] public Rect rect;
        public byte tab;
        public string parent;
        [System.NonSerialized] public Object gameObject;
        public string path;
        public bool active = true;
        public float slider = 1.0f;
        [System.NonSerialized] public float percent;
        public bool randomPositionToggle = false;
        public bool randomRotationToggle = false;
        public bool[] randomRotationXYZ = new bool[3] { false, false, false };
        public bool randomScaleToggle = false;
        public float randomScaleMin = 0.1f;
        public float randomScaleMax = 1.0f;

        public Quaternion GetRotation()
        {
            float[] randomEuler = { 0.0f, 0.0f, 0.0f };

            for (int i = 0; i < 3; ++i)
            {
                if (randomRotationXYZ[i])
                {
                    randomEuler[i] = Random.Range(0, 361);
                }
            }

            return Quaternion.identity * Quaternion.Euler(randomEuler[0], randomEuler[1], randomEuler[2]);
        }
    };

    public class PrefabsTab
    {
        public byte index = 0;
        [SerializeField] public List<Prefab> prefabs = new List<Prefab>();
        public int prefabIter = 0;
        [SerializeField] public Object parent;
        [SerializeField] public Vector2 menuScroll;

        public PrefabsTab(byte index)
        {
            this.index = index;
        }
    }

    [SerializeField] public List<PrefabsTab> prefabsTab;
    [SerializeField]
    public Dictionary<byte, char> tabsNames = new Dictionary<byte, char>()
    {
        {1, 'A' },
        {2, 'B' },
        {3, 'C' },
        {4, 'D' },
        {5, 'E' },
        {6, 'F' },
        {7, 'G' },
        {8, 'H' },
        {9, 'I' },
        {10, 'J' },
        {11, 'K' },
        {12, 'L' },
    };
    public int selectedTab = 0;
    public byte numberOfTabs;

    static bool optionsFolded;

    public PrefabSelectionWindow()
    {
        numberOfTabs++;
        prefabsTab = new List<PrefabsTab>
        {
            new PrefabsTab(numberOfTabs)
        };
    }

    public void DrawGUI(int gridWidth)
    {
        prefabsTab[selectedTab].parent = EditorGUILayout.ObjectField(new GUIContent("Parent"), prefabsTab[selectedTab].parent, typeof(GameObject), true);

        #region Tabs
        EditorGUILayout.BeginHorizontal();

        string[] tabNames = prefabsTab.Select(t => tabsNames[t.index].ToString()).ToArray();
        selectedTab = GUILayout.Toolbar(selectedTab, tabNames, EditorStyles.toolbarButton);

        GUIStyle buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
        buttonStyle.normal.textColor = Color.magenta;
        buttonStyle.fixedWidth = 18;

        if (GUILayout.Button("-", buttonStyle) && numberOfTabs > 1)
        {
            if (selectedTab == prefabsTab.Count - 1)
            {
                selectedTab--;
            }

            numberOfTabs--;
            prefabsTab.RemoveAt(prefabsTab.Count - 1);
        }

        if (GUILayout.Button("+", buttonStyle) && numberOfTabs < 12)
        {
            numberOfTabs++;
            prefabsTab.Add(new PrefabsTab(numberOfTabs));
        }

        EditorGUILayout.EndHorizontal();
        #endregion

        #region Slots Buttons
        // Adds another prefab slot
        if (GUILayout.Button("Add\nPrefab Slot"))
        {
            prefabsTab[selectedTab].prefabs.Add(new Prefab());
        }

        // Clear all prefab slots
        if (prefabsTab[selectedTab].prefabs.Count > 0 && GUILayout.Button("Clear Slots"))
        {
            bool confirm = EditorUtility.DisplayDialog("Clear all slots", "Do you want to clear all the prefabs slots?", "Yes", "No");

            if (confirm)
            {
                prefabsTab[selectedTab].prefabs.Clear();
            }
        }

        if (prefabsTab[selectedTab].prefabs.Count > 0)
        {
            EditorGUILayout.Separator();
        }
        #endregion

        #region Prefabs Scroll Menu
        prefabsTab[selectedTab].menuScroll = EditorGUILayout.BeginScrollView(prefabsTab[selectedTab].menuScroll);
        EditorGUILayout.BeginHorizontal();

        // Keep track of the slider values
        float sliderTot = 0.0f;

        // Draw prefabs options
        for (int i = 0; i < prefabsTab[selectedTab].prefabs.Count; ++i)
        {
            if (0 == i % gridWidth && i > 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
            }
            prefabsTab[selectedTab].prefabs[i].rect = EditorGUILayout.BeginVertical(GUILayout.Width(125));
            prefabsTab[selectedTab].prefabs[i].rect = EditorGUILayout.BeginHorizontal();
            prefabsTab[selectedTab].prefabs[i].rect = EditorGUILayout.BeginVertical();
            GUILayout.Label(AssetPreview.GetAssetPreview(prefabsTab[selectedTab].prefabs[i].gameObject), GUILayout.Width(110), GUILayout.Height(110));
            if (GUI.Button(new Rect(prefabsTab[selectedTab].prefabs[i].rect.x + 85, prefabsTab[selectedTab].prefabs[i].rect.y + 5.0f, 20, 20), "X", new GUIStyle(EditorStyles.toolbarButton)))
            {
                prefabsTab[selectedTab].prefabs.RemoveAt(i--);
                continue;
            }
            if (prefabsTab[selectedTab].prefabs[i].gameObject)
            {
                Rect rect = prefabsTab[selectedTab].prefabs[i].rect;
                prefabsTab[selectedTab].prefabs[i].active = GUI.Toggle(new Rect(rect.x + 7.5f, rect.y + 5.0f, 100, 90), prefabsTab[selectedTab].prefabs[i].active, "");
                string percent = (prefabsTab[selectedTab].prefabs[i].percent * 100).ToString();
                percent = ((percent.Length > 4) ? percent.Substring(0, 4) : percent) + "%";
                EditorGUI.LabelField(new Rect(rect.x + 78, rect.y + 92, 40, 20), percent);
            }
            EditorGUILayout.EndVertical();
            prefabsTab[selectedTab].prefabs[i].slider = GUILayout.VerticalSlider(prefabsTab[selectedTab].prefabs[i].slider, 1.0f, 0.0f, GUILayout.Height(110));
            sliderTot += prefabsTab[selectedTab].prefabs[i].gameObject ? prefabsTab[selectedTab].prefabs[i].slider : 0f;
            EditorGUILayout.EndHorizontal();

            // GameObject
            prefabsTab[selectedTab].prefabs[i].gameObject = EditorGUILayout.ObjectField(prefabsTab[selectedTab].prefabs[i].gameObject, typeof(GameObject), false);

            // Options
            optionsFolded = EditorGUILayout.Foldout(optionsFolded, "Options");
            if (optionsFolded)
            {
                EditorGUILayout.BeginHorizontal();
                prefabsTab[selectedTab].prefabs[i].randomPositionToggle = EditorGUILayout.Toggle(prefabsTab[selectedTab].prefabs[i].randomPositionToggle, GUILayout.Width(10));
                EditorGUILayout.LabelField("Random Position", GUILayout.Width(105));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                prefabsTab[selectedTab].prefabs[i].randomRotationToggle = EditorGUILayout.Toggle(prefabsTab[selectedTab].prefabs[i].randomRotationToggle, GUILayout.Width(10));
                EditorGUILayout.LabelField("Random Rotation", GUILayout.Width(105));
                EditorGUILayout.EndHorizontal();
                if (prefabsTab[selectedTab].prefabs[i].randomRotationToggle)
                {
                    EditorGUILayout.BeginHorizontal();
                    prefabsTab[selectedTab].prefabs[i].randomRotationXYZ[0] = EditorGUILayout.ToggleLeft("X", prefabsTab[selectedTab].prefabs[i].randomRotationXYZ[0], GUILayout.Width(25));
                    prefabsTab[selectedTab].prefabs[i].randomRotationXYZ[1] = EditorGUILayout.ToggleLeft("Y", prefabsTab[selectedTab].prefabs[i].randomRotationXYZ[1], GUILayout.Width(25));
                    prefabsTab[selectedTab].prefabs[i].randomRotationXYZ[2] = EditorGUILayout.ToggleLeft("Z", prefabsTab[selectedTab].prefabs[i].randomRotationXYZ[2], GUILayout.Width(25));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                prefabsTab[selectedTab].prefabs[i].randomScaleToggle = EditorGUILayout.Toggle(prefabsTab[selectedTab].prefabs[i].randomScaleToggle, GUILayout.Width(10));
                EditorGUILayout.LabelField("Random Scale", GUILayout.Width(105));
                EditorGUILayout.EndHorizontal();
                if (prefabsTab[selectedTab].prefabs[i].randomScaleToggle)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Min", GUILayout.Width(60));
                    EditorGUILayout.LabelField("Max", GUILayout.Width(60));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    prefabsTab[selectedTab].prefabs[i].randomScaleMin = EditorGUILayout.FloatField(prefabsTab[selectedTab].prefabs[i].randomScaleMin, GUILayout.Width(60));
                    prefabsTab[selectedTab].prefabs[i].randomScaleMax = EditorGUILayout.FloatField(prefabsTab[selectedTab].prefabs[i].randomScaleMax, GUILayout.Width(60));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
        #endregion

        #region Percentages
        if (sliderTot > 0.0f)
        {
            for (int i = 0; i < prefabsTab[selectedTab].prefabs.Count; ++i)
            {
                prefabsTab[selectedTab].prefabs[i].percent = prefabsTab[selectedTab].prefabs[i].slider / sliderTot;
            }
        }
        else
        {
            for (int i = 0; i < prefabsTab[selectedTab].prefabs.Count; ++i)
            {
                prefabsTab[selectedTab].prefabs[i].percent = 0.0f;
            }
        }
        #endregion
    }

    // Check if we have active prefabs available
    public bool HasPrefab()
    {
        if (prefabsTab[selectedTab].prefabs.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < prefabsTab[selectedTab].prefabs.Count; ++i)
        {
            if (prefabsTab[selectedTab].prefabs[i].active && prefabsTab[selectedTab].prefabs[i].gameObject && prefabsTab[selectedTab].prefabs[i].percent > 0.0f)
            {
                return true;
            }
        }

        return false;
    }

    // Return a valid prefab
    public Prefab GetPrefab()
    {
        var validPrefabs = new List<ProportionValue<Prefab>>();

        var activePrefabs = prefabsTab[selectedTab].prefabs.Where(p => p.gameObject && p.active);

        if (activePrefabs.Count() > 1)
        {
            foreach (Prefab prefab in activePrefabs)
            {
                if (prefab.percent > 0.0f)
                {
                    validPrefabs.Add(ProportionValue.Create(prefab.percent, prefab));
                }
            }

            return validPrefabs.ChooseRandomly();
        }

        return activePrefabs.First();
    }

    // Return the next prefab in line
    public Prefab IteratePrefabs()
    {
        return prefabsTab[selectedTab].prefabIter < prefabsTab[selectedTab].prefabs.Count ? prefabsTab[selectedTab].prefabs[prefabsTab[selectedTab].prefabIter++] : prefabsTab[selectedTab].prefabs[(prefabsTab[selectedTab].prefabIter = 0)];
    }

    #region Handle Prefabs Data
    public string ToJson(params object[] settings)
    {
        List<Prefab> prefabsToSave = new List<Prefab>();

        for (int i = 0; i < numberOfTabs; i++)
        {
            foreach (Prefab prefab in prefabsTab[i].prefabs)
            {
                prefabsToSave.Add(new Prefab()
                {
                    tab = (byte)(i + 1),
                    parent = prefabsTab[i].parent?.name,
                    path = prefab.gameObject != null ? AssetDatabase.GetAssetPath(prefab.gameObject.GetInstanceID()) : null,
                    active = prefab.active,
                    slider = prefab.slider,
                    randomPositionToggle = prefab.randomPositionToggle,
                    randomRotationToggle = prefab.randomRotationToggle,
                    randomRotationXYZ = prefab.randomRotationXYZ,
                    randomScaleToggle = prefab.randomScaleToggle,
                    randomScaleMin = prefab.randomScaleMin,
                    randomScaleMax = prefab.randomScaleMax
                });
            }
        }

        string json = JsonHelper.ToJson(prefabsToSave.ToArray(), false, settings);

        return json.ToString();
    }

    public void LoadFromJson(Prefab[] data)
    {
        ResetTabs();

        foreach (var loadedPrefab in data)
        {
            Prefab prefab = loadedPrefab;
            prefab.gameObject = AssetDatabase.LoadAssetAtPath<Object>(prefab.path);

            if (prefabsTab[prefab.tab - 1].parent == null || prefabsTab[prefab.tab - 1].parent.name != prefab.parent)
            {
                if (!string.IsNullOrEmpty(prefab.parent))
                {
                    prefabsTab[prefab.tab - 1].parent = GameObject.Find(prefab.parent);
                }
            }

            prefabsTab[prefab.tab - 1].prefabs.Add(prefab);
        }
    }

    void ResetTabs()
    {
        prefabsTab.Clear();
        for (byte i = 0; i < numberOfTabs; i++)
        {
            prefabsTab.Add(new PrefabsTab((byte)(i + 1)));
        }

        foreach (var tab in prefabsTab)
        {
            tab.prefabs.Clear();
        }
    }
    #endregion
}
