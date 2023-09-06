// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BootstrapBlazor.Components;

/// <summary>
/// 选项
/// </summary>
public class WebSerialOptions
{
    /// <summary>
    /// 波特率列表
    /// </summary>
    [JsonIgnore]
    public static List<int> BaudRateList = new List<int> { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 256000, 460800, 512000, 750000, 921600, 1500000 };

    /// <summary>
    /// 波特率。默认 9600
    /// </summary>
    public int? BaudRate { get; set; } = 9600;

    /// <summary>
    /// 数据位, 7 或 8。默认 8
    /// </summary>
    [DisplayName("数据位")]
    public int? DataBits { get; set; } = 8;

    /// <summary>
    /// 流控制, none、even、odd。默认 "none"。
    /// </summary>
    [JsonIgnore]
    [DisplayName("流控制")]
    public WebSerialFlowControlType? ParityType { get; set; } = WebSerialFlowControlType.none;

    [DisplayName("流控制")]
    public string? Parity { get => ParityType.ToString(); }

    /// <summary>
    /// 停止位, 1 或 2。默认为1。
    /// </summary>
    [DisplayName("停止位")]
    public int? StopBits { get; set; } = 1;

    /// <summary>
    /// 读写缓冲区。默认 255
    /// </summary>
    [DisplayName("读写缓冲区")]
    public int? BufferSize { get; set; } = 255;

    /// <summary>
    /// 校验位, "none"或"hardware"。默认值为"none"。
    /// </summary>
    [JsonIgnore]
    [DisplayName("校验")]
    public WebSerialParityType? FlowControlType { get; set; } = WebSerialParityType.none;

    [DisplayName("校验")]
    public string? FlowControl { get => FlowControlType.ToString(); }

    [DisplayName("HEX发送")]
    public bool InputWithHex { get; set; }

    [DisplayName("HEX接收")]
    public bool OutputInHex { get; set; }

    [DisplayName("自动断帧")]
    public bool AutoFrameBreak { get; set; } = true;

    [DisplayName("断帧字符(默认\\n)")]
    public string? FrameBreakChar { get; set; } 

}

public enum WebSerialParityType
{
    /// <summary>
    /// 每个数据字不发送奇偶校验位
    /// </summary>
    [Description("未启用")]
    none,

    /// <summary>
    /// 数据字加上奇偶校验位具有偶奇偶校验
    /// </summary>
    [Description("偶校验")]
    even,

    /// <summary>
    /// 数据字加奇偶校验位具有奇校验
    /// </summary>
    [Description("奇校验")]
    odd
}

public enum WebSerialFlowControlType
{
    /// <summary>
    /// 未启用流量控制
    /// </summary>
    [Description("未启用")]
    none,

    /// <summary>
    /// 启用使用 RTS 和 CTS 信号的硬件流控制
    /// </summary>
    [Description("硬件")]
    hardware,
}
