﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using EDT;
using MAP;
using MapMagic;
using ACT;
using System.Reflection;
using UnityEditor.Animations;

public class EditorMenu : Editor
{
    [MenuItem("工具/添加Colldier")]
    static void AttachCollider()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject go = gameObjects[i];
            MeshFilter fillter = go.GetComponent<MeshFilter>();
            if (fillter != null && fillter.sharedMesh)
            {
                BoxCollider collider = go.GET<BoxCollider>();
                collider.size = fillter.sharedMesh.bounds.size;
                collider.center = fillter.sharedMesh.bounds.center;
            }
        }
    }

    [MenuItem("工具/添加CharacterController")]
    static void AttachCharacterController()
    {
        GameObject go = Selection.activeGameObject;
        SkinnedMeshRenderer fillter = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (fillter != null && fillter.sharedMesh)
        {
            CharacterController cc = go.GET<CharacterController>();
            cc.radius = (fillter.bounds.size.x + fillter.bounds.size.z) / 4f;
            cc.center = fillter.bounds.center;
            cc.height = fillter.bounds.size.y;

        }
    }

    [MenuItem("工具/获取网格数据")]
    static void GetMeshData()
    {
        if (Selection.activeObject == null)
        {
            return;
        }
        GameObject go = Selection.activeObject as GameObject;
        if (go == null)
            return;
        MeshFilter filter = go.GetComponent<MeshFilter>();
        if (filter == null || filter.sharedMesh == null)
        {
            return;
        }
        string s = string.Empty;
        for (int i = 0; i < filter.sharedMesh.triangles.Length; i++)
        {
            s += filter.sharedMesh.triangles[i] + ",";
        }
        Debug.Log("triangles:" + s);

        string m = string.Empty;
        for (int i = 0; i < filter.sharedMesh.vertices.Length; i++)
        {
            m += filter.sharedMesh.vertices[i].ToString() + ",";
        }
        Debug.Log("vertices:" + m);
    }

    [MenuItem("工具/替换黑乎乎的Shader")]
    static void ReplaceDiffuse()
    {
        Shader newShader = Shader.Find("MyMobile/Diffuse");
        List<Transform> list = new List<Transform>();
        GTTools.FindAllChildren(Selection.activeGameObject.transform, list);
        for (int i = 0; i < list.Count; i++)
        {
            GameObject go = list[i].gameObject;
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                continue;
            }
            Material[] materials = renderer.sharedMaterials;
            for (int k = 0; k < materials.Length; k++)
            {
                Material mat = materials[k];
                if (mat.shader.name == "MyMobile/Diffuse_old" || mat.shader == null)
                {
                    mat.shader = newShader;
                }
            }
        }
    }

    [MenuItem("工具/MyMobile Diffuse 换 MyMobile VertexLit Diffuse")]
    static void ReplaceDiffuse1()
    {
        ReplaceShader("MyMobile/VertexLit/Diffuse", "MyMobile/Diffuse");
    }

    [MenuItem("工具/MyMobile Diffuse_old 换 MyMobile VertexLit Diffuse")]
    static void ReplaceDiffuse2()
    {
        ReplaceShader("MyMobile/VertexLit/Diffuse", "MyMobile/Diffuse_old");
    }

    static void ReplaceShader(string shaderName1, string shaderName2)
    {
        Shader newShader = Shader.Find(shaderName1);
        List<Transform> list = new List<Transform>();
        GTTools.FindAllChildren(Selection.activeGameObject.transform, list);
        for (int i = 0; i < list.Count; i++)
        {
            GameObject go = list[i].gameObject;
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                continue;
            }
            Material[] materials = renderer.sharedMaterials;
            for (int k = 0; k < materials.Length; k++)
            {
                Material mat = materials[k];
                if (mat.shader.name == shaderName2 || mat.shader == null)
                {
                    mat.shader = newShader;
                }
            }
        }
    }

    [MenuItem("工具/一键替换字体")]
    static void ReplaceFont()
    {
        List<string> files = new List<string>();
        GTTools.GetDirs(Application.dataPath + "/Resources/Guis", ref files);
        string replacePath = Application.dataPath.Replace("//Assets", string.Empty);
        List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
        for (int i = 0; i < files.Count; i++)
        {
            if (!files[i].EndsWith(".prefab"))
            {
                continue;
            }

            string s = "Assets" + files[i].Replace(replacePath, string.Empty);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
            if (obj != null)
            {
                objs.Add(obj);
            }
        }

        Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/Droid Sans.ttf");
        NGUIText.dynamicFont = font;
        NGUISettings.ambigiousFont = font;
        for (int i = 0; i < objs.Count; i++)
        {
            UnityEngine.GameObject go = (GameObject)objs[i];
            if (go == null) return;
            GameObject clone = GameObject.Instantiate(go);
            UILabel[] labels = clone.GetComponentsInChildren<UILabel>(true);
            foreach (var current in labels)
            {
                current.ambigiousFont = font;
                current.trueTypeFont = font;
            }
            PrefabUtility.ReplacePrefab(clone, go);
            GameObject.DestroyImmediate(clone);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

    }

    [MenuItem("工具/自动更改ShaderError")]
    static void AutoChangeShaderError()
    {
        Shader newShader = Shader.Find("Mobile/Diffuse");
        Texture2D t2d = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Res/Texture/terrain_yewai_jinglingcaodi.tga");
        UnityEngine.GameObject[] objs = Selection.gameObjects;
        for (int i = 0; i < objs.Length; i++)
        {
            UnityEngine.GameObject child = objs[i];
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            renderer.sharedMaterial.shader = newShader;
            renderer.sharedMaterial.SetTexture("_MainTex", t2d);
            renderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(10, 10));
        }
        AssetDatabase.Refresh();
    }


    [MenuItem("编辑器/打包")]
    static void Build()
    {
        EditorResourceBuild.Build();
    }


    [MenuItem("编辑器/创建地图场景")]
    static void BuildLandscapeMap()
    {
        MAPWorldLandscape ftWorldLandscapeMap = new GameObject("MAPWorldLandscape").AddComponent<MAPWorldLandscape>();
        ftWorldLandscapeMap.AddGroups();
    }

    [MenuItem("编辑器/创建关卡部署")]
    static void BuildMap()
    {
        MAPWorldMap ftWorldMap = new GameObject("MAPWorldMap").AddComponent<MAPWorldMap>();
        ftWorldMap.AddGroups();
        ftWorldMap.transform.name = string.Format("关卡部署");
    }

    [MenuItem("编辑器/自动生成地形")]
    static void BuildMagicWorld()
    {
        MapMagic.MapMagic g = new GameObject().AddComponent<MapMagic.MapMagic>();
        g.gens = GeneratorsAsset.Default();
        g.guiGens = g.gens;
        EditorUtility.SetDirty(g);
    }

    [MenuItem("编辑器/删除无效材质")]
    static void DeleteInvalidMat()
    {
        UnityEngine.Object[] mats = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        for (int i = 0; i < mats.Length; i++)
        {
            Debug.Log(mats[i].name);
        }
    }

    [MenuItem("编辑器/创建ControlLayer")]
    static void CreateSkill()
    {
        AnimatorController ac = Selection.activeObject as AnimatorController;
        if (ac == null)
        {
            return;
        }
        AnimatorControllerLayer layer = new AnimatorControllerLayer();
        GTTools.Copy(layer, ac.layers[1]);
        layer.name = "Layer2";
        ac.AddLayer(layer);
        AssetDatabase.Refresh();
    }

    [MenuItem("编辑器/修改ControlLayer名字")]
    static void ChangeLayerStateName()
    {
        AnimatorController ac = Selection.activeObject as AnimatorController;
        if (ac == null)
        {
            return;
        }
        for (int i = 1; i < 3; i++)
        {
            AnimatorControllerLayer layer = ac.layers[i];
            for (int k = 0; k < layer.stateMachine.states.Length; k++)
            {
                string name = layer.stateMachine.states[k].state.name;
                if (name.Contains("Skill") && name.Contains("Do") == false)
                {
                    layer.stateMachine.states[k].state.name = "Do" + name;
                }
                if (name.Contains("Combo") && name.Contains("Do") == false)
                {
                    layer.stateMachine.states[k].state.name = "Do" + name;
                }

                if (name.Contains("_") &&
                    name.Contains("_0") == false)
                {
                    layer.stateMachine.states[k].state.name = name.Replace("_", "_0");
                }
            }
        }

        for (int i = 0; i < ac.parameters.Length; i++)
        {
            string paramName = ac.parameters[i].name;
            if (paramName.Contains("_") &&
                paramName.Contains("_0") == false)
            {
                Debug.LogError(paramName.Replace("_", "_0"));

                AnimatorControllerParameter d = new AnimatorControllerParameter();
                GTTools.Copy(d, ac.parameters[i]);
                d.name = paramName.Replace("_", "_0");
                ac.parameters[i] = d;
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/提取新动画")]
    static void GetFilteredtoNewAnim()
    {
        UnityEngine.Object[] selectionAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
        foreach (UnityEngine.Object asset in selectionAsset)
        {
            string   path1      = AssetDatabase.GetAssetPath(asset);
            string   path2      = path1.Replace("Assets/Res/Character/", string.Empty);
            string[] pathArray  = path2.Split('/');
            string   parentPath = pathArray[0];
            string   dir        = "Assets/Res/Actor/" + parentPath + "/Anim";
            string   path3      = dir + "/" + asset.name + ".anim";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            AnimationClip newClip = new AnimationClip();
            EditorUtility.CopySerialized(asset, newClip);
            AssetDatabase.CreateAsset(newClip, path3);
        }
        AssetDatabase.Refresh();
    }
}
