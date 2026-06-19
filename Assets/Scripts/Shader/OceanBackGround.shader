Shader "Unlit/OceanBackGround"
{
     Properties
    {
        _BaseColor ("Base Color", Color) = (0.04, 0.12, 0.18, 1)
        _FogColor  ("Fog Color",  Color) = (0.0, 0.31, 0.35, 1)
        _FogSpeed  ("Fog Speed",  Float) = 0.4
        _FogScale  ("Fog Scale",  Float) = 1.8
        _FogStrength("Fog Strength", Float) = 0.18
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BaseColor, _FogColor;
            float  _FogSpeed, _FogScale, _FogStrength;

            struct a2v { float4 pos : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv  = v.uv;
                return o;
            }

            float hash(float2 p) {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float smoothNoise(float2 p) {
                float2 i = floor(p), f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(hash(i),           hash(i + float2(1,0)), f.x),
                    lerp(hash(i+float2(0,1)), hash(i + float2(1,1)), f.x),
                    f.y);
            }

            float fbm(float2 p) {
                float v = 0, a = 0.5;
                for (int i = 0; i < 3; i++) {
                    v += a * smoothNoise(p);
                    p *= 2.1; a *= 0.5;
                }
                return v;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t   = _Time.y * _FogSpeed;

                float n1 = fbm(uv * _FogScale + float2(t * 0.3, t * 0.17));
                float n2 = fbm(uv * _FogScale * 0.7 + float2(-t * 0.2, t * 0.25));
                float fog = (n1 + n2) * 0.5 * _FogStrength;

                float4 col = lerp(_BaseColor, _FogColor, fog);
                return col;
            }
            ENDHLSL
        }
    }
}
