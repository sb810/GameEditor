using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class SelectedButtonGraphicsHandler : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject prefabToCompare;
    [SerializeField] private PrefabInButton prefabToObserve;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite defaultSprite;
    private GameObject prefabHolder;
    
    // Start is called before the first frame update
    void Start()
    {
        prefabHolder = prefabToObserve.gameObject;
    }

    public void Evaluate()
    {
        image.sprite = prefabHolder.GetComponent<PrefabInButton>().prefab == prefabToCompare 
            ? activeSprite 
            : defaultSprite;
    }
}
