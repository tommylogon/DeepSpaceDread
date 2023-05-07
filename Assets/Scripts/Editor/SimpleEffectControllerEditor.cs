using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleEffectController))]
public class SimpleEffectControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleEffectController simpleEffectController = (SimpleEffectController)target;
        if (GUILayout.Button("Play Effect"))
        {
            simpleEffectController.PlayEffect();
        }
    }
}
