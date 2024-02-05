using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionModule : MonoBehaviour, IInteractable
{
    public virtual void Interact()
    {
    }
}
