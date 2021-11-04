using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This handles the initialization and ending of the dialogue, printing it to screen, etc.
public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Queue<string> sentences;
    private Queue<Sprite> expressions;

    private GameObject continueButton;

    // Initializations. Make sure all the text fields and mentor are not visible yet.
    void Start()
    {
        sentences = new Queue<string>();
        expressions = new Queue<Sprite>();
        continueButton = GameObject.Find("ContinueButton");

        nameText.enabled = false;
        dialogueText.enabled = false;
        continueButton.SetActive(false);
        spriteRenderer.enabled = false;
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Fade in dialogue box and make the text fields visible
        animator.SetBool("IsOpen", true);

        nameText.enabled = true;
        dialogueText.enabled = true;
        continueButton.SetActive(true);
        spriteRenderer.enabled = true;

        nameText.text = dialogue.name;

        // Clear anything that may be lingering in the queues
        sentences.Clear();
        expressions.Clear();

        // Add dialogue and corresponding expressions to the queues
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        foreach (Sprite expression in dialogue.expressions)
        {
            expressions.Enqueue(expression);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
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

    void EndDialogue()
    {
        // Hide everything and fade out the dialogue box
        continueButton.SetActive(false);
        nameText.enabled = false;
        dialogueText.enabled = false;
        spriteRenderer.enabled = false;
        animator.SetBool("IsOpen", false);
    }
}
