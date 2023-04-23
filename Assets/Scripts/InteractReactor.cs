using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class InteractReactor : MonoBehaviour
{
    public GameObject[] objectsToDeactivate;
    public Light2D[] lightsToChange;
    public AudioSource[] audioSources;

    [SerializeField] private string interactText = "Press E to interact";
    [SerializeField] private string message = "";
    [SerializeField] private bool canInteract = false;

    public string code = "1234";

    // Start is called before the first frame update
    void Start()
    {
        UIController.Instance.OnReactorButton_Clicked += CheckCode;
    }

    // Update is called once per frame
    void Update()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.E))
        {
            UIController.Instance.ShowReactorInput();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            UIController.Instance.ShowMessage(interactText);
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            UIController.Instance.HideMessage();
            
        }
    }

    public void CheckCode(string code)
    {
        if( code == this.code)
          {
            foreach (var item in objectsToDeactivate)
            {
                item.gameObject.SetActive(false);
            }
            foreach (Light2D item in lightsToChange)
            {
                item.color = Color.red;
            }
            foreach(AudioSource item in audioSources)
            {
                item.Play();
            }
            UIController.Instance.HideReactorInput();
            UIController.Instance.ShowMessage(message);
            Timer.instance.PauseTimer(false);
            UIController.Instance.ShowTimer();
        }
        else
        {
            UIController.Instance.HideReactorInput();
            UIController.Instance.ShowMessage("Wrong code");
        }
    }
}
