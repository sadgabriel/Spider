using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private AudioSource SfxSource;
    [SerializeField] private AudioClip BgmClip;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayBgm();
    }

    public void PlayBgm()
    {
        if (BgmSource != null && BgmClip != null)
        {
            BgmSource.clip = BgmClip;
            BgmSource.loop = true;
            BgmSource.Play();
        }
    }

    public void StopBgm()
    {
        if (BgmSource != null)
        {
            BgmSource.Stop();
        }
    }

    public void PlaySfx(AudioClip clip, float volume = 1.0f)
    {
        if (SfxSource != null && clip != null)
        {
            SfxSource.PlayOneShot(clip, volume);
        }
    }
}
