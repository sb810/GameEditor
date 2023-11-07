using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LevelEditor
{
    public class CheckPlacementAction : MonoBehaviour
    {
        [FormerlySerializedAs("checkedObject")] [SerializeField] private CheckPlacement[] checkedObjects;
        [FormerlySerializedAs("checkedPrefabInButton")] [SerializeField] private PrefabInButton[] checkedPrefabInButtons;
        [SerializeField] private UnityEvent actionOnMaxReached;
        [SerializeField] private UnityEvent actionOnNeitherMinNorMax;
        [SerializeField] private UnityEvent actionOnMinReached;

        private readonly List<GameObject> objects = new();

        private void Start()
        {
            if(checkedObjects.Length > 0)
                objects.AddRange(Array.ConvertAll(checkedObjects, input => input.gameObject));
            if(checkedPrefabInButtons.Length > 0)
                objects.AddRange(Array.ConvertAll(checkedPrefabInButtons, input => input.gameObject));
        }

        public void Evaluate()
        {
            if (objects.Count == 0) return;

            int numberMin = 0;
            int numberMax = 0;
            
            Debug.Log("Evaluating " + objects.Count + " objects.");
            
            foreach (var checkedObject in objects)
            {
                if (!checkedObject.TryGetComponent(out CheckPlacement placement))
                    placement = checkedObject.GetComponent<PrefabInButton>().prefab.GetComponent<CheckPlacement>();
                
                Debug.Log("Placement : " + placement);
                
                int distance = GameManager.Instance.BuildingManager.GetPrefabDistanceFromLimit(placement.gameObject);
                if (distance == placement.nbLimit)
                    numberMin++;
                else if (distance == 0)
                    numberMax++;
            }
            
            Debug.Log("Max : " + numberMax + ", min : " + numberMin);

            if (numberMax == objects.Count)
                actionOnMaxReached.Invoke();
            else if(numberMin == objects.Count)
                actionOnMinReached.Invoke();
            else actionOnNeitherMinNorMax.Invoke();
        }
    }
}
