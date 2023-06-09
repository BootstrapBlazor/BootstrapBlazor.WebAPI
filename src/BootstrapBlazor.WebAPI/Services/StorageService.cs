using Microsoft.JSInterop;

namespace BootstrapBlazor.WebAPI.Services;

public interface IStorage
{
    public Task RemoveValue(string key);
    public Task SetValue<TValue>(string key, TValue value) where TValue : class;
    public Task<TValue?> GetValue<TValue>(string key, TValue? def = null) where TValue : class;
}

public class StorageService : IStorage
{
    readonly IJSRuntime JSRuntime; 

    public StorageService(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime; 
    }

    public async Task SetValue<TValue>(string key, TValue value) where TValue : class
    {
        await JSRuntime.InvokeVoidAsync("eval", $"localStorage.setItem('{key}', '{value}')");
    }

    public async Task<TValue?> GetValue<TValue>(string key, TValue? def) where TValue : class
    {
        var cValue = await JSRuntime.InvokeAsync<TValue>("eval", $"localStorage.getItem('{key}');"); 
        return cValue??def;
    }
    public async Task RemoveValue(string key)
    {
         await JSRuntime.InvokeVoidAsync("eval", $"localStorage.removeItem('{key}')");
    }
     

}



