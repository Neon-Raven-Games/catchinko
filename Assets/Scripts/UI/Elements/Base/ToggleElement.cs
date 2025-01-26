using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleElement : MonoBehaviour, IElement
{
    public Action OnUpdated { get; set; }

    private Toggle _toggle;
    public void SetValue(bool toggled) => 
        _toggle.isOn = toggled;
    
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    protected virtual void OnToggleValueChanged(bool toggled)
    {
        OnUpdated?.Invoke();
    }
}
