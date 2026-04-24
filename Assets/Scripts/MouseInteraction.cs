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
        if (Camera.main == null)
        {
            Debug.LogError("No camera tagged 'MainCamera' in scene!");
            return null;
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint, interactableLayer);

        return hit ? hit.gameObject : null;
    }
}
