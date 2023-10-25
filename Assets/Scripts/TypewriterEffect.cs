using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

// attach to UI Text component (with the full text already there)

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private bool active;
    
    private TMP_Text tmp;
    private string story;
    [SerializeField] private bool waitForTextUpdate = true;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float characterDelay = 0.125f;
    
    private bool awoken = false;

    public void SetActive(bool a)
    {
        active = a;
    }

    private void Awake ()
    {
        if (awoken || !active) return;
        awoken = true;
        
        tmp = GetComponent<TMP_Text> ();
        story = tmp.text;
        
        if(!waitForTextUpdate)
            StartCoroutine (PlayText());
    }

    public void UpdateText(string str)
    {
        if (!active) return;
        
        Awake();

        story = str;
        if(!waitForTextUpdate)
            StopCoroutine(PlayText());
        StartCoroutine(PlayText());
    }


    private IEnumerator PlayText()
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