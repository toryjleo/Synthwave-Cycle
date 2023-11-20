using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Adds new dropdowns for customlit shader which sets various hidden shader properties
/// </summary>
public class MyLitCustomInspector : MyCustomShaderInspector 
{
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (newShader.name == "Custom/MyLit")
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
