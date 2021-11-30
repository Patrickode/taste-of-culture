using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

public class DialogueGraphView : GraphView
{

    public readonly Vector2 DefaultNodeSize = new Vector2(200, 200);
    public readonly Vector2 EntryPosition = new Vector2(200, 200);
    public readonly Vector2 MoveSpeed = new Vector2(50, 50);

    private int[] layers = new int[100];
    private List<DialogueNode> repeatedNodes = new List<DialogueNode>();

    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ClickSelector());
        ContentDragger contentDragger = new ContentDragger
        {
            panSpeed = MoveSpeed
        };
        this.AddManipulator(contentDragger);
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // AddElement(GenerateEntryPointNode());
    }

    private DialogueNode GenerateEntryPointNode(Sentence sentence)
    {
        DialogueNode node = new DialogueNode
        {
            title = sentence.speaker.value,
            Layer = 1,
            Index = 1,
            sentence = sentence,
            GUID = System.Guid.NewGuid().ToString(),
            DialogueText = "Start Text",
            EntryPoint = true
        };

        Port generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputPorts.Add(generatedPort);
        node.outputContainer.Add(generatedPort);

        TextField text = new TextField();
        text.SetValueWithoutNotify(sentence.text);
        text.multiline = true;
        text.style.maxWidth = 200;
        text.style.flexBasis = 150f;
        text.style.flexDirection = FlexDirection.Column;
        text.style.flexWrap = Wrap.Wrap;
        text.RegisterValueChangedCallback(evt => sentence.text = evt.newValue);
        node.titleContainer.Add(text);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(EntryPosition, DefaultNodeSize));
        return node;
    }

    public void CreateNode(DialogueNode node)
    {
        AddElement(node);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach(funcCall: port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts; 
    }

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        DialogueNode dialogueNode = new DialogueNode
        {
            title = nodeName,
            DialogueText = "text",
            GUID = System.Guid.NewGuid().ToString()
        };

        Port inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        Button button = new Button(clickEvent: () =>
        {
            AddChoicePort(dialogueNode);
        })
        {
            text = "New Choice"
        };
        dialogueNode.titleContainer.Add(button);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, DefaultNodeSize));

        return dialogueNode;
    }

    public DialogueNode CreateFollowingNode(Sentence sentence, int layer, int index, DialogueNode parentNode, int parentPortIndex)
    {
        Debug.Log($"Layer{layer}, Index{index}, {sentence.name}");
        Debug.Log($"Parent: {parentNode.Layer}, {parentNode.Index}");

        DialogueNode dialogueNode = new DialogueNode
        {
            title = sentence.speaker.value,
            Layer = layer,
            sentence = sentence,
            Index = index,
            GUID = System.Guid.NewGuid().ToString(),
            DialogueText = "text"
        };

        Port inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        dialogueNode.inputPort = inputPort;
        inputPort.portName = "Previous";
        Edge edge = inputPort.ConnectTo(parentNode.outputPorts[parentPortIndex]);
        Add(edge);

        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.SetPosition(new Rect(new Vector2(300 * layer - 100, 250 * index - 50), DefaultNodeSize));

        if (sentence.HasOptions())
        {
            for (int i = 0; i < sentence.options.Count; i++)
            {
                Port port = GeneratePort(dialogueNode, Direction.Output);
                TextField text = new TextField();
                text.SetValueWithoutNotify(sentence.options[i].text);
                text.RegisterValueChangedCallback(evt => sentence.options[i].text = evt.newValue);
                text.multiline = true;
                text.style.flexBasis = 100f;

                port.contentContainer.Add(text);
                port.portName = (i + 1).ToString();
                dialogueNode.outputPorts.Add(port);
                dialogueNode.outputContainer.Add(port);
            }
        }
        else
        {
            TextField text = new TextField();
            text.SetValueWithoutNotify(sentence.text);
            text.multiline = true;
            text.style.flexBasis = 150f;

            text.RegisterValueChangedCallback(evt => sentence.text = evt.newValue);
            dialogueNode.titleContainer.Add(text);

            Port port = GeneratePort(dialogueNode, Direction.Output);
            port.portName = "Next";
            dialogueNode.outputPorts.Add(port);
            dialogueNode.outputContainer.Add(port);
        }

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

        return dialogueNode;
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); // Arbitrary type
    }

    public void AddChoicePort(DialogueNode dialogueNode)
    {
        Port generatedPort = GeneratePort(dialogueNode, Direction.Output);

        int outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";

        //TextField textField = new TextField
        //{
        //    name = string.Empty,
        //    value = choicePortName
        //};
        //textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        //generatedPort.contentContainer.Add(new Label(" "));
        //generatedPort.contentContainer.Add(textField);
        //Button deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        //{
        //    text = "X"
        //};
        //generatedPort.contentContainer.Add(deleteButton);

        //generatedPort.portName = choicePortName;

        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public void AutoGenerateNodes(Sentence startingSentence)
    {
        DialogueNode entryNode = GenerateEntryPointNode(startingSentence);
        CreateNode(entryNode);
        DialogueNode node = null;
        
        if (startingSentence.nextSentence != null)
        {
            node = CreateFollowingNode(startingSentence.nextSentence, 2, 1, entryNode, 0);
            CreateNode(node);
        }
        Recursion(startingSentence.nextSentence, 2, node);
    }

    public void SaveConnections()
    {
        ResetSentenceLinks();

        ports.ForEach(funcCall: port =>
        {
            if (port.direction == Direction.Output)
            {
                DialogueNode outputNode = (DialogueNode)port.node;
                List<Edge> edges = port.connections.ToList();
                if (edges.Count != 0)
                {
                    DialogueNode inputNode = (DialogueNode)edges[0].input.node;
                    Debug.Log($"{outputNode.sentence.name} is connected to {inputNode.sentence.name}");
                    if (outputNode.sentence.HasOptions())
                    {
                        if (port.portName != null)
                        {
                            int index = int.Parse(port.portName) - 1;
                            outputNode.sentence.options[index].nextSentence = inputNode.sentence;
                        }
                    }
                    else
                    {
                        outputNode.sentence.nextSentence = inputNode.sentence;
                    }
                }
            }
        });
    }

    private void ResetSentenceLinks()
    {
        ports.ForEach(funcCall: port =>
        {
            DialogueNode outputNode = (DialogueNode)port.node;
            if (outputNode.sentence.HasOptions())
            {
                foreach (Choice option in outputNode.sentence.options)
                {
                    option.nextSentence = null;
                }
            }
            else
            {
                outputNode.sentence.nextSentence = null;
            }
        });
    }

    private bool CheckRepeat(Sentence sentence)
    {
        foreach (DialogueNode node in repeatedNodes)
        {
            if (node.sentence == sentence)
                return true;
        }
        return false;
    }

    DialogueNode GetRepeatedNode(Sentence sentence)
    {
        foreach (DialogueNode node in repeatedNodes)
        {
            if (node.sentence == sentence)
                return node;
        }
        return null;
    }

    private void Recursion(Sentence sentence, int layer, DialogueNode parentNode)
    {
        Sentence currentSentence = sentence;

        while (currentSentence != null)
        {
            if (!currentSentence.HasOptions())
            {
                currentSentence = currentSentence.nextSentence;
                if (currentSentence != null)
                {
                    bool repeated = CheckRepeat(currentSentence);
                    if (!repeated)
                    {
                        RenderNormally();
                    }
                    else
                    {
                        Debug.Log($"{currentSentence.name} repeated");
                        Debug.Log($"connected to {parentNode.sentence.name}'s {1} output port");
                        ConnectRepeated();
                    }
                }
                return;
            }
            else
            {
                List<Choice> options = currentSentence.options;
                for (int i = 0; i < options.Count; i++)
                {
                    currentSentence = options[i].nextSentence;
                    if (currentSentence != null)
                    {
                        bool repeated = CheckRepeat(currentSentence);
                        if (!repeated)
                        {
                            RenderNormally(i);
                        }
                        else
                        {
                            Debug.Log($"{currentSentence.name} repeated");
                            Debug.Log($"connected to {parentNode.sentence.name}'s {i + 1} output port");
                            ConnectRepeated(i);
                        }
                    }
                }
                return;
            }
        }

        void ConnectRepeated(int i = 0)
        {
            DialogueNode inputNode = GetRepeatedNode(currentSentence);
            if (inputNode != null)
            {
                Port inputPort = inputNode.inputPort;
                Port outputPort = parentNode.outputPorts[i];

                Edge edge = inputPort.ConnectTo(outputPort);
                Add(edge);
            }
        }

        void RenderNormally(int i = 0)
        {
            layers[layer + 1]++;
            DialogueNode node;
            node = CreateFollowingNode(currentSentence, layer + 1, layers[layer + 1], parentNode, i);
            CreateNode(node);
            repeatedNodes.Add(node);
            Recursion(currentSentence, layer + 1, node);
        }
    }

//    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
//    {
//        IEnumerable<Edge> targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

//        if (targetEdge.Any())
//        {
//            Edge edge = targetEdge.First();
//            edge.input.Disconnect(edge);
//            RemoveElement(targetEdge.First());
//        }

//        dialogueNode.outputContainer.Remove(generatedPort);
//        dialogueNode.RefreshPorts();
//        dialogueNode.RefreshExpandedState();
//    }
}
