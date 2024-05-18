using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceTagManager : MonoBehaviour
{


    public void ClearTag(string toClear){
        foreach(GameObject go in gameObject.scene.GetRootGameObjects()){
            AdvanceTag at=go.GetComponent<AdvanceTag>();
            if(at!=null)
            if(at.getTag()==toClear){
                LevelEditor.GameManager.Instance.BuildingManager.placedObject.Remove(at.gameObject);
                GameObject.DestroyImmediate(at.gameObject);
            }
        }
    }


}
