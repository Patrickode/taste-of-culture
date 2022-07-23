using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <b>Code Author: cxode, via <br/>
/// <see href="https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/#post-7841490"/></b>
/// <br/><br/>
/// Doing certain things in OnValidate can flood the log with warning messages (sometimes outright incorrectly!).<br/>
/// To avoid this, you can run that OnValidate code through this class's methods instead.<br/>
/// <i>Edited by Patrick Mitchell @ <see href="https://patrickode.github.io"/>.</i>
/// </summary>
public static class ValidationUtility
{
    /// <summary>
    /// <i>NOTE: This method's contents are wrapped in an </i><c>#if UNITY_EDITOR</c><i>.</i><br/><br/>
    /// Call this during OnValidate (or any other editor-scripting context that needs this).<br/>
    /// Runs <paramref name="thingToDo"/> once, after all inspectors have 
    /// been updated (see <see cref="UnityEditor.EditorApplication.delayCall"/>).<br/>
    /// This can prevent annoying and irrelevant unity warnings/errors from clogging the log.
    /// </summary>
    public static void DoOnDelayCall(Action thingToDo)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += DoThing;

        void DoThing()
        {
            UnityEditor.EditorApplication.delayCall -= DoThing;
            thingToDo();
        }
#endif
    }

    /// <summary>
    /// <i>NOTE: This method's contents are wrapped in an </i><c>#if UNITY_EDITOR</c><i>.</i><br/><br/>
    /// Call this during OnValidate (or any other editor-scripting context that needs this).<br/>
    /// Runs <paramref name="thingToDo"/> once, on the editor's "generic update" 
    /// (see <see cref="UnityEditor.EditorApplication.update"/>).<br/>
    /// This can prevent annoying and irrelevant unity warnings/errors from clogging the log.
    /// </summary>
    public static void DoOnEditorUpdate(Action thingToDo)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += DoThing;

        void DoThing()
        {
            UnityEditor.EditorApplication.update -= DoThing;
            thingToDo();
        }
#endif
    }

    /// <remarks>
    /// This overload checks if <paramref name="callSource"/> is null before performing <paramref name="thingToDo"/>.<br/>
    /// If you want <paramref name="thingToDo"/> to happen regardless if <paramref name="callSource"/> is null or destroyed,<br/>
    /// call the overload without the <paramref name="callSource"/> parameter instead.
    /// </remarks>
    /// <param name="callSource">Pass <see langword="this"/> into this parameter. It'll be null checked 
    /// before <paramref name="thingToDo"/> is called.</param>
    /// <inheritdoc cref="DoOnDelayCall(Action)"/>
    public static void DoOnDelayCall(MonoBehaviour callSource, Action thingToDo)
    {
#if UNITY_EDITOR
        DoOnDelayCall(() =>
        {
            if (!callSource) return;
            thingToDo();
        });
#endif
    }

    /// <inheritdoc cref="DoOnEditorUpdate(Action)"/>
    /// <inheritdoc cref="DoOnDelayCall(MonoBehaviour, Action)"/>
    public static void DoOnEditorUpdate(MonoBehaviour callSource, Action thingToDo)
    {
#if UNITY_EDITOR
        DoOnEditorUpdate(() =>
        {
            if (!callSource) return;
            thingToDo();
        });
#endif
    }
}