using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SimpleMoveAnimationFunction : MonoBehaviour
{
    [LabelText("Animated transform")] [SerializeField]
    private RectTransform rect;

    [FoldoutGroup("Properties")]
    [LabelText("Initial position")]
    [Tooltip("Relative to the transform anchored position")]
    [SerializeField]
    private Vector2 initialPositionRelative;

    [FoldoutGroup("Properties")]
    [LabelText("End position")]
    [Tooltip("Relative to the calculated initial position")]
    [SerializeField]
    private Vector2 endPositionRelative;

    [FoldoutGroup("Properties")] [SerializeField]
    private float durationSeconds;

    [FormerlySerializedAs("delaySeconds")] [FoldoutGroup("Properties")] [SerializeField]
    private float initialDelay;

    [FoldoutGroup("Properties")] [SerializeField]
    private AnimationCurve smoothingCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [FoldoutGroup("Properties")] [SerializeField]
    private bool animateOnStart;

    [FoldoutGroup("Events")] [SerializeField]
    private UnityEvent onAnimationStart;

    [FoldoutGroup("Events")] [SerializeField]
    private UnityEvent onAnimationEnd;

    private Vector2 initialPositionAbsolute;
    private Vector2 endPositionAbsolute;
    private float animationTime;
    private bool playInReverse;
    private bool animating;
    private bool doAnimationUpdate;

    public void SetDelay(float delay)
    {
        initialDelay = delay;
    }

    private void Awake()
    {
        if (!rect) rect = GetComponent<RectTransform>();
        
        CachedTransform cached = rect.gameObject.GetComponent<CachedTransform>() ??
                                 rect.gameObject.AddComponent<CachedTransform>();
        
        Vector2 pos = cached.AnchoredPosition;
        initialPositionAbsolute = pos + initialPositionRelative;
        endPositionAbsolute = pos + endPositionRelative;
        animationTime = 0f;
    }

    private void OnEnable()
    {
        if (!animateOnStart) return;
        rect.anchoredPosition = initialPositionAbsolute;
        Play();
    }

    [Button]
    public void Play()
    {
        PlayDelayed(initialDelay);
    }
    
    public void PlayDelayed(float delay)
    {
        if (animating)
        {
            if (!playInReverse) return;
        }
        else animationTime = 0f;

        playInReverse = false;
        StartCoroutine(AnimateDelayed(delay));
    }

    [Button]
    public void PlayReversed()
    {
        PlayReversedDelayed(initialDelay);
    }
    
    public void PlayReversedDelayed(float delay)
    {
        if (animating)
        {
            if (playInReverse) return;
        }
        else animationTime = durationSeconds;

        playInReverse = true;
        StartCoroutine(AnimateDelayed(delay));
    }

    private IEnumerator AnimateDelayed(float delay)
    {
        animating = true;
        doAnimationUpdate = false;
        yield return new WaitForSeconds(delay);
        doAnimationUpdate = true;
        onAnimationStart.Invoke();
        yield return null;
    }

    private void Update()
    {
        if (!animating) return;

        float t = animationTime / durationSeconds;
        float s = smoothingCurve.Evaluate(t);

        if ((t <= 0 && playInReverse) || (t >= 1 && !playInReverse))
        {
            s = playInReverse ? 0 : 1;
            onAnimationEnd.Invoke();
            animating = false;
            doAnimationUpdate = false;
        }

        rect.anchoredPosition = new Vector2(
            Mathf.LerpUnclamped(initialPositionAbsolute.x, endPositionAbsolute.x, s),
            Mathf.LerpUnclamped(initialPositionAbsolute.y, endPositionAbsolute.y, s)
        );

        if (doAnimationUpdate)
            animationTime += playInReverse
                ? -Time.deltaTime
                : Time.deltaTime;
    }
}