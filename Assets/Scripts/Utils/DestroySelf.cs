using UnityEngine;

namespace Utils
{
    public class DestroySelf : MonoBehaviour
    {
        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
