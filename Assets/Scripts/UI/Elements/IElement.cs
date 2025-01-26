using System;

public interface IElement 
{
    Action OnUpdated { get; set; }

}

public interface IValueElement : IElement
{
    float Value { get; set; }
    int MaxValue { get; set; }
    void SetValue(float value);
}

