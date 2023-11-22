using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyMixCustomInspector : MyCustomShaderInspector
{
    string[] PropertyNames = { "_ColorMap", "_ColorTint", "_NormalMap", "_NormalStrength", "_MetalnessMap",
    "_MetalnessStrength", "_RoughnessMapToggle", "_SmoothnessMask", "_Smoothness", "_SpecularMap", "_SpecularTint", "_EmissionMap",
    "_EmissionTint", "_ClearCoatMask", "_ClearCoatStrength", "_ClearCoatSmoothnessMask", "_ClearCoatSmoothness",
    "_ParallaxMap", "_ParallaxStrength"};

    protected bool foldedOutMat1 = false;
    protected bool foldedOutMat2 = false;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        foldedOutMat1 = CreateMaterialDropdown(materialEditor, properties, '1', foldedOutMat1);
        foldedOutMat2 = CreateMaterialDropdown(materialEditor, properties, '2', foldedOutMat2);
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

            foreach (string Reference in PropertyNames)
            {
                MaterialProperty Property = ShaderGUI.FindProperty(Reference + groupNum, properties);
                materialEditor.ShaderProperty(Property, Property.displayName);

                //EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        return retVal;
    }

    protected override void ShaderOptimizations(Material material)
    {
        // Only enables based on first material's properties
        /*
        EnableDisableKeyword(material, "_ParallaxMap1", "_PARALLAX_MAP1");
        EnableDisableKeyword(material, "_ClearCoatMask1", "_CC_MASK");
        EnableDisableKeyword(material, "_ClearCoatSmoothnessMask1", "_CCS_MASK");
        EnableDisableKeyword(material, "_EmissionMap1", "_EMISSION_MAP");
        */
    }
}
