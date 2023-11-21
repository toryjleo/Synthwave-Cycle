using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMixCustomInspector : MyCustomShaderInspector
{
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
