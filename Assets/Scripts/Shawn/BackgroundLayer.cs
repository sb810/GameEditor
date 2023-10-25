using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float cameraPositionMultiplier = 1.0f;
    [SerializeField] private float windStrength;
    [Min(1)] [SerializeField] private int positiveInstances = 1;
    [Min(1)] [SerializeField] private int negativeInstances = 1;

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;

        for (int i = 1; i <= positiveInstances; i++)
            CreateInstance(i * 18);
        
        for (int i = 1; i <= negativeInstances; i++)
            CreateInstance(i * -18);


        //Instantiate(obj, transform);
    }

    private void CreateInstance(int positionX)
    {
        var obj = new GameObject { transform = { parent = transform } };
        var s = obj.AddComponent<SpriteRenderer>();
        var thisS = GetComponent<SpriteRenderer>();
        s.sprite = thisS.sprite;
        s.sortingLayerID = thisS.sortingLayerID;
        s.sortingOrder = thisS.sortingOrder;
        s.transform.localPosition =
            new Vector3(positionX, 0, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localPosition =
            new Vector3(windStrength * Time.time - cameraPositionMultiplier * mainCamera.transform.position.x % 18, 0, 0);
    }
}