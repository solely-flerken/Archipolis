Shader "Unlit/PlacementValidityShader"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {} 
        _OverlayColor ("Overlay Color", Color) = (0, 0, 0, 0) // Default transparent overlay color
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Overlay"
        } 

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap); // Use base texture (no changes to this)
            SAMPLER(sampler_BaseMap);
            float4 _OverlayColor; // Color overlay

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                // Sample the base texture as usual
                // Apply overlay color on top, but don't affect alpha blending of base texture
                return baseColor * (1 - _OverlayColor.a) + _OverlayColor * _OverlayColor.a;
                // Add the overlay only to the color, not alpha
            }
            ENDHLSL
        }
    }
}