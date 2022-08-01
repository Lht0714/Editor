using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EditorManager 
{
    static EditorManager ins;
    public static EditorManager Get()
    {
        if (ins == null)
        {
            ins = new EditorManager();
        }
        return ins;
    }

    playerSkillData skilldata = new playerSkillData();
    List<MapData> maplist = new List<MapData>();
    GameObject player;
    public Player playerScript;
    public void Init(playerSkillData da, GameObject pl)
    {
        skilldata = da;
        player=pl;
        playerScript = player.GetComponent<Player>();
    }
    public void Init(List<MapData> da)
    {
        maplist = da;
    }
    public void SaveData()
    {
        string path = Application.dataPath + "/Json/" + skilldata.name + ".json";
        File.WriteAllText(path, JsonConvert.SerializeObject(skilldata));
    }
    public void SaveMapData()
    {
        string path = Application.dataPath + "/Json/map.json";
        File.WriteAllText(path, JsonConvert.SerializeObject(maplist));
    }
}
