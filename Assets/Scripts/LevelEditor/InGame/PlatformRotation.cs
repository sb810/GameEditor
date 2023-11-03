using UnityEngine;

namespace LevelEditor.InGame
{
    public class PlatformRotation : MonoBehaviour
    {
        public float speed;
        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(0, 0, speed, Space.World);
        }
    }
}
