namespace IME_Change_alert;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var popup = new PopupForm();
        bool? prev = ImeStatus.IsOpen();

        //0.1秒ごとにポーリング(IMEの状態を確認して検知)する
        var pollTimer = new System.Windows.Forms.Timer { Interval = 100 };
        pollTimer.Tick += (s, e) =>
        {
            bool? now = ImeStatus.IsOpen();
            if (now == null) return;       

            //切替検知したら表示
            if (now != prev)
            {
                popup.ShowPopup(now == true ? "あ" : "A");
                prev = now;
            }
        };

        pollTimer.Start();
        Application.Run();
    }    
}