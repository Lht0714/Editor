using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
/// <summary>
/// 技能详情窗口
/// </summary>
public class SkillEnityEditor : EditorWindow
{
    playerSkillData skilldata;
    skillShowType skill;
    float SkillSpeed;
    string[] skillComponent = new string[] { "null", "动画", "声音", "特效" };
    int skillIndex;
    public List<SkillBase> list = new List<SkillBase>();
    public void OnCreate(skillShowType item, playerSkillData skilldata)
    {
        this.skilldata = skilldata;
        skill = item;
        foreach (var data in skill.mydata)
        {
            Add(data);
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("播放"))
        {
            foreach (var item in list)
            {
                if (item is AnimatorClipSkill)
                {
                    AnimatorClipSkill temp = item as AnimatorClipSkill;
                    EditorManager.Get().playerScript.AniAdd(temp.clip,temp.time);
                }
                else if (item is AudioClipSkill)
                {
                    AudioClipSkill temp = item as AudioClipSkill;
                    EditorManager.Get().playerScript.bgmAdd(temp.clip, temp.time);
                }
                else if (item is GameobjectClipSkill)
                {
                    GameobjectClipSkill temp = item as GameobjectClipSkill;
                    EditorManager.Get().playerScript.EffAdd(temp.clip, temp.time);
                }
            }
        }
        if (GUILayout.Button("停止"))
        {
            EditorManager.Get().playerScript.StopAll();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("速度");
        float speed = EditorGUILayout.Slider(SkillSpeed, 0, 5);
        if (speed != SkillSpeed)
        {
            SkillSpeed = speed;
            Time.timeScale = SkillSpeed;
        }
        GUILayout.BeginHorizontal("box");
        skillIndex = EditorGUILayout.Popup(skillIndex, skillComponent);
        if (GUILayout.Button("添加"))
        {
            if (skillIndex==0)
            {
                Debug.Log("不能添加空组件");
                return;
            }
            SkillJson skillclip = new SkillJson();
            switch (skillIndex)
            {
                case 1:
                    skillclip.type = "动画";
                    break;
                case 2:
                    skillclip.type = "声音";
                    break;
                case 3:
                    skillclip.type = "特效";
                    break;
            }
            skillclip.name = "";
            skillclip.path = "";
            skillclip.time = 0;
            skill.mydata.Add(skillclip);
            Add(skillclip);
            EditorManager.Get().SaveData();
        }
        EditorGUILayout.EndHorizontal();
        int thislen = 0;
        foreach (var item in skill.mydata)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(item.name);
            if (GUILayout.Button("删除"))
            {
                skill.RemoveSkill(item.name);
                list.RemoveAt(thislen);
                EditorManager.Get().SaveData();
                break;
            }
            EditorGUILayout.EndHorizontal();
            float newtime= EditorGUILayout.FloatField(item.time);
            if (newtime != item.time)
            {
                list[thislen].time=newtime;
                item.time = newtime;
                EditorManager.Get().SaveData();
            }
            if (item.type=="动画")
            {
                AnimatorClipSkill temp= list[thislen] as AnimatorClipSkill;
                AnimationClip tempclip = EditorGUILayout.ObjectField(temp.clip, typeof(AnimationClip), false) as AnimationClip;
                if (temp.clip!= tempclip)
                {
                    temp.clip = tempclip;
                    item.path = AssetDatabase.GetAssetPath(tempclip);
                    temp.path = item.path;
                    item.name = tempclip.name;
                    temp.name = item.name;
                    EditorManager.Get().SaveData();
                }
            }
            else if (item.type == "声音")
            {
                AudioClipSkill temp = list[thislen] as AudioClipSkill;
                AudioClip tempclip = EditorGUILayout.ObjectField(temp.clip, typeof(AudioClip), false) as AudioClip;
                if (temp.clip != tempclip)
                {
                    temp.clip = tempclip;
                    item.path = AssetDatabase.GetAssetPath(tempclip);
                    temp.path = item.path;
                    item.name = tempclip.name;
                    temp.name = item.name;
                    EditorManager.Get().SaveData();
                }
            }
            else if (item.type == "特效")
            {
                GameobjectClipSkill temp = list[thislen] as GameobjectClipSkill;
                GameObject tempclip = EditorGUILayout.ObjectField(temp.clip, typeof(GameObject), false) as GameObject;
                if (temp.clip != tempclip)
                {
                    temp.clip = tempclip;
                    item.path = AssetDatabase.GetAssetPath(tempclip);
                    temp.path = item.path;
                    item.name = tempclip.name;
                    temp.name = item.name;
                    EditorManager.Get().SaveData();
                }
            }
            thislen++;
        }

    }

    public void Add(SkillJson data)
    {
        if (data.type == "动画")
        {
            AnimatorClipSkill skill = new AnimatorClipSkill();
            skill.name = data.name;
            skill.path = data.path;
            skill.time = data.time;
            skill.type = data.type;
            skill.clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(skill.path);
            list.Add(skill);
        }
        else if (data.type == "声音")
        {
            AudioClipSkill skill = new AudioClipSkill();
            skill.name = data.name;
            skill.path = data.path;
            skill.time = data.time;
            skill.type = data.type;
            skill.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(skill.path);
            list.Add(skill);
        }
        else if (data.type == "特效")
        {
            GameobjectClipSkill skill = new GameobjectClipSkill();
            skill.name = data.name;
            skill.path = data.path;
            skill.time = data.time;
            skill.type = data.type;
            skill.clip = AssetDatabase.LoadAssetAtPath<GameObject>(skill.path);
            list.Add(skill);
        }
    }
   
}
