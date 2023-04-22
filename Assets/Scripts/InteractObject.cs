using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractObject : MonoBehaviour
{
    [SerializeField] private string interactText = "Press E to interact";
    [SerializeField] private string message = "";
    [SerializeField] private bool canInteract = false;
    private TypewriterEffect typewriterEffect;


    private Label messageLabel;


    // Start is called before the first frame update
    void Start()
    {
        var root = GameObject.FindGameObjectWithTag("UIDocument").GetComponent<UIDocument>().rootVisualElement;
        typewriterEffect =  gameObject.AddComponent<TypewriterEffect>();
        messageLabel = root.Q<Label>("message-lable");
    }

    // Update is called once per frame
    void Update()
    {

        if(canInteract && Input.GetKey(KeyCode.E))
        {
            typewriterEffect.StartTextWriter(messageLabel, message);
            //messageLabel.text = message;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            messageLabel.text = interactText;
            messageLabel.visible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            messageLabel.visible = false;
        }
    }
}
