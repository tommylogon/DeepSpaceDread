using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float delay = 0.1f;
    public string fullText;

    private string currentText = "";

    private Label label;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void StartTextWriter(Label lableToWrite, string textToWrite)
    {
        label = lableToWrite;
        fullText = textToWrite;
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            label.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
}
