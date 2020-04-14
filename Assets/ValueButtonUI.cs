using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueButtonUI : MonoBehaviour
{
    public float Min, Max, Increment, smallIncrement;
    public float CurrentValue { get { return currentValue; } set { SetNewValue(value); } }
    private float currentValue;
    public Text text;
    public string ValueName;
    public bool isPercentage = false;
    public bool loopableNumber = false;


    // Start is called before the first frame update
    void Start()
    {
        SetNewValue(currentValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementUp() {
        SetNewValue(currentValue + Increment);
    }

    public void IncrementDown() {
        SetNewValue(currentValue - Increment);
    }

    public void SmallIncrementUp() {
        SetNewValue(currentValue + smallIncrement);
    }

    public void SmallIncrementDown() {
        SetNewValue(currentValue - smallIncrement);
    }

    public void SetNewValue(float inputValue) {
        if (loopableNumber) {
            currentValue = Mathf.Repeat(inputValue, Max);
        } else {
            currentValue = Mathf.Clamp(inputValue, Min, Max);
        }
        if(isPercentage)
            text.text = ValueName + ": " + (currentValue*100).ToString("P");
        else
            text.text = ValueName + ": " + currentValue.ToString("F2"); 
    }
}
