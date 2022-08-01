using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Effect : SkillBase
{
    Player player;

    /// <summary>
    /// 拖过来的特效预制体
    /// </summary>
    public GameObject gameClip;

    /// <summary>
    /// 
    /// </summary>
    GameObject InstatePart;

    /// <summary>
    /// 特效开关
    /// </summary>
    ParticleSystem ps;

    public Skill_Effect(Player _player)
    {
        player = _player;
    }

    public override void Init()
    {
        if (gameClip.GetComponent<ParticleSystem>())
        {
            ps = InstatePart.GetComponent<ParticleSystem>();
            ps.Stop();
        }
    }

    public void SetGameClip(GameObject tuoClip)
    {
        gameClip = tuoClip;
        if (gameClip.GetComponent<ParticleSystem>())
        {
            InstatePart = GameObject.Instantiate(gameClip, player.effectsParent);
            ps = InstatePart.GetComponent<ParticleSystem>();
            ps.Stop();
        }
        name = tuoClip.name;
    }

    public override void Play()
    {
        if (ps != null)
        {
            ps.Play();
        }
    }

    public override void Stop()
    {
        if (ps != null)
        {
            ps.Stop();
        }
    }
}
