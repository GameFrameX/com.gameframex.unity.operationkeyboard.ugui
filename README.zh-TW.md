<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (UGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（UGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（UGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（UGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(UGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 目錄導航

- [項目簡介](#項目簡介)
- [快速開始](#快速開始)
- [使用範例](#使用範例)
- [文檔與資源](#文檔與資源)
- [社區與支援](#社區與支援)
- [開源協議](#開源協議)

---

## 項目簡介

GameFrameX OperationKeyboard（UGUI 綁定）是鍵盤家族的 UI 綁定包：通過 `IOperationKeyboardField` 實現把 Unity `InputField` 與 `TMP_InputField` 綁定到核心 `OperationKeyboard` 門面，並自帶運行時自動發現選中輸入框的自動掛載器——零面板改動實現小遊戲鍵盤自動適配。

### 核心特性

- **雙綁定器** - `UGUIInputFieldKeyboardBinder`（`InputField`）與 `TMPInputFieldKeyboardBinder`（`TMP_InputField`）同為一等公民；聚焦建立鍵盤會話，失焦解綁
- **自舉自動掛載器** - `DontDestroyOnLoad` 自舉低頻輪詢 `EventSystem.currentSelectedGameObject`，發現輸入框即綁定（無需逐面板配置）
- **原生事件流** - input 事件賦值 `text`（`onValueChanged` 照常觸發）並置尾光標；confirm / complete 終值落定並觸發 `EndEdit`（與原生移動端一致）；失焦解綁且不重複觸發 `EndEdit`
- **工具集探測** - `versionDefines` 探測 `com.unity.ugui` / `com.unity.textmeshpro`；各綁定器僅在對應工具集存在時編譯（發現即註冊）

### 系統需求

- Unity 2019.4 或更高版本
- `com.unity.ugui`（legacy `InputField` 綁定器）和 / 或 `com.unity.textmeshpro`（`TMP_InputField` 綁定器）

---

## 快速開始

### 安裝

選擇以下任一方式：

1. 編輯 Unity 項目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
   ```json
   {
     "scopedRegistries": [
       {
         "name": "GameFrameX",
         "url": "https://gameframex.upm.alianblank.uk",
         "scopes": [
           "com.gameframex"
         ]
       }
     ],
     "dependencies": {
       "com.gameframex.unity.operationkeyboard.ugui": "1.0.0"
     }
   }
   ```

   `scopes` 控制哪些包通過此註冊表解析。只有以 `com.gameframex` 開頭的包才會從這個註冊表獲取。

2. 直接在 `manifest.json` 的 `dependencies` 節點下添加以下內容：
   ```json
   {
      "com.gameframex.unity.operationkeyboard.ugui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加庫，地址為： `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git`
4. 直接下載倉庫放置到 Unity 項目的 `Packages` 目錄下，會自動加載識別。

## 使用範例

### 自動適配（零面板改動）

本包與平台渠道包（如 `com.gameframex.unity.operationkeyboard.wechatminigame`）及小遊戲 SDK 一起安裝。WebGL 小遊戲構建下，點按 `InputField` / `TMP_InputField` 即拉起平台鍵盤；任何面板都無需添加組件。

```csharp
// 遊戲代碼無需任何改動——自動掛載器負責發現與綁定。
// 業務回調照常工作：
inputField.onValueChanged.AddListener(value => { /* 鍵盤輸入時觸發 */ });
inputField.onEndEdit.AddListener(value => { /* confirm / complete / 失焦時觸發 */ });
```

### 手動綁定（兜底）

顯式掛載保留可用（逐字段適配器工程的遷移路徑）：

```csharp
// legacy InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.UGUIInputFieldKeyboardBinder>();
// TMP_InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.TMPInputFieldKeyboardBinder>();
```

### 事件流映射

| 平台鍵盤事件 | 字段行為 |
|---|---|
| input | `text = value`（`onValueChanged` 觸發）+ 光標置尾 |
| confirm / complete | 終值落定、鍵盤收起、觸發 `onEndEdit` |
| 失焦 / 窗口關閉 | 鍵盤收起、會話解綁（不重複觸發 `EndEdit`） |

### 主要類

- **`UGUIInputFieldKeyboardBinder`** / **`TMPInputFieldKeyboardBinder`**：`IOperationKeyboardField` 實現；聚焦 Attach、失焦 Detach、再點按重新 Attach。
- **`OperationKeyboardUGUIAutoBinder`**：自舉自動掛載器；`DontDestroyOnLoad` + 低頻 `EventSystem` 輪詢。
- **`GameFrameXOperationKeyboardUGUICroppingHelper`**：構建裁剪優化用的防裁剪助手。

### 注意事項

- 未註冊渠道包（Editor / 原生平台）時門面 no-op：不攔截任何事件，原生 `TouchScreenKeyboard` 路徑完整保留。
- `maxLength` 由核心恆傳 -1（小遊戲鍵盤按拼音組合串計數，硬上限會掐斷中文輸入）。

---

## 文檔與資源

- **完整文檔**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **問題反饋**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/issues)

---

## 社區與支援

- **QQ 討論群**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **功能建議**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/discussions)

---

## 依賴
| 包 | 說明 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard 核心包 |
| com.gameframex.unity | GameFrameX 核心 |
| com.unity.ugui | 經 versionDefines 探測 |
| com.unity.textmeshpro | 經 versionDefines 探測 |

## 更新日誌

查看 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases) 了解更新日誌。

## 開源協議

詳見 [LICENSE.md](LICENSE.md) 文件。
