using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    public event Action OnTimeOut;

    public event Action<string> OnTimeChanged;

    [SerializeField] private float initialTime = 60f;

    private static float timeLeft = 0f;
    private static bool isTimeOut = false;

    private static bool isPaused = false;

    private void Awake()
    {
        instance = this;

    }
    void Start()
    {


        isPaused=true; 
        isTimeOut=false;
        timeLeft = instance.initialTime;


    }


    void Update()
    {
        if (isTimeOut || isPaused)
        {
            return;
        }
        else
        {
            timeLeft -= Time.deltaTime;
            OnTimeChanged?.Invoke(FormatTime(timeLeft));
        }
       
        ;


        if (timeLeft <= 0 && !isTimeOut)
        {
            isTimeOut = true;

            OnTimeOut?.Invoke();

        }


    }

    public string FormatTime()
    {
        return FormatTime(timeLeft);
    }

    public static string FormatTime(float timeToFormat)
    {
        int minutes = Mathf.FloorToInt(timeToFormat / 60);
        int seconds = Mathf.FloorToInt(timeToFormat % 60);
        int milliseconds = Mathf.FloorToInt((timeToFormat - Mathf.Floor(timeToFormat)) * 1000f);
        if(timeLeft <= 0)
        {
            return "00:00:000";
        }
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void AddTime(float timeToAdd) 
    { 
        timeLeft += timeToAdd;
    }
    public void RemoveTime(float timeToRemove) { timeLeft -= timeToRemove; }

    public bool IsTimedOut() { return isTimeOut; }

    public void PauseTimer(bool state) { isPaused = state; }


    public float GetTimeLeft() { return timeLeft; }

    public void SetInitialTime(float time) { initialTime = time; }



}
