using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager I { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip bgmClip;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip zombieDeathClip;
    [SerializeField] private AudioClip batSwingClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private AudioClip pushClip;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    private void Start()
    {
        PlayBGM();
    }

    private void OnDestroy()
    {
        if (I == this)
        {
            I = null;
        }
    }

    private void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null)
        {
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;

        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void PlayZombieDeath()
    {
        PlaySFX(zombieDeathClip);
    }

    public void PlayBatSwing()
    {
        PlaySFX(batSwingClip);
    }

    public void PlayHeal()
    {
        PlaySFX(healClip);
    }

    public void PlayPush()
    {
        PlaySFX(pushClip);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}