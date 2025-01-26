using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class ShowIf : PropertyAttribute
{
    public string ConditionalFieldName { get; private set; }

    public ShowIf(string conditionalFieldName)
    {
        ConditionalFieldName = conditionalFieldName;
    }
}