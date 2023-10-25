using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    private BuildingManager buildingManager;
    
    public GameObject gameOverMenu;
    public GameObject inGameUI;

    [HideInInspector] public GameObject player;
    private void Start()
    {
        buildingManager = GameManager.Instance.BuildingManager;
    }

    public void Editor()
    {
        buildingManager.Stop();
        gameOverMenu.SetActive(false);
    }

    public void Checkpoint()
    {
        inGameUI.SetActive(true);
        player.SetActive(true);
        player.GetComponent<Player>().TpToLastCheckpoint();
        gameOverMenu.SetActive(false);
    }

    public void StartFromBeginning()
    {
        buildingManager.Stop();
        buildingManager.Play();
        gameOverMenu.SetActive(false);
    }
}
