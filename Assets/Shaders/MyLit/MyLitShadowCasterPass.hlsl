// Pull in custom common toolbox
#include "MyLitCommon.hlsl"

/*------------------------------ Properties ------------------------------*/
// Null

/*------------------------------ Shader ------------------------------*/
// Common unity semantics: https://hackingwithunity.com/semantics-in-shader-unity/
struct Attributes 
{
	float3 positionOS : POSITION; // Position in object space
	float3 normalOS : NORMAL;
#ifdef _ALPHA_CUTOUT
	float2 uv : TEXCOORD0;
#endif
};

struct Interpolators
{
	// This value should contain the position in clip space (which is similar to a position on screen)
	// when output from the vertex function. It will be transformed into pixel position of the current
	// fragment on the screen when read from the fragment function
	float4 positionCS : SV_POSITION;
#ifdef _ALPHA_CUTOUT
	float2 uv : TEXCOORD0;
#endif
};

float3 _LightDirection; // URP provided variable

float4 GetShadowCasterPositionCS(float3 positionWS, float3 normalWS)
{
	float3 lightDirectionWS = _LightDirection;
	float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
#if UNITY_REVERSED_Z
	positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
	positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif
	return positionCS;
}

Interpolators Vertex(Attributes input)
{
	Interpolators output;

	// These helper functions, found in URP/ShaderLib/ShaderVariablesFunctions.hlsl
	// transform object space values into world and clip space
	VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
	VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

	// Pass position data to the fragment function
	output.positionCS = GetShadowCasterPositionCS(posnInputs.positionWS, normInputs.normalWS);
#ifdef _ALPHA_CUTOUT
	output.uv = TRANSFORM_TEX(input.uv, _ColorMap);
#endif

	return output;
}

// The fragment function. This runs once per fragment, which you can think of as a pixel on the screen
// It must output the final color of this pixel
float4 Fragment(Interpolators input) : SV_TARGET
{
#ifdef _ALPHA_CUTOUT
	float2 uv = input.uv;
	float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, input.uv);
	TestAlphaClip(colorSample); // Clipped fragments will not be shown as shadow and will be discarded
#endif
	return 0;
}