using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MessageDatabase))]
public class MessageDatabaseEditor : Editor
{
    private MessageDatabase messageDatabase;
    private string searchFilter = "";

    private void OnEnable()
    {
        messageDatabase = (MessageDatabase)target;
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector layout
        base.OnInspectorGUI();

        // Add a search bar
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Search Messages", EditorStyles.boldLabel);
        searchFilter = EditorGUILayout.TextField("Search by Key:", searchFilter);

        // Display filtered messages
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Filtered Messages", EditorStyles.boldLabel);
        DisplayFilteredMessages();
    }

    private void DisplayFilteredMessages()
    {
        if (string.IsNullOrEmpty(searchFilter))
        {
            return;
        }

        List<MessageDatabase.MessageEntry> filteredMessages = messageDatabase.messages.FindAll(
            entry => entry.key.ToLower().Contains(searchFilter.ToLower())
        );

        foreach (var message in filteredMessages)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Key: " + message.key);
            EditorGUILayout.TextArea(message.message, GUILayout.Height(50));
            EditorGUILayout.EndVertical();
        }
    }
}