using UnityEngine;

public abstract class InputValidator<T> : ScriptableObject
{
    protected string inputValue;
    protected T targetValue;
    public abstract bool Validate();
}

public class FloatValidator : InputValidator<float>
{
    public override bool Validate()
    {
        return float.TryParse(inputValue, out targetValue);
    }
}