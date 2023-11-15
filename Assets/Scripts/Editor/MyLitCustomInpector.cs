using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Adds new dropdowns for customlit shader which sets various hidden shader properties
/// </summary>
public class MyLitCustomInspector : ShaderGUI
{
    public enum SurfaceType
    {
        Opaque, TransparentBlend, TransparentCutout
    }

    public enum FaceRenderingMode 
    {
        FrontOnly, NoCulling, DoubleSided
    }

    public enum BlendType
    {
        Alpha, Premultiplied, Additive, Multiply
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (newShader.name == "Unlit/MyLit")
        {
            UpdateSurfaceTypeMyLit(material);
        }
    }

    /// <summary>
    /// Adds dropdown for various shader features
    /// </summary>
    /// <param name="materialEditor"></param>
    /// <param name="properties"></param>
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {

        Material material = materialEditor.target as Material;
        var surfaceProp = BaseShaderGUI.FindProperty("_SurfaceType", properties, true);
        var blendProp   = BaseShaderGUI.FindProperty("_BlendType", properties, true);
        var faceProp    = BaseShaderGUI.FindProperty("_FaceRenderingMode", properties, true);

        EditorGUI.BeginChangeCheck();

        surfaceProp.floatValue = (int)(SurfaceType)EditorGUILayout.EnumPopup("Surface type", (SurfaceType)surfaceProp.floatValue);
        blendProp.floatValue = (int)(BlendType)EditorGUILayout.EnumPopup("Blend Type", (BlendType)blendProp.floatValue);
        faceProp.floatValue    = (int)(FaceRenderingMode)EditorGUILayout.EnumPopup("Face rendering mode", (FaceRenderingMode)faceProp.floatValue);
        
        base.OnGUI(materialEditor, properties);
        
        if (EditorGUI.EndChangeCheck())
        {
            UpdateSurfaceTypeMyLit(material);
        }

    }

    /// <summary>
    /// Sets various surface properties of the shader, using dropdown menues
    /// </summary>
    /// <param name="material">Material to be impacted</param>
    private void UpdateSurfaceTypeMyLit(Material material)
    {
        //--- SurfaceType ---
        SurfaceType surface = (SurfaceType)material.GetFloat("_SurfaceType");
        switch (surface)
        {
            case SurfaceType.Opaque:
                material.renderQueue = (int)RenderQueue.Geometry;
                material.SetOverrideTag("RenderType", "Opaque");
                break;
            case SurfaceType.TransparentCutout:
                material.renderQueue = (int)RenderQueue.AlphaTest;
                material.SetOverrideTag("RenderType", "TransparentCutout");
                break;
            case SurfaceType.TransparentBlend:
                material.renderQueue = (int)RenderQueue.Transparent;
                material.SetOverrideTag("RenderType", "Transparent");
                break;
        }

        switch (surface)
        {
            case SurfaceType.Opaque:
            case SurfaceType.TransparentCutout:
                material.SetInt("_SourceBlend", (int)BlendMode.One);
                material.SetInt("_DestBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                break;
            case SurfaceType.TransparentBlend:
                material.SetInt("_SourceBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DestBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                break;
        }

        material.SetShaderPassEnabled("ShadowCaster", surface != SurfaceType.TransparentBlend);

        if (surface == SurfaceType.TransparentCutout) 
        {
            material.EnableKeyword("_ALPHA_CUTOUT");
        }
        else 
        {
            material.DisableKeyword("_ALPHA_CUTOUT");
        }

        //--- FaceRenderingMode ---
        FaceRenderingMode faceRenderingMode = (FaceRenderingMode)material.GetFloat("_FaceRenderingMode");
        if (faceRenderingMode == FaceRenderingMode.FrontOnly)
        {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);
        }
        else
        {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }

        if (faceRenderingMode == FaceRenderingMode.DoubleSided)
        {
            material.EnableKeyword("_DOUBLE_SIDED_NORMALS");
        }
        else
        {
            material.DisableKeyword("_DOUBLE_SIDED_NORMALS");
        }

        // --- BlendType ---
        BlendType blend = (BlendType)material.GetFloat("_BlendType");
        switch (surface)
        {
            case SurfaceType.Opaque:
            case SurfaceType.TransparentCutout:
                material.SetInt("_SourceBlend", (int)BlendMode.One);
                material.SetInt("_DestBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                break;
            case SurfaceType.TransparentBlend:
                switch (blend)
                {
                    case BlendType.Alpha:
                        material.SetInt("_SourceBlend", (int)BlendMode.SrcAlpha);
                        material.SetInt("_DestBlend", (int)BlendMode.OneMinusSrcAlpha);
                        break;
                    case BlendType.Premultiplied:
                        material.SetInt("_SourceBlend", (int)BlendMode.One);
                        material.SetInt("_DestBlend", (int)BlendMode.OneMinusSrcAlpha);
                        break;
                    case BlendType.Additive:
                        material.SetInt("_SourceBlend", (int)BlendMode.SrcAlpha);
                        material.SetInt("_DestBlend", (int)BlendMode.One);
                        break;
                    case BlendType.Multiply:
                        material.SetInt("_SourceBlend", (int)BlendMode.Zero);
                        material.SetInt("_DestBlend", (int)BlendMode.SrcColor);
                        break;
                }
                material.SetInt("_ZWrite", 0);
                break;
        }
        if (surface == SurfaceType.TransparentBlend && blend == BlendType.Premultiplied)
        {
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        else
        {
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
    
        // --- Further Optimizations ---
        EnableDisableKeyword(material, "_ParallaxMap", "_ParMap");
        EnableDisableKeyword(material, "_ClearCoatMask", "_CCMask");
        EnableDisableKeyword(material, "_ClearCoatSmoothnessMask", "_CCSMask");
        EnableDisableKeyword(material, "_EmissionMap", "_EmissionMap");
    }

/// <summary>
/// Disables functionality for a given map if it is not assigned in-editor
/// </summary>
/// <param name="material">Material to be impacted</param>
/// <param name="mapName">Name of the map to check</param>
/// <param name="compilerPragma">Name of the compiler directive to enable/disable</param>
    private void EnableDisableKeyword(Material material, string mapName, string compilerPragma)
    {
        if(material.GetTexture(mapName) == null)
        {
            material.DisableKeyword(compilerPragma);
        }
        else
        {
            material.EnableKeyword(compilerPragma);
        }
    }

}
