using UnityEngine;

namespace LevelEditor
{
    public class HandleVisibility : MonoBehaviour
    {
        public float displayDistance = 1f;

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mouseDistance = Vector2.Distance(mousePosition, gameObject.transform.position);

            if(mouseDistance<displayDistance)
            {
                spriteRenderer.enabled = true;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
    }
}
