using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public GameObject levelEditorUI;
    public GameObject inGameUI;
    public Scrollbar map;
    Vector3 pos;
    [HideInInspector] public GameObject pendingObj;
    private SpriteRenderer pendingObjSpriteRenderer;
    [SerializeField] private Material[] materials;

    public float gridSize;
    public bool gridOn = true;

    public bool canPlace = true;

    public List<GameObject> placedObject = new List<GameObject>();
    [HideInInspector] public List<GameObject> objectToDestroy = new List<GameObject>();

    private Camera cam;

    public float camLimit;

    private bool mouseOnTrash = false;
    private bool firstPlacement = false;

    [HideInInspector] public SaveLoadLevel saveLvl;

    SelectObject selectObj;

    [HideInInspector] public Vector3 decalage;

    public Sprite trashIn;
    public Sprite trashOut;

    public GameObject explosionFX;
    
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        saveLvl = GetComponent<SaveLoadLevel>();
        saveLvl.LoadData("Level");
        saveZID.Clear();
        selectObj = GetComponent<SelectObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            LoadZ();
        }

        if (pendingObj == null) return;

        pendingObj.transform.position = gridOn
            ? new Vector3(Snapping.Snap(pos.x, gridSize), Snapping.Snap(pos.y, gridSize), 0)
            : pos;

        UpdateMaterials();

        if (Input.GetMouseButtonUp(0))
        {
            if (canPlace && !mouseOnTrash) PlaceObject();
            else
            {
                firstPlacement = false;
                Delete();
            }
        }
        
        Debug.Log(pendingObj);
    }

    void PlaceObject()
    {
        if (!mouseOnTrash)
        {
            pendingObjSpriteRenderer.material = materials[2];
            if (firstPlacement)
            {
                placedObject.Add(pendingObj);
                firstPlacement = false;
            }

            pendingObj = null;
        }
    }

    private void FixedUpdate()
    {
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        pos = new Vector3(mousePosition.x, mousePosition.y, 0) - decalage;
    }

    public void SelectObject(PrefabInButton script)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) return;

        GameObject objectToSpawn = script.prefab;

        if (IsPrefabLimitExceeded(objectToSpawn)) return;

        SaveZ();
        pendingObj = Instantiate(objectToSpawn, pos, transform.rotation);
        pendingObjSpriteRenderer = pendingObj.GetComponent<SpriteRenderer>();
        pendingObjSpriteRenderer.sortingLayerName = "Topmost";
        firstPlacement = true;
        selectObj.selectedObj = pendingObj;
    }

    void UpdateMaterials()
    {
        if (!pendingObj) return;
        if (!pendingObjSpriteRenderer) pendingObjSpriteRenderer = pendingObj.GetComponent<SpriteRenderer>();
        pendingObjSpriteRenderer.material = canPlace && !mouseOnTrash
            ? materials[0]
            : materials[1];
    }

    public void Play()
    {
        saveLvl.SaveData("Level");
        levelEditorUI.SetActive(false);
        inGameUI.SetActive(true);

        foreach (GameObject obj in placedObject)
        {
            if (obj == null) continue;

            obj.GetComponent<SpriteRenderer>().enabled = false;
            obj.GetComponent<Collider2D>().enabled = false;

            int startChildCount = obj.transform.childCount;
            for (int i = 2; i < startChildCount; i++)
                obj.transform.GetChild(2).parent = obj.transform.GetChild(1);

            obj.transform.GetChild(0).gameObject.SetActive(false);
            obj.transform.GetChild(1).gameObject.SetActive(true);
        }

        cam.GetComponent<CameraController>().enabled = true;
        cam.GetComponent<CameraController>().camLimit = camLimit;
        cam.GetComponent<CameraController>().FindPlayer(cam.GetComponent<CameraController>().faceLeft);

        saveZID.Clear();
    }

    public void Stop()
    {
        Debug.Log("Stopped from Building Manager!");

        levelEditorUI.SetActive(true);
        saveLvl.LoadData("Level");

        foreach (GameObject obj in objectToDestroy)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        objectToDestroy.Clear();

        inGameUI.SetActive(false);
        cam.GetComponent<CameraController>().enabled = false;

        cam.transform.position = new Vector3(0, 0, -10);
    }

    private void Delete()
    {
        Vector3 camPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Instantiate(explosionFX, new Vector3(camPos.x, camPos.y, 0), Quaternion.identity, levelEditorUI.transform);
        
        if (!firstPlacement)
        {
            int index = placedObject.IndexOf(pendingObj);
            if (index < 0) return;
            placedObject.RemoveAt(index);
        }
        
        Destroy(pendingObj);
        pendingObj = null;
        selectObj.selectedObj = null;
    }

    public void MouseEnterTrash(GameObject obj)
    {
        mouseOnTrash = true;
        if (selectObj.selectedObj != null)
            obj.SetActive(true);
    }

    public void MouseEnterTrashImage(GameObject obj)
    {
        mouseOnTrash = true;
        if (selectObj.selectedObj != null)
            obj.GetComponent<Image>().sprite = trashIn;
    }

    public void MouseExitTrash(GameObject obj)
    {
        mouseOnTrash = false;
        obj.SetActive(false);
    }

    public void MouseExitTrashImage(GameObject obj)
    {
        mouseOnTrash = false;
        if (selectObj.selectedObj != null)
            obj.GetComponent<Image>().sprite = trashOut;
    }


    private bool IsPrefabLimitExceeded(GameObject prefab)
    {
        int limit = prefab.GetComponent<CheckPlacement>().nbLimit;
        if (limit == 0) return false;
        
        int actualNb = 0;
        foreach (GameObject obj in placedObject)
        {
            if (obj.name.Replace("(Clone)", string.Empty) == prefab.name)
            {
                actualNb++;
            }
        }

        return actualNb >= limit;
    }

    public void MoveMap()
    {
        cam.transform.position = new Vector3(camLimit * map.value, 0, -10);
    }

    public void Reset()
    {
        saveLvl.ClearData();
    }

    // ctrlZ
    List<string> saveZID = new List<string>();

    public void SaveZ()
    {
        saveLvl.SaveData("Z" + saveZID.Count);
        saveZID.Add("Z" + saveZID.Count);
    }

    private void LoadZ()
    {
        if (saveZID.Count != 0)
        {
            saveLvl.LoadData(saveZID[^1]);
            saveZID.RemoveAt(saveZID.Count - 1);
        }
    }

    public void ClearZ()
    {
        foreach (string save in saveZID)
        {
            PlayerPrefs.DeleteKey(save);
        }

        saveZID.Clear();
    }
}