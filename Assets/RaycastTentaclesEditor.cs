using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RaycastTentacles))]
public class RaycastTentaclesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector

        RaycastTentacles script = (RaycastTentacles)target;

        // Button to spawn tentacles
        if (GUILayout.Button("Spawn Tentacles") )
        {
            script.SpawnTentacles();
        }
    }
}
