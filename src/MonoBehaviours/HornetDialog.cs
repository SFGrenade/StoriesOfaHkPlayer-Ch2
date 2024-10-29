using JetBrains.Annotations;
using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours;

[UsedImplicitly]
public class HornetDialog : MonoBehaviour
{
    [UsedImplicitly]
    public string EncounterPdBoolName = "SOFHKP_CH2_TALK_TO_HORNET_1";
    [UsedImplicitly]
    public string Dialogue1ConvoSheet = "Hornet";
    [UsedImplicitly]
    public string Dialogue1ConvoKey = "";
    [UsedImplicitly]
    public string Dialogue2ConvoSheet = "Hornet";
    [UsedImplicitly]
    public string Dialogue2ConvoKey = "";

    public void Start()
    {
    }

    private void Log(string message)
    {
    }

    private void Log(object message)
    {
    }
}