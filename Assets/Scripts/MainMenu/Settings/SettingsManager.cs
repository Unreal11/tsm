using System.Collections.Generic;
using Core.Client;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        public GameObject authWindow;
        public Button searchBattleButton;
        public Button cancelSearchBattleButton;

        [Header("AudioSettings")]
        public AudioMixer audioMixer;
        public Slider volumeSlider;

        public AudioMixer sfxMixer;
        public Slider sfxSlider;


        [Header("VideoSettings")]
        public TMP_Dropdown qualityDropdown;
        public TMP_Dropdown resolutionDropdown;
        public Toggle fullScreenToggle;
        private int screenInt;
        private Resolution[] _resolutions;
        private bool isFullScreen;

        private const string prefName = "optionvalue";
        private const string resolutionName = "resolutionoption";

        public void SearchMatch()
        {
            MatchClientController.SearchingMatch();
        }

        public void CancelSearchMatch()
        {
            MatchClientController.CancelSearchingMatch();
        }

        private void Awake()
        {
            screenInt = PlayerPrefs.GetInt("togglestate");

            if (screenInt == 1)
            {
                isFullScreen = true;
                fullScreenToggle.isOn = true;
            }
            else
            {
                fullScreenToggle.isOn = false;
            }

            resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
            {
                PlayerPrefs.SetInt(resolutionName, resolutionDropdown.value);
                Save();

            }));

            qualityDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
            {
                PlayerPrefs.SetInt(prefName, qualityDropdown.value);
                Save();

            }));
        }

        private void Start()
        {
            try
            {
                if (searchBattleButton != null)
                    searchBattleButton.onClick.AddListener(MatchClientController.SearchingMatch);

                if (cancelSearchBattleButton != null)
                    cancelSearchBattleButton.onClick.AddListener(MatchClientController.CancelSearchingMatch);

                AuthClientController.authSuccessEvent.AddListener(() => authWindow.SetActive(false));

                if (MainClient.IsAuth() && authWindow != null)
                    authWindow.SetActive(false);

                if (PlayerPrefs.HasKey("MVolume"))
                {
                    volumeSlider.value = PlayerPrefs.GetFloat("MVolume", 0f);
                }
                else
                {
                    volumeSlider.value = -15f;

                    PlayerPrefs.SetFloat("MVolume", volumeSlider.value);
                }

                audioMixer.SetFloat("volume", PlayerPrefs.GetFloat("MVolume"));

                if (PlayerPrefs.HasKey("SFXVolume"))
                {
                    sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0f);
                }
                else
                {
                    sfxSlider.value = -15f;

                    PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
                }

                sfxMixer.SetFloat("sfxvolume", PlayerPrefs.GetFloat("SFXVolume"));

                qualityDropdown.value = PlayerPrefs.GetInt(prefName, 2);
                _resolutions = Screen.resolutions;

                resolutionDropdown.ClearOptions();

                List<string> options = new List<string>();
                int currentResolutionIndex = 0;

                for (int i = 0; i < _resolutions.Length; i++)
                {
                    string option = _resolutions[i].width + "x" + _resolutions[i].height + " " +
                                    _resolutions[i].refreshRate + "Hz";
                    options.Add(option);

                    if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height &&
                    _resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                    {
                        currentResolutionIndex = i;
                    }
                }

                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = PlayerPrefs.GetInt(resolutionName, currentResolutionIndex);
                resolutionDropdown.RefreshShownValue();

                PlayerPrefs.Save();
            }
            catch { }
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        public void FillAccountInfo()
        {
            //Тут будем заполнять инфу о имени, ранге колоде и т.д.
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];

            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void ChangeVolume(float volume)
        {
            PlayerPrefs.SetFloat("MVolume", volume);
            audioMixer.SetFloat("volume", PlayerPrefs.GetFloat("MVolume"));
        }

        public void ChangeEffectsVolume(float volume)
        {
            PlayerPrefs.SetFloat("SFXVolume", volume);
            sfxMixer.SetFloat("sfxvolume", PlayerPrefs.GetFloat("SFXVolume"));
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;

            if (isFullScreen == false)
            {
                PlayerPrefs.SetInt("togglestate", 0);
            }
            else
            {
                isFullScreen = true;
                PlayerPrefs.SetInt("togglestate", 1);
            }
        }
    }
}