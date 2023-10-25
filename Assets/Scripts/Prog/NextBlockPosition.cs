using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBlockPosition : MonoBehaviour
{
    private BlockAssembly assembler;
    private void Start()
    {
        assembler = transform.parent.gameObject.GetComponent<BlockAssembly>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bloc") &&  (CodeBlockManager.pendingObj == assembler.gameObject || assembler.lastOfThePendingBlock) && other.GetComponent<BlockAssembly>().type != BlockAssembly.BlocType.Start)
        {
            assembler.canAssembleNext = true;
            assembler.collidingBlock = other.gameObject;
            assembler.collidingNextBlock = other.gameObject;

            other.GetComponent<BlockAssembly>().previousBlockPosition.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bloc"))
        {
            assembler.canAssembleNext = false;
            assembler.collidingBlock = null;
            assembler.collidingNextBlock = null;

            if (other.GetComponent<BlockAssembly>().type != BlockAssembly.BlocType.Start)
                other.GetComponent<BlockAssembly>().previousBlockPosition.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
