using System.Runtime.InteropServices;

namespace IME_Change_alert;

static class ImeStatus
{
    //手前のウィンドウ画面の番号を取得
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    //ウィンドウ内のIMEの番号(入力モードの種類)を取得
    [DllImport("imm32.dll")]
    static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

    //ウィンドウに通知
    [DllImport("user32.dll")]
    static extern IntPtr SendMessageTimeoutW(IntPtr hWnd, uint msg, UIntPtr wParam,
        IntPtr lParam, uint flags, uint timeout, out UIntPtr result);

    //定数
    const uint WM_IME_CONTROL = 0x0283;
    const uint IMC_GETOPENSTATUS = 0x0005;
    const uint SMTO_ABORTIFHUNG = 0x0002;

    //IMEオンオフ確認(オンならtrue,オフならfalse,取得不可ならnull)
    public static bool? IsOpen(){
        IntPtr hwnd = GetForegroundWindow();
        IntPtr ime = ImmGetDefaultIMEWnd(hwnd);
        if(ime == IntPtr.Zero){
            return null;
        }
        IntPtr ok = SendMessageTimeoutW(ime, WM_IME_CONTROL,(UIntPtr)IMC_GETOPENSTATUS,IntPtr.Zero,SMTO_ABORTIFHUNG,100, out UIntPtr result);
        if(ok == IntPtr.Zero){
            return null;
        }
        return result != UIntPtr.Zero;

    }

}