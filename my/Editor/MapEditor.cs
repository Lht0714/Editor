using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
public class MapEditor : EditorWindow 
{
    [MenuItem("tool/地图编辑器")]
    static void Create()
    {
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
        if (window != null)
        {
            window.Show();
        }
    }

    List<MapData> maplist = new List<MapData>();
    string mapName="";

    private void OnEnable()
    {
        string path = Application.dataPath + "/Json/map.json";
        if (File.Exists(path))
        {
            maplist = JsonConvert.DeserializeObject<List<MapData>>(File.ReadAllText(path));
        }
        EditorManager.Get().Init(maplist);
    }

    private void OnGUI()
    {
        mapName = GUILayout.TextField(mapName);
        if (GUILayout.Button("创建场景"))
        {
            if (mapName.Length >= 1)
            {
                MapData data = new MapData();
                data.mapName = mapName;
                maplist.Add(data);
                EditorManager.Get().SaveMapData();
            }
        }
        foreach (var item in maplist)
        {
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button(item.mapName))
            {
                SceneEditor window = (SceneEditor)EditorWindow.GetWindow(typeof(SceneEditor));
                if (window != null)
                {
                    window.Show();
                    window.OnCreate(item);
                }
            }
            if (GUILayout.Button("移除"))
            {
                maplist.Remove(item);
                EditorManager.Get().SaveMapData();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        
    }
}
