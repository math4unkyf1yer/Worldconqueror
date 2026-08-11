Shader "Unlit/ParchmentBackGround"
{
    Properties
    {
        _BaseColor    ("Base Color", Color) = (0.16, 0.12, 0.06, 1)
        _WarmColor    ("Warm Tint", Color) = (0.45, 0.30, 0.10, 1)
        _HatchColor   ("Hatch Color", Color) = (0.55, 0.38, 0.14, 1)
        _HatchDensity ("Hatch Density", Float) = 60.0
        _HatchAngle   ("Hatch Angle", Float) = 0.45
        _HatchThick   ("Hatch Thickness", Float) = 0.12
        _HatchOpacity ("Hatch Opacity", Float) = 0.09
        _WarmRadius   ("Warm Glow Radius", Float) = 0.55
        _WarmStrength ("Warm Glow Strength", Float) = 0.22
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BaseColor, _WarmColor, _HatchColor;
            float _HatchDensity, _HatchAngle, _HatchThick;
            float _HatchOpacity, _WarmRadius, _WarmStrength;

            struct a2v {
                float4 pos : POSITION;
                float2 uv  : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv  = v.uv;
                return o;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float smoothNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                float x1 = lerp(a, b, f.x);
                float x2 = lerp(c, d, f.x);

                return lerp(x1, x2, f.y);
            }

            float hatch(float2 uv, float angle, float density, float thickness)
            {
                float s = sin(angle);
                float c = cos(angle);

                float2 ruv = float2(c * uv.x - s * uv.y,
                        s * uv.x + c * uv.y);

                float lines = frac(ruv.y * density);

                 float t0 = (float)0.0;
                  float t1 = (float)thickness;
                float t2 = (float)(1.0 - thickness);
                 float t3 = (float)1.0;

                   float a = smoothstep(t0, t1, lines);
                float b = smoothstep(t2, t3, lines);

                  return a * b;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float warp = smoothNoise(uv * 4.0 + float2(0.3, 0.7)) * 0.012;
                float2 wuv = uv + warp;

                float h1 = hatch(wuv, _HatchAngle, _HatchDensity, _HatchThick);
                float h2 = hatch(wuv, _HatchAngle + 1.5708, _HatchDensity * 1.4, _HatchThick * 0.6);

                float hatchMask = saturate(h1 * _HatchOpacity + h2 * _HatchOpacity * 0.5);

                float2 glowOrigin = float2(0.25, 0.75);
                float glowDist = length(uv - glowOrigin) / _WarmRadius;
                float glow = saturate(1.0 - glowDist * glowDist) * _WarmStrength;

                float4 col = lerp(_BaseColor, _WarmColor, glow);
                col = lerp(col, _HatchColor, hatchMask);

                return col;
            }

            ENDHLSL
        }
    }
}
