using System;
using System.Collections.Generic;
using JetBrains.Annotations;
// using Sirenix.OdinInspector;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace CodingExercises
{
    public class CodeBlockManager : MonoBehaviour
    {
        // [AssetsOnly] 
        [SerializeField] private GameObject explosionFX;

        [SerializeField] private GameObject blocksList;

        private Vector3 pos;
        public Vector3 posOffset;

        [FormerlySerializedAs("decalage")] [HideInInspector]
        public Vector3 offset;

        public static GameObject pendingObj;

        [FormerlySerializedAs("blocRestant")] [SerializeField]
        private TMP_Text remainingBlocks;

        public int maxBlocNb;
        public int blocNb = 0;

        private bool mouseOnTrash = false;
        private Camera mainCamera;

        public GameObject startBlock;
        private static Transform staticTransform;

        private static CodeZoneBlocksManager codeZoneBlocksManager;
        private static CodeZoneData codeZoneData;

        public static bool shouldFlowback;
        public static bool hasPendingBlock;
        private bool shouldApplyBlockReflow;

        [SerializeField] private Material canPlaceMaterial;
        [SerializeField] private Material cantPlaceMaterial;

        private void Awake()
        {
            mainCamera = Camera.main;
            staticTransform = transform;
            codeZoneBlocksManager = startBlock.GetComponentInParent<CodeZoneBlocksManager>();
            UpdateRemainingBlocksCount(remainingBlocks.text);
        }

        private void FixedUpdate()
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos = new Vector3(mousePosition.x, mousePosition.y, 0) - offset;
        }

        public void UpdateRemainingBlocksCount(string label)
        {
            remainingBlocks.text = label.Substring(0, label.Length - 1) + (maxBlocNb - blocNb);
        }

        private bool isBlockMappingInvalid()
        {
            
            int hovered = codeZoneData.HoveredPositionIndex;
            BlockAssembly[] mappings = codeZoneData.indexedBlockPositionMappings;

            //if (hoveredPositionIndex > mappings.Length - 1) hoveredPositionIndex = mappings.Length - 1;

            return mouseOnTrash
                   || hovered < 1;
            // || (hovered > 0 && mappings[hovered - 1] == null) ;
        }
        
        private void Update()
        {
            if (pendingObj == null) return;

            codeZoneData = codeZoneBlocksManager.GetCodeZoneData();
            //int hoveredPositionIndex = codeZoneData.HoveredPositionIndex;
            //BlockAssembly[] mappings = codeZoneData.indexedBlockPositionMappings;
            // int totalIndexes = codeZoneData.TotalPositionIndexes;
            //Debug.Log("POSITION INDEX : " + codeZoneData.HoveredPositionIndex);

            if (shouldFlowback)
            {
                FlowbackBlocks();
                shouldFlowback = false;
            }

            ReflowBlocks();
            HighlightNextPosition(codeZoneData.HoveredPositionIndex);

            pendingObj.transform.position = pos + posOffset;

            pendingObj.GetComponent<Image>().material = isBlockMappingInvalid() ? cantPlaceMaterial : canPlaceMaterial;

            if (Input.GetMouseButtonUp(0))
            {
                if (isBlockMappingInvalid()) Delete();
                else PlaceBlock();
                codeZoneBlocksManager.RecalculateData();
                codeZoneData = codeZoneBlocksManager.GetCodeZoneData();
                PostBlockReflow();
            }

            UpdateRemainingBlocksCount(remainingBlocks.text);
        }

        private static int GetBlockSize(BlockAssembly block)
        {
            if (block.type is BlockAssembly.BlocType.Default or BlockAssembly.BlocType.Start)
                return 1;

            int size = 3;
            foreach (var child in block.GetComponentsInChildren<BlockAssembly>())
            {
                if (child == block) continue;
                size++;
                if (child.type is BlockAssembly.BlocType.If or BlockAssembly.BlocType.Loop)
                    size += GetBlockSize(child);
            }

            return size;
        }

        private void HighlightNextPosition(int highlightIndex)
        {
            BlockAssembly[] mapping = codeZoneData.indexedBlockPositionMappings;
            mapping ??= startBlock.transform.parent.GetComponentsInChildren<BlockAssembly>();
            
            

            for (var i = 0; i < mapping.Length; i++)
            {
                if (mapping[i]?.type is BlockAssembly.BlocType.If or BlockAssembly.BlocType.Loop)
                {
                    bool isFirst = true;
                    for (var j = i - 1; j >= 0; j--)
                        if (mapping[j] == mapping[i])
                            isFirst = false;

                    //if(!isFirst) Debug.Log("LAST FOUND : INDEX " + i);
                    //if(isFirst) Debug.Log("FIRST FOUND : INDEX " + i);

                    /*if (isFirst && highlightIndex - 1 == i)
                    {
                        Debug.Log("Midblock should highlight : INDEX " + i + ", Midblock : " + mapping[i].midBlockPosition);
                        mapping[i].midBlockPosition.SetActive(true);
                    }*/

                    if (isFirst)
                        mapping[i].midBlockPosition.SetActive(highlightIndex - 1 == i);
                    else mapping[i].nextBlockPosition.SetActive(highlightIndex - 1 == i);

                    //mapping[i].midBlockPosition.SetActive(isFirst && highlightIndex - 1 == i);
                    //mapping[i].nextBlockPosition.SetActive(!isFirst && highlightIndex - 1 == i); 
                }
                else mapping[i]?.nextBlockPosition.SetActive(highlightIndex - 1 == i);
            }
        }

        private void SnapBlock(int snapPositionIndex)
        {
            RectTransform rt = (RectTransform)pendingObj.transform;

            BlockAssembly[] mappings = codeZoneData.indexedBlockPositionMappings;
            BlockAssembly previous = mappings[snapPositionIndex - 1];
            BlockAssembly twoBefore = snapPositionIndex > 1 ? mappings[snapPositionIndex - 2] : null;
            RectTransform parent = null;
            bool isLoop = false;

            if (twoBefore != null && previous != null &&
                previous.type is BlockAssembly.BlocType.Loop or BlockAssembly.BlocType.If)
            {
                Debug.Log("BOTH AND LOOP");

                parent = (RectTransform)previous.transform;
                rt.parent = parent;
                isLoop = true;
                rt.anchoredPosition = new Vector2(17, -17);
                pendingObj.GetComponent<BlockAssembly>().midBase = previous.gameObject;
                pendingObj.transform.SetSiblingIndex(0);
            }
            else if (previous == null && twoBefore != null)
            {
                Debug.Log("TWO BEFORE BUT NOT PREVIOUS");

                BlockAssembly parentLoop = mappings[snapPositionIndex];
                //BlockAssembly lastOfLoop = parentLoop.midBlock.GetComponent<BlockAssembly>();
                //while (lastOfLoop.nextBlock != null)
                //    lastOfLoop = lastOfLoop.nextBlock.GetComponent<BlockAssembly>();
                // RectTransform lastof

                parent = (RectTransform)parentLoop.transform;
                rt.parent = parent;
                isLoop = true;
                rt.anchoredPosition = new Vector2(17, twoBefore == parentLoop 
                    ? -17 
                    : ((RectTransform)twoBefore.transform).anchoredPosition.y - 17 * GetBlockSize(twoBefore));
                pendingObj.GetComponent<BlockAssembly>().midBase = parent.gameObject;
                pendingObj.transform.SetSiblingIndex(twoBefore == parentLoop
                    ? 0
                    : twoBefore.transform.GetSiblingIndex() + 1);
            }
            else if (previous.midBase != null)
            {
                Debug.Log("MIDBASE");
                parent = (RectTransform)previous.midBase.transform;
                rt.parent = parent;
                isLoop = true;
                var positioner = (RectTransform)previous.transform;

                Vector2 positionerAnchoredPosition = positioner.anchoredPosition;
                rt.anchoredPosition = new Vector2(17, positionerAnchoredPosition.y - 17 * GetBlockSize(previous));
                pendingObj.GetComponent<BlockAssembly>().midBase = previous.midBase;
                pendingObj.transform.SetSiblingIndex(positioner.GetSiblingIndex() + 1);
            }
            else
            {
                Debug.Log("NORMAL");

                RectTransform st = (RectTransform)startBlock.transform;
                rt.parent = st.parent;
                rt.localPosition = st.localPosition + Vector3.down * (snapPositionIndex * 17);
                pendingObj.transform.SetSiblingIndex(previous.transform.GetSiblingIndex() + 1);
                rt.sizeDelta = new Vector2(st.sizeDelta.x, rt.sizeDelta.y);
            }

            if (isLoop)
            {
                Vector2 parentSizeDelta = parent.sizeDelta;
                rt.sizeDelta = new Vector2(parentSizeDelta.x - 17, rt.sizeDelta.y);
            }

            pendingObj.GetComponent<Image>().raycastTarget = true;
            HighlightNextPosition(-1);
        }

        private void FlowbackBlocks()
        {
            int hoveredBlockIndex = codeZoneData.HoveredPositionIndex;
            BlockAssembly[] allBlocks = codeZoneData.indexedBlockPositionMappings;
            int pendingBlockSize = GetBlockSize(pendingObj.GetComponent<BlockAssembly>());
            BlockAssembly parentLoop = null;
            for (var i = 1; i < allBlocks.Length; i++)
            {
                var block = allBlocks[i];
                if (block == null)
                {
                    i++;
                    if (allBlocks[i] == parentLoop) parentLoop = null;
                    continue;
                } // skip two; one for empty block, and one for end of loop/if.

                RectTransform rt = (RectTransform)block.transform;
                CachedTransform c = block.GetComponent<CachedTransform>();

                Vector2 position = c.AnchoredPosition;
                float positionAdjustment =
                    (hoveredBlockIndex > 0 && i >= hoveredBlockIndex) && parentLoop == null ? 17 * pendingBlockSize : 0;
                rt.anchoredPosition = new Vector2(position.x, position.y + positionAdjustment);
                
                if (block.type is BlockAssembly.BlocType.If or BlockAssembly.BlocType.Loop)
                {
                    if (i >= hoveredBlockIndex && parentLoop == null)
                        parentLoop = block;

                    Vector2 cSizeDelta = c.sizeDelta;
                    float heightAdjustment =
                        (hoveredBlockIndex > i && hoveredBlockIndex <= GetLastIndexOfDynamicBlock(block))
                            ? pendingBlockSize * -17
                            : 0;
                    rt.sizeDelta = new Vector2(cSizeDelta.x, cSizeDelta.y + heightAdjustment);
                }

                c.ApplyAll();

                //Debug.Log("BLOCK : " + block.type);
                //Debug.Log("PENDING SIZE : " + pendingBlockSize);
            }
        }

        private int GetLastIndexOfDynamicBlock(BlockAssembly block)
        {
            BlockAssembly[] allBlocks = codeZoneData.indexedBlockPositionMappings;
            bool firstFound = false;
            for (int i = 0; i < allBlocks.Length; i++)
            {
                if (allBlocks[i] == block)
                {
                    if (!firstFound) firstFound = true;
                    else return i;
                }
            }

            return -1;
        }

        private void ReflowBlocks()
        {
            int hoveredBlockIndex = codeZoneData.HoveredPositionIndex;
            BlockAssembly[] allBlocks = codeZoneData.indexedBlockPositionMappings;
            int pendingBlockSize = GetBlockSize(pendingObj.GetComponent<BlockAssembly>());
            BlockAssembly parentLoop = null;
            for (var i = 1; i < allBlocks.Length; i++)
            {
                var block = allBlocks[i];
                if (block == null)
                {
                    i++;
                    if (allBlocks[i] == parentLoop) parentLoop = null;
                    continue;
                } // skip two; one for empty block, and one for end of loop/if.

                RectTransform rt = (RectTransform)block.transform;
                CachedTransform c = block.GetComponent<CachedTransform>();

                Vector2 cPos = c.AnchoredPosition;
                float positionAdjustment = hoveredBlockIndex > 0 && i >= hoveredBlockIndex && parentLoop == null
                    ? -17 * pendingBlockSize
                    : 0;
                rt.anchoredPosition = new Vector2(cPos.x, cPos.y + positionAdjustment);

                if (block.type is BlockAssembly.BlocType.If or BlockAssembly.BlocType.Loop) // Resize dynamic blocks
                {
                    if (i >= hoveredBlockIndex && parentLoop == null)
                        parentLoop = block;

                    Vector2 cSizeDelta = c.sizeDelta;
                    float heightAdjustment =
                        (hoveredBlockIndex > i && hoveredBlockIndex <= GetLastIndexOfDynamicBlock(block))
                            ? pendingBlockSize * 17
                            : 0;
                    rt.sizeDelta = new Vector2(cSizeDelta.x, cSizeDelta.y + heightAdjustment);
                }

                Debug.Log("Index : " + hoveredBlockIndex + ", I : " + i + ", parentLoop : " + parentLoop);
            }


            //var blocks = startBlock.transform.parent.GetComponentsInChildren<BlockAssembly>();
            // Fixme this doesn't need to be recalculated once a frame, only on grab.

            /*int[] heights = new int[blocks.Length];
            for (var index = 0; index < blocks.Length; index++)
            {
                if (blocks[index].type is BlockAssembly.BlocType.Loop or BlockAssembly.BlocType.If)
                    heights[index] = 3 + blocks[index].GetComponentsInChildren<BlockAssembly>().Length;
                else heights[index] = 1;
            }*/

            //for (var index = 1; index < blocks.Length; index++)
            //{
            //int previousBlocksHeight = 

            /*int adjustedPositionIndex = index;
            if (index >= pendingPositionIndex && pendingPositionIndex > 0)
            {
                if (pendingObj.GetComponent<BlockAssembly>().type is BlockAssembly.BlocType.Loop or BlockAssembly.BlocType.If)
                {
                    adjustedPositionIndex = index + 2 + pendingObj.GetComponentsInChildren<BlockAssembly>().Length;
                } else adjustedPositionIndex = index + 1;
            }

            if(blocks[index-1].type is BlockAssembly.BlocType.Loop or BlockAssembly.BlocType.If)
                adjustedPositionIndex = adjustedPositionIndex + 1 + blocks[index-1].GetComponentsInChildren<BlockAssembly>().Length;

            blocks[index].transform.localPosition =
                startBlock.transform.localPosition + Vector3.down * (adjustedPositionIndex * 17); */
            //}
        }

        private void PostBlockReflow()
        {
            BlockAssembly[] mappings = codeZoneData.indexedBlockPositionMappings;
            foreach (var block in mappings)
            {
                if (block == null) continue;
                block.nextBlock = null;
                block.midBlock = null;
                block.midBase = null;
                block.GetComponent<Image>().raycastTarget = GetBlockSize(block) <= 3;
                if (!block.gameObject.TryGetComponent(out CachedTransform c))
                    c = block.gameObject.AddComponent<CachedTransform>();
                c.ApplyAll();
            }

            for (var i = 0; i < mappings.Length - 1 && i >= 0; i++)
            {
                if (mappings[i] == null) continue;
                
                if (i >= 2
                    && mappings[i - 1] != null
                    && mappings[i - 1].type is BlockAssembly.BlocType.Loop
                        or BlockAssembly.BlocType.If
                    && mappings[i - 1].midBase != null
                    && mappings[i - 2] == null)
                {
                    mappings[i].midBase = mappings[i - 1].midBase;
                }

                // Dynamic blocks : no need to handle if empty
                if (i <= mappings.Length - 3 && mappings[i] == mappings[i + 2])
                    continue;
                
                if (mappings[i].type is BlockAssembly.BlocType.If && mappings[i - 1] != null)
                    i = AssignMidBlocks(i, mappings[i]);

                if (i < 0 || i >= mappings.Length - 1) break;

                if (mappings[i + 1] != null)
                {
                    mappings[i].nextBlock = mappings[i + 1].gameObject;
                    if (mappings[i].midBase != null) mappings[i + 1].midBase = mappings[i].midBase;
                    if (i > 0 && mappings[i - 1] != null && mappings[i].type is BlockAssembly.BlocType.Loop)
                        mappings[i + 1].midBase = mappings[i].gameObject;
                }
            }
        }

        private int AssignMidBlocks(int index, BlockAssembly parent)
        {
            Debug.Log("Assigning midblocks. Index = " + index);
            BlockAssembly[] mappings = codeZoneData.indexedBlockPositionMappings;
            if (mappings[index + 1] != null)
            {
                mappings[index].midBlock = mappings[index + 1].gameObject;
                mappings[index + 1].midBase = parent.gameObject;
            }

            for (var i = index + 1; i < mappings.Length - 1; i++)
            {
                if (mappings[i] == null) continue;
                if (mappings[i + 1] == null) continue;
                if (mappings[i] == parent) return i;
                if (mappings[i].type is BlockAssembly.BlocType.Loop && mappings[i - 1] == null) continue;

                if (mappings[i].type is BlockAssembly.BlocType.If && mappings[i - 1] != null)
                {
                    i = AssignMidBlocks(i, mappings[i]);
                    continue;
                }

                mappings[i].nextBlock = mappings[i + 1].gameObject;
                mappings[i].midBase = parent.gameObject;
                if (mappings[i] != parent) continue;
                if (mappings[i + 1].type is BlockAssembly.BlocType.If)
                    return AssignMidBlocks(i + 1, mappings[i + 1]);
                return i + 1;
            }

            return -1;
        }

        private void PlaceBlock()
        {
            //bool canBeSnapped = true;
            //int snapPositionIndex = startBlock.transform.parent.childCount;

            //if (canBeSnapped)

            SnapBlock(codeZoneData.HoveredPositionIndex);
            if (!pendingObj.TryGetComponent(out CachedTransform c))
                c = pendingObj.AddComponent<CachedTransform>();
            c.ApplyAll();

            pendingObj.GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);
            MouseIconHandler.Instance.SetCursorLock(false);
            MouseIconHandler.Instance.SetCursorDefault();
            hasPendingBlock = false;
            pendingObj = null;
            return;

            BlockAssembly pending = pendingObj.GetComponent<BlockAssembly>();
            BlockAssembly lastOfPending = GetLastBlockInPendingAssembly().GetComponent<BlockAssembly>();

            if (pending.collidingBlock != null)
            {
                if (pending.canAssembleNext && pending.canAssemblePrevious)
                {
                    if (pending.canAssemblePrevious &&
                        pending.collidingBlock.GetComponent<BlockAssembly>().nextBlock == null)
                    {
                        if (pending.collidingBlock.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If &&
                            !pending.collidingWithBlockEnd)
                        {
                            pending.collidingBlock.GetComponent<BlockAssembly>().midBlock = pendingObj;
                            pending.previousBlock = pending.collidingBlock;
                        }
                        else
                        {
                            pending.collidingBlock.GetComponent<BlockAssembly>().nextBlock = pendingObj;
                            pending.previousBlock = pending.collidingBlock;
                        }
                    }

                    if (pending.canAssembleNext &&
                        pending.collidingNextBlock.GetComponent<BlockAssembly>().previousBlock == null)
                    {
                        pending.transform.position = pending.collidingNextBlock.GetComponent<BlockAssembly>()
                            .previousBlockPosition.transform.position;
                        pending.collidingNextBlock.GetComponent<BlockAssembly>().previousBlock = pendingObj;
                        pending.nextBlock = pending.collidingNextBlock;
                    }

                    pendingObj = null;
                    return;
                }

                if (pending.canAssembleNext &&
                    pending.collidingBlock.GetComponent<BlockAssembly>().previousBlock == null)
                {
                    pending.transform.position = pending.collidingBlock.GetComponent<BlockAssembly>()
                        .previousBlockPosition.transform.position;
                    pending.collidingBlock.GetComponent<BlockAssembly>().previousBlock = pendingObj;
                    pending.nextBlock = pending.collidingBlock;
                }
                else if (pending.canAssemblePrevious &&
                         pending.collidingBlock.GetComponent<BlockAssembly>().nextBlock == null)
                {
                    if (pending.collidingBlock.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If &&
                        !pending.collidingWithBlockEnd)
                    {
                        pending.collidingBlock.GetComponent<BlockAssembly>().midBlock = pendingObj;
                        pending.previousBlock = pending.collidingBlock;
                    }
                    else
                    {
                        pending.collidingBlock.GetComponent<BlockAssembly>().nextBlock = pendingObj;
                        pending.previousBlock = pending.collidingBlock;
                    }
                }

                else if (pending.collidingBlock.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
                {
                    if (pending.canAssemblePrevious &&
                        pending.collidingBlock.GetComponent<BlockAssembly>().midBlock == null)
                    {
                        pending.collidingBlock.GetComponent<BlockAssembly>().midBlock = pendingObj;
                        pending.previousBlock = pending.collidingBlock;
                    }
                }
            }
            else if (lastOfPending.canAssembleNext &&
                     lastOfPending.collidingBlock.GetComponent<BlockAssembly>().previousBlock == null)
            {
                lastOfPending.collidingBlock.GetComponent<BlockAssembly>().previousBlock = lastOfPending.gameObject;
                lastOfPending.nextBlock = lastOfPending.collidingBlock;
            }

            pendingObj = null;
        }

        public static void SelectBlock(GameObject block)
        {
            MouseIconHandler.Instance.SetCursorHandHold();
            MouseIconHandler.Instance.SetCursorLock(true);

            pendingObj = block;
            hasPendingBlock = true;
            pendingObj.transform.SetParent(staticTransform);
            codeZoneBlocksManager.RecalculateData();

            pendingObj.GetComponent<BlockAssembly>().nextBlock = null;
            foreach (var image in pendingObj.GetComponentsInChildren<Image>())
                image.raycastTarget = false;
        }

        public void InstantiateNewBlock(GameObject bloc)
        {
            if (blocNb >= maxBlocNb) return;
            blocNb++;
            blocksList.GetComponent<BatchButtonActivator>().SetAllElementsInteractable(blocNb < maxBlocNb);
            Transform t = transform;
            SelectBlock(Instantiate(bloc, pos, t.rotation, t));
        }


        private List<GameObject> objectsToDestroy = new();

        private void Delete()
        {
            CreateDeleteList();

            foreach (GameObject obj in objectsToDestroy)
            {
                Instantiate(explosionFX, obj.transform.position, Quaternion.identity, transform);
                Destroy(obj);
                blocNb--;
                blocksList.GetComponent<BatchButtonActivator>().SetAllElementsInteractable(blocNb < maxBlocNb);
                if (blocNb < 0) blocNb = 0;
            }

            MouseIconHandler.Instance.SetCursorLock(false);
            MouseIconHandler.Instance.SetCursorDefault();
            hasPendingBlock = false;
            pendingObj = null;
        }

        private void CreateDeleteList()
        {
            objectsToDestroy.Clear();

            objectsToDestroy.Add(pendingObj);

            if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
            {
                if (pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                {
                    GameObject saveFirstIf = pendingObj;
                    while (pendingObj.GetComponent<BlockAssembly>().nextBlock != null ||
                           pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                    {
                        if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
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

                    pendingObj = saveFirstIf;
                }
            }

            if (pendingObj.GetComponent<BlockAssembly>().nextBlock != null)
            {
                pendingObj = pendingObj.GetComponent<BlockAssembly>().nextBlock;

                while (pendingObj.GetComponent<BlockAssembly>().nextBlock != null ||
                       pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                {
                    objectsToDestroy.Add(pendingObj);

                    if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
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

                objectsToDestroy.Add(pendingObj);
            }
        }

        private void ReadBlocIf()
        {
            if (pendingObj.GetComponent<BlockAssembly>().midBlock != null)
            {
                pendingObj = pendingObj.GetComponent<BlockAssembly>().midBlock;
                while (pendingObj.GetComponent<BlockAssembly>().nextBlock != null ||
                       pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                {
                    objectsToDestroy.Add(pendingObj);

                    if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
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


                objectsToDestroy.Add(pendingObj);
            }
        }

        public void MouseEnterTrash()
        {
            mouseOnTrash = true;
        }

        public void MouseExitTrash()
        {
            mouseOnTrash = false;
        }

        private GameObject GetLastBlockInPendingAssembly()
        {
            if (pendingObj.GetComponent<BlockAssembly>().nextBlock == null) return pendingObj;

            GameObject lastObj = pendingObj;
            while (lastObj.GetComponent<BlockAssembly>().nextBlock != null)
            {
                lastObj = lastObj.GetComponent<BlockAssembly>().nextBlock;
            }

            return lastObj;
        }
    }
}