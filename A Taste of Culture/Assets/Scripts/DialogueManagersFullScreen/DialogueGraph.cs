using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class DialogueGraph : EditorWindow
{
    // private string _fileName = "New Narrative";
    ObjectField sentenceField;
    DialogueGraphView _graphView;
    Toolbar toolbar;
    Sentence startingSentence;

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);

        _graphView.SendToBack();
    }

    private void GenerateToolbar()
    {
        toolbar = new Toolbar();

        Button button = new Button(clickEvent: () =>
        {
            DialogueContainer dialogue = (DialogueContainer)sentenceField.value;
            startingSentence = dialogue.startingSentence;
            if (startingSentence != null)
            {
                rootVisualElement.Remove(_graphView);
                ConstructGraphView();
                _graphView.AutoGenerateNodes(startingSentence);
            }
        })
        {
            text = "Show Dialogue Graph"
        };

        Button saveButton = new Button(clickEvent: () => { _graphView.SaveConnections(); })
        {
            text = "Save"
        };
        Button clearButton = new Button(clickEvent: () => { Clear(); })
        {
            text = "Clear"
        };

        sentenceField = new ObjectField
        {
            objectType = typeof(DialogueContainer)
        };
        toolbar.Add(sentenceField);
        toolbar.Add(button);
        toolbar.Add(saveButton);
        toolbar.Add(clearButton);

        rootVisualElement.Add(toolbar);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void Clear()
    {
        rootVisualElement.Remove(_graphView);
        rootVisualElement.Remove(toolbar);

        ConstructGraphView();
        GenerateToolbar();
    }
}
