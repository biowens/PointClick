using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.UnityIntegration;

public class UIDialogueManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueText;
    [SerializeField]
    private int dialogueSecTimer;

    [Header("Game Events")]
    [SerializeField]
    private GameEvent lockClick;
    [SerializeField]
    private GameEvent unlockClick;

    private TMP_Text dialogueTextComp;
    private IEnumerator dialogueTimerCoroutine;
    private IEnumerator dialogueClickCoroutine;

    void Awake()
    {
        dialogueTextComp = getDialogueTextComponent();

        blankDialogue();
    }

    public void startDialogue()
    {
        
    }

    public void displayDialogue(string dialogueText)
    {
        lockClick.Raise();

        dialogueTextComp.text = dialogueText;
        
        dialogueTimerCoroutine = dialogueAutoBlankTimer(dialogueSecTimer);
        dialogueClickCoroutine = dialogueClickClose();

        Debug.Log("Start dialogueAutoBlankTimer coroutine");
        StartCoroutine(dialogueTimerCoroutine);
        Debug.Log("Start StartCoroutine coroutine");
        StartCoroutine(dialogueClickCoroutine);
    }

    private IEnumerator dialogueAutoBlankTimer(int seconds) 
    {
        yield return new WaitForSecondsRealtime(seconds);

        blankDialogue();
        unlockClick.Raise();
        StopCoroutine(dialogueClickCoroutine);
        Debug.Log("dialogueAutoBlankTimer ended");
    }

    private IEnumerator dialogueClickClose()
    {
        yield return new WaitUntil(() => Input.anyKey);

        blankDialogue();
        unlockClick.Raise();
        StopCoroutine(dialogueTimerCoroutine);
        Debug.Log("dialogueClickClose ended");
    }

    private void blankDialogue()
    {
        dialogueTextComp.text = "";
    }

    private TMP_Text getDialogueTextComponent()
    {
        return dialogueText.GetComponent<TMP_Text>();
    }
}
