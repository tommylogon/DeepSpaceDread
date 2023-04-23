using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float delay = 0.01f;
    public string fullText;

    private string currentText = "";

    private Label label;
    private Coroutine showTextCoroutine;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void StartTextWriter(Label lableToWrite, string textToWrite)
    {
        label = lableToWrite;
        fullText = textToWrite;
        // Stop the previous coroutine if it's still running
        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);
        }

        // Start the new coroutine and store a reference to it
        showTextCoroutine = StartCoroutine(ShowText());
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
    // Check if the ShowText coroutine has finished
    public bool IsTextWriterFinished()
    {
        return showTextCoroutine == null;
    }
}
