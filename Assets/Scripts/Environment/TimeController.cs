using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimeController : MonoBehaviour {

    public static TimeController s_instance;

    private delegate IEnumerator CallbackCoroutine(float s);

    private void Awake()
    {
        s_instance = this;
    }

    public static void StopTimeFor(float seconds)
    {
        Time.timeScale = 0f;
        s_instance.StartCoroutine(s_instance.RestoreTimeScaleAfter(seconds));
    }

    public static void StopTimeForWithDelay(float secondsToStop, float startDelay)
    {
        s_instance.ModifyTimeScaleAfter(0f, startDelay, secondsToStop, s_instance.RestoreTimeScaleAfter);
    }

    public static void SlowTimeFor(float timeScale, float seconds)
    {
        Time.timeScale = timeScale;
        s_instance.StartCoroutine(s_instance.RestoreTimeScaleAfter(seconds));
    }

    private IEnumerator RestoreTimeScaleAfter(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1f;
    }

    private IEnumerator ModifyTimeScaleAfter(float timeScale, float secondsBeforeMod, float secondsToHold, CallbackCoroutine restoreTimeCoroutine) {
        yield return new WaitForSecondsRealtime(secondsBeforeMod);
        Time.timeScale = timeScale;
        this.StartCoroutine(restoreTimeCoroutine(secondsToHold));
    }

}