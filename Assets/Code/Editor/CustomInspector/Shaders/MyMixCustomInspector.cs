using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyMixCustomInspector : MyCustomShaderInspector
{
    string[] MaterialPropertyNames = { "_ColorMap", "_ColorTint", "_NormalMap", "_NormalStrength", "_MetalnessMap",
    "_MetalnessStrength", "_RoughnessMapToggle", "_SmoothnessMask", "_Smoothness", "_EmissionMap",
    "_EmissionTint", "_ClearCoatMask", "_ClearCoatStrength",
    "_ParallaxMap", "_ParallaxStrength"};
    string[] RandomPropertyNames = { "_SimpleNoiseStrength", "_SimpleNoiseScaleX", "_SimpleNoiseScaleZ",
    "_ClassicNoiseStrength", "_ClassicNoiseScaleX", "_ClassicNoiseScaleZ", "_CurrentTileX", "_CurrentTileZ" };

    protected bool foldedOutMat1 = false;
    protected bool foldedOutMat2 = false;
    protected bool foldedOutRando = false;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        foldedOutMat1 = CreateMaterialDropdown(materialEditor, properties, '1', foldedOutMat1);
        foldedOutMat2 = CreateMaterialDropdown(materialEditor, properties, '2', foldedOutMat2);
        foldedOutRando = CreateRandoAlgoDropdown(materialEditor, properties, foldedOutRando);
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (newShader.name == "Custom/MyMix")
        {
            UpdateSurfaceType(material);
        }
    }

    private bool CreateMaterialDropdown(MaterialEditor materialEditor, MaterialProperty[] properties, char groupNum, bool foldedOut) 
    {
        bool retVal = EditorGUILayout.BeginFoldoutHeaderGroup(foldedOut, "Properties for Material " + groupNum);
        if (retVal)
        {

            foreach (string Reference in MaterialPropertyNames)
            {
                MaterialProperty Property = ShaderGUI.FindProperty(Reference + groupNum, properties);
                materialEditor.ShaderProperty(Property, Property.displayName);

                //EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        return retVal;
    }

    private bool CreateRandoAlgoDropdown(MaterialEditor materialEditor, MaterialProperty[] properties, bool foldedOut)
    {
        bool retVal = EditorGUILayout.BeginFoldoutHeaderGroup(foldedOut, "Random Algorithm Adjustments");
        if (retVal)
        {

            foreach (string Reference in RandomPropertyNames)
            {
                MaterialProperty Property = ShaderGUI.FindProperty(Reference, properties);
                materialEditor.ShaderProperty(Property, Property.displayName);

                //EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        return retVal;
    }

    protected override void ShaderOptimizations(Material material)
    {
    }
}
