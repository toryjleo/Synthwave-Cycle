Shader "Custom/MyMix"
{
    // Properties set per material, exposed in the material inspector
    Properties
    {
        [Header(Surface options)] // Creates a text header

        [Toggle(_ADDITIONAL_LIGHTS)] _EnableMultipleLights("Enable Multiple Lights", Float) = 1.0

        // Format the properties like so: <_PropertyName>(<Material Inspector Name>, <DataType>) <DefaultVal>
        // Convention states that property names start with an underscore
        [HideInInspector] _ColorTint1("[1] Tint", Color) = (1, 1, 1, 1)
        [HideInInspector] _ColorMap1("[1] Color Map", 2D)    = "white" {}
        [HideInInspector] _ColorTint2("[2] Tint", Color) = (1, 1, 1, 1)
        [HideInInspector] _ColorMap2("[2] Color Map", 2D)    = "white" {}
        
        [HideInInspector] [NoScaleOffset][Normal] _NormalMap1("[1] Normal Map", 2D) = "bump" {} // Uses OpenGL bitangent Convention
        [HideInInspector] _NormalStrength1("[1] Normal strength", Range(0, 1)) = 1
        [HideInInspector] [NoScaleOffset][Normal] _NormalMap2("[2] Normal Map", 2D) = "bump" {} // Uses OpenGL bitangent Convention
        [HideInInspector] _NormalStrength2("[2] Normal strength", Range(0, 1)) = 1
        
        [HideInInspector] [NoScaleOffset] _MetalnessMap1("[1] Metalness Map", 2D) = "white" {} // Make sure srgb is turned off in the import settings for textures that are Metalness Map
        [HideInInspector] _MetalnessStrength1("[1] Metalness strength", Range(0, 1)) = 0
        [HideInInspector] [NoScaleOffset] _MetalnessMap2("[2] Metalness Map", 2D) = "white" {} // Make sure srgb is turned off in the import settings for textures that are Metalness Map
        [HideInInspector] _MetalnessStrength2("[2] Metalness strength", Range(0, 1)) = 0


        [HideInInspector] [Toggle(_ROUGHNESS_SETUP1)] _RoughnessMapToggle1("Use roughness map", Float) = 0
        [HideInInspector] [Toggle(_ROUGHNESS_SETUP2)] _RoughnessMapToggle2("Use roughness map", Float) = 0

        [HideInInspector] [NoScaleOffset] _SmoothnessMask1("[1] Smoothness mask", 2D) = "white" {}
        [HideInInspector] _Smoothness1("[1] Smoothness multiplier", Range(0, 1)) = 0.5
        [HideInInspector] [NoScaleOffset] _SmoothnessMask2("[2] Smoothness mask", 2D) = "white" {}
        [HideInInspector] _Smoothness2("[2] Smoothness multiplier", Range(0, 1)) = 0.5

        [HideInInspector] [NoScaleOffset] _EmissionMap1("[1] Emission map", 2D) = "white" {}
        [HideInInspector] [HDR]           _EmissionTint1("[1] Emission tint", Color) = (0, 0, 0, 0)
        [HideInInspector] [NoScaleOffset] _EmissionMap2("[2] Emission map", 2D) = "white" {}
        [HideInInspector] [HDR]           _EmissionTint2("[2] Emission tint", Color) = (0, 0, 0, 0)

        [HideInInspector] [NoScaleOffset] _ClearCoatMask1("[1] Clear coat mask", 2D) = "white" {}
        [HideInInspector] _ClearCoatStrength1("[1] Clear coat strength", Range(0, 1)) = 0
        [HideInInspector] [NoScaleOffset] _ClearCoatMask2("[2] Clear coat mask", 2D) = "white" {}
        [HideInInspector] _ClearCoatStrength2("[2] Clear coat strength", Range(0, 1)) = 0

        [HideInInspector] [NoScaleOffset] _ParallaxMap1("[1] Height/parallax/displacement map", 2D) = "white" {}
        [HideInInspector] _ParallaxStrength1("[1] Parallax strength", Range(0, 1)) = 0.005
        [HideInInspector] [NoScaleOffset] _ParallaxMap2("[2] Height/parallax/displacement map", 2D) = "white" {}
        [HideInInspector] _ParallaxStrength2("[2] Parallax strength", Range(0, 1)) = 0.005

        [HideInInspector] _Cull("Cull mode", Float) = 2
        
        [HideInInspector] _SourceBlend("Source blend", Float) = 0
        [HideInInspector] _DestBlend("Destination blend", Float) = 0
        [HideInInspector] _ZWrite("ZWrite", Float) = 0

        [HideInInspector] _SurfaceType("Surface type", Float) = 0
        [HideInInspector] _BlendType("Blend type", Float) = 0
        [HideInInspector] _FaceRenderingMode("Face rendering type", Float) = 0

        // Noise properties
        [HideInInspector] _SimpleNoiseScaleX("Simple Noise Scale X-axis", Float) = 1
        [HideInInspector] _SimpleNoiseScaleZ("Simple Noise Scale Z-axis", Float) = 1
        [HideInInspector] _SimpleNoiseStrength("Simple Noise Strength", Float) = 0

        [HideInInspector] _ClassicNoiseScaleX("Perlin Noise Scale X-axis", Float) = 1
        [HideInInspector] _ClassicNoiseScaleZ("Perlin Noise Scale Z-axis", Float) = 1
        [HideInInspector] _ClassicNoiseStrength("Perlin Noise Strength", Float) = 0

        [HideInInspector] _CurrentTileX("The x-axis of the current tile", Float) = 0
        [HideInInspector] _CurrentTileZ("The z-axis of the current tile", Float) = 0
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
            #pragma shader_feature_local _ROUGHNESS_SETUP1
            #pragma shader_feature_local _ROUGHNESS_SETUP2
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON


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
            #include "MyMixForwardLitPass.hlsl"
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

            #include "../MyLit/MyLitShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "MyMixCustomInspector"
}
