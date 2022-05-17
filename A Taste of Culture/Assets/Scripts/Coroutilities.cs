using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// A collection of Coroutine helper functions that can be used anywhere with access to a MonoBehavior.<br/>
/// Written and edited by Patrick Mitchell @ <see href="https://patrickode.github.io/"/> over the course of multiple projects.
/// </summary>
public static class Coroutilities
{
    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> in <paramref name="delay"/> seconds.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called after <paramref name="delay"/> seconds.</param>
    /// <param name="delay">How many seconds to wait before calling <paramref name="thingToDo"/>.</param>
    /// <param name="realTime">Whether to delay in real time or scaled time; see <see cref="Time.timeScale"/>.</param>
    /// <returns>The delay coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoAfterDelay(MonoBehaviour coroutineCaller, Action thingToDo, float delay, bool realTime = false)
    {
        //If delay is too low, just call the function immediately and bail out
        if (delay <= 0)
        {
            thingToDo();
            return null;
        }
        //Otherwise, start a coroutine which will call the function in delay seconds
        return coroutineCaller.StartCoroutine(DoAfterDelay(thingToDo, delay, realTime));
    }

    /// <inheritdoc cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/>
    private static IEnumerator DoAfterDelay(Action thingToDo, float delay, bool realTime = false)
    {
        if (realTime)
            yield return new WaitForSecondsRealtime(delay);
        else
            yield return new WaitForSeconds(delay);

        thingToDo();
    }



    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> after a given number of <paramref name="frames"/>.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called after <paramref name="delay"/> seconds.</param>
    /// <param name="frames">How many frames to wait before calling <paramref name="thingToDo"/>.</param>
    /// <returns>The delay coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoAfterDelayFrames(MonoBehaviour coroutineCaller, Action thingToDo, int frames)
    {
        //If delay is too low, just call the function immediately and bail out
        if (frames <= 0)
        {
            thingToDo();
            return null;
        }
        //Otherwise, start a coroutine which will call the function in delay seconds
        return coroutineCaller.StartCoroutine(DoAfterDelayFrames(thingToDo, frames));
    }

    /// <inheritdoc cref="DoAfterDelayFrames(MonoBehaviour, Action, int)"/>
    private static IEnumerator DoAfterDelayFrames(Action thingToDo, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        thingToDo();
    }



    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> every <paramref name="interval"/> seconds for <paramref name="duration"/> seconds.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called for <paramref name="delay"/> seconds.</param>
    /// <param name="duration">How many seconds <paramref name="thingToDo"/> should happen for.</param>
    /// <param name="interval"><paramref name="thingToDo"/> will run every <paramref name="interval"/> seconds.<br/>
    ///     <i>(At minimum, every frame; anything less than <see cref="Time.deltaTime"/> will behave the same as <see cref="Time.deltaTime"/>.)</i></param>
    /// <param name="realTime">Whether to repeatedly do <paramref name="thingToDo"/> in real time or scaled time; see <see cref="Time.timeScale"/>.</param>
    /// <returns>The coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoForSeconds(MonoBehaviour coroutineCaller, Action thingToDo, float duration, float interval = 0, bool realTime = false)
    {
        return coroutineCaller.StartCoroutine(DoForSeconds(thingToDo, duration, interval, realTime));
    }

    /// <inheritdoc cref="DoForSeconds(MonoBehaviour, Action, float, float, bool)"/>
    private static IEnumerator DoForSeconds(Action thingToDo, float duration, float interval = 0, bool realTime = false)
    {
        float intervalTimer = 0;
        for (float timer = 0; timer < duration; timer += realTime ? Time.unscaledDeltaTime : Time.deltaTime)
        {
            //After interval seconds (tracked using intervalTimer), run thingToDo and reset the interval timer
            intervalTimer += realTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (intervalTimer >= interval)
            {
                intervalTimer = 0;
                thingToDo();
            }

            yield return null;
        }
    }



    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> once <paramref name="predicate"/> evaluates to true.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called once <paramref name="predicate"/> evaluates to true.</param>
    /// <param name="predicate">Delegate or lambda that will be evaluated every frame. Once it evaluates to true, call <paramref name="thingToDo"/>.</param>
    /// <returns>The coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoWhen(MonoBehaviour coroutineCaller, Action thingToDo, Func<bool> predicate)
    {
        return coroutineCaller.StartCoroutine(DoWhen(thingToDo, predicate));
    }

    /// <inheritdoc cref="DoWhen(MonoBehaviour, Action, Func{bool})"/>
    private static IEnumerator DoWhen(Action thingToDo, Func<bool> predicate)
    {
        yield return new WaitUntil(predicate);
        thingToDo();
    }



    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> every <paramref name="interval"/> seconds, until <paramref name="predicate"/> evaluates to true.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called until <paramref name="predicate"/> evaluates to true.</param>
    /// <param name="predicate">Delegate or lambda that will be evaluated every frame. Once it evaluates to true, stop calling <paramref name="thingToDo"/>.</param>
    /// <param name="interval"><paramref name="thingToDo"/> will run every <paramref name="interval"/> seconds.<br/>
    ///     <i>(At minimum, every frame; anything less than <see cref="Time.deltaTime"/> will behave the same as <see cref="Time.deltaTime"/>.)</i></param>
    /// <returns>The coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoUntil(MonoBehaviour coroutineCaller, Action thingToDo, Func<bool> predicate, float interval = 0, bool realTime = false)
    {
        return coroutineCaller.StartCoroutine(DoUntil(thingToDo, predicate, interval, realTime));
    }

    /// <inheritdoc cref="DoUntil(MonoBehaviour, Action, Func{bool}, float, bool)"/>
    private static IEnumerator DoUntil(Action thingToDo, Func<bool> predicate, float interval, bool realTime = false)
    {
        float intervalTimer = 0;
        while (!predicate())
        {
            //After interval seconds (tracked using intervalTimer), run thingToDo and reset the interval timer
            intervalTimer += realTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (intervalTimer >= interval)
            {
                intervalTimer = 0;
                thingToDo();
            }

            yield return null;
        }
    }



    /// <summary>
    /// Stops a coroutine if it is not null. If it <i>is</i> null (presumed already stopped), this method does nothing.
    /// </summary>
    /// <param name="coroutineLocation">The <see cref="MonoBehaviour"/> that <paramref name="coroutine"/> was started in.</param>
    /// <param name="coroutine">The coroutine to try to stop.</param>
    /// <returns>Whether the coroutine was successfully stopped.</returns>
    public static bool TryStopCoroutine(MonoBehaviour coroutineLocation, Coroutine coroutine)
    {
        if (coroutine != null)
        {
            coroutineLocation.StopCoroutine(coroutine);
            return true;
        }
        return false;
    }

    /// <param name="coroutine">The coroutine to try to stop. <b>Will be set to null if succesfully stopped.</b></param>
    /// <remarks><b>This overload will also nullify the reference to <paramref name="coroutine"/> if it isn't already null.</b><br/>
    /// (A stopped coroutine reference generally isn't good for much, so this may save you an assignment outside of this function.)</remarks>    
    /// <inheritdoc cref="TryStopCoroutine(MonoBehaviour, Coroutine)"/>
    public static bool TryStopCoroutine(MonoBehaviour coroutineLocation, ref Coroutine coroutine)
    {
        if (TryStopCoroutine(coroutineLocation, coroutine))
        {
            coroutine = null;
            return true;
        }
        return false;
    }
}