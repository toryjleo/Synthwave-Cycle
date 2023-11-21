// Pull in custom common toolbox
#include "MyMixCommon.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"


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

// The fragment function. This runs once per fragment, which you can think of as a pixel on the screen
// It must output the final color of this pixel
float4 Fragment(Interpolators input
#ifdef _DOUBLE_SIDED_NORMALS
	, FRONT_FACE_TYPE frontFace : FRONT_FACE_SEMANTIC
#endif
) : SV_TARGET
{
	// Normal
	float3 normalWS = input.normalWS;
#ifdef _DOUBLE_SIDED_NORMALS
	normalWS *= IS_FRONT_VFACE(frontFace, 1, -1); // Multiply Normal vector by 1 or -1 depending if this face is facing the camera
#endif

	// View Direction
	float3 positionWS = input.positionWS;
	float3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS); // In ShaderVariablesFunctions.hlsl
	float3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, normalWS, viewDirWS); // In ParallaxMapping.hlsl, normal must NOT be normalized

	// Height Map Calculation
	float2 uv = input.uv;
#ifdef _ParMap1
	uv += ParallaxMapping(TEXTURE2D_ARGS(_ParallaxMap1, sampler_ParallaxMap1), viewDirTS, _ParallaxStrength1, uv);
#endif


	// Get Normal map
	float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap1, sampler_NormalMap1, uv), _NormalStrength1);
	float3x3 tangentToWorld = CreateTangentToWorld(normalWS, input.tangentWS.xyz, input.tangentWS.w);
	normalWS = normalize(TransformTangentToWorld(normalTS, tangentToWorld));
	//return float4((normalWS + 1) * 0.5, 1); // Returns normals TEST

	// Color Sample
	float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap1, sampler_ColorMap1, uv);
	TestAlphaClip(colorSample);

	// Update lighting input data for lighting calculations
	InputData lightingInput = (InputData)0;
	lightingInput.positionWS = positionWS;
	lightingInput.normalWS = normalWS;
	lightingInput.viewDirectionWS = viewDirWS;
	lightingInput.shadowCoord = TransformWorldToShadowCoord(positionWS); // Sample the shadow coord from the shadow map
#if UNITY_VERSION >= 202120
	lightingInput.positionCS = input.positionCS;
	lightingInput.tangentToWorld = tangentToWorld;
#endif

	SurfaceData surfaceInput = (SurfaceData)0;
	surfaceInput.albedo = colorSample.rgb * _ColorTint1.rgb;
	surfaceInput.alpha = colorSample.a * _ColorTint1.a;
	surfaceInput.specular = 1;
	surfaceInput.normalTS = normalTS;
#ifdef _SPECULAR_SETUP
	surfaceInput.specular = SAMPLE_TEXTURE2D(_SpecularMap1, sampler_SpecularMap1, uv).rgb * _SpecularTint1;
	surfaceInput.metallic = 0;
#else
	surfaceInput.metallic = SAMPLE_TEXTURE2D(_MetalnessMap1, sampler_MetalnessMap1, uv).r * _MetalnessStrength1;
#endif
	float smoothnessSample = SAMPLE_TEXTURE2D(_SmoothnessMask1, sampler_SmoothnessMask1, uv).r * _Smoothness1;
#ifdef _ROUGHNESS_SETUP
	smoothnessSample = 1 - smoothnessSample;
#endif
	surfaceInput.smoothness = smoothnessSample;
#ifdef _EmissionMap
	surfaceInput.emission   = SAMPLE_TEXTURE2D(_EmissionMap1, sampler_EmissionMap1, uv).rgb * _EmissionTint1;
#endif
#ifdef _CCMask
	surfaceInput.clearCoatMask = SAMPLE_TEXTURE2D(_ClearCoatMask1, sampler_ClearCoatMask1, uv).r * _ClearCoatStrength1;
#endif
#ifdef _CCSMask
	surfaceInput.clearCoatSmoothness = SAMPLE_TEXTURE2D(_ClearCoatSmoothnessMask1, sampler_ClearCoatSmoothnessMask1, uv).r * _ClearCoatSmoothness;
#endif

	return UniversalFragmentPBR(lightingInput, surfaceInput);
}

float4 RunUniversalPBR(Texture2D colorMap, SamplerState colorMapSampler,  float4 colorTint,
                       Texture2D normalMap, SamplerState normalMapSampler, float normalStrength,
					   Texture2D metalnessMap, SamplerState metalnessMapSampler, float metalnessStrength,
				       Texture2D smoothnessMap, SamplerState smoothnessMapSampler, float smoothnessStrength,
                       Texture2D parallaxMap, SamplerState parallaxMapSampler, float parallaxStrength,
					   Texture2D specularMap, SamplerState specularMapSampler, float3 specularTint,
					   Texture2D emissionMap, SamplerState emissionMapSampler, float4 emissionTint,
					   Texture2D clearCoatMask, SamplerState clearCoatSampler, float clearCoatStrength,
					   Texture2D clearCoatSmoothnessMask, SamplerState clearCoatSmoothnessSampler, float clearCoatSmoothnessStrength,
                       float2 uv, float3 normalWS, float3 positionWS, float4 tangentWS, float4 positionCS
#ifdef _DOUBLE_SIDED_NORMALS
	, FRONT_FACE_TYPE frontFace : FRONT_FACE_SEMANTIC
#endif
)
{
	// Normal
#ifdef _DOUBLE_SIDED_NORMALS
	normalWS *= IS_FRONT_VFACE(frontFace, 1, -1); // Multiply Normal vector by 1 or -1 depending if this face is facing the camera
#endif
	
	// View Direction
    float3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS); // In ShaderVariablesFunctions.hlsl
    float3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, normalWS, viewDirWS); // In ParallaxMapping.hlsl, normal must NOT be normalized

	// Height Map Calculation
#ifdef _ParMap1
	uv += ParallaxMapping(TEXTURE2D_ARGS(parallaxMap, parallaxMapSampler), viewDirTS, parallaxStrength, uv);
#endif
	
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
	
#ifdef _SPECULAR_SETUP
	surfaceInput.specular = SAMPLE_TEXTURE2D(specularMap, specularMapSampler, uv).rgb * specularTint;
	surfaceInput.metallic = 0;
#else
    surfaceInput.metallic = SAMPLE_TEXTURE2D(metalnessMap, metalnessMapSampler, uv).r * metalnessStrength;
#endif
    float smoothnessSample = SAMPLE_TEXTURE2D(smoothnessMap, smoothnessMapSampler, uv).r * smoothnessStrength;
#ifdef _ROUGHNESS_SETUP
	smoothnessSample = 1 - smoothnessSample;
#endif
    surfaceInput.smoothness = smoothnessSample;
#ifdef _EmissionMap
	surfaceInput.emission   = SAMPLE_TEXTURE2D(emissionMap, emissionMapSampler, uv).rgb * emissionTint;
#endif
#ifdef _CCMask
	surfaceInput.clearCoatMask = SAMPLE_TEXTURE2D(clearCoatMask, clearCoatSampler, uv).r * clearCoatStrength;
#endif
#ifdef _CCSMask
	surfaceInput.clearCoatSmoothness = SAMPLE_TEXTURE2D(clearCoatSmoothnessMask, clearCoatSmoothnessSampler, uv).r * clearCoatSmoothnessStrength;
#endif
    return UniversalFragmentPBR(lightingInput, surfaceInput);
}
