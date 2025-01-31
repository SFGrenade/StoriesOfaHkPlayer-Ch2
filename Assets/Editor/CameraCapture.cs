using System.IO;
using UnityEngine;
using UnityEditor;

public class CameraCapture : MonoBehaviour
{
    protected static int fileCounter;

    [MenuItem("Camera/Render View")]
    static public void Capture()
    {
        int width = 1920 * 2 * 2;
        int height = 1080 * 2 * 2;

        // Create a render texture
        RenderTexture renderOnThis = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture origActiveRenderTexture = RenderTexture.active;

        RenderTexture.active = renderOnThis;

        // Clear the render texture before rendering cameras
        GL.Clear(true, true, Color.clear);

        // Get all cameras and sort by depth (ascending order)
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        System.Array.Sort(cameras, (a, b) => a.depth.CompareTo(b.depth));

        // Render each camera on top of the previous ones
        foreach (var camera in cameras)
        {
            RenderTexture origTargetTexture = camera.targetTexture;
            camera.targetTexture = renderOnThis;
            camera.Render();
            camera.targetTexture = origTargetTexture;
        }

        // Read pixels from render texture
        Texture2D image = new Texture2D(renderOnThis.width, renderOnThis.height, TextureFormat.ARGB32, false);
        image.ReadPixels(new Rect(0, 0, renderOnThis.width, renderOnThis.height), 0, 0);
        image.Apply();

        // Restore original active render texture
        RenderTexture.active = origActiveRenderTexture;
        renderOnThis.Release();

        // Save as PNG
        byte[] bytes = image.EncodeToPNG();
        DestroyImmediate(image);

        // Cleanup
        DestroyImmediate(renderOnThis);

        // Ensure directory exists
        string directory = "Screenshots";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes($"{directory}/{fileCounter}.png", bytes);
        fileCounter++;
    }
}
