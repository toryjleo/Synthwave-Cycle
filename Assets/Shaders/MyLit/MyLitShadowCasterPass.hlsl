// Pull in URP library functions and our own common functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

/*------------------------------ Properties ------------------------------*/
// Null

/*------------------------------ Shader ------------------------------*/
// Common unity semantics: https://hackingwithunity.com/semantics-in-shader-unity/
struct Attributes 
{
	float3 positionOS : POSITION; // Position in object space
	float3 normalOS : NORMAL;
};

struct Interpolators
{
	// This value should contain the position in clip space (which is similar to a position on screen)
	// when output from the vertex function. It will be transformed into pixel position of the current
	// fragment on the screen when read from the fragment function
	float4 positionCS : SV_POSITION;
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

	return output;
}

// The fragment function. This runs once per fragment, which you can think of as a pixel on the screen
// It must output the final color of this pixel
float4 Fragment(Interpolators input) : SV_TARGET
{
	return 0;
}