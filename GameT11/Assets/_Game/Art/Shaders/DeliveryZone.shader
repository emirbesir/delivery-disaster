Shader "Emir/URP/DeliveryZone"
{
    Properties
    {
        [Header(Base Settings)]
        _BaseColor ("Base Color", Color) = (1.0, 0.85, 0.2, 1.0)
        _Smoothness ("Smoothness", Range(0,1)) = 0.1
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        [Header(Pulse Animation)]
        _PulseSpeed ("Pulse Speed", Range(0.1, 5.0)) = 1.5
        _PulseIntensity ("Pulse Intensity", Range(0.0, 1.0)) = 0.15
        _PulseColor ("Pulse Color", Color) = (1.0, 1.0, 0.8, 1.0)
        
        [Header(Rim Lighting)]
        _RimStrength ("Rim Strength", Range(0.0, 2.0)) = 0.3
        _RimPower ("Rim Power", Range(0.1, 10.0)) = 2.0
        _RimColor ("Rim Color", Color) = (1.0, 1.0, 0.8, 1.0)
        
        [Header(Edge Control)]
        _EdgeWidth ("Edge Width", Range(0.01, 0.5)) = 0.15
        _EdgeSoftness ("Edge Softness", Range(0.01, 0.3)) = 0.05
        
        [Header(Surface)]
        _SurfaceVariation ("Surface Variation", Range(0.0, 0.5)) = 0.1
        _AlphaBase ("Alpha Base", Range(0.0, 1.0)) = 0.85
        _AlphaVariation ("Alpha Variation", Range(0.0, 0.5)) = 0.1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 viewDirWS : TEXCOORD3;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Smoothness;
                float _Metallic;
                float _PulseSpeed;
                float _PulseIntensity;
                float4 _PulseColor;
                float _RimStrength;
                float _RimPower;
                float4 _RimColor;
                float _EdgeWidth;
                float _EdgeSoftness;
                float _SurfaceVariation;
                float _AlphaBase;
                float _AlphaVariation;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = input.uv;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                // Normalize vectors
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // Base color
                float3 baseColor = _BaseColor.rgb;
                
                // Gentle pulsing animation using Unity's time
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                float softPulse = smoothstep(0.3, 0.7, pulse);
                
                // Fresnel for rim/edge detection
                float fresnel = 1.0 - saturate(dot(normalWS, viewDirWS));
                float rim = pow(fresnel, _RimPower);
                
                // Edge detection using UV coordinates - creates outline effect
                float2 edgeUV = abs(input.uv - 0.5) * 2.0; // Remap UV to 0-1 from center
                float edgeDistance = max(edgeUV.x, edgeUV.y); // Distance to nearest edge
                
                // Adjustable edge width and softness
                float edgeStart = 1.0 - _EdgeWidth;
                float edgeEnd = edgeStart + _EdgeSoftness;
                float edgeFactor = smoothstep(edgeStart, edgeEnd, edgeDistance);
                
                // Combine rim and edge effects
                float edgeIntensity = max(rim * _RimStrength, edgeFactor);
                
                // Surface variation for subtle texture on edges only
                float surfaceVariation = sin(input.uv.x * PI * 4) * sin(input.uv.y * PI * 4) * _SurfaceVariation;
                surfaceVariation *= edgeIntensity; // Only apply to visible edges
                
                // Final color is mostly the base color with pulse
                float3 finalColor = baseColor + (_PulseColor.rgb * softPulse * _PulseIntensity);
                finalColor += _RimColor.rgb * rim * 0.5; // Subtle rim enhancement
                finalColor += baseColor * surfaceVariation;
                
                // Alpha is primarily based on edge visibility with pulse
                float baseAlpha = edgeIntensity * _AlphaBase;
                float pulseAlpha = softPulse * _AlphaVariation;
                float alpha = baseAlpha + pulseAlpha * edgeIntensity;
                
                // Ensure some minimum visibility during pulse
                alpha = max(alpha, edgeIntensity * 0.1);
                
                // Apply basic lighting only to visible parts
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 lighting = mainLight.color * (NdotL * 0.3 + 0.7); // Softer lighting
                
                finalColor *= lighting;
                
                return float4(finalColor, alpha);
            }
            
            ENDHLSL
        }
        
        // Shadow caster pass for proper shadow receiving
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}