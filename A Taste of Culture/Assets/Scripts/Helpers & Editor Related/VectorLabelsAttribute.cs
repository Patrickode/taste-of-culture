using UnityEngine;

/// <summary>
/// Minorly edited from Unity Answers user "Hellium" via 
/// <see href="http://answers.unity.com/answers/1573684/view.html"/>.
/// </summary>
public class VectorLabelsAttribute : PropertyAttribute
{
    public readonly string[] Labels;
    public readonly float LabelPadding;
    public readonly float PaddingBtwnCoords;

    /// <param name="labels">The labels to go with each value in the vector, in XYZW order.</param>
    /// <param name="labelPadding">The extra space between each label and its corresponding value.</param>
    /// <param name="paddingBtwnCoords">The padding between each value in the target vector.</param>
    public VectorLabelsAttribute(float labelPadding, float paddingBtwnCoords, params string[] labels)
    {
        Labels = labels;
        LabelPadding = labelPadding;
        PaddingBtwnCoords = paddingBtwnCoords;
    }
    /// <inheritdoc cref="VectorLabelsAttribute(float, float, string[])"/>
    public VectorLabelsAttribute(float paddingBtwnCoords, params string[] labels)
    {
        Labels = labels;
        LabelPadding = 0;
        PaddingBtwnCoords = paddingBtwnCoords;
    }
    /// <inheritdoc cref="VectorLabelsAttribute(float, float, string[])"/>
    public VectorLabelsAttribute(params string[] labels)
    {
        Labels = labels;
        LabelPadding = 0;
        PaddingBtwnCoords = 2;
    }
}