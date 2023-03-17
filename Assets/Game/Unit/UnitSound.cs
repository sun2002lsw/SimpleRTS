using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class UnitSound : MonoBehaviour
{
    private AudioSource unitAudioSource;

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
        unitAudioSource = GetComponent<AudioSource>();
    }

    public void PlayAttackSound() { playOneRandomSound(attackSounds, null); }
    public void PlayTakeDamageSound() { playOneRandomSound(takeDamageSounds, null); }
    public void PlaySelectVoiceSound(AudioSource system) { playOneRandomSound(selectVoiceSounds, system); }
    public void PlayStopVoiceSound(AudioSource system) { playOneRandomSound(stopVoiceSounds, system); }
    public void PlayHoldVoiceSound(AudioSource system) { playOneRandomSound(holdVoiceSounds, system); }
    public void PlayMoveVoiceSound(AudioSource system) { playOneRandomSound(moveVoiceSounds, system); }
    public void PlayAttackVoiceSound(AudioSource system) { playOneRandomSound(attackVoiceSounds, system); }
    public void PlayPainVoiceSound() { playOneRandomSound(painVoiceSounds, null); }
    public void PlayDeathVoiceSound() { playOneRandomSound(deathVoiceSounds, null); }

    private void playOneRandomSound(List<AudioClip> sounds, AudioSource systemAudioSoure)
    {
        AudioSource audioSource;
        if (systemAudioSoure != null)
            audioSource = systemAudioSoure;
        else
            audioSource = unitAudioSource;

        int idx = Random.Range(0, sounds.Count);
        audioSource.clip = sounds[idx];
        audioSource.Play();
    }
}
