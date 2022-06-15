using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collection of Coroutine helper functions that can be used anywhere with access to a MonoBehavior.<br/>
/// Written and edited by Patrick Mitchell @ <see href="https://patrickode.github.io/"/> over the course of multiple projects.
/// </summary>
public static class Coroutilities
{
    /// <summary>
    /// Calls <paramref name="thingToDo"/> in <paramref name="delay"/> seconds.
    /// </summary>
    /// <remarks>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since </i><see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/><i><br/> 
    /// is a MonoBehavior function, and MonoBehaviours cannot be static.)</i>
    /// </remarks>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that'll call the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called after <paramref name="delay"/> seconds.</param>
    /// <param name="delay">How many seconds to wait before calling <paramref name="thingToDo"/>.</param>
    /// <param name="realTime">Whether to delay in real time or scaled time; see <see cref="Time.timeScale"/>.</param>
    /// <returns>
    /// The coroutine this function will start. Use it with <see cref="MonoBehaviour.StopCoroutine(Coroutine)"/> or<br/>
    /// <see cref="TryStopCoroutine(MonoBehaviour, Coroutine)"/> to cancel <paramref name="thingToDo"/>.
    /// </returns>
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

    /// <remarks></remarks> <inheritdoc cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/>
    private static IEnumerator DoAfterDelay(Action thingToDo, float delay, bool realTime = false)
    {
        if (realTime)
            yield return new WaitForSecondsRealtime(delay);
        else
            yield return new WaitForSeconds(delay);

        thingToDo();
    }



    /// <summary>
    /// Calls <paramref name="thingToDo"/> after a given number of <paramref name="frames"/>.
    /// </summary>
    /// <param name="thingToDo">The function or lambda expression that will be called after <paramref name="frames"/> frames.</param>
    /// <param name="frames">How many frames to wait before calling <paramref name="thingToDo"/>.</param>
    /// <inheritdoc cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/>
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

    /// <remarks></remarks> <inheritdoc cref="DoAfterDelayFrames(MonoBehaviour, Action, int)"/>
    private static IEnumerator DoAfterDelayFrames(Action thingToDo, int frames)
    {
        for (int i = 0; i < frames; i++)
            yield return null;

        thingToDo();
    }



    /// <summary>
    /// Calls <paramref name="thingToDo"/> after <paramref name="yielder"/>, whatever it is, is done.
    /// </summary>
    /// <param name="thingToDo">The function or lambda expression that will be called after <paramref name="yielder"/>.</param>
    /// <param name="yielder">The thing that comes before <paramref name="thingToDo"/>. This could be a <see cref="Coroutine"/>, 
    /// an <see cref="AsyncOperation"/>, or any other child of <see cref="YieldInstruction"/>.</param>
    /// <inheritdoc cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/>
    public static Coroutine DoAfterYielder(MonoBehaviour coroutineCaller, Action thingToDo, YieldInstruction yielder)
        => coroutineCaller.StartCoroutine(DoAfterYielder(thingToDo, yielder));

    /// <remarks></remarks> <inheritdoc cref="DoAfterYielder(MonoBehaviour, Action, YieldInstruction)"/>
    private static IEnumerator DoAfterYielder(Action thingToDo, YieldInstruction yielder)
    {
        yield return yielder;
        thingToDo();
    }

    /// <summary>
    /// Calls <paramref name="thingToDo"/> after all <paramref name="yielders"/> <b>(run in parallel)</b> are done.
    /// </summary>
    /// <param name="yielders">The things that come before <paramref name="thingToDo"/>. They could be 
    /// <see cref="Coroutine"/>s, <see cref="AsyncOperation"/>s, or any other child of <see cref="YieldInstruction"/>.</param>
    /// <inheritdoc cref="DoAfterYielder(MonoBehaviour, Action, YieldInstruction)"/>
    public static Coroutine DoAfterYielder(MonoBehaviour coroutineCaller, Action thingToDo, params YieldInstruction[] yielders)
    {
        HashSet<YieldInstruction> activeYielders = new HashSet<YieldInstruction>();
        foreach (var yielder in yielders)
        {
            //Add this yielder to our set of active ones. When it's done, remove it.
            activeYielders.Add(yielder);
            DoAfterYielder(coroutineCaller, () => activeYielders.Remove(yielder), yielder);
        }

        return coroutineCaller.StartCoroutine(DoWhen(thingToDo, () => activeYielders.Count < 1));
    }



    /// <summary>
    /// Calls <paramref name="thingToDo"/> after a sequence of <paramref name="yielders"/>, called one by one in the order they were passed.
    /// </summary>
    /// <param name="yielders">Any number of functions or lambdas that return <see cref="YieldInstruction"/>s; for example,<br/>
    /// <c>() =&gt; <see cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/></c>.</param>
    /// <inheritdoc cref="DoAfterYielder(MonoBehaviour, Action, YieldInstruction[])"/>
    public static Coroutine DoAfterSequence(MonoBehaviour coroutineCaller, Action thingToDo, params Func<object>[] yielders)
        => coroutineCaller.StartCoroutine(DoAfterSequence(thingToDo, yielders));

    private static IEnumerator DoAfterSequence(Action thingToDo, params Func<object>[] yielders)
    {
        //Call each of the yielders and wait on the YieldInstruction they return.
        foreach (var yielder in yielders)
            yield return yielder();

        thingToDo();
    }



    /// <summary>
    /// Calls <paramref name="thingToDo"/> every <paramref name="interval"/> seconds for <paramref name="duration"/> seconds.
    /// </summary>
    /// <param name="thingToDo">The function or lambda expression that will be called for <paramref name="delay"/> seconds.</param>
    /// <param name="duration">How many seconds <paramref name="thingToDo"/> should happen for.</param>
    /// <param name="interval"><paramref name="thingToDo"/> will run every <paramref name="interval"/> seconds.<br/>
    /// <i>
    ///     (At minimum, every frame; anything less than </i><see cref="Time.deltaTime"/><i> will behave the<br/>same as </i><see cref="Time.deltaTime"/>.<i>)
    /// </i></param>
    /// <param name="realTime">Whether to repeatedly do <paramref name="thingToDo"/> in real time or scaled time; see <see cref="Time.timeScale"/>.</param>
    /// <inheritdoc cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/>
    public static Coroutine DoForSeconds(MonoBehaviour coroutineCaller, Action thingToDo, float duration, float interval = 0, bool realTime = false)
        => coroutineCaller.StartCoroutine(DoForSeconds(thingToDo, duration, interval, realTime));

    /// <remarks></remarks> <inheritdoc cref="DoForSeconds(MonoBehaviour, Action, float, float, bool)"/>
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
    /// Calls <paramref name="thingToDo"/> for each thing in <paramref name="eachOfThese"/>.
    /// </summary>
    /// <param name="thingToDo">The thing to do for <paramref name="eachOfThese"/>.</param>
    /// <param name="eachOfThese">A "<see langword="foreach"/>-able" collection of things. <paramref name="thingToDo"/> will happen once for each of them.</param>
    /// <param name="timeBetween">How long to wait between each call of <paramref name="thingToDo"/> in seconds.</param>
    /// <param name="realTime">Whether to <see langword="yield"/> in real time or scaled time; see <see cref="Time.timeScale"/>.</param>
    /// <inheritdoc cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/>
    public static Coroutine DoForEach(MonoBehaviour coroutineCaller, Action thingToDo, IEnumerable eachOfThese, float timeBetween = 0, bool realTime = false)
        => coroutineCaller.StartCoroutine(DoForEach(thingToDo, eachOfThese, timeBetween, realTime));

    /// <remarks></remarks> <inheritdoc cref="DoForEach(MonoBehaviour, Action, IEnumerable, float, bool)"/>
    private static IEnumerator DoForEach(Action thingToDo, IEnumerable eachOfThese, float timeBetween = 0, bool realTime = false)
    {
        foreach (var _ in eachOfThese)
        {
            thingToDo();

            if (realTime)
                yield return new WaitForSeconds(timeBetween);
            else
                yield return new WaitForSecondsRealtime(timeBetween);
        }
    }

    /// <summary>
    /// Calls <paramref name="thingToDo"/> for each thing in <paramref name="eachOfThese"/>.<br/> 
    /// This overload takes in a type, so the <see langword="foreach"/> iteration variable can be used as a 
    /// parameter in <paramref name="thingToDo"/>.
    /// </summary>
    /// <typeparam name="T">The type of thing contained in <paramref name="eachOfThese"/>. <paramref name="thingToDo"/> takes
    /// one argument of type <typeparamref name="T"/>,<br/>i.e., the the <see langword="foreach"/> iteration variable.</typeparam>
    /// <inheritdoc cref="DoForEach(Action, IEnumerable, float, bool)"/>
    public static Coroutine DoForEach<T>(MonoBehaviour coroutineCaller, Action<T> thingToDo, IEnumerable<T> eachOfThese, float timeBetween = 0, bool realTime = false)
        => coroutineCaller.StartCoroutine(DoForEach(thingToDo, eachOfThese, timeBetween, realTime));

    /// <remarks></remarks> <inheritdoc cref="DoForEach{T}(MonoBehaviour, Action{T}, IEnumerable{T}, float, bool)"/>
    private static IEnumerator DoForEach<T>(Action<T> thingToDo, IEnumerable<T> eachOfThese, float timeBetween = 0, bool realTime = false)
    {
        foreach (var item in eachOfThese)
        {
            thingToDo(item);

            if (realTime)
                yield return new WaitForSeconds(timeBetween);
            else
                yield return new WaitForSecondsRealtime(timeBetween);
        }
    }



    /// <summary>
    /// Calls <paramref name="thingToDo"/> once <paramref name="predicate"/> evaluates to true.
    /// </summary>
    /// <param name="thingToDo">The function or lambda expression that will be called once <paramref name="predicate"/> evaluates to true.</param>
    /// <param name="predicate">Delegate or lambda that will be evaluated every frame. Once it evaluates to true, call <paramref name="thingToDo"/>.</param>
    /// <inheritdoc cref="DoAfterDelay(MonoBehaviour, Action, float, bool)"/>
    public static Coroutine DoWhen(MonoBehaviour coroutineCaller, Action thingToDo, Func<bool> predicate)
    {
        return coroutineCaller.StartCoroutine(DoWhen(thingToDo, predicate));
    }

    /// <remarks></remarks> <inheritdoc cref="DoWhen(MonoBehaviour, Action, Func{bool})"/>
    private static IEnumerator DoWhen(Action thingToDo, Func<bool> predicate)
    {
        yield return new WaitUntil(predicate);
        thingToDo();
    }



    /// <summary>
    /// Calls <paramref name="thingToDo"/> every <paramref name="interval"/> seconds, until <paramref name="predicate"/> evaluates to true.
    /// </summary>
    /// <param name="thingToDo">The function or lambda expression that will be called until <paramref name="predicate"/> evaluates to true.</param>
    /// <param name="predicate">Delegate or lambda that will be evaluated every frame. Once it evaluates to true, stop calling <paramref name="thingToDo"/>.</param>
    /// <inheritdoc cref="DoForSeconds(MonoBehaviour, Action, float, float, bool)"/>
    public static Coroutine DoUntil(MonoBehaviour coroutineCaller, Action thingToDo, Func<bool> predicate, float interval = 0, bool realTime = false)
    {
        return coroutineCaller.StartCoroutine(DoUntil(thingToDo, predicate, interval, realTime));
    }

    /// <remarks></remarks> <inheritdoc cref="DoUntil(MonoBehaviour, Action, Func{bool}, float, bool)"/>
    private static IEnumerator DoUntil(Action thingToDo, Func<bool> predicate, float interval = 0, bool realTime = false)
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

    /// <remarks>
    /// <b>This overload will also nullify the reference to <paramref name="coroutine"/> if it isn't already null.</b><br/>
    /// (A stopped coroutine reference generally isn't good for much, so this may save you an assignment outside of this function.)
    /// </remarks>    
    /// <param name="coroutine">The coroutine to try to stop. <b>Will be set to null if succesfully stopped.</b></param>
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