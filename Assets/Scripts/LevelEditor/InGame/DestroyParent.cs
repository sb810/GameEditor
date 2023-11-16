using UnityEngine;

namespace LevelEditor.InGame
{
    public class DestroyParent : MonoBehaviour
    {
        public void DeathFunction()
        {
            transform.parent.gameObject.SetActive(false);
        }

    }
}
