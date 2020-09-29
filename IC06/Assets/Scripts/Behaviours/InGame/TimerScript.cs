using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    // REFERENCES
    public RectTransform rectTransform;
    public Image Fill;
    public Image Frame;
    public Image Back;

    // PROPRIETES
    [Range(1f, 5f)]
    public float vitesseFade = 2f;
    [Range(0.1f, 1f)]
    public float opaciteBack = 0.75f;

    [HideInInspector]
    public string playerName;   // Nom du joueur ayant activé le timer

    // METHODES
    void Start()
    {
        // REFERENCES MANQUANTES
        if(Fill == null)
        {
            Fill = transform.Find("Fill").GetComponent<Image>();
        }
        if (Frame == null)
        {
            Frame = transform.Find("Frame").GetComponent<Image>();
        }
        if (Back == null)
        {
            Back = transform.Find("Back").GetComponent<Image>();
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        this.gameObject.SetActive(false);
    }

    public void LaunchTimer(float duration)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(LaunchTimer_CRT(duration));
    }
    public IEnumerator LaunchTimer_CRT(float duration)
    {
        // INIT
        float tempsRestant = duration;
        StartCoroutine(FadeIn());

        // ANIMATION
        while (tempsRestant > 0)
        {
            // DECREMENTATION
            tempsRestant -= Time.deltaTime;

            // POSITION
            Fill.fillAmount = tempsRestant / duration;

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(FadeOut());
        yield return null;
    }

    private IEnumerator FadeIn()
    {
        // INIT
        float delta = 0f;

        // ANIMATION
        while (delta < 1)
        {
            // INCREMENTATION
            delta += Time.deltaTime * vitesseFade;

            // ALPHA
            Color c = Fill.color;
            c.a = delta;
            Fill.color = c;

            c = Frame.color;
            c.a = delta;
            Frame.color = c;

            if(delta < opaciteBack)
            {
                c = Back.color;
                c.a = delta;
                Back.color = c;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private IEnumerator FadeOut()
    {
        // INIT
        float delta = 1f;

        // ANIMATION
        while (delta > 0)
        {
            // INCREMENTATION
            delta -= Time.deltaTime * vitesseFade;

            // ALPHA
            Color c = Fill.color;
            c.a = delta;
            Fill.color = c;

            c = Frame.color;
            c.a = delta;
            Frame.color = c;

            if (delta < opaciteBack)
            {
                c = Back.color;
                c.a = delta;
                Back.color = c;
            }

            yield return new WaitForEndOfFrame();
        }

        this.gameObject.SetActive(false);
        yield return null;
    }
}
