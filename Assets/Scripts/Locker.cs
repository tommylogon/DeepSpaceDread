using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour

{

    
    // Set this in the Inspector to the layer you want to hide the player on
    public LayerMask hiddenLayer;


    // Store a reference to the player so we can manipulate its layer
    private GameObject player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Hide the player by setting its layer to the hidden layer
            player = other.gameObject;
            player.layer = (int)Mathf.Log(hiddenLayer.value, 2);
            player.GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponent<PlayerController>().ChangeFOVStatus(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object leaving the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Unhide the player by setting its layer back to the default layer
            player.layer = LayerMask.NameToLayer("Player");
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.GetComponent<PlayerController>().ChangeFOVStatus(true);
        }
    }
}
