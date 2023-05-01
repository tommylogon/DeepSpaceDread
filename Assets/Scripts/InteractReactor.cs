using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class InteractReactor : Interactable
{
    public enum ReactorState
    {
        Unstable, 
        PullControlRods,
        ShutDownReactor,
        Stabilize
    }

    public string[] messages;
    public Door[] doorToUnlock;
    public FlickerLight[] EnviromentalLights;
    public Light2D[] WarningLights;
    public AudioSource[] audioSources;

    public ReactorState reactorState = ReactorState.Unstable;

    public string reactorCode = "421369";

    private bool unlocked = false;

    private Light2D reactorLight;

    // Start is called before the first frame update
    void Start()
    {
        reactorLight = GetComponentInChildren<Light2D>();
        UIController.Instance.OnReactorOKButton_Clicked += CheckCode;
        UIController.Instance.OnPullControlRodsButton_Clicked += PullControlRods;
        UIController.Instance.OnShutDownReactorButton_Clicked += ShutDownReactor;
        UIController.Instance.OnStabilizeButton_Clicked += Stabilize;
    }
    public override void Interact()
    {
        if (canInteract)
        {
            UIController.Instance.ShowReactorPanel();
        }
    }


    public void CheckCode(string code)
    {
       if(reactorCode == code)
        {
            unlocked = true;
            UIController.Instance.ShowUnlockedStatus();
            UIController.Instance.ShowReactorControlls();
            UIController.Instance.HideReactorLogin();
            

        }
        else
        {            
            UIController.Instance.ShowMessage("Wrong code");
        }
    }

    public void PullControlRods()
    {
        if (unlocked)
        {
            UnlockAllDoors();

            StartCoroutine(ChangeLightColor(Color.red,30f, 30f));

            foreach (Light2D item in WarningLights)
            {
                if (item != null)
                {
                    item.color = Color.red;
                }

            }
            foreach (AudioSource item in audioSources)
            {
                if(item != null)
                {
                    item.Play();
                }
                
            }
            reactorState = ReactorState.PullControlRods;
            UIController.Instance.HideReactorPanel();
            UIController.Instance.ShowMessage(messages[0]);
            Timer.instance.PauseTimer(false);
            UIController.Instance.ShowTimer();
        }


    }

    public void ShutDownReactor()
    {
        if (unlocked)
        {
            UnlockAllDoors();
            StartCoroutine(ChangeLightColor(Color.blue, 0f, 30f));
            foreach (Light2D item in WarningLights)
            {
                if (item != null)
                {
                    item.intensity = 0;
                }

            }

            foreach(FlickerLight light in EnviromentalLights)
            {
                if (light != null)
                {
                    light.minIntensity = 0;
                    light.maxIntensity = 0;

                }
            }

            player.GetComponent<Rigidbody2D>().drag = 0;
            player.GetComponent<PlayerController>().spriteCanRotate = true;
            player.GetComponent<AudioSource>().enabled = false; ;

            UIController.Instance.HideReactorPanel();
            UIController.Instance.ShowMessage(messages[1]);
            reactorState = ReactorState.ShutDownReactor;
        }

    }

    public void Stabilize()
    {
        if (unlocked)
        {
            foreach (Light2D item in WarningLights)
            {
                if (item != null)
                {
                    item.intensity = 0;
                }

            }

            foreach (FlickerLight light in EnviromentalLights)
            {
                if (light != null)
                {
                    light.minIntensity = 0.95f;
                    light.maxIntensity = 1;
                    light.turnOffChance = 0f;
                    light.light2D.pointLightOuterRadius = 5f;
                    light.tag = "IgnoreRaycast";

                }
            }
            reactorState = ReactorState.Stabilize;
            UIController.Instance.HideReactorPanel();
            UIController.Instance.ShowMessage(messages[2]);
        }
    }

    private void UnlockAllDoors()
    {
        foreach (Door item in doorToUnlock)
        {
            if (item != null)
            {
                item.SetDoorToLocked(false);
            }

        }
    }

    private IEnumerator ChangeLightColor(Color endColor,float endIntensity, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(1);
            reactorLight.color = Color.Lerp(reactorLight.color, endColor, elapsedTime / duration);
            reactorLight.intensity = Mathf.Lerp(reactorLight.intensity, endIntensity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        reactorLight.color = endColor;
        reactorLight.intensity = endIntensity;
    }
}
