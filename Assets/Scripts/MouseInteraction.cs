using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseInteract();
    }

    private void MouseInteract()
    {
        Mouse currentMouse = Mouse.current;

        
            GameObject interactedObject = GetMouseOverObject(currentMouse.position.ReadValue());

            if (interactedObject != null)
            {

                Interactable interactable = interactedObject.GetComponent<Interactable>();

                if (interactable != null)
                {
                    interactable.ShowInteractablePrompt();

                }
            }
            else
            {
                UIController.Instance.HideInteraction();
            }
        
        
    }

    private GameObject GetMouseOverObject(Vector2 mousePosition)
    {
        Vector2 rayOrigin = Camera.main.ScreenToWorldPoint(mousePosition);
        Ray ray = new Ray(rayOrigin, Vector3.forward);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero, float.MaxValue, interactableLayer);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}
