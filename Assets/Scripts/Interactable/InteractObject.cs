using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractObject : Interactable
{
    
    [SerializeField] protected string questInfo = "";
    [SerializeField] private bool saves = false;
    [SerializeField] private Door doorToUnlock;


    protected override void Start()
    {
        base.Start();
        
        if (doorToUnlock != null)
        {
            doorToUnlock.SetDoorToLocked(true);
        }
    }


    

    // Update is called once per frame
    public override void Interact()
    {
        if (closeEnoughToInteract)
        {
            string randomMessage = "";
            if (messages.Count > 0 && messages[0] != "")
            {
                randomMessage = messages[0];
                if (messages.Count > 1)
                {
                    randomMessage = messages[Random.Range(0, messages.Count)];
                }
            }
            else
            {
                randomMessage = "NOT ASSIGNED, SHAME";
            }
            
            
            UIController.Instance.ShowMessage(randomMessage);

            if (saves)
            {
                UIController.Instance.SaveToMemory(questInfo);
            }
            if(doorToUnlock != null)
            {
                doorToUnlock.SetDoorToLocked(false);
            }
        }
    }

}
