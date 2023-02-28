// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;
using UAParser;

namespace BootstrapBlazor.Components;

/// <summary>
/// WebApi 组件基类
/// </summary>
public partial class WebApi : IAsyncDisposable
{
    [Inject] IJSRuntime? JS { get; set; }
    private IJSObjectReference? module;

    private DotNetObjectReference<WebApi>? Instance { get; set; }
    private UAInfo? ClientInfo { get; set; }

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    /// <summary>
    /// 获得/设置 电池信息回调方法
    /// </summary>
    [Parameter]
    public Func<BatteryStatus, Task>? OnBatteryResult { get; set; }

    /// <summary>
    /// 获得/设置 浏览器信息回调方法
    /// </summary>
    [Parameter]
    public Func<UAInfo, Task>? OnUserAgentResult { get; set; }

    /// <summary>
    /// 显示调试信息
    /// </summary>
    [Parameter]
    public bool ShowInfo { get; set; }

    string? UserAgents { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.WebAPI/app.js" + "?v=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                Instance = DotNetObjectReference.Create(this);
                if (OnBatteryResult != null) await GetBattery();
                if (OnNetworkInfoResult != null) await GetNetworkInfo();
                if (OnUserAgentResult != null)
                {
                    var userAgent = await module!.InvokeAsync<string>("getUserAgent");
                    var parser = Parser.GetDefault();
                    var clientInfo = parser.Parse(userAgent);
                    ClientInfo = new UAInfo(clientInfo.String, clientInfo.OS, clientInfo.Device, clientInfo.UA);
                    await OnUserAgentResult.Invoke(ClientInfo);
                    if (ShowInfo)
                    {
                        Console.WriteLine(userAgent);
                        UserAgents = userAgent;
                        StateHasChanged();
                    }
                }
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }

    /// <summary>
    /// 获取电量
    /// </summary>
    public virtual async Task GetBattery()
    {
        try
        {
            await module!.InvokeVoidAsync("GetBattery", Instance);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 获取电池信息完成回调方法
    /// </summary>
    /// <param name="batteryStatus"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetBatteryResult(BatteryStatus batteryStatus)
    {
        try
        {
            if (OnBatteryResult != null) await OnBatteryResult.Invoke(batteryStatus);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }


    /// <summary>
    /// 获取网络信息
    /// </summary>
    public virtual async Task GetNetworkInfo()
    {
        try
        {
            await module!.InvokeVoidAsync("GetNetworkInfo", Instance);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 获得/设置 网络信息回调方法
    /// </summary>
    [Parameter]
    public Func<NetworkInfoStatus, Task>? OnNetworkInfoResult { get; set; }

    /// <summary>
    /// 获取网络信息完成回调方法
    /// </summary>
    /// <param name="networkInfoStatus"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetNetworkInfoResult(NetworkInfoStatus networkInfoStatus)
    {
        try
        {
            if (OnNetworkInfoResult != null) await OnNetworkInfoResult.Invoke(networkInfoStatus);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }


}
