using System.Collections;
using System.Collections.Generic;
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
}
