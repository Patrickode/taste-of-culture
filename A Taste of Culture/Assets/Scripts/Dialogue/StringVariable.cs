using UnityEngine;

[CreateAssetMenu]
public class StringVariable : ScriptableObject
{
    public string value;

    public void SetValue(string s)
    {
        value = s;
    }

    public void SetValue(StringVariable s)
    {
        value = s.value;
    }
}