using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private bool locked = false;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Collider2D doorCollider;
    private bool isOpen = false;

    public override void Interact()
    {
        base.Interact();
        if (!locked)
        {
            if (isOpen)
            {
                PlaySound("CloseDoor");
            }
            else
            {
                PlaySound("OpenDoor");
            }
            
            ToggleDoor();
            
        }
        if (messages.Count > 0 && messages[0] != "")
        {
            PlaySound("LockedDoor");
            UIController.Instance.ShowMessage(messages[0]);
        }
        


    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        doorAnimator.SetBool("Open", isOpen);
        doorCollider.enabled = !isOpen;
        
    }

    public void SetDoorToLocked(bool interactable)
    {
        locked = interactable;
    }
}
