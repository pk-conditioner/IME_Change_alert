# IME Change Alert

IME(日本語入力)のON/OFFが切り替わった瞬間に、画面中央へ「あ / A」を一瞬ポップアップ表示する Windows 常駐ツールです。

## 作った理由
 入力モードの切り替えに気づかず打ち間違えることが多々あったので、その改善と学習を兼ねて 

## 機能
| 機能 | 状態 |
|---|---|
| 入力中のウィンドウの IME の ON/OFF を取得(Windows 11 のメモ帳にも対応) | 実装済み |
| 枠なし・最前面・半透明のポップアップを表示(フォーカスを奪わない) | 実装済み |
| IME の ON/OFF を 100ms ごとに監視し、切り替わりを検知 | 実装済み |
| 表示時間・大きさ・色・透明度を `appsettings.json` で変更可能 | 予定 |
| タスクトレイ常駐、右クリックメニューから終了 | 予定 |

## 動かし方
### 必要環境
- Windows 11
- .NET 9 SDK

### 実行
```
git clone [<リポジトリURL>](https://github.com/pk-conditioner/IME_Change_alert.git)
cd IME_Change_alert
dotnet run
```

## 設定(appsettings.json)
| 項目 | 意味 | 例 |
|---|---|---|
| FontSize | 文字サイズ | 48 |
| Width / Height | ポップアップの大きさ | 120 |
| Opacity | 透明度 | 0.8 |
| Duration | 表示秒数 | 0.6 |
| BgColor / TextColor | 背景色 / 文字色 | "#333333" / "#FFFFFF" |
<!-- 実際のキー名は実装に合わせて直す -->

## 仕組み
前面ウィンドウ → `ImmGetDefaultIMEWnd` で IME ウィンドウを取得 → `SendMessageTimeoutW(WM_IME_CONTROL, IMC_GETOPENSTATUS)` で ON/OFF を問い合わせています。

| ファイル | 役割 |
|---|---|
| `ImeStatus.cs` | IME状態の取得(P/Invoke) |
| `PopupForm.cs` | ポップアップ表示 |
| `Program.cs` | タイマーで監視、トレイアイコン、起動 |
| `appsettings.json` | 見た目の設定 |

## 技術選定の経緯
- **Python (ctypes)**: 最初に IME 状態の取得部分を試作
- **Ruby (Fiddle)**: 状態取得・ポーリングまで実装(追記:動作確認したところ不具合有り、)。ポップアップ用の GUI ライブラリ(tk / gtk3)が Windows で導入できず断念。コードは `legacy/ruby/` に残しています
- **C++ (Win32 API)**: 窓の登録やメッセージループを自前で書く必要があり、規模に対して重いため見送り
- **C# (WinForms)**: 枠なし・最前面・半透明・フォーカスを奪わない窓・タイマーが標準で揃っており採用

## 工夫した点
- **入力中のアプリからフォーカスを奪わない**
  `ShowWithoutActivation` を上書きし、さらに窓の拡張スタイルに `WS_EX_NOACTIVATE` を指定。ポップアップが出ても、打っている途中の文字入力が途切れない
- **最前面の指定は窓を作る時点で行う**
  `TopMost = true` を後から設定すると、そのタイミングで窓が前面に出てフォーカスを奪うことがあるため、`CreateParams` で `WS_EX_TOPMOST` を指定
- **タスクバーや Alt+Tab に出さない**
  `ShowInTaskbar = false` と `WS_EX_TOOLWINDOW` を指定
- **画面の拡大率が変わっても文字が見切れない**
  フォントの大きさを `GraphicsUnit.Pixel` で指定し、窓の大きさ(ピクセル)と単位をそろえた
- **相手のアプリが固まっていても巻き込まれない**
  IME への問い合わせに `SendMessageTimeoutW` + `SMTO_ABORTIFHUNG` を使い、応答がなければ 100ms で諦める
- **連続で切り替えても表示が途切れない**
  表示中に再度呼ばれたら、消すまでのタイマーを最初から数え直す
- **Windows 11 のメモ帳に対応**
  新しいメモ帳は、外側の窓(`GetForegroundWindow` の結果)に問い合わせても IME の状態が返らない(既知の問題)。診断用のスクリプトで外側の窓と入力欄(`RichEditD2DPT`)の値を比べて原因を特定し、`GetGUIThreadInfo` で入力中の部品を取得してそこに問い合わせるようにした。取得できないときは従来どおり外側の窓に問い合わせる

## 今後の予定
- ウィンドウをクリックで切り替えただけのときは表示しない(IME を自分で切り替えたときだけ表示する)
- 見た目の設定を `appsettings.json` に出す
- タスクトレイ常駐と終了メニュー
- あ / カ / A の出し分け(`IMC_GETCONVERSIONMODE`)
- PC 起動時の自動起動、exe 1つにまとめる(`dotnet publish`)

## 開発について
実装は自分で行い、設計の相談相手として AI(Claude)を利用しました。