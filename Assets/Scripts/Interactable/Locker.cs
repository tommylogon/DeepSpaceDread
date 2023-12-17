using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Locker : Interactable

{
   

    // Set this in the Inspector to the layer you want to hide the player on
    public LayerMask hiddenLayer;
    
    private bool lockerIsOccupied = false;

    private Resource lockerHealth;

    private Light2D lockerLight;

    protected override void Start()
    {
        lockerHealth = GetComponent<Resource>();
        lockerLight = GetComponentInChildren<Light2D>();
        base.Start();
    }

    private void Update()
    {
        if(lockerIsOccupied)
        {
            player.transform.position = transform.position;
        }
    }
    public override void Interact()
    {
        base.Interact();
        if(lockerHealth != null && lockerHealth.GetValue() > 0) 
        {
            if (!lockerIsOccupied)
            {
                lockerIsOccupied = true;
                PlayMessage(0,2);

            }
            else
            {
                lockerIsOccupied = false;
            }
            PlaySound("LockerOpen");
            
        }
        else
        {
            lockerIsOccupied = false;

            PlaySound("LockerBroken");
            playRandomMessage = false;
            PlayMessage(3);
        }
        

        player.ShowPlayer(!lockerIsOccupied);

    }


    public void TakeDamage(int damage)
    {
        lockerHealth.ReduceRecource(damage);
        lockerLight.color = Color.red;
    }

}
