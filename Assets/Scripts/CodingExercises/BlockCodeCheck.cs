using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodingExercises
{
    public class BlockCodeCheck : MonoBehaviour
    {
        public List<BlocFunction.Function> codeList = new();
        private readonly List<GameObject> bloc = new();
        [FormerlySerializedAs("enemyList")] [FormerlySerializedAs("ennemyList")] public List<GameObject> levelEntitiesList = new();
        private GameObject pendingObj;

        private Vector3 startPosition;
        private Vector3 startScale;
        private Dictionary<BlocFunction.Function, Methods> methodByName = new();

        public delegate IEnumerator Methods();

        public GameObject boss;
        public GameObject checkpoint;
        public float speed;
        private float baseSpeed;
        [HideInInspector] public bool playerIsDead;
        // private Vector3 playerStartPosition;
        private int index;
        public float waitTime;
        private bool boucleOn;
        private int boucleIndex;

        [SerializeField] private float flipOffset;

        private void Start()
        {
            startPosition = boss.transform.position;
            startScale = boss.transform.localScale;
            baseSpeed = speed;

            Initialize();
        }

        public void CreateCodeList()
        {
            ResetPreview();
            codeList.Clear();
            bloc.Clear();

            pendingObj = gameObject;
        
            if(pendingObj.GetComponent<BlockAssembly>().nextBlock != null)
            {
                pendingObj = pendingObj.GetComponent<BlockAssembly>().nextBlock.gameObject;
                int i = 0;

                while (pendingObj.GetComponent<BlockAssembly>().nextBlock != null || pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                {
                    codeList.Add(pendingObj.GetComponent<BlocFunction>().function);
                    bloc.Add(pendingObj);

                    if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
                    {
                        GameObject saveIf = pendingObj;
                        ReadBlocIf();
                        pendingObj = saveIf;
                        codeList.Add(BlocFunction.Function.EndIf);
                        bloc.Add(pendingObj.transform.GetChild(0).gameObject);
                    }

                    if (pendingObj.GetComponent<BlockAssembly>().nextBlock == null && pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                    {
                        index = 0;
                        boucleOn = false;
                        CodeString();
                        StartCoroutine(methodByName[codeList[0]]());
                        return;
                    }
                    
                    if (pendingObj.GetComponent<BlockAssembly>().nextBlock != null)
                        pendingObj = pendingObj.GetComponent<BlockAssembly>().nextBlock;
                    
                    ++i;
                    if (i > 100)
                        throw new OverflowException("Infinite loop detected!");
                }


                codeList.Add(pendingObj.GetComponent<BlocFunction>().function);
                bloc.Add(pendingObj);
                if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
                {
                    codeList.Add(BlocFunction.Function.EndIf);
                    bloc.Add(pendingObj.transform.GetChild(0).gameObject);
                }   

                index = 0;
                boucleOn = false;
                CodeString();
                StartCoroutine(methodByName[codeList[0]]());
           
            }
        }

        private void ReadBlocIf()
        {
            if(pendingObj.GetComponent<BlockAssembly>().midBlock != null)
            {
                pendingObj = pendingObj.GetComponent<BlockAssembly>().midBlock;
                while (pendingObj.GetComponent<BlockAssembly>().nextBlock != null || pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                {
                    codeList.Add(pendingObj.GetComponent<BlocFunction>().function);
                    bloc.Add(pendingObj);

                    if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
                    {
                        GameObject saveIf = pendingObj;
                        ReadBlocIf();
                        pendingObj = saveIf;
                        codeList.Add(BlocFunction.Function.EndIf);
                        bloc.Add(pendingObj.transform.GetChild(0).gameObject);
                    }

                    if (pendingObj.GetComponent<BlockAssembly>().nextBlock == null && pendingObj.GetComponent<BlockAssembly>().midBlock != null)
                        return;
                    if (pendingObj.GetComponent<BlockAssembly>().nextBlock != null)
                    {
                        pendingObj = pendingObj.GetComponent<BlockAssembly>().nextBlock;
                    }
                }
            

                codeList.Add(pendingObj.GetComponent<BlocFunction>().function);
                bloc.Add(pendingObj);
                if (pendingObj.GetComponent<BlockAssembly>().type == BlockAssembly.BlocType.If)
                    codeList.Add(BlocFunction.Function.EndIf);
            }
        }

        public void UnhighlightAllBlocks()
        {
            foreach(GameObject obj in bloc)
            {
                if (obj != null && obj.TryGetComponent(out BlockAssembly assembly))
                    assembly.SetExecutionHighlightActive(false);
            }
        }

        public void ResetPreview()
        {
            StopAllCoroutines();
            Animator animator = boss.GetComponent<Animator>();
            animator.SetBool(IsWalkingAnimHash, false);
            animator.Rebind();
            animator.Update(0f);
            boss.transform.position = startPosition;
            boss.transform.localScale = startScale;
            boss.transform.rotation = Quaternion.Euler(Vector3.zero);
            boss.GetComponent<PreviewBoss>().facing = 1;
            speed = baseSpeed;
            checkpoint.GetComponent<PreviewCheckpoint>().ResetCounters();
            foreach(Transform obj in checkpoint.transform)
            {
                obj.gameObject.SetActive(true);
                obj.GetComponent<SpriteRenderer>().enabled = true;
            }
            foreach(GameObject obj in levelEntitiesList)
            {
                obj.SetActive(true);
                Animator objAnim = obj.GetComponent<Animator>();
                objAnim.SetBool(IsDeadAnimHash, false);
                objAnim.Rebind();
                objAnim.Update(0f);
                obj.GetComponent<BoxCollider2D>().enabled = true;
            }
            
            UnhighlightAllBlocks();
        }

        //Function List
        private void Initialize()
        {
            methodByName.Add(BlocFunction.Function.Avancer, Avancer);
            methodByName.Add(BlocFunction.Function.Attaquer, Attaquer);
            methodByName.Add(BlocFunction.Function.Flip, Flip);
            methodByName.Add(BlocFunction.Function.Boucle, Boucle);
            methodByName.Add(BlocFunction.Function.IfJoueur, IfJoueur);
            methodByName.Add(BlocFunction.Function.IfObstacle, IfObstacle);
            methodByName.Add(BlocFunction.Function.EndIf, EndIf);
        }


        private IEnumerator Avancer()
        {

            bloc[index].GetComponent<BlockAssembly>().SetExecutionHighlightActive(true);
            index++;
            print(index + ": Avancer");
            float elapsedTime = 0f;
            Vector3 currentPos = boss.transform.position;
            Vector3 goToPos = currentPos + (Vector3.left * speed);

            boss.GetComponent<Animator>().SetBool(IsWalkingAnimHash, true);

            while (elapsedTime < waitTime)
            {
                boss.transform.position = Vector3.Lerp(currentPos, goToPos, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            boss.transform.position = goToPos;
            boss.GetComponent<Animator>().SetBool(IsWalkingAnimHash, false);
            yield return new WaitForSeconds(0.5f);

            bloc[index-1].GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);
            if (index<codeList.Count)
                StartCoroutine(methodByName[codeList[index]]());
            else if (boucleOn)
            {
                index = boucleIndex;
                StartCoroutine(methodByName[codeList[index]]());
            }

            yield return null;
        }

        private IEnumerator Attaquer()
        {
            bloc[index].GetComponent<BlockAssembly>().SetExecutionHighlightActive(true);
            index++;
            print(index + ": Attaquer");

            //boss.GetComponent<PreviewBoss>().Attack();
            boss.GetComponent<Animator>().SetTrigger(AttackAnimHash);
            yield return new WaitForSeconds(1f);

            bloc[index - 1].GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);
            if (index < codeList.Count)
                StartCoroutine(methodByName[codeList[index]]());
            else if (boucleOn)
            {
                index = boucleIndex;
                StartCoroutine(methodByName[codeList[index]]());
            }

            yield return null;
        }

        private IEnumerator Flip()
        {
            PreviewBoss pb = boss.GetComponent<PreviewBoss>();
            bloc[index].GetComponent<BlockAssembly>().SetExecutionHighlightActive(true);
            index++;
            print(index + ": Flip");
            var localScale = boss.transform.localScale;
            boss.transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            pb.facing *= -1;
            boss.transform.position = boss.transform.position + Vector3.right * flipOffset * pb.facing;
            speed = -speed;

            yield return new WaitForSeconds(0.5f);

            bloc[index-1].GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);
            if (index < codeList.Count)
                StartCoroutine(methodByName[codeList[index]]());
            else if (boucleOn)
            {
                index = boucleIndex;
                StartCoroutine(methodByName[codeList[index]]());
            }

            yield return null;
        }

        private IEnumerator Boucle()
        {
            boucleOn = true;
            boucleIndex = index;

            bloc[index].GetComponent<BlockAssembly>().SetExecutionHighlightActive(true);
            index++;
            print(index + ": Boucle");
            yield return new WaitForSeconds(0.5f);

            bloc[index-1].GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);
            if (index < codeList.Count)
                StartCoroutine(methodByName[codeList[index]]());
            else if (boucleOn)
            {
                index = boucleIndex;
                StartCoroutine(methodByName[codeList[index]]());
            }
            

            yield return null;
        }


        private IEnumerator IfJoueur()
        {
            bloc[index].GetComponent<BlockAssembly>().SetExecutionHighlightActive(true);
            index++;
            print("IfJoueur");
            yield return new WaitForSeconds(0.5f);

            bloc[index - 1].GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);

            if (boss.GetComponent<PreviewBoss>().seePlayer)
            {
                StartCoroutine(methodByName[codeList[index]]());
            }
            else
            {
                while (codeList[index] != BlocFunction.Function.EndIf)
                {
                    index++;
                }
                StartCoroutine(methodByName[codeList[index]]());

            }

            yield return null;
        }

        private IEnumerator IfObstacle()
        {
            bloc[index].GetComponent<BlockAssembly>().SetExecutionHighlightActive(true);
            index++;
            print(index+": IfObstacle");
            yield return new WaitForSeconds(0.5f);

            bloc[index - 1].GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);

            if (boss.GetComponent<PreviewBoss>().hole)
            {
                StartCoroutine(methodByName[codeList[index]]());
            }
            else
            {
                while (codeList[index] != BlocFunction.Function.EndIf)
                {
                    index++;
                }
                StartCoroutine(methodByName[codeList[index]]());
            }

            yield return null;
        }

        private IEnumerator EndIf()
        {
            bloc[index].GetComponent<BlockAssembly>().SetExecutionHighlightActive(true);
            index++;
            print(index + ": EndIf");
            yield return new WaitForSeconds(0.5f);

            bloc[index-1].GetComponent<BlockAssembly>().SetExecutionHighlightActive(false);

            if (index < codeList.Count)
                StartCoroutine(methodByName[codeList[index]]());
            else if (boucleOn)
            {
                index = boucleIndex;
                StartCoroutine(methodByName[codeList[index]]());
            }


            yield return null;
        }


        public TMP_Text codeText;
        private static readonly int IsWalkingAnimHash = Animator.StringToHash("IsWalking");
        private static readonly int AttackAnimHash = Animator.StringToHash("Attack");
        private static readonly int IsDeadAnimHash = Animator.StringToHash("IsDead");

        public void CodeString()
        {
            string code = "Start() \n{\n";
            int tabNb = 1;
            foreach (BlocFunction.Function line in codeList)
            {
                if (line == BlocFunction.Function.EndIf)
                    tabNb--;

                for (int i = 0; i < tabNb; i++)
                {
                    code += "\t";
                }
            
                if(line == BlocFunction.Function.EndIf)
                {
                    code += "}\n";
                }   
                else if(line == BlocFunction.Function.IfJoueur)
                {
                    code += "If(Joueur)\n";
                    for (int i = 0; i < tabNb; i++)
                    {
                        code += "\t";
                    }
                    code += "{\n";
                    tabNb++;
                }
                else if (line == BlocFunction.Function.IfObstacle)
                {
                    code += "If(Obstacle)\n";
                    for (int i = 0; i < tabNb; i++)
                    {
                        code += "\t";
                    }
                    code += "{\n";
                    tabNb++;
                }
                else if (line == BlocFunction.Function.Boucle)
                {
                    code += "Boucle()\n";
                    for (int i = 0; i < tabNb; i++)
                    {
                        code += "\t";
                    }
                    code += "{\n";
                    tabNb++;
                }
                else
                {
                    code += line.ToString() + "();\n";
                }
          
            }
            code += "}";
            codeText.text = code;
        }

    }
}
