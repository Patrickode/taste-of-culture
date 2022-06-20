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

    /// <summary>
    /// See <c>StandaloneFileBrowser/Plugins/StandaloneFileBrowser.jslib</c> for this function's definition.<br/>
    /// It basically just does what <see cref="StandaloneFileBrowser.SaveFilePanel(string, string, string, ExtensionFilter)"/>
    /// does, but in JS, because WebGL shenanigans, or something.
    /// </summary>
    /// <param name="filename">The default name of the file to be downloaded.</param>
    /// <param name="byteArray">The thing to download in the form of a byte array.</param>
    /// <param name="byteArraySize">The length of <paramref name="byteArray"/>.</param>
    [DllImport("__Internal")]
    private static extern void DownloadFileImmediate(string filename, byte[] byteArray, int byteArraySize);
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
                DownloadFileImmediate(sourceFileName, fileBytes, fileBytes.Length);
            },
            request.SendWebRequest());
#else
        string userPath = StandaloneFileBrowser.SaveFilePanel(
            "Save Recipe", "",
            sourceFileName.Split('.')[0],
            recipeFilter);

        if (!string.IsNullOrEmpty(userPath))
            File.Copy(sourcePath, userPath, true);
#endif
    }
}