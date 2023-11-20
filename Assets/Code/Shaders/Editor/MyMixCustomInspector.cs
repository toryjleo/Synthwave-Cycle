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
        EnableDisableKeyword(material, "_ParallaxMap", "_ParMap");
        EnableDisableKeyword(material, "_ClearCoatMask", "_CCMask");
        EnableDisableKeyword(material, "_ClearCoatSmoothnessMask", "_CCSMask");
        EnableDisableKeyword(material, "_EmissionMap", "_EmissionMap");
    }
}
