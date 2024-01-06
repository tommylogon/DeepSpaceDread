using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    public Image fadeImage; // Assign this in the inspector with a black UIImage
    public float fadeDuration = 2f; // Duration in seconds for the fade

    void Start()
    {
        StartCoroutine(FadeToBlack(true));
    }

    IEnumerator FadeToBlack(bool fadeIn)
    {
        float elapsedTime = 0f;
        Color currentColor = fadeImage.color;
        float startAlpha = fadeIn ? 1f : 0f; // Start fully opaque if fading in, transparent if fading out
        float endAlpha = fadeIn ? 0f : 1f;   // End fully transparent if fading in, opaque if fading out

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            alpha = fadeIn ? Mathf.Lerp(startAlpha, endAlpha, alpha) : Mathf.Lerp(endAlpha, startAlpha, alpha);
            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        // Ensure it ends at the exact alpha value
        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, endAlpha);
    }
}
