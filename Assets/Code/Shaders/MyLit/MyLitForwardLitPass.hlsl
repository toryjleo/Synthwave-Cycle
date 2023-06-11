// Pull in custom common toolbox
#include "MyLitCommon.hlsl"
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
	output.uv = TRANSFORM_TEX(input.uv, _ColorMap); // Also applies offset variables (<name>_ST)
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
	uv += ParallaxMapping(TEXTURE2D_ARGS(_ParallaxMap, sampler_ParallaxMap), viewDirTS, _ParallaxStrength, uv);

	// Get Normal map
	float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv), _NormalStrength);
	float3x3 tangentToWorld = CreateTangentToWorld(normalWS, input.tangentWS.xyz, input.tangentWS.w);
	normalWS = normalize(TransformTangentToWorld(normalTS, tangentToWorld));
	//return float4((normalWS + 1) * 0.5, 1); // Returns normals TEST

	// Color Sample
	float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
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
	surfaceInput.albedo = colorSample.rgb * _ColorTint.rgb;
	surfaceInput.alpha = colorSample.a * _ColorTint.a;
	surfaceInput.specular = 1;
	surfaceInput.normalTS = normalTS;
#ifdef _SPECULAR_SETUP
	surfaceInput.specular = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, uv).rgb * _SpecularTint;
	surfaceInput.metallic = 0;
#else
	surfaceInput.metallic = SAMPLE_TEXTURE2D(_MetalnessMap, sampler_MetalnessMap, uv).r * _MetalnessStrength;
#endif
	float smoothnessSample = SAMPLE_TEXTURE2D(_SmoothnessMask, sampler_SmoothnessMask, uv).r * _Smoothness;
#ifdef _ROUGHNESS_SETUP
	smoothnessSample = 1 - smoothnessSample;
#endif
	surfaceInput.smoothness = smoothnessSample;
	surfaceInput.emission   = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb * _EmissionTint;
	surfaceInput.clearCoatMask = SAMPLE_TEXTURE2D(_ClearCoatMask, sampler_ClearCoatMask, uv).r * _ClearCoatStrength;
	surfaceInput.clearCoatSmoothness = SAMPLE_TEXTURE2D(_ClearCoatSmoothnessMask, sampler_ClearCoatSmoothnessMask, uv).r * _ClearCoatSmoothness;

	return UniversalFragmentPBR(lightingInput, surfaceInput);
}
