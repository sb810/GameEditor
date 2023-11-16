using System.Collections;
using UnityEngine;

namespace Utils
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float seconds = 1.0f;
    
        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }

        private void Start()
        {
            StartCoroutine(Timer());
        }
    }
}
