using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleizeEdgeCollider : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private EdgeCollider2D edgeColl;
    [Space(5)]
    [SerializeField] private bool clockwise;
    [SerializeField] [Range(0, 360)] private float startAngle;
    [Space(5)]
    [SerializeField] [Range(3, 300)] private int numEdges = 12;
    [Tooltip("360 = a full circle. 180 = a half circle. 0 = nothing.")]
    [SerializeField] [Range(0, 360)] private int arcLength = 360;
    [ApplyButton(true)] [SerializeField] [Min(0)] private float radius = 0.5f;
    private float radStart;
    private float radLength;

#pragma warning disable IDE0051, IDE0052, CS0414 // Remove unread private members
    private void OnApplyClicked(UnityEditor.SerializedProperty _) => Circleize();
#pragma warning restore IDE0051, IDE0052, CS0414 // Remove unread private members

    /// <summary>
    /// Adapted from http://answers.unity.com/comments/1575166/view.html.
    /// </summary>
    private void Circleize()
    {
        if (!edgeColl || numEdges < 3 || arcLength <= 0) return;

        radStart = startAngle * Mathf.Deg2Rad;
        radLength = arcLength * Mathf.Deg2Rad;

        if (clockwise)
        {
            radStart *= -1;
            radLength *= -1;
        }

        //Add an extra edge if arc length is a full circle, to close the circle.
        Vector2[] points = new Vector2[numEdges + 1];
        for (int i = 0; i < numEdges; i++)
        {
            float angle = radStart + radLength * i / numEdges;
            points[i] = new Vector2(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle));
        }

        //"Connect" the end points; this ensures proper collision normals (represented in the
        //editor by magenta lines). We only do this if making a full circle.
        //  See https://docs.unity3d.com/ScriptReference/EdgeCollider2D-adjacentStartPoint.html
        if (arcLength >= 360)
        {
            //First, close the circle.
            points[numEdges] = points[0];
            //Then set the virtual points to their "adjacent" point in the array.
            edgeColl.useAdjacentStartPoint = true;
            edgeColl.adjacentStartPoint = points[numEdges - 1];
            edgeColl.useAdjacentEndPoint = true;
            edgeColl.adjacentEndPoint = points[1];
        }
        else
        {
            //Our last point is the end of the arcLength angle.
            points[numEdges] = new Vector2(
                radius * Mathf.Cos(radStart + radLength),
                radius * Mathf.Sin(radStart + radLength));

            edgeColl.useAdjacentStartPoint = false;
            edgeColl.useAdjacentEndPoint = false;
        }

        edgeColl.points = points;
    }
#endif
}