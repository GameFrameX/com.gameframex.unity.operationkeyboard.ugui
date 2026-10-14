<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (UGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（UGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（UGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（UGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(UGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 目录导航

- [项目简介](#项目简介)
- [快速开始](#快速开始)
- [使用示例](#使用示例)
- [文档与资源](#文档与资源)
- [社区与支持](#社区与支持)
- [开源协议](#开源协议)

---

## 项目简介

GameFrameX OperationKeyboard（UGUI 绑定）是键盘家族的 UI 绑定包：通过 `IOperationKeyboardField` 实现把 Unity `InputField` 与 `TMP_InputField` 绑定到核心 `OperationKeyboard` 门面，并自带运行时自动发现选中输入框的自动挂载器——零面板改动实现小游戏键盘自动适配。

### 核心特性

- **双绑定器** - `UGUIInputFieldKeyboardBinder`（`InputField`）与 `TMPInputFieldKeyboardBinder`（`TMP_InputField`）同为一等公民；聚焦建立键盘会话，失焦解绑
- **自举自动挂载器** - `DontDestroyOnLoad` 自举低频轮询 `EventSystem.currentSelectedGameObject`，发现输入框即绑定（无需逐面板配置）
- **原生事件流** - input 事件赋值 `text`（`onValueChanged` 照常触发）并置尾光标；confirm / complete 终值落定并触发 `EndEdit`（与原生移动端一致）；失焦解绑且不重复触发 `EndEdit`
- **工具集探测** - `versionDefines` 探测 `com.unity.ugui` / `com.unity.textmeshpro`；各绑定器仅在对应工具集存在时编译（发现即注册）

### 系统要求

- Unity 2019.4 或更高版本
- `com.unity.ugui`（legacy `InputField` 绑定器）和 / 或 `com.unity.textmeshpro`（`TMP_InputField` 绑定器）

---

## 快速开始

### 安装

选择以下任一方式：

1. 编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
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

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

2. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {
      "com.gameframex.unity.operationkeyboard.ugui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为： `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

## 使用示例

### 自动适配（零面板改动）

本包与平台渠道包（如 `com.gameframex.unity.operationkeyboard.wechatminigame`）及小游戏 SDK 一起安装。WebGL 小游戏构建下，点按 `InputField` / `TMP_InputField` 即拉起平台键盘；任何面板都无需添加组件。

```csharp
// 游戏代码无需任何改动——自动挂载器负责发现与绑定。
// 业务回调照常工作：
inputField.onValueChanged.AddListener(value => { /* 键盘输入时触发 */ });
inputField.onEndEdit.AddListener(value => { /* confirm / complete / 失焦时触发 */ });
```

### 手动绑定（兜底）

显式挂载保留可用（逐字段适配器工程的迁移路径）：

```csharp
// legacy InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.UGUIInputFieldKeyboardBinder>();
// TMP_InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.TMPInputFieldKeyboardBinder>();
```

### 事件流映射

| 平台键盘事件 | 字段行为 |
|---|---|
| input | `text = value`（`onValueChanged` 触发）+ 光标置尾 |
| confirm / complete | 终值落定、键盘收起、触发 `onEndEdit` |
| 失焦 / 窗口关闭 | 键盘收起、会话解绑（不重复触发 `EndEdit`） |

### 主要类

- **`UGUIInputFieldKeyboardBinder`** / **`TMPInputFieldKeyboardBinder`**：`IOperationKeyboardField` 实现；聚焦 Attach、失焦 Detach、再点按重新 Attach。
- **`OperationKeyboardUGUIAutoBinder`**：自举自动挂载器；`DontDestroyOnLoad` + 低频 `EventSystem` 轮询。
- **`GameFrameXOperationKeyboardUGUICroppingHelper`**：构建裁剪优化用的防裁剪助手。

### 注意事项

- 未注册渠道包（Editor / 原生平台）时门面 no-op：不拦截任何事件，原生 `TouchScreenKeyboard` 路径完整保留。
- `maxLength` 由核心恒传 -1（小游戏键盘按拼音组合串计数，硬上限会掐断中文输入）。

---

## 文档与资源

- **完整文档**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **问题反馈**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/issues)

---

## 社区与支持

- **QQ 讨论群**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **功能建议**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/discussions)

---

## 依赖
| 包 | 说明 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard 核心包 |
| com.gameframex.unity | GameFrameX 核心 |
| com.unity.ugui | 经 versionDefines 探测 |
| com.unity.textmeshpro | 经 versionDefines 探测 |

## 更新日志

查看 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases) 了解更新日志。

## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。
