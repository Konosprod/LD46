using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<SettingsManager>();

                if(_instance == null)
                {
                    _instance = new GameObject("Spawned SettingsManager", typeof(SettingsManager)).GetComponent<SettingsManager>();
                }
            }

            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    private Resolution[] resolutions;
    public Slider sliderVolumeSFX;
    public Slider sliderVolumeMusic;
    public Dropdown dropdownResolutions;
    public Toggle toggleFullscreen;


    public GameSettings settings;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        settings = new GameSettings();

        resolutions = Screen.resolutions;
        toggleFullscreen.onValueChanged.AddListener(OnFullscreenChanged);
        dropdownResolutions.onValueChanged.AddListener(OnResolutionChanged);
        sliderVolumeMusic.onValueChanged.AddListener(OnMusicVolumeChanged);
        sliderVolumeSFX.onValueChanged.AddListener(OnSfxVolumeChanged);

        foreach (Resolution r in resolutions)
        {
            dropdownResolutions.options.Add(new Dropdown.OptionData(r.ToString()));
        }

        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load()
    {
        try
        {
            settings = JsonUtility.FromJson<GameSettings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

            toggleFullscreen.isOn = settings.fullScreen;
            dropdownResolutions.value = settings.resolution;
            sliderVolumeMusic.value = settings.musicVolume;
            sliderVolumeSFX.value = settings.sfxVolume;
        }
        catch(System.Exception e)
        {
            Debug.LogError(e);

            toggleFullscreen.isOn = false;
            dropdownResolutions.value = GetResolutionIndex();
            sliderVolumeMusic.value = 1f;
            sliderVolumeSFX.value = 1f;
            settings.firstRun = true;
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(settings, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", json);
    }

    private void OnResolutionChanged(int newValue)
    {
        Screen.SetResolution(resolutions[newValue].width, resolutions[newValue].height, Screen.fullScreen);
        settings.resolution = newValue;
    }

    private void OnFullscreenChanged(bool newValue)
    {
        settings.fullScreen = Screen.fullScreen = newValue;
    }

    private void OnMusicVolumeChanged(float newValue)
    {
        settings.musicVolume = newValue;
        AudioManager.instance.SetMusicVolume(newValue);
    }

    private void OnSfxVolumeChanged(float newValue)
    {
        settings.sfxVolume = newValue;
        AudioManager.instance.SetSfxVolume(newValue);
    }

    private int GetResolutionIndex()
    {
        for(int i = 0; i < resolutions.Length; i++)
        {
            if(resolutions[i].ToString() == Screen.currentResolution.ToString())
            {
                return i;
            }
        }

        return 0;
    }
}
