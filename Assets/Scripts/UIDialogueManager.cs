using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

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
    
    private Story currentStory;
    public bool dialogueIsPlaying;

    void Awake()
    {
        dialogueTextComp = getDialogueTextComponent();
        dialogueIsPlaying = false;
        blankDialogue();
    }

    private IEnumerator RefreshDialogue()
    {

        // Read all the content until we can't continue any more
		while (currentStory.canContinue) 
        {
            // Continue gets the next line of the story
			string text = currentStory.Continue ();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the line of dialogue
            Debug.Log("Displaying text: " + text);
			displayDialogue(text);

            // Wait until current dialogue is done playing
            Debug.Log("dialogueIsPlaying = " + dialogueIsPlaying);
            yield return new WaitUntil(() => !dialogueIsPlaying);
        }

        if(currentStory.currentChoices.Count > 0) 
        {

        }
    }

    public void Dialogue(TextAsset dialogueJSON)
    {
        currentStory = new Story(dialogueJSON.text);

        StartCoroutine(RefreshDialogue());
    }

    public void displayDialogue(string dialogueText)
    {
        lockClick.Raise();
        dialogueIsPlaying = true;

        dialogueTextComp.text = dialogueText;
        
        dialogueTimerCoroutine = dialogueAutoBlankTimer(dialogueSecTimer);
        dialogueClickCoroutine = dialogueClickClose();

        Debug.Log("Start dialogueAutoBlankTimer coroutine");
        StartCoroutine(dialogueTimerCoroutine);
        Debug.Log("Start dialogueClickClose coroutine");
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
        yield return new WaitForSecondsRealtime(0.25f);
        yield return new WaitUntil(() => Input.anyKey);

        blankDialogue();
        unlockClick.Raise();
        StopCoroutine(dialogueTimerCoroutine);
        Debug.Log("dialogueClickClose ended");
    }

    private void blankDialogue()
    {
        dialogueTextComp.text = "";
        dialogueIsPlaying = false;
    }

    private TMP_Text getDialogueTextComponent()
    {
        return dialogueText.GetComponent<TMP_Text>();
    }
}
