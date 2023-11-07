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
        [Range(0,5)]
        [SerializeField] private float transitionTime;
        [Range(0,5)]
        [SerializeField] private float transitionBlockAnimationTime;

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

        private void OverrideSettings()
        {
            if (!overrideSettings) return;
            if (widthOfTransitionInBlocks > 0) Transitioner.Instance._widthOfTransitionInBlocks = widthOfTransitionInBlocks;
            if (transitionBlockPrefab) Transitioner.Instance._transitionBlockPrefab = transitionBlockPrefab;
            if (transitionBlockSprite) Transitioner.Instance._transitionBlockSprite = transitionBlockSprite;
            if (transitionOrderPrefab) Transitioner.Instance._transitionOrderPrefab = transitionOrderPrefab;
            if (transitionTime > 0) Transitioner.Instance._transitionTime = transitionTime;
            if (transitionBlockAnimationTime > 0) Transitioner.Instance._transitionBlockAnimationTime = transitionBlockAnimationTime;
        }
    }
}
