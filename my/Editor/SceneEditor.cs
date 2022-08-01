using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class SceneEditor : EditorWindow
{
    MapData mydata;
    Transform Root;
    List<GameObject> list = new List<GameObject>();
    List<string> str = new List<string>() { "Null","Player","Moster","NPC","Collect","Other"};
    public void OnCreate(MapData data)
    {
        Root = GameObject.Find("Root").transform;
        mydata = data;
        string path = Application.dataPath + "/Json/map.json";
        Debug.Log(mydata.modelList.Count);
        if (File.Exists(path))
        {
            foreach (var item in mydata.modelList)
            {
                Debug.Log("!");
                GameObject clone = GameObject.Instantiate(Resources.Load<GameObject>("PlayerModel/" + item.modelName), Root, false);
                clone.tag = item.modelType.ToString();
                clone.name = item.name;
                clone.transform.position = new Vector3(item.posX, item.posY, item.posZ);
                clone.transform.eulerAngles = new Vector3(item.X, item.Y, item.Z);
                list.Add(clone);
            }
        }
        else
        {
            Add();
        }
        
    }

    private void OnGUI()
    {
        if (Root.childCount != mydata.modelList.Count)
        {
            list.Clear();
            mydata.modelList.Clear();
            Add();
        }
        int len = 0;
        foreach (var item in mydata.modelList)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(item.name);
            int index=EditorGUILayout.Popup((int)item.modelType, str.ToArray());
            if (index!= (int)item.modelType)
            {
                item.modelType = (ModelType)index;
                list[len].tag = item.modelType.ToString();
                EditorManager.Get().SaveMapData();
            }
            EditorGUILayout.EndHorizontal();
            len++;
        }
        if (GUILayout.Button("保存"))
        {
            int thislen = 0;
            foreach (var item in mydata.modelList)
            {
                item.posX = list[thislen].transform.position.x;
                item.posY = list[thislen].transform.position.y;
                item.posZ = list[thislen].transform.position.z;
                item.X = list[thislen].transform.eulerAngles.x;
                item.Y = list[thislen].transform.eulerAngles.y;
                item.Z = list[thislen].transform.eulerAngles.z;
                thislen++;
            }
            EditorManager.Get().SaveMapData();
        }
    }

    public void Add()
    {
        for (int i = 0; i < Root.childCount; i++)
        {
            GameObject gameObject = Root.GetChild(i).gameObject;
            list.Add(gameObject);

            ModelData model = new ModelData();
            model.name = gameObject.name;
            model.modelName = gameObject.name.Replace("(Clone)","");
            model.posX = gameObject.transform.position.x;
            model.posY = gameObject.transform.position.y;
            model.posZ = gameObject.transform.position.z;
            model.X = gameObject.transform.eulerAngles.x;
            model.Y = gameObject.transform.eulerAngles.y;
            model.Z = gameObject.transform.eulerAngles.z;
            if (!str.Contains(gameObject.transform.tag))
            {
                model.modelType = ModelType.Null;
                gameObject.transform.tag = "Null";
            }
            else
            {
                model.modelType = (ModelType)Enum.Parse(typeof(ModelType), gameObject.transform.tag);
            }
            mydata.modelList.Add(model);
        }
    }
    private void OnDestroy()
    {
        foreach (var item in list)
        {
            DestroyImmediate(item);
        }
    }
}
