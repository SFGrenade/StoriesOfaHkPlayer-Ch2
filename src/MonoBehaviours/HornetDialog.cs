using System;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using SFCore.MonoBehaviours;
using UnityEngine;
using Logger = Modding.Logger;
using SFCore.Utils;
using UnityEngine.Audio;

namespace StoriesOfaHkPlayer_Ch2.MonoBehaviours;

public class HornetDialog : MonoBehaviour
{
    public string EncounterPdBoolName = "SOFHKP_CH2_TALK_TO_HORNET_1";
    public string Dialogue1ConvoSheet = "Hornet";
    public string Dialogue1ConvoKey = "";
    public string Dialogue2ConvoSheet = "Hornet";
    public string Dialogue2ConvoKey = "";

    private GameObject beforeFightPrefab;

    public void FixedUpdate()
    {
        Vector3 origLocalScale = transform.localScale;

        if (HeroController.instance.transform.position.x < transform.position.x)
        {
            origLocalScale.x = -1f;
        }
        else
        {
            origLocalScale.x = 1f;
        }

        transform.localScale = origLocalScale;
        beforeFightPrefab.transform.localScale = origLocalScale;
    }

    public void Start()
    {
        beforeFightPrefab = Instantiate(PrefabHolder.Hornet2BossEncounterPrefab);
        beforeFightPrefab.SetActive(false);
        beforeFightPrefab.transform.position = transform.position;
        //beforeFightPrefab.SetActive(true); // DEBUG
        beforeFightPrefab.transform.localScale.Scale(transform.localScale);


        beforeFightPrefab.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 8f);
        beforeFightPrefab.GetComponent<BoxCollider2D>().size = new Vector2(6f, 22f);

        var bfpDIPT = beforeFightPrefab.AddComponent<DeactivateIfPlayerdataTrue>();
        bfpDIPT.boolName = EncounterPdBoolName;

        var encounterFsm = beforeFightPrefab.LocateMyFSM("Encounter");
        if (encounterFsm.FsmStates[0].Fsm == null)
        {
            encounterFsm.Preprocess();
        }
        //encounterFsm.RemoveAction("Init", 1);
        encounterFsm.GetAction<PlayerDataBoolTest>("Init", 1).boolName = EncounterPdBoolName;
        encounterFsm.RemoveTransition("Init", "DESTROY");

        encounterFsm.RemoveAction("Point", 2);
        encounterFsm.AddMethod("Point", () =>
        {
            this.gameObject.SetActive(false);
        });

        var dialogueVars1 = new[] { new HutongGames.PlayMaker.FsmVar(typeof(string)), new HutongGames.PlayMaker.FsmVar(typeof(string)) };
        dialogueVars1[0].SetValue(Dialogue1ConvoKey);
        dialogueVars1[1].SetValue(Dialogue1ConvoSheet);
        encounterFsm.GetAction<CallMethodProper>("Dialogue", 1).parameters = dialogueVars1;

        encounterFsm.RemoveAction("Blizzard Start", 6);
        encounterFsm.RemoveAction("Blizzard Start", 5);
        encounterFsm.RemoveAction("Blizzard Start", 4);
        encounterFsm.GetAction<Wait>("Blizzard Start", 3).time = 1f;
        // encounterFsm.RemoveAction("Blizzard Start", 2);
        // encounterFsm.RemoveAction("Blizzard Start", 1);
        // encounterFsm.RemoveAction("Blizzard Start", 0);

        var dialogueVars2 = new[] { new HutongGames.PlayMaker.FsmVar(typeof(string)), new HutongGames.PlayMaker.FsmVar(typeof(string)) };
        dialogueVars2[0].SetValue(Dialogue2ConvoKey);
        dialogueVars2[1].SetValue(Dialogue2ConvoSheet);
        encounterFsm.GetAction<CallMethodProper>("Dialogue 2", 0).parameters = dialogueVars2;

        encounterFsm.GetAction<SetPlayerDataBool>("Start Fight", 0).boolName = EncounterPdBoolName;
        encounterFsm.RemoveAction("Start Fight", 4);
        encounterFsm.RemoveAction("Start Fight", 3);
        encounterFsm.RemoveAction("Start Fight", 2);
        encounterFsm.RemoveAction("Start Fight", 1);

        encounterFsm.SetState(encounterFsm.Fsm.StartState);
        //encounterFsm.MakeLog();
        beforeFightPrefab.SetActive(true);
    }

    private void Log(string message)
    {
        Logger.Log($"[{GetType().FullName?.Replace(".", "]:[")}] - {message}");
    }

    private void Log(object message)
    {
        Logger.Log($"[{GetType().FullName?.Replace(".", "]:[")}] - {message}");
    }
}