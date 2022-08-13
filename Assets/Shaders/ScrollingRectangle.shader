Shader "Unlit/ScrollingRectangle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float Unity_Rectangle_float(float2 UV, float Width, float Height)
            {
                float2 d = abs(UV * 2 - 1) - float2(Width, Height);
                d = 1 - d / fwidth(d);
                return saturate(min(d.x, d.y));
            }

            float Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset)
            {
                return UV * Tiling + Offset;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                int numDots = 33;
                half2 offset = half2(_Time.y, 0);
                half2 tiling = half2(numDots, 1);
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                half2 updatedCoords = (i.uv * tiling) + offset;
                updatedCoords = frac(updatedCoords);
                float rect = Unity_Rectangle_float(updatedCoords, .5f, .5f);
                //return half4(updatedCoords.x, updatedCoords.y, 0, 1);
                clip(rect - .2f);
                return half4(rect, rect, rect, rect);
            }
            ENDCG
        }
    }
}
