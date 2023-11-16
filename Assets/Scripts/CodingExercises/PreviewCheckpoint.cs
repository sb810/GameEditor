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
        public int enemyTotal;
        [HideInInspector] public int enemyCount;

        public int checkpointTotal;
        [HideInInspector] public int checkpointCount;

        [SerializeField] private CompletableCondition flagCondition;
        [SerializeField] private CompletableCondition playerCondition;
        [SerializeField] private CompletableCondition allyCondition;

        private bool hasWon;

        public void ResetCounters()
        {
            Start();
        }
        
        public void CollectCheckpoint()
        {
            checkpointCount--;
            flagCondition.UpdateConditionCount(checkpointTotal - checkpointCount, checkpointTotal);
            if(checkpointCount == 0) flagCondition.Check();
        }

        public void KillAlly()
        {
            allyCondition.MarkRed();
        }
        
        public void KillEnemy()
        {
            enemyCount--;
            playerCondition.UpdateConditionCount(enemyTotal - enemyCount, enemyTotal);
            if(enemyCount == 0) playerCondition.Check();
        }

        private void Start()
        {
            checkpointCount = checkpointTotal;
            if(flagCondition)
            {
                flagCondition.UpdateConditionCount(0, checkpointTotal);
                flagCondition.Uncheck();
            }
            
            enemyCount = enemyTotal;
            if(playerCondition)
            {
                playerCondition.UpdateConditionCount(0, enemyTotal);
                playerCondition.Uncheck();
            }
            
            if(allyCondition) allyCondition.Check();
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
