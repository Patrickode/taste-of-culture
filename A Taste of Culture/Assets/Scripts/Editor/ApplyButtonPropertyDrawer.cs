using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A custom property drawer for adding an "apply" button next to/beneath a field.
/// Largely extrapolated from <see href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html"/>.
/// </summary>
[CustomPropertyDrawer(typeof(ApplyButtonAttribute))]
public class ApplyButtonPropertyDrawer : PropertyDrawer
{
    private readonly float xSpacing = 5;
    private readonly float ySpacing = 2;

    private ApplyButtonAttribute _typedAttrib;
    private ApplyButtonAttribute TypedAttrib
    {
        get
        {
            _typedAttrib ??= attribute as ApplyButtonAttribute;
            return _typedAttrib;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //If using a new line, double height plus spacing.
        if (TypedAttrib.newLine)
            return base.GetPropertyHeight(property, label) * 2 + ySpacing;
        else
            return base.GetPropertyHeight(property, label);
    }

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        Rect prefixlessPos = position;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        float thirdWidth = position.width / 3;
        Rect fieldRect = position;
        Rect buttonRect = prefixlessPos;

        if (TypedAttrib.newLine)
        {
            fieldRect.height /= 2;
            buttonRect.height /= 2;
            buttonRect.y += fieldRect.height + ySpacing;
        }
        else
        {
            fieldRect.width = thirdWidth * 2;
            buttonRect.width = thirdWidth - xSpacing;
            buttonRect.x += fieldRect.width + xSpacing;
        }

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(fieldRect, property, GUIContent.none);

        //Make a button. If it's clicked...
        GUIStyle bStyle = new GUIStyle(GUI.skin.button);
        bStyle.padding.top = 0;
        bStyle.padding.bottom = 0;

        if (GUI.Button(buttonRect, "Apply", bStyle))
        {
            //...and if it's a monobehaviour, let it handle the click logic.
            //  Yes, SendMessage is icky, but all my alternatives were icky, too.
            if (property.serializedObject.targetObject is MonoBehaviour propSource)
                propSource.SendMessage("OnApplyClicked", property, SendMessageOptions.DontRequireReceiver);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}