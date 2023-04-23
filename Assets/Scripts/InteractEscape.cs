using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractEscape : MonoBehaviour
{
    [SerializeField] private string interactText = "Press E to interact";
    [SerializeField] private string message = "";
    [SerializeField] private bool canInteract = false;
    [SerializeField] private bool onboard = false;

    [SerializeField] private Transform targetPos;

    private GameObject player;




    void Update()
    {

        if (canInteract && Input.GetKey(KeyCode.E))
        {
            player.transform.position = transform.position;
            player.GetComponent<SpriteRenderer>().sortingOrder = -2;


            
        }
        if (onboard)
        {
            Vector2.MoveTowards(transform.position, targetPos.position, 4 * Time.deltaTime);
            UIController.Instance.ShowGameOver(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            UIController.Instance.ShowMessage(interactText);
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            UIController.Instance.HideMessage();
            player = null;
        }
    }
}
