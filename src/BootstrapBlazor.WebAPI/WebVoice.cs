// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************


using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BootstrapBlazor.Components;

/// <summary>
/// Voice
/// </summary>
public class WebVoice
{
    /// <summary>
    /// 声音
    /// </summary>
    /// <returns></returns>
    [DisplayName("声音")]
    public string? Name { get; set; }

    /// <summary>
    /// 默认
    /// </summary>
    /// <returns></returns>
    [DisplayName("默认")]
    [JsonPropertyName("default")]
    public bool IsDefault { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    /// <returns></returns>
    [DisplayName("语言")]
    public string? Lang { get; set; }

    /// <summary>
    /// 语言URI
    /// </summary>
    /// <returns></returns>
    [DisplayName("语言URI")]
    public string? VoiceURI { get; set; }

    /// <summary>
    /// 本地服务
    /// </summary>
    /// <returns></returns>
    [DisplayName("本地服务")]
    public bool LocalService { get; set; }


}
