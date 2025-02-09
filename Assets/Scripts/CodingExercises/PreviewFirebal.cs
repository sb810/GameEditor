using UnityEngine;

namespace CodingExercises
{
    public class PreviewFirebal : MonoBehaviour
    {

        public float speed = 1;
        // Update is called once per frame
        private void Start()
        {
            if(speed<0)
                transform.localScale *= -1;
        }

        private void Update()
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                collision.gameObject.SetActive(false);
                GameObject.Find("Checkpoint").GetComponent<PreviewCheckpoint>().enemyCount--;
                Destroy(gameObject);
            }
            if (collision.CompareTag("Enemy"))
            {
                GameObject.Find("BlocStart").GetComponent<BlockCodeCheck>().ResetPreview();
                Destroy(gameObject);
            }
        }
    }
}
