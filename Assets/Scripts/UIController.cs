using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private Label messageLabel;
    private Label interactoinLabel;
    private Label gameOver;
    private Label timerLabel;
    private Label memoryText;

    private VisualElement gameStatePanel;
    private VisualElement reactorInputPanel;
    private VisualElement menuPanel;
    private VisualElement controlsPanel;
    private VisualElement settingsPanel;

    private TextField reactorInputText;

    private TypewriterEffect typewriterEffect;

    public event Action<string> OnReactorButton_Clicked;

    private Coroutine hideMessageCoroutine;

    private List<string> memoryList = new List<string>();



    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;
        typewriterEffect = gameObject.AddComponent<TypewriterEffect>();
        messageLabel = root.Q<Label>("message-lable");
        interactoinLabel = root.Q<Label>("InteractionLable");
        memoryText = root.Q<Label>("MemoryText");
        gameOver = root.Q<Label>("StartOrGameOver");

        gameStatePanel = root.Q<VisualElement>("GameStatePanel");
        reactorInputPanel = root.Q<VisualElement>("ReactorInputPanel");
        reactorInputText = root.Q<TextField>("ReactorInputTextField");
        menuPanel = root.Q<VisualElement>("MenuPanel");
        controlsPanel = root.Q<VisualElement>("ControlsPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");

        Button exitButton = root.Q<Button>("ExitButton");
        Button reactorButton = root.Q<Button>("ReactorButton");
        Button controlsButton = root.Q<Button>("ControlsButton");
        Button settingsButton = root.Q<Button>("SettingsButton");


        timerLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("Countdown");

        messageLabel.visible = false;
        reactorInputPanel.visible = false;
        timerLabel.visible = false;
        ToggleMenu();

        controlsButton.clicked += () => { ShowControlsPanel(); };
        settingsButton.clicked += () => { ShowSettingsPanel(); };
        reactorButton.clicked += ReactorButton_Clicked;
        exitButton.clicked += ExitButton_Clicked;
        Timer.instance.OnTimeChanged += UpdateTimer;
        
    }

    

    private void ExitButton_Clicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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
        // Stop the existing coroutine if one is already running
        if (hideMessageCoroutine != null)
        {
            StopCoroutine(hideMessageCoroutine);
        }

        typewriterEffect.StartTextWriter(messageLabel, message);
        messageLabel.visible = true;
    }

    public void HideMessage()
    {
        messageLabel.visible = false;
    }

    public void HideMessageAfterDelay(float delay)
    {
        // Stop the existing coroutine if one is already running
        if (hideMessageCoroutine != null)
        {
            StopCoroutine(hideMessageCoroutine);
        }

        hideMessageCoroutine = StartCoroutine(HideMessageAfterDelayCoroutine(delay));
    }

    private IEnumerator HideMessageAfterDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideMessage();
    }

    public void ShowGameOver(bool type)
    {
        gameStatePanel.visible = true;

        if (!type)
        {
            gameOver.text = "GAME OVER \r\n PRESS R OR BUTTON B TO TRY AGAIN";
        }
        else { gameOver.text = "YOU SURVIVED \r\n PRESS R OR BUTTON B TO RETRY."; }
            
       

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
        
        if (!memoryList.Contains(text))
        {
            memoryList.Add(text);
        }

        
        memoryText.text = string.Join("\n", memoryList);
    }

    public void ShowInteraction(string text)
    {
        interactoinLabel.visible = true;
        interactoinLabel.text = text;
    }

    public void HideInteraction()
    {
        interactoinLabel.visible= false;
    }

    internal void ToggleMenu()
    {
        menuPanel.visible = !menuPanel.visible;
    }
    public void ShowControlsPanel()
    {
        controlsPanel.visible = true;
    }

    public void HideControlsPanel()
    {
        controlsPanel.visible = false;
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.visible = true;
    }

    public void HideSettingsPanel()
    {
        settingsPanel.visible = false;
    }

}
