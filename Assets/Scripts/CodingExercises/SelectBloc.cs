using UnityEngine;

namespace CodingExercises
{
    public class SelectBloc : MonoBehaviour
    {
        public GameObject selectedObj;
        private CodeBlockManager codeBlockManager;

        private void Start()
        {
            codeBlockManager = GetComponent<CodeBlockManager>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && CodeBlockManager.pendingObj == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.CompareTag("Bloc") && hit.collider.GetComponent<BlockAssembly>().type != BlockAssembly.BlocType.Start)
                    {
                        Select(hit.collider.gameObject);
                    }
                    else
                    {
                        if (selectedObj != null) Deselect();
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
            if (obj == selectedObj) return;

            if (selectedObj != null) Deselect();

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            codeBlockManager.offset = new Vector3(mousePosition.x, mousePosition.y, 0) - obj.transform.position;

            selectedObj = obj;


            if(selectedObj.GetComponent<BlockAssembly>().previousBlock != null)
            {
                if (selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.Start)
                {
                    selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().midBlock = null;
                    selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().nextBlock = null;
                }

                if (selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.Loop)
                {
                    selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().midBlock = null;
                    selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().nextBlock = null;
                }

                if (selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().midBlock == selectedObj)
                {
                    selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().midBlock = null;
                }
                else
                {
                    selectedObj.GetComponent<BlockAssembly>().previousBlock.GetComponent<BlockAssembly>().nextBlock = null;
                }
            
                selectedObj.GetComponent<BlockAssembly>().previousBlock = null;
            }


            GetLastObjOfbloc().GetComponent<BlockAssembly>().lastOfThePendingBlock = true;

            Move();
        }

        private void Deselect()
        {
            GetLastObjOfbloc().GetComponent<BlockAssembly>().lastOfThePendingBlock = false;
            selectedObj = null;
        }

        public void Delete()
        {
            GameObject objToDestroy = selectedObj;
            Deselect();
            Destroy(objToDestroy);
        }
        public void Move()
        {
            CodeBlockManager.pendingObj = selectedObj;
        }

        private GameObject GetLastObjOfbloc()
        {
            if (selectedObj.GetComponent<BlockAssembly>().nextBlock != null)
            {
                GameObject lastObj = selectedObj;
                while (lastObj.GetComponent<BlockAssembly>().nextBlock != null)
                {
                    lastObj = lastObj.GetComponent<BlockAssembly>().nextBlock;
                }
                return lastObj;
            }
            else
            {
                return selectedObj;
            }
        }
    }
}
