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
}