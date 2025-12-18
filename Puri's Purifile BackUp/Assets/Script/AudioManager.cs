using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--------------Audio Sources--------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [Header("--------------Audio Clips--------------")]
    
    public AudioClip background;
    public AudioClip jump;
    public AudioClip laser;
    public AudioClip powerUp;
    public AudioClip pickup;
    public AudioClip alienHit;
    public AudioClip alienChasing;
    public AudioClip alienDeath;
    public AudioClip smash;

   private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

}