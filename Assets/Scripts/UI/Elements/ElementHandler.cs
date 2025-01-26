using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementHandler : MonoBehaviour
{
    /// <summary>
    /// The spawn area. Ideally this would be on a child, but leaving it here because we need
    /// to decouple this by abstraction. Ran out of time.
    /// </summary>
    /// 
    // Ideally, I would dynamically populate these if I had more time. I would put them into a dictionary
    // this way we could index them easily in the code. We just have to define an enum, and give it a 
    // property on the base element. Then we can call the element like: ElementHandler.SetToggle(ToggleElements.Grid, true);
    // private readonly Dictionary<ToggleElements, ToggleElement> _toggleElements = new();
    // private readonly Dictionary<ValueElements, ValueElement> _valueElements = new();

    // private List<IElement> _elements;

    // private void Awake()
    // {
    // _elements = GetComponentsInChildren<IElement>().ToList();
    // foreach (var element in _elements) element.SpawnArea = spawnArea;
    // }

    /// <summary>
    /// The sphere count value element that adjusts the number of spheres in the spawning area.
    /// </summary>
    public ValueElement sphereCountElement;

    /// <summary>
    /// The sphere speed value element that adjusts the number of spheres in the spawning area.
    /// </summary>
    public ValueElement sphereSpeedElement;

    /// <summary>
    /// The reverse path element to reverse the path.
    /// </summary>
    public ToggleElement reversePathElement;
    
    /// <summary>
    /// The colorize element to color the spheres.
    /// </summary>
    public ToggleElement colorizeElement;

    /// <summary>
    /// The grid element to toggle between a grid and a line.
    /// </summary>
    public ToggleElement gridElement;

    /// <summary>
    /// Deselects the currently selected element. It's annoying when it blocks input.
    /// </summary>
    public void Deselect()
    {
        if (EventSystem.current.currentSelectedGameObject == null) return;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Awake()
    {
        // _toggleElements.Add(ToggleElements.ReversePath, reversePathElement);
        // _toggleElements.Add(ToggleElements.Colorize, colorizeElement);
        // _toggleElements.Add(ToggleElements.GridAndLinePath, gridElement);
        //
        // _valueElements.Add(ValueElements.SphereCount, sphereCountElement);
        // _valueElements[ValueElements.SphereCount].SpawnArea = spawnArea;
        // _valueElements.Add(ValueElements.SphereSpeed, sphereSpeedElement);
        // _valueElements[ValueElements.SphereSpeed].SpawnArea = spawnArea;
    }

    // public void SetToggle(ToggleElements element , bool toggled)
    // {
    //     if (_toggleElements.ContainsKey(element)) _toggleElements[element].SetValue(toggled);
    //     else Debug.LogWarning($"The Toggle Element not found in dictionary: {element}");
    // }
    //
    // public void SetValue(ValueElements element, float value)
    // {
    //     if (_valueElements.ContainsKey(element)) _valueElements[element].SetValue(value);
    //     else Debug.LogWarning($"The Value Element not found in dictionary: {element}");
    // }

    /// <summary>
    /// Sets the max value and updates visuals. Will not call <see cref="IElement.OnUpdated"/>
    /// </summary>
    /// <param name="valueElement"></param>
    /// <param name="value"></param>
    // public void SetMaxValue(ValueElements valueElement, int value)
    // {
    //     if (_valueElements.ContainsKey(valueElement))
    //     {
    //         _valueElements[valueElement].MaxValue = value;
    //         if (_valueElements[valueElement].Value > value) _valueElements[valueElement].UpdateVisualsWithValue(value);
    //         
    //     }
    //     else Debug.LogWarning($"The Value Element not found in dictionary: {valueElement}");
    // }
    //
    // public void UpdateVisuals(ValueElements valueElement)
    // {
    //     if (_valueElements.ContainsKey(valueElement))
    //         _valueElements[valueElement].UpdateVisuals();
    //     else Debug.LogWarning($"The Value Element not found in dictionary: {valueElement}");
    // }
    //
    // public ValueElement GetElement(ValueElements valueElement)
    // {
    //     if (_valueElements.ContainsKey(valueElement)) return _valueElements[valueElement];
    //    
    //     Debug.LogWarning($"The Value Element not found in dictionary: {valueElement}");
    //     return null;
    // }
    // public IElement GetElement(ToggleElements toggleElement)
    // {
    //     if (_toggleElements.ContainsKey(toggleElement)) return _toggleElements[toggleElement];
    //
    //     Debug.LogWarning($"The Value Element not found in dictionary: {toggleElement}");
    //     return null;
    // }
}