using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://gamedevbeginner.com/how-to-zoom-a-camera-in-unity-3-methods-with-examples/
public class OrtheographicZoom : MonoBehaviour
{
    
        public Camera cam;
        public float maxZoom = 5;
        public float minZoom = 20;
        public float sensitivity = 1;
        public float speed = 30;
        float targetZoom;
        void Start()
        {
            targetZoom = cam.orthographicSize;
        }
        void Update()
        {
            targetZoom -= Input.mouseScrollDelta.y * sensitivity;
            targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);
            float newSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, speed * Time.deltaTime);
            cam.orthographicSize = newSize;
        }
    
}
