using System;
using UnityEngine;

namespace UI.Elements
{
    public class ButtonElement : MonoBehaviour, IElement
    {
        public Action OnUpdated { get; set; }
    }
}