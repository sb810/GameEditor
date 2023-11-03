using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

namespace Utils
{
    public class AsyncSceneLoader : MonoBehaviour
    {
        private enum TransitionType
        {
            Fade,
            Wipe
        }

        [Header("Transition")] [SerializeField]
        private TransitionType transitionType;

        [SerializeField] private Transform endPositionTransform;
        [SerializeField] private float transitionDurationSeconds;
        [SerializeField] private bool playOnStart;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Scene Loading")] [SerializeField]
        private GameObject betweenScenesObject;

        [SerializeField] private Image progressBar;

        [Header("Callbacks")] [SerializeField] private UnityEvent onTransitionStart;
        [SerializeField] private UnityEvent onTransitionEnd;

        private Vector3 startPosition;
        private Vector3 endPosition;
        private SpriteRenderer sr;
        private float elapsedTime;


        // [SerializeField] private AnimationCurve curve;
        // public Password password;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();

            startPosition = transform.position;
            if (endPositionTransform != null) endPosition = endPositionTransform.position;
            if (playOnStart)
                StartCoroutine(transitionType == TransitionType.Wipe && endPositionTransform ? WipeIn() : FadeIn());
        }

        private IEnumerator WipeIn()
        {
            onTransitionStart.Invoke();
            elapsedTime = 0;
            while (elapsedTime < transitionDurationSeconds)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition,
                    (curve.Evaluate(elapsedTime / transitionDurationSeconds)));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = endPosition;
            onTransitionEnd.Invoke();
            gameObject.SetActive(false);

            yield return null;
        }

        private IEnumerator WipeOut()
        {
            onTransitionStart.Invoke();
            elapsedTime = 0;
            while (elapsedTime < transitionDurationSeconds)
            {
                transform.position = Vector3.Lerp(endPosition, startPosition,
                    curve.Evaluate(elapsedTime / transitionDurationSeconds));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = startPosition;
            onTransitionEnd.Invoke();
            yield return null;
        }

        private IEnumerator FadeIn()
        {
            onTransitionStart.Invoke();
            elapsedTime = 0;
            Color col = sr.color;
            while (elapsedTime < transitionDurationSeconds)
            {
                sr.color = new Color(col.r, col.g, col.b, Mathf.Lerp(1, 0, (elapsedTime / transitionDurationSeconds)));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            sr.color = new Color(col.r, col.g, col.b, 0);
            onTransitionEnd.Invoke();
            gameObject.SetActive(false);
            yield return null;
        }

        public IEnumerator FadeOut()
        {
            onTransitionStart.Invoke();
            elapsedTime = 0;
            Color col = sr.color;
            while (elapsedTime < transitionDurationSeconds)
            {
                sr.color = new Color(col.r, col.g, col.b, Mathf.Lerp(0, 1, (elapsedTime / transitionDurationSeconds)));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            sr.color = new Color(col.r, col.g, col.b, 1);
            onTransitionEnd.Invoke();
            yield return null;
        }

        public void LoadScene(int buildIndex)
        {
            StartCoroutine(LoadSceneAsync(buildIndex));
        }

        private IEnumerator LoadSceneAsync(int buildIndex)
        {
            yield return StartCoroutine(transitionType == TransitionType.Wipe && endPositionTransform
                ? WipeOut()
                : FadeOut());
            // yield return new WaitForSeconds(transitionDurationSeconds);

            float tempTransitionDuration = transitionDurationSeconds;
            transitionDurationSeconds = 0.5f;
            StartCoroutine(transitionType == TransitionType.Wipe && endPositionTransform ? WipeIn() : FadeIn());

            // var scene = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
            SceneManager.LoadScene(buildIndex);

            // scene.allowSceneActivation = false;
            betweenScenesObject.SetActive(true);

            /* int iterations = 0;
        do
        {
            ++iterations;
            Debug.Log("In While ! Iteration " + iterations + ", progress " + scene.progress);
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, scene.progress, 3 * Time.deltaTime);
            yield return null;
        } while (scene.progress < 0.9f && iterations < 100); */


            yield return StartCoroutine(transitionType == TransitionType.Wipe && endPositionTransform
                ? WipeOut()
                : FadeOut());
            // yield return new WaitForSeconds(0.5f);
            transitionDurationSeconds = tempTransitionDuration;

            // scene.allowSceneActivation = true;
        }
    }
}