// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using System.Text.Json.Serialization;

namespace BootstrapBlazor.Components;

/// <summary>
/// 选项
/// </summary>
public class WebSerialOptions
{

    /// <summary>
    /// 波特率。默认 9600
    /// </summary>
    public int? BaudRate { get; set; } = 9600;

    /// <summary>
    /// 数据位, 7 或 8。默认 8
    /// </summary>
    public int? DataBits { get; set; } = 8;

    /// <summary>
    /// 校验位, none、even、odd。默认 "none"。
    /// </summary>
    [JsonIgnore]
    public WebSerialFlowControlType? ParityType { get; set; } = WebSerialFlowControlType.none;

    public string? Parity { get => ParityType.ToString(); }

    /// <summary>
    /// 停止位, 1 或 2。默认为1。
    /// </summary>
    public int? StopBits { get; set; } = 1;

    /// <summary>
    /// 读写缓冲区。默认 255
    /// </summary>
    public int? BufferSize { get; set; } = 255;

    /// <summary>
    /// 流控制, "none"或"hardware"。默认值为"none"。
    /// </summary>
    [JsonIgnore]
    public WebSerialParityType? FlowControlType { get; set; } = WebSerialParityType.none;

    public string? FlowControl { get => FlowControlType.ToString(); }


}

public enum WebSerialParityType
{
    /// <summary>
    /// 每个数据字不发送奇偶校验位
    /// </summary>
    none,

    /// <summary>
    /// 数据字加上奇偶校验位具有偶奇偶校验
    /// </summary>
    even,

    /// <summary>
    /// 数据字加奇偶校验位具有奇校验
    /// </summary>
    odd
}

public enum WebSerialFlowControlType
{
    /// <summary>
    /// 未启用流量控制
    /// </summary>
    none,

    /// <summary>
    /// 启用使用 RTS 和 CTS 信号的硬件流控制
    /// </summary>
    hardware,
}
