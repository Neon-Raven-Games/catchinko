using System;
using System.Collections.Generic;
using System.Linq;
using Overworld;
using UnityEngine;


public class OverWorldMap : MonoBehaviour
{
    internal List<OverWorldInnerLevel> levels;
    private void Awake() => levels = GetComponentsInChildren<OverWorldInnerLevel>().ToList();
}
