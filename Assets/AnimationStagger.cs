using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimationStagger : MonoBehaviour
{
    private static readonly int Offset = Animator.StringToHash("Offset");

    // Start is called before the first frame update
    void Start()
    {
        float offset = Random.Range(0f, 1f);
        Debug.Log(offset);
        GetComponent<Animator>().SetFloat(Offset, offset);
        Debug.Log(GetComponent<Animator>().GetFloat(Offset));
    }
}
