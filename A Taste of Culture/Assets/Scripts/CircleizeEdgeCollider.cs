using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleizeEdgeCollider : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private EdgeCollider2D edgeColl;
    [SerializeField] [Range(3, 300)] private int numEdges = 12;
    [ApplyButton(true)] [SerializeField] [Min(0)] private float radius = 0.5f;

#pragma warning disable IDE0051, IDE0052, CS0414 // Remove unread private members
    private void OnApplyClicked(UnityEditor.SerializedProperty _) => Circleize();
#pragma warning restore IDE0051, IDE0052, CS0414 // Remove unread private members

    /// <summary>
    /// Adapted from http://answers.unity.com/comments/1575166/view.html.
    /// </summary>
    private void Circleize()
    {
        if (!edgeColl) return;

        Vector2[] points = new Vector2[numEdges + 1];
        for (int i = 0; i < numEdges; i++)
        {
            //float targetRadius = 0.5f * 
            float angle = 2 * Mathf.PI * i / numEdges;
            points[i] = new Vector2(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle));
        }
        points[numEdges] = points[0];

        edgeColl.points = points;

        //"Connect" the end points; this ensures proper collision normals
        //  See https://docs.unity3d.com/ScriptReference/EdgeCollider2D-adjacentStartPoint.html
        edgeColl.useAdjacentStartPoint = true;
        edgeColl.adjacentStartPoint = points[numEdges - 1];
        edgeColl.useAdjacentEndPoint = true;
        edgeColl.adjacentEndPoint = points[1];
    }
#endif
}