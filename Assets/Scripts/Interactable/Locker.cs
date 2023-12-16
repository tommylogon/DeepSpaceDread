using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : Interactable

{
   

    // Set this in the Inspector to the layer you want to hide the player on
    public LayerMask hiddenLayer;
    
    private bool playerInsideLocker = false;

    private Resource lockerHealth;

    protected override void Start()
    {
        lockerHealth = GetComponent<Resource>();
        base.Start();
    }

    private void Update()
    {
        if(playerInsideLocker)
        {
            player.transform.position = transform.position;
        }
    }
    public override void Interact()
    {
        base.Interact();

        if (!playerInsideLocker)
        {
            // Hide the player by setting its layer to the hidden layer
            player.layer = (int)Mathf.Log(hiddenLayer.value, 2);
            player.GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponent<PlayerController>().ChangeFlashlighStatus(false);

            playerInsideLocker = true;
            player.GetComponent<PlayerController>().IsInsideLocker = true;
        }
        else
        {
            // Unhide the player by setting its layer back to the default layer
            player.layer = LayerMask.NameToLayer("Player");
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.GetComponent<PlayerController>().ChangeFlashlighStatus(true);

            playerInsideLocker = false;
            player.GetComponent<PlayerController>().IsInsideLocker = false;
        }
        
        
    }
}
