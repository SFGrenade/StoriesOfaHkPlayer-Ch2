using Modding;
using SFCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UObject = UnityEngine.Object;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Modding.Patches;
using Modding.Utils;
using Newtonsoft.Json;
using SFCore.Generics;
using SFCore.Utils;
using UnityEngine.UI;

namespace StoriesOfaHkPlayer_Ch2;

public class StoriesOfaHkPlayer_Ch2 : SaveSettingsMod<SettingsClass>
{
    public static StoriesOfaHkPlayer_Ch2 Instance;

    public LanguageStrings LangStrings { get; private set; }

    private AssetBundle _abAssets = null;
    private AssetBundle _abScenes = null;

    public override string GetVersion() => Util.GetVersion(Assembly.GetExecutingAssembly());

    public override List<ValueTuple<string, string>> GetPreloadNames()
    {
        return new List<ValueTuple<string, string>>
        {
            new ValueTuple<string, string>("White_Palace_18", "White Palace Fly"),
            new ValueTuple<string, string>("White_Palace_18", "saw_collection/wp_saw"),
            new ValueTuple<string, string>("White_Palace_18", "saw_collection/wp_saw (2)"),
            new ValueTuple<string, string>("White_Palace_18", "Soul Totem white_Infinte"),
            new ValueTuple<string, string>("White_Palace_18", "Area Title Controller"),
            new ValueTuple<string, string>("White_Palace_18", "glow response lore 1/Glow Response Object (11)"),
            new ValueTuple<string, string>("White_Palace_18", "_SceneManager"),
            new ValueTuple<string, string>("White_Palace_18", "Inspect Region"),
            new ValueTuple<string, string>("White_Palace_18", "_Managers/PlayMaker Unity 2D"),
            new ValueTuple<string, string>("White_Palace_18", "Music Region (1)"),
            new ValueTuple<string, string>("White_Palace_17", "WP Lever"),
            new ValueTuple<string, string>("White_Palace_17", "White_ Spikes"),
            new ValueTuple<string, string>("White_Palace_17", "Cave Spikes Invis"),
            new ValueTuple<string, string>("White_Palace_09", "Quake Floor"),
            new ValueTuple<string, string>("Grimm_Divine", "Charm Holder"),
            new ValueTuple<string, string>("White_Palace_03_hub", "WhiteBench"),
            new ValueTuple<string, string>("Crossroads_07", "Breakable Wall_Silhouette"),
            new ValueTuple<string, string>("Deepnest_East_Hornet_boss", "Hornet Outskirts Battle Encounter"),
            new ValueTuple<string, string>("Deepnest_East_Hornet_boss", "Hornet Boss 2"),
            new ValueTuple<string, string>("White_Palace_03_hub", "door1"),
            new ValueTuple<string, string>("White_Palace_03_hub", "Dream Entry")
        };
    }

    private void LoadAssetbundles()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        if (_abAssets == null)
        {
            using Stream s = asm.GetManifestResourceStream("StoriesOfaHkPlayer_Ch2.Resources.storiesofahkplayer_ch2");
            if (s != null)
            {
                _abAssets = AssetBundle.LoadFromStream(s);
            }
        }

        if (_abScenes == null)
        {
            using Stream s = asm.GetManifestResourceStream("StoriesOfaHkPlayer_Ch2.Resources.storiesofahkplayer_ch2_scenes");
            if (s != null)
            {
                _abScenes = AssetBundle.LoadFromStream(s);
            }
        }
    }

    public StoriesOfaHkPlayer_Ch2() : base("Stories of a HK player - Chapter 2")
    {
        LoadAssetbundles();

        LangStrings = new LanguageStrings(Assembly.GetExecutingAssembly(), "StoriesOfaHkPlayer_Ch2.Resources.Language.json", Encoding.UTF8);

        InitCallbacks();
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        Log("Initializing");
        Instance = this;

        PrefabHolder.Preloaded(preloadedObjects);

        var tmpStyle = MenuStyles.Instance.styles.First(x => x.styleObject.name.Contains("StoriesOfaHkPlayer_Ch2 Style"));
        MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.ToList().IndexOf(tmpStyle), false, false);

        GameManager.instance.StartCoroutine(WaitForTitle());

        Log("Initialized");
    }

    private void InitCallbacks()
    {
        MenuStyleHelper.AddMenuStyleHook += AddMenuStyle;
        On.AudioManager.ApplyMusicCue += LoadMenuMusic;

        ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
        ModHooks.SetPlayerBoolHook += OnSetPlayerBoolHook;
        ModHooks.GetPlayerIntHook += OnGetPlayerIntHook;
        ModHooks.SetPlayerIntHook += OnSetPlayerIntHook;
        ModHooks.ApplicationQuitHook += SaveGlobalSettings;
        ModHooks.LanguageGetHook += OnLanguageGetHook;

        ChangeMainMenuStartGameButton();
    }

    private IEnumerator WaitForTitle()
    {
        yield return new WaitUntil(() => GameObject.Find("LogoTitle") != null);
        UIManager.EditMenus += GiveUiTextOutline;
    }

    private void GiveUiTextOutline()
    {
        foreach(var item in UIManager.instance.gameObject.GetComponentsInChildren<Text>(true))
        {
            var outline = item.gameObject.GetOrAddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }
        UIManager.EditMenus -= GiveUiTextOutline;
    }

    #region Menu Style stuff

    private void LoadMenuMusic(On.AudioManager.orig_ApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime,
        bool applySnapshot)
    {
        // Insert Custom Audio into main MusicCue
        var infos = (MusicCue.MusicChannelInfo[])musicCue.GetType().GetField("channelInfos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(musicCue);

        var origAudio = infos[0].GetAttr<MusicCue.MusicChannelInfo, AudioClip>("clip");
        if (origAudio != null && origAudio.name.Equals("Title"))
        {
            infos[(int)MusicChannels.Tension] = new MusicCue.MusicChannelInfo();
            infos[(int)MusicChannels.Tension].SetAttr("clip", _abAssets.LoadAsset<AudioClip>("Ch2-Menu-Music-Wrapped"));
            // Don't sync this audio with the not-as-long normal main menu theme
            infos[(int)MusicChannels.Tension].SetAttr("sync", MusicChannelSync.ExplicitOff);
        }

        musicCue.GetType().GetField("channelInfos", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(musicCue, infos);
        orig(self, musicCue, delayTime, transitionTime, applySnapshot);
    }

    private (string languageString, GameObject styleGo, int titleIndex, string unlockKey, string[] achievementKeys, MenuStyles.MenuStyle.CameraCurves
        cameraCurves, AudioMixerSnapshot musicSnapshot) AddMenuStyle(MenuStyles self)
    {
        GameObject menuStyleGo = GameObject.Instantiate(_abAssets.LoadAsset<GameObject>("MenuStyle"));
        menuStyleGo.name = "StoriesOfaHkPlayer_Ch2 Style";
        menuStyleGo.transform.position = new Vector3(0, 0, 0);
        menuStyleGo.transform.SetParent(self.transform, true);
        UnityEngine.Object.DontDestroyOnLoad(menuStyleGo);
        menuStyleGo.SetActive(false);

        menuStyleGo.Find("Canvas").GetComponent<Canvas>().worldCamera = GameCameras.instance.mainCamera;
        menuStyleGo.Find("Wind-Effect-1").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader =
            Shader.Find(menuStyleGo.Find("Wind-Effect-1").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader.name);
        menuStyleGo.Find("Wind-Effect-2").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader =
            Shader.Find(menuStyleGo.Find("Wind-Effect-2").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader.name);

        foreach (MenuStyles.MenuStyle style in self.styles)
        {
            if (style.displayName == "UI_MENU_STYLE_CLASSIC")
            {
                GameObject.Instantiate(style.styleObject.Find("Audio Player Actor 2D"), menuStyleGo.transform, true);
                GameObject.Instantiate(style.styleObject.Find("Audio Player Actor 2D (1)"), menuStyleGo.transform, true);
                break;
            }
        }

        var cameraCurves = new MenuStyles.MenuStyle.CameraCurves();
        cameraCurves.saturation = 1f;
        cameraCurves.redChannel = new AnimationCurve();
        cameraCurves.redChannel.AddKey(new Keyframe(0f, 0f));
        cameraCurves.redChannel.AddKey(new Keyframe(1f, 1f));
        cameraCurves.greenChannel = new AnimationCurve();
        cameraCurves.greenChannel.AddKey(new Keyframe(0f, 0f));
        cameraCurves.greenChannel.AddKey(new Keyframe(1f, 1f));
        cameraCurves.blueChannel = new AnimationCurve();
        cameraCurves.blueChannel.AddKey(new Keyframe(0f, 0f));
        cameraCurves.blueChannel.AddKey(new Keyframe(1f, 1f));

        menuStyleGo.SetActive(true);
        return ("UI_MENU_STYLE_STORIESOFAHKPLAYER_CH2", menuStyleGo, -1, "", null, cameraCurves,
            Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music").FindSnapshot("Tension Only"));
    }

    #endregion Menu Style stuff

    #region Main Menu start button stuff

    private void ChangeMainMenuStartGameButton()
    {
        On.UIManager.UIMainStartGame += UIManagerOnUIMainStartGame;
        On.UIManager.StartNewGame += UIManagerOnStartNewGame;
        On.GameManager.LoadGame += GameManagerOnLoadGame;
        On.DesktopPlatform.ReadSaveSlot += DesktopPlatformOnReadSaveSlot;
    }

    private void UIManagerOnUIMainStartGame(On.UIManager.orig_UIMainStartGame orig, UIManager self)
    {
        self.GetAttr<UIManager, GameManager>("gm").profileID = -1;
        self.StartNewGame(false, false);
    }

    private void UIManagerOnStartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
    {
        if (self.GetAttr<UIManager, GameManager>("gm").profileID == -1)
        {
            self.SetAttr<UIManager, bool>("permaDeath", permaDeath);
            self.SetAttr<UIManager, bool>("bossRush", bossRush);
            self.GetAttr<UIManager, InputHandler>("ih").StopUIInput();
            self.GetAttr<UIManager, MenuAudioController>("uiAudioPlayer").PlayStartGame();

            self.GetAttr<UIManager, InputHandler>("ih").StartUIInput();

            self.GetAttr<UIManager, GameManager>("gm").LoadGameFromUI(self.GetAttr<UIManager, GameManager>("gm").profileID);
            self.GetAttr<UIManager, InputHandler>("ih").StopUIInput();
            self.GetAttr<UIManager, MenuAudioController>("uiAudioPlayer").PlayStartGame();
            MenuStyles.Instance.StopAudio();
            self.StartCoroutine(self.HideCurrentMenu());
        }
        else
        {
            orig(self, permaDeath, bossRush);
        }
    }

    private void GameManagerOnLoadGame(On.GameManager.orig_LoadGame orig, GameManager self, int saveSlot, Action<bool> callback)
    {
        bool origValue = self.gameConfig.useSaveEncryption;
        if (saveSlot == -1)
        {
            self.gameConfig.useSaveEncryption = false;
        }

        orig(self, saveSlot, callback);

        if (saveSlot == -1)
        {
            self.gameConfig.useSaveEncryption = origValue;
        }
    }

    private void DesktopPlatformOnReadSaveSlot(On.DesktopPlatform.orig_ReadSaveSlot orig, DesktopPlatform self, int slotIndex, Action<byte[]> callback)
    {
        if (slotIndex == -1)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using Stream s = asm.GetManifestResourceStream("StoriesOfaHkPlayer_Ch2.Resources.Save.json");

            using MemoryStream ms = new MemoryStream();
            s.CopyTo(ms);

            callback(ms.ToArray());
        }
        else
        {
            orig(self, slotIndex, callback);
        }
    }

    #endregion Main Menu start button stuff

    #region Hooks

    private string OnLanguageGetHook(string key, string sheet, string orig)
    {
        string ret = orig;
        if (LangStrings.ContainsKey(key, sheet))
        {
            ret = LangStrings.Get(key, sheet);
            ret = ret.Replace("{USERNAME}", Environment.UserName.ToUpperInvariant());
        }

        return ret;
    }

    private bool OnGetPlayerBoolHook(string target, bool orig)
    {
        var tmpField = ReflectionHelper.GetFieldInfo(typeof(SettingsClass), target);
        if (tmpField != null)
        {
            return (bool)tmpField.GetValue(SaveSettings);
        }

        if (target == "alwaysFalse")
        {
            return false;
        }

        return orig;
    }

    private bool OnSetPlayerBoolHook(string target, bool orig)
    {
        var tmpField = ReflectionHelper.GetFieldInfo(typeof(SettingsClass), target);
        if (tmpField != null)
        {
            tmpField.SetValue(SaveSettings, orig);
        }

        return orig;
    }

    private int OnGetPlayerIntHook(string target, int orig)
    {
        var tmpField = ReflectionHelper.GetFieldInfo(typeof(SettingsClass), target);
        if (tmpField != null)
        {
            return (int)tmpField.GetValue(SaveSettings);
        }

        return orig;
    }

    private int OnSetPlayerIntHook(string target, int orig)
    {
        var tmpField = ReflectionHelper.GetFieldInfo(typeof(SettingsClass), target);
        if (tmpField != null)
        {
            tmpField.SetValue(SaveSettings, orig);
        }

        return orig;
    }

    #endregion Hooks

    private void DebugLogGo(GameObject go, string indent = "")
    {
        Log($"{indent}- GameObject '{go.name}' ({go.activeSelf}/{go.activeInHierarchy})");
        foreach (Component component in go.GetComponents<Component>())
        {
            Log($"{indent}  - Component '{component.GetType()}'");
        }

        for (int i = 0; i < go.transform.childCount; i++)
        {
            DebugLogGo(go.transform.GetChild(i).gameObject, $"{indent}  ");
        }
    }
}