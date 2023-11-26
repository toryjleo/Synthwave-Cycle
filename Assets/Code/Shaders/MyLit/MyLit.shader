Shader "Custom/MyLit"
{
    // Properties set per material, exposed in the material inspector
    Properties
    {
        [Header(Surface options)] // Creates a text header

        [Toggle(_ADDITIONAL_LIGHTS)] _EnableMultipleLights("Enable Multiple Lights", Float) = 1.0

        // Format the properties like so: <_PropertyName>(<Material Inspector Name>, <DataType>) <DefaultVal>
        // Convention states that property names start with an underscore
        _ColorTint("Tint", Color) = (1, 1, 1, 1)
        _ColorMap("Color Map", 2D)    = "white" {}
        
        [NoScaleOffset][Normal] _NormalMap("Normal Map", 2D) = "bump" {} // Uses OpenGL bitangent Convention
        _NormalStrength("Normal strength", Range(0, 1)) = 1
        
        [NoScaleOffset] _MetalnessMap("Metalness Map", 2D) = "white" {} // Make sure srgb is turned off in the import settings for textures that are Metalness Map
        _MetalnessStrength("Metalness strength", Range(0, 1)) = 0
        
        [Toggle(_SPECULAR_SETUP)] _SpecularSetupToggle("Use specular workflow", Float) = 0
        [NoScaleOffset] _SpecularMap("Specular map", 2D) = "white" {}
        _SpecularTint("Specular tint", Color) = (1, 1, 1, 1)

        [Toggle(_ROUGHNESS_SETUP)] _RoughnessMapToggle("Use roughness map", Float) = 0
        [NoScaleOffset] _SmoothnessMask("Smoothness mask", 2D) = "white" {}
        _Smoothness("Smoothness multiplier", Range(0, 1)) = 0.5

        [NoScaleOffset] _EmissionMap("Emission map", 2D) = "white" {}
        [HDR]           _EmissionTint("Emission tint", Color) = (0, 0, 0, 0)

        [NoScaleOffset] _ClearCoatMask("Clear coat mask", 2D) = "white" {}
        _ClearCoatStrength("Clear coat strength", Range(0, 1)) = 0
        [NoScaleOffset] _ClearCoatSmoothnessMask("Clear coat smoothness mask", 2D) = "white" {}
        _ClearCoatSmoothness("Clear coat smoothness", Range(0, 1)) = 0

        [NoScaleOffset] _ParallaxMap("Height/parallax/displacement map", 2D) = "white" {}
        _ParallaxStrength("Parallax strength", Range(0, 1)) = 0.005

        [HideInInspector] _Cull("Cull mode", Float) = 2
        
        [HideInInspector] _SourceBlend("Source blend", Float) = 0
        [HideInInspector] _DestBlend("Destination blend", Float) = 0
        [HideInInspector] _ZWrite("ZWrite", Float) = 0

        [HideInInspector] _SurfaceType("Surface type", Float) = 0
        [HideInInspector] _BlendType("Blend type", Float) = 0
        [HideInInspector] _FaceRenderingMode("Face rendering type", Float) = 0
    }
    // SubShaaders allow for different behavior and optionf for different pipelines and platforms
    SubShader
    {
        // These tags are shared by all passes in this sub shader
        // This SubShader is tagged for use in the UniversalPipeline
        Tags{"RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque"}

        // Shaders can have several passes which are used to render different data about the material
        // Each pass has it's own vertex and fragment function and shader variant keywords
        Pass
        {
            Name "ForwardLit" // For debugging
            Tags{"LightMode" = "UniversalForward"} // Pass specific tags. UniversalForward is the value for a color pass
    
            Blend[_SourceBlend][_DestBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM // Begin HLSL code

            #define _NORMALMAP
            #define _CLEARCOATMAP
            #pragma shader_feature_local _ADDITIONAL_LIGHTS
            #pragma shader_feature_local _ALPHA_CUTOUT
            #pragma shader_feature_local _DOUBLE_SIDED_NORMALS
            #pragma shader_feature_local _SPECULAR_SETUP
            #pragma shader_feature_local _ROUGHNESS_SETUP
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _ParMap
            #pragma shader_feature_local _CCMask
            #pragma shader_feature_local _CCSMask
            #pragma shader_feature_local _EmissionMap


#if UNITY_VERSION >= 202120
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#else
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS // Tells unity to compile a version of the shader that works with and without main light shadows. This creates variants of the forward lit pass
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE // Handles soft shadows automatically
#endif
            #pragma multi_compile_fragment _ _SHADOWS_SOFT      // Handles soft shadows automatically
#if UNITY_VERSION >= 202120
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
#endif


            // Register our programmable stage functions
            #pragma vertex Vertex     // Register my custom vertex shader function
            #pragma fragment Fragment // Register my custom fragment shader function

            // Inlcude my custom HLSL code
            #include "MyLitForwardLitPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}

            ColorMask 0 // Since we do not need color, turn off color to optimize this shader

            Cull[_Cull]
            
            HLSLPROGRAM

            #pragma shader_feature_local _ALPHA_CUTOUT
            #pragma shader_feature_local _DOUBLE_SIDED_NORMALS

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "MyLitShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "MyLitCustomInspector"
}
