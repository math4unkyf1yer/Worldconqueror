using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSFXSource;
    public AudioClip ButtonSound;
    public AudioClip ButtonCoinSound;
    public AudioClip SpawnSound;
    public AudioClip teritoryAttack;
    public AudioClip winSound;
    public AudioClip loseSound;

    //Abilities
    public AudioClip arrowLoose;
    public AudioClip mageFire;
    public AudioClip explosion;

    bool sfxSound = true;
    private void Start()
    {
        audioSFXSource = GetComponent<AudioSource>();
    }

    public void PlayButtonSound()
    {
        if (sfxSound)
        {
            audioSFXSource.PlayOneShot(ButtonSound);
        }
    }

    public void PlayButtonCoinSound()
    {
        if (sfxSound)
        {
            audioSFXSource.PlayOneShot(ButtonCoinSound);
        }
    }

    public void PlayWinSound()
    {
        if (sfxSound)
        {
            audioSFXSource.PlayOneShot(winSound);
        }
    }
    public void PlayLoseSound()
    {
        if (sfxSound)
        {
            audioSFXSource.PlayOneShot(loseSound);
        }
    }

    public void ChangeSfxStatus(bool sfxOn)
    {
        sfxSound = sfxOn;

        audioSFXSource.mute = !sfxSound;

    }

    public bool GetSfx()
    {
        return sfxSound;
    }

}
