#Controller管理

require_relative 'ime_status'
require 'yaml'

#0.1秒ごとにポーリング(IMEの状態を確認して検知)する
prev = nil
loop do
    now = ime_open?
    if now != prev
        puts(now ? "あ" : "A")
        prev = now
    end
    sleep 0.1 #カウント秒数
end



