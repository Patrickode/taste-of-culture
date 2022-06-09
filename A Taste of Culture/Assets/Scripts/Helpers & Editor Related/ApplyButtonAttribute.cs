using UnityEngine;

public class ApplyButtonAttribute : PropertyAttribute
{
    public readonly bool newLine;

    public ApplyButtonAttribute(bool newLine = false)
    {
        this.newLine = newLine;
    }
}
