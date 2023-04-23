using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    private Label messageLabel;
    private Label gameOver;
    private Label timerLabel;
    private Label memoryText;
    private VisualElement gameStatePanel;
    private VisualElement reactorInputPanel;
    private TextField reactorInputText;
    private TypewriterEffect typewriterEffect;
    private Button reactorButton;

    public event Action<string> OnReactorButton_Clicked;



    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;
        typewriterEffect = gameObject.AddComponent<TypewriterEffect>();
        messageLabel = root.Q<Label>("message-lable");
        gameOver = root.Q<Label>("StartOrGameOver");
        gameStatePanel = root.Q<VisualElement>("GameStatePanel");
        reactorInputPanel = root.Q<VisualElement>("ReactorInputPanel");
        reactorInputText = root.Q<TextField>("ReactorInputTextField");
        reactorButton = root.Q<Button>("ReactorButton");
        memoryText = root.Q<Label>("MemoryText");

        timerLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("Countdown");

        messageLabel.visible = false;
        reactorInputPanel.visible = false;
        timerLabel.visible = false;

        reactorButton.clicked += ReactorButton_Clicked;
        Timer.instance.OnTimeChanged += UpdateTimer;
        
    }


    public void UpdateTimer(string time)
    {
        timerLabel.text = time;
    }
    public void ReactorButton_Clicked()
    {
        Debug.Log("Clicked button");
        Debug.Log(GetTextInput());
        OnReactorButton_Clicked?.Invoke(GetTextInput());
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
        gameStatePanel.visible = true;

        if (!type)
        {
            gameOver.text = "GAME OVER \r\n PRESS E TO TRY AGAIN";
        }
        else { gameOver.text = "YOU SURVIVED \r\n PRESS R to RETRY."; }
            
       

    }
    public void HideGameOver()
    {
        gameStatePanel.visible = false;
    }

    public void ShowReactorInput()
    {
        reactorInputPanel.visible = true;
    }

    public void HideReactorInput()
    {
        reactorInputPanel.visible = false;
    }

    public string GetTextInput()
    {
        return reactorInputText.text;
        
    }

    public void ShowTimer()
    {
        timerLabel.visible = true;
    }

    public void SaveToMemory(string text)
    {
        memoryText.text = text;
    }

}
