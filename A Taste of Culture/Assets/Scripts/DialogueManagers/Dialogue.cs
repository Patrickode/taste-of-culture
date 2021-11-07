using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

// This is a basic class to hold relevant information about Dialogue.
// Includes name of mentor, the sentences they say during a given dialogue line, and their expressions
public class Dialogue
{
    public string name;

    [TextArea(3, 10)] // This just makes the textbox bigger in Unity for easy reading
    public string[] sentences;

    public Sprite[] expressions;
}
