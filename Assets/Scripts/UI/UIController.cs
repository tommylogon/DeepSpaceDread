using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    private TypewriterEffect typewriterEffect;

    private Label messageLabel;
    private Label interactoinLabel;
    private Label gameOverLable;
    private Label timerLabel;
    private Label memoryLable;
    private Label codeLable;

    private VisualElement gameStatePanel;
    private VisualElement reactorInputPanel;
    private VisualElement controlsPanel;
    private VisualElement settingsPanel;
    public VisualElement LockStatus;
    public VisualElement unlockedStatus;
    public VisualElement CodeDisplay;
    public VisualElement Keypad;
    public VisualElement ReactorControls;

    [SerializeField]private VisualTreeAsset PauseMenuUI;
    [SerializeField] private VisualTreeAsset ReactorUI;
    [SerializeField] private VisualTreeAsset TimerUI;

    private VisualElement timerPanel;
    private VisualElement reactorPanel;
    private VisualElement pauseMenuPanel;

    

    public event Action<string> OnReactorOKButton_Clicked;
    public event Action OnPullControlRodsButton_Clicked;
    public event Action OnShutDownReactorButton_Clicked;
    public event Action OnStabilizeButton_Clicked;


    private Coroutine hideMessageCoroutine;

    private List<string> memoryList = new List<string>();

    private VisualElement centerSection;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        Instance = this;
        typewriterEffect = gameObject.AddComponent<TypewriterEffect>();

        pauseMenuPanel = PauseMenuUI.CloneTree();
        pauseMenuPanel.style.display = DisplayStyle.None;
        var root = GetComponent<UIDocument>().rootVisualElement;
        centerSection = root.Q<VisualElement>("Center");
        if(root == null)
        {
            Debug.Log("root is null");
        }
        
        centerSection.Insert(0, pauseMenuPanel);
        RegisterPauseMenuButtons(pauseMenuPanel);

        // --- Instantiate Timer UI (if using separate UXML) ---
        if (TimerUI != null)
        {
            timerPanel = TimerUI.CloneTree();
            centerSection.Insert(1, timerPanel);
            timerLabel = timerPanel.Q<Label>("Countdown");
            timerLabel.visible = false;
        }
        else
        {
            // Fallback: look for timer in main UXML
            timerLabel = root.Q<Label>("Countdown");
            if (timerLabel != null) timerLabel.visible = false;
        }

        // --- Instantiate Reactor UI ---
        reactorPanel = ReactorUI.CloneTree();
        reactorPanel.style.display = DisplayStyle.None;
        centerSection.Insert(2, reactorPanel);

        // Register all reactor elements
        RegisterReactorElements(reactorPanel);
        RegisterReactorButtons(reactorPanel);

        // Register main UI elements
        RegisterElementReferenses(root);
        ClearTextFields();
    }
private void RegisterReactorElements(VisualElement root)
    {
        reactorInputPanel = root.Q<VisualElement>("ReactorInputPanel");
        CodeDisplay = root.Q<VisualElement>("CodeDisplay");
        Keypad = root.Q<VisualElement>("Keypad");
        ReactorControls = root.Q<VisualElement>("ReactorControls");
        LockStatus = root.Q<VisualElement>("LockStatus");
        unlockedStatus = root.Q<VisualElement>("UnlockedStatus");
        codeLable = root.Q<Label>("CodeLabel");

        // Hide controls initially
        ReactorControls.style.display = DisplayStyle.None;
    }

    private void RegisterElementReferenses(VisualElement root)
    {
        messageLabel = root.Q<Label>("message-lable");
        interactoinLabel = root.Q<Label>("InteractionLabel");
        memoryLable = root.Q<Label>("MemoryText");
        gameStatePanel = root.Q<VisualElement>("GameStatePanel");
        gameOverLable = root.Q<Label>("StartOrGameOver");
    }

    private void OnEnable()
    {
        Timer.instance.OnTimeChanged += UpdateTimer;
    }
    private void OnDisable()
    {
        Timer.instance.OnTimeChanged -= UpdateTimer;
    }

    private void RegisterPauseMenuButtons(VisualElement root)
    {
        Button resumeButton = root.Q<Button>("ResumeButton");
        Button restartButton = root.Q<Button>("RestartButton");
        Button settingsButton = root.Q<Button>("SettingsButton");
        Button exitButton = root.Q<Button>("ExitButton");

        settingsButton.clicked += ShowSettingsPanel_Clicked;
        resumeButton.clicked += TogglePauseMenu_Clicked;
        restartButton.clicked += RestartButton_Clicked;
        exitButton.clicked += ExitButton_Clicked;
    }
    public void RegisterReactorButtons(VisualElement root)
    {
        Button PullControlRodsButton = root.Q<Button>("PullControlRodsButton");
        Button ShutDownReactorButton = root.Q<Button>("ShutDownReactorButton");
        Button StabilizeButton = root.Q<Button>("StabilizeButton");
        Button controlsButton = root.Q<Button>("ControlsButton");

        if (controlsButton != null) controlsButton.clicked += ShowControlsPanel;
        if (PullControlRodsButton != null) PullControlRodsButton.clicked += PullControlRodsButton_Clicked;
        if (ShutDownReactorButton != null) ShutDownReactorButton.clicked += ShutDownReactorButton_Clicked;
        if (StabilizeButton != null) StabilizeButton.clicked += StabilizeButton_Clicked;

        // Keypad buttons 0-9
        for (int i = 0; i <= 9; i++)
        {
            Button button = root.Q<Button>($"Button{i}");
            if (button != null) RegisterKeypadButton(button);
        }

        Button OKButton = root.Q<Button>("OKButton");
        if (OKButton != null) RegisterOKButton(OKButton);

        Button XButton = root.Q<Button>("XButton");
        if (XButton != null) RegisterXButton(XButton);

        Button closePanelButton = root.Q<Button>("ClosePanelButton");
        if (closePanelButton != null) closePanelButton.clicked += ClosePanelButton_Clicked;
    }

    private void ExitButton_Clicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

   private void ClearTextFields()
    {
        messageLabel.text = "";
        memoryLable.text = "";
        HideMessage();
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
        reactorPanel.style.display = DisplayStyle.Flex;
        codeLable.text = "";
        // Reset to login state
        CodeDisplay.style.display = DisplayStyle.Flex;
        Keypad.style.display = DisplayStyle.Flex;
        ReactorControls.style.display = DisplayStyle.None;
        LockStatus.style.display = DisplayStyle.Flex;
        unlockedStatus.style.display = DisplayStyle.None;
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
        if (interactoinLabel != null)
        {
            interactoinLabel.visible= false;
        }
    }

    internal void TogglePauseMenu_Clicked()
    {
        pauseMenuPanel.style.display = pauseMenuPanel.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void ShowControlsPanel()
    {
        controlsPanel.visible = true;
    }

    public void HideControlsPanel()
    {
        controlsPanel.visible = false;
    }

    public void ShowSettingsPanel_Clicked()
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
        return reactorPanel != null && reactorPanel.style.display != DisplayStyle.None;
    }
    public void RestartButton_Clicked()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void ResumeButton_Clicked()
    {

    }
    
}
