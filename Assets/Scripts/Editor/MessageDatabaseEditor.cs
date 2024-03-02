using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MessageDatabase;

[CustomEditor(typeof(MessageDatabase))]
public class MessageDatabaseEditor : Editor
{
    private MessageDatabase messageDatabase;
    private string searchFilter = "";
    private Tag filterTag = Tag.General;
    private string newKey = "";
    private string newMessage = "";
    private Tag newTag = Tag.General;

    private string jsonFilePath = "Assets/messages.json";

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

        // Add a tag filter
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Filter by Tag", EditorStyles.boldLabel);
        filterTag = (Tag)EditorGUILayout.EnumPopup("Tag:", filterTag);

        // Add new message section
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Add New Message", EditorStyles.boldLabel);
        newKey = EditorGUILayout.TextField("Key:", newKey);
        newTag = (Tag)EditorGUILayout.EnumPopup("Tag:", newTag);
        newMessage = EditorGUILayout.TextArea(newMessage, GUILayout.Height(50));

        if (GUILayout.Button("Add New"))
        {
            AddNewMessage();
        }

        // Display all messages
        GUILayout.Space(10);
        EditorGUILayout.LabelField("All Messages", EditorStyles.boldLabel);
        DisplayAllMessages();

        // Display filtered messages
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Filtered Messages", EditorStyles.boldLabel);
        DisplayMessages();

        // Validation and error checking
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Validation and Error Checking", EditorStyles.boldLabel);
        if (GUILayout.Button("Check for Errors"))
        {
            ValidateMessages();
        }

        

       

        GUILayout.Space(10);
        EditorGUILayout.LabelField("JSON Operations", EditorStyles.boldLabel);
        if (GUILayout.Button("Write to JSON File"))
        {
            WriteToJsonFile();
        }

        // Read from JSON file button and file selection
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Read from JSON File", EditorStyles.boldLabel);
        if (GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }
        if (GUILayout.Button("Read from JSON File"))
        {
            ReadFromJsonFile();
        }

        
    }
    
    private void DisplayMessages()
    {
        if (string.IsNullOrEmpty(searchFilter) && filterTag == Tag.General)
        {
            DisplayAllMessages();
            return;
        }

        foreach(var entry in messageDatabase.messageList)
        {
            
            if((entry.key.ToLower().Contains(searchFilter.ToLower()) || string.IsNullOrEmpty(searchFilter)) && entry.tag == filterTag)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("Key:", entry.key);
                entry.tag = (Tag)EditorGUILayout.EnumPopup("Tag:", entry.tag);
                entry.message = EditorGUILayout.TextArea(entry.message, GUILayout.Height(50));
                EditorGUILayout.EndVertical();
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(messageDatabase);
        }
    }

    private void DisplayAllMessages()
    {
        EditorGUI.BeginChangeCheck();

        foreach (var entry in messageDatabase.messageList)
        {

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Key:", entry.key);
            entry.tag = (Tag)EditorGUILayout.EnumPopup("Tag:", entry.tag);
            entry.message = EditorGUILayout.TextArea(entry.message, GUILayout.Height(50));
            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(messageDatabase);
        }
    }

    private void DisplayFilteredMessages()
    {
        if (string.IsNullOrEmpty(searchFilter) && filterTag == Tag.General)
        {
            return;
        }

        List<MessageDatabase.MessageEntry> filteredMessages = messageDatabase.messageList.FindAll(
            entry => (entry.key.ToLower().Contains(searchFilter.ToLower()) || string.IsNullOrEmpty(searchFilter)) &&
                      entry.tag == filterTag
        );

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < filteredMessages.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            filteredMessages[i].key = EditorGUILayout.TextField("Key:", filteredMessages[i].key);
            filteredMessages[i].tag = (Tag)EditorGUILayout.EnumPopup("Tag:", filteredMessages[i].tag);
            filteredMessages[i].message = EditorGUILayout.TextArea(filteredMessages[i].message, GUILayout.Height(50));
            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(messageDatabase);
        }
    }
    private void ValidateMessages()
    {
        // Check for duplicate keys
        var duplicateKeys = messageDatabase.messageList.GroupBy(x => x.key)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        if (duplicateKeys.Any())
        {
            Debug.LogError("Duplicate keys found: " + string.Join(", ", duplicateKeys));
        }
        else
        {
            Debug.Log("No duplicate keys found.");
        }

        // Check for empty keys or messages
        bool emptyKeyFound = false;
        bool emptyMessageFound = false;
        foreach (var message in messageDatabase.messageList)
        {
            if (string.IsNullOrEmpty(message.key))
            {
                emptyKeyFound = true;
            }

            if (string.IsNullOrEmpty(message.message))
            {
                emptyMessageFound = true;
            }
        }

        if (emptyKeyFound)
        {
            Debug.LogError("Empty keys found.");
        }
        else
        {
            Debug.Log("No empty keys found.");
        }

        if (emptyMessageFound)
        {
            Debug.LogError("Empty messages found.");
        }
        else
        {
            Debug.Log("No empty messages found.");
        }
    }
    private void AddNewMessage()
    {
        if (string.IsNullOrEmpty(newKey) || string.IsNullOrEmpty(newMessage))
        {
            Debug.LogWarning("Both key and message must be non-empty to add a new message.");
            return;
        }

        if (messageDatabase.messageList.Any(entry => entry.key == newKey))
        {
            Debug.LogWarning($"A message with the key '{newKey}' already exists.");
            return;
        }

        messageDatabase.messageList.Add(new MessageEntry
        {
            key = newKey,
            tag = newTag,
            message = newMessage
        });

        // Clear the input fields
        newKey = "";
        newMessage = "";
        newTag = Tag.General;

        EditorUtility.SetDirty(messageDatabase);
    }

    private void WriteToJsonFile()
    {
        string json = JsonUtility.ToJson(messageDatabase);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log("Messages written to JSON file: " + jsonFilePath);
    }

    private void ReadFromJsonFile()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            JsonUtility.FromJsonOverwrite(json, messageDatabase);
            Debug.Log("Messages read from JSON file: " + jsonFilePath);
        }
        else
        {
            Debug.LogError("JSON file does not exist: " + jsonFilePath);
        }
    }
}