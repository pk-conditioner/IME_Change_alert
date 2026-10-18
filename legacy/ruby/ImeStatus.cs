#Model担当

require 'fiddle'
require 'fiddle/import'

#モジュール
module Win
    extend Fiddle::Importer
    dlload 'user32.dll','imm32.dll'
    extern 'void* GetForegroundWindow()'
    extern 'void* ImmGetDefaultIMEWnd(void*)'
    extern 'Intptr_t SendMessageTimeoutW(void*, unsighed int, uintptr_t,intptr_t,unsigned int, unsigned int,void* )'
end

#定数
WH_IME_CONTROL = 0x0283
IMC_GETOPENSTATUS = 0x0005
SMTO_ABORTIFHUNG = 0x0002

#IMEオンオフ確認(オンならtrue,オフならfalse,取得不可ならnil)
def ime_open?
    hwnd = Win.GetForegroundWindow()
    ime = Win.ImmGetDefaultIMEWnd(hwnd)
    return nil if ime.null?

    buf = Fiddle::Pointer.malloc(Fiddle::SIZEOF_VOIDP, Fiddle::RUBY_FREE)
    ok = Win.SendMessageTimeoutW(ime, WH_IME_CONTROL,IMC_GETOPENSTATUS,0,SMTO_ABORTIFHUNG,100,buf)
    return nil if ok == 0
    buf[0,Fiddle::SIZEOF_VOIDP].unpack1('J') != 0
end
p ime_open? #オンオフ結果を出力