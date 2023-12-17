using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractObject : Interactable
{
    
    
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
        PlayMessage(0);

        if (doorToUnlock != null)
        {
            doorToUnlock.SetDoorToLocked(false);
        }
    }

}
