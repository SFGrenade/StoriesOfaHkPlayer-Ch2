using System;
using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours;

public class PaperText : MonoBehaviour
{
    public bool DisplayText = false;
    public UnityEngine.UI.Text TextComponent;

    private void Start()
    {
        TextComponent.font = UIManager.instance.saveProfileTitle.GetComponent<UnityEngine.UI.Text>().font;
        if (!DisplayText)
        {
            TextComponent.text = Language.Language.Get("SOFHKP-CH2-USERNAME", "UI");
        }
    }

    private static void Log(string message)
    {
        Modding.Logger.Log($"[StoriesOfaHkPlayer_Ch2][MonoBehaviours][PaperText] - {message}");
    }
    private static void Log(object message)
    {
        Log($"{message}");
    }
}