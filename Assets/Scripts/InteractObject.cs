using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractObject : MonoBehaviour
{
    [SerializeField] private string interactText = "Press E to interact";
    [SerializeField] private string message = "";
    [SerializeField] private bool canInteract = false;
    


    

    // Update is called once per frame
    void Update()
    {

        if(canInteract && Input.GetKey(KeyCode.E))
        {
            UIController.Instance.ShowMessage(message);
            //messageLabel.text = message;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            UIController.Instance.ShowMessage(interactText);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            UIController.Instance.HideMessage();

        }
    }
}
