using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractObject : MonoBehaviour
{
    [SerializeField] private string interactText = "Press E to interact";
    [SerializeField] private string message = "";
    [SerializeField] private string questInfo = "";

    [SerializeField] private bool canInteract = false;
    [SerializeField] private bool Saves = false;

    


    

    // Update is called once per frame
    void Update()
    {

        if(canInteract && Input.GetKey(KeyCode.E))
        {
            UIController.Instance.ShowMessage(message);
            if (Saves)
            {
                UIController.Instance.SaveToMemory(questInfo);
            }
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
