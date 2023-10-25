using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> backgroundList;
    
    public void SetBackground(int index) {
        for(int i = 0; i < backgroundList.Count; i++) {
            backgroundList[i].SetActive(i == index);
        }    
    }
}
