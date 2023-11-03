using System;
using System.Collections;
using System.Collections.Generic;
using LevelEditor.Network;
using PlayerData;
using UnityEngine;

namespace LevelEditor.Save
{
    public class SaveManager : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> prefabByName = new();
        public List<GameObject> prefabsList = new();

        //private string testFolder = "Save";
        // private readonly string testFile = "Level";

        private List<Vector3> myPositions = new();
        private List<Quaternion> myRotations = new();
        private List<Vector3> myScales = new();
        private List<string> myPrefabs = new();
        private List<int> myIndexes = new();

        private BuildingManager manager;
        private NetworkManager network;

        private void Awake()
        {
            manager = GameManager.Instance.BuildingManager;
            network = GameManager.Instance.NetworkManager;
            InitializePrefabsByName();
        }

        private void Start()
        {
            //manager = GameManager.Instance.BuildingManager;
            //network = GameManager.Instance.NetworkManager;
            StartCoroutine(!string.IsNullOrEmpty(PlayerDataManager.Data.id) ? ReloadBeforeSaving() : AutoSave());
        }

        private IEnumerator ReloadBeforeSaving()
        {
            yield return network.StartCoroutine(network.SendWebRequest("GET"));
            ReloadLevelData();
            StartCoroutine(AutoSave());
        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator AutoSave()
        {
            SaveDataToNetwork();
            yield return new WaitForSeconds(30);
            StartCoroutine(AutoSave());
            yield return null;
        }
        public void SaveDataToNetwork()
        {
            myPositions.Clear();
            myRotations.Clear();
            myScales.Clear();
            myPrefabs.Clear();
            myIndexes.Clear();

            int newIndex = 0;
            foreach (GameObject obj in manager.placedObject)
            {
                myPositions.Add(obj.transform.position);
                myRotations.Add(obj.transform.rotation);
                myScales.Add(obj.transform.localScale);
                myPrefabs.Add(obj.name.Replace("(Clone)", string.Empty));
                myIndexes.Add(newIndex);
                newIndex++;
            }

            PlayerDataManager.Data.levelData = new SavedLevel
            {
                positions = myPositions,
                prefabs = myPrefabs,
                rotations = myRotations,
                scales = myScales,
                indexes = myIndexes,
                backgroundIndex = GameManager.Instance.BackgroundManager.CurrentBackgroundIndex
            };
            
            Debug.Log("Saving... Data is " + PlayerDataManager.Data.levelData);

            if (!string.IsNullOrEmpty(PlayerDataManager.Data.id))
                network.UpdateNetworkSavedData();
            else
                network.UploadNewSaveData();
        }

        public void ReloadLevelData()
        {
            
            
            foreach (GameObject obj in manager.placedObject)
                Destroy(obj);
            manager.placedObject.Clear();
            
            SavedLevel loadedData = PlayerDataManager.Data.levelData;
            
            Debug.Log("Reloading... Data is " + PlayerDataManager.Data.levelData);

            myPositions = loadedData.positions;
            myRotations = loadedData.rotations;
            myScales = loadedData.scales;
            myPrefabs = loadedData.prefabs;
            myIndexes = loadedData.indexes;
        
            GameManager.Instance.BackgroundManager.SetBackground(loadedData.backgroundIndex);

            foreach (int i in myIndexes)
            {
                GameObject myObject = Instantiate(GetPrefabByName(myPrefabs[i]));
                manager.placedObject.Add(myObject);
                myObject.transform.position = myPositions[i];
                myObject.transform.rotation = myRotations[i];
                myObject.transform.localScale = myScales[i];
            }
            
            if (manager.placedObject.Count == 0)
                InstantiateDefaultLevel();
        }

        private void InstantiateDefaultLevel()
        {
            Debug.Log("Instantiating default level !");
            
            GameObject player = Instantiate(GetPrefabByName("Player"));
            manager.placedObject.Add(player);
            player.transform.position = new Vector3(-7.75f, -4, 0);

            GameObject finish = Instantiate(GetPrefabByName("Finish"));
            manager.placedObject.Add(finish);
            finish.transform.position = new Vector3(7.75f, -4f, 0);

            for (int i = 0; i < 6; i++)
            {
                GameObject platform = Instantiate(GetPrefabByName("PlatformForestL"));
                manager.placedObject.Add(platform);
                platform.transform.position = new Vector3(-7.5f + 3 * i, -5.5f, 0);
            }
        }

        public void ClearData()
        {
            PlayerDataManager.Data.levelData = new SavedLevel();
            ReloadLevelData();
        }

        private void InitializePrefabsByName()
        {
            foreach (GameObject prefab in prefabsList)
                prefabByName.TryAdd(prefab.name, prefab);
        }

        private GameObject GetPrefabByName(string prefabName)
        {
            if (prefabByName.TryGetValue(prefabName, out var prefab))
                return prefab;

            Debug.LogWarning("No prefab associated to name \"" + prefabName + "\" !");
            return null;
        }
    }
}