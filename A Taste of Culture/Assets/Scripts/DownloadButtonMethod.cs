using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using SFB;

public class DownloadButtonMethod : MonoBehaviour
{
    private ExtensionFilter recipeFilter = new ExtensionFilter("Portable Document Format", "pdf");

    // if preprocessors and enclosed code from
    // https://github.com/gkngkc/UnityStandaloneFileBrowser/blob/master/Assets/StandaloneFileBrowser/Sample/CanvasSampleSaveFileImage.cs
    // Would have helped if it said that WebGL needs ENTIRELY DIFFERENT CODE up front instead of hidden away in a sample, but what can you do
#if UNITY_WEBGL && !UNITY_EDITOR
    private byte[] fileBytes;

    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Called from browser
    public void OnFileDownload()
    {
        Debug.Log("File successfully downloaded");
    }
#endif

    public void Download(string sourceFileName)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, sourceFileName);
        string userPath = null;
#if UNITY_WEBGL && !UNITY_EDITOR
        fileBytes = File.ReadAllBytes(sourcePath);
        DownloadFile(gameObject.name, "OnFileDownload", "sample.png", fileBytes, fileBytes.Length);
#else
        userPath = StandaloneFileBrowser.SaveFilePanel("Save Recipe", "", "recipe.pdf", recipeFilter);
#endif

        if (!string.IsNullOrEmpty(userPath))
            File.Copy(sourcePath, userPath, true);
        else
            Debug.LogError("userPath is null or empty");
    }
}