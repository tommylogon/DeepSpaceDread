using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractObject : Interactable
{
    
    [SerializeField] private string questInfo = "";
    [SerializeField] private bool saves = false;
    [SerializeField] private Door doorToUnlock;

    
    private void Start()
    {
        if (doorToUnlock != null)
        {
            doorToUnlock.SetDoorToLocked(true);
        }
    }


    

    // Update is called once per frame
    public override void Interact()
    {
        if (canInteract)
        {
            UIController.Instance.ShowMessage(message);
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
