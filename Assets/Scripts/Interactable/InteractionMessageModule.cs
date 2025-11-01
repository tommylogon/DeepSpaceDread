using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMessageModule : InteractionModule
{
    [SerializeField]public List<string> messageKeys;
    
    [Tooltip("The database where all messages are stored.")]    
    public MessageDatabase messageDatabase;
    public MessageDatabase.Tag interactionTagType;
    [SerializeField] private bool playRandomMessage;

    [SerializeField] private string questInfo = "";
    [SerializeField] private bool saves = false;



    // Start is called before the first frame update




    public override void Interact()
    {
        if (messageDatabase == null || messageKeys.Count == 0)
        {
            Debug.LogWarning("InteractionMessageModule on " + gameObject.name + " is not configured correctly. Assign a MessageDatabase and at least one key.");
            return;
        }

        string keyToPlay = "";

        // Decide which key to use based on your improved logic
        if (playRandomMessage && messageKeys.Count > 1)
        {
            // If random is ticked and we have multiple options, pick a random key
            int randomIndex = Random.Range(0, messageKeys.Count);
            keyToPlay = messageKeys[randomIndex];
        }
        else
        {
            // Otherwise, just use the first key in the list.
            // This works for single-item lists and multi-item lists where random is off.
            keyToPlay = messageKeys[0];
        }

        // Get the message from the database using the selected key
        string messageToShow = messageDatabase.GetMessage(keyToPlay);

        // Show the message on the UI
        UIController.Instance.ShowMessage(messageToShow);

        // --- End of New Logic ---

        // The logic for saving information remains the same
        SaveToInformationLog();
    }
    
    // This method can still be useful if other scripts need to trigger a specific message.
    public void PlaySpecificMessage(string key)
    {
        if (messageDatabase != null)
        {
            string messageToShow = messageDatabase.GetMessage(key);
            UIController.Instance.ShowMessage(messageToShow);
        }
    }

    private void SaveToInformationLog()
    {
        if (saves)
        {
                UIController.Instance.SaveToMemory(questInfo);
        }
    }
    
}
