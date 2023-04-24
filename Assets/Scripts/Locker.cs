using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : Interactable

{
   

    // Set this in the Inspector to the layer you want to hide the player on
    public LayerMask hiddenLayer;

    private bool playerInsideLocker = false;

  
    public override void Interact()
    {
        base.Interact();

        if (!playerInsideLocker)
        {
            // Hide the player by setting its layer to the hidden layer
            player.layer = (int)Mathf.Log(hiddenLayer.value, 2);
            player.GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponent<PlayerController>().ChangeFOVStatus(false);

            playerInsideLocker = true;
        }
        else
        {
            // Unhide the player by setting its layer back to the default layer
            player.layer = LayerMask.NameToLayer("Player");
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.GetComponent<PlayerController>().ChangeFOVStatus(true);

            playerInsideLocker = false;
        }
    }
}
