using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public void SetRandomRotation()
    {
        float randomZRotation = Random.Range(0f, 360f); // Adjust the range as needed
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, randomZRotation);
    }
}
