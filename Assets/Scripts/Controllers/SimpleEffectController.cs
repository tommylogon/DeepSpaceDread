using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SimpleEffectController : MonoBehaviour
{
    [SerializeField] private ParticleSystem effect;
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Light2D light2D;
    [SerializeField] private float lightIntensity;
    [SerializeField] private float fadeDuration = 1f;



    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayEffect()
    {
        if (effect != null)
        {
            effect.Play();
        }

        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }

        if (light2D != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeLightIntensity());
        }
    }
    private IEnumerator FadeLightIntensity()
    {
        float initialIntensity = 0;
        light2D.intensity = lightIntensity;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            light2D.intensity = Mathf.Lerp(lightIntensity, initialIntensity, timer / fadeDuration);
            yield return null;
        }

        light2D.intensity = initialIntensity;
    }
}
