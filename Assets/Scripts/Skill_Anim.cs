using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Anim : SkillBase
{
    Player player;

    Animator ani;

    public AnimationClip aniClip;

    AnimatorOverrideController Aoc;

    public Skill_Anim(Player _player)
    {
        player = _player;
        ani = _player.gameObject.GetComponent<Animator>();
        Aoc = _player.overrideController;
    }
    public override void Init()
    {
        Aoc["Start"] = aniClip;
    }

    public void SetAniClip(AnimationClip animnClip)
    {
        aniClip = animnClip;
        name = aniClip.name;
        Aoc["Start"] = aniClip;
    }

    public override void Play()
    {
        base.Play();

        ani.StopPlayback();

        AnimatorStateInfo stateInfo = ani.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Idle1"))
        {
            ani.SetTrigger("Play");
        }
    }

    public override void Stop()
    {
        base.Stop();
        ani.StopPlayback();
    }
}
