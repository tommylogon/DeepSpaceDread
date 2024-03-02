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

    public List<MessageEntry> messageList;

   

    public string GetMessage(string key)
    {
        foreach (MessageEntry entry in messageList)
        {
            if (entry.key == key)
            {
                return entry.message;
            }
        }

        return "Message not found";
    }

    public List<string> GetMessages(List<string> keys)
    {
        List<string> retrievedMessages = new List<string>();
        foreach (string key in keys)
        {
            retrievedMessages.Add(GetMessage(key));
        }

        return retrievedMessages;
    }
    public List<string> GetKeys()
    {
        List<string> keys = new List<string>();
        
            foreach (MessageEntry entry in messageList)
            {
                keys.Add(entry.key);
            }
            
        
        return keys;
    }

    public void ReplaceStringInMessage(string key,string whatToReplace, string replacement)
    {

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
