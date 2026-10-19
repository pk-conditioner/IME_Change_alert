using System.Runtime.InteropServices;

namespace IME_Change_alert;

static class ImeStatus
{
    //部品への問い合わせ用構造体(GetGUIThreadINfoが書き込みに使用)
    [StructLayout(LayoutKind.Sequential)]
    struct RECT { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    struct GUITHREADINFO
    {
        public int cbSize, flags;
        public IntPtr hwndActive, hwndFocus, hwndCapture, hwndMenuOwner, hwndMoveSize, hwndCaret;
        public RECT rcCaret;
    }

    [DllImport("user32.dll")]
    static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO info);

    //手前のウィンドウ画面の番号を取得
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    //ウィンドウのIME用の番号を取得
    [DllImport("imm32.dll")]
    static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

    //ウィンドウに通知(メッセージを送り返信待つ)
    [DllImport("user32.dll")]
    static extern IntPtr SendMessageTimeoutW(IntPtr hWnd, uint msg, UIntPtr wParam,
        IntPtr lParam, uint flags, uint timeout, out UIntPtr result);

    //定数
    const uint WM_IME_CONTROL = 0x0283;
    const uint IMC_GETOPENSTATUS = 0x0005;
    const uint SMTO_ABORTIFHUNG = 0x0002;

    //IMEオンオフ確認(オンならtrue,オフならfalse,取得不可ならnull)
    public static bool? IsOpen(){
        var info = new GUITHREADINFO { cbSize = Marshal.SizeOf<GUITHREADINFO>() };

        //フォーカスがある部品に問い合わせ(win11メモ帳対応)
        IntPtr hwnd = (GetGUIThreadInfo(0, ref info) && info.hwndFocus != IntPtr.Zero)
            ? info.hwndFocus               // 入力中の部品
            : GetForegroundWindow();       // 取得できなければ

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