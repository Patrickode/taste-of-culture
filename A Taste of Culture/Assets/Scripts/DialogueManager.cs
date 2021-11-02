using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Queue<string> sentences;
    private Queue<Sprite> expressions;

    private GameObject continueButton;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        expressions = new Queue<Sprite>();
        continueButton = GameObject.Find("ContinueButton");

        nameText.enabled = false;
        dialogueText.enabled = false;
        continueButton.SetActive(false);
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        nameText.enabled = true;
        dialogueText.enabled = true;
        continueButton.SetActive(true);

        nameText.text = dialogue.name;

        sentences.Clear();
        expressions.Clear();

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

        string sentence = sentences.Dequeue();
        Sprite expression = expressions.Dequeue();
        spriteRenderer.sprite = expression;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.005f);
        }
    }

    void EndDialogue()
    {
        continueButton.SetActive(false);
        nameText.enabled = false;
        dialogueText.enabled = false;
        animator.SetBool("IsOpen", false);
    }
}
