using LevelEditor.Network;
using LevelEditor.Save;
using PlayerData;
using UnityEngine;

namespace LevelEditor
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public SaveManager SaveManager { get; private set; }
        public BuildingManager BuildingManager { get; private set; }
        public ObjectSelectionManager ObjectSelectionManager { get; private set; }
        public GameOverManager GameOverManager { get; private set; }
        public NetworkManager NetworkManager { get; private set; }
        public BackgroundManager BackgroundManager { get; private set; }

        public AdvanceTagManager AdvanceTagManager { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            SaveManager = GetComponent<SaveManager>();
            BuildingManager = GetComponent<BuildingManager>();
            ObjectSelectionManager = GetComponent<ObjectSelectionManager>();
            GameOverManager = GetComponent<GameOverManager>();
            NetworkManager = GetComponent<NetworkManager>();
            BackgroundManager = GetComponent<BackgroundManager>();
            AdvanceTagManager = GetComponent<AdvanceTagManager>();
        }
    }
}