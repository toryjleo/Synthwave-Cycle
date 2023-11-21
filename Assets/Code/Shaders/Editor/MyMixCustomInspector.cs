using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyMixCustomInspector : MyCustomShaderInspector
{
    protected bool foldedOut = true;
    string[] PropertyNames = { "_ColorMap1", "_ColorTint1", "_NormalMap1", "_NormalStrength1", "_MetalnessMap1", 
        "_MetalnessStrength1", "_SpecularMap1", "_SpecularTint1", "_SmoothnessMask1", "_Smoothness1", "_EmissionMap1",
    "_EmissionTint1", "_ClearCoatMask1", "_ClearCoatStrength1", "_ClearCoatSmoothnessMask1", "_ClearCoatSmoothness1",
    "_ParallaxMap1", "_ParallaxStrength1"};
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        foldedOut = EditorGUILayout.BeginFoldoutHeaderGroup(foldedOut, "Properties for Material 1");
        if (foldedOut)
        { 
            
            foreach (string Reference in PropertyNames)
            {
                MaterialProperty SunReference = ShaderGUI.FindProperty(Reference, properties);
                materialEditor.ShaderProperty(SunReference, SunReference.displayName);

                //EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (newShader.name == "Custom/MyMix")
        {
            UpdateSurfaceType(material);
        }
    }

    protected override void ShaderOptimizations(Material material)
    {
        // Only enables based on first material's properties
        EnableDisableKeyword(material, "_ParallaxMap1", "_PARALLAX_MAP");
        EnableDisableKeyword(material, "_ClearCoatMask1", "_CC_MASK");
        EnableDisableKeyword(material, "_ClearCoatSmoothnessMask1", "_CCS_MASK");
        EnableDisableKeyword(material, "_EmissionMap1", "_EMISSION_MAP");
    }
}
