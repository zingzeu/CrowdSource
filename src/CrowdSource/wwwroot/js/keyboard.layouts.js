﻿/* VIRTUAL KEYBOARD DEMO - https://github.com/Mottie/Keyboard */
$(function() {

    jQuery.keyboard.layouts['buc'] = {
      'name': 'BUC',
      'normal': [
        "a ă à ā á â",
        "a̤ ă̤ à̤ ā̤ á̤ â̤",
        "e ĕ è ē é ê",
        "e̤ ĕ̤ è̤ ē̤ é̤ ê̤",
        "i ĭ ì ī {empty} {empty}",
        "o ŏ ò ō ó ô",
        "o̤ ŏ̤ ò̤ ō̤ ó̤ ô̤",
        "u ŭ ù ū {empty} {empty}",
        "ṳ ṳ̆ ṳ̀ ṳ̄ {empty} û"
      ]
    };

    jQuery.keyboard.layouts['bopomofo'] = {
      'name': 'BoPoMoFo',
      'normal': [ 
        "ㄅ ㄉ ˇ ˋ ㄓ ˊ ˙ ㄚ ㄞ ㄢ",
        "ㄆ ㄊ ㄍ ㄐ ㄔ ㄗ ㄧ ㄛ ㄟ ㄣ",
        "ㄇ ㄋ ㄎ ㄑ ㄕ ㄘ ㄨ ㄜ ㄠ ㄤ",
        "ㄈ ㄌ ㄏ ㄒ ㄖ ㄙ ㄩ ㄝ ㄡ ㄥ",
        "{sp:6} ㄦ"]
    };

    jQuery.keyboard.layouts['radicals'] = {
      'name': 'Chinese Radicals',
      'normal': [ 
        "⺄ 乛 冖 宀 ⺮ 亠 ⺌ 艹",
        "冂 凵 勹 匚 彐 疒 辶 廴",
        "亻 氵 冫 刂 卩 阝 ⺪ 饣",
        "彳 忄 扌 丬 犭 衤 礻 糹",
        "夂 攵 攴 尢 屮 巛 廾 彑",
        "日 曰 彡 殳 爿 糸 艸 豸"]
    };

    jQuery.keyboard.layouts['cdo'] = {
        'name': 'Eastern Min',
        'normal': [
            "〇 𠆧 価 𡅏 仱 𣍐 爿",
            "𠋡 ⿰亻鞋 {empty:5}"
        ]
    };
    
});
