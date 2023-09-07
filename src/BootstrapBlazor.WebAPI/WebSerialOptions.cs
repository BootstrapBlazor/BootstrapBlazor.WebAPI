// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
    /// 停止位, 1 或 2。默认为1。
    /// </summary>
    [DisplayName("停止位")]
    public int? StopBits { get; set; } = 1;

    /// <summary>
    /// 流控制, none、even、odd。默认 "none"。
    /// </summary>
    [JsonIgnore]
    [DisplayName("流控制")]
    public WebSerialFlowControlType? ParityType { get; set; } = WebSerialFlowControlType.none;

    [DisplayName("流控制")]
    public string? Parity { get => ParityType.ToString(); }

    /// <summary>
    /// 读写缓冲区。默认 255
    /// </summary>
    [DisplayName("读写缓冲区")]
    public int? BufferSize { get; set; } = 255;

    /// <summary>
    /// 校验位, "none"或"hardware"。默认值为"none"。
    /// </summary>
    [DisplayName("校验")]
    public WebSerialParityType? FlowControlType { get; set; } = WebSerialParityType.none;

    [DisplayName("校验")]
    public string? FlowControl { get => FlowControlType.ToString(); }

    /// <summary>
    /// HEX发送
    /// </summary>
    [DisplayName("HEX发送")]
    public bool InputWithHex { get; set; }

    /// <summary>
    /// HEX接收
    /// </summary>
    [DisplayName("HEX接收")]
    public bool OutputInHex { get; set; }

    /// <summary>
    /// 自动连接设备
    /// </summary>
    [DisplayName("自动连接设备")]
    public bool AutoConnect { get; set; } = true;

    /// <summary>
    /// 自动断帧方式
    /// </summary>
    [JsonIgnore]
    [DisplayName("自动断帧方式")]
    public AutoFrameBreakType AutoFrameBreakType { get; set; } = AutoFrameBreakType.Character;

    [DisplayName("自动断帧方式")]
    public string? autoFrameBreak { get => AutoFrameBreakType.ToString(); }

    /// <summary>
    /// 断帧字符(默认\n)
    /// </summary>
    [DisplayName("断帧字符(默认\\n)")]
    public string? FrameBreakChar { get; set; }

    /// <summary>
    /// 中断 (未完成)
    /// </summary>
    [DisplayName("中断(未完成)")] 
    public bool Break { get; set; }

    /// <summary>
    /// 数据终端就绪 (未完成)
    /// </summary>
    [JsonPropertyName("DTR")]
    [DisplayName("DTR(未完成)")]
    public bool DTR { get; set; }

    /// <summary>
    /// 请求发送 (未完成)
    /// </summary>
    [JsonPropertyName("RTS")]
    [DisplayName("RTS(未完成)")]
    public bool RTS { get; set; } 

    /// <summary>
    /// 连接按钮文本/Connect button title
    /// </summary>
    [DisplayName("连接按钮文本")]
    public string? ConnectBtnTitle { get; set; } = "连接";

    /// <summary>
    /// 断开连接按钮文本/Connect button title
    /// </summary>
    [DisplayName("断开连接按钮文本")]
    public string? DisconnectBtnTitle { get; set; } = "断开连接";

    /// <summary>
    /// 写入按钮文本/Write button title
    /// </summary>
    [DisplayName("写入按钮文本")]
    public string? WriteBtnTitle { get; set; } = "写入";

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

/// <summary>
/// 自动断帧方式
/// </summary>
public enum AutoFrameBreakType
{
    /// <summary>
    /// 未启用自动断帧
    /// </summary>
    [Description("未启用")]
    none,

    /// <summary>
    /// 字符断帧
    /// </summary>
    [Description("字符断帧")]
    Character,

    /// <summary>
    /// 空闲中断 (未完成)
    /// </summary>
    [Description("空闲中断(未完成)")]
    Timeout,

    /// <summary>
    /// 帧头、帧尾 (未实现)
    /// <para></para>例如: 帧头（AA 、BB） + 数据长度 + 数据  + CRC校验 + 帧尾（CC、DD）
    /// </summary>
    [Description("帧头帧尾(未实现)")]
    FrameTail,

    /// <summary>
    /// 字符间隔 (未实现)
    /// </summary>
    [Description("字符间隔(未实现)")]
    CharacterInterval,
}
