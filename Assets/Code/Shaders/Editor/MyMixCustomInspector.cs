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
        EnableDisableKeyword(material, "_ParallaxMap1", "_ParMap1");
        EnableDisableKeyword(material, "_ClearCoatMask1", "_CCMask");
        EnableDisableKeyword(material, "_ClearCoatSmoothnessMask1", "_CCSMask");
        EnableDisableKeyword(material, "_EmissionMap1", "_EmissionMap");
    }
}
