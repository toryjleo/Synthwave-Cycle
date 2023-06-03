#ifndef MY_LIT_COMMON_INCLUDED
#define MY_LIT_COMMON_INCLUDED

// Pull in URP library functions and our own common functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

/*------------------------------ Properties ------------------------------*/
// Must match the associated property name
float4 _ColorMap_ST; // Offset variables must be declared as <_TextureVarName>_ST
float4 _ColorTint;
float _Cutoff;
float _NormalStrength;
float _MetalnessStrength;
float3 _SpecularTint;
float _Smoothness;

TEXTURE2D(_ColorMap);  SAMPLER(sampler_ColorMap); // Sampler variables must be declared as sample_<_TextureVarName>
TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
TEXTURE2D(_MetalnessMap); SAMPLER(sampler_MetalnessMap);
TEXTURE2D(_SpecularMap); SAMPLER(sampler_SpecularMap);
TEXTURE2D(_SmoothnessMask); SAMPLER(sampler_SmoothnessMask);

void TestAlphaClip(float4 colorSample)
{
#ifdef _ALPHA_CUTOUT
	clip(colorSample.a * _ColorTint.a - _Cutoff); // Discard any transparent part of the image
#endif
}

#endif