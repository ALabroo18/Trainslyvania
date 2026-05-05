using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioSlider : MonoBehaviour
{

    // Min and max on an audio mixer
    float minVolume = -80;
    float maxVolume = 20f;
    
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private float masterVolume;

    private float musicVolume;

    private float sfxVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        mixer.GetFloat("Master", out masterVolume);
        mixer.GetFloat("Music", out musicVolume);
        mixer.GetFloat("SFX", out sfxVolume);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(masterVolume);
        Debug.Log(musicVolume);
        Debug.Log(sfxVolume);

    }

    public void ChangeMaster(float value)
    {
        masterVolume = masterSlider.value;
        mixer.SetFloat("Master",  masterVolume);
    }

    public void ChangeMusic ()
    {
        musicVolume = musicSlider.value;
        mixer.SetFloat("Music",  musicVolume);
    }

    public void ChangeSFX()
    {
        sfxVolume = sfxSlider.value;
        mixer.SetFloat("SFX",  sfxVolume);
    }
}
