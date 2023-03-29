using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class UIDialogueManager : MonoBehaviour
{
    private static UIDialogueManager instance;

    [SerializeField]
    private int dialogueSecTimer;

    [Header("GameObjects")]
    [SerializeField]
    private GameObject dialogueText;
    
    [SerializeField]
    private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

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
    public bool choiceIsPlaying;

    void Awake()
    {
        if (instance != null) {
            Debug.LogWarning("More than one UiDialogueManager");
        }
        instance = this;
    }

    private void Start()
    {
        dialogueTextComp = getDialogueTextComponent();
        dialogueIsPlaying = false;
        choiceIsPlaying = false;
        blankDialogue();

        // Get all the textmeshpro components of the choice buttons
        choicesText = new TextMeshProUGUI[choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choicesText[i] = choices[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public static UIDialogueManager GetInstance()
    {
        return instance;
    }

    private IEnumerator RefreshDialogue()
    {
        // Lock click, just in case
        lockClick.Raise();

        ClearChoiceButtons();
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
            DisplayChoices();
        }
        // If we've read all the content and there's no choices, the story is finished!
		else {
			ClearChoiceButtons();
            unlockClick.Raise();
		}
    }

    public void Dialogue(TextAsset dialogueJSON)
    {
        currentStory = new Story(dialogueJSON.text);

        StartCoroutine(RefreshDialogue());
    }

    public void displayDialogue(string dialogueText)
    {
        dialogueIsPlaying = true;

        dialogueTextComp.text = dialogueText;
        
        dialogueTimerCoroutine = dialogueAutoBlankTimer(dialogueSecTimer);
        dialogueClickCoroutine = dialogueClickClose();

        Debug.Log("Start dialogueAutoBlankTimer coroutine");
        StartCoroutine(dialogueTimerCoroutine);
        Debug.Log("Start dialogueClickClose coroutine");
        StartCoroutine(dialogueClickCoroutine);
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        ClearChoiceButtons();

        choiceIsPlaying = true;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " 
            + currentChoices.Count + " Number of choices allowed: " + choices.Length);
        }
        for (int i = 0; i < currentChoices.Count; i++)
        {
            choices[i].SetActive(true);
            choicesText[i].text = currentChoices[i].text;
            
            Choice choice = currentChoices [i];

            choices[i].GetComponent<Button>().onClick.AddListener (delegate {
                OnClickChoiceButton (choice);
            });
        }
    }

    void OnClickChoiceButton (Choice choice) {
		currentStory.ChooseChoiceIndex (choice.index);
		StartCoroutine(RefreshDialogue());
	}


    private void ClearChoiceButtons()
    {
        foreach (TextMeshProUGUI buttonText in choicesText)
        {
            buttonText.text = "";
        }
        foreach (GameObject button in choices)
        {
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.SetActive(false);
        }
        choiceIsPlaying = false;
    }

    private IEnumerator dialogueAutoBlankTimer(int seconds) 
    {
        yield return new WaitForSecondsRealtime(seconds);

        blankDialogue();
        StopCoroutine(dialogueClickCoroutine);
        Debug.Log("dialogueAutoBlankTimer ended");
    }

    private IEnumerator dialogueClickClose()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        yield return new WaitUntil(() => Input.anyKey);

        blankDialogue();
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
