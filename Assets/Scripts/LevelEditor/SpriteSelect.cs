using UnityEngine;

namespace LevelEditor
{
    public class SpriteSelect : MonoBehaviour
    {
        public PrefabInButton button;

        public void ChoosePrefab(GameObject prefab)
        {
            button.prefab = prefab;
        }
    }
}
