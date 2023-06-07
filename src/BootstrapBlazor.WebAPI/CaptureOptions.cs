// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************


using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace BootstrapBlazor.Components;

/// <summary>
/// 截屏
/// </summary>
public class CaptureOptions
{

    /// <summary>
    /// 持续获取截图
    /// </summary>
    /// <returns></returns>
    [DisplayName("持续获取截图")]
    public bool Continuous { get; set; }

    /// <summary>
    /// 使用摄像头,否则使用屏幕. 默认为 true
    /// </summary>
    /// <returns></returns>
    [DisplayName("使用摄像头")]
    public bool Camera { get; set; } = true;

    /// <summary>
    /// 显示log
    /// </summary>
    [DisplayName("显示log")]
    public bool Debug { get; set; }

    /// <summary>
    /// 图像质量,默认为 0.8
    /// </summary>
    [DisplayName("图像质量")]
    public double Quality { get; set; } = 0.8d;


}
