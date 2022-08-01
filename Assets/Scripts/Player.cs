using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class Player : MonoBehaviour
{
    public Dictionary<string, List<SkillBase>> skillDic = new Dictionary<string, List<SkillBase>>();

    /// <summary>
    /// 运行时控制器
    /// </summary>
    RuntimeAnimatorController RunControl;
    /// <summary>
    /// 重写动画控制器
    /// </summary>
    public AnimatorOverrideController overrideController;
    /// <summary>
    /// 特效挂点
    /// </summary>
    public Transform effectsParent;
    /// <summary>
    /// 声音
    /// </summary>
    AudioSource audioSource;
    /// <summary>
    /// 动画控制器
    /// </summary>
    Animator ani;
    /// <summary>
    /// 存放一个人身上所有的技能组件
    /// </summary>
    public List<SkillBase> curSkillComponents = new List<SkillBase>();

    private void Awake()
    {
        ani = gameObject.GetComponent<Animator>();
    }

    public static Player Init(string PlayerName)
    {
        if (PlayerName != null)
        {
            string path = "Assets/AllModel/" + PlayerName + ".prefab";
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj != null)
            {
                Player player = Instantiate(obj).GetComponent<Player>();
                //先实例化 重写控制器
                player.overrideController = new AnimatorOverrideController();
                //赋值Player模板
                player.overrideController.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Player");
                //将自己的动画控制器 赋值   ：重写控制器
                player.ani.runtimeAnimatorController = player.overrideController;

                player.audioSource = player.gameObject.AddComponent<AudioSource>();
                player.effectsParent = player.transform.Find("effectsparent");
                player.name = PlayerName;
                //读取技能json
                player.LoadAllSkill();
                return player;
            }
        }
        return null;
    }
    public class SkillJson
    {
        public string name;
        public Dictionary<string, List<SkillComponentsData>> skillComponents = new Dictionary<string, List<SkillComponentsData>>();
    }

    public class SkillComponentsData
    {
        public string ComponentName;

        /// <summary>
        /// 技能组件基础信息
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="tri"></param>
        public SkillComponentsData(string cn)
        {
            ComponentName = cn;
        }
    }

    public void OnSave()
    {
        List<SkillJson> skills = new List<SkillJson>();
        foreach (var item in skillDic)
        {
            SkillJson skillJson = new SkillJson();
            skillJson.name = item.Key;
            foreach (var ite in item.Value)
            {
                if (ite is Skill_Anim)
                {
                    if (!skillJson.skillComponents.ContainsKey("动画"))
                    {
                        skillJson.skillComponents.Add("动画", new List<SkillComponentsData>());
                    }
                    skillJson.skillComponents["动画"].Add(new SkillComponentsData(ite.name));
                }
                else if (ite is Skill_Audio)
                {
                    if (!skillJson.skillComponents.ContainsKey("音效"))
                    {
                        skillJson.skillComponents.Add("音效", new List<SkillComponentsData>());
                    }
                    skillJson.skillComponents["音效"].Add(new SkillComponentsData(ite.name));
                }
                else if (ite is Skill_Effect)
                {
                    if (!skillJson.skillComponents.ContainsKey("特效"))
                    {
                        skillJson.skillComponents.Add("特效", new List<SkillComponentsData>());
                    }
                    skillJson.skillComponents["特效"].Add(new SkillComponentsData(ite.name));
                }
            }
            skills.Add(skillJson);
        }
        string str = JsonConvert.SerializeObject(skills);
        File.WriteAllText("Assets/" + gameObject.name + ".txt", str);
    }

    public void LoadAllSkill()
    {
        Debug.Log(gameObject.name);
        if (File.Exists("Assets/" + gameObject.name + ".txt"))
        {
            string str = File.ReadAllText("Assets/" + gameObject.name + ".txt");
            List<SkillJson> skills = JsonConvert.DeserializeObject<List<SkillJson>>(str);
            foreach (var item in skills)
            {
                skillDic.Add(item.name, new List<SkillBase>());
                foreach (var ite in item.skillComponents)
                {
                    foreach (var it in ite.Value)
                    {
                        if (ite.Key.Equals("动画"))
                        {
                            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/GameData/Anim/" + it.ComponentName + ".anim"); ;///空
                            Skill_Anim _Anim = new Skill_Anim(this);
                            _Anim.SetAniClip(clip);
                            skillDic[item.name].Add(_Anim);
                        }
                        else if (ite.Key.Equals("音效"))
                        {
                            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/GameData/Audio/" + it.ComponentName + ".mp3");
                            Skill_Audio _Anim = new Skill_Audio(this);
                            _Anim.SetAudioClip(clip);
                            skillDic[item.name].Add(_Anim);
                        }
                        else if (ite.Key.Equals("特效"))
                        {
                            GameObject clip = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameData/Effect/Skill/" + it.ComponentName + ".prefab");
                            Skill_Effect _Anim = new Skill_Effect(this);
                            _Anim.SetGameClip(clip);
                            skillDic[item.name].Add(_Anim);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// 添加新技能
    /// </summary>
    /// <param name="newSkillName"></param>
    /// <returns></returns>
    public List<SkillBase> AddNewSkill(string newSkillName)
    {
        if (skillDic.ContainsKey(newSkillName))
        {
            return skillDic[newSkillName];
        }
        skillDic.Add(newSkillName, new List<SkillBase>());

        return skillDic[newSkillName];
    }

    /// <summary>
    /// 获取技能
    /// </summary>
    /// <param name="skillName"></param>
    /// <returns></returns>
    public List<SkillBase> GetSkill(string skillName)
    {
        if (skillDic.ContainsKey(skillName))
        {
            return skillDic[skillName];
        }
        return null;
    }

    /// <summary>
    /// 删除技能
    /// </summary>
    /// <param name="newSkillName"></param>
    public void DesSkill(string newSkillName)
    {
        if (skillDic.ContainsKey(newSkillName))
        {
            skillDic.Remove(newSkillName);
        }
    }

    public void OnDestory()
    {
        Destroy(gameObject);
    }
}