using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class AsyncSceneLoader : MonoBehaviour
{
    private enum TransitionType
    {
        Fade,
        Wipe
    }
    [Header("Transition")]
    [SerializeField] private TransitionType transitionType;
    [SerializeField] private Transform endPositionTransform;
    [SerializeField] private float transitionDurationSeconds;
    [SerializeField] private bool playOnStart;
    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);


    [Header("Scene Loading")]
    [SerializeField] private GameObject betweenScenesObject;
    [SerializeField] private Image progressBar;

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
        if(endPositionTransform != null) endPosition = endPositionTransform.position;
        if (playOnStart)
            StartCoroutine(transitionType == TransitionType.Wipe && endPositionTransform ? WipeIn() : FadeIn());
    }

    private IEnumerator WipeIn()
    {
        elapsedTime = 0;
        while (elapsedTime < transitionDurationSeconds)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (curve.Evaluate(elapsedTime / transitionDurationSeconds)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        gameObject.SetActive(false);
        
        yield return null;
    }
    
    private IEnumerator WipeOut()
    {
        elapsedTime = 0;
        while (elapsedTime < transitionDurationSeconds)
        {
            transform.position = Vector3.Lerp(endPosition, startPosition, curve.Evaluate(elapsedTime / transitionDurationSeconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
        
        yield return null;
    }

    private IEnumerator FadeIn()
    {
        elapsedTime = 0;
        Color col = sr.color;
        while (elapsedTime < transitionDurationSeconds)
        {
            sr.color = new Color(col.r, col.g, col.b, Mathf.Lerp(1, 0, (elapsedTime / transitionDurationSeconds)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        sr.color = new Color(col.r, col.g, col.b, 0);
        gameObject.SetActive(false);
        
        yield return null;
    }

    public IEnumerator FadeOut()
    {
        elapsedTime = 0;
        Color col = sr.color;
        while (elapsedTime < transitionDurationSeconds)
        {
            sr.color = new Color(col.r, col.g, col.b, Mathf.Lerp(0, 1, (elapsedTime / transitionDurationSeconds)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        sr.color = new Color(col.r, col.g, col.b, 1);
        
        yield return null;
    }
    
    public async void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOut());
        await Task.Delay(Mathf.CeilToInt(transitionDurationSeconds * 1000));

        float tempTransitionDuration = transitionDurationSeconds;
        transitionDurationSeconds = 0.5f;
        StartCoroutine(FadeIn());

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        betweenScenesObject.SetActive(true);

        do
        {
            await Task.Delay(100);
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, scene.progress, 3 * Time.deltaTime);
        } while (scene.progress < 0.9f);


        StartCoroutine(FadeOut());
        await Task.Delay(500);
        transitionDurationSeconds = tempTransitionDuration;
        
        scene.allowSceneActivation = true;
    }
}
