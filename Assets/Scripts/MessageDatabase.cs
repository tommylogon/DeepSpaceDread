using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "MessageDatabase", menuName = "Messages/Message Database", order = 1)]
public class MessageDatabase : ScriptableObject
{
    [System.Serializable]
    public class MessageEntry
    {
        public string key;
        [TextArea(3, 10)]
        public string message;

        public Tag tag;
    }

    public List<MessageEntry> messages;

    public string GetMessage(string key)
    {
        foreach (MessageEntry entry in messages)
        {
            if (entry.key == key)
            {
                return entry.message;
            }
        }

        return "Message not found";
    }
    public enum Tag
    {
        General,
        Tutorial,
        Combat,
        UI,
        Body,
        Door,
        Panel,
        Special
        // Add more tags as needed
    }
}
