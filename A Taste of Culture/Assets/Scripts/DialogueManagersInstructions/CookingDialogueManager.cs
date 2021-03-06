using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This handles the initialization and ending of the dialogue, printing it to screen, etc.
public class CookingDialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public GameObject continueButton;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public CookingSceneManager sceneManager;

    private Queue<string> sentences;
    private Queue<Sprite> expressions;

    // Initializations. Make sure all the text fields and mentor are not visible yet.
    void Start()
    {
        sentences = new Queue<string>();
        expressions = new Queue<Sprite>();

        nameText.enabled = false;
        dialogueText.enabled = false;
        continueButton.SetActive(false);
        spriteRenderer.enabled = false;

    }

    public void StartDialogue(Dialogue dialogue, bool showContinueButton = true, float fadeInDuration = 0.25f)
    {

        // Fade in dialogue box and make the text fields visible
        //   CrossFade goes from current state to argument state, as opposed to SetTrigger 
        //   just queueing(?) another animation
        animator.CrossFade("DialogueBoxOpen", fadeInDuration);
        //animator.SetTrigger("StartDialogue");

        nameText.enabled = true;
        dialogueText.enabled = true;
        continueButton.SetActive(showContinueButton);
        spriteRenderer.enabled = true;

        nameText.text = dialogue.name;

        // Clear anything that may be lingering in the queues
        sentences.Clear();
        expressions.Clear();

        // Add dialogue and corresponding expressions to the queues
        foreach (string sentence in dialogue.sentences)
            sentences.Enqueue(sentence);

        foreach (Sprite expression in dialogue.expressions)
            expressions.Enqueue(expression);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count < 1)
        {
            EndDialogue();
            return;
        }

        // Pop out sentence and expression and type them on screen
        string sentence = sentences.Dequeue();
        Sprite expression = expressions.Dequeue();
        spriteRenderer.sprite = expression;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        // For each letter, type it on screen and wait for a very short period of time
        // This creates the scrolling effect
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.005f);
        }
    }

    public void EndDialogue(float fadeOutDuration = 0.25f)
    {
        // Hide everything and fade out the dialogue box
        continueButton.SetActive(false);
        nameText.enabled = false;
        dialogueText.enabled = false;
        spriteRenderer.enabled = false;
        animator.CrossFade("DialogueBoxClose", fadeOutDuration);
        //animator.SetTrigger("EndDialogue");

        if (sceneManager != null && sceneManager.dialogueString == "intro")
        {
            sceneManager.IntroEnded();
        }
        else if (sceneManager != null && sceneManager.dialogueString == "error")
        {
            sceneManager.MarginsEnded();
        }
    }
}