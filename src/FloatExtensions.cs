using UnityEngine;

public static class FloatExtensions
{
    public static float ExponentialScale(this float inputValue, float midValue, float maxValue)
    {
        float m = maxValue / midValue;
        float c = Mathf.Log(Mathf.Pow(m - 1, 2));
        float b = maxValue / (Mathf.Exp(c) - 1);
        float a = -1 * b;
        return a + b * Mathf.Exp(c * inputValue);
    }
}
