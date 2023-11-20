#ifndef MY_LIT_COMMON_INCLUDED
#define MY_LIT_COMMON_INCLUDED

// Pull in URP library functions and our own common functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

/*------------------------------ Properties ------------------------------*/
// Must match the associated property name
float4 _ColorMap1_ST; // Offset variables must be declared as <_TextureVarName>_ST
float4 _ColorTint1;
float  _Cutoff;
float  _NormalStrength1;
float  _MetalnessStrength1;
float3 _SpecularTint1;
float  _Smoothness1;
float3 _EmissionTint1;
float  _ParallaxStrength1;
float _ClearCoatStrength1;
float _ClearCoatSmoothness;

TEXTURE2D(_ColorMap1);  SAMPLER(sampler_ColorMap1); // Sampler variables must be declared as sample_<_TextureVarName>
TEXTURE2D(_NormalMap1); SAMPLER(sampler_NormalMap1);
TEXTURE2D(_MetalnessMap1); SAMPLER(sampler_MetalnessMap1);
TEXTURE2D(_SpecularMap1); SAMPLER(sampler_SpecularMap1);
TEXTURE2D(_SmoothnessMask1); SAMPLER(sampler_SmoothnessMask1);
TEXTURE2D(_EmissionMap1);    SAMPLER(sampler_EmissionMap1);
TEXTURE2D(_ParallaxMap1);    SAMPLER(sampler_ParallaxMap1);
TEXTURE2D(_ClearCoatMask1); SAMPLER(sampler_ClearCoatMask1);
TEXTURE2D(_ClearCoatSmoothnessMask1); SAMPLER(sampler_ClearCoatSmoothnessMask1);

void TestAlphaClip(float4 colorSample)
{
#ifdef _ALPHA_CUTOUT
	clip(colorSample.a * _ColorTint.a - _Cutoff); // Discard any transparent part of the image
#endif
}

#endif