Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", 2D) = "white" {}
        _Metallic ("Metallic", 2D) = "white" {}
        _Emission("Emission", 2D) = "white" {}
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

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _Glossiness;
        sampler2D _Metallic;
        sampler2D _Emission;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //float smoothness = .01f;
            float smoothness = 1 - tex2D(_Glossiness, IN.uv_MainTex).a;
            //float smooth = 1 - roughness;
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = tex2D(_Metallic, IN.uv_MainTex);
            o.Smoothness = smoothness;
            o.Emission = tex2D(_Emission, IN.uv_MainTex);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
