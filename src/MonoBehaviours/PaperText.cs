using System;
using System.Collections;
using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours;

public class PaperText : MonoBehaviour
{
    public bool DisplayUsername = false;
    public UnityEngine.UI.Text TextComponent;
    public bool FadeOutWhenPdBoolTrue;
    public string PdBoolName;

    private SpriteRenderer sr;
    private Color srColor;
    private Color textColor;
    private IEnumerator fadeOutCoroutine;

    private void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        srColor = sr.color;
        textColor = TextComponent.color;
        if (UIManager.instance != null)
        {
            if (UIManager.instance.saveProfileTitle != null)
            {
                if (UIManager.instance.saveProfileTitle.GetComponent<UnityEngine.UI.Text>() != null)
                {
                    TextComponent.font = UIManager.instance.saveProfileTitle.GetComponent<UnityEngine.UI.Text>().font;
                }
            }
        }
        if (DisplayUsername)
        {
            TextComponent.text = Language.Language.Get("SOFHKP_CH2_USERNAME", "UI");
        }
        if (FadeOutWhenPdBoolTrue && PlayerData.instance.GetBool(PdBoolName))
        {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!FadeOutWhenPdBoolTrue)
        {
            return;
        }
        if (PlayerData.instance.GetBool(PdBoolName) && fadeOutCoroutine == null)
        {
            fadeOutCoroutine = FadeOut();
            StartCoroutine(fadeOutCoroutine);
        }
    }

    private IEnumerator FadeOut()
    {
        while (srColor.a > 0 || TextComponent.color.a > 0)
        {
            srColor.a = Mathf.Clamp(srColor.a - Time.deltaTime,0.0f, 1.0f);
            textColor.a = Mathf.Clamp(textColor.a - Time.deltaTime,0.0f, 1.0f);
            sr.color = srColor;
            TextComponent.color = textColor;
            yield return null;
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