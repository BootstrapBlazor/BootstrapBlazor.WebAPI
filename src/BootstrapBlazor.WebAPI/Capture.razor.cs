// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace BootstrapBlazor.Components;

/// <summary>
/// Capture 组件基类
/// </summary>
public partial class Capture : IAsyncDisposable
{
    [Inject] IJSRuntime? JS { get; set; }
    private IJSObjectReference? module;
    private DotNetObjectReference<Capture>? Instance { get; set; }

    /// <summary>
    /// UI界面元素的引用对象
    /// </summary>
    public ElementReference Element { get; set; }

    private CaptureOptions? Options { get; set; } = new CaptureOptions();

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    /// <summary>
    /// 获得/设置 截屏回调方法
    /// </summary>
    [Parameter]
    public Func<Stream, Task>? OnCaptureResult { get; set; }

    /// <summary>
    /// 获得/设置 截屏base64回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnCapture { get; set; }

    /// <summary>
    /// 截图按钮文本/Capture button title
    /// </summary>
    [Parameter]
    [NotNull]
    public string? CaptureBtnTitle { get; set; }

    /// <summary>
    /// 获得/设置 持续获取截图
    /// </summary>
    [Parameter]
    public bool Continuous { get; set; }

    /// <summary>
    /// 使用摄像头,否则使用屏幕. 默认为 true
    /// </summary>
    [Parameter]
    public bool Camera { get; set; } = true;

    /// <summary>
    /// 获得/设置 显示内置UI
    /// </summary>
    [Parameter]
    public bool ShowUI { get; set; } = true;

    /// <summary>
    /// 获得/设置 显示结果
    /// </summary>
    [Parameter]
    public bool ShowResult { get; set; } = true;

    /// <summary>
    /// 获得/设置 显示log
    /// </summary>
    [Parameter]
    public bool Debug { get; set; }

    [Parameter]
    public bool Auto { get; set; } = true;

    /// <summary>
    /// 图像质量,默认为 0.8
    /// </summary>
    [Parameter]
    public double Quality { get; set; } = 0.8d;

    /// <summary>
    /// 选择设备按钮文本/Select device button title
    /// </summary>
    [Parameter]
    public string SelectDeviceBtnTitle { get; set; } = "选择设备";

    protected override void OnInitialized()
    {
        CaptureBtnTitle = CaptureBtnTitle ?? (Camera ? "拍照" : "截屏");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.WebAPI/app.js" + "?v=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                Instance = DotNetObjectReference.Create(this);
                if (Auto)
                    await Start();
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    public async Task Dispose()
    {
        await module!.InvokeVoidAsync("Capture", Instance, Element, Options, "Destroy");
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await module!.InvokeVoidAsync("Capture", Instance, Element, Options, "Destroy");
        Instance?.Dispose();
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }

    /// <summary>
    /// 截屏
    /// </summary>
    public virtual async Task Start() => await Start(null, null, null);


    /// <summary>
    /// 截屏
    /// </summary>
    public virtual async Task Start(bool? continuous, bool? camera, bool? debug)
    {
        try
        {
            Options = Options ?? new CaptureOptions();
            Options.Continuous = continuous ?? Continuous;
            Options.Camera = camera ?? Camera;
            Options.Debug = debug ?? Debug;
            Options.Quality = Quality;
            await module!.InvokeVoidAsync("Capture", Instance, Element, Options, "Start");
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 截屏完成回调方法
    /// </summary>
    /// <param name="base64encodedstring"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetCaptureResult(string base64encodedstring)
    {
        try
        {
            if (OnCaptureResult != null)
            {
                await OnCaptureResult.Invoke(DataUrl2Stream(base64encodedstring));
            }
            if (OnCapture != null)
            {
                await OnCapture.Invoke(base64encodedstring);
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 从 DataUrl 转换为 Stream
    /// <para>Convert from a DataUrl to an Stream</para>
    /// </summary>
    /// <param name="base64encodedstring"></param>
    /// <returns></returns>
    public static Stream DataUrl2Stream(string base64encodedstring)
    {
        var base64Data = Regex.Match(base64encodedstring, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
        var bytes = Convert.FromBase64String(base64Data);
        var stream = new MemoryStream(bytes);
        return stream;
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
