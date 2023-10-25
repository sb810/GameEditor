using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Shawn;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CodeBlockManager : MonoBehaviour
{
    [AssetsOnly] [SerializeField] private GameObject explosionFX;

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

    private void Awake()
    {
        mainCamera = Camera.main;
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

private void Update()
    {
        if (pendingObj == null) return;

        int index = startBlock.GetComponentInParent<CodeZoneBlocksManager>().GetCurrentHoveredBlockPositionIndex();
        ReflowBlocks(index);
        HighlightBlock(index);
        
        pendingObj.transform.position = pos + posOffset;
        if (Input.GetMouseButtonUp(0))
        {
            if (mouseOnTrash || index < 0) Delete();
            else PlaceBlock();
            PostBlockReflow();
        }
        
        UpdateRemainingBlocksCount(remainingBlocks.text);
    }

    private void HighlightBlock(int index)
    {
        var children = startBlock.transform.parent.GetComponentsInChildren<BlockAssembly>();
        for (var i = 0; i < children.Length; i++)
        {
            children[i].nextBlockPosition.SetActive(i == index - 1);
        }
    }

    private void SnapBlock(int snapPositionIndex)
    {
        pendingObj.transform.parent = startBlock.transform.parent;
        pendingObj.transform.SetSiblingIndex(snapPositionIndex);
        HighlightBlock(0);
        pendingObj.transform.localPosition = startBlock.transform.localPosition + Vector3.down * (snapPositionIndex * 17 + 12);
    }

    private void ReflowBlocks(int pendingPositionIndex)
    {
        var children = startBlock.transform.parent.GetComponentsInChildren<BlockAssembly>();
        
        for (var index = 1; index < children.Length; index++)
        {
            int adjustedPositionIndex = index >= pendingPositionIndex && pendingPositionIndex > 0 ? index + 1 : index;
            children[index].transform.localPosition =
                startBlock.transform.localPosition + Vector3.down * (adjustedPositionIndex * 17 + 12);
        }
    }

    private void PostBlockReflow()
    {
        var children = startBlock.transform.parent.GetComponentsInChildren<BlockAssembly>();
        for (var i = 0; i < children.Length-1; i++)
        {
            children[i].nextBlock = children[i + 1].gameObject;
        }
    }
    
    private void PlaceBlock()
    {
        //bool canBeSnapped = true;
        //int snapPositionIndex = startBlock.transform.parent.childCount;

        //if (canBeSnapped)
        
        
        SnapBlock(startBlock.GetComponentInParent<CodeZoneBlocksManager>().GetCurrentHoveredBlockPositionIndex());
        
        MouseIconHandler.Instance.SetCursorLock(false);
        MouseIconHandler.Instance.SetCursorDefault();
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

    public void SelectObject(GameObject bloc)
    {
        if (blocNb >= maxBlocNb) return;
        
        MouseIconHandler.Instance.SetCursorHandHold();
        MouseIconHandler.Instance.SetCursorLock(true);
        blocNb++;
        Transform t = transform;
        pendingObj = Instantiate(bloc, pos, t.rotation, t);
        pendingObj.GetComponent<Image>().raycastTarget = false;
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
            if (blocNb < 0) blocNb = 0;
        }

        MouseIconHandler.Instance.SetCursorLock(false);
        MouseIconHandler.Instance.SetCursorDefault();
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

    public void NextScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("ProgLvl", sceneIndex + 1);
        if (sceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex + 1);
        }
        else
            SceneManager.LoadScene(1);
    }
}