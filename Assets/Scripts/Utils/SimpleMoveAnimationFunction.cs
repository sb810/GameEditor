using System.Collections;
// using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Utils
{
    public class SimpleMoveAnimationFunction : MonoBehaviour
    {
        //[LabelText("Animated transform")]
        [SerializeField]
        private RectTransform rect;

        //[FoldoutGroup("Properties")]
        //[LabelText("Initial position")]
        [Tooltip("Relative to the transform anchored position")]
        [SerializeField]
        private Vector2 initialPositionRelative;

        //[FoldoutGroup("Properties")]
        //[LabelText("End position")]
        [Tooltip("Relative to the calculated initial position")]
        [SerializeField]
        private Vector2 endPositionRelative;

        //[FoldoutGroup("Properties")]
        [SerializeField]
        private float durationSeconds;

        //[FoldoutGroup("Properties")]
        [FormerlySerializedAs("delaySeconds")]  [SerializeField]
        private float initialDelay;

        //[FoldoutGroup("Properties")]
        [SerializeField]
        private AnimationCurve smoothingCurve = AnimationCurve.Linear(0, 0, 1, 1);

        //[FoldoutGroup("Properties")] 
        [SerializeField]
        private bool animateOnStart;

        //[FoldoutGroup("Events")] 
        [SerializeField]
        private UnityEvent onAnimationStart;

        // [FoldoutGroup("Events")] 
        [SerializeField]
        private UnityEvent onAnimationEnd;

        private Vector2 initialPositionAbsolute;
        private Vector2 endPositionAbsolute;
        private float animationTime;
        private bool playInReverse;
        private bool animating;
        private bool doAnimationUpdate;

        private bool doDelayUpdate;
        private float delayTime;
        private float currentDelay;

        public void SetDelay(float delay)
        {
            initialDelay = delay;
        }

        private void Awake()
        {
            Time.timeScale = 1;
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

        // [Button]
        public void Play()
        {
            PlayDelayed(initialDelay);
        }
    
        public void PlayDelayed(float delay)
        {
            if (!gameObject.activeSelf) return;
            if (animating)
            {
                if (!playInReverse) return;
            }
            else animationTime = 0f;

            playInReverse = false;
            AnimateDelayed(delay);
        }

        // [Button]
        public void PlayReversed()
        {
            PlayReversedDelayed(initialDelay);
        }
    
        public void PlayReversedDelayed(float delay)
        {
            if (!gameObject.activeSelf) return;
            if (animating)
            {
                if (playInReverse) return;
            }
            else animationTime = durationSeconds;

            playInReverse = true;
            AnimateDelayed(delay);
        }

        private void AnimateDelayed(float delay)
        {
            currentDelay = delay;
            delayTime = 0;
            doDelayUpdate = true;
            //Debug.Log("AnimateDelayed Started ! Waiting for " + delay + " seconds.");
            //StopAllCoroutines();
            //Debug.Log("Stopped all coroutines...");
            animating = true;
            doAnimationUpdate = false;
            //yield return new WaitForSecondsRealtime(delay);
            //Debug.Log("Waited for " + delay + " seconds. Animation update should start.");
            //doAnimationUpdate = true;
            //onAnimationStart.Invoke();
            //yield return null;
            //Debug.Log("AnimateDelayed Ended.");
        }

        private void Update()
        {
            if (doDelayUpdate)
            {
                if (delayTime >= currentDelay)
                {
                    doDelayUpdate = false;
                    doAnimationUpdate = true;
                    onAnimationStart.Invoke();
                }
                    
                delayTime += Time.deltaTime;
            }
            
            if (!animating) return;
            
            float t = animationTime / durationSeconds;
            float s = Mathf.Clamp(smoothingCurve.Evaluate(t), 0, 2);

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
}