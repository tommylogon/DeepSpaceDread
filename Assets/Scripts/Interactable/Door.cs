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
        if (locked)
        {
            if(messages.Count > 0 && messages[0] != "")
            {

            }
            UIController.Instance.ShowMessage(messages[0]);
            return;
        }

        base.Interact();
        ToggleDoor();
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
