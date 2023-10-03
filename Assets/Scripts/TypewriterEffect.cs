using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

// attach to UI Text component (with the full text already there)

public class TypewriterEffect : MonoBehaviour 
{

    private TMP_Text tmp;
    private string story;
    [SerializeField] private bool waitForTextUpdate = true;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float characterDelay = 0.125f;

    void Awake () 
    {
        tmp = GetComponent<TMP_Text> ();
        story = tmp.text;
        
        if(!waitForTextUpdate)
            StartCoroutine (PlayText());
    }

    public void UpdateText(string str)
    {
        story = str;
        StopCoroutine(PlayText());
        StartCoroutine(PlayText());
    }
    

    IEnumerator PlayText()
    {
        tmp.text = "";
        
        yield return new WaitForSeconds(startDelay);
        foreach (char c in story) 
        {
            tmp.text += c;
            yield return new WaitForSeconds (characterDelay);
        }
    }

}