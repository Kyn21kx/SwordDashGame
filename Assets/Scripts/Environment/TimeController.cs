using System.Collections;
using UnityEngine;

public class TimeController : MonoBehaviour {

    public static TimeController s_instance;

    private void Awake()
    {
        s_instance = this;
    }

    public static void StopTimeFor(float seconds)
    {
        Time.timeScale = 0f;
        s_instance.StartCoroutine(s_instance.RestoreTimeScaleAfter(seconds));
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

}