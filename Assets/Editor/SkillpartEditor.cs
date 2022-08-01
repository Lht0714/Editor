using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillpartEditor : EditorWindow
{

    Player player;
    List<SkillBase> skillComponents;

    public void SetInitSkill(List<SkillBase> skillComs, Player _player)
    {
        player = _player;
        skillComponents = skillComs;
        player.curSkillComponents = skillComponents;
    }

    string[] comType = new string[] { "null", "动画", "音效", "特效" };
    int skillComIndex;
    private Vector2 ScrollViewPos;

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("播放"))
        {
            foreach (var item in skillComponents)
            {
                item.Play();
            }
        }
        if (GUILayout.Button("停止"))
        {
            foreach (var item in skillComponents)
            {
                item.Stop();
            }
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        skillComIndex = EditorGUILayout.Popup(skillComIndex, comType);
        if (GUILayout.Button("添加"))
        {
            switch (skillComIndex)
            {
                case 1:
                    skillComponents.Add(new Skill_Anim(player));
                    break;
                case 2:
                    skillComponents.Add(new Skill_Audio(player));
                    break;
                case 3:
                    skillComponents.Add(new Skill_Effect(player));
                    break;
            }
        }
        GUILayout.EndHorizontal();


        ScrollViewPos = GUILayout.BeginScrollView(ScrollViewPos, false, true);
        foreach (var item in skillComponents)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(item.name);
            if (GUILayout.Button("删除"))
            {
                skillComponents.Remove(item);
                break;
            }
            GUILayout.EndHorizontal();

            if (item is Skill_Anim)
            {
                SkillAnim(item as Skill_Anim);
            }
            else if (item is Skill_Audio)
            {
                SkillAudio(item as Skill_Audio);
            }
            else if (item is Skill_Effect)
            {
                Skill_Effect(item as Skill_Effect);
            }
        }
        if (GUILayout.Button("保存"))
        {
            Debug.Log("保存");
            player.OnSave();
        }
        GUILayout.EndScrollView();
    }

    private void Skill_Effect(Skill_Effect _Effect)
    {
        GameObject go = EditorGUILayout.ObjectField(_Effect.gameClip, typeof(GameObject), false) as GameObject;
        if (_Effect.gameClip != go)
        {
            _Effect.SetGameClip(go);
        }
    }

    private void SkillAudio(Skill_Audio _Audio)
    {
        AudioClip clip = EditorGUILayout.ObjectField(_Audio.audioClip, typeof(AudioClip), false) as AudioClip;
        if (_Audio.audioClip != clip)
        {
            _Audio.SetAudioClip(clip);
        }
    }

    private void SkillAnim(Skill_Anim _Anim)
    {
        AnimationClip ap = EditorGUILayout.ObjectField(_Anim.aniClip, typeof(AnimationClip), false) as AnimationClip;
        if (_Anim.aniClip != ap)
        {
            _Anim.SetAniClip(ap);
        }
    }
}
