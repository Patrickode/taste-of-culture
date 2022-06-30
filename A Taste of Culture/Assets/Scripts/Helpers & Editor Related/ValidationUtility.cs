using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <b>Code Author: cxode, via <br/>
/// <see href="https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/#post-7841490"/></b><br/>
/// Sometimes, when you use Unity's built-in OnValidate, it will spam you with a very annoying warning message,
/// even though nothing has gone wrong.<br/>
/// To avoid this, you can run your OnValidate code through this utility.
/// Minorly edited by Patrick Mitchell.
/// </summary>
public static class ValidationUtility
{
    /// <summary>
    /// Call this during OnValidate (or any other editor-scripting context that needs this).<br/>
    /// Runs <paramref name="thingToDo"/> once, after all inspectors have been updated. This can prevent<br/>
    /// annoying and irrelevant unity warnings/errors from clogging the log.
    /// </summary>
    /// <example><code>
    /// private void OnValidate()
    ///{
    ///    ValidationUtility.SafeOnValidate(() =>
    ///    {
    ///        // Put your OnValidate code here
    ///    });
    ///}
    /// </code></example>
    public static void DoOnDelayCall(Action thingToDo)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += _OnValidate;


        void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;

            thingToDo();
        }
#endif
    }

    /// <remarks>
    /// This overload checks if <paramref name="callSource"/> is null before performing <paramref name="onValidateAction"/>.<br/>
    /// If you want <paramref name="onValidateAction"/> to happen regardless if <paramref name="callSource"/> is null or destroyed,<br/>
    /// call the overload without the <paramref name="callSource"/> parameter.
    /// </remarks>
    /// <param name="callSource">Pass <see langword="this"/> into this parameter. It'll be null checked 
    /// before <paramref name="onValidateAction"/> is called.</param>
    /// <inheritdoc cref="DoOnDelayCall(Action)"/>
    public static void DoOnDelayCall(MonoBehaviour callSource, Action onValidateAction)
    {
#if UNITY_EDITOR
        DoOnDelayCall(() =>
        {
            if (!callSource)
                return;

            onValidateAction();
        });
#endif
    }
}