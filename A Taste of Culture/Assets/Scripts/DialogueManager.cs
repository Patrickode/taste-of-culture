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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        sentences = new Queue<string>();
        expressions = new Queue<Sprite>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("StartDialogue");
        animator.SetBool("IsOpen", true);

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
        Debug.Log("DisplayNextSentence");
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
        Debug.Log("TypeSentence");
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }

    void EndDialogue()
    {
        Debug.Log("EndDialogue");
        animator.SetBool("IsOpen", false);
    }
}
