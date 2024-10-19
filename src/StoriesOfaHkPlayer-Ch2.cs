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
using System.Text;
using SFCore.Utils;

namespace StoriesOfaHkPlayer_Ch2;

public class StoriesOfaHkPlayer_Ch2 : Mod
{
    public static StoriesOfaHkPlayer_Ch2 Instance;

    public LanguageStrings LangStrings { get; private set; }

    private AssetBundle _abAssets = null;
    private AssetBundle _abScenes = null;

    public override string GetVersion() => Util.GetVersion(Assembly.GetExecutingAssembly());

    public override List<ValueTuple<string, string>> GetPreloadNames()
    {
        return new List<(string, string)>()
        {
            ("Room_shop", "_SceneManager")
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

    public override void Initialize()
    {
        Log("Initializing");
        Instance = this;

        var tmpStyle = MenuStyles.Instance.styles.First(x => x.styleObject.name.Contains("StoriesOfaHkPlayer_Ch2 Style"));
        MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.ToList().IndexOf(tmpStyle), false, false);

        Log("Initialized");
    }

    private void InitCallbacks()
    {
        MenuStyleHelper.AddMenuStyleHook += AddMenuStyle;
        On.AudioManager.ApplyMusicCue += LoadMenuMusic;

        ModHooks.LanguageGetHook += OnLanguageGetHook;
    }

    private void LoadMenuMusic(On.AudioManager.orig_ApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
    {
        // Insert Custom Audio into main MusicCue
        var infos = (MusicCue.MusicChannelInfo[]) musicCue.GetType().GetField("channelInfos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(musicCue);
            
        var origAudio = infos[0].GetAttr<MusicCue.MusicChannelInfo, AudioClip>("clip");
        if (origAudio != null && origAudio.name.Equals("Title"))
        {
            infos[(int) MusicChannels.Tension] = new MusicCue.MusicChannelInfo();
            infos[(int) MusicChannels.Tension].SetAttr("clip", _abAssets.LoadAsset<AudioClip>("Ch2-Menu-Music-Wrapped"));
            // Don't sync this audio with the not-as-long normal main menu theme
            infos[(int) MusicChannels.Tension].SetAttr("sync", MusicChannelSync.ExplicitOff);
        }
        musicCue.GetType().GetField("channelInfos", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(musicCue, infos);
        orig(self, musicCue, delayTime, transitionTime, applySnapshot);
    }

    private (string languageString, GameObject styleGo, int titleIndex, string unlockKey, string[] achievementKeys, MenuStyles.MenuStyle.CameraCurves cameraCurves, AudioMixerSnapshot musicSnapshot) AddMenuStyle(MenuStyles self)
    {
        GameObject menuStyleGo = GameObject.Instantiate(_abAssets.LoadAsset<GameObject>("MenuStyle"));
        menuStyleGo.name = "StoriesOfaHkPlayer_Ch2 Style";
        menuStyleGo.transform.position = new Vector3(0, 0, 0);
        menuStyleGo.transform.SetParent(self.transform, true);
        UnityEngine.Object.DontDestroyOnLoad(menuStyleGo);
        menuStyleGo.SetActive(false);

        menuStyleGo.Find("Canvas").GetComponent<Canvas>().worldCamera = GameCameras.instance.mainCamera;
        menuStyleGo.Find("Wind-Effect-1").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader = Shader.Find(menuStyleGo.Find("Wind-Effect-1").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader.name);
        menuStyleGo.Find("Wind-Effect-2").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader = Shader.Find(menuStyleGo.Find("Wind-Effect-2").GetComponent<ParticleSystemRenderer>().sharedMaterial.shader.name);

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
        return ("UI_MENU_STYLE_STORIESOFAHKPLAYER_CH2", menuStyleGo, -1, "", null, cameraCurves, Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music").FindSnapshot("Tension Only"));
    }

    private string OnLanguageGetHook(string key, string sheet, string orig)
    {
        Log($"Sheet: {sheet}; Key: {key}");
        string ret = orig;
        if (LangStrings.ContainsKey(key, sheet))
        {
            ret = LangStrings.Get(key, sheet);
        }
        if (sheet == "UI" && key == "SOFHKP-CH2-USERNAME")
        {
            ret = Environment.UserName.ToUpperInvariant();
        }
        Log($"=> {ret}");
        return ret;
    }

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