using UnityEngine;
using UnityEngine.UI;

public class ToggleAudioVolume : MonoBehaviour
{
    public AudioSource backgroundAudio;
    public AudioSource otherAudio;

    public Button bgMusic;
    public Button audioMusic;

    public Sprite bgMusicOnSprite;
    public Sprite bgMusicOffSprite;
    public Sprite audioMusicOnSprite;
    public Sprite audioMusicOffSprite;

    private bool isBackgroundMuted;
    private bool isOtherAudioMuted;

    private const string BackgroundAudioKey = "BackgroundAudioMuted";
    private const string OtherAudioKey = "OtherAudioMuted";

    private void Start()
    {
        // Load the saved states when the scene starts
        isBackgroundMuted = PlayerPrefs.GetInt(BackgroundAudioKey, 0) == 1;
        isOtherAudioMuted = PlayerPrefs.GetInt(OtherAudioKey, 0) == 1;

        ApplyAudioSettings();
    }

    public void ToggleBackgroundMusic()
    {
        isBackgroundMuted = !isBackgroundMuted;
        PlayerPrefs.SetInt(BackgroundAudioKey, isBackgroundMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyBackgroundAudioSettings();
        Debug.Log("Background music volume set to " + (isBackgroundMuted ? "0 (muted)" : "0.03 (unmuted)"));
    }

    public void ToggleOtherAudio()
    {
        isOtherAudioMuted = !isOtherAudioMuted;
        PlayerPrefs.SetInt(OtherAudioKey, isOtherAudioMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyOtherAudioSettings();
        Debug.Log("Other audio volume set to " + (isOtherAudioMuted ? "0 (muted)" : "1 (unmuted)"));
    }

    private void ApplyAudioSettings()
    {
        ApplyBackgroundAudioSettings();
        ApplyOtherAudioSettings();
    }

    private void ApplyBackgroundAudioSettings()
    {
        if (backgroundAudio != null)
            backgroundAudio.volume = isBackgroundMuted ? 0f : 0.03f;

        if (bgMusic != null)
        {
            Image buttonImage = bgMusic.GetComponent<Image>();
            if (buttonImage != null)
                buttonImage.sprite = isBackgroundMuted ? bgMusicOffSprite : bgMusicOnSprite;
        }
    }

    private void ApplyOtherAudioSettings()
    {
        if (otherAudio != null)
            otherAudio.volume = isOtherAudioMuted ? 0f : 1f;

        if (audioMusic != null)
        {
            Image buttonImage = audioMusic.GetComponent<Image>();
            if (buttonImage != null)
                buttonImage.sprite = isOtherAudioMuted ? audioMusicOffSprite : audioMusicOnSprite;
        }
    }
}
