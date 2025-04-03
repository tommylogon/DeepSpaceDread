using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMessageModule : InteractionModule
{
    [SerializeField]private List<string> messageKeys;
    private List<string> messages;
    
    private MessageDatabase messageDatabase;
    public MessageDatabase.Tag interactionTagType;
    [SerializeField] private bool playRandomMessage;

    [SerializeField] private string questInfo = "";
    [SerializeField] private bool saves = false;

    

    // Start is called before the first frame update
    void Start()
    {
        if (messageDatabase != null)
        {
            
            messages = messageDatabase.GetMessages(messageKeys);
        }
       

    }



    public override void Interact()
    {
        PlayMessage("Default");
    }
    public  void Interact(string key)
    {
        PlayMessage(key);
    }
    private void PlayMessage(string key)
    {
        
        string selectedMessage = "Empty";
        if (messages != null && messages.Count > 0 && messages[0] != "")
        {
            if(playRandomMessage)
            {
                selectedMessage = PlayRandomMessage();
            }
            else
            {
                selectedMessage = messageDatabase.GetMessage(key);
            }
        }
        UIController.Instance.ShowMessage(selectedMessage);
    }
    private string PlayRandomMessage()
    {
        System.Random random = new System.Random();
        if(playRandomMessage && messages.Count > 1)
        {
            return messages[random.Next(0, messages.Count)];
        }
        return "";
    }
    private void SaveToInformationLog()
    {
        if (saves)
        {
                UIController.Instance.SaveToMemory(questInfo);
        }
    }
    
}
