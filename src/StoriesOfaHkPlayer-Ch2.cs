using Modding;
using SFCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UObject = UnityEngine.Object;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine.Video;
using System.Linq;
using System.Text;
using SFCore.Utils;

namespace StoriesOfaHkPlayer_Ch2;

public class StoriesOfaHkPlayer_Ch2 : Mod
{
    public static StoriesOfaHkPlayer_Ch2 Instance;

    public LanguageStrings LangStrings { get; private set; }

    private AssetBundle _abTitleScreen = null;

    public override string GetVersion() => Util.GetVersion(Assembly.GetExecutingAssembly());

    public override List<ValueTuple<string, string>> GetPreloadNames()
    {
        return new List<(string, string)>()
        {
            ("Room_shop", "_SceneManager")
        };
    }

    public StoriesOfaHkPlayer_Ch2() : base("Stories of a HK player - Chapter 2")
    {
        LangStrings = new LanguageStrings(Assembly.GetExecutingAssembly(), "StoriesOfaHkPlayer_Ch2.Resources.Language.json", Encoding.UTF8);

        InitCallbacks();
    }

    public override void Initialize()
    {
        Log("Initializing");
        Instance = this;

        var tmpStyle = MenuStyles.Instance.styles.First(x => x.styleObject.name.Contains("StoriesOfaHkPlayer_Ch2 Style"));
        MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.ToList().IndexOf(tmpStyle), false);

        Log("Initialized");
    }

    private void InitCallbacks()
    {
        MenuStyleHelper.AddMenuStyleHook += AddMenuStyle;

        ModHooks.LanguageGetHook += OnLanguageGetHook;
    }

    private (string languageString, GameObject styleGo, int titleIndex, string unlockKey, string[] achievementKeys, MenuStyles.MenuStyle.CameraCurves cameraCurves, AudioMixerSnapshot musicSnapshot) AddMenuStyle(MenuStyles self)
    {
        GameObject menuStyleGo = new GameObject("StoriesOfaHkPlayer_Ch2 Style");
        menuStyleGo.SetActive(false);

        menuStyleGo.transform.SetParent(self.gameObject.transform);
        menuStyleGo.transform.localPosition = new Vector3(0, -1.2f, 0);

        #region Loading assetbundle

        if (_abTitleScreen == null)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream s = asm.GetManifestResourceStream("StoriesOfaHkPlayer_Ch2.Resources.storiesofahkplayer_ch2"))
            {
                if (s != null)
                {
                    _abTitleScreen = AssetBundle.LoadFromStream(s);
                }
            }
        }

        #endregion

        var aSource = menuStyleGo.AddComponent<AudioSource>();
        aSource.clip = null;
        aSource.outputAudioMixerGroup = self.styles[0].styleObject.Find("Audio Player Actor 2D (1)")
            .GetComponent<AudioSource>().outputAudioMixerGroup;
        aSource.mute = false;
        aSource.bypassEffects = false;
        aSource.bypassListenerEffects = false;
        aSource.bypassReverbZones = false;
        aSource.playOnAwake = true;
        aSource.loop = true;
        aSource.priority = 128;
        aSource.volume = 1;
        aSource.pitch = 1;
        aSource.panStereo = 0;
        aSource.spatialBlend = 0;
        aSource.reverbZoneMix = 1;
        aSource.dopplerLevel = 0;
        aSource.spread = 0;
        aSource.rolloffMode = AudioRolloffMode.Custom;
        aSource.maxDistance = 58.79711f;
        aSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(new []
        {
            new Keyframe(45.86174f, 1),
            new Keyframe(55.33846f, 0)
        }));
        var vp = menuStyleGo.AddComponent<VideoPlayer>();
        //vp.playOnAwake = false;
        vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
        vp.renderMode = VideoRenderMode.CameraFarPlane;
        vp.isLooping = true;
        vp.targetCamera = GameCameras.instance.mainCamera;
        vp.source = VideoSource.VideoClip;
        vp.clip = _abTitleScreen.LoadAsset<VideoClip>("StoriesOfaHkPlayer_Ch2");
        vp.SetTargetAudioSource(0, aSource);
        UObject.DontDestroyOnLoad(vp.clip);

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
        UObject.DontDestroyOnLoad(menuStyleGo);
        menuStyleGo.SetActive(true);
        //PrintDebug(menuStyleGo);
        return ("UI_MENU_STYLE_STORIESOFAHKPLAYER_CH2", menuStyleGo, -1, "", null, cameraCurves, Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music").FindSnapshot("Silent"));
    }

    private string OnLanguageGetHook(string key, string sheet, string orig)
    {
        //Log($"Sheet: {sheet}; Key: {key}");
        if (LangStrings.ContainsKey(key, sheet))
        {
            return LangStrings.Get(key, sheet);
        }
        return orig;
    }

    private static void SetInactive(GameObject go)
    {
        if (go == null) return;

        UnityEngine.Object.DontDestroyOnLoad(go);
        go.SetActive(false);
    }

    private static void SetInactive(UnityEngine.Object go)
    {
        if (go != null)
        {
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
    }
}