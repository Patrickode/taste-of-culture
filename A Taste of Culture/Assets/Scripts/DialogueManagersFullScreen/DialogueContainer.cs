using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class DialogueContainer : ScriptableObject
{
    public Sentence startingSentence;
    public bool isAvailable;
}
