using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    private Label messageLabel;
    private Label gameOver;
    private TypewriterEffect typewriterEffect;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;
        typewriterEffect = gameObject.AddComponent<TypewriterEffect>();
        messageLabel = root.Q<Label>("message-lable");
        gameOver = root.Q<Label>("StartOrGameOver");

        messageLabel.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMessage(string message)
    {
        typewriterEffect.StartTextWriter(messageLabel, message);
        messageLabel.visible = true;
    }

    public void HideMessage()
    {
        messageLabel.visible = false;
    }

    public void ShowGameOver(bool type)
    {
        messageLabel.visible = true;

        if (!type)
        {
            gameOver.text = "GAME OVER \r\n PRESS E TO TRY AGAIN";
        }
        else { gameOver.text = "YOU SURVIVED"; }
            
       

    }
    public void HideGameOver()
    {
        gameOver.visible = false;
    }

}
