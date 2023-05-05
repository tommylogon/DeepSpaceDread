using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject throwableObjectInventory;

    public void AddToInventory(GameObject pickupItem)
    {
        if (pickupItem != null)
        {
            if (throwableObjectInventory != null)
            {
                throwableObjectInventory.SetActive(true);
            }
            throwableObjectInventory = pickupItem;
        }
    }

    public GameObject GetThrowableObject()
    {
        return throwableObjectInventory;
    }

    public void ClearThrowableObject()
    {
        throwableObjectInventory = null;
    }
}
