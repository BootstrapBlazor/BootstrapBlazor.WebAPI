# Blazor WebAPI 组件 (浏览器信息/电池信息/网络信息/截屏/录像/上传/Cookie/LocalStorage/WebSerial/语音识别和合成)

1. 电池信息类
2. 网络信息类
3. 浏览器信息类
4. 截屏录像类
5. 上传
6. Cookie/LocalStorage

示例:

https://blazor.app1.es/WebAPI

https://blazor.app1.es/screencapture

https://blazor.app1.es/Storages

https://blazor.app1.es/UploadToBase64s

https://blazor.app1.es/WebSerials

https://blazor.app1.es/Speechs

使用方法:

1.nuget包

```BootstrapBlazor.WebAPI```

2._Imports.razor 文件 或者页面添加 添加组件库引用

```@using BootstrapBlazor.Components```


3.razor页面
```
<WebApi OnBatteryResult="@OnBatteryResult" OnNetworkInfoResult="@OnNetworkInfoResult" OnUserAgentResult="OnUserAgentResult" OnError="@OnError" ShowInfo />

<Capture OnCaptureResult="@OnCaptureResult" OnError="@OnError" Camera="false" />


```
```
@code{

    private Task OnBatteryResult(BatteryStatus item)
    {
        xx = item;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnNetworkInfoResult(NetworkInfoStatus item)
    {
        xx = item;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnUserAgentResult(UAInfo item)
    {
        xx = item;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnError(string message)
    {
        this.message = message;
        StateHasChanged();
        return Task.CompletedTask;
    }

    //OCR库
    private OCR? OCR { get; set; } 

    private async Task OnCaptureResult(Stream item)
    {
        if (OCR!=null) await OCR.OCRFromStream(item);
        StateHasChanged();
    }

} 
```
----
#### 更新历史

v7.5.2
- UploadToBase64 添加外层用户自定义样式

v7.5.1
- Capture 添加 1.保存最后使用设备ID下次自动调用, 2.指定摄像头设备ID

v7.5.0
- WebVoice 优化

v7.4.5
- 添加查询语音合成发音状态

v7.4.2
- WebSerial 自动检查状态

v7.4.2
- WebSerial 添加信号数据处理

v7.4.0
- WebSpeech 语音识别优化

v7.3.6
- WebSerial 添加 自动连接设备/自动断帧方式/Break/DTR/RTS/按钮文本自定义

v7.3.5
- WebSerial 添加 连接状态/HEX发送/HEX接收/自动断帧/断帧字符

v7.2.7 - 7.3.4
- 添加 WebSpeech 语音识别/合成组件

v7.2.4
- 添加 串口读写 WebSerial组件

v7.2.2
- Capture 组件添加指定高宽,可保存高质量图片

v7.2
- 添加 Cookie / LocalStorage 服务
- 添加 上传图片 UploadToBase64 组件
- Capture 组件优化

---
#### Blazor 组件

[条码扫描 ZXingBlazor](https://www.nuget.org/packages/ZXingBlazor#readme-body-tab)
[![nuget](https://img.shields.io/nuget/v/ZXingBlazor.svg?style=flat-square)](https://www.nuget.org/packages/ZXingBlazor) 
[![stats](https://img.shields.io/nuget/dt/ZXingBlazor.svg?style=flat-square)](https://www.nuget.org/stats/packages/ZXingBlazor?groupby=Version)

[图片浏览器 Viewer](https://www.nuget.org/packages/BootstrapBlazor.Viewer#readme-body-tab)
  
[条码扫描 BarcodeScanner](Densen.Component.Blazor/BarcodeScanner.md)
   
[手写签名 Handwritten](Densen.Component.Blazor/Handwritten.md)

[手写签名 SignaturePad](https://www.nuget.org/packages/BootstrapBlazor.SignaturePad#readme-body-tab)

[定位/持续定位 Geolocation](https://www.nuget.org/packages/BootstrapBlazor.Geolocation#readme-body-tab)

[屏幕键盘 OnScreenKeyboard](https://www.nuget.org/packages/BootstrapBlazor.OnScreenKeyboard#readme-body-tab)

[百度地图 BaiduMap](https://www.nuget.org/packages/BootstrapBlazor.BaiduMap#readme-body-tab)

[谷歌地图 GoogleMap](https://www.nuget.org/packages/BootstrapBlazor.Maps#readme-body-tab)

[蓝牙和打印 Bluetooth](https://www.nuget.org/packages/BootstrapBlazor.Bluetooth#readme-body-tab)

[PDF阅读器 PdfReader](https://www.nuget.org/packages/BootstrapBlazor.PdfReader#readme-body-tab)

[文件系统访问 FileSystem](https://www.nuget.org/packages/BootstrapBlazor.FileSystem#readme-body-tab)

[光学字符识别 OCR](https://www.nuget.org/packages/BootstrapBlazor.OCR#readme-body-tab)

[电池信息/网络信息 WebAPI](https://www.nuget.org/packages/BootstrapBlazor.WebAPI#readme-body-tab)

#### AlexChow

[今日头条](https://www.toutiao.com/c/user/token/MS4wLjABAAAAGMBzlmgJx0rytwH08AEEY8F0wIVXB2soJXXdUP3ohAE/?) | [博客园](https://www.cnblogs.com/densen2014) | [知乎](https://www.zhihu.com/people/alex-chow-54) | [Gitee](https://gitee.com/densen2014) | [GitHub](https://github.com/densen2014)


![ChuanglinZhou](https://user-images.githubusercontent.com/8428709/205942253-8ff5f9ca-a033-4707-9c36-b8c9950e50d6.png)
