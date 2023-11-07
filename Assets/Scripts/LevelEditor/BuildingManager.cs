using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LevelEditor.InGame;
using LevelEditor.Save;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LevelEditor
{
    public class BuildingManager : MonoBehaviour
    {
        [SerializeField] private CheckPlacementAction[] postPlacementActions;
        
        public GameObject levelEditorUI;
        public GameObject inGameUI;
        public Scrollbar map;
        private Vector3 pos;
        [HideInInspector] public GameObject pendingObj;
        public SpriteRenderer pendingObjSpriteRenderer;
        [SerializeField] private Material[] materials;

        public float gridSize;
        public bool gridOn = true;

        public bool canPlace = true;

        public List<GameObject> placedObject = new();
        [HideInInspector] public List<GameObject> objectToDestroy = new();

        private Camera cam;

        public float camLimit;

        private bool mouseOnTrash;
        private bool firstPlacement;

        [HideInInspector] public SaveManager saveLvl;

        private ObjectSelectionManager selectionManager;

        [FormerlySerializedAs("decalage")] [HideInInspector] public Vector3 offset;

        [SerializeField] private Sprite trashIn;
        [SerializeField] private Sprite trashOut;

        [SerializeField] private GameObject explosionFX;
    
        // private GameObject visualHeldItem;
        // private SpriteRenderer visualHeldItemSpriteRenderer;

        private void Start()
        {
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            saveLvl = GetComponent<SaveManager>();
            saveLvl.ReloadLevelData();
            // saveZID.Clear();
            selectionManager = GameManager.Instance.ObjectSelectionManager;
            // visualHeldItem = new();
            //visualHeldItemSpriteRenderer = visualHeldItem.AddComponent<SpriteRenderer>();
            //visualHeldItemSpriteRenderer.sortingLayerName = "Topmost";
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.B))
            //{
            //    LoadZ();
            //}

            if (pendingObj == null) return;

            pendingObj.transform.position = gridOn
                ? new Vector3(Snapping.Snap(pos.x, gridSize), Snapping.Snap(pos.y, gridSize), 0)
                : pos;

            // visualHeldItem.transform.position = pos;

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

        private void PlaceObject()
        {
            if (!mouseOnTrash)
            {
                pendingObjSpriteRenderer.material = materials[2];
                pendingObjSpriteRenderer.sortingLayerName = "Foreground";
                if (firstPlacement)
                {
                    placedObject.Add(pendingObj);
                    firstPlacement = false;
                }

                //visualHeldItemSpriteRenderer.sprite = null;
                pendingObj = null;
            }

            foreach (var action in postPlacementActions)
                action.Evaluate();
        }

        private void FixedUpdate()
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            pos = new Vector3(mousePosition.x, mousePosition.y, 0) - offset;
        }

        public void SelectObject(PrefabInButton script)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) || GetPrefabDistanceFromLimit(script.prefab) == 0) return;
            GameObject objectToSpawn = script.prefab;

            // SaveZ();
            pendingObj = Instantiate(objectToSpawn, pos, transform.rotation);
            pendingObjSpriteRenderer = pendingObj.GetComponent<SpriteRenderer>();
            pendingObjSpriteRenderer.sortingLayerName = "Topmost";

            // visualHeldItemSpriteRenderer.sprite = pendingObjSpriteRenderer.sprite;
            // visualHeldItemSpriteRenderer.sortingOrder += 1;

            // UpdatePlacementButton(script);

            canPlace = true;
            firstPlacement = true;
            selectionManager.selectedObj = pendingObj;
        }

        private void UpdateMaterials()
        {
            if (!pendingObj) return;
            if(canPlace) Debug.Log("Can place !");
            if(mouseOnTrash) Debug.Log("Mouse on trash !");
            if (!pendingObjSpriteRenderer) pendingObjSpriteRenderer = pendingObj.GetComponent<SpriteRenderer>();
            pendingObjSpriteRenderer.material = canPlace && !mouseOnTrash
                ? materials[0] // CanPlace - Green
                : materials[1]; // CantPlace - Red
            // visualHeldItemSpriteRenderer.material = canPlace && !mouseOnTrash
            // ? materials[2] // Normal - White 
            // : materials[1]; // CantPlace - Red
        }

        public void PlayDelayed(float delay)
        {
            StartCoroutine(Play(delay));
        }

        public void Play()
        {
            StartCoroutine(Play(0));
        }

        public IEnumerator Play(float delay)
        {
            yield return new WaitForSeconds(delay);
        
            saveLvl.SaveDataToNetwork();
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

            // saveZID.Clear();

            yield return null;
        }

        private IEnumerator Stop(float delay)
        {
            yield return new WaitForSeconds(delay);
        
            levelEditorUI.SetActive(true);
            saveLvl.ReloadLevelData();

            foreach (var toDestroy in objectToDestroy.Where(toDestroy => toDestroy != null))
                Destroy(toDestroy);

            objectToDestroy.Clear();

            inGameUI.SetActive(false);
            cam.GetComponent<CameraController>().enabled = false;

            cam.transform.position = new Vector3(0, 0, -10);

            yield return null;
        }

        public void Stop()
        {
            StartCoroutine(Stop(0));
        }
    
        public void StopDelayed(float delay)
        {
            StartCoroutine(Stop(delay));
        }

        private void Delete()
        {
            Vector3 camPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(explosionFX, new Vector3(camPos.x, camPos.y, 0), Quaternion.identity, levelEditorUI.transform);
        
            if (!firstPlacement)
            {
                int index = placedObject.IndexOf(pendingObj);
                if (index >= 0) placedObject.RemoveAt(index);
            }
        
            Destroy(pendingObj);
            pendingObj = null;
            selectionManager.selectedObj = null;
            
            foreach (var action in postPlacementActions)
                action.Evaluate();
        }

        public void MouseEnterTrash(GameObject go)
        {
            mouseOnTrash = true;
            if (selectionManager.selectedObj != null)
                go.SetActive(true);
        }

        public void MouseEnterTrashImage(GameObject go)
        {
            mouseOnTrash = true;
            if (selectionManager.selectedObj != null)
                go.GetComponent<Image>().sprite = trashIn;
        }

        public void MouseExitTrash(GameObject go)
        {
            mouseOnTrash = false;
            go.SetActive(false);
        }

        public void MouseExitTrashImage(GameObject go)
        {
            mouseOnTrash = false;
            if (selectionManager.selectedObj != null)
                go.GetComponent<Image>().sprite = trashOut;
        }


        public int GetPrefabDistanceFromLimit(GameObject prefab)
        {
            int limit = prefab.GetComponent<CheckPlacement>().nbLimit;

            if (limit == 0) return -1;
        
            int actualNb = 0;
            foreach (GameObject obj in placedObject)
            {
                if (obj.name.Replace("(Clone)", string.Empty) == prefab.name)
                    actualNb++;
            }

            return limit - actualNb;
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
        //private List<string> saveZID = new List<string>();

        /*public void SaveZ()
        {
            saveLvl.SaveDataToNetwork("Z" + saveZID.Count);
            saveZID.Add("Z" + saveZID.Count);
        }

        private void LoadZ()
        {
            if (saveZID.Count != 0)
            {
                saveLvl.ReloadLevelData(saveZID[^1]);
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
        }*/
    }
}