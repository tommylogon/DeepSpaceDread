using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickerLight : MonoBehaviour
{
    public Light2D light2D;
    public float minIntensity = 0.1f;
    public float maxIntensity = 1f;
    public float flickerSpeed = 0.1f;
    public float turnOffChance = 0.1f; // Chance to turn off the light (0-1 range)
    public float minOffTime = 1f; // Minimum time the light stays off
    public float maxOffTime = 3f; // Maximum time the light stays off
    

    private float randomizer;

    void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            

                float randomValue = Random.value;
                if (randomValue < turnOffChance)
                {
                    light2D.intensity = 0;
                    float offTime = Random.Range(minOffTime, maxOffTime);
                    yield return new WaitForSeconds(offTime);
                    light2D.intensity = 1;
                }

                float randomIntensity = Random.Range(minIntensity, maxIntensity);
                light2D.intensity = randomIntensity;
                
            
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}
