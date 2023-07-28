using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazor.Components;

/// <summary>
/// 浏览器 WebSerial
/// </summary>
public partial class WebSerial : IAsyncDisposable
{
    [Inject] IJSRuntime? JS { get; set; }
    private IJSObjectReference? module;
    private DotNetObjectReference<WebSerial>? Instance { get; set; }

    /// <summary>
    /// UI界面元素的引用对象
    /// </summary>
    public ElementReference Element { get; set; }

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    /// <summary>
    /// 获得/设置 Log回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnLog { get; set; }

    /// <summary>
    /// 获得/设置 收到数据回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnReceive { get; set; }

    /// <summary>
    /// 连接按钮文本/Connect button title
    /// </summary>
    [Parameter]
    [NotNull]
    public string? ConnectBtnTitle { get; set; }

    /// <summary>
    /// 写入按钮文本/Write button title
    /// </summary>
    [Parameter]
    [NotNull]
    public string? WriteBtnTitle { get; set; }

    /// <summary>
    /// 获得/设置 显示内置UI
    /// </summary>
    [Parameter]
    public bool ShowUI { get; set; } = true;

    /// <summary>
    /// 获得/设置 显示log
    /// </summary>
    [Parameter]
    public bool Debug { get; set; }

    protected override void OnInitialized()
    {
        ConnectBtnTitle = ConnectBtnTitle ?? "连接";
        WriteBtnTitle = WriteBtnTitle ?? "写入";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.WebAPI/webSerial.js" + "?v=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                Instance = DotNetObjectReference.Create(this);
                await module!.InvokeVoidAsync("Init", Instance, Element, "Start");
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        Instance?.Dispose();
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }
    /// <summary>
    /// Start
    /// </summary>
    public virtual async Task Start()
    {
        try
        {
            await module!.InvokeVoidAsync("Init", Instance, Element, "Start");
        }
        catch 
        {
        }
    }

    /// <summary>
    /// 收到数据回调方法
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task ReceiveData(string msg)
    {
        try
        {
            if (OnReceive != null)
            {
                await OnReceive.Invoke(msg);
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Log回调方法
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetLog(string msg)
    {
        try
        {
            if (OnLog != null)
            {
                await OnLog.Invoke(msg);
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 错误回调方法
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetError(string error)
    {
        if (OnError != null) await OnError.Invoke(error);
    }
}
