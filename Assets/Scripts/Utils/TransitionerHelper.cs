using UnityEngine;

namespace Utils
{
    public class TransitionerHelper : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private bool overrideSettings;
        [Space][Header("Settings")]
        [Range(0,20)]
        [SerializeField] private int widthOfTransitionInBlocks;
        [SerializeField] private GameObject transitionBlockPrefab;
        [SerializeField] private Sprite transitionBlockSprite;
        [SerializeField] private GameObject transitionOrderPrefab;

        public void LoadScene(string sceneName)
        {
            OverrideSettings();
            Transitioner.Instance.TransitionToScene(sceneName);
        }
        
        public void LoadScene(int sceneIndex)
        {
            OverrideSettings();
            Transitioner.Instance.TransitionToScene(sceneIndex);
        }

        public void OverrideSettings()
        {
            if (!overrideSettings) return;
            if (widthOfTransitionInBlocks > 0) Transitioner.Instance._widthOfTransitionInBlocks = widthOfTransitionInBlocks;
            if (transitionBlockPrefab) Transitioner.Instance._transitionBlockPrefab = transitionBlockPrefab;
            if (transitionBlockSprite) Transitioner.Instance._transitionBlockSprite = transitionBlockSprite;
            if (transitionOrderPrefab) Transitioner.Instance._transitionOrderPrefab = transitionOrderPrefab;
        }
    }
}
