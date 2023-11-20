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

}
