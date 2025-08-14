using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;


    public bool isMainMenu;
    private AudioClip[] arrRadioStation;
    public AudioClip[] arrPop;
    public AudioClip[] arrCalm;
    public AudioClip button;
    public AudioClip MainMenu;

    private int currentClipIndex = 0; // Track the current clip index

    private void Start()
    {
        if (isMainMenu)
        {
            musicSource.clip = MainMenu;
            musicSource.Play();
        }
        else
        {
            arrRadioStation = arrPop;
            PlayNextClip();
        }
    }

    private void Update()
    {
        if (!isMainMenu)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                SwitchStation();
            }

            // Check if the current clip has finished playing
            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                PlayNextClip();
            }
        }

    }

    public void PlayButtons()
    {
        sfxSource.PlayOneShot(button);
    }

    private void PlayNextClip()
    {
        if (arrRadioStation == null || arrRadioStation.Length == 0) return;

        // Set the next clip index (loop back to 0 if at the end)
        currentClipIndex = (currentClipIndex + 1) % arrRadioStation.Length;
        musicSource.clip = arrRadioStation[currentClipIndex];
        musicSource.Play();
    }

    public AudioClip RandomizeSound(AudioClip[] audioClips)
    {
        if (audioClips == null || audioClips.Length == 0) return null;
        int selectSFX = Random.Range(0, audioClips.Length);
        return audioClips[selectSFX];
    }

    public void SwitchStation()
    {
        if (arrRadioStation == arrPop)
        {
            arrRadioStation = arrCalm;
        }
        else
        {
            arrRadioStation = arrPop;
        }

        currentClipIndex = -1; // Reset index so PlayNextClip starts at 0
        PlayNextClip();
    }
}
