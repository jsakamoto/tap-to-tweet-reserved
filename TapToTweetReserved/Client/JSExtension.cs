using Microsoft.JSInterop;

namespace TapToTweetReserved.Client;

public static class JSExtension
{
    public static void SetInitialFocus(this IJSRuntime jSRuntime)
    {
        (jSRuntime as IJSInProcessRuntime)?.InvokeVoid("eval", "setTimeout(()=>(document.querySelector('*[autofocus]')||{focus:()=>{}}).focus(), 200)");
    }
}
