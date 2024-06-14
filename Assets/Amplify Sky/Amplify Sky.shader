// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Skybox/Amplify Sky"
{
	Properties
	{
		_SkyDay("SkyDay", Color) = (0.5660378,0.4672482,0.4672482,0)
		_SkyNight("SkyNight", Color) = (0.5660378,0.4672482,0.4672482,0)
		_SunRadiusA("Sun Radius A", Range( 0 , 0.1)) = 0
		_SunRadiusB("Sun Radius B", Range( 0 , 0.1)) = 0
		_SunPower("SunPower", Range( 1 , 100)) = 0
		_Horizon("Horizon", Range( 1 , 10)) = 6.2
		_StarsTexture("StarsTexture", 2D) = "white" {}
		_FogFade("FogFade", Range( 0.2 , 5)) = 1
		_StarsTiling("StarsTiling", Range( 1 , 8)) = 0
		_StarsRotationSpeed("StarsRotationSpeed", Range( 0 , 0.1)) = 0
		_SunHalo("SunHalo", Range( 1 , 100)) = 4.97
		_SunHaloEdgeBlend("SunHaloEdgeBlend", Range( 1 , 10)) = 1000
		_DuskAmount("DuskAmount", Range( 0.2 , 0.8)) = 0.5
		_CloudNoise("CloudNoise", 2D) = "white" {}
		_CloudHeight("CloudHeight", Range( 0.2 , 2)) = 0
		_Windspeed("Windspeed", Range( -1 , 1)) = 0.1
		_TextureSample2("Texture Sample 2", 2D) = "bump" {}
		_StarsTile("StarsTile", Range( 0.5 , 1)) = 0
		_CloudScatter("CloudScatter", Range( 0.5 , 2)) = 0
		_CloudWhiteness("CloudWhiteness", Range( 1 , 10)) = 1

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest Always
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			//This is a late directive
			
			uniform float4 _SkyDay;
			uniform float4 _SkyNight;
			uniform float _FogFade;
			uniform sampler2D _StarsTexture;
			uniform float _StarsTile;
			uniform float _StarsRotationSpeed;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform sampler2D _CloudNoise;
			uniform float _Windspeed;
			uniform float _CloudHeight;
			uniform float _CloudScatter;
			uniform float _StarsTiling;
			uniform float _DuskAmount;
			uniform float _SunRadiusA;
			uniform float _SunRadiusB;
			uniform float _SunPower;
			uniform float _SunHalo;
			uniform float _SunHaloEdgeBlend;
			uniform float _Horizon;
			uniform sampler2D _TextureSample2;
			uniform float _CloudWhiteness;
			inline float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max( 0.001f , dot( inVec , inVec ) );
				return inVec* rsqrt( dp3);
			}
			
			struct Gradient
			{
				int type;
				int colorsLength;
				int alphasLength;
				float4 colors[8];
				float2 alphas[8];
			};
			
			Gradient NewGradient(int type, int colorsLength, int alphasLength, 
			float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
			float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
			{
				Gradient g;
				g.type = type;
				g.colorsLength = colorsLength;
				g.alphasLength = alphasLength;
				g.colors[ 0 ] = colors0;
				g.colors[ 1 ] = colors1;
				g.colors[ 2 ] = colors2;
				g.colors[ 3 ] = colors3;
				g.colors[ 4 ] = colors4;
				g.colors[ 5 ] = colors5;
				g.colors[ 6 ] = colors6;
				g.colors[ 7 ] = colors7;
				g.alphas[ 0 ] = alphas0;
				g.alphas[ 1 ] = alphas1;
				g.alphas[ 2 ] = alphas2;
				g.alphas[ 3 ] = alphas3;
				g.alphas[ 4 ] = alphas4;
				g.alphas[ 5 ] = alphas5;
				g.alphas[ 6 ] = alphas6;
				g.alphas[ 7 ] = alphas7;
				return g;
			}
			
			float2 UnStereo( float2 UV )
			{
				#if UNITY_SINGLE_PASS_STEREO
				float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
				UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
				#endif
				return UV;
			}
			
			float3 InvertDepthDir72_g2( float3 In )
			{
				float3 result = In;
				#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
				result *= float3(1,1,-1);
				#endif
				return result;
			}
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord1 = v.vertex;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float3 worldSpaceLightDir = Unity_SafeNormalize(UnityWorldSpaceLightDir(WorldPosition));
				float dotResult1243 = dot( worldSpaceLightDir , float3(0,-1,0) );
				float DayNight1248 = dotResult1243;
				float4 lerpResult1254 = lerp( _SkyDay , _SkyNight , DayNight1248);
				float4 SkyColor1257 = lerpResult1254;
				float dotResult1476 = dot( i.ase_texcoord1.xyz , float3(0,1.36,0) );
				float temp_output_1705_0 = saturate( dotResult1476 );
				float3 desaturateInitialColor1698 = SkyColor1257.rgb;
				float desaturateDot1698 = dot( desaturateInitialColor1698, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar1698 = lerp( desaturateInitialColor1698, desaturateDot1698.xxx, ( ( 1.0 - saturate( _FogFade ) ) - temp_output_1705_0 ) );
				float3 normalizeResult1325 = ASESafeNormalize( WorldPosition );
				float2 temp_output_1323_0 = (normalizeResult1325).xz;
				float temp_output_1327_0 = saturate( (normalizeResult1325).y );
				float mulTime1341 = _Time.y * _StarsRotationSpeed;
				float cos1378 = cos( mulTime1341 );
				float sin1378 = sin( mulTime1341 );
				float2 rotator1378 = mul( ( ( ( temp_output_1323_0 * ( 1.0 - temp_output_1327_0 ) ) + temp_output_1323_0 ) * _StarsTile ) - float2( 0.1,0.9 ) , float2x2( cos1378 , -sin1378 , sin1378 , cos1378 )) + float2( 0.1,0.9 );
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float clampDepth1684 = Linear01Depth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float SolidMask1310 = step( 1.0 , clampDepth1684 );
				float temp_output_1337_0 = pow( temp_output_1327_0 , 1.2 );
				float mulTime1473 = _Time.y * _Windspeed;
				float3 normalizeResult1450 = ASESafeNormalize( WorldPosition );
				float2 temp_output_1458_0 = ( ( (normalizeResult1450).xz * _CloudHeight ) / abs( (normalizeResult1450).y ) );
				float2 panner1472 = ( mulTime1473 * float2( 0.1,0.1 ) + temp_output_1458_0);
				float2 temp_output_1489_0 = ( temp_output_1458_0 * float2( 0.25,0.2 ) );
				float blendOpSrc1480 = tex2D( _CloudNoise, panner1472 ).a;
				float blendOpDest1480 = ( 1.0 - tex2D( _CloudNoise, temp_output_1489_0 ).a );
				float lerpBlendMode1480 = lerp(blendOpDest1480,( blendOpDest1480 / max(blendOpSrc1480,0.00001) ),_CloudScatter);
				float3 normalizeResult1203 = normalize( WorldPosition );
				float2 appendResult1229 = (float2(( atan2( WorldPosition.x , WorldPosition.z ) / ( 2.0 * UNITY_PI ) ) , ( ( asin( (normalizeResult1203).y ) / 2.0 ) / ( 0.5 * UNITY_PI ) )));
				float2 Spheremap1240 = ( appendResult1229 * _StarsTiling );
				float temp_output_1784_0 = (Spheremap1240).y;
				float saferPower1804 = abs( temp_output_1784_0 );
				float Ground1802 = saturate( pow( saferPower1804 , 0.5 ) );
				float dotResult1271 = dot( i.ase_texcoord1.xyz , float3(0,-1,0) );
				float blendOpSrc1815 = Ground1802;
				float blendOpDest1815 = step( dotResult1271 , 0.0 );
				float HorizonCutoff1403 = ( saturate( min( blendOpSrc1815 , blendOpDest1815 ) ));
				Gradient gradient1677 = NewGradient( 0, 2, 4, float4( 1, 1, 1, 0 ), float4( 1, 1, 1, 1 ), 0, 0, 0, 0, 0, 0, float2( 0, 0 ), float2( 0.06810662, 0.07353323 ), float2( 0.7264706, 0.282353 ), float2( 1, 0.9911803 ), 0, 0, 0, 0 );
				float2 UV22_g3 = ase_screenPosNorm.xy;
				float2 localUnStereo22_g3 = UnStereo( UV22_g3 );
				float2 break64_g2 = localUnStereo22_g3;
				float clampDepth69_g2 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
				#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g2 = ( 1.0 - clampDepth69_g2 );
				#else
				float staticSwitch38_g2 = clampDepth69_g2;
				#endif
				float3 appendResult39_g2 = (float3(break64_g2.x , break64_g2.y , staticSwitch38_g2));
				float4 appendResult42_g2 = (float4((appendResult39_g2*2.0 + -1.0) , 1.0));
				float4 temp_output_43_0_g2 = mul( unity_CameraInvProjection, appendResult42_g2 );
				float3 temp_output_46_0_g2 = ( (temp_output_43_0_g2).xyz / (temp_output_43_0_g2).w );
				float3 In72_g2 = temp_output_46_0_g2;
				float3 localInvertDepthDir72_g2 = InvertDepthDir72_g2( In72_g2 );
				float4 appendResult49_g2 = (float4(localInvertDepthDir72_g2 , 1.0));
				float saferPower1289 = abs( SampleGradient( gradient1677, (0.0 + (distance( mul( unity_CameraToWorld, appendResult49_g2 ) , float4( _WorldSpaceCameraPos , 0.0 ) ) - _ProjectionParams.y) * (1.0 - 0.0) / (_ProjectionParams.z - _ProjectionParams.y)) ).a );
				float FadeAmount1683 = saturate( pow( saferPower1289 , _FogFade ) );
				float CloudsAlpha1451 = ( ( 1.0 - ( saturate( lerpBlendMode1480 )) ) * HorizonCutoff1403 * dotResult1476 * FadeAmount1683 * SolidMask1310 );
				float Stars1255 = ( tex2D( _StarsTexture, rotator1378 ).a * SolidMask1310 * saturate( DayNight1248 ) * temp_output_1337_0 * ( 1.0 - CloudsAlpha1451 ) );
				Gradient gradient1429 = NewGradient( 0, 4, 2, float4( 1, 1, 1, 0 ), float4( 1, 0.8473313, 0.495283, 0.5676509 ), float4( 0.7924528, 0.3326806, 0.3598714, 0.8882429 ), float4( 0.5294118, 0.5021929, 0.4666667, 1 ), 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float temp_output_1272_0 = saturate( ( DayNight1248 + _DuskAmount ) );
				float4 SunColor1264 = SampleGradient( gradient1429, temp_output_1272_0 );
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = Unity_SafeNormalize( ase_worldViewDir );
				float dotResult1202 = dot( ase_worldViewDir , worldSpaceLightDir );
				float SunInView1207 = dotResult1202;
				float temp_output_1205_0 = min( _SunRadiusA , _SunRadiusB );
				float temp_output_1208_0 = max( _SunRadiusA , _SunRadiusB );
				float saferPower1228 = abs( saturate( -SunInView1207 ) );
				float temp_output_1228_0 = pow( saferPower1228 , _SunHalo );
				float4 lerpResult1249 = lerp( ( SunColor1264 * ( saturate( temp_output_1228_0 ) * pow( FadeAmount1683 , _SunHaloEdgeBlend ) * 0.6 ) ) , float4( float3(0,0,0) , 0.0 ) , ( ( 1.0 - temp_output_1228_0 ) * HorizonCutoff1403 ));
				float4 SunDisk1256 = saturate( ( ( SunColor1264 * pow( saturate( (1.0 + (-SunInView1207 - ( 1.0 - ( temp_output_1205_0 * temp_output_1205_0 ) )) * (0.0 - 1.0) / (( 1.0 - ( temp_output_1208_0 * temp_output_1208_0 ) ) - ( 1.0 - ( temp_output_1205_0 * temp_output_1205_0 ) ))) ) , _SunPower ) * SolidMask1310 * HorizonCutoff1403 ) + lerpResult1249 ) );
				Gradient gradient1841 = NewGradient( 0, 2, 3, float4( 1, 1, 1, 0 ), float4( 1, 1, 1, 1 ), 0, 0, 0, 0, 0, 0, float2( 0, 0 ), float2( 0.007843138, 0.1147021 ), float2( 1, 1 ), 0, 0, 0, 0, 0 );
				float temp_output_1_0_g4 = _ProjectionParams.y;
				float Visibility1827 = SampleGradient( gradient1841, saturate( ( ( ( _ProjectionParams.z - temp_output_1_0_g4 ) / ( 500.0 - temp_output_1_0_g4 ) ) * temp_output_1337_0 ) ) ).a;
				float saferPower1277 = abs( ( 1.0 - abs( dotResult1271 ) ) );
				Gradient gradient1834 = NewGradient( 0, 2, 3, float4( 1, 1, 1, 0 ), float4( 1, 1, 1, 1 ), 0, 0, 0, 0, 0, 0, float2( 0, 0 ), float2( 0.01816297, 0.7705958 ), float2( 1, 1 ), 0, 0, 0, 0, 0 );
				float ReconstructedDepth1732 = SampleGradient( gradient1677, (0.0 + (distance( mul( unity_CameraToWorld, appendResult49_g2 ) , float4( _WorldSpaceCameraPos , 0.0 ) ) - _ProjectionParams.y) * (1.0 - 0.0) / (_ProjectionParams.z - _ProjectionParams.y)) ).a;
				float saferPower1752 = abs( SampleGradient( gradient1834, ReconstructedDepth1732 ).a );
				float3 desaturateInitialColor1717 = SunColor1264.rgb;
				float desaturateDot1717 = dot( desaturateInitialColor1717, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar1717 = lerp( desaturateInitialColor1717, desaturateDot1717.xxx, SunInView1207 );
				float3 Horizon1380 = ( pow( saferPower1277 , _Horizon ) * saturate( pow( saferPower1752 , 2.0 ) ) * desaturateVar1717 * FadeAmount1683 * 0.4 );
				float4 color1511 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float4 color1465 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 lerpResult1514 = lerp( color1465 , SunColor1264 , ( 1.0 - SunInView1207 ));
				float2 panner1518 = ( mulTime1473 * float2( 0.01,0.01 ) + temp_output_1489_0);
				float dotResult1508 = dot( float3(0,1,0) , UnpackNormal( tex2D( _TextureSample2, panner1518 ) ) );
				float4 lerpResult1510 = lerp( color1511 , lerpResult1514 , dotResult1508);
				float4 CloudColor1468 = saturate( ( lerpResult1510 * ( _CloudWhiteness * CloudsAlpha1451 ) * ( 1.0 - DayNight1248 ) ) );
				float4 lerpResult1467 = lerp( ( float4( desaturateVar1698 , 0.0 ) + Stars1255 + ( SunDisk1256 * Visibility1827 ) + float4( ( Horizon1380 * Visibility1827 ) , 0.0 ) ) , CloudColor1468 , ( CloudsAlpha1451 * temp_output_1705_0 * Visibility1827 ));
				float4 break1363 = lerpResult1467;
				float4 appendResult1362 = (float4(break1363.r , break1363.g , break1363.b , FadeAmount1683));
				
				
				finalColor = appendResult1362;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback "Skybox/Procedural"
}
/*ASEBEGIN
Version=18935
-1920;0;1920;1059;-4403.435;898.3868;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;1195;-694.0165,-1994.653;Inherit;False;1936.427;594.839;;14;1235;1229;1224;1222;1220;1218;1215;1214;1211;1206;1203;1336;1240;1334;UV Space;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1334;-659.6759,-1911.377;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;1203;-458.0146,-1655.751;Inherit;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;1206;-199.3687,-1659.516;Inherit;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ASinOpNode;1211;6.100341,-1652.147;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;1214;156.4833,-1789.992;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ATan2OpNode;1218;-318.6083,-1889.229;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;1215;111.1003,-1527.147;Inherit;False;1;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;1220;-356.0832,-1773.991;Inherit;False;1;0;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;1224;-150.0837,-1889.994;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;1222;362.1003,-1657.397;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;1229;600.1473,-1896.054;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1336;561.3533,-1768.442;Inherit;False;Property;_StarsTiling;StarsTiling;8;0;Create;True;0;0;0;False;0;False;0;4.88;1;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1445;2694.081,580.2797;Inherit;False;3767.936;1705.207;;37;1446;1447;1448;1449;1451;1454;1459;1458;1450;1460;1461;1463;1464;1471;1472;1473;1479;1480;1474;1475;1476;1489;1491;1494;1503;1506;1508;1509;1510;1518;1553;1588;1590;1608;1606;1691;1718;Clouds;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1235;828.8713,-1896.917;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;8;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;1665;4337.883,-530.6304;Inherit;False;Reconstruct World Position From Depth;-1;;2;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;1666;4430.773,-443.8026;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;1197;-1013.889,1073.552;Inherit;False;3078.883;1274.357;Comment;40;1252;1251;1249;1310;1233;1405;1375;1234;1244;1231;1426;1227;1309;1365;1425;1428;1226;1427;1304;1228;1217;1421;1221;1209;1225;1407;1213;1219;1205;1208;1201;1210;1204;1207;1202;1199;1200;1684;1813;1814;Sun Disk;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1240;1013.796,-1902.521;Inherit;False;Spheremap;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1449;2766.807,1397.356;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;1230;-2285.854,268.7061;Inherit;False;1262.359;557.5327;0-1 day;2;1238;1342;Sky Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.ProjectionParams;1676;4785.646,-263.3813;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;1450;3009.506,1398.083;Inherit;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1783;78.85511,499.082;Inherit;True;1240;Spheremap;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;1667;4778.544,-480.803;Inherit;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1342;-2227.796,624.3172;Inherit;False;Constant;_Vector4;Vector 4;14;0;Create;True;0;0;0;False;0;False;0,-1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;1238;-2263.83,437.7209;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;1200;-846.816,1649.313;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;1199;-902.4789,1802.195;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;1668;5100.664,-480.9028;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;700;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1202;-621.2647,1733.316;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1243;-1975.471,530.4926;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;1448;3313.653,1389.773;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;1784;298.4831,499.2393;Inherit;True;False;True;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;1454;3088.779,1693.322;Inherit;True;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1463;3229.477,1271.603;Inherit;False;Property;_CloudHeight;CloudHeight;14;0;Create;True;0;0;0;False;0;False;0;0.46;0.2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1677;5059.184,-718.7015;Inherit;False;0;2;4;1,1,1,0;1,1,1,1;0,0;0.06810662,0.07353323;0.7264706,0.282353;1,0.9911803;0;1;OBJECT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1207;-487.0094,1728.452;Inherit;False;SunInView;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1285;5182.616,-159.7402;Inherit;False;Property;_FogFade;FogFade;7;0;Create;True;0;0;0;False;0;False;1;5;0.2;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1444;1773.728,-1515.91;Inherit;False;2421.298;1315.932;;22;1324;1325;1326;1327;1323;1328;1329;1379;1341;1330;1378;1368;1369;1312;1337;1253;1311;1255;1520;1559;1585;1586;STARS;1,1,1,1;0;0
Node;AmplifyShaderEditor.GradientSampleNode;1680;5302.173,-642.9435;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;1804;623.821,502.0257;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1258;-738.6635,-997.0153;Inherit;False;2318.952;1278.672;;25;1266;1267;1380;1382;1384;1277;1383;1276;1263;1391;1278;1271;1385;1402;1403;1717;1733;1752;1753;1772;1815;1816;1833;1834;1835;Horizon;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1248;-1735.356,701.7637;Inherit;False;DayNight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;1459;3502.992,1683.424;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1262;-2288.963,1108.742;Inherit;False;1060.232;512.7081;shift sunlight to red at dusk;7;1430;1272;1268;1265;1274;1526;1429;Sun Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1464;3518.79,1312.166;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1210;117.4125,2020.665;Inherit;False;1207;SunInView;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1782;1013.736,500.8227;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1274;-2256.824,1532.888;Inherit;False;Property;_DuskAmount;DuskAmount;12;0;Create;True;0;0;0;False;0;False;0.5;0.8;0.2;0.8;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1471;3594.389,2043.009;Inherit;False;Property;_Windspeed;Windspeed;15;0;Create;True;0;0;0;False;0;False;0.1;-0.13;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1268;-2160.755,1432.127;Inherit;False;1248;DayNight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1289;5693.402,-184.8972;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1324;1936,-1328;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;1267;-688.6633,-947.015;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1201;-974.5771,1389.715;Inherit;False;Property;_SunRadiusB;Sun Radius B;3;0;Create;True;0;0;0;False;0;False;0;0.0553;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1266;-667.7512,-795.5538;Inherit;False;Constant;_Vector2;Vector 2;17;0;Create;True;0;0;0;False;0;False;0,-1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;1458;3794.072,1396.14;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1204;-973.0161,1259.452;Inherit;False;Property;_SunRadiusA;Sun Radius A;2;0;Create;True;0;0;0;False;0;False;0;0.0184;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;1325;2183.117,-1329.11;Inherit;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMinOpNode;1205;-662.3954,1263.873;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1694;5945.635,-184.8358;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;1208;-659.2136,1371.172;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1265;-1981.69,1435.912;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1802;1465.493,397.2177;Inherit;False;Ground;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;1473;4004.713,1796.718;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1271;-471.3631,-924.9932;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1447;3731.342,971.181;Inherit;True;Property;_CloudNoise;CloudNoise;13;0;Create;True;0;0;0;False;0;False;None;069891be2dd3add40b2369f064c8ed83;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.NegateNode;1219;309.3545,2013.384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1489;4108.6,976.5787;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.25,0.2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;1225;454.7036,1991.721;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;1326;2368.878,-1245.179;Inherit;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;1402;-388.9421,-651.7049;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1429;-1951.33,1154.9;Inherit;False;0;4;2;1,1,1,0;1,0.8473313,0.495283,0.5676509;0.7924528,0.3326806,0.3598714,0.8882429;0.5294118,0.5021929,0.4666667,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1209;-512.4595,1263.365;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1213;-509.5887,1377.497;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1407;296.7268,2101.44;Inherit;False;Property;_SunHalo;SunHalo;10;0;Create;True;0;0;0;False;0;False;4.97;1;1;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;1472;4121.141,1566.139;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1816;-358.8655,-431.7461;Inherit;False;1802;Ground;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1683;6143.404,-191.8036;Inherit;True;FadeAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1479;4360.308,1333.801;Inherit;True;Property;_TextureSample1;Texture Sample 1;14;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;1684;-338.291,1588.332;Inherit;False;1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1272;-1864.036,1435.42;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1212;-549.5321,1155.369;Inherit;False;1207;SunInView;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1327;2601.573,-1078.888;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1221;-358.459,1378.279;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;1430;-1717.265,1153.687;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;1588;4660.366,1535.53;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;1309;-119.0649,1578.263;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1217;-368.11,1264.933;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1228;668.0889,1965.219;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1421;137.8183,1885.823;Inherit;False;Property;_SunHaloEdgeBlend;SunHaloEdgeBlend;11;0;Create;True;0;0;0;False;0;False;1000;6.27;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1304;190.9646,1785.357;Inherit;False;1683;FadeAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1446;4352.71,1105.988;Inherit;True;Property;_TextureSample0;Texture Sample 0;14;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1590;4287.656,1687.727;Inherit;False;Property;_CloudScatter;CloudScatter;18;0;Create;True;0;0;0;False;0;False;0;0.862;0.5;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;1815;-126.8655,-594.7461;Inherit;True;Darken;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;1216;-357.13,1162.318;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1732;5354.665,-424.8639;Inherit;False;ReconstructedDepth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1403;189.5008,-558.7775;Inherit;False;HorizonCutoff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;1323;2370.221,-1334.662;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;1425;751.2286,1636.464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;1226;-153.5459,1294.914;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1427;472.8667,1841.668;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1428;774.9735,1738.476;Inherit;False;Constant;_Float2;Float 2;12;0;Create;True;0;0;0;False;0;False;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1475;5202.771,1620.919;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1310;195.9153,1568.855;Inherit;False;SolidMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1264;-1417.367,1150.081;Inherit;False;SunColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1328;2751.573,-1082.888;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1474;5217.427,1779.579;Inherit;False;Constant;_Vector0;Vector 0;17;0;Create;True;0;0;0;False;0;False;0,1.36,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BlendOpsNode;1480;4833.046,1272.957;Inherit;True;Divide;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1365;380.2331,1108.522;Inherit;True;1264;SunColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1503;5177.379,1164.174;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1426;925.357,1624.644;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1329;2933.19,-1321.489;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1461;5378.475,1222.841;Inherit;True;1403;HorizonCutoff;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1733;238.3923,-101.5975;Inherit;True;1732;ReconstructedDepth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1691;5376.194,1413.972;Inherit;False;1683;FadeAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1476;5491.276,1619.426;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1718;5386.508,1490.952;Inherit;False;1310;SolidMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1231;-128.5809,1480.134;Inherit;False;Property;_SunPower;SunPower;4;0;Create;True;0;0;0;False;0;False;0;5.3;1;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ProjectionParams;1822;4835.747,-1468.786;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;1227;32.79408,1294.17;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1233;900.1481,1853.495;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1405;424.0385,1590.921;Inherit;True;1403;HorizonCutoff;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1834;385.153,-205.6896;Inherit;False;0;2;3;1,1,1,0;1,1,1,1;0,0;0.01816297,0.7705958;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.PowerNode;1337;2794.712,-676.1814;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;1835;618.153,-140.6896;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;1518;4747.351,973.4072;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.01,0.01;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1375;996.2978,1115.387;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1460;5690.793,1163.081;Inherit;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1330;3080.75,-1195.916;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1814;1141.988,1826.767;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1379;2710.623,-949.1236;Inherit;False;Property;_StarsRotationSpeed;StarsRotationSpeed;9;0;Create;True;0;0;0;False;0;False;0;0.0066;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1585;3074.449,-1412.063;Inherit;False;Property;_StarsTile;StarsTile;17;0;Create;True;0;0;0;False;0;False;0;0.97;0.5;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;1278;-110.3188,-924.2631;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1234;191.5671,1418.187;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1515;5103.592,422.208;Inherit;False;1207;SunInView;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1244;1136.566,1340.62;Inherit;False;Constant;_Black;Black;17;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;1824;5340.266,-1474.5;Inherit;False;Inverse Lerp;-1;;4;09cbe79402f023141a4dc1fddd4c9511;0;3;1;FLOAT;300;False;2;FLOAT;500;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1263;241.87,-930.9716;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1385;-56.96741,-366.1963;Inherit;False;1264;SunColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1465;5279.243,180.5684;Inherit;False;Constant;_CloudsColor;Clouds Color;16;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;1249;1361.531,1321.867;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1391;-56.78528,-281.9064;Inherit;False;1207;SunInView;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1451;5936.475,1155.971;Inherit;False;CloudsAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;1341;3006.641,-945.9386;Inherit;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1752;612.9003,-508.4364;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1506;5048.072,941.7697;Inherit;True;Property;_TextureSample2;Texture Sample 2;16;0;Create;True;0;0;0;False;0;False;-1;None;dbaa92ba7db42704a85a851c6ec42c96;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1276;145.5084,-643.4259;Inherit;False;Property;_Horizon;Horizon;5;0;Create;True;0;0;0;False;0;False;6.2;8.56;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1839;5614.938,-1394.813;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1586;3287.75,-1265.811;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;1516;5283.91,424.3597;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1251;715.1922,1226.509;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;1509;5054.315,780.8196;Inherit;False;Constant;_Vector1;Vector 1;19;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;1496;5311.084,507.01;Inherit;False;1264;SunColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1384;754.6476,-428.2093;Inherit;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1491;6250.587,974.3992;Inherit;False;1248;DayNight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1383;472.5449,-706.8677;Inherit;True;1683;FadeAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1606;5979.312,744.7211;Inherit;False;Property;_CloudWhiteness;CloudWhiteness;19;0;Create;True;0;0;0;False;0;False;1;10;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1247;-1742.676,501.6989;Inherit;False;Property;_SkyNight;SkyNight;1;0;Create;True;0;0;0;False;0;False;0.5660378,0.4672482,0.4672482,0;0.04453837,0.06069587,0.1226329,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1520;3348.045,-504.5638;Inherit;False;1451;CloudsAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1368;3377.238,-594.3051;Inherit;False;1248;DayNight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;1717;166.8929,-289.6255;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;1511;5525.901,159.6654;Inherit;False;Constant;_CloudsColordark;Clouds Color dark;16;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;1252;1660.845,1215.25;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1250;-1743.117,331.8415;Inherit;False;Property;_SkyDay;SkyDay;0;0;Create;True;0;0;0;False;0;False;0.5660378,0.4672482,0.4672482,0;0.3655641,0.4574289,0.5,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;1826;5831.135,-1394.217;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1514;5515.447,345.6149;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;1277;483.3081,-941.9279;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1508;5462.933,861.7148;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1753;757.1001,-623.7359;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;1378;3242.494,-994.0188;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.1,0.9;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GradientNode;1841;5716.481,-1553.744;Inherit;False;0;2;3;1,1,1,0;1,1,1,1;0,0;0.007843138,0.1147021;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1382;970.614,-757.1031;Inherit;True;5;5;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;1813;1913.649,1320.569;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1312;3369.542,-736.8191;Inherit;False;1310;SolidMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1510;5934.333,491.8384;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;1369;3546.157,-592.1822;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1254;-1456.835,412.8286;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1494;6429.359,912.7611;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;1842;5988.481,-1559.744;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;1559;3539.732,-503.6943;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1253;3442.161,-1023.505;Inherit;True;Property;_StarsTexture;StarsTexture;6;0;Create;True;0;0;0;False;0;False;-1;None;e474a57db8c003146a084a65b9939696;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1608;6257.687,661.8621;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1696;5450.375,-330.927;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1699;5646.22,-648.6992;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1257;-849.8181,417.1964;Inherit;False;SkyColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1827;6303.314,-1399.612;Inherit;True;Visibility;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1705;5798.737,-376.4401;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1311;3758.833,-759.2067;Inherit;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1380;1403.648,-760.7583;Inherit;True;Horizon;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1256;2305.852,1211.562;Inherit;False;SunDisk;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1490;6575.311,576.0009;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1255;3910.905,-763.3586;Inherit;True;Stars;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1828;6409.455,-1123.902;Inherit;False;1827;Visibility;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1260;6164.077,-908.8248;Inherit;False;1257;SkyColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1259;6558.11,-1419.579;Inherit;False;1256;SunDisk;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;1607;6867.483,497.786;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1381;6687.439,-1243.642;Inherit;False;1380;Horizon;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1704;5997.212,-669.0952;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1829;6746.455,-1345.902;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1468;7187.383,477.2751;Inherit;True;CloudColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1825;6878.809,-1230.953;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1261;6722.212,-1489.136;Inherit;False;1255;Stars;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1453;7081.697,-1198.16;Inherit;False;1451;CloudsAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;1698;6372.857,-913.011;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1469;7292.717,-1301.044;Inherit;False;1468;CloudColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1273;7022.541,-1459.648;Inherit;True;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1692;7303.866,-1138.34;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1467;7631.188,-1325.104;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;1363;8171.378,-1300.532;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;1727;8118.938,-1149.563;Inherit;False;1683;FadeAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1818;8174.777,-1906.781;Inherit;False;Constant;_Color0;Color 0;21;0;Create;True;0;0;0;False;0;False;0.5340241,1,0.4575472,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1803;7630.47,-1664.847;Inherit;False;1802;Ground;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;1806;7657.071,-1822.818;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.8;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1817;8196.526,-1544.242;Inherit;False;1732;ReconstructedDepth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1821;8493.793,-1678.381;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StickyNoteNode;1830;5453.458,-1698.682;Inherit;False;540.1172;111.9119;Visibility;;1,1,1,1;Will  base this on camera far clip, so fog adjusts to fit any draw distance. This also means we don't need to modify the skybox at all to change fog etc;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;1793;101.305,319.8815;Inherit;False;1683;FadeAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;1362;8334.563,-1300.09;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StickyNoteNode;1700;6103.875,-969.8016;Inherit;False;500.7544;180.0936;;;1,1,1,1;Once fog fade goes below one, sky will be desaturated;0;0
Node;AmplifyShaderEditor.ScreenDepthNode;1832;5105.093,-293.3214;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1775;7875.784,-1590.789;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1791;7235.261,-1979.25;Inherit;False;1257;SkyColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StickyNoteNode;1553;5103.366,1567.716;Inherit;False;648.7798;369.9;Fade clouds to horizon;;1,1,1,1;;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1786;89.07525,237.6898;Inherit;False;Property;_GroundLevel;GroundLevel;20;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1787;719.8834,240.7186;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;1833;380.153,-281.6896;Inherit;False;1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;1772;-635.9821,-510.0381;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1794;7231.133,-1886.887;Inherit;False;1264;SunColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1526;-1660.255,1430.118;Inherit;False;DuskTime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1789;275.8272,363.9254;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1792;7457.386,-1859.22;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.5019608,0.5019608,0.5019608,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1785;437.15,237.3089;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1820;8191.682,-1741.221;Inherit;False;Constant;_Color1;Color 1;21;0;Create;True;0;0;0;False;0;False;0,0.08817609,0.3679245,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1357;8690.352,-1314.68;Float;False;True;-1;2;ASEMaterialInspector;100;1;Skybox/Amplify Sky;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;True;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;True;2;False;-1;True;7;False;-1;True;False;17.17;False;-1;69.61;False;-1;True;1;RenderType=Transparent=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;Skybox/Procedural;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;1203;0;1334;0
WireConnection;1206;0;1203;0
WireConnection;1211;0;1206;0
WireConnection;1214;0;1211;0
WireConnection;1218;0;1334;1
WireConnection;1218;1;1334;3
WireConnection;1224;0;1218;0
WireConnection;1224;1;1220;0
WireConnection;1222;0;1214;0
WireConnection;1222;1;1215;0
WireConnection;1229;0;1224;0
WireConnection;1229;1;1222;0
WireConnection;1235;0;1229;0
WireConnection;1235;1;1336;0
WireConnection;1240;0;1235;0
WireConnection;1450;0;1449;0
WireConnection;1667;0;1665;0
WireConnection;1667;1;1666;0
WireConnection;1668;0;1667;0
WireConnection;1668;1;1676;2
WireConnection;1668;2;1676;3
WireConnection;1202;0;1200;0
WireConnection;1202;1;1199;0
WireConnection;1243;0;1238;0
WireConnection;1243;1;1342;0
WireConnection;1448;0;1450;0
WireConnection;1784;0;1783;0
WireConnection;1454;0;1450;0
WireConnection;1207;0;1202;0
WireConnection;1680;0;1677;0
WireConnection;1680;1;1668;0
WireConnection;1804;0;1784;0
WireConnection;1248;0;1243;0
WireConnection;1459;0;1454;0
WireConnection;1464;0;1448;0
WireConnection;1464;1;1463;0
WireConnection;1782;0;1804;0
WireConnection;1289;0;1680;4
WireConnection;1289;1;1285;0
WireConnection;1458;0;1464;0
WireConnection;1458;1;1459;0
WireConnection;1325;0;1324;0
WireConnection;1205;0;1204;0
WireConnection;1205;1;1201;0
WireConnection;1694;0;1289;0
WireConnection;1208;0;1204;0
WireConnection;1208;1;1201;0
WireConnection;1265;0;1268;0
WireConnection;1265;1;1274;0
WireConnection;1802;0;1782;0
WireConnection;1473;0;1471;0
WireConnection;1271;0;1267;0
WireConnection;1271;1;1266;0
WireConnection;1219;0;1210;0
WireConnection;1489;0;1458;0
WireConnection;1225;0;1219;0
WireConnection;1326;0;1325;0
WireConnection;1402;0;1271;0
WireConnection;1209;0;1205;0
WireConnection;1209;1;1205;0
WireConnection;1213;0;1208;0
WireConnection;1213;1;1208;0
WireConnection;1472;0;1458;0
WireConnection;1472;1;1473;0
WireConnection;1683;0;1694;0
WireConnection;1479;0;1447;0
WireConnection;1479;1;1489;0
WireConnection;1272;0;1265;0
WireConnection;1327;0;1326;0
WireConnection;1221;0;1213;0
WireConnection;1430;0;1429;0
WireConnection;1430;1;1272;0
WireConnection;1588;0;1479;4
WireConnection;1309;1;1684;0
WireConnection;1217;0;1209;0
WireConnection;1228;0;1225;0
WireConnection;1228;1;1407;0
WireConnection;1446;0;1447;0
WireConnection;1446;1;1472;0
WireConnection;1815;0;1816;0
WireConnection;1815;1;1402;0
WireConnection;1216;0;1212;0
WireConnection;1732;0;1680;4
WireConnection;1403;0;1815;0
WireConnection;1323;0;1325;0
WireConnection;1425;0;1228;0
WireConnection;1226;0;1216;0
WireConnection;1226;1;1217;0
WireConnection;1226;2;1221;0
WireConnection;1427;0;1304;0
WireConnection;1427;1;1421;0
WireConnection;1310;0;1309;0
WireConnection;1264;0;1430;0
WireConnection;1328;0;1327;0
WireConnection;1480;0;1446;4
WireConnection;1480;1;1588;0
WireConnection;1480;2;1590;0
WireConnection;1503;0;1480;0
WireConnection;1426;0;1425;0
WireConnection;1426;1;1427;0
WireConnection;1426;2;1428;0
WireConnection;1329;0;1323;0
WireConnection;1329;1;1328;0
WireConnection;1476;0;1475;0
WireConnection;1476;1;1474;0
WireConnection;1227;0;1226;0
WireConnection;1233;0;1228;0
WireConnection;1337;0;1327;0
WireConnection;1835;0;1834;0
WireConnection;1835;1;1733;0
WireConnection;1518;0;1489;0
WireConnection;1518;1;1473;0
WireConnection;1375;0;1365;0
WireConnection;1375;1;1426;0
WireConnection;1460;0;1503;0
WireConnection;1460;1;1461;0
WireConnection;1460;2;1476;0
WireConnection;1460;3;1691;0
WireConnection;1460;4;1718;0
WireConnection;1330;0;1329;0
WireConnection;1330;1;1323;0
WireConnection;1814;0;1233;0
WireConnection;1814;1;1405;0
WireConnection;1278;0;1271;0
WireConnection;1234;0;1227;0
WireConnection;1234;1;1231;0
WireConnection;1824;1;1822;2
WireConnection;1824;3;1822;3
WireConnection;1263;0;1278;0
WireConnection;1249;0;1375;0
WireConnection;1249;1;1244;0
WireConnection;1249;2;1814;0
WireConnection;1451;0;1460;0
WireConnection;1341;0;1379;0
WireConnection;1752;0;1835;4
WireConnection;1506;1;1518;0
WireConnection;1839;0;1824;0
WireConnection;1839;1;1337;0
WireConnection;1586;0;1330;0
WireConnection;1586;1;1585;0
WireConnection;1516;0;1515;0
WireConnection;1251;0;1365;0
WireConnection;1251;1;1234;0
WireConnection;1251;2;1310;0
WireConnection;1251;3;1405;0
WireConnection;1717;0;1385;0
WireConnection;1717;1;1391;0
WireConnection;1252;0;1251;0
WireConnection;1252;1;1249;0
WireConnection;1826;0;1839;0
WireConnection;1514;0;1465;0
WireConnection;1514;1;1496;0
WireConnection;1514;2;1516;0
WireConnection;1277;0;1263;0
WireConnection;1277;1;1276;0
WireConnection;1508;0;1509;0
WireConnection;1508;1;1506;0
WireConnection;1753;0;1752;0
WireConnection;1378;0;1586;0
WireConnection;1378;2;1341;0
WireConnection;1382;0;1277;0
WireConnection;1382;1;1753;0
WireConnection;1382;2;1717;0
WireConnection;1382;3;1383;0
WireConnection;1382;4;1384;0
WireConnection;1813;0;1252;0
WireConnection;1510;0;1511;0
WireConnection;1510;1;1514;0
WireConnection;1510;2;1508;0
WireConnection;1369;0;1368;0
WireConnection;1254;0;1250;0
WireConnection;1254;1;1247;0
WireConnection;1254;2;1248;0
WireConnection;1494;0;1491;0
WireConnection;1842;0;1841;0
WireConnection;1842;1;1826;0
WireConnection;1559;0;1520;0
WireConnection;1253;1;1378;0
WireConnection;1608;0;1606;0
WireConnection;1608;1;1451;0
WireConnection;1696;0;1285;0
WireConnection;1699;0;1696;0
WireConnection;1257;0;1254;0
WireConnection;1827;0;1842;4
WireConnection;1705;0;1476;0
WireConnection;1311;0;1253;4
WireConnection;1311;1;1312;0
WireConnection;1311;2;1369;0
WireConnection;1311;3;1337;0
WireConnection;1311;4;1559;0
WireConnection;1380;0;1382;0
WireConnection;1256;0;1813;0
WireConnection;1490;0;1510;0
WireConnection;1490;1;1608;0
WireConnection;1490;2;1494;0
WireConnection;1255;0;1311;0
WireConnection;1607;0;1490;0
WireConnection;1704;0;1699;0
WireConnection;1704;1;1705;0
WireConnection;1829;0;1259;0
WireConnection;1829;1;1828;0
WireConnection;1468;0;1607;0
WireConnection;1825;0;1381;0
WireConnection;1825;1;1828;0
WireConnection;1698;0;1260;0
WireConnection;1698;1;1704;0
WireConnection;1273;0;1698;0
WireConnection;1273;1;1261;0
WireConnection;1273;2;1829;0
WireConnection;1273;3;1825;0
WireConnection;1692;0;1453;0
WireConnection;1692;1;1705;0
WireConnection;1692;2;1828;0
WireConnection;1467;0;1273;0
WireConnection;1467;1;1469;0
WireConnection;1467;2;1692;0
WireConnection;1363;0;1467;0
WireConnection;1806;0;1792;0
WireConnection;1821;0;1818;0
WireConnection;1821;1;1820;0
WireConnection;1821;2;1817;0
WireConnection;1362;0;1363;0
WireConnection;1362;1;1363;1
WireConnection;1362;2;1363;2
WireConnection;1362;3;1727;0
WireConnection;1775;0;1806;0
WireConnection;1775;1;1467;0
WireConnection;1775;2;1803;0
WireConnection;1787;0;1785;0
WireConnection;1787;1;1789;0
WireConnection;1772;0;1271;0
WireConnection;1526;0;1272;0
WireConnection;1789;0;1793;0
WireConnection;1792;0;1791;0
WireConnection;1792;1;1794;0
WireConnection;1785;0;1784;0
WireConnection;1785;1;1786;0
WireConnection;1357;0;1362;0
ASEEND*/
//CHKSM=57D8CEC3E32029915B2DB3B6C81B743CC227921C