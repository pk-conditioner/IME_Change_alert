namespace IME_Change_alert;

public partial class PopupForm : Form
{
    private Label label;
    private System.Windows.Forms.Timer hideTimer;

    // キーボードフォーカスを奪わない
    protected override bool ShowWithoutActivation => true;

    //ウィンドウの拡張スタイル
    protected override CreateParams CreateParams{
        get{
            const int WS_EX_TOPMOST        =   0x00000008;
            const int WS_EX_TOOLWINDOW     =   0x00000080;
            const int WS_EX_NOACTIVATE     =   0x08000000;
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
            return cp;
        }
    }

    

    //ウィンドウ表示設定
    public PopupForm()
    {
        //表示設定
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Size = new Size(120, 120);
        Opacity = 0.8;
        BackColor = Color.FromArgb(0x33,0x33,0x33);

        //ラベル設定
        label = new Label();
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleCenter;
        label.Font = new Font("Yu Gothic UI", 48,GraphicsUnit.Pixel); //pointだと見切れるのでpixel指定
        label.ForeColor = Color.White;
        Controls.Add(label);

        //タイマー
        hideTimer = new System.Windows.Forms.Timer{Interval=600};
        hideTimer.Tick += (s,e) => { hideTimer.Stop(); Hide(); };


    }
    //出力処理時の内容
    public void ShowPopup(String text){
        label.Text = text;

        //画面中央位置の計算
        Rectangle area = Screen.PrimaryScreen!.WorkingArea;
        Location = new Point(area.Left + (area.Width - Width)/2,area.Top + (area.Height - Height)/2);

        Show();

        //タイマー
        hideTimer.Stop();
        hideTimer.Start();
    }
}       
