using System.Collections;
using PlayerData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodingExercises
{
    public class PreviewCheckpoint : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private int unlockedLevelID;
        public GameObject winScreen;
        public BlockCodeCheck codeCheck;
        public float enemyTotal;
        [HideInInspector] public float enemyCount;

        public float checkpointTotal;
        [HideInInspector] public float checkpointCount;

        private bool hasWon = false;

        private void Start()
        {
            enemyCount = enemyTotal;
            checkpointCount = checkpointTotal;
        }
        private void Update()
        {
            if (hasWon || enemyCount > 0 || checkpointCount > 0) return;
            StartCoroutine(Win());
            hasWon = true;
        }

        private IEnumerator Win()
        {
            if (PlayerDataManager.Data.unlockedCodingLevel < unlockedLevelID)
            {
                PlayerDataManager.Data.unlockedCodingLevel = unlockedLevelID;
                PlayerDataManager.Data.selectedCodingLevel = unlockedLevelID;
            }

            networkManager.UpdateNetworkSavedData();
            yield return new WaitForSeconds(2);
            winScreen.SetActive(true);
            yield return null;
        }
    }
}
