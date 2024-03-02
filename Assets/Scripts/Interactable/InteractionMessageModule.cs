using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMessageModule : InteractionModule
{
    public List<string> messages;
    public List<string> messageKeys;
    public MessageDatabase messageDatabase;
    public MessageDatabase.Tag interactionTagType;
    [SerializeField] protected bool playRandomMessage;

    [SerializeField] protected string questInfo = "";
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
        PlayMessage();
    }
    public  void Interact(string key)
    {

    }
    protected void PlayMessage(int messageIndex = 0, int messageRange = 1)
    {
        
            string selectedMessage = "It's Empty...";
            if (messages.Count > 0 && messages[0] != "")
            {
                if (playRandomMessage || messageRange > 1)
                {

                    if (messages.Count > 1)
                    {
                        selectedMessage = messages[Random.Range(0, messageRange)];
                        if (selectedMessage == "")
                        {
                            selectedMessage = "Ugh...";
                        }
                    }

                }
                else
                {
                    selectedMessage = messages[messageIndex];
                }
            }
            UIController.Instance.ShowMessage(selectedMessage);

            if (saves)
            {
                UIController.Instance.SaveToMemory(questInfo);
            }

        
    }
}
