using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class DialogueNode : Node
{
    public Port parentPort;
    public int Layer;
    public int Index;
    public Sentence sentence;
    public string GUID;
    public string DialogueText;
    public bool EntryPoint = false;

    public Port inputPort;
    public List<Port> outputPorts = new List<Port>();
}
