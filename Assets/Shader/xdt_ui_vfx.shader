Shader "XDT/UI/VFX"
{
    Properties
    {


         _Surface("__surface", Float) = 0.0
         _Blend("__blend", Float) = 0.0
		 _ALPHATEST("__clip", Float) = 0
		 _SrcBlend("__src", Float) = 1.0
         _DstBlend("__dst", Float) = 0.0
		 _ZWrite("__zw", Float) = 1.0
		 _CullMode ("__cull", Float) = 2.0
         _MainTex ("贴图", 2D) = "white" {}
        //溶解
        _DissovleTexture("R溶解", 2D) = "white"{}
        _diss("溶解开关", Float) = 0
		_disse("溶解边缘光开关", Float) = 0
        DissovleIntensity("溶解值", Range( 0, 1)) = 1
        _DissolveMax("溶解光滑值", Range( 0, 10)) = 0
        DissovleBrightEdge_Width("溶解边缘发光宽度", Range(0,1)) = 0
        [HDR]DissovleBrightEdge_Color("溶解边缘发光颜色", Color) = (1,1,1,1)

        [HDR]_Color("偏向色", Color) = (1, 1, 1, 1)
        [HDR]_QColor("最终偏向色修正", Color) = (1, 1, 1, 1)
        _Alpha("透明度",  Range(0 , 1)) = 0
        _AlphaMin("透明度最小值", Range(0, 1)) = 0.2
        _AlphaMax("透明度最大值", Range(0, 1)) = 1
        _AlphaBreathSpeed("透明度呼吸速度", Range(0, 10)) = 0

        _RSpeed("运动速度", Range(-10 , 10)) = 0
        _Rotation("旋转", Range(-180, 180)) = 0
        _RotationSpeed("旋转速度", Range(-3, 3)) = 0
        _RotationMASK("遮罩旋转", Range(-180, 180)) = 0
        _rot("旋转开关", float) = 0
        _gray("去色开关", float) = 0
        _colordiss("使用顶点G控制溶解", float) = 0
        _rotmask("遮罩旋转开关", float) = 0
        _san("闪光顶点色开关", float) = 0
        _valpha("闪光顶点色开关2", float) = 0
        _alphaBreath("透明度呼吸开关", float) = 0
        _scaleBreath("缩放呼吸开关", float) = 0
        _ScaleStretch("缩放程度", float) = 1
        _ScaleMin("收缩最小值", float) = 0.5
        _ScaleMax("收缩最大值", float) = 1.5
        _ScaleSpeed("缩放速度", float) = 4
        _Mask("遮罩图", 2D) = "white" {}

        _Saturation("色彩饱和度", Float) = 1	//调整饱和度
		_Contrast("色彩对比度", Float) = 1		//调整对比度
        _TexColorIntensity("偏向色强度", Range(1 , 10)) = 1    //亮度
		_colortz("色彩修正开关", Float) = 0

        //UV扰动
		_UVrd("扰动开关", Float) = 0
		NoiseTexture("UV扰动贴图RG",2D) = "white"{}
		_Speed("R扰动速度",Range(-10,10))=0
		_Speed2("G扰动速度",Range(-10,10))=0
		_Speed3("扰动强度",Range(0,0.1))=0.02
		
	    

        //Stencil
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        
        
    }
    SubShader
    {
       Tags{ "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

       HLSLINCLUDE

		//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        //#include "UnityUI.cginc"

		//声明属性
		CBUFFER_START(UnityPerMaterial)

        uniform half _rot;
        uniform half _alphaBreath;
        uniform half _scaleBreath;
        uniform half _gray;
        uniform half _colordiss;
        uniform half _rotmask;
        uniform half _san;
        uniform half _valpha;
        uniform half _colortz;
        uniform float4 _ClipRect;
        uniform half _UVrd;
        
        uniform half _diss;
        uniform half _disse;
        half DissovleIntensity;
		half DissovleBrightEdge_Width;
		half4 DissovleBrightEdge_Color;
		half _DissolveMax;

        sampler2D _MainTex;
        sampler2D _Mask;
        sampler2D _DissovleTexture;
        sampler2D NoiseTexture;
        half4 _DissovleTexture_ST;
        half4 NoiseTexture_ST;
        half4 _MainTex_ST;
        half4 _Mask_ST;
        half4 _Color;
        half4 _QColor;
        half _Alpha;
        half _AlphaMin;
        half _AlphaMax;
        half _AlphaBreathSpeed;
        half _ScaleStretch;
        half _ScaleSpeed;
        half _ScaleMin;
        half _ScaleMax;
        half _RSpeed;
        half _Rotation;
        half _RotationSpeed;
        half _RotationMASK;
        half _TexColorIntensity;
       
        half _Saturation;//饱和度
		half _Contrast; //对比度
        
        //UV扰动
		half _Speed;
		half _Speed2;
		half _Speed3;
 
        CBUFFER_END
		ENDHLSL

        Pass
       {

            Stencil
		    {
			    Ref [_Stencil]
			    Comp [_StencilComp]
			    Pass [_StencilOp] 
			    ReadMask [_StencilReadMask]
			    WriteMask [_StencilWriteMask]
		    }

            Blend[_SrcBlend][_DstBlend]
            //Cull Back
            Cull [_CullMode]
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            struct VertexInput
			{
				 half4 vertex : POSITION;
				 half4 texcoord : TEXCOORD0;
                 float4 color : COLOR;
                 half4 maskUV : TEXCOORD1;
			};

            struct VertexOutput
			{
				half4 pos : SV_POSITION;
                float4 worldPosition : TEXCOORD1;
                half2 uvmask : TEXCOORD2;
				half2 uv : TEXCOORD3;
                float4 color : COLOR;
                half4 mask_uv : TEXCOORD4;
                half4 noise_uv : TEXCOORD5;
			};

            VertexOutput vert ( VertexInput v  )
			{
                VertexOutput o = (VertexOutput)0;
                o.worldPosition = v.vertex;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvmask = TRANSFORM_TEX(v.texcoord, _Mask);
                o.color = v.color;
                o.mask_uv = v.maskUV;
                //uv扰动
				o.noise_uv.xy = TRANSFORM_TEX(o.uv.xy, NoiseTexture);
				o.noise_uv.x += _Time * _Speed;
                o.noise_uv.y += _Time * _Speed2;

                return o;
            }


            float4 frag ( VertexOutput v  ) : SV_Target
			{   
                float2 UV = v.uv;
                float2 MaskUV = v.uvmask;
                float time = _Time * _RSpeed;

                if(_scaleBreath!=0)
                {
                    //float time = _Time.y * 2.0;//_Speed;
                    //float tiling = (sin(_Time.y * _ScaleSpeed) * _ScaleStretch + 1.0) * 0.5 + 1.0;
                    float sin01 = (sin(_Time.y * _ScaleSpeed) * 0.5 + 1.0);
                    float tiling = lerp(_ScaleMin, _ScaleMax, sin01);
                    float2 uvTiling = float2(tiling, tiling);
                    float offset = (1.0 - tiling) * 0.5;
                    float2 uvOffset = float2(offset, offset);
                    UV = UV * uvTiling + uvOffset;
                }
               
                //贴图旋转计算
                if (_rot!=0)
				{
                 
                   float2 div= (_MainTex_ST.xy / 2.0) + _MainTex_ST.zw;
				   UV -= div;
                   float s, c;
				   sincos(radians(_Rotation + _Time.w * _RotationSpeed * 180.0), s, c);
				   float2x2 rotMatrix = float2x2(c, -s, s, c);
				   UV = mul(UV.xy, rotMatrix);
                   UV.xy += div;
                }
                float4 col = tex2D(_MainTex, UV+ float2(time,0));

                //遮罩旋转
                if(_rotmask!=0)
                {
                   float2 divmask= (_Mask_ST.xy / 2.0) + _Mask_ST.zw;
				   MaskUV -= divmask;
                   float s2, c2;
				   sincos(radians(_RotationMASK), s2, c2);
				   float2x2 rotMatrixMask = float2x2(c2, -s2, s2, c2);
				   MaskUV = mul(MaskUV.xy, rotMatrixMask);
                   MaskUV.xy += divmask;
                }
                
                float mask = tex2D(_Mask, MaskUV).r;

                //顶点闪光控制
                if (_san!=0)
			    {
                    col = tex2D(_MainTex, UV+ float2((v.color.r-0.5)*2.0,0));
                }

              
                float4 kp2color = col * _Color; 

                //UV扰动计算
				if(_UVrd!=0)
				{
					//v.noise_uv.xy += v.diss_uv.zw;
					half4 noiseCol = tex2D(NoiseTexture, v.noise_uv.xy);
					half2 noiseuv = {noiseCol.r,noiseCol.g};
					noiseuv = (noiseuv-0.5)*2;
					kp2color = tex2D(_MainTex, v.uv.xy + noiseuv * _Speed3) * _Color;
				}

                if(_valpha!=0)
                {
                     kp2color.a = col.a * v.color.r * mask;
                }
                else
                {
                    if(_alphaBreath!=0)
                    {
                        half interp = sin(_Time.w * _AlphaBreathSpeed) * 0.5 + 0.5;
                        kp2color.a = col.a * lerp(_AlphaMin, _AlphaMax, interp) * mask;
                    }
                    else
                    {
                        kp2color.a = col.a * _Alpha * mask;
                    }
                } 
              
               
                //图像去色
                 if (_gray!=0)
				{
                    half grey = dot(kp2color.rgb, half3(0.22, 0.707, 0.071));
                    kp2color.rgb = half3(grey, grey, grey); 
                }




                //图像颜色调整
                if(_colortz!=0)
				{
					//饱和度计算
					half grax = 0.2125 * kp2color.r + 0.7154 * kp2color.g + 0.0721 * kp2color.b;
					half3 graxColor = half3(grax, grax, grax);
					kp2color.rgb = lerp(graxColor, kp2color.rgb, _Saturation);

					//对比度计算
					half3 avgColor = half3(0.5, 0.5, 0.5);
					kp2color.rgb = lerp(avgColor, kp2color.rgb, _Contrast);
				}
               
                kp2color = kp2color * _QColor * _TexColorIntensity;

                
                
                
                
                
            
                half _Clip1 = 1;

				//溶解计算
				if(_diss!=0)
				{

                  
                  


                    half2 uv_DissovleTexture = v.mask_uv.xy * _DissovleTexture_ST.xy + _DissovleTexture_ST.zw;
                    half NormalDisolve = tex2D( _DissovleTexture, uv_DissovleTexture ).r;

                    //使用顶点色G控制溶解值
                     if(_colordiss!=0)
				    {
                         DissovleIntensity = v.color.g; 
                    }


                    _Clip1 = saturate(NormalDisolve * _DissolveMax - (lerp(_DissolveMax,-1,DissovleIntensity)));
                   

                   

                    //溶解边缘发光计算
				    if(_disse!=0)
				    {	
					    half clipMap = saturate(1 - (_Clip1 +DissovleBrightEdge_Width));
					    kp2color.rgb += clipMap * DissovleBrightEdge_Color.rgb;
				    }
				}
                
               








                //支持 RectMask2D遮罩
                float2 inside = step(_ClipRect.xy,v.worldPosition.xy) * step(v.worldPosition.xy,_ClipRect.zw);
                kp2color.a *= inside.x * inside.y * _Clip1;

                return kp2color;
            }


            ENDHLSL

       }
    }

    CustomEditor "sh_ui_gui"
}
