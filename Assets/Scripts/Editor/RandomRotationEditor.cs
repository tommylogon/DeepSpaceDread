using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RotateObject))]
[CanEditMultipleObjects]
public class RandomRotationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Set Random Rotation"))
        {
            foreach (var targetObject in targets)
            {
                RotateObject rotateObject = (RotateObject)targetObject;
                Undo.RecordObject(rotateObject.transform, "Set Random Rotation");

                // Set random rotation for each selected object
                rotateObject.SetRandomRotation();
            }
        }
    }
}
