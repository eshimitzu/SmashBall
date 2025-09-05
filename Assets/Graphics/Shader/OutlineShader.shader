Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.2)) = 0.03
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        // Pass 0: Outline — render front-culled, expanded mesh as solid color
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            Cull Front
            ZWrite On
            ZTest LEqual
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;

            v2f vertOutline(appdata v)
            {
                v2f o;
                float3 n = normalize(v.normal);
                float4 pos = v.vertex;
                pos.xyz += n * _OutlineWidth;
                o.pos = UnityObjectToClipPos(pos);
                return o;
            }

            fixed4 fragOutline(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // Pass 1: Base color with texture, no lighting (unlit)
        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }

            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                return tex * _Color;
            }
            ENDCG
        } 
    }

    FallBack Off
}
