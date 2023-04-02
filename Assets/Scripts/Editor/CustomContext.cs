using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Creates extra options in the Unity context menu
/// </summary>
public class CustomContext
{

    private const string CUSTOM_LIT_SHADER_DIR = "Assets/Scripts/Editor/";
    private const string CUSTOM_LIT_SHADER_NAME = "CustomLit.shader";
    private const string PHYSICALLY_BASED_NAME = "PBR.shader";

    /// <summary>
    /// 
    /// </summary>
    [MenuItem("Assets/Create/Shader/Universal Render Pipeline/Lit HLSL")]
    private static void CreateCustomLitShader() 
    {
        StreamReader reader = new StreamReader(CUSTOM_LIT_SHADER_DIR + CUSTOM_LIT_SHADER_NAME);
        string shaderRaw = reader.ReadToEnd();
        reader.Close();
        ProjectWindowUtil.CreateAssetWithContent(
        CUSTOM_LIT_SHADER_NAME,
        shaderRaw);
    }

    /// <summary>
    /// 
    /// </summary>
    [MenuItem("Assets/Create/Shader/Universal Render Pipeline/PBR_HLSL")]
    private static void CreatePBRShader()
    {
        StreamReader reader = new StreamReader(CUSTOM_LIT_SHADER_DIR + PHYSICALLY_BASED_NAME);
        string shaderRaw = reader.ReadToEnd();
        reader.Close();
        ProjectWindowUtil.CreateAssetWithContent(
        PHYSICALLY_BASED_NAME,
        shaderRaw);
    }
}
