using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodingExercises
{
    public class CodeZoneData
    {
        public int TotalPositionIndexes;
        public int HoveredPositionIndex;
        public BlockAssembly[] indexedBlockPositionMappings;

        public CodeZoneData(int t, int c, BlockAssembly[] pair)
        {
            TotalPositionIndexes = t;
            HoveredPositionIndex = c;
            indexedBlockPositionMappings = pair;
        }
    }
    
    public class CodeZoneBlocksManager : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Canvas canvas;
        private bool hovering;
        private float mousePositionY;
        
        private const int BlockHeight = 17;
        private const int StartBlockOffset = 12;
        
        private int totalBlocksHeight;
        private int totalBlockPositions;
        private BlockAssembly[] indexedBlockPositionMappings;

        private void Start()
        {
            RecalculateData();
        }

        public void OnPointerMove(PointerEventData eventData)
        {        
            RectTransform rectTransform = transform as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, Camera.main, out var localMousePosition))
            {
                mousePositionY = -(localMousePosition.y - 129);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            RecalculateData();
            hovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // RecalculateData();
            hovering = false;
        }

        public void RecalculateData()
        {
            totalBlockPositions = GetChildrenPositionsAmount(transform);
            indexedBlockPositionMappings = new BlockAssembly[totalBlockPositions];
            GetChildrenPositionPairings(0, transform);
            totalBlocksHeight = totalBlockPositions * BlockHeight + StartBlockOffset;
        }

        private int GetChildrenPositionsAmount(Transform parent)
        {
            if (parent.childCount == 0) return 0;
            List<BlockAssembly> children = new();
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).TryGetComponent(out BlockAssembly blockAssembly))
                    children.Add(blockAssembly);
            }
            
            int positions = 0;
            foreach (var child in children)
            {
                if (child.type is BlockAssembly.BlocType.Loop or BlockAssembly.BlocType.If)
                {
                    positions += GetChildrenPositionsAmount(child.transform) + 3;
                }
                else positions++;
            }
            return positions;
        }

        private void GetChildrenPositionPairings(int index, Transform parent)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                if (!parent.GetChild(i).TryGetComponent(out BlockAssembly block)) continue;
                Debug.Log("Regular ! TYPE " + block.type + ", INDEX " + index);
                if (block.type is BlockAssembly.BlocType.Loop or BlockAssembly.BlocType.If)
                {
                    indexedBlockPositionMappings[index++] = block;
                    index = GetChildrenPositionPairingsInBlock(index, block);
                    indexedBlockPositionMappings[index++] =
                        null; // logic : if previous is null, then it's the end of the block
                    indexedBlockPositionMappings[index++] = block;
                }
                else indexedBlockPositionMappings[index++] = block;
            }
        }

        private int GetChildrenPositionPairingsInBlock(int index, BlockAssembly block)
        {
            Transform t = block.transform;
            for (var i = 0; i < t.childCount; i++)
            {
                if (!t.GetChild(i).TryGetComponent(out BlockAssembly child)) continue;
                if (child == block) continue;
                Debug.Log("In block ! TYPE " + child.type + ", INDEX " + index);
                if (child.type is BlockAssembly.BlocType.Loop or BlockAssembly.BlocType.If)
                {
                    indexedBlockPositionMappings[index++] = child;
                    index = GetChildrenPositionPairingsInBlock(index, child);
                    indexedBlockPositionMappings[index++] = null; // logic : if previous is null, then it's the end of the block
                    indexedBlockPositionMappings[index++] = child;
                }
                else indexedBlockPositionMappings[index++] = child;
            }

            return index;
        }

        public CodeZoneData GetCodeZoneData()
        {
            CodeZoneData data = new CodeZoneData(totalBlockPositions,-1, indexedBlockPositionMappings);
            if (!hovering) return data;

            if (mousePositionY >= totalBlocksHeight)
            {
                data.HoveredPositionIndex = totalBlockPositions;
                return data;
            }
        
            int index = (int)(mousePositionY / BlockHeight);
            data.HoveredPositionIndex = Mathf.Clamp(index, 0, totalBlockPositions);
            return data;
        }
    }
}
