using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SkillMain : EditorWindow
{
    [MenuItem("工具/技能编辑器")]
    public static void Open()
    {
        var w = EditorWindow.GetWindow<SkillMain>();
        w.Show();
    }

    /// <summary>
    /// 玩家编辑器类
    /// </summary>
    class PlayerEditor
    {
        public int folderIndex = 0;
        public int fileIndex = 0;

        public string folderName = string.Empty;
        public string fileName = string.Empty;

        public List<string> HeroList = new List<string>();
        public Player player = null;
    }

    PlayerEditor m_Player = new PlayerEditor();
    /// <summary>
    /// 文件夹集合
    /// </summary>
    public List<string> FolderList = new List<string>();
    /// <summary>
    /// 预制体集合
    /// </summary>
    public List<string> FileList = new List<string>();
    /// <summary>
    /// 文件夹路径
    /// </summary>
    string FolderPath = "Assets/GameData/Model";
    /// <summary>
    /// 按文件名存储预制体
    /// </summary>
    Dictionary<string, List<string>> FolderPrefabsDic = new Dictionary<string, List<string>>();

    /// <summary>
    /// 技能详细窗口
    /// </summary>
    SkillpartEditor ParticularWindow;


    private void OnEnable()
    {
        GetFolder();

        GetFiles();
    }
    private void GetFolder()
    {
        FolderList.Clear();
        FolderList.Add("All");
        string[] folders = Directory.GetDirectories(FolderPath);
        foreach (var item in folders)
        {
            FolderList.Add(Path.GetFileName(item));
        }
    }

    private void GetFiles()
    {
        FileList.Clear();
        string[] files = Directory.GetFiles(FolderPath, ".prefab", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            FileList.Add(Path.GetFileNameWithoutExtension(item));
        }
        FileList.Sort();
        FileList.Insert(0, "null");
        m_Player.HeroList.AddRange(FileList);
    }

    /// <summary>
    /// 新建技能名字
    /// </summary>
    string newSkillName = string.Empty;

    Vector2 pos;
    private void OnGUI()
    {
        int folderIndex = EditorGUILayout.Popup(m_Player.folderIndex, FolderList.ToArray());
        if (folderIndex != m_Player.folderIndex)
        {
            m_Player.folderIndex = folderIndex;
            m_Player.fileIndex = -1;
            string folderName = FolderList[m_Player.folderIndex];
            List<string> list;
            if (folderName.Equals("All"))
            {
                list = FileList;
            }
            else
            {
                if (!FolderPrefabsDic.TryGetValue(folderName, out list))
                {
                    list = new List<string>();
                    string[] files = Directory.GetFiles(FolderPath + "/" + folderName, "*.prefab", SearchOption.AllDirectories); ;
                    foreach (var item in files)
                    {
                        list.Add(Path.GetFileNameWithoutExtension(item));
                    }
                }
                FolderPrefabsDic[folderName] = list;
            }
            m_Player.HeroList.Clear();
            m_Player.HeroList.AddRange(list);
        }

        int fileIndex = EditorGUILayout.Popup(m_Player.fileIndex, m_Player.HeroList.ToArray());
        if (fileIndex != m_Player.fileIndex)
        {
            m_Player.fileIndex = fileIndex;
            if (m_Player.fileName != m_Player.HeroList[m_Player.fileIndex])
            {
                m_Player.fileName = m_Player.HeroList[m_Player.fileIndex];
                if (!string.IsNullOrEmpty(m_Player.fileName))
                {
                    if (m_Player.player != null)
                    {
                        m_Player.player.OnDestory();
                    }
                    m_Player.player = Player.Init(m_Player.fileName);
                }
            }
        }

        newSkillName = GUILayout.TextField(newSkillName);
        if (GUILayout.Button("创建新技能"))
        {
            if (!string.IsNullOrEmpty(newSkillName) && m_Player.player != null)
            {
                List<SkillBase> skills = m_Player.player.AddNewSkill(newSkillName);
                OpenSkillWindow(newSkillName, skills);

                newSkillName = "";
            }
        }


        if (m_Player.player != null)
        {
            pos = GUILayout.BeginScrollView(pos, false, true);

            foreach (var item in m_Player.player.skillDic)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(item.Key))
                {
                    var skillComs = m_Player.player.GetSkill(item.Key);
                    foreach (var ite in skillComs)
                    {
                        ite.Init();
                    }
                    OpenSkillWindow(item.Key, skillComs);
                }

                if (GUILayout.Button("删除技能", new GUILayoutOption[]
                {
                    GUILayout.Width(60),
                    GUILayout.Height(20)
                }))
                {
                    m_Player.player.DesSkill(item.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

    }

    /// <summary>
    /// 打开窗口方法
    /// </summary>
    /// <param name="newSkillName"></param>
    /// <param name="skills"></param>
    private void OpenSkillWindow(string newSkillName, List<SkillBase> skills)
    {
        if (skills != null)
        {
            if (ParticularWindow == null)
            {
                ParticularWindow = EditorWindow.GetWindow<SkillpartEditor>("");
            }
            ParticularWindow.titleContent = new GUIContent(newSkillName);

            //给详细窗口  传递数据
            ParticularWindow.SetInitSkill(skills, m_Player.player);
            ParticularWindow.Show();
            ParticularWindow.Repaint();
        }
    }
}