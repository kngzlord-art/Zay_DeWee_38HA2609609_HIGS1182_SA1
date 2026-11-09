using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider, sfxSlider, musicSlider;
    public GameObject audiopanel;


    private void Start()
    {
     

        float musicVol, sfxVol, masterVol;

        audioMixer.GetFloat("MusicVolume", out musicVol);
        audioMixer.GetFloat("MasterVolume", out masterVol);
        audioMixer.GetFloat("SFXVolume", out sfxVol);

        musicSlider.value = Mathf.Pow(10, musicVol / 20);
        sfxSlider.value = Mathf.Pow(10, sfxVol / 20);
        masterSlider.value = Mathf.Pow(10, masterVol / 20);


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (audiopanel.activeSelf)
            {
                CloseAudioPanel();
            }
            else
            {
                OpenAudioPanel();
            }
        }
    }

    public void setMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
    public void setMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    public void setsfxVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void OpenAudioPanel()
    {
        audiopanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseAudioPanel()
    {
        audiopanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}