using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace CodingExercises
{
    public class BlockAssembly : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [FormerlySerializedAs("previousBloc")] public GameObject previousBlock;
        [FormerlySerializedAs("midBloc")] public GameObject midBlock;
        [FormerlySerializedAs("nextBloc")] public GameObject nextBlock;

        public bool canAssembleNext;
        public bool canAssemblePrevious;
        [FormerlySerializedAs("collidingBloc")] public GameObject collidingBlock;
        [FormerlySerializedAs("collidingNextBloc")] public GameObject collidingNextBlock;

        [FormerlySerializedAs("previousBlocPosition")] [HideInInspector]
        public GameObject previousBlockPosition;

        [FormerlySerializedAs("nextBlocPosition")]
        public GameObject nextBlockPosition;

        [FormerlySerializedAs("midBlocPosition")] 
        public GameObject midBlockPosition;

        [FormerlySerializedAs("lastOfThePendingBloc")] [HideInInspector] public bool lastOfThePendingBlock;

        [FormerlySerializedAs("collidingWithBlocEnd")] [HideInInspector] public bool collidingWithBlockEnd;

        [SerializeField] private GameObject highlightFX;
        [SerializeField] private GameObject highlightArrow;
    
        public GameObject bot;
        //private Vector3 botBasePos;
        //private Vector3 nextBlocPosBasePos;
        //private Vector3 midBaseBlocPosBasePos;

        public GameObject midBase;
        public float midBaseRescaleOffset;
        public float midBasePosRescaleOffset;
        public float nextPosRescaleOffset;
    
        private List<GameObject> codeList = new();
        private GameObject pendingObj;

        public enum BlocType
        {
            Default,
            If,
            Loop,
            Start
        }


        public BlocType type;

        private void Start()
        {
            /*switch (type)
            {
                case BlocType.If:
                    botBasePos = bot.transform.localPosition;
                    nextBlocPosBasePos = nextBlockPosition.transform.localPosition;
                    midBaseBlocPosBasePos = midBase.transform.localPosition;
                    break;
                case BlocType.Loop:
                    botBasePos = bot.transform.localPosition;
                    midBaseBlocPosBasePos = midBase.transform.localPosition;
                    break;
            }*/
        }

        public void SetExecutionHighlightActive(bool active)
        {
            highlightFX.SetActive(active);
            highlightArrow.SetActive(active);
        }
        
        public void SetHoverHighlightActive(bool active)
        {
            highlightFX.SetActive(active);
        }

        private void Update()
        {
            /*if (CodeBlockManager.pendingObj == gameObject || lastOfThePendingBlock)
            {
                previousBlockPosition.GetComponent<PreviousBlockPosition>().enabled = true;
                nextBlockPosition.GetComponent<NextBlockPosition>().enabled = true;
            }
            else if (type != BlocType.Start)
            {
                previousBlockPosition.GetComponent<PreviousBlockPosition>().enabled = false;
                nextBlockPosition.GetComponent<NextBlockPosition>().enabled = false;
            }*/

            /*
            if (previousBlock != null)
            {
                transform.position = previousBlock.GetComponent<BlockAssembly>().nextBlockPosition.transform.position;

                if (previousBlock.GetComponent<BlockAssembly>().type == BlocType.If &&
                    previousBlock.GetComponent<BlockAssembly>().midBlock != null)
                {
                    if (previousBlock.GetComponent<BlockAssembly>().midBlock == gameObject)
                    {
                        transform.position = previousBlock.GetComponent<BlockAssembly>().midBlockPosition.transform.position;
                    }
                }

                previousBlockPosition.SetActive(false);
            }
            else if (type != BlocType.Start)
            {
                previousBlockPosition.SetActive(true);
            }*/

            // nextBlockPosition.SetActive(nextBlock == null);
            
            /*
            if (type == BlocType.If)
            {
                //resize
                if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse0))
                {
                    float nbOfMidBloc = GetNumberOfMidBloc();
                    bot.transform.localPosition = botBasePos - new Vector3(0, nextPosRescaleOffset * nbOfMidBloc, 0);
                    nextBlockPosition.transform.localPosition =
                        nextBlocPosBasePos - new Vector3(0, nextPosRescaleOffset * nbOfMidBloc, 0);

                    midBase.transform.localScale = new Vector3(1, (nbOfMidBloc + 1) * midBaseRescaleOffset, 1);
                }

                midBlockPosition.SetActive(midBlock == null);
            }

            if (type == BlocType.Loop)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse0))
                {
                    float nbOfMidBloc = GetNumberOfMidBloc();
                    bot.transform.localPosition = botBasePos - new Vector3(0, nextPosRescaleOffset * nbOfMidBloc, 0);
                    midBase.transform.localScale = new Vector3(1, (nbOfMidBloc + 1) * midBaseRescaleOffset, 1);
                }
            }*/
        }

        private int GetNumberOfMidBloc()
        {
            if (type == BlocType.Loop)
                midBlock = nextBlock;


            if (midBlock != null)
            {
                codeList.Clear();
                pendingObj = gameObject;
                ReadBlocIf();


                int objCount = 0;
                foreach (GameObject obj in codeList)
                {
                    if (obj.GetComponent<BlockAssembly>().type == BlocType.If ||
                        obj.GetComponent<BlockAssembly>().type == BlocType.Loop) objCount += 3;
                    else objCount++;
                }

                return objCount;
            }

            return 0;
        }

        private void ReadBlocIf()
        {
            if (pendingObj.GetComponent<BlockAssembly>().midBlock != null)
            {
                pendingObj = pendingObj.GetComponent<BlockAssembly>().midBlock;
                while (pendingObj.GetComponent<BlockAssembly>().nextBlock != null ||
                       pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                {
                    codeList.Add(pendingObj);

                    if (pendingObj.GetComponent<BlockAssembly>().type == BlocType.If)
                    {
                        GameObject saveIf = pendingObj;
                        ReadBlocIf();
                        pendingObj = saveIf;
                    }

                    if (pendingObj.GetComponent<BlockAssembly>().nextBlock == null &&
                        pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                        return;
                    if (pendingObj.GetComponent<BlockAssembly>().nextBlock != null)
                        pendingObj = pendingObj.GetComponent<BlockAssembly>().nextBlock;
                }

                codeList.Add(pendingObj);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (type == BlocType.Start || CodeBlockManager.hasPendingBlock) return;
            CodeBlockManager.shouldFlowback = true;
            CodeBlockManager.SelectBlock(gameObject);
            //MouseIconHandler.Instance.SetCursorHandHold();
            //MouseIconHandler.Instance.SetCursorLock(true);
        }
    
        public void OnPointerUp(PointerEventData eventData)
        {
            //MouseIconHandler.Instance.SetCursorHandHold();
            //MouseIconHandler.Instance.SetCursorLock(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.reentered) eventData.fullyExited = true;
            if (!eventData.fullyExited) return;
            if (type == BlocType.Start || CodeBlockManager.hasPendingBlock) return;
            highlightFX.SetActive(true);
            MouseIconHandler.Instance.SetCursorHandOpenQueued();
            eventData.fullyExited = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //if (!eventData.fullyExited) return;
            if (type == BlocType.Start || CodeBlockManager.hasPendingBlock) return;
            highlightFX.SetActive(false);
            MouseIconHandler.Instance.SetCursorDefaultQueued();
            //eventData.fullyExited = false;
        }

    
    }
}