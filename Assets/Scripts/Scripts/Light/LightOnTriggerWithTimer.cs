using UnityEngine;
using System.Collections;

public class LightOnTriggerWithTimer : MonoBehaviour
{
    [Header("Target Light")]
    public Light lightToControl;

    [Header("Delays")]
    [Tooltip("ÊÃÎíÑ ÇáÊÔÛíá ÈÚÏ ÏÎæá ÇááÇÚÈ (ËæÇäí)")]
    public float delayOnEnter = 0.5f;
    [Tooltip("ÊÃÎíÑ ÇáÅØİÇÁ ÈÚÏ ÎÑæÌ ÇááÇÚÈ (ËæÇäí)")]
    public float delayOffExit = 0.5f;

    [Header("Auto Off")]
    [Tooltip("áæ ÃßÈÑ ãä ÕİÑ: íØİí ÊáŞÇÆíğÇ ÈÚÏ åĞå ÇáãÏÉ ãä ÇáÊÔÛíá (ËæÇäí)")]
    public float autoOffAfter = 0f;

    Coroutine onRoutine, offRoutine, autoOffRoutine;
    bool playerInside;

    void Start()
    {
        if (lightToControl) lightToControl.enabled = false; // íÈÏÃ ØÇİí
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !lightToControl) return;

        playerInside = true;

        // ÃáÛö Ãí ÊÇíãÑ ÅØİÇÁ ŞíÏ ÇáÊÔÛíá
        if (offRoutine != null) StopCoroutine(offRoutine);

        // ÇÈÏÃ ÊÇíãÑ ÇáÊÔÛíá
        if (onRoutine != null) StopCoroutine(onRoutine);
        onRoutine = StartCoroutine(TurnOnAfter(delayOnEnter));
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !lightToControl) return;

        playerInside = false;

        // ÃáÛö Ãí ÊÇíãÑ ÊÔÛíá ŞíÏ ÇáÊÔÛíá
        if (onRoutine != null) StopCoroutine(onRoutine);

        // ÇÈÏÃ ÊÇíãÑ ÇáÅØİÇÁ
        if (offRoutine != null) StopCoroutine(offRoutine);
        offRoutine = StartCoroutine(TurnOffAfter(delayOffExit));
    }

    IEnumerator TurnOnAfter(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        // áæ ÎÑÌ ÇááÇÚÈ ŞÈá ãÇ íßãøá ÇáÊÇíãÑ¡ áÇ ÊÔÛøá
        if (!playerInside) yield break;

        lightToControl.enabled = true;

        // ÔÛøá ãÄŞøÊ ÇáÅØİÇÁ ÇáÊáŞÇÆí (ÇÎÊíÇÑí)
        if (autoOffAfter > 0f)
        {
            if (autoOffRoutine != null) StopCoroutine(autoOffRoutine);
            autoOffRoutine = StartCoroutine(AutoOffCountdown(autoOffAfter));
        }
    }

    IEnumerator TurnOffAfter(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        // áæ ÑÌÚ ÇááÇÚÈ ŞÈá ãÇ íßãøá ÇáÊÇíãÑ¡ áÇ ÊØİí
        if (playerInside) yield break;

        lightToControl.enabled = false;

        // ÃæŞİ ãÄŞøÊ ÇáÅØİÇÁ ÇáÊáŞÇÆí áæ ßÇä ÔÛÇá
        if (autoOffRoutine != null)
        {
            StopCoroutine(autoOffRoutine);
            autoOffRoutine = null;
        }
    }

    IEnumerator AutoOffCountdown(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // íØİí ÍÊì áæ ÇááÇÚÈ ÏÇÎá—ÇÍĞİ ÇáÔÑØ ÇáÊÇáí ÅĞÇ ÊÈÛÇå íØİí ÏÇÆãğÇ
        if (playerInside) yield break;

        lightToControl.enabled = false;
    }
}
