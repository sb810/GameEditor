using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPlacement : MonoBehaviour
{
    private BuildingManager buildManager;
    public int nbLimit;

    private void Start()
    {
        buildManager = GameManager.Instance.BuildingManager;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Placement"))
        {
            buildManager.canPlace = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Placement"))
        {
            buildManager.canPlace = false;
        }
    }
}
