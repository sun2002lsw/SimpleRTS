using System.Collections.Generic;
using UnityEngine;

public class UnitSound : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private List<AudioClip> attackSounds;
    [SerializeField]
    private List<AudioClip> takeDamageSounds;
    [SerializeField]
    private List<AudioClip> selectVoiceSounds;
    [SerializeField]
    private List<AudioClip> stopVoiceSounds;
    [SerializeField]
    private List<AudioClip> holdVoiceSounds;
    [SerializeField]
    private List<AudioClip> moveVoiceSounds;
    [SerializeField]
    private List<AudioClip> attackVoiceSounds;
    [SerializeField]
    private List<AudioClip> painVoiceSounds;
    [SerializeField]
    private List<AudioClip> deathVoiceSounds;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttackSound() { playOneRandomSound(attackSounds, false); }
    public void PlayTakeDamageSound() { playOneRandomSound(takeDamageSounds, false); }
    public void PlaySelectVoiceSound() { playOneRandomSound(selectVoiceSounds, true); }
    public void PlayStopVoiceSound() { playOneRandomSound(stopVoiceSounds, true); }
    public void PlayHoldVoiceSound() { playOneRandomSound(holdVoiceSounds, true); }
    public void PlayMoveVoiceSound() { playOneRandomSound(moveVoiceSounds, true); }
    public void PlayAttackVoiceSound() { playOneRandomSound(attackVoiceSounds, true); }
    public void PlayPainVoiceSound() { playOneRandomSound(painVoiceSounds, false); }
    public void PlayDeathVoiceSound() { playOneRandomSound(deathVoiceSounds, false); }

    private void playOneRandomSound(List<AudioClip> sounds, bool isUIsound)
    {
        if (isUIsound)
            audioSource.spatialBlend = 0;
        else
            audioSource.spatialBlend = 1;

        int idx = Random.Range(0, sounds.Count);
        audioSource.clip = sounds[idx];
        audioSource.Play();
    }
}
