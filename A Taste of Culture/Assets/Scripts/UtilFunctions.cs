using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilFunctions
{
    private static Vector2 GameViewOrScreenSize() =>
#if UNITY_EDITOR
        UnityEditor.Handles.GetMainGameViewSize();
#else
        new Vector2(Screen.width, Screen.height);
#endif
    /// <summary>
    /// Returns whether the point <paramref name="coords"/> is offscreen or not.
    /// </summary>
    /// <param name="coords">Pixel coordinates representing a position on the screen.</param>
    /// <param name="conversionCam">If this isn't null, this method will convert <paramref name="coords"/> to pixel coords 
    /// itself using this camera,<br/>so the coords don't have to be converted beforehand.</param>
    public static bool PixelCoordsOffScreen(Vector2 coords, Camera conversionCam = null)
    {
        Vector2 maxPixelPos = GameViewOrScreenSize();

        if (conversionCam)
            coords = conversionCam.WorldToScreenPoint(coords);

        return coords.x <= 0 || coords.x >= maxPixelPos.x ||
            coords.y <= 0 || coords.y >= maxPixelPos.y;
    }
    /// <remarks>
    /// This overload will use the <see cref="Bounds.min"/> and <see cref="Bounds.max"/> to check if the <i>entire</i>
    /// bounding box is off-screen.
    /// </remarks>
    /// <param name="coords">An area </param>
    /// <inheritdoc cref="PixelCoordsOffScreen(Vector2, Camera)"/>
    public static bool PixelCoordsOffScreen(Bounds coords, Camera conversionCam)
    {
        Vector2 maxPixelPos = GameViewOrScreenSize();

        if (conversionCam)
        {
            coords.min = conversionCam.WorldToScreenPoint(coords.min);
            coords.max = conversionCam.WorldToScreenPoint(coords.max);
        }
        else Debug.LogWarning("PixelCoordsOffScreen was passed some bounds without a conversion cam; the bounds likely " +
            "aren't in screen space already, so this function my return strange/incorrect results.");

        return coords.max.x <= 0 || coords.min.x >= maxPixelPos.x ||
            coords.max.y <= 0 || coords.min.y >= maxPixelPos.y;
    }

    /// <summary>
    /// Gets the renderers of <paramref name="obj"/> and its children, and returns the combined bounds of 
    /// all the active/enabled ones.<br/>
    /// Note this returns <i><b>rendered</b></i> bounds, not the bounds of the object's collider.
    /// </summary>
    /// <remarks>
    /// <b>Developer's Note:</b> Renderer bounds tend to include transparent fringes, i.e. padding. See<br/>
    /// <see cref="UnityEngine.Sprites.DataUtility.GetPadding(Sprite)"/>.
    /// </remarks>
    /// <param name="obj">The object to get the total rendered bounds of.</param>
    /// <returns>A <see cref="Bounds"/> that encapsulates the all the active/enabled renderers of 
    /// <paramref name="obj"/> and its children.</returns>
    public static Bounds GetTotalRenderedBounds(this GameObject obj)
    {
        //If this parent obj isn't active, none of the children will be either, so we can return early.
        if (!obj.activeInHierarchy)
            return new Bounds(obj.transform.position, Vector3.zero);

        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        return GetTotalRenderedBoundsNonAlloc(rends, obj.transform.position);
    }
    /// <summary>
    /// Returns the combined <see cref="Bounds"/> of all the active/enabled renderers in <paramref name="rends"/>.
    /// </summary>
    /// <param name="rends">The renderers to get the total rendered <see cref="Bounds"/> of.</param>
    /// <param name="defaultCenter">The default center position to use for zero-sized <see cref="Bounds"/>/failures.</param>
    /// <returns></returns> <inheritdoc cref="GetTotalRenderedBounds(GameObject)"/>
    public static Bounds GetTotalRenderedBoundsNonAlloc(Renderer[] rends, Vector3 defaultCenter = default)
    {
        //If there are no renderers, there's no rendered bounds. Return a zero-sized bounds.
        if (rends == null || rends.Length < 1)
            return new Bounds(defaultCenter, Vector3.zero);

        //Init a bounds with the first enabled/active renderer, then expand it to include all the others that're
        //enabled/active.
        Bounds? totalBounds = null;
        foreach (Renderer rend in rends)
            if (rend.enabled && rend.gameObject.activeInHierarchy)
            {
                if (totalBounds is Bounds tBounds)
                    tBounds.Encapsulate(rend.bounds);
                else
                    totalBounds = rend.bounds;
            }

        //If we didn't get to init a bounds, that means none of the renderers are actually showing. In that case,
        //return a zero-sized bounds.
        return totalBounds is Bounds result
            ? result
            : new Bounds(defaultCenter, Vector3.zero);
    }

    public static Bounds EncapsulateAll(params Bounds[] bounds)
    {
        if (bounds.Length < 1)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds result = bounds[0];
        for (int i = 1; i < bounds.Length; i++)
            result.Encapsulate(bounds[i]);

        return result;
    }

    /// <summary>
    /// Gets the <see cref="Bounds"/> of this renderer, gets the padding of its sprite, and makes/returns a<br/>
    /// resized <see cref="Bounds"/> without that padding.
    /// </summary>
    public static Bounds GetBoundsSansPadding(this SpriteRenderer rend)
    {
        if (!rend.sprite)
            return rend.bounds;

        Vector4 padding = UnityEngine.Sprites.DataUtility.GetPadding(rend.sprite) / rend.sprite.pixelsPerUnit;
        padding = Vector4.Scale(padding, new Vector4(
            rend.transform.localScale.x, rend.transform.localScale.y,
            rend.transform.localScale.x, rend.transform.localScale.y));

        //Subtract the combined horizontal/vertical padding from size, then shift the center by half the amount of each side
        Bounds newBounds = rend.bounds;
        newBounds.size -= Vector3.right * (padding.x + padding.z);
        newBounds.center += Vector3.right * (padding.x / 2) + Vector3.left * (padding.z / 2);
        newBounds.size -= Vector3.up * (padding.y + padding.w);
        newBounds.center += Vector3.up * (padding.y / 2) + Vector3.down * (padding.w / 2);

        return newBounds;
    }

    #region Courtesy of unitycoder via https://gist.github.com/unitycoder/58f4b5d80f423d29e35c814a9556f9d9
    public static void DrawBounds(Bounds b, Color c = default, float duration = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);    //---
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);    //+--
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);    //+-+
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);    //--+

        Debug.DrawLine(p1, p2, c, duration);
        Debug.DrawLine(p2, p3, c, duration);
        Debug.DrawLine(p3, p4, c, duration);
        Debug.DrawLine(p4, p1, c, duration);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);    //-+-
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);    //++-
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);    //+++
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);    //-++

        Debug.DrawLine(p5, p6, c, duration);
        Debug.DrawLine(p6, p7, c, duration);
        Debug.DrawLine(p7, p8, c, duration);
        Debug.DrawLine(p8, p5, c, duration);

        // sides
        Debug.DrawLine(p1, p5, c, duration);
        Debug.DrawLine(p2, p6, c, duration);
        Debug.DrawLine(p3, p7, c, duration);
        Debug.DrawLine(p4, p8, c, duration);
    }

    // https://forum.unity.com/threads/debug-drawbox-function-is-direly-needed.1038499/
    public static void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c);
        Debug.DrawLine(point2, point3, c);
        Debug.DrawLine(point3, point4, c);
        Debug.DrawLine(point4, point1, c);

        Debug.DrawLine(point5, point6, c);
        Debug.DrawLine(point6, point7, c);
        Debug.DrawLine(point7, point8, c);
        Debug.DrawLine(point8, point5, c);

        Debug.DrawLine(point1, point5, c);
        Debug.DrawLine(point2, point6, c);
        Debug.DrawLine(point3, point7, c);
        Debug.DrawLine(point4, point8, c);

        // optional axis display (...which apparently causes compiler errors because these get methods don't exist on Matrix4x4s?)
        //Debug.DrawRay(m.GetPosition(), m.GetForward(), Color.magenta);
        //Debug.DrawRay(m.GetPosition(), m.GetUp(), Color.yellow);
        //Debug.DrawRay(m.GetPosition(), m.GetRight(), Color.red);
    }
    #endregion
}
