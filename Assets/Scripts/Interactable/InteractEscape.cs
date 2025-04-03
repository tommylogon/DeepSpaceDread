using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InteractEscape : Interactable
{

    [SerializeField] private bool onboard = false;

    [SerializeField] private Transform targetPos;

    public InteractReactor reactor;

    public override void Interact()
    {
        if (closeEnoughToInteract)
        {
            playerRef.transform.position = transform.position;
            playerRef.GetComponent<SpriteRenderer>().sortingOrder = -2;
            onboard = true;
        }
    }

    void Update()
    {

        if (onboard)
        {
            playerRef.transform.position = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, targetPos.position, 4 * Time.deltaTime);
            UIController.Instance.ShowGameOver(true);
            Timer.instance.PauseTimer(true);

            if (reactor.reactorState == InteractReactor.ReactorState.Unstable)
            {
                //UIController.Instance.ShowMessage(messages[0]);
            }
            else if (reactor.reactorState == InteractReactor.ReactorState.Critical)
            {
                //UIController.Instance.ShowMessage(messages[1]);
            }
            else if (reactor.reactorState == InteractReactor.ReactorState.Online)
            {
                //UIController.Instance.ShowMessage(messages[2]);
            }
            else
            {
                //UIController.Instance.ShowMessage("Reactor Offline");
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}
