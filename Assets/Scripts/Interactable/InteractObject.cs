using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractObject : Interactable
{
    
    
    [SerializeField] private Door doorToUnlock;

    [SerializeField] private GameObject directionIndicator;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject indicatorTarget;

    protected override void Start()
    {
        base.Start();
        
        if (doorToUnlock != null)
        {
            doorToUnlock.SetDoorToLocked(true);
        }
        if (indicatorPrefab != null)
        {
            directionIndicator = Instantiate(indicatorPrefab);
            directionIndicator.GetComponent<DirectionIndicator>().SetDistances(0.5f, 7);
        }
        SetIndicator();


    }

    private void SetIndicator()
    {
        if (directionIndicator != null)
        {
            
            if(indicatorTarget != null)
            {
                directionIndicator.GetComponent<DirectionIndicator>().SetTarget(gameObject);
            }
            
            if(playerRef != null)
            {
                directionIndicator.GetComponent<DirectionIndicator>().SetPlayer(playerRef.gameObject);
            }
            
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
        SetIndicator();
    }

}
