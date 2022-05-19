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
    private byte[] fileBytes = null;

    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Called by browser/DownloadFile, apparently the reason that DownloadFile needs a gameObjectName as a parameter
    public void OnFileDownload() => Debug.Log("File successfully downloaded");
#endif

    public void Download(string sourceFileName)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, sourceFileName);

#if UNITY_WEBGL && !UNITY_EDITOR
        var request = UnityEngine.Networking.UnityWebRequest.Get(sourcePath);
        Coroutilities.DoAfter(this,
            () =>
            {
                fileBytes = request.downloadHandler.data;
                DownloadFile(gameObject.name, "OnFileDownload", sourceFileName, fileBytes, fileBytes.Length);
            },
            request.SendWebRequest());
#else
        string userPath = StandaloneFileBrowser.SaveFilePanel(
            "Save Recipe", "",
            sourceFileName.Split('.')[0],
            recipeFilter);

        if (!string.IsNullOrEmpty(userPath))
            File.Copy(sourcePath, userPath, true);
        else
            Debug.LogError("userPath is null or empty");
#endif
    }
}