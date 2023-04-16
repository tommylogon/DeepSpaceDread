using UnityEditor;
using UnityEngine;

/// <summary>
/// Find the mouse position when it's over a SceneView.
/// </summary>
[InitializeOnLoad]
public class MouseHelper : Editor
{
    static Vector2 position;

    public static Vector2 Position
    {
        get { return position; }
    }

    static MouseHelper()
    {
        SceneView.duringSceneGui += UpdateView;
    }

    static void UpdateView(SceneView sceneView)
    {
        if (Event.current != null)
        {
            position = new Vector2(Event.current.mousePosition.x + sceneView.position.x, Event.current.mousePosition.y + sceneView.position.y);
        }
    }
}