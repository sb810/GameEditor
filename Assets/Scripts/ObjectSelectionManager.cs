using UnityEngine;

public class ObjectSelectionManager : MonoBehaviour
{
    public GameObject selectedObj;
    private BuildingManager buildManager;

    private void Start()
    {
        buildManager = GameManager.Instance.BuildingManager;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && buildManager.pendingObj == null)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
            if (hit.collider != null)
            {
                if(hit.collider.gameObject.CompareTag("Placement"))
                {
                    buildManager.SaveZ();
                    Select(hit.collider.gameObject);
                }
            }
            else
            {
                if (selectedObj != null) Deselect();
            }
        }
        if (Input.GetMouseButtonUp(0) && selectedObj != null)
        {
            Deselect();
        }
    }

    private void Select(GameObject obj)
    {
        if (selectedObj != null) Deselect();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        buildManager.decalage = new Vector3(mousePosition.x, mousePosition.y, 0) -obj.transform.position;

        selectedObj = obj;
        buildManager.pendingObj = selectedObj;
        buildManager.pendingObjSpriteRenderer = selectedObj.GetComponent<SpriteRenderer>();
    }

    private void Deselect()
    {
        selectedObj = null;
    }

    public void Delete()
    {
        GameObject objToDestroy = selectedObj;
        Deselect();
        Destroy(objToDestroy);
    }
}
