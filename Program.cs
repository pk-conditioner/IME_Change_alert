namespace IME_Change_alert;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Thread.Sleep(3000); //IME切替確認用(この時間内にIMEの動作確認をする)
        Console.WriteLine(ImeStatus.IsOpen());
        var popup = new PopupForm();
        popup.ShowPopup("あ");
        Application.Run();
        
    }    
}