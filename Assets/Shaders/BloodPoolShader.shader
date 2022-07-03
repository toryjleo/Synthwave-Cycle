Shader "Custom/BloodPoolShader"
{
    Properties
    {
        _AlphaMask("AlphaMask", 2D) = "white" {}
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AlphaMaskRotation("Alpha Mask Rotation", Float) = 0.0
        _MainTexRotation("Main Texture Rotation", Float) = 0.0
        _AlphaMaskScale("Alpha Mask Scale", Float) = 1.0
        _MainTexScale("Main Texture Scale", Float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

        

        struct Input
        {
            float2 uv_AlphaMask;
            float2 uv_MainTex;
        };

        float _AlphaMaskRotation;
        float _MainTexRotation;

        float _AlphaMaskScale;
        float _MainTexScale;


        sampler2D _AlphaMask;
        sampler2D _MainTex;

        /// <summary>
        /// Function to rotate UV's.
        /// </summary>
        /// <param name="texcoord">UV going in.</param>
        /// <param name="rotation">Amount to rotate UV.</param>
        float2 rotateUV(float2 texcoord, float rotation, float scale = 1)
        {
            texcoord.xy -= 0.5;
            float s = sin(rotation);
            float c = cos(rotation);
            float2x2 rotationMatrix = float2x2(c, -s, s, c);
            rotationMatrix *= 0.5;
            rotationMatrix += 0.5;
            rotationMatrix = rotationMatrix * 2 - 1;
            rotationMatrix = rotationMatrix / scale;
            texcoord.xy = mul(texcoord.xy, rotationMatrix);
            texcoord.xy += 0.5;
            return texcoord.xy;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 alphauv = rotateUV(IN.uv_AlphaMask, _AlphaMaskRotation, _AlphaMaskScale);
            fixed4 alpha = tex2D(_AlphaMask, alphauv);

            // Calculate where to sample the alphamask
            float2 mtuv = rotateUV(IN.uv_MainTex, _MainTexRotation, _MainTexScale);
            fixed4 c = tex2D(_MainTex, mtuv);

            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = alpha.a;

            // Clip anything that is outside of the alpha or the albedo textures.
            clip(alpha.a - .01);
            clip(c.a - .01);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
