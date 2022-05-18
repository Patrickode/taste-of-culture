using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("GameEvents")]
    public GameEvent ConversationEnded;
    public GameEvent ConversationStarted;
    [Space]
    public StringVariable playerName;
    [Tooltip("For the typing animation: determine how long it takes for each character to appear")]
    public float timeBetweenChars = 0.05f;
    [Header("UI")]
    public Text playerNameTextUI;
    public Text npcNameTextUI;
    public Text playerTextUI;
    public Text npcTextUI;
    public SpriteRenderer playerSprite;
    public SpriteRenderer npcSprite;

    [Tooltip("The part of the UI that displays the UI")]
    public GameObject DialogueUI;
    [Tooltip("The text UIs that displays options")]
    public Text[] optionsUI;

    public Animator animator;

    DialogueContainer dialogue;
    Sentence currentSentence;

    public void StartDialogue(DialogueContainer dialogueContainer)
    {
        if (!dialogueContainer.isAvailable)
            return;
        if (ConversationStarted != null)
            ConversationStarted.Raise();

        npcTextUI.text = null;
        playerTextUI.text = null;
        playerSprite.sprite = null;
        npcSprite.sprite = null;
        HideOptions();
        DialogueUI.SetActive(false);

        dialogue = dialogueContainer;
        if (playerNameTextUI != null)
            playerNameTextUI.text = playerName.value;
        currentSentence = dialogue.startingSentence;

        DisplayDialogue();
    }

    public void GoToNextSentence()
    {
        currentSentence = currentSentence.nextSentence;
        DisplayDialogue();
    }

    public void DisplayDialogue()
    {
        if (currentSentence == null)
        {
            EndDialogue();
            return;
        }

        if (!currentSentence.HasOptions())
        {
            DialogueUI.SetActive(true);
            HideOptions();

            Text dialogueText;
            if (currentSentence.speaker.value == playerName.value)
            {
                if (playerNameTextUI != null)
                {
                    playerNameTextUI.text = playerName.value;
                }
                dialogueText = playerTextUI;
                playerSprite.sprite = currentSentence.expression;
            }
            else
            {
                if (npcNameTextUI != null)
                {
                    npcNameTextUI.text = currentSentence.speaker.name;
                }
                dialogueText = npcTextUI;
                playerSprite.sprite = currentSentence.expression;
            }

            StopAllCoroutines();
            StartCoroutine(TypeOut(currentSentence.text, dialogueText));
        }
        else
        {
            DisplayOptions();
        }
    }

    IEnumerator TypeOut(string sentence, Text textbox)
    {
        textbox.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            textbox.text += letter;
            yield return new WaitForSeconds(timeBetweenChars);
        }
    }

    public void OptionsOnClick(int index)
    {
        Choice option = currentSentence.options[index];
        if (option.consequence != null)
        {
            Debug.Log($"Raised consequence event \"{option.consequence}\" of option \"{option.text}\"");
            option.consequence.Raise();
        }
        currentSentence = option.nextSentence;

        DisplayDialogue();
    }

    public void DisplayOptions()
    {
        DialogueUI.SetActive(false);

        if (currentSentence.speaker.value == playerName.value && playerNameTextUI != null)
        {
            playerNameTextUI.text = playerName.value;
        }

        if (currentSentence.options.Count <= optionsUI.Length)
        {
            for (int i = 0; i < currentSentence.options.Count; i++)
            {
                //Debug.Log(currentSentence.options[i].text);
                optionsUI[i].text = currentSentence.options[i].text;
                optionsUI[i].gameObject.SetActive(true);
            }
        }
    }

    public void HideOptions()
    {
        foreach (Text option in optionsUI)
        {
            option.gameObject.SetActive(false);
        }
    }

    public void EndDialogue()
    {
        DialogueUI.SetActive(false);
        //Debug.Log("Dialogue Ended");
        if (ConversationEnded != null)
            ConversationEnded.Raise();
    }
}
