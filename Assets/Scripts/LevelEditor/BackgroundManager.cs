using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> backgroundList;
        public int CurrentBackgroundIndex { get; private set; }
    
        public void SetBackground(int index)
        {
            CurrentBackgroundIndex = index;
            for(int i = 0; i < backgroundList.Count; i++) {
                backgroundList[i].SetActive(i == index);
            }    
        }
    }
}
