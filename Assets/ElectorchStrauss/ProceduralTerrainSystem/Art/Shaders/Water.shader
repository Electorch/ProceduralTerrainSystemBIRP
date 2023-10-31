// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Water" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
    _MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
    _xScale ("ScaleX",float) = 1
    _zScale ("ScaleZ",float) = 3
    _phase ("Phase",float) = 1
    _ScrollXSpeed("X", Range(-10,10)) = 1
    _ScrollYSpeed("Y", Range(-10,10)) = 1
}
 
SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    Cull Off
    LOD 300
   
 
CGPROGRAM
#pragma surface surf Lambert alpha vertex:vert
 
sampler2D _MainTex;
sampler2D _BumpMap;
samplerCUBE _Cube;

float _zScale;
float _xScale;
float _phase;
fixed4 _Color;
fixed4 _ReflectColor;
fixed _ScrollXSpeed;
fixed _ScrollYSpeed;

struct Input {
    float2 uv_MainTex;
    float2 uv_BumpMap;
    INTERNAL_DATA
};
 
void vert (inout appdata_full v) {
    float phase = _Time * 20.0;
    float3 baseWorldPos = unity_ObjectToWorld._m03_m13_m23;
    float4 wpos = mul( unity_ObjectToWorld, v.vertex);
    float offset = (wpos.x + (wpos.z * _zScale)) * _xScale;
    wpos.y = sin(phase + offset) * _phase + baseWorldPos.y;
    v.vertex = mul(unity_WorldToObject, wpos);
}
 
void surf (Input IN, inout SurfaceOutput o) {
    fixed2 scrolledUV = IN.uv_MainTex;
    fixed xScrollValue = _ScrollXSpeed * _Time;
    fixed yScrollValue = _ScrollYSpeed * _Time;
    scrolledUV += fixed2(xScrollValue, yScrollValue);
    fixed4 tex = tex2D(_MainTex, scrolledUV);
    fixed4 c = tex * _Color;
    o.Albedo = c.rgb;
    o.Normal = UnpackNormal(tex2D(_BumpMap, scrolledUV));
    o.Emission = _ReflectColor.rgb;
    o.Alpha = 0.9;
}
ENDCG
}
 
FallBack "Reflective/VertexLit"
}