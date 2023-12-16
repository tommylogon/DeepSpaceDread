using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Resource : MonoBehaviour
{
    public enum ResourceType
    {
        Health, Shield, Fuel, Ammo, Energy, Cargo, Cells,
    }
    [SerializeField] private ResourceType type;
    [SerializeField] private float currentValue = 100;
    [SerializeField] private float maxValue = 100;
    [SerializeField] private float minValue = 0;

    [SerializeField] private float regenDelay = 5f;
    [SerializeField] private float regenTimer = 0f;
    [SerializeField] private float regenAmount = 1f;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] private bool canRecharge = false;

    private float nextTick;

    public event Action OnEmpty;
    public event Action OnFull;
    public event Action OnRecharge;
    public event Action OnValueChange;


    void Update()
    {
        AutoRecharge();
    }

    public bool IsVulnerable()
    {
        return canTakeDamage;
    }
    public void SetVulnerable(bool status)
    {
        canTakeDamage = status;
    }

    public bool IsValueBetweenMinAndMax()
    {
        if(currentValue > minValue && currentValue < maxValue)
        {
            
            return true;
        }
        
        return false;
    }

    public float GetValue() { return currentValue; }

    public void ReduceRecource(float newValue)
    {
        currentValue -= newValue;
        validateCurrentValue();
        if (currentValue <= 0)
        {
            OnEmpty?.Invoke();
        }
    }
    public void IncreaseResource(float newValue)
    {
        currentValue += newValue;

        OnValueChange?.Invoke();
        if (currentValue >= maxValue)
        {
            OnFull?.Invoke();
        }
    }

    private void RechargeResource()
    {
        currentValue += regenAmount;
        regenTimer = 0; // Reset timer after recharging
        
        OnRecharge?.Invoke();
        validateCurrentValue();

        if (!canTakeDamage && currentValue == maxValue)
        {
            canTakeDamage = true;
        }
    }

    public void validateCurrentValue()
    {
        OnValueChange?.Invoke();
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
    }

    public void ResetRefillTimer()
    {
        regenTimer = 0f;
    }

    public void AutoRecharge()
    {
        if (currentValue < maxValue && canRecharge)
        {
            OnRecharge?.Invoke();
            regenTimer += Time.deltaTime;
                     

            if (canTakeDamage && regenTimer > regenDelay)
            {
                RechargeResource();
            }
            else if (!canTakeDamage)
            {
                RechargeResource();
            }

            
        }
    }
   
}
