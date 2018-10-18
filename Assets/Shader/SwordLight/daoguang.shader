// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:4,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9063,x:33788,y:32795,varname:node_9063,prsc:2|normal-7204-OUT,emission-3529-OUT,alpha-8707-OUT;n:type:ShaderForge.SFN_Tex2d,id:850,x:31770,y:32709,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_850,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6d4a094f58e0dd74185c4c63020a8276,ntxv:0,isnm:False|UVIN-3293-OUT;n:type:ShaderForge.SFN_If,id:459,x:31666,y:32938,varname:node_459,prsc:2|A-1342-OUT,B-5856-R,GT-1623-OUT,EQ-1623-OUT,LT-7299-OUT;n:type:ShaderForge.SFN_Tex2d,id:5856,x:31192,y:32969,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_5856,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7d26bf5d9949fc148aaacb4f2fae7819,ntxv:0,isnm:False|UVIN-5121-OUT;n:type:ShaderForge.SFN_Vector1,id:1623,x:31192,y:33144,varname:node_1623,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7299,x:31192,y:33207,varname:node_7299,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:8707,x:32854,y:32945,varname:node_8707,prsc:2|A-850-A,B-8858-A,C-459-OUT;n:type:ShaderForge.SFN_VertexColor,id:8858,x:31159,y:32800,varname:node_8858,prsc:2;n:type:ShaderForge.SFN_If,id:2124,x:31666,y:33063,varname:node_2124,prsc:2|A-8858-R,B-5856-R,GT-1623-OUT,EQ-1623-OUT,LT-7299-OUT;n:type:ShaderForge.SFN_Add,id:1342,x:31377,y:32757,varname:node_1342,prsc:2|A-2095-OUT,B-8858-R;n:type:ShaderForge.SFN_ValueProperty,id:2095,x:31136,y:32635,ptovrint:False,ptlb:0_勾边大小,ptin:_0_,varname:node_2095,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.09;n:type:ShaderForge.SFN_Subtract,id:3805,x:31941,y:32981,varname:node_3805,prsc:2|A-459-OUT,B-2124-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7276,x:31804,y:33222,ptovrint:False,ptlb:1_勾边亮度,ptin:_1_,varname:node_7276,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Color,id:5389,x:31968,y:32552,ptovrint:False,ptlb:3_color,ptin:_3_color,varname:node_5389,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3584906,c2:0.1432594,c3:0.05918476,c4:1;n:type:ShaderForge.SFN_Lerp,id:7204,x:33070,y:33356,varname:node_7204,prsc:2|A-3224-OUT,B-850-A,T-2822-OUT;n:type:ShaderForge.SFN_Vector3,id:3224,x:32733,y:33201,varname:node_3224,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:2822,x:32511,y:33643,ptovrint:False,ptlb:4_扭曲强度,ptin:_4_,varname:node_2822,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3422265,max:1;n:type:ShaderForge.SFN_Multiply,id:1502,x:33146,y:33529,varname:node_1502,prsc:2|A-70-OUT,B-2822-OUT,C-7199-OUT;n:type:ShaderForge.SFN_Vector1,id:7199,x:32668,y:33756,varname:node_7199,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:70,x:32736,y:33447,varname:node_70,prsc:2|A-6643-OUT,B-8858-A;n:type:ShaderForge.SFN_Add,id:3529,x:32947,y:32744,varname:node_3529,prsc:2|A-969-OUT,B-68-OUT;n:type:ShaderForge.SFN_Append,id:6643,x:32497,y:33338,varname:node_6643,prsc:2|A-850-R,B-850-G;n:type:ShaderForge.SFN_Multiply,id:969,x:32627,y:32627,varname:node_969,prsc:2|A-5389-RGB,B-4714-OUT;n:type:ShaderForge.SFN_Subtract,id:4714,x:32198,y:32721,varname:node_4714,prsc:2|A-850-RGB,B-933-OUT;n:type:ShaderForge.SFN_Multiply,id:68,x:32502,y:32777,varname:node_68,prsc:2|A-5389-RGB,B-4512-OUT;n:type:ShaderForge.SFN_Multiply,id:4512,x:32167,y:33045,varname:node_4512,prsc:2|A-3805-OUT,B-7276-OUT;n:type:ShaderForge.SFN_Slider,id:1353,x:30035,y:32423,ptovrint:False,ptlb:Size,ptin:_Size,varname:node_1353,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4501191,max:1;n:type:ShaderForge.SFN_RemapRange,id:1945,x:30392,y:32430,varname:node_1945,prsc:2,frmn:0,frmx:1,tomn:0.65,tomx:4|IN-1353-OUT;n:type:ShaderForge.SFN_TexCoord,id:9764,x:30349,y:32094,varname:node_9764,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:2673,x:30349,y:32308,varname:node_2673,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:551,x:30627,y:32417,varname:node_551,prsc:2|A-2673-OUT,B-1945-OUT;n:type:ShaderForge.SFN_Clamp01,id:3702,x:31114,y:32204,varname:node_3702,prsc:2|IN-6551-OUT;n:type:ShaderForge.SFN_Add,id:6551,x:30916,y:32204,varname:node_6551,prsc:2|A-29-OUT,B-7376-OUT;n:type:ShaderForge.SFN_Multiply,id:29,x:30627,y:32210,varname:node_29,prsc:2|A-9764-UVOUT,B-1945-OUT;n:type:ShaderForge.SFN_Subtract,id:7376,x:30824,y:32353,varname:node_7376,prsc:2|A-2673-OUT,B-551-OUT;n:type:ShaderForge.SFN_ComponentMask,id:647,x:31440,y:32310,varname:node_647,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-5121-OUT;n:type:ShaderForge.SFN_Append,id:3293,x:31676,y:32431,varname:node_3293,prsc:2|A-647-OUT,B-9764-V;n:type:ShaderForge.SFN_SwitchProperty,id:5121,x:31124,y:32428,ptovrint:False,ptlb:UV_switch,ptin:_UV_switch,varname:node_5121,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-9764-UVOUT,B-3702-OUT;n:type:ShaderForge.SFN_Multiply,id:933,x:32020,y:32754,varname:node_933,prsc:2|A-850-RGB,B-3805-OUT;proporder:850-5856-5121-1353-5389-2095-7276-2822;pass:END;sub:END;*/

Shader "Shader Forge/daoguang_rongjie" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        [MaterialToggle] _UV_switch ("UV_switch", Float ) = 0
        _Size ("Size", Range(0, 1)) = 0.4501191
        [HDR]_3_color ("3_color", Color) = (0.3584906,0.1432594,0.05918476,1)
        _0_ ("0_勾边大小", Float ) = 0.09
        _1_ ("1_勾边亮度", Float ) = 5
        _4_ ("4_扭曲强度", Range(0, 1)) = 0.3422265
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu switch 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _0_;
            uniform float _1_;
            uniform float4 _3_color;
            uniform float _4_;
            uniform float _Size;
            uniform fixed _UV_switch;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float node_1945 = (_Size*3.35+0.65);
                float node_2673 = 0.5;
                float2 _UV_switch_var = lerp( i.uv0, saturate(((i.uv0*node_1945)+(node_2673-(node_2673*node_1945)))), _UV_switch );
                float2 node_3293 = float2(_UV_switch_var.r,i.uv0.g);
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_3293, _Diffuse));
                float3 normalLocal = lerp(float3(0,0,1),float3(_Diffuse_var.a,_Diffuse_var.a,_Diffuse_var.a),_4_);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
////// Lighting:
////// Emissive:
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(_UV_switch_var, _Noise));
                float node_459_if_leA = step((_0_+i.vertexColor.r),_Noise_var.r);
                float node_459_if_leB = step(_Noise_var.r,(_0_+i.vertexColor.r));
                float node_7299 = 0.0;
                float node_1623 = 1.0;
                float node_459 = lerp((node_459_if_leA*node_7299)+(node_459_if_leB*node_1623),node_1623,node_459_if_leA*node_459_if_leB);
                float node_2124_if_leA = step(i.vertexColor.r,_Noise_var.r);
                float node_2124_if_leB = step(_Noise_var.r,i.vertexColor.r);
                float node_3805 = (node_459-lerp((node_2124_if_leA*node_7299)+(node_2124_if_leB*node_1623),node_1623,node_2124_if_leA*node_2124_if_leB));
                float3 emissive = ((_3_color.rgb*(_Diffuse_var.rgb-(_Diffuse_var.rgb*node_3805)))+(_3_color.rgb*(node_3805*_1_)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_Diffuse_var.a*i.vertexColor.a*node_459));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
