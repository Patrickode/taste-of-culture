using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

[CustomEditor(typeof(Sentence))]
public class SentenceEditor : Editor
{
    Sentence currentSentence;
    ReorderableList list;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("options"), true, true, true, true);
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Options");
        };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2;

            currentSentence.options[index].text = EditorGUI.TextField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                currentSentence.options[index].text
            );

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 5, 150, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("nextSentence"), GUIContent.none
            );

            if (GUILayout.Button($"Create Next Sentence for Option{index + 1}"))
            {
                Sentence newSentence = CreateInstance<Sentence>();
                DirectoryInfo dir = Directory.GetParent(AssetDatabase.GetAssetPath(target));
                string uniqueDir = AssetDatabase.GenerateUniqueAssetPath(dir + "/New Sentence.asset");
                Debug.Log(uniqueDir);

                AssetDatabase.CreateAsset(newSentence, uniqueDir);
                AssetDatabase.SaveAssets();

                currentSentence.options[index].nextSentence = newSentence;
            }

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + 10, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("consequence"), GUIContent.none
            );

            list.elementHeight = EditorGUIUtility.singleLineHeight * 4;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        currentSentence = (Sentence)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("speaker"));
        EditorGUILayout.Space(); 
        EditorGUILayout.PropertyField(serializedObject.FindProperty("expression"));
        EditorGUILayout.Space();

        if (!currentSentence.HasOptions())
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("text"), GUILayout.MinHeight(40), GUILayout.ExpandHeight(true));

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextSentence"));

            if (GUILayout.Button("New", GUILayout.Width(50)))
            {
                Sentence newSentence = CreateInstance<Sentence>();
                DirectoryInfo dir = Directory.GetParent(AssetDatabase.GetAssetPath(target));
                string uniqueDir = AssetDatabase.GenerateUniqueAssetPath(dir + "/New Sentence.asset");
                Debug.Log(uniqueDir);

                AssetDatabase.CreateAsset(newSentence, uniqueDir);
                AssetDatabase.SaveAssets();

                currentSentence.nextSentence = newSentence;

                Selection.activeObject = newSentence;
                EditorUtility.FocusProjectWindow();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
