using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours.Patcher;

class PatchBlocker : MonoBehaviour
{
    public enum Type
    {
        WALL,
        FLOOR
    }

    public Type type;
    public string pdBool;

    public void Start()
    {
    }
}