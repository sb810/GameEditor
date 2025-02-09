using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LevelEditor
{
    public class FindPlayer : MonoBehaviour
    {
        public ChampSelect champSelect;
        public GameObject button; // button from object selector, should contain a PrefabInButton
        [FormerlySerializedAs("button")] public Image buttonImage; // previous button<s image (can be in another gameobject)
        public GameObject prefab; // button from ChampSelect, should contain a PrefabInButton

        private void Start()
        {
            champSelect.player = gameObject;
            buttonImage.sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            button.gameObject.GetComponent<PrefabInButton>().prefab = prefab.GetComponent<PrefabInButton>().prefab;
        }

    }
}
