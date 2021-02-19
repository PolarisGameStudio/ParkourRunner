Shader "Adverty/AdShader" 
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        [HideInInspector]_WatermarkTex("_WatermarkTex (RGB)", 2D) = "white" {}
        [HideInInspector]_WatermarkIsVisible("Watermark Is Visible", Float) = 0
        [HideInInspector]_Metallic("Metallic", Range(0,1)) = 0.0
        [HideInInspector]_Color("Color", Color) = (1, 1, 1, 1)
        [HideInInspector]_StencilWriteMaskID("Stencil Write Mask ID", Float) = 0
        [HideInInspector]_PlayButtonTexture("Play Button Texture (RGB)", 2D) = "white" {}
        [HideInInspector]_PlayButtonScale("Play Button Scale", Float) = 0.025
        [HideInInspector]_PlayButtonOpacity("Play Button Opacity", Float) = 0.5
        [HideInInspector]_PlayButtonScaleSpeed("Play Button Scale Speed", Float) = 5
        [HideInInspector]_PlayButtonOpacitySpeed("Play Button Opacity Speed", Float) = 5
        [HideInInspector]_BackgroundRatioPlayButton("Play Button Base Ratio", Vector) = (1, 1, 0, 0)
        [HideInInspector]_PlayButtonIsVisible("Play Button is Visible", Float) = 0
        [HideInInspector]_WatermarkIsVisible("Watermark is Visible", Float) = 1
        [HideInInspector]_WatermarkUvSize("Watermark UV size", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque" "DisableBatching" = "True" }

        Stencil
        {
            Ref 255
            WriteMask[_StencilWriteMaskID]
            Pass Replace
        }

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        struct Input 
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        sampler2D _PlayButtonTexture;
        sampler2D _WatermarkTex;
        float2 _WatermarkUvSize;
        fixed _WatermarkIsVisible;
        fixed _PlayButtonIsVisible;
        float2 _BackgroundRatioPlayButton;
        half _Glossiness;
        half _Metallic;
        half _GrayFactor;
        fixed4 _Color;
        fixed _MirrorTexture;

        float _PlayButtonScale;
        float _PlayButtonOpacity;
        float _PlayButtonScaleSpeed;
        float _PlayButtonOpacitySpeed;

        fixed4 addPlayButtonToTexture(float2 uv, fixed4 main)
        {
            float scale = 1 + sin(_Time.y * _PlayButtonScaleSpeed) * _PlayButtonScale;
            float opacity = 1 - sin(_Time.y * _PlayButtonOpacitySpeed) * _PlayButtonOpacity;

            _BackgroundRatioPlayButton *= scale;

            float2 overlayOffset = (_BackgroundRatioPlayButton * sign(uv) - 1) * 0.5f; //uvs where overlay starts
            float2 overlayUV = saturate(_BackgroundRatioPlayButton * uv - overlayOffset) * scale;

            fixed4 button = tex2D(_PlayButtonTexture, overlayUV);
            button.a *= _PlayButtonIsVisible * opacity;

            half3 mainTexVisible = main.rgb * (1 - button.a);
            half3 buttonTexVisible = button.rgb * (button.a);

            float3 finalColor = saturate(mainTexVisible + buttonTexVisible);

            return fixed4(finalColor.rgb, 0.5 * (main.a + button.a));
        }

        fixed4 addWatermark(float2 uv, fixed4 main)
        {
            float2 watermarkRepeatFactor = float2(uv.x < 0, uv.y < 0);
            fixed2 watermarkUv;
            watermarkUv = watermarkRepeatFactor / _WatermarkUvSize + uv / _WatermarkUvSize;

            watermarkRepeatFactor += uv;

            float watermarkFactor = all(step(watermarkRepeatFactor, _WatermarkUvSize));
            fixed4 watermark = lerp(main, tex2D(_WatermarkTex, watermarkUv), watermarkFactor);

            return lerp(watermark, main, 1 - watermark.a);
        }

        void surf(Input IN, inout SurfaceOutputStandard o) 
        {
            float2 mainUv = IN.uv_MainTex;

            #if SHADER_API_METAL || SHADER_API_VULKAN
                mainUv.y = lerp(mainUv.y, 1.0f - mainUv.y, _MirrorTexture);
            #endif

            fixed4 main = tex2D(_MainTex, mainUv);

            fixed4 col = lerp(main, addWatermark(IN.uv_MainTex, main), _WatermarkIsVisible);
            col = addPlayButtonToTexture(IN.uv_MainTex, col) * _Color;
            col = lerp(col, float4(0.5, 0.5, 0.5, 1), _GrayFactor); 

            o.Albedo = col.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
