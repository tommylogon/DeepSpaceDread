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
        if (canInteract)
        {
            player.transform.position = transform.position;
            player.GetComponent<SpriteRenderer>().sortingOrder = -2;
            onboard = true;
        }
    }

    void Update()
    {

        if (onboard)
        {
            player.transform.position = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, targetPos.position, 4 * Time.deltaTime);
            UIController.Instance.ShowGameOver(true);
            Timer.instance.PauseTimer(true);

            switch (reactor.reactorState)
            {
                case InteractReactor.ReactorState.PullControlRods:
                    UIController.Instance.ShowMessage(messages[0]);
                    break;
                case InteractReactor.ReactorState.ShutDownReactor:
                    UIController.Instance.ShowMessage(messages[1]);
                    break;
                case InteractReactor.ReactorState.Stabilize:
                    UIController.Instance.ShowMessage(messages[2]);
                    break;
                default:
                    break;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}
