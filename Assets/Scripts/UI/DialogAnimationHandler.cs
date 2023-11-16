using System.Collections;
using UnityEngine;

namespace UI
{
    public class DialogAnimationHandler : MonoBehaviour
    {
        [SerializeField] private float animationDuration = 0.5f;
        private float counter;
    
        private Vector2 targetSize;
        private RectTransform rt;
    
        // Start is called before the first frame update
        private void Start()
        {
            rt = GetComponent<RectTransform>();
            targetSize = rt.rect.size;
            rt.sizeDelta = Vector2.zero;

            StartCoroutine(ScaleUp());
        }

        private IEnumerator ScaleUp()
        {
            do
            {
                rt.sizeDelta = new Vector2(Mathf.Lerp(rt.sizeDelta.x, targetSize.x, counter / animationDuration),
                    Mathf.Lerp(rt.sizeDelta.y, targetSize.y, counter / animationDuration));
                counter += Time.deltaTime;
            } while (counter < animationDuration);
            yield return null;
        }

        // Update is called once per frame
        private void Update()
        {
        
        }
    }
}
