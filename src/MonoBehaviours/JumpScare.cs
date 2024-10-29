using System;
using System.Collections;
using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours;

public class JumpScare : MonoBehaviour
{
    public string PdBoolNameToListenFor = "";
    public string PdBoolNameIfDone = "";
    public SpriteRenderer Sprite;
    public AudioClip AudioClipToPlay;
    public float FadeInTime = 0.1f;
    public float FadeOutTime = 1.0f;
    public bool IsFinal = false;

    private Color srColor;
    private IEnumerator jumpScareCoroutine;
    private GameObject customAudioSource;

    private void Start()
    {
        srColor = Sprite.color;

        srColor.a = 0.0f;
        Sprite.color = srColor;

        if (PlayerData.instance.GetBool(PdBoolNameIfDone))
        {
            gameObject.SetActive(false);
            return;
        }
        customAudioSource = Instantiate(PrefabHolder.WpSawWithSoundPrefab, transform);
        customAudioSource.transform.localPosition = Vector3.zero;
        Destroy(customAudioSource.GetComponent<SpriteRenderer>());
        Destroy(customAudioSource.GetComponent<Animator>());
        Destroy(customAudioSource.GetComponent<CircleCollider2D>());
        Destroy(customAudioSource.GetComponent<BoxCollider2D>());
        Destroy(customAudioSource.GetComponent<DamageHero>());
        Destroy(customAudioSource.GetComponent<TinkEffect>());
        customAudioSource.GetComponent<AudioSource>().clip = null;
        customAudioSource.GetComponent<AudioSource>().loop = false;
        customAudioSource.GetComponent<AudioSource>().volume = 1.0f;
        customAudioSource.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (PlayerData.instance.GetBool(PdBoolNameToListenFor) && jumpScareCoroutine == null)
        {
            jumpScareCoroutine = JumpScareCoroutine();
            StartCoroutine(jumpScareCoroutine);
        }
    }

    private IEnumerator JumpScareCoroutine()
    {
        AudioClipToPlay.PlayOnSource(customAudioSource.GetComponent<AudioSource>());
        float currentTime = 0.0f;
        while (currentTime < FadeInTime)
        {
            currentTime += Time.deltaTime;
            srColor.a = Mathf.Lerp(0.0f, 1.0f, currentTime / FadeInTime);
            Sprite.color = srColor;
            yield return null;
        }
        // who knows what to do once we are fully faded in
        if (IsFinal)
        {
            // quit the game
            UIManager.instance.QuitGame();
            //Application.Quit();
            yield break;
        }
        currentTime = 0.0f;
        while (currentTime < FadeOutTime)
        {
            currentTime += Time.deltaTime;
            srColor.a = Mathf.Lerp(1.0f, 0.0f, currentTime / FadeOutTime);
            Sprite.color = srColor;
            yield return null;
        }

        PlayerData.instance.SetBool(PdBoolNameIfDone, true);
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