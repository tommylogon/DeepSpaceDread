using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractionMessageModule))]
public class InteractionMessageModuleEditor : Editor
{
    private InteractionMessageModule messageModule;
    private string selectedKey = "";
    private int selectedIndex = 0;
    private void OnEnable()
    {
        messageModule = (InteractionMessageModule)target;
    }

    public override void OnInspectorGUI()
    {        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Message Key", EditorStyles.boldLabel);
        // Display dropdown of possible keys
        if (messageModule.messageDatabase != null && messageModule.messageDatabase.messageList != null && messageModule.messageDatabase.messageList.Count > 0)
        {
            var keysForTag = messageModule.messageDatabase.messageList.Where(entry => entry.tag == messageModule.interactionTagType)
                                                              .Select(entry => entry.key)
                                                              .ToArray();

            // Ensure there are keys for the selected tag
            if (keysForTag.Length == 0)
            {
                EditorGUILayout.HelpBox("No keys available for the selected tag.", MessageType.Info);
                return;
            }
            var keysToAdd = keysForTag.Except(messageModule.messageKeys).ToArray();
            selectedIndex = EditorGUILayout.Popup("Key:", selectedIndex, keysToAdd);
            if (selectedIndex >= 0 && selectedIndex < keysToAdd.Length)
            {
                selectedKey = keysToAdd[selectedIndex];
            }
            // Add button to add selected key
            if (GUILayout.Button("Add Key"))
            {
                AddSelectedKey();
            }
        }
    }
    private void AddSelectedKey()
    {
        if (!string.IsNullOrEmpty(selectedKey) && !messageModule.messageKeys.Contains(selectedKey))
        {
            messageModule.messageKeys.Add(selectedKey);
            selectedKey = ""; // Clear selected key after adding
        }
    }
}
