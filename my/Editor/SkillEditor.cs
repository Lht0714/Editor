using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
/// <summary>
/// 选择人物技能列表窗口
/// </summary>
public class SkillEditor : EditorWindow
{
    playerSkillData skilldata = new playerSkillData();
    GameObject player;
    public List<string> typeList = new List<string>() { "All", "Enemy", "Player" };
    public List<string> modelList = new List<string>() { "刀客", "忍者", "白虎王" };
    int typeindex, modelindex;
    string skillName="";
    
    [MenuItem("tool/技能编辑器")]
    static void skill()
    {
        if (Application.isPlaying)
        {
            SkillEditor window = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor));
            if (window != null)
            {
                window.Show();
            }
        }
    }
    private void OnEnable()
    {
        MessageCenter.Get().OnAddListen("SaveSkillJson",(a)=> {
            SaveData();
        });
        ChangeSkillData();
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical("box");
        int index1 = EditorGUILayout.Popup(typeindex, typeList.ToArray());
        if (index1 != typeindex)
        {
            typeindex = index1;
            modelList.Clear();
            if (index1 == 0)
            {
                modelList = new List<string>() { "刀客", "忍者", "白虎王" };
            }
            else if (index1 == 1)
            {
                modelList = new List<string>() { "白虎王" };
            }
            else if (index1 == 2)
            {
                modelList = new List<string>() { "刀客", "忍者" };
            }
            modelindex = 0;
            ChangeSkillData();
        }
        int model = EditorGUILayout.Popup(modelindex, modelList.ToArray());
        if (model!= modelindex)
        {
            modelindex = model;
            ChangeSkillData();
        }
        skillName = GUILayout.TextField(skillName);
        if (GUILayout.Button("创建技能"))
        {
            if (skillName.Length>=1)
            {
                skillShowType newSkill = new skillShowType();
                newSkill.myName = skillName;
                skilldata.list.Add(newSkill);
                SaveData();
            }
        }
        foreach (var item in skilldata.list)
        {
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button(item.myName))
            {
                SkillEnityEditor NewWindow = (SkillEnityEditor)EditorWindow.GetWindow(typeof(SkillEnityEditor));
                if (NewWindow != null)
                {
                    NewWindow.Show();
                    NewWindow.OnCreate(item, skilldata);
                }
            }
            if (GUILayout.Button("删除"))
            {
                skilldata.RemoveSkill(item.myName);
                SaveData();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    public void ChangeSkillData()
    {
        string path = Application.dataPath + "/Json/" + modelList[modelindex] + ".json";
        if (player!=null)
        {
            Destroy(player);
        }
        player= GameObject.Instantiate(Resources.Load<GameObject>("PlayerModel/" + modelList[modelindex]));
        player.AddComponent<Player>();
        if (File.Exists(path))
        {
            skilldata= JsonConvert.DeserializeObject<playerSkillData>(File.ReadAllText(path));
        }
        else
        {
            skilldata = new playerSkillData();
            skilldata.name = modelList[modelindex];
        }
        EditorManager.Get().Init(skilldata, player);
    }

    public void SaveData()
    {
        string path = Application.dataPath + "/Json/" + modelList[modelindex] + ".json";
        File.WriteAllText(path,JsonConvert.SerializeObject(skilldata));
    }
}
