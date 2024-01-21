using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private Label messageLabel;
    private Label interactoinLabel;
    private Label gameOverLable;
    private Label timerLabel;
    private Label memoryLable;
    private Label codeLable;

    private VisualElement gameStatePanel;
    private VisualElement reactorInputPanel;
    private VisualElement menuPanel;
    private VisualElement controlsPanel;
    private VisualElement settingsPanel;
    public VisualElement LockStatus;
    public VisualElement unlockedStatus;
    public VisualElement CodeDisplay;
    public VisualElement Keypad;
    public VisualElement ReactorControls;

    public VisualElement PauseMenuUXML;
    public VisualElement ReactorUXML;
    public VisualElement TimerUXML;

    private TypewriterEffect typewriterEffect;

    public event Action<string> OnReactorOKButton_Clicked;
    public event Action OnPullControlRodsButton_Clicked;
    public event Action OnShutDownReactorButton_Clicked;
    public event Action OnStabilizeButton_Clicked;


    private Coroutine hideMessageCoroutine;

    private List<string> memoryList = new List<string>();



    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        typewriterEffect = gameObject.AddComponent<TypewriterEffect>();
        messageLabel = root.Q<Label>("message-lable");
        interactoinLabel = root.Q<Label>("InteractionLable");
        memoryLable = root.Q<Label>("MemoryText");
        gameOverLable = root.Q<Label>("StartOrGameOver");

        gameStatePanel = root.Q<VisualElement>("GameStatePanel");
        reactorInputPanel = root.Q<VisualElement>("ReactorInputPanel");

        CodeDisplay = root.Q<VisualElement>("CodeDisplay");
        Keypad = root.Q<VisualElement>("Keypad");
        ReactorControls = root.Q<VisualElement>("ReactorControls");

        codeLable = root.Q<Label>("CodeLabel");
        
        menuPanel = root.Q<VisualElement>("MenuPanel");
        controlsPanel = root.Q<VisualElement>("ControlsPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");

       

        LockStatus = root.Q<VisualElement>("LockStatus");
        unlockedStatus = root.Q<VisualElement>("UnlockedStatus");

        timerLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("Countdown");

        messageLabel.visible = false;
        reactorInputPanel.style.display = DisplayStyle.None;
        timerLabel.visible = false;
        menuPanel.style.display = DisplayStyle.None;

        ReactorControls.style.display = DisplayStyle.None;
        
       
        Timer.instance.OnTimeChanged += UpdateTimer;

        RegisterButtons(root);
        

    }

    public void RegisterButtons(VisualElement root)
    {
        Button exitButton = root.Q<Button>("ExitButton");
        Button PullControlRodsButton = root.Q<Button>("PullControlRodsButton");
        Button ShutDownReactorButton = root.Q<Button>("ShutDownReactorButton");
        Button StabilizeButton = root.Q<Button>("StabilizeButton");
        Button controlsButton = root.Q<Button>("ControlsButton");
        Button settingsButton = root.Q<Button>("SettingsButton");
        Button resumeButton = root.Q<Button>("ResumeButton");
        Button restartButton = root.Q<Button>("RestartButton");

        controlsButton.clicked += () => { ShowControlsPanel(); };
        settingsButton.clicked += () => { ShowSettingsPanel(); };
        PullControlRodsButton.clicked += PullControlRodsButton_Clicked;
        ShutDownReactorButton.clicked += ShutDownReactorButton_Clicked;
        StabilizeButton.clicked += StabilizeButton_Clicked;

        resumeButton.clicked += ToggleMenu;
        restartButton.clicked += RestartButton_Clicked ;
        exitButton.clicked += ExitButton_Clicked;

        for (int i = 0; i <= 9; i++)
        {
            Button button = root.Q<Button>($"Button{i}");
            RegisterKeypadButton(button);
        }

        Button OKButton = root.Q<Button>("OKButton");
        RegisterOKButton(OKButton);

        Button XButton = root.Q<Button>("XButton");
        RegisterXButton(XButton);

        Button closePanelButton = root.Q<Button>("ClosePanelButton");
        closePanelButton.clicked += ClosePanelButton_Clicked;
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
            gameOverLable.text = "GAME OVER \r\n PRESS R OR BUTTON B TO TRY AGAIN";
        }
        else { gameOverLable.text = "YOU SURVIVED \r\n PRESS R OR BUTTON B TO RETRY."; }
            
       

    }
    public void HideGameOver()
    {
        gameStatePanel.visible = false;
    }

    public void ShowReactorPanel()
    {
        codeLable.text = "";
        reactorInputPanel.style.display = DisplayStyle.Flex;
    }

    public void HideReactorPanel()
    {
        reactorInputPanel.style.display =DisplayStyle.None;
    }

    public void ShowReactorLogin()
    {
        codeLable.text = "";
        CodeDisplay.style.display = DisplayStyle.Flex;
    }

    public void HideReactorLogin()
    {
        CodeDisplay.style.display = DisplayStyle.None;
        Keypad.style.display = DisplayStyle.None;
    }

    public void ShowReactorControlls()
    {
        
        ReactorControls.style.display = DisplayStyle.Flex;
    }

    public void HideReactorControlls()
    {
        ReactorControls.style.display = DisplayStyle.None;
    }

    public string GetTextInput()
    {
        return codeLable.text;
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

        
        memoryLable.text = string.Join("\n", memoryList);
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
        menuPanel.style.display = menuPanel.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
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

    private void RegisterKeypadButton(Button button)
    {
        button.clicked += () =>
        {
            codeLable.text += button.text;
        };
    }
    public void RegisterOKButton(Button button)
    {
        button.clicked += () =>
        {
            OnReactorOKButton_Clicked?.Invoke(codeLable.text);
        };
    }

    public void RegisterXButton(Button button)
    {
        button.clicked += () =>
        {
            string currentText = codeLable.text;
            if (currentText.Length > 0)
            {

                codeLable.text = currentText.Substring(0, currentText.Length - 1);
            }
        };
    }

    public void PullControlRodsButton_Clicked()
    {
        OnPullControlRodsButton_Clicked?.Invoke();
    }
    public void ShutDownReactorButton_Clicked()
    {
        OnShutDownReactorButton_Clicked?.Invoke();
    }
    public void StabilizeButton_Clicked()
    {
        OnStabilizeButton_Clicked?.Invoke();
    }

    public void ShowUnlockedStatus()
    {
        LockStatus.style.display = DisplayStyle.None;
        unlockedStatus.style.display = DisplayStyle.Flex;

    }
    private void ClosePanelButton_Clicked()
    {
        HideReactorPanel();
    }

    internal bool IsReactorShowing()
    {
        return reactorInputPanel.style.display != DisplayStyle.None;
    }
    public void RestartButton_Clicked()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void ResumeButton_Clicked()
    {

    }
}
