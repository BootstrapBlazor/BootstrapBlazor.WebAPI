using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BootstrapBlazor.Components;

/// <summary>
/// 
/// </summary>
public partial class WebSpeech : IAsyncDisposable
{
    [Inject] IJSRuntime? JS { get; set; }
    private IJSObjectReference? module;
    private DotNetObjectReference<WebSpeech>? Instance { get; set; }

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
    /// 获得/设置 识别完成回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnResult { get; set; }

    /// <summary>
    /// 获得/设置 状态信息回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnStatus { get; set; }

    /// <summary>
    /// 显示语音识别调试信息
    /// </summary>
    [Parameter]
    public bool ShowSpeechInfo { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.WebAPI/WebSpeech.razor.js" + "?v=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                Instance = DotNetObjectReference.Create(this);
                await InitWebapi();
            } 
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    [JSInvokable]
    public async Task GetStatus(string val)
    {
        if (OnStatus != null) await OnStatus.Invoke(val);
    }

    [JSInvokable]
    public async Task GetResult(string val)
    {
        if (OnResult != null) await OnResult.Invoke(val);
    }

    /// <summary>
    /// 初始化语音
    /// </summary>
    public virtual async Task InitWebapi()
    {
        try
        {
            await module!.InvokeVoidAsync("InitWebapi", Instance, Element);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 语音识别
    /// </summary>
    /// <returns></returns>
    public virtual async Task<string> SpeechRecognition(string lang = "zh-CN")
    {
        try
        {
            return await module!.InvokeAsync<string>("SpeechRecognition", Instance, lang);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
            return e.Message;
        }
    }

    /// <summary>
    /// 语音合成（文字转语音）
    /// </summary>
    /// <param name="text">文字</param>
    /// <param name="lang">语言</param>
    /// <param name="rate">速率, 范围可以在 0.1（最低）和 10（最高）之间</param>
    /// <param name="picth">音高,范围可以在 0（最低）和 2（最高）之间</param>
    /// <param name="volume">音量, 浮点数，介于 0（最低）和 1（最高）之间</param>
    /// <param name="voiceURI">语音引擎名称</param>
    /// <returns></returns>
    public virtual async Task SpeechSynthesis(string text, string lang = "zh-CN", double rate = 1, double picth = 1, double volume = 1, string? voiceURI = null)
    {
        try
        {
            await module!.InvokeVoidAsync("SpeechSynthesis", Instance, text, lang, rate, picth, volume, voiceURI);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    public virtual async Task SpeechRecognitionStop()
    {
        try
        {
            var res = await module!.InvokeAsync<bool>("SpeechRecognitionStop", Instance);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    public virtual async Task SpeechStop()
    {
        try
        {
            var res = await module!.InvokeAsync<bool>("SpeechStop", Instance);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    public virtual async Task<List<WebVoice>?> GetVoiceList(bool orderByName = false)
    {
        try
        {
            List<WebVoice> res = await module!.InvokeAsync<List<WebVoice>>("GetVoiceList");
            var retry = 0;
            while (res == null || res.Count == 0)
            {
                await Task.Delay(200);
                res = await module!.InvokeAsync<List<WebVoice>>("GetVoiceList", Instance);
                retry++;
                if (retry == 5)
                {
                    return null;
                }
            }
            try
            {
                return orderByName ? (res?.OrderByDescending(a => a.LocalService).ThenBy(a => a.Lang).ThenBy(a => a.Name).ToList()) : res;
            }
            catch (Exception)
            { 
                return res;
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke("GetVoiceList:" + e.Message);
        }
        return null;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        Instance?.Dispose();
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }

    

}
