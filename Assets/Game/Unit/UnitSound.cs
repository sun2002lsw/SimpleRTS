using System.Collections.Generic;
using UnityEngine;

public class UnitSound : MonoBehaviour
{
    private AudioSource audioSource;

    // SOUND
    [SerializeField]
    private List<AudioClip> attackSounds;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttackSound()
    {
        int attackSoundIdx = Random.Range(0, attackSounds.Count);
        audioSource.clip = attackSounds[attackSoundIdx];
        audioSource.Play();
    }
}
