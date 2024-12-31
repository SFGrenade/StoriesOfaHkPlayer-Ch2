using SFCore.Utils;
using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours.Patcher;

class PatchAreaTitleController : MonoBehaviour
{
    [Range(0, 10)]
    public float Pause = 3f;
    public bool AlwaysVisited = false;
    public bool DisplayRight = false;
    public bool OnlyOnRevisit = false;
    public bool SubArea = true;
    public bool WaitForTrigger = false;
    public string AreaEvent = "";
    public string VisitedBool = "";

    public void Awake()
    {
        GameObject atc = Instantiate(PrefabHolder.PopAreaTitleCtrlPrefab);
        atc.SetActive(false);
        atc.transform.localPosition = transform.position;
        atc.transform.localEulerAngles = transform.eulerAngles;
        atc.transform.localScale = transform.lossyScale;

        PlayMakerFSM atcFsm = atc.LocateMyFSM("Area Title Controller");
        atcFsm.GetFloatVariable("Unvisited Pause").Value = Pause;
        atcFsm.GetFloatVariable("Visited Pause").Value = Pause;

        atcFsm.GetBoolVariable("Always Visited").Value = AlwaysVisited;
        atcFsm.GetBoolVariable("Display Right").Value = DisplayRight;
        atcFsm.GetBoolVariable("Only On Revisit").Value = OnlyOnRevisit;
        atcFsm.GetBoolVariable("Sub Area").Value = SubArea;
        atcFsm.GetBoolVariable("Visited Area").Value = PlayerData.instance.GetBool(VisitedBool);
        atcFsm.GetBoolVariable("Wait for Trigger").Value = WaitForTrigger;

        atcFsm.GetStringVariable("Area Event").Value = AreaEvent;
        atcFsm.GetStringVariable("Visited Bool").Value = VisitedBool;

        atcFsm.GetGameObjectVariable("Area Title").Value = GameObject.Find("Area Title");

        atc.AddComponent<NonBouncer>();
        atc.SetActive(true);

        Destroy(gameObject);
    }
}