using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiBar : MonoBehaviour
{
    [SerializeField] Slider normalBar;
    [SerializeField] Slider easeBar;
    [SerializeField] float easing;
    [SerializeField] TextMeshProUGUI textt;

    private float maxValue;
    public float currentValue;

    public System.Func<float> GetValueFunc; // Delegate to get the current value

    // Setup the UI bar to track a specific variable
    public void Setup(System.Func<float> getValueFunc, float initialValue, float maxValue)
    {
        GetValueFunc = getValueFunc;
        this.maxValue = maxValue;
        normalBar.maxValue = maxValue;
        easeBar.maxValue = maxValue;
        SetValue(initialValue);
    }

    // Set the current value of the tracked variable
    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0f, maxValue);
    }

    private void Update()
    {
        // Update the UI bars and text
        float targetValue = GetValueFunc();
        normalBar.value = targetValue;
        easeBar.value = Mathf.Lerp(easeBar.value, targetValue, easing);
        textt.text = $"{targetValue:0}/{maxValue}";
    }
}
