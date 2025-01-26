using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueElement : MonoBehaviour, IValueElement
{
    public Action OnUpdated { get; set; }
    
    public int maxValue = 99;

    [SerializeField] private float updateStep = 0.1f;
    
    private bool _shouldInvalidate;
    private float _lastUpdate;
    private Scrollbar _scrollBar;
    private TMP_InputField _inputField;
    private int _lastValidTextIntValue;
    private float _value;
    public float Value
    {
        get => _value;
        set => SetValue(_value);
    }

    public int MaxValue
    {
        get => maxValue;
        set => maxValue = value;
    }
    private void Start()
    {
        AssignComponents();
        ValidateComponents();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        OnUpdated += UpdateHandler;
        _scrollBar.onValueChanged.AddListener(OnSliderValueChanged);
        _inputField.onValueChanged.AddListener(UpdateElementValue);
    }

    private void OnScrollBarFinished() =>
        OnSliderValueChanged(_scrollBar.value);

    private void OnTextInputFinished() =>
        UpdateElementValue(_inputField.text);

    private void AssignComponents()
    {
        _scrollBar = GetComponentInChildren<Scrollbar>();
        _inputField = GetComponentInChildren<TMP_InputField>();
    }

    private void ValidateComponents()
    {
        if (_scrollBar == null) throw new Exception("Slider not found");
        if (_inputField == null) throw new Exception("InputField not found");
    }

    private void OnSliderValueChanged(float value)
    {
        value *= maxValue;
        UpdateComponentTextInputValue((int) value);
        OnValueChanged();
    }

    private void UpdateHandler()
    {
        if (Time.time - _lastUpdate < updateStep)
        {
            _shouldInvalidate = true;
            return;
        }
        
        _lastUpdate = Time.time;
        
        // updates based off scroll bar value
        // UpdateSpawnArea(Mathf.CeilToInt(_scrollBar.value * maxValue));
    }

    /// <summary>
    /// Update the spawn are values, and is coupled to this implementation. We should abstract this out.
    /// </summary>
    private void UpdateSpawnArea(int value)
    {

    }
    
    /// <summary>
    /// Updates the visuals of the element, as well as the value of the element.
    /// Calls an update by invoking <see cref="OnUpdated"/>.
    /// </summary>
    /// <param name="stringValue"></param>
    private void UpdateElementValue(string stringValue)
    {
        if (!ValidInput()) return;
        UpdateSliderWithinBounds(int.Parse(stringValue));
        OnValueChanged();
    }
    
    /// <summary>
    /// Update the slider and adjust it to be within the bounds of the max value, assuming values less than 0 are
    /// invalid. To improve this for reusability, we could allow the value below zero, and allow the user to clamp
    /// the values between a range. I would use Odin's Sirenix.OdinInspector.MinMaxSliderAttribute for this.
    /// </summary>
    private void UpdateSliderWithinBounds(int value)
    {
        if (value > maxValue) value = maxValue;
        else if (value < 0) value = 0;
        _value = value;
        UpdateComponentSliderValue(value);
    }

    private bool ValidInput()
    {
        if (string.IsNullOrEmpty(_inputField.text)) return false;

        if (int.TryParse(_inputField.text, out int currentIntValue))
        {
            if (currentIntValue == _lastValidTextIntValue) return false;
            _lastValidTextIntValue = currentIntValue;
        }
        else
        {
            _inputField.SetTextWithoutNotify(_lastValidTextIntValue.ToString());
            _inputField.caretPosition = _inputField.text.Length;
            return false;
        }

        return true;
    }

    public void UpdateVisualsWithValue(float value)
    {
        UpdateComponentSliderValue(value);
        UpdateComponentTextInputValue(value);
    }
    public void UpdateVisuals()
    {
        UpdateComponentSliderValue(_value);
        UpdateComponentTextInputValue(_value);
    } 
    private void UpdateComponentSliderValue(float value) =>
        _scrollBar.value = value / maxValue;

    private void UpdateComponentTextInputValue(float value) =>
        _inputField.text = value.ToString();

    private void OnValueChanged() =>
        OnUpdated?.Invoke();

    public void Update()
    {
        if (!_shouldInvalidate) return;
        InvalidateElement();
    }

    private void InvalidateElement()
    {
        _shouldInvalidate = false;
        OnScrollBarFinished();
        OnTextInputFinished();
    }

    /// <summary>
    /// Sets the value of the text and slider element, then calls an update, invoking <see cref="OnUpdated"/>
    /// </summary>
    /// <param name="value">The value that this element will now represent.</param>
    public void SetValue(float value)
    {
        UpdateComponentTextInputValue(value);
        UpdateElementValue(_value.ToString());
    }
}