using UnityEngine.Rendering;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class CustomToolbarButtonWindow : EditorWindow
{
    private Texture sourceTexture;
    private string filePath;
    private int width = 256;
    private int height = 256;
    private SaveTextureToFileUtility.SaveTextureFileFormat fileFormat = SaveTextureToFileUtility.SaveTextureFileFormat.PNG;
    private int jpgQuality = 95;
    private bool asynchronous = true;

    [MenuItem("Window/Custom Toolbar Button")]
    static void Init()
    {
        CustomToolbarButtonWindow window = (CustomToolbarButtonWindow)EditorWindow.GetWindow(typeof(CustomToolbarButtonWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Save Texture to File", EditorStyles.boldLabel);
        sourceTexture = EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture), false) as Texture;
        filePath = EditorGUILayout.TextField("File Path", filePath);
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        fileFormat = (SaveTextureToFileUtility.SaveTextureFileFormat)EditorGUILayout.EnumPopup("File Format", fileFormat);
        if (fileFormat == SaveTextureToFileUtility.SaveTextureFileFormat.JPG)
        {
            jpgQuality = EditorGUILayout.IntSlider("JPG Quality", jpgQuality, 0, 100);
        }
        asynchronous = EditorGUILayout.Toggle("Asynchronous", asynchronous);

        if (GUILayout.Button("Save"))
        {
            SaveTexture();
        }
    }

    void SaveTexture()
    {
        if (sourceTexture == null || string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Source Texture or File Path is not specified!");
            return;
        }

        SaveTextureToFileUtility.SaveTextureToFile(sourceTexture, filePath, width, height, fileFormat, jpgQuality, asynchronous, (success) =>
        {
            if (success)
            {
                Debug.Log("Texture saved successfully!");
            }
            else
            {
                Debug.LogError("Failed to save texture!");
            }
        });
    }
}

public class SaveTextureToFileUtility
{
    public enum SaveTextureFileFormat
    {
        EXR, JPG, PNG, TGA
    }

    public static void SaveTextureToFile(Texture source,
                                          string filePath,
                                          int width,
                                          int height,
                                          SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG,
                                          int jpgQuality = 95,
                                          bool asynchronous = true,
                                          System.Action<bool> done = null)
    {
        if (!(source is Texture2D || source is RenderTexture))
        {
            done?.Invoke(false);
            return;
        }

        if (width < 0 || height < 0)
        {
            width = source.width;
            height = source.height;
        }

        var resizeRT = RenderTexture.GetTemporary(width, height, 0);
        Graphics.Blit(source, resizeRT);

        var narray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        var request = AsyncGPUReadback.RequestIntoNativeArray(ref narray, resizeRT, 0, (AsyncGPUReadbackRequest request) =>
        {
            if (!request.hasError)
            {
                NativeArray<byte> encoded;

                switch (fileFormat)
                {
                    case SaveTextureFileFormat.EXR:
                        encoded = ImageConversion.EncodeNativeArrayToEXR(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    case SaveTextureFileFormat.JPG:
                        encoded = ImageConversion.EncodeNativeArrayToJPG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0, jpgQuality);
                        break;
                    case SaveTextureFileFormat.TGA:
                        encoded = ImageConversion.EncodeNativeArrayToTGA(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    default:
                        encoded = ImageConversion.EncodeNativeArrayToPNG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                }

                System.IO.File.WriteAllBytes(filePath, encoded.ToArray());
                encoded.Dispose();
            }

            narray.Dispose();

            done?.Invoke(!request.hasError);
        });

        if (!asynchronous)
            request.WaitForCompletion();
    }
}
