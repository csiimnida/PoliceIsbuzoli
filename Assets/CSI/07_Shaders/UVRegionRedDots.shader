Shader "CSI/UVRegionRedDots"
{
    Properties
    {
        _MainTex ("Base Color Mask (RGB regions)", 2D) = "white" {}
        _RegionColor ("Target Region Color", Color) = (1,0,0,1)
        _Tolerance ("Region Color Tolerance", Range(0,1)) = 0.1
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _DotColor ("Dot Color", Color) = (1,0,0,1)
        _Density ("Dot Density", Range(0,1)) = 0.0
        _DotSize ("Dot Size", Range(0.001,0.2)) = 0.05
        _Tiling ("Dot Tiling", Vector) = (10,10,0,0)
        _Seed ("Noise Seed", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        Cull Off ZWrite On ZTest LEqual
        Pass
        {
            Name "Unlit"
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _RegionColor;
            float _Tolerance;
            float4 _BackgroundColor;
            float4 _DotColor;
            float _Density;
            float _DotSize;
            float4 _Tiling; // xy tiling
            float _Seed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Hash to pseudo-random in [0,1)
            float2 hash22(float2 p)
            {
                // from IQ
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * (p3.zy + p3.xz));
            }

            float rand(float2 n)
            {
                return frac(sin(dot(n, float2(12.9898,78.233))) * 43758.5453);
            }

            // Returns 1 inside a dot, 0 outside
            float dotPattern(float2 uv, float2 tiling, float dotSize, float density, float seed)
            {
                // tile coordinates
                float2 gridUV = uv * tiling + seed;
                float2 cell = floor(gridUV);
                float2 fuv = frac(gridUV);

                // per-cell random threshold for density
                float th = rand(cell + seed);
                if (th > density) return 0; // cell inactive if above density

                // random center inside the cell for organic feel
                float2 center = hash22(cell + seed);
                float dist = distance(fuv, center);
                float inside = smoothstep(dotSize, dotSize * 0.8, -dist + dotSize);
                return saturate(inside);
            }

            float regionMask(float3 rgb, float3 target, float tol)
            {
                float d = distance(rgb, target);
                return step(d, tol);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 maskCol = tex2D(_MainTex, i.uv);
                float m = regionMask(maskCol.rgb, _RegionColor.rgb, _Tolerance);

                // Dots only inside masked region
                float dots = dotPattern(i.uv, _Tiling.xy, _DotSize, saturate(_Density), _Seed);
                float3 col = lerp(_BackgroundColor.rgb, _BackgroundColor.rgb, 1.0); // base
                col = lerp(col, _DotColor.rgb, dots * m);

                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
