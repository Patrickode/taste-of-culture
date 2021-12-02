using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Sentence : ScriptableObject
{
    [Tooltip("A string variable representing the player or NPC's name")]
    public StringVariable speaker;

    [TextArea(3, 10)]
    public string text = "text";

    public Sprite expression;

    [Tooltip("Available only when this dialogue has no choices")]
    public Sentence nextSentence;

    public List<Choice> options = new List<Choice>();

    public bool HasOptions()
    {
        return options.Count != 0;
    }
}


[System.Serializable]
public class Choice
{
    [TextArea(3, 10)]
    public string text;
    public Sentence nextSentence;
    public GameEvent consequence;
}