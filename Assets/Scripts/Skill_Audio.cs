using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Audio : SkillBase
{
    Player player;

    AudioSource audioSource;

    public AudioClip audioClip;
    public Skill_Audio(Player _player)
    {
        player = _player;
        audioSource = _player.gameObject.GetComponent<AudioSource>();
    }
    public override void Init()
    {
        audioSource.clip = audioClip;
    }
    public override void Play()
    {
        audioSource.Play();
    }

    public override void Stop()
    {
        audioSource.Stop();
    }

    public void SetAudioClip(AudioClip ac)
    {
        audioClip = ac;
        name = audioClip.name;
        audioSource.clip = audioClip;
    }
}
