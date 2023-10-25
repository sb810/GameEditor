using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousBlockPosition : MonoBehaviour
{
    private BlockAssembly assembler;

    private void Start()
    {
        assembler = transform.parent.gameObject.GetComponent<BlockAssembly>();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Bloc") && other.GetComponent<BlockAssembly>().type != BlockAssembly.BlocType.If && CodeBlockManager.pendingObj == assembler.gameObject)
        {
            assembler.canAssemblePrevious = true;
            assembler.collidingBlock = other.gameObject;
            other.GetComponent<BlockAssembly>().nextBlockPosition.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (other.CompareTag("Bloc") && other.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If && CodeBlockManager.pendingObj == assembler.gameObject)
        {
            assembler.canAssemblePrevious = true;
            assembler.collidingBlock = other.gameObject;
            other.GetComponent<BlockAssembly>().midBlockPosition.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (other.CompareTag("BlocEnd") &&  CodeBlockManager.pendingObj == assembler.gameObject)
        {
            assembler.canAssemblePrevious = true;
            assembler.collidingWithBlockEnd = true;
            assembler.collidingBlock = other.transform.parent.gameObject;
            other.transform.parent.GetComponent<BlockAssembly>().nextBlockPosition.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bloc"))
        {
            assembler.canAssemblePrevious = false;
            assembler.collidingBlock = null;
            if(other.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
            {
                other.GetComponent<BlockAssembly>().midBlockPosition.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                other.GetComponent<BlockAssembly>().nextBlockPosition.GetComponent<SpriteRenderer>().enabled = false;
            }
            
        }

        if (other.CompareTag("BlocEnd"))
        {

            assembler.canAssemblePrevious = false;
            assembler.collidingWithBlockEnd = false;


            assembler.collidingBlock = null;

            other.transform.parent.GetComponent<BlockAssembly>().nextBlockPosition.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
