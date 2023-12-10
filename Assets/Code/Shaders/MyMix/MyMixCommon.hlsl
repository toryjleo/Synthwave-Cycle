#ifndef MY_LIT_COMMON_INCLUDED
#define MY_LIT_COMMON_INCLUDED

// Pull in URP library functions and our own common functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

/*------------------------------ Properties ------------------------------*/
// Must match the associated property name
float4 _ColorMap1_ST; // Offset variables must be declared as <_TextureVarName>_ST
float4 _ColorMap2_ST;
float4 _ColorTint1;
float4 _ColorTint2;
float  _Cutoff;
float  _NormalStrength1;
float  _NormalStrength2;
float  _MetalnessStrength1;
float  _MetalnessStrength2;
float  _Smoothness1;
float  _Smoothness2;
float3 _EmissionTint1;
float3 _EmissionTint2;
float  _ParallaxStrength1;
float  _ParallaxStrength2;
float  _ClearCoatStrength1;
float  _ClearCoatStrength2;
float  _ClearCoatSmoothness1;
float  _ClearCoatSmoothness2;

float _SimpleNoiseScale;
float _SimpleNoiseStrength;
float _ClassicNoiseScale;
float _ClassicNoiseStrength;

TEXTURE2D(_ColorMap1);  SAMPLER(sampler_ColorMap1); // Sampler variables must be declared as sample_<_TextureVarName>
TEXTURE2D(_ColorMap2);  SAMPLER(sampler_ColorMap2);
TEXTURE2D(_NormalMap1); SAMPLER(sampler_NormalMap1);
TEXTURE2D(_NormalMap2); SAMPLER(sampler_NormalMap2);
TEXTURE2D(_MetalnessMap1); SAMPLER(sampler_MetalnessMap1);
TEXTURE2D(_MetalnessMap2); SAMPLER(sampler_MetalnessMap2);
TEXTURE2D(_SmoothnessMask1); SAMPLER(sampler_SmoothnessMask1);
TEXTURE2D(_SmoothnessMask2); SAMPLER(sampler_SmoothnessMask2);
TEXTURE2D(_EmissionMap1);    SAMPLER(sampler_EmissionMap1);
TEXTURE2D(_EmissionMap2);    SAMPLER(sampler_EmissionMap2);
TEXTURE2D(_ParallaxMap1);    SAMPLER(sampler_ParallaxMap1);
TEXTURE2D(_ParallaxMap2);    SAMPLER(sampler_ParallaxMap2);
TEXTURE2D(_ClearCoatMask1); SAMPLER(sampler_ClearCoatMask1);
TEXTURE2D(_ClearCoatMask2); SAMPLER(sampler_ClearCoatMask2);

void TestAlphaClip(float4 colorSample)
{
#ifdef _ALPHA_CUTOUT
	clip(colorSample.a * _ColorTint.a - _Cutoff); // Discard any transparent part of the image
#endif
}

#endif