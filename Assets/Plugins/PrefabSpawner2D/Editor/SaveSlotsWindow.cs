using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveSlotsWindow : EditorWindow
{
    public static string slotsSetName;
    readonly string saveFilePath = "Assets\\Plugins\\PrefabSpawner2D\\Editor\\Data\\";

    public static void ShowWindow()
    {
        SaveSlotsWindow window = GetWindow<SaveSlotsWindow>(true, "Save Slots", true);
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 290, 10);
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        slotsSetName = EditorGUILayout.TextField("Slots Set Name:", slotsSetName);

        if (GUILayout.Button("Save"))
        {
            PrefabSpawner2DEditor editor = GetWindow<PrefabSpawner2DEditor>();
            string fileName = $"{saveFilePath}{slotsSetName}.json";

            Directory.CreateDirectory(saveFilePath);

            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }
            else
            {
                bool confirm = EditorUtility.DisplayDialog("Save Slots", $"The file '{slotsSetName}' already exists.\nDo you want to override it?", "Yes", "No");

                if (!confirm)
                {
                    return;
                }
            }

            File.WriteAllText(fileName, editor.ToJson());
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }
}
