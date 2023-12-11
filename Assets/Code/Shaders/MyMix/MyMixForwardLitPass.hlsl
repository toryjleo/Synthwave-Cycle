// Pull in custom common toolbox
#include "MyMixCommon.hlsl"
#include "MyRandom.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"

/*------------------------------ Shader ------------------------------*/
// Common unity semantics: https://hackingwithunity.com/semantics-in-shader-unity/
struct Attributes 
{
	float3 positionOS : POSITION; // Position in object space
	float2 uv         : TEXCOORD0; // Material texture UVs
	float3 normalOS   : NORMAL; // Normal in object space
	float4 tangentOS  : TANGENT;
};

struct Interpolators
{
	// This value should contain the position in clip space (which is similar to a position on screen)
	// when output from the vertex function. It will be transformed into pixel position of the current
	// fragment on the screen when read from the fragment function
	float4 positionCS : SV_POSITION;
	// The following variables will retain their values from the vertex stage, except the
	// rasterizer will interpolate them between vertices
	float2 uv         : TEXCOORD0; // Material texture UVs
	float3 positionWS : TEXCOORD1;
	float3 normalWS   : TEXCOORD2;
	float4 tangentWS  : TEXCOORD3;
};

Interpolators Vertex(Attributes input)
{
	Interpolators output;

	// These helper functions, found in URP/ShaderLib/ShaderVariablesFunctions.hlsl
	// transform object space values into world and clip space
	VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
	VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);

	// Pass position and orientation data to the fragment function
	output.positionCS = posnInputs.positionCS;
	output.uv = TRANSFORM_TEX(input.uv, _ColorMap1); // Also applies offset variables (<name>_ST)
	output.positionWS = posnInputs.positionWS;
	output.normalWS = normInputs.normalWS;
	output.tangentWS = float4(normInputs.tangentWS, input.tangentOS.w);

	return output;
}

float SampleSmoothness(Texture2D smoothnessMap, SamplerState smoothnessMapSampler, float smoothnessStrength, float2 uv)
{
    return SAMPLE_TEXTURE2D(smoothnessMap, smoothnessMapSampler, uv).r * smoothnessStrength;
}


float4 RunUniversalPBR(Texture2D colorMap, SamplerState colorMapSampler, float4 colorTint,
                       Texture2D normalMap, SamplerState normalMapSampler, float normalStrength,
					   Texture2D metalnessMap, SamplerState metalnessMapSampler, float metalnessStrength,
				       float smoothnessSample,
					   Texture2D emissionMap, SamplerState emissionMapSampler, float3 emissionTint,
					   Texture2D clearCoatMask, SamplerState clearCoatSampler, float clearCoatStrength,
                       float2 uv, float3 normalWS, float3 positionWS, float4 tangentWS, float4 positionCS, float3 viewDirWS
					   
#ifdef _DOUBLE_SIDED_NORMALS
	, FRONT_FACE_TYPE frontFace : FRONT_FACE_SEMANTIC
#endif
)
{
	
	// Get Normal map
    float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(normalMap, normalMapSampler, uv), normalStrength);
    float3x3 tangentToWorld = CreateTangentToWorld(normalWS, tangentWS.xyz, tangentWS.w);
    normalWS = normalize(TransformTangentToWorld(normalTS, tangentToWorld));
	//return float4((normalWS + 1) * 0.5, 1); // Returns normals TEST
	
	// Color Sample
    float4 colorSample = SAMPLE_TEXTURE2D(colorMap, colorMapSampler, uv);
    TestAlphaClip(colorSample);
	
	// Update lighting input data for lighting calculations
    InputData lightingInput = (InputData) 0;
    lightingInput.positionWS = positionWS;
    lightingInput.normalWS = normalWS;
    lightingInput.viewDirectionWS = viewDirWS;
    lightingInput.shadowCoord = TransformWorldToShadowCoord(positionWS); // Sample the shadow coord from the shadow map
#if UNITY_VERSION >= 202120
	lightingInput.positionCS = positionCS;
	lightingInput.tangentToWorld = tangentToWorld;
#endif
	
    SurfaceData surfaceInput = (SurfaceData) 0;
    surfaceInput.albedo = colorSample.rgb * colorTint.rgb;
    surfaceInput.alpha = colorSample.a * colorTint.a;
    surfaceInput.specular = 1;
    surfaceInput.normalTS = normalTS;
	
    surfaceInput.metallic = SAMPLE_TEXTURE2D(metalnessMap, metalnessMapSampler, uv).r * metalnessStrength;
	
    surfaceInput.smoothness = smoothnessSample;
	surfaceInput.emission   = SAMPLE_TEXTURE2D(emissionMap, emissionMapSampler, uv).rgb * emissionTint;
	surfaceInput.clearCoatMask = SAMPLE_TEXTURE2D(clearCoatMask, clearCoatSampler, uv).r * clearCoatStrength;
    return UniversalFragmentPBR(lightingInput, surfaceInput);
    return float4(1, 1, 1, 1);
}

float4 GetColorsMaterial1(Interpolators input)
{
    float3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS); // In ShaderVariablesFunctions.hlsl
    float3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, viewDirWS); // In ParallaxMapping.hlsl, normal must NOT be normalized

	
	// Height Map Calculation For First Material
    float2 uv1 = input.uv;
    uv1 += ParallaxMapping(TEXTURE2D_ARGS(_ParallaxMap1, sampler_ParallaxMap1), viewDirTS, _ParallaxStrength1, uv1);
	
	// Smoothness Map Calculation For First Material
    float smoothnessSample1 = SampleSmoothness(_SmoothnessMask1, sampler_SmoothnessMask1, _Smoothness1, uv1);
#ifdef _ROUGHNESS_SETUP1
	smoothnessSample1 = 1 - smoothnessSample1;
#endif
	
    return RunUniversalPBR(_ColorMap1, sampler_ColorMap1, _ColorTint1,
	_NormalMap1, sampler_NormalMap1, _NormalStrength1,
	_MetalnessMap1, sampler_MetalnessMap1, _MetalnessStrength1,
	smoothnessSample1,
	_EmissionMap1, sampler_EmissionMap1, _EmissionTint1,
	_ClearCoatMask1, sampler_ClearCoatMask1, _ClearCoatStrength1,
	uv1, input.normalWS, input.positionWS, input.tangentWS, input.positionCS, viewDirWS);
}

float4 GetColorsMaterial2(Interpolators input)
{
    float3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS); // In ShaderVariablesFunctions.hlsl
    float3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, viewDirWS); // In ParallaxMapping.hlsl, normal must NOT be normalized

	// Height Map Calculation For Second Material
    float2 uv2 = input.uv;
    uv2 += ParallaxMapping(TEXTURE2D_ARGS(_ParallaxMap2, sampler_ParallaxMap2), viewDirTS, _ParallaxStrength2, uv2);
	
	// Smoothness Map Calculation For Second Material
    float smoothnessSample2 = SampleSmoothness(_SmoothnessMask2, sampler_SmoothnessMask2, _Smoothness2, uv2);
#ifdef _ROUGHNESS_SETUP2
	smoothnessSample2 = 1 - smoothnessSample2;
#endif
	
    return RunUniversalPBR(_ColorMap2, sampler_ColorMap2, _ColorTint2,
	_NormalMap2, sampler_NormalMap2, _NormalStrength2,
	_MetalnessMap2, sampler_MetalnessMap2, _MetalnessStrength2,
	smoothnessSample2,
	_EmissionMap2, sampler_EmissionMap2, _EmissionTint2,
	_ClearCoatMask2, sampler_ClearCoatMask2, _ClearCoatStrength2,
	uv2, input.normalWS, input.positionWS, input.tangentWS, input.positionCS, viewDirWS);
}

// The fragment function. This runs once per fragment, which you can think of as a pixel on the screen
// It must output the final color of this pixel
float4 Fragment(Interpolators input
#ifdef _DOUBLE_SIDED_NORMALS
	, FRONT_FACE_TYPE frontFace : FRONT_FACE_SEMANTIC
#endif
) : SV_TARGET
{
	// Normal flipping
#ifdef _DOUBLE_SIDED_NORMALS
	input.normalWS *= IS_FRONT_VFACE(frontFace, 1, -1); // Multiply Normal vector by 1 or -1 depending if this face is facing the camera
#endif
	
    float4 rgba1 = GetColorsMaterial1(input);
	
	
    float4 rgba2 = GetColorsMaterial2(input);
	
    float noiseMask = 0.0f;
    float2 currentTileCoords = float2(_CurrentTileX, _CurrentTileY);
    noiseMask += SimplexNoise((input.uv + currentTileCoords) * _SimpleNoiseScale) * _SimpleNoiseStrength;
    noiseMask += ClassicNoise((input.uv + currentTileCoords) * _ClassicNoiseScale) * _ClassicNoiseStrength;
    noiseMask = clamp(noiseMask, 0, 1);
	
    float4 result = lerp(rgba1, rgba2, noiseMask);
	
    return result;
}

