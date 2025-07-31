using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip expGainClip;
    [SerializeField] private float expGainVolume = 0.5f;
    [SerializeField] private AudioClip demolitionClip;
    [SerializeField] private float demolitionVolume = 0.5f;
    [SerializeField] private AudioClip buildClip;
    [SerializeField] private float buildVolume = 0.5f;
    [SerializeField] private AudioClip enemyAttackClip;
    [SerializeField] private float enemyAttackVolume = 0.5f;

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
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBgm()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlaySfx(AudioClip clip, float volume = 1.0f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayExpGainSfx()
    {
        PlaySfx(expGainClip, expGainVolume);
    }

    public void PlayDemolitionSfx()
    {
        PlaySfx(demolitionClip, demolitionVolume);
    }

    public void PlayBuildSfx()
    {
        PlaySfx(buildClip, buildVolume);
    }

    public void PlayEnemyAttackSfx()
    {
        PlaySfx(enemyAttackClip, enemyAttackVolume);
    }
}
