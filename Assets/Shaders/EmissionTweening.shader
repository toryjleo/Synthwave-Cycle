Shader "Custom/EmissionTweening"
{
    Properties
    {
        _EmissionColor1("_EmissionColor1", Color) = (1,1,1,1)
        _EmissionColor2("_EmissionColor2", Color) = (1,1,1,1)
        _EmissionSlider("EmissionSlider", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
        int placeholder; // "IN" needs to hold *something*
        };

        fixed4 _EmissionColor1;
        fixed4 _EmissionColor2;
        half _EmissionSlider;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = lerp(_EmissionColor1.rgba, _EmissionColor2.rgba, _EmissionSlider).rgba;;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Emission = lerp(_EmissionColor1.rgb, _EmissionColor2.rgb, _EmissionSlider).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
