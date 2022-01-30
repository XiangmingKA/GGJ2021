using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class sh_ui_gui: ShaderGUI
{
    #region EnumsAndClasses

    public enum SurfaceType
    {
        Opaque,
        Transparent
    }

    public enum BlendMode
    {
        Alpha,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Premultiply, // Physically plausible transparency mode, implemented as alpha pre-multiply
        Additive,
        Multiply
    }

    public enum RenderFace
    {
        Front = 2,
        Back = 1,
        Both = 0
    }


    protected class Styles
    {
        // 表面基础属性
        public static readonly GUIContent SurfaceOptions = new GUIContent("表面属性",
            "显示Shader的基础表面属性");
        public static readonly GUIContent surfaceType = new GUIContent("表面类型",
            "指定该表面为 不透明的表面 或 带有透明度的表面");
        public static readonly GUIContent blendingMode = new GUIContent("混合模式",
            "指定该表面与背景的混合模式");
        public static readonly GUIContent cullingText = new GUIContent("双面渲染",
             "指定只渲染正面、背面或双面都渲染");
        public static readonly GUIContent RcolorOptions = new GUIContent("使用顶点R控制UV",
             "开启顶点R控制UV");
        public static readonly GUIContent GrayOptions = new GUIContent("去色开关",
          "开启顶点R控制UV");
        public static readonly GUIContent ValphaOptions = new GUIContent("使用顶点R控制透明度",
             "开启顶点R控制透明度");
        public static readonly GUIContent colordissOptions = new GUIContent("使用顶点G控制溶解值",
             "开启顶点R控制透明度");

        // 基本属性
        public static readonly GUIContent BasicOptions = new GUIContent("基本属性",
             "显示Shader的基本属性");
        public static readonly GUIContent mainTextureText = new GUIContent("贴图",
             "指定该Shader的贴图");

        public static readonly GUIContent dissTextureText = new GUIContent("溶解图",
         "指定该Shader的溶解图");
        public static readonly GUIContent maskTextureText = new GUIContent("遮罩",
             "指定该Shader的遮罩");
        
        
        public static readonly GUIContent alphaText = new GUIContent("透明度",
             "指定该Shader的整体透明度");
        public static readonly GUIContent alphaBreathSpeedText = new GUIContent("透明度呼吸速度",
             "指定透明度呼吸的速度");
        public static readonly GUIContent alphaMinText = new GUIContent("透明度最小值",
             "指定透明度呼吸时的最小值");    
        public static readonly GUIContent alphaMaxText = new GUIContent("透明度最大值",
             "指定透明度呼吸时的最大值");    
        public static readonly GUIContent scaleStretchText = new GUIContent("缩放程度",
             "指定缩放的半径范围"); 
        public static readonly GUIContent scaleMinText = new GUIContent("收缩最小值",
             "指定收缩的最小程度"); 
        public static readonly GUIContent scaleMaxText = new GUIContent("收缩最大值",
             "指定收缩的最大程度"); 
        public static readonly GUIContent scaleSpeedText = new GUIContent("缩放速度",
             "指定缩放的速度");    
        public static readonly GUIContent adjustColorText = new GUIContent("偏向色",
             "指定该Shader的偏向色");
        public static readonly GUIContent finalColorText = new GUIContent("最终偏向色修正",
            "指定该Shader的最终偏向色修正");
        public static readonly GUIContent rsspeedText = new GUIContent("运动速度",
             "指定该Shader的运动速度");
        public static readonly GUIContent _TexColorIntensityTex = new GUIContent("亮度",
          "修正偏向色的强度");


        // 溶解效果
        public static readonly GUIContent DissOptions =
               new GUIContent("溶解效果", "为Shader设置溶解效果");
        public static readonly GUIContent enableDissText =
               new GUIContent("启用溶解效果", "开启该选项，可为Shader设置溶解效果");
       
        public static readonly GUIContent dissStartText =
               new GUIContent("溶解值", "设定溶解值");
        public static readonly GUIContent dissMaxText =
               new GUIContent("溶解光滑度", "设定溶解的光滑度");
        public static readonly GUIContent dissColorText =
               new GUIContent("溶解边缘光色", "设置溶解的边缘发光颜色");
        public static readonly GUIContent dissAddText =
               new GUIContent("溶解边缘光宽度", "设置溶解的边缘发光宽度");
        public static readonly GUIContent dissEText =
               new GUIContent("溶解边缘光开关", "设置溶解的边缘发光开关");
    

        // 色彩调整
        public static readonly GUIContent ColortzOptions =
               new GUIContent("色彩修正", "为Shader设置色彩修正效果");
        public static readonly GUIContent enableCtzText =
               new GUIContent("启用色彩修正", "开启该选项，可为Shader设置色彩修正效果");
        public static readonly GUIContent CtzdbText =
               new GUIContent("色彩对比度", "指定该Shader的色彩对比度");
        public static readonly GUIContent CtzbhText =
               new GUIContent("色彩饱和度", "指定该shader的色彩饱和度");

        //UV扰动
        public static readonly GUIContent RdOptions =
               new GUIContent("扰动效果", "为Shader设置扰动效果");
        public static readonly GUIContent enableRdText =
               new GUIContent("启用扰动效果", "开启该选项，可为Shader设置扰动效果");
        public static readonly GUIContent rdTextureText =
               new GUIContent("扰动贴图", "指定该Shader的扰动贴图");
        public static readonly GUIContent rdspeedxText =
               new GUIContent("R扰动速度", "指定水扰动的X轴速率");
        public static readonly GUIContent rdspeedyText =
               new GUIContent("G扰动速度", "指定水扰动的Y轴速率");
        public static readonly GUIContent rdinstText =
               new GUIContent("扰动强度", "指定扰动的强度");


        //贴图旋转
        public static readonly GUIContent XuanzhuangOptions = new GUIContent("贴图旋转",
            "为Shader设置旋转效果");
        public static readonly GUIContent rotText = new GUIContent("贴图旋转角度",
            "指定该Shader的旋转角度");
        public static readonly GUIContent rotSpeedText = new GUIContent("贴图旋转速度",
            "指定该Shader的旋转速度");

        //遮罩旋转
        public static readonly GUIContent XuanzhuangMaskOptions = new GUIContent("遮罩旋转",
            "为Shader设置遮罩旋转效果");
        public static readonly GUIContent rotMaskText = new GUIContent("遮罩旋转角度",
            "指定该Shader的遮罩旋转角度");

    }

    #endregion

    protected MaterialEditor materialEditor { get; set; }
    // 表面属性
    protected MaterialProperty surfaceTypeProp { get; set; }
    protected MaterialProperty blendModeProp { get; set; }
    protected MaterialProperty cullingProp { get; set; }
    protected MaterialProperty rcolorProp { get; set; }
    protected MaterialProperty grayProp { get; set; }
    protected MaterialProperty valphaProp { get; set; }
    protected MaterialProperty colordissProp { get; set; }
    protected MaterialProperty alphaBreathProp { get; set; }



// 基本属性
protected MaterialProperty dissTextureProp { get; set; }
    protected MaterialProperty maskTextureProp { get; set; }
    protected MaterialProperty adjustColorProp { get; set; }
    protected MaterialProperty finalColorProp { get; set; }
    protected MaterialProperty alphaProp { get; set; }
    protected MaterialProperty alphaBreathSpeedProp { get; set; }
    protected MaterialProperty alphaMinProp { get; set; }
    protected MaterialProperty alphaMaxProp { get; set; }
    protected MaterialProperty TexColorIntensity { get; set; }
    protected MaterialProperty rsspeedProp { get; set; }
    protected MaterialProperty scaleBreathProp { get; set; }
    protected MaterialProperty ScaleStretchProp { get; set; }
    protected MaterialProperty ScaleMinProp { get; set; }
    protected MaterialProperty ScaleMaxProp { get; set; }
    protected MaterialProperty ScaleSpeedProp { get; set; }


    //贴图旋转
    protected MaterialProperty xuanProp { get; set; }
    protected MaterialProperty xuanzhuangProp { get; set; }
    protected MaterialProperty xuanzhuangSpeedProp { get; set; }
    //mask旋转
    protected MaterialProperty xuanMaskProp { get; set; }
    protected MaterialProperty xuanzhuangMaskProp { get; set; }

    //溶解效果
    protected MaterialProperty mainTextureProp { get; set; }
    protected MaterialProperty enableDissolveProp { get; set; }
    protected MaterialProperty dissolveValueProp { get; set; }
    protected MaterialProperty dissolveColorProp { get; set; }
    protected MaterialProperty dissolveSmoonProp { get; set; }
    protected MaterialProperty dissolveEdgeProp { get; set; }
    protected MaterialProperty dissolveEProp { get; set; }

    //扰动效果
    protected MaterialProperty enableRdProp { get; set; }
    protected MaterialProperty rdTextureProp { get; set; }
    protected MaterialProperty rdspeedxProp { get; set; }
    protected MaterialProperty rdspeedyProp { get; set; }
    protected MaterialProperty rdinstProp { get; set; }

    //色彩修正
    protected MaterialProperty enableCtzProp { get; set; }
    protected MaterialProperty CtzbhProp { get; set; }
    protected MaterialProperty CtzdbProp { get; set; }

    private bool foldoutTypeSurface = false;
    private bool foldoutTypeRd = false;
    private bool foldoutTypeBasic = true;
    private bool foldoutTypexuanzhuang = false;
    private bool foldoutTypexuanzhuangMask = false;
    private bool foldoutTypeCtz = false;
    private bool foldoutTypeDissolve = false;


    // private bool foldoutTypePaomo = false;
    // private bool foldoutTypejiaosan = false;

    private float enableXuan = 0;
    private float enableXuanMask = 0;
    private float enableCtz = 0;
    private float enableDissolve = 0;
    private float enableRd = 0;
    private float enableAlphaBreath = 0;
    private float enableScaleBreath = 0;


    public virtual void FindProperties(MaterialProperty[] properties)
    {
        // 表面属性
        surfaceTypeProp = FindProperty("_Surface", properties);
        blendModeProp = FindProperty("_Blend", properties);
        cullingProp = FindProperty("_CullMode", properties);
        rcolorProp = FindProperty("_san", properties);
        grayProp = FindProperty("_gray", properties);
        valphaProp = FindProperty("_valpha", properties);
        colordissProp = FindProperty("_colordiss", properties);
        alphaBreathProp = FindProperty("_alphaBreath", properties);
        alphaBreathSpeedProp = FindProperty("_AlphaBreathSpeed", properties);
        scaleBreathProp = FindProperty("_scaleBreath", properties);
        ScaleStretchProp = FindProperty("_ScaleStretch", properties);
        ScaleMinProp = FindProperty("_ScaleMin", properties);
        ScaleMaxProp = FindProperty("_ScaleMax", properties);
        ScaleSpeedProp = FindProperty("_ScaleSpeed", properties);



        //溶解效果
        dissTextureProp = FindProperty("_DissovleTexture", properties, false);
        enableDissolveProp = FindProperty("_diss", properties);
        dissolveValueProp = FindProperty("DissovleIntensity", properties);
        dissolveColorProp = FindProperty("DissovleBrightEdge_Color", properties);
        dissolveSmoonProp = FindProperty("_DissolveMax", properties);
        dissolveEdgeProp = FindProperty("DissovleBrightEdge_Width", properties);
        dissolveEProp = FindProperty("_disse", properties);

        //基本属性
        mainTextureProp = FindProperty("_MainTex", properties, false);
        maskTextureProp = FindProperty("_Mask", properties, false);
        adjustColorProp = FindProperty("_Color", properties, false);
        finalColorProp = FindProperty("_QColor", properties, false);
        alphaProp = FindProperty("_Alpha", properties, false);
        alphaMinProp = FindProperty("_AlphaMin", properties, false);
        alphaMaxProp = FindProperty("_AlphaMax", properties, false);
        TexColorIntensity = FindProperty("_TexColorIntensity", properties, false);
        rsspeedProp = FindProperty("_RSpeed", properties, false);

        xuanProp = FindProperty("_rot", properties, false);
        xuanzhuangProp = FindProperty("_Rotation", properties, false);
        xuanzhuangSpeedProp = FindProperty("_RotationSpeed", properties, false);

        xuanMaskProp = FindProperty("_rotmask", properties, false);
        xuanzhuangMaskProp = FindProperty("_RotationMASK", properties, false);

        //扰动效果
        enableRdProp = FindProperty("_UVrd", properties);
        rdTextureProp = FindProperty("NoiseTexture", properties);
        rdspeedxProp = FindProperty("_Speed", properties);
        rdspeedyProp = FindProperty("_Speed2", properties);
        rdinstProp = FindProperty("_Speed3", properties);

        //色彩修正效果
        enableCtzProp = FindProperty("_colortz", properties);
        CtzbhProp = FindProperty("_Saturation", properties);
        CtzdbProp = FindProperty("_Contrast", properties);


    }

public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] properties)
    {
        if (matEditor == null)
            throw new ArgumentNullException("materialEditor");

        materialEditor = matEditor;
        FindProperties(properties);

        DrawBanner();

        EditorGUILayout.Space();

        Material material = materialEditor.target as Material;

        EditorGUI.BeginChangeCheck();
        DrawSurfaceOptions(material);
        DrawBasicOptions(material);
        DrawXuanOptions(material);
        DrawXuanMaskOptions(material);
        ColorXzOptions(material);
        DrawDissolveOptions(material);
        DrawRdOptions(material);

        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in materialEditor.targets)
            {
                SetMaterialKeywords((Material)obj, properties);
            }
        }
        base.OnGUI(materialEditor, properties);
    }


    //版头图片
    static Texture2D bannerTexture = null;
    static GUIStyle title = null;
    static GUIStyle linkStyle = null;
    static string repoURL = "https://confluence.xindong.com/pages/viewpage.action?pageId=118010446";


    void DrawBanner()
    {
        if (bannerTexture == null)
            bannerTexture = Resources.Load<Texture2D>("tex_kp2_shader_header_bar");

        if (title == null)
        {
            title = new GUIStyle();
            title.fontSize = 20;
            title.alignment = TextAnchor.UpperLeft;
            title.normal.textColor = new Color(1f, 1f, 1f);
        }


        if (linkStyle == null) linkStyle = new GUIStyle();

        if (bannerTexture != null)
        {
            GUILayout.Space(4);
            var rect = GUILayoutUtility.GetRect(0, int.MaxValue, 120, 120);
            EditorGUI.DrawPreviewTexture(rect, bannerTexture, null, ScaleMode.ScaleAndCrop);
            EditorGUI.LabelField(rect, "UI专用: sh_ui", title);
            if (GUI.Button(rect, "", linkStyle))
            {
                Application.OpenURL(repoURL);
            }
            GUILayout.Space(4);
        }
    }

    public virtual void DrawSurfaceOptions(Material material)
    {
        foldoutTypeSurface = EditorGUILayout.Foldout(foldoutTypeSurface, Styles.SurfaceOptions, true);
        if (foldoutTypeSurface)
        {
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                EditorGUI.BeginChangeCheck();
                DoPopup(Styles.surfaceType, surfaceTypeProp, Enum.GetNames(typeof(SurfaceType)));
                if (EditorGUI.EndChangeCheck())
                {
                    if (surfaceTypeProp.floatValue == 1)
                    {
                        material.SetFloat("_ZWrite", 0);
                    }
                    else
                    {
                        material.SetFloat("_ZWrite", 1);
                    }
                }

                if ((SurfaceType)material.GetFloat("_Surface") == SurfaceType.Transparent)
                    DoPopup(Styles.blendingMode, blendModeProp, Enum.GetNames(typeof(BlendMode)));

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = cullingProp.hasMixedValue;
                var culling = (RenderFace)cullingProp.floatValue;
                culling = (RenderFace)EditorGUILayout.EnumPopup(Styles.cullingText, culling);
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(Styles.cullingText.text);
                    cullingProp.floatValue = (float)culling;
                    material.doubleSidedGI = (RenderFace)cullingProp.floatValue != RenderFace.Front;
                }

                EditorGUI.showMixedValue = false;
                if (rcolorProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = rcolorProp.hasMixedValue;
                    var standardPBR =
                        EditorGUILayout.ToggleLeft(Styles.RcolorOptions, rcolorProp.floatValue == 1.0f);
                    if (EditorGUI.EndChangeCheck())
                        rcolorProp.floatValue = standardPBR ? 1.0f : 0.0f;
                    EditorGUI.showMixedValue = false;
                }

                if (grayProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = grayProp.hasMixedValue;
                    var gray =
                        EditorGUILayout.ToggleLeft(Styles.GrayOptions, grayProp.floatValue == 1.0f);
                    if (EditorGUI.EndChangeCheck())
                        grayProp.floatValue = gray ? 1.0f : 0.0f;
                    EditorGUI.showMixedValue = false;
                }
                


                if (valphaProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = valphaProp.hasMixedValue;
                    var va =
                        EditorGUILayout.ToggleLeft(Styles.ValphaOptions, valphaProp.floatValue == 1.0f);
                    if (EditorGUI.EndChangeCheck())
                        valphaProp.floatValue = va ? 1.0f : 0.0f;
                    EditorGUI.showMixedValue = false;
                }

                if (colordissProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = colordissProp.hasMixedValue;
                    var colordiss =
                        EditorGUILayout.ToggleLeft(Styles.colordissOptions, colordissProp.floatValue == 1.0f);
                    if (EditorGUI.EndChangeCheck())
                        colordissProp.floatValue = colordiss ? 1.0f : 0.0f;
                    EditorGUI.showMixedValue = false;
                }
                


            }
        }
        GUILayout.Space(20);
    }

    public virtual void DrawBasicOptions(Material material)
    {
        foldoutTypeBasic = EditorGUILayout.Foldout(foldoutTypeBasic, Styles.BasicOptions, true);
        if (foldoutTypeBasic)
        {
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                if (mainTextureProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    materialEditor.TextureProperty(mainTextureProp, Styles.mainTextureText.text, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        materialEditor.RegisterPropertyChangeUndo(Styles.mainTextureText.text);
                        material.SetTexture("_MainTex", mainTextureProp.textureValue);
                    }
                }

                if (maskTextureProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    materialEditor.TextureProperty(maskTextureProp, Styles.maskTextureText.text, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        materialEditor.RegisterPropertyChangeUndo(Styles.maskTextureText.text);
                        material.SetTexture("_Mask", maskTextureProp.textureValue);
                    }
                }

                if (adjustColorProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    var adjustColor = adjustColorProp.colorValue;
                    adjustColor = EditorGUILayout.ColorField(Styles.adjustColorText, adjustColor, true, true, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        adjustColorProp.colorValue = adjustColor;
                    }
                }
                
                if (finalColorProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    var finalColor = finalColorProp.colorValue;
                    finalColor = EditorGUILayout.ColorField(Styles.finalColorText, finalColor, true, true, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        finalColorProp.colorValue = finalColor;
                    }
                }

                if(alphaBreathProp != null)
                {
                    enableAlphaBreath = alphaBreathProp.floatValue;
                    enableAlphaBreath = EditorGUILayout.ToggleLeft("开启透明度呼吸", enableAlphaBreath > 0 ? true : false) ? 1 : 0;
                    alphaBreathProp.floatValue = enableAlphaBreath;

                    if(enableAlphaBreath > 0)
                    {
                        if (alphaBreathSpeedProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = alphaBreathSpeedProp.hasMixedValue;
                            var alphaMinValue =
                                EditorGUILayout.Slider(Styles.alphaBreathSpeedText, alphaBreathSpeedProp.floatValue, 0.0f, 10.0f);
                            if (EditorGUI.EndChangeCheck())
                                alphaBreathSpeedProp.floatValue = alphaMinValue;
                            EditorGUI.showMixedValue = false;
                        }
                        if (alphaMinProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = alphaMinProp.hasMixedValue;
                            var alphaMinValue =
                                EditorGUILayout.Slider(Styles.alphaMinText, alphaMinProp.floatValue, 0.0f, 1.0f);
                            if (EditorGUI.EndChangeCheck())
                                alphaMinProp.floatValue = alphaMinValue;
                            EditorGUI.showMixedValue = false;
                        }
                        if (alphaMaxProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = alphaMaxProp.hasMixedValue;
                            var alphaMaxValue =
                                EditorGUILayout.Slider(Styles.alphaMaxText, alphaMaxProp.floatValue, 0.0f, 1.0f);
                            if (EditorGUI.EndChangeCheck())
                                alphaMaxProp.floatValue = alphaMaxValue;
                            EditorGUI.showMixedValue = false;
                        }
                        
                    }
                    else
                    {
                        if (alphaProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = alphaProp.hasMixedValue;
                            var alphaValue =
                                EditorGUILayout.Slider(Styles.alphaText, alphaProp.floatValue, 0.0f, 1.0f);
                            if (EditorGUI.EndChangeCheck())
                                alphaProp.floatValue = alphaValue;
                            EditorGUI.showMixedValue = false;
                        }
                    }
                }

                

                if (TexColorIntensity != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = TexColorIntensity.hasMixedValue;
                    var texInten =
                        EditorGUILayout.Slider(Styles._TexColorIntensityTex, TexColorIntensity.floatValue, 1f, 10f);
                    if (EditorGUI.EndChangeCheck())
                        TexColorIntensity.floatValue = texInten;
                    EditorGUI.showMixedValue = false;
                }

                if (rsspeedProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = rsspeedProp.hasMixedValue;
                    var alphaValue =
                        EditorGUILayout.Slider(Styles.rsspeedText, rsspeedProp.floatValue, -50f, 50f);
                    if (EditorGUI.EndChangeCheck())
                        rsspeedProp.floatValue = alphaValue;
                    EditorGUI.showMixedValue = false;
                }

                if (scaleBreathProp != null)
                {
                    enableScaleBreath = scaleBreathProp.floatValue;
                    enableScaleBreath = EditorGUILayout.ToggleLeft("开启缩放呼吸", enableScaleBreath > 0 ? true : false) ? 1 : 0;
                    scaleBreathProp.floatValue = enableScaleBreath;

                    if(enableScaleBreath > 0)
                    {
                        // if (ScaleStretchProp != null)
                        // {
                        //     EditorGUI.BeginChangeCheck();
                        //     EditorGUI.showMixedValue = ScaleStretchProp.hasMixedValue;
                        //     var scaleStretchValue =
                        //         EditorGUILayout.Slider(Styles.scaleStretchText, ScaleStretchProp.floatValue, 0.0f, 3.0f);
                        //     if (EditorGUI.EndChangeCheck())
                        //         ScaleStretchProp.floatValue = scaleStretchValue;
                        //     EditorGUI.showMixedValue = false;
                        // }
                        if (ScaleMinProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = ScaleMinProp.hasMixedValue;
                            var scaleMinValue =
                                EditorGUILayout.Slider(Styles.scaleMinText, ScaleMinProp.floatValue, 0.0f, 3.0f);
                            if (EditorGUI.EndChangeCheck())
                                ScaleMinProp.floatValue = scaleMinValue;
                            EditorGUI.showMixedValue = false;
                        }
                        if (ScaleMaxProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = ScaleMaxProp.hasMixedValue;
                            var scaleMaxValue =
                                EditorGUILayout.Slider(Styles.scaleMaxText, ScaleMaxProp.floatValue, 0.0f, 3.0f);
                            if (EditorGUI.EndChangeCheck())
                                ScaleMaxProp.floatValue = scaleMaxValue;
                            EditorGUI.showMixedValue = false;
                        }
                        if (ScaleSpeedProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = ScaleSpeedProp.hasMixedValue;
                            var scaleSpeedValue =
                                EditorGUILayout.Slider(Styles.scaleSpeedText, ScaleSpeedProp.floatValue, 0.0f, 25.0f);
                            if (EditorGUI.EndChangeCheck())
                                ScaleSpeedProp.floatValue = scaleSpeedValue;
                            EditorGUI.showMixedValue = false;
                        }
                    }
                }


            }
        }
        GUILayout.Space(20);
    }

    public void DrawRdOptions(Material material)
    {
        if (enableRd > 0)
        {
            foldoutTypeRd = EditorGUILayout.Foldout(foldoutTypeRd, Styles.RdOptions, true, EditorStyles.foldoutHeader);
        }
        else
        {
            foldoutTypeRd = EditorGUILayout.Foldout(foldoutTypeRd, Styles.RdOptions, true);
        }


        if (foldoutTypeRd)
        {
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                if (enableRdProp != null)
                {
                    enableRd = enableRdProp.floatValue;
                    enableRd = EditorGUILayout.ToggleLeft("开启扰动效果", enableRd > 0 ? true : false) ? 1 : 0;
                    enableRdProp.floatValue = enableRd;
                    if (enableRd > 0)
                    {
                        if (rdTextureProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            materialEditor.TextureProperty(rdTextureProp, Styles.rdTextureText.text, true);
                            if (EditorGUI.EndChangeCheck())
                            {
                                materialEditor.RegisterPropertyChangeUndo(Styles.rdTextureText.text);
                                material.SetTexture("NoiseTexture", rdTextureProp.textureValue);
                            }
                        }

                        if (rdspeedxProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = rdspeedxProp.hasMixedValue;
                            var XEnd =
                                EditorGUILayout.Slider(Styles.rdspeedxText, rdspeedxProp.floatValue, 0f, 10f);
                            if (EditorGUI.EndChangeCheck())
                                rdspeedxProp.floatValue = XEnd;
                            EditorGUI.showMixedValue = false;
                        }

                        if (rdspeedyProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = rdspeedyProp.hasMixedValue;
                            var YEnd =
                                EditorGUILayout.Slider(Styles.rdspeedyText, rdspeedyProp.floatValue, 0f, 10f);
                            if (EditorGUI.EndChangeCheck())
                                rdspeedyProp.floatValue = YEnd;
                            EditorGUI.showMixedValue = false;
                        }

                        if (rdinstProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = rdinstProp.hasMixedValue;
                            var Rd =
                                EditorGUILayout.Slider(Styles.rdinstText, rdinstProp.floatValue, 0f, 0.1f);
                            if (EditorGUI.EndChangeCheck())
                                rdinstProp.floatValue = Rd;
                            EditorGUI.showMixedValue = false;
                        }

                    }
                }
            }
        }
        GUILayout.Space(20);
    }

    public void DrawDissolveOptions(Material material)
    {
        if (enableDissolve > 0)
        {
            foldoutTypeDissolve = EditorGUILayout.Foldout(foldoutTypeDissolve, Styles.DissOptions, true, EditorStyles.foldoutHeader);
        }
        else
        {
            foldoutTypeDissolve = EditorGUILayout.Foldout(foldoutTypeDissolve, Styles.DissOptions, true);
        }


        if (foldoutTypeDissolve)
        {
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                if (enableDissolveProp != null)
                {
                    enableDissolve = enableDissolveProp.floatValue;
                    enableDissolve = EditorGUILayout.ToggleLeft("开启溶解效果", enableDissolve > 0 ? true : false) ? 1 : 0;
                    enableDissolveProp.floatValue = enableDissolve;
                    if (enableDissolve > 0)
                    {

                       

                        if (dissolveEProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = dissolveEProp.hasMixedValue;
                            var dissE =
                                EditorGUILayout.ToggleLeft(Styles.dissEText, dissolveEProp.floatValue == 1.0f);
                            if (EditorGUI.EndChangeCheck())
                                dissolveEProp.floatValue = dissE ? 1.0f : 0.0f;
                            EditorGUI.showMixedValue = false;
                        }

                     

                        //溶解
                        if (dissTextureProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            materialEditor.TextureProperty(dissTextureProp, Styles.dissTextureText.text, true);
                            if (EditorGUI.EndChangeCheck())
                            {
                                materialEditor.RegisterPropertyChangeUndo(Styles.dissTextureText.text);
                                material.SetTexture("_DissovleTexture", dissTextureProp.textureValue);
                            }
                        }



                        if (dissolveValueProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = dissolveValueProp.hasMixedValue;
                            var dissolveValue =
                                EditorGUILayout.Slider(Styles.dissStartText, dissolveValueProp.floatValue, 0f, 1f);
                            if (EditorGUI.EndChangeCheck())
                                dissolveValueProp.floatValue = dissolveValue;
                            EditorGUI.showMixedValue = false;
                        }


                        if (dissolveSmoonProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = dissolveSmoonProp.hasMixedValue;
                            var fogEnd =
                                EditorGUILayout.Slider(Styles.dissMaxText, dissolveSmoonProp.floatValue, 0f, 10f);
                            if (EditorGUI.EndChangeCheck())
                                dissolveSmoonProp.floatValue = fogEnd;
                            EditorGUI.showMixedValue = false;
                        }



                        if (dissolveEdgeProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = dissolveEdgeProp.hasMixedValue;
                            var EdgeEnd =
                                EditorGUILayout.Slider(Styles.dissAddText, dissolveEdgeProp.floatValue, 0f, 1f);
                            if (EditorGUI.EndChangeCheck())
                                dissolveEdgeProp.floatValue = EdgeEnd;
                            EditorGUI.showMixedValue = false;
                        }



                        if (dissolveColorProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            var dissolveColor = dissolveColorProp.colorValue;
                            dissolveColor = EditorGUILayout.ColorField(Styles.dissColorText, dissolveColor, true, true, true);
                            if (EditorGUI.EndChangeCheck())
                            {
                                dissolveColorProp.colorValue = dissolveColor;
                            }
                        }
                    }
                }
            }
        }
        GUILayout.Space(20);
    }

    public virtual void DrawXuanOptions(Material material)
    {
       

        if (enableXuan > 0)
        {
            foldoutTypexuanzhuang = EditorGUILayout.Foldout(foldoutTypexuanzhuang, Styles.XuanzhuangOptions, true, EditorStyles.foldoutHeader);
        }
        else
        {
            foldoutTypexuanzhuang = EditorGUILayout.Foldout(foldoutTypexuanzhuang, Styles.XuanzhuangOptions, true);
        }

        if (foldoutTypexuanzhuang)
        {
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                if (xuanProp != null)
                {
                    enableXuan = xuanProp.floatValue;
                    enableXuan = EditorGUILayout.ToggleLeft("开启贴图旋转效果", enableXuan > 0 ? true : false) ? 1 : 0;
                    xuanProp.floatValue = enableXuan;

                    if (enableXuan > 0)
                    {

                        if (xuanzhuangProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = xuanzhuangProp.hasMixedValue;
                            var snowStrength =
                                EditorGUILayout.Slider(Styles.rotText, xuanzhuangProp.floatValue, -180f, 180f);
                            if (EditorGUI.EndChangeCheck())
                                xuanzhuangProp.floatValue = snowStrength;
                            EditorGUI.showMixedValue = false;
                        }
                        if (xuanzhuangSpeedProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = xuanzhuangSpeedProp.hasMixedValue;
                            var snowStrength =
                                EditorGUILayout.Slider(Styles.rotSpeedText, xuanzhuangSpeedProp.floatValue, -0.2f, 0.2f);
                            if (EditorGUI.EndChangeCheck())
                                xuanzhuangSpeedProp.floatValue = snowStrength;
                            EditorGUI.showMixedValue = false;
                        }
                    }
                }
            }
        }
        GUILayout.Space(20);

    }

    public void ColorXzOptions(Material material)
    {
        if (enableCtz > 0)
        {
            foldoutTypeCtz = EditorGUILayout.Foldout(foldoutTypeCtz, Styles.ColortzOptions, true, EditorStyles.foldoutHeader);
        }
        else
        {
            foldoutTypeCtz = EditorGUILayout.Foldout(foldoutTypeCtz, Styles.ColortzOptions, true);
        }


        if (foldoutTypeCtz)
        {
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                if (enableCtzProp != null)
                {
                    enableCtz = enableCtzProp.floatValue;
                    enableCtz = EditorGUILayout.ToggleLeft("开启色彩修正效果", enableCtz > 0 ? true : false) ? 1 : 0;
                    enableCtzProp.floatValue = enableCtz;
                    if (enableCtz > 0)
                    {
                        if (CtzbhProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = CtzbhProp.hasMixedValue;
                            var ctzbh =
                                EditorGUILayout.Slider(Styles.CtzbhText, CtzbhProp.floatValue, 0f, 2f);
                            if (EditorGUI.EndChangeCheck())
                                CtzbhProp.floatValue = ctzbh;
                            EditorGUI.showMixedValue = false;
                        }

                        if (CtzdbProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = CtzdbProp.hasMixedValue;
                            var ctzdb =
                                EditorGUILayout.Slider(Styles.CtzdbText, CtzdbProp.floatValue, 0f, 2f);
                            if (EditorGUI.EndChangeCheck())
                                CtzdbProp.floatValue = ctzdb;
                            EditorGUI.showMixedValue = false;
                        }


                    }
                }
            }
        }
        GUILayout.Space(20);
    }

    public virtual void DrawXuanMaskOptions(Material material)
    {


        if (enableXuanMask > 0)
        {
            foldoutTypexuanzhuangMask = EditorGUILayout.Foldout(foldoutTypexuanzhuangMask, Styles.XuanzhuangMaskOptions, true, EditorStyles.foldoutHeader);
        }
        else
        {
            foldoutTypexuanzhuangMask = EditorGUILayout.Foldout(foldoutTypexuanzhuangMask, Styles.XuanzhuangMaskOptions, true);
        }

        if (foldoutTypexuanzhuangMask)
        {
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                if (xuanMaskProp != null)
                {
                    enableXuanMask = xuanMaskProp.floatValue;
                    enableXuanMask = EditorGUILayout.ToggleLeft("开启遮罩旋转效果", enableXuanMask > 0 ? true : false) ? 1 : 0;
                    xuanMaskProp.floatValue = enableXuanMask;

                    if (enableXuanMask > 0)
                    {

                        if (xuanzhuangMaskProp != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = xuanzhuangMaskProp.hasMixedValue;
                            var maskStrength =
                                EditorGUILayout.Slider(Styles.rotMaskText, xuanzhuangMaskProp.floatValue, -180f, 180f);
                            if (EditorGUI.EndChangeCheck())
                                xuanzhuangMaskProp.floatValue = maskStrength;
                            EditorGUI.showMixedValue = false;
                        }
                    }
                }
            }
        }
        GUILayout.Space(20);

    }


    public void DoPopup(GUIContent label, MaterialProperty property, string[] options)
    {
        DoPopup(label, property, options, materialEditor);
    }

    public static void DoPopup(GUIContent label, MaterialProperty property, string[] options, MaterialEditor materialEditor)
    {
        if (property == null)
            throw new ArgumentNullException("property");

        EditorGUI.showMixedValue = property.hasMixedValue;

        var mode = property.floatValue;
        EditorGUI.BeginChangeCheck();
        mode = EditorGUILayout.Popup(label, (int)mode, options);
        if (EditorGUI.EndChangeCheck())
        {
            materialEditor.RegisterPropertyChangeUndo(label.text);
            property.floatValue = mode;
        }

        EditorGUI.showMixedValue = false;
    }

    public static void SetupMaterialBlendMode(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        bool alphaClip = material.GetFloat("_ALPHATEST") == 1;
        if (alphaClip)
        {
            material.SetFloat("_ALPHATEST", 1);
        }
        else
        {
            material.SetFloat("_ALPHATEST", 0);
        }

        SurfaceType surfaceType = (SurfaceType)material.GetFloat("_Surface");
        if (surfaceType == SurfaceType.Opaque)
        {
            if (alphaClip)
            {
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                material.SetOverrideTag("RenderType", "TransparentCutout");
            }
            else
            {
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                material.SetOverrideTag("RenderType", "Opaque");
            }
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        else
        {
            BlendMode blendMode = (BlendMode)material.GetFloat("_Blend");
            var queue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            // Specific Transparent Mode Settings
            switch (blendMode)
            {
                case BlendMode.Alpha:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    break;
                case BlendMode.Premultiply:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    break;
                case BlendMode.Additive:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    break;
                case BlendMode.Multiply:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.EnableKeyword("_ALPHAMODULATE_ON");
                    break;
            }
            // General Transparent Material Settings
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = queue;
        }
    }

    public void SetMaterialKeywords(Material material, MaterialProperty[] properties)
    {
        SetupMaterialBlendMode(material);
        
    }
}
