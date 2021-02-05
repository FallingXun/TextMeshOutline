# TextMeshOutline
TextMeshPro描边投影优化

### TMP简介
TMP(TextMeshPro)比起原本使用的text，能有更丰富的字体效果表现，基于SDF的实现，可以让字体不会在放大显示后变得模糊，避免了字体贴图的突然扩张。同时，描边和投影效果也不再使用多个text偏移叠加的实现方式，能较大程度减少顶点数量和overdraw。

### 问题描述
TMP的描边效果，可以通过调节各个参数来修改描边效果，包括描边颜色、描边粗细等。然而随之带来的问题是，每个不一样的参数都会产生新的材质。MeshRender可以通过MPB + GPUInstance来实现合批，避免大量材质球的创建，而UGUI下CansvasRender则不能使用这个方法，从而导致大量材质球的产生，以及界面draw call严重上升。(当前版本 com.unity.textmeshpro@1.4.1)

### 问题分析
总体来说，问题主要有以下两个：
1. 产生大量材质球
2. UI的DC较高   

方案一：   
> 创建材质管理器，根据参数动态生成材质球，有相同参数的使用同一个材质   
>> 优点：减少了材质球的生成   
>> 缺点：参数类型太多时，动态生成的材质球也会很多

方案二：   
> 预规划参数，预先生成对应参数的材质球，使用时从固定的材质球中切换
>> 优点：较大程度限制了材质球的数量，减少批次数量   
>> 缺点：限制了美术效果的样式

方案三：   
> 修改参数传入方式，通过顶点参数传入，修改shader的相关参数读取
>> 优点：参数可随意设置不受限制，不再有额外材质产生，所有参数类型都可合批   
>> 缺点：需要改动插件源码，有一定复杂度

### 优化流程
方案三的优化方式能够有较好的收益，所以下面会对方案三的优化流程进行介绍。

查看TMP的Inspector面板（使用TMP_Shader.shader）

![TMP描边参数](https://github.com/FallingXun/TextMeshOutline/blob/main/Images/TMP描边参数.png)

可以看到，描边是通过 Softness、Dilate、Outline下的Color、Thickness 四个参数来设置效果的。

TMP_ShaderUtilities.cs中，可以找到对应的shader定义变量：   
> Softness：对应shader的_OutlineSoftness（暂未使用）   
> Dilate：对应shader的_FaceDilate   
> Outline下的Color：对应shader的_OutlineColor   
> Thickness：对应shader的_OutlineWidth   

因此，需要将这些参数传入顶点，这里使用了uv4来进行存储 faceDilate 和 outlineThickness，使用了 tangent 来存储 OutlineColor。（后面会介绍为什么使用 tangent ）

![TMP描边参数变量计算](https://github.com/FallingXun/TextMeshOutline/blob/main/Images/TMP描边参数变量计算.png)

可以看到，faceDilate 和 outlineThickness 的计算都和 scaleRatio_A 有关联，而 scaleRatio_A 对应shader里的 _ScaleRatioA ，所以，除了这两个参数外，还需要将 scaleRatio_A 传入顶点。

因此，一共有4个参数通过顶点传输，这里参考了 uv2 的使用方式，将 faceDilate 和 outlineThickness 通过 PackUV 方法压缩成 uv4.x，scaleRatio_A则为 uv4.y。

outlineColor 有 rgba 4个值信息，如果使用uv3，则需要压缩再传入，在shader侧再解压出来使用。而查看shader，并没有使用 tangent 进行计算，并且 tangent 也是 Vector4 的变量，所以将描边颜色直接传入 tangent ，可以直接在shader侧读出颜色直接使用。

然而，当transform发生旋转时，会发现输出的outLineColor发生了变化。经过分析，应该是顶点的 tangent 值传到shader前，乘上了旋转矩阵，因此为了保证最终的 tangent 值能够正确，需要在C#侧将 tangent 左乘上旋转矩阵的逆矩阵，即

![颜色值左乘旋转矩阵逆矩阵](https://github.com/FallingXun/TextMeshOutline/blob/main/Images/颜色值左乘旋转矩阵逆矩阵.png)

同理，投影效果相关参数的修改流程也是一样的。

修改完相关C#代码和shader后，会发现shader侧并不能成功读取到新增的 uv4 和tangent 的数据，因为Canvas默认优化这些数据的传输，如下图：

![Canvas的AdditionalShaderChannels](https://github.com/FallingXun/TextMeshOutline/blob/main/Images/Canvas的AdditionalShaderChannels.png)

![Canvas设置顶点数据读取](https://github.com/FallingXun/TextMeshOutline/blob/main/Images/Canvas设置顶点数据读取.png)

所以，需要在 TMPro_UGUI_Private.cs 中，原本计算使用增加 uv4 和 tangent 的使用。

### 优化结果

![TMP优化效果展示](https://github.com/FallingXun/TextMeshOutline/blob/main/Images/TMP优化效果展示.png)

如上图所示，优化后，这里显示的有340个TMP，参数各异，但都能合批处理。

### 优化总结
项目上使用最多的字体效果就是描边和投影，所以将这两个部分进行优化，就已经能满足绝大部分项目上的需求，相对来说还是比较实用的。然而还有存在一些小问题：
1. 修改字体投影效果，需要开启UnderlayOn，去掉宏的时候不能正常处理，暂未定位到原因。
2. 如果是实时发生旋转变化的UI，由于逆矩阵的计算放在C#侧，所以需要同步更新刷新颜色值。