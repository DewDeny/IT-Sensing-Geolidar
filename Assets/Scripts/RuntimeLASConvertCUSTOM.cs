using pointcloudviewer.binaryviewer;
using System.Diagnostics;
using System.IO;
using System.Collections;
using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;
using SFB;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RuntimeLASConvertCUSTOM : MonoBehaviour
{
    public PointCloudViewerDX11 binaryViewerDX11;
    //public string lasFile = "runtime-example.las";

    // inside streaming assets
    [Tooltip("Place your downloaded converter in this folder, or set correct path here (relative to StreamingAssets or absolute path to outside project")]
    public string commandlinePath = "PointCloudConverterX64/PointCloudConverter.exe";

    [Header("Path readOnly")]
    public TMP_Text inputText;
    public TMP_Text outputText;
    public TMP_Text readyFileText;

    [HideInInspector]
    public bool isConverting = false;
    string inputFile, readyFile;
    string outputFolder;

    //miscellanous
    public GameObject[] customPanel, customButton;

    void Start()
    {
        inputFile = Path.Combine(Application.streamingAssetsPath, "LASFiles/TOWER_CROP_PROYEKSI_UTM_49S_crop.las");
        outputFolder = Path.Combine(Application.streamingAssetsPath, "ConversionResult/");

        if (inputFile != null)
            inputText.text = inputFile;

        if (outputFolder != null)
            outputText.text = outputFolder;

        //   StartConversion();
    }

    public void StartConversion()
    {
        isConverting = true;

        //INPUT FILE
        var sourceFile = inputFile;

        // check if full path or relative to streaming assets
        if (Path.IsPathRooted(sourceFile) == false)
        {
            sourceFile = Path.Combine(Application.streamingAssetsPath, sourceFile);
        }

        if (File.Exists(sourceFile))
        {
            Debug.Log("Converting file: " + sourceFile);
        }
        else
        {
            Debug.LogError("Input file missing: " + sourceFile);
            return;
        }

        //OUTPUT FOLDER
        //   outputPath = Path.GetDirectoryName(sourceFile); //ddd

        if (outputFolder == null)
        {
            outputFolder = Path.Combine(Application.streamingAssetsPath, "ConversionResult/"); //mmmmmmm
        }

        //CONVERTER'S EXECUTABLE
        var exePath = Path.Combine(Application.streamingAssetsPath, commandlinePath);

        // check if converter is available
        if (File.Exists(exePath) == false)
        {
            Debug.LogError("Missing standalone converter exe: " + exePath);
            Debug.Log("You can download it from https://github.com/unitycoder/PointCloudConverter/releases");
            return;
        }

        //PROCESS
        // NOTE should do this in separate thread, so no need to wait for conversion in mainthread..

        var process = new Process();
        process.StartInfo.FileName = exePath;
        // more params https://github.com/unitycoder/UnityPointCloudViewer/wiki/Commandline-Tools
        process.StartInfo.Arguments =
            "-input=" + sourceFile + " " +
            "-swap=true " +
            "-output=" + outputFolder + " " +
            "-exportformat=ucpc "; //mmmmmm
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //startInfo.WindowStyle = ProcessWindowStyle.Minimized;
        //var process = Process.Start(startInfo);
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += ConversionLog;
        process.ErrorDataReceived += ConversionLog;

        process.Exited += ConversionDone;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        //Debug.Log(startInfo.Arguments);
        Debug.Log("[RuntimeLASConvert] Conversion is running..");
    }

    private void ConversionLog(object sender, DataReceivedEventArgs e)
    {
        Debug.Log("<color=grey>[ConverterOutput] " + e.Data + "</color>");
    }

    void ConversionDone(object sender, System.EventArgs e)
    {
        isConverting = false;
        string basename = Path.GetFileNameWithoutExtension(inputFile);
        string outputFile = Path.Combine(outputFolder, basename + ".ucpc");

        // check if output exists
        if (File.Exists(outputFile))
        {
            Debug.Log("Reading output file: " + outputFile);
            binaryViewerDX11.CallReadPointCloudThreaded(outputFile);
        }
        else
        {
            Debug.LogError("File not found: " + outputFile);
        }
    }

    public void PickLASFile()
    {
#if UNITY_EDITOR
        inputFile = EditorUtility.OpenFilePanel(
            "Select LAS file",
            Application.streamingAssetsPath,
            "las"
        );
#else
        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Select LAS file",
            Application.streamingAssetsPath,
            "las",
            false
            );
        inputFile = paths[0];
#endif
        inputText.text = inputFile;
    }

    public void PickOutputFolder()
    {
#if UNITY_EDITOR
        outputFolder = EditorUtility.OpenFolderPanel(
            "Select output folder",
            Path.Combine(Application.streamingAssetsPath, "ConversionResult/"),
            ""
        );
#else
        var paths = StandaloneFileBrowser.OpenFolderPanel(
            "Select output folder",
            Path.Combine(Application.streamingAssetsPath, "ConversionResult/"),
            false
            );
        outputFolder = paths[0];           
#endif
        outputText.text = outputFolder;
    }

    //MISC FUNCTIONS

    public void PickFile() //Unity only, non-build
    {
#if UNITY_EDITOR
        readyFile = EditorUtility.OpenFilePanel(
            "Select LAS file",
            Application.streamingAssetsPath,
            "ucpc"
        );
#else
        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Select LAS file",
            Application.streamingAssetsPath,
            "ucpc",
            false
            );
        readyFile = paths[0];
#endif
        readyFileText.text = readyFile;
    }

    public void OpenFile()
    {
        binaryViewerDX11.CallReadPointCloudThreaded(readyFile);
    }

    public void OpenPanel(int thisBut)
    {
        if (!customPanel[thisBut].activeSelf)
        {
            int i = Mathf.Abs(thisBut - 1);
            customPanel[i].gameObject.SetActive(false);

            customPanel[thisBut].SetActive(true);
            customPanel[thisBut].transform.position = new Vector2(customButton[thisBut].transform.position.x, customPanel[thisBut].transform.position.y);
        }
        else
            customPanel[thisBut].SetActive(false);
    }
}