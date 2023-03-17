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

    public void PlayAttackSound() { playOneRandomSound(attackSounds); }
    public void PlayTakeDamageSound() { playOneRandomSound(takeDamageSounds); }
    public void PlaySelectVoiceSound() { playOneRandomSound(selectVoiceSounds); }
    public void PlayMoveVoiceSound() { playOneRandomSound(moveVoiceSounds); }
    public void PlayAttackVoiceSound() { playOneRandomSound(attackVoiceSounds); }
    public void PlayPainVoiceSound() { playOneRandomSound(painVoiceSounds); }
    public void PlayDeathVoiceSound() { playOneRandomSound(deathVoiceSounds); }

    private void playOneRandomSound(List<AudioClip> sounds)
    {
        int idx = Random.Range(0, sounds.Count);
        audioSource.clip = sounds[idx];
        audioSource.Play();
    }
}
