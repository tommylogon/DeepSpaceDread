using UnityEngine;
using UnityEngine.Rendering.Universal;


public class InteractReactor : Interactable
{
    public GameObject[] objectsToDeactivate;
    public Light2D[] lightsToChange;
    public AudioSource[] audioSources;

    public string code = "1234";

    // Start is called before the first frame update
    void Start()
    {
        UIController.Instance.OnReactorButton_Clicked += CheckCode;
    }
    public override void Interact()
    {
        if (canInteract)
        {
            UIController.Instance.ShowReactorInput();
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
