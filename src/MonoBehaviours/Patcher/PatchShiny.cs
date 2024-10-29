using System.Collections.Generic;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using UnityEngine;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours.Patcher;

class PatchShiny : MonoBehaviour
{
    public int CharmId = 0;
    public int Type = 0;
    public bool Activated = false;
    public bool Charm = false;
    public bool DashCloak = false;
    public bool ExitDream = false;
    public bool FlingL = false;
    public bool FlingOnStart = false;
    public bool Journal = false;
    public bool KingsBrand = false;
    public bool MantisClaw = false;
    public bool PureSeed = false;
    public bool Quake = false;
    public bool ShowCharmTute = false;
    public bool SlugFling = false;
    public bool SuperDash = false;
    public string ItemName = "";
    public string PdBoolName = "";

    public Sprite ItemSprite;

    public void Start()
    {
        var shinyParent = Instantiate(PrefabHolder.ShinyPrefab);
        shinyParent.name = "Map";
        shinyParent.SetActive(false);
        shinyParent.transform.GetChild(0).gameObject.SetActive(true);
        shinyParent.transform.position = transform.position;

        var shinyFsm = shinyParent.transform.GetChild(0).gameObject.LocateMyFSM("Shiny Control");
        var shinyFsmVars = shinyFsm.FsmVariables;
        shinyFsmVars.FindFsmInt("Charm ID").Value = CharmId;
        shinyFsmVars.FindFsmInt("Type").Value = Type;
        shinyFsmVars.FindFsmBool("Activated").Value = Activated;
        shinyFsmVars.FindFsmBool("Charm").Value = Charm;
        shinyFsmVars.FindFsmBool("Dash Cloak").Value = DashCloak;
        shinyFsmVars.FindFsmBool("Exit Dream").Value = ExitDream;
        shinyFsmVars.FindFsmBool("Fling L").Value = FlingL;
        shinyFsmVars.FindFsmBool("Fling On Start").Value = FlingOnStart;
        shinyFsmVars.FindFsmBool("Journal").Value = Journal;
        shinyFsmVars.FindFsmBool("King's Brand").Value = KingsBrand;
        shinyFsmVars.FindFsmBool("Mantis Claw").Value = MantisClaw;
        shinyFsmVars.FindFsmBool("Pure Seed").Value = PureSeed;
        shinyFsmVars.FindFsmBool("Quake").Value = Quake;
        shinyFsmVars.FindFsmBool("Show Charm Tute").Value = ShowCharmTute;
        shinyFsmVars.FindFsmBool("Slug Fling").Value = SlugFling;
        shinyFsmVars.FindFsmBool("Super Dash").Value = SuperDash;
        shinyFsmVars.FindFsmString("Item Name").Value = ItemName;
        shinyFsmVars.FindFsmString("PD Bool Name").Value = PdBoolName;

        var isAction = shinyFsm.GetAction<IntSwitch>("Trinket Type", 0);
        var tmpCompareTo = new List<FsmInt>(isAction.compareTo);
        tmpCompareTo.Add(tmpCompareTo.Count + 1);
        isAction.compareTo = tmpCompareTo.ToArray();
        shinyFsmVars.FindFsmInt("Trinket Num").Value = tmpCompareTo.Count;
        var tmpSendEvent = new List<FsmEvent>(isAction.sendEvent) { FsmEvent.FindEvent("PURE SEED") };
        isAction.sendEvent = tmpSendEvent.ToArray();

        shinyFsm.CopyState("Love Key", "_CustomState");

        shinyFsm.GetAction<SetPlayerDataBool>("_CustomState", 0).boolName = shinyFsmVars.FindFsmString("PD Bool Name");
        shinyFsm.GetAction<SetSpriteRendererSprite>("_CustomState", 1).sprite = ItemSprite;
        shinyFsm.GetAction<GetLanguageString>("_CustomState", 2).convName = ItemName;
        shinyFsm.RemoveAction("_CustomState", 4);

        shinyFsm.AddTransition("Trinket Type", "PURE SEED", "_CustomState");

        shinyParent.SetActive(true);
    }
}