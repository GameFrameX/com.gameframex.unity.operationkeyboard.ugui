<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (UGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（UGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（UGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（UGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(UGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Table of Contents

- [Project Overview](#project-overview)
- [Quick Start](#quick-start)
- [Usage Examples](#usage-examples)
- [Documentation & Resources](#documentation--resources)
- [Community & Support](#community--support)
- [License](#license)

---

## Project Overview

GameFrameX OperationKeyboard (UGUI Binding) is the UI binding package of the keyboard family: it binds Unity `InputField` and `TMP_InputField` to the core `OperationKeyboard` facade through `IOperationKeyboardField` implementations, and ships an auto-binder bootstrap that discovers selected input fields at runtime - zero panel changes for automatic mini-game keyboard adaptation.

### Key Features

- **Dual Binder** - `UGUIInputFieldKeyboardBinder` (`InputField`) and `TMPInputFieldKeyboardBinder` (`TMP_InputField`) are equal first-class citizens; focus attaches the keyboard session, blur detaches
- **Auto-Binder Bootstrap** - a `DontDestroyOnLoad` bootstrap polls `EventSystem.currentSelectedGameObject` at a low frequency, discovers input fields and binds them automatically (no per-panel setup); once the `[GameFrameX]` runtime host is up, the bootstrap node relocates under `[GameFrameX]/UI/Keyboard`
- **Native Event Flow** - input events assign `text` (`onValueChanged` fires as usual) and move the caret to the end; confirm / complete finalize the value and trigger `EndEdit` (same as native mobile); blur detaches without duplicating `EndEdit`
- **Toolkit Probing** - `versionDefines` probe `com.unity.ugui` / `com.unity.textmeshpro`; each binder compiles only when its toolkit is present (discovery means registration)

### System Requirements

- Unity 2019.4 or higher
- `com.unity.ugui` (legacy `InputField` binder) and / or `com.unity.textmeshpro` (`TMP_InputField` binder)

---

## Quick Start

### Installation

Choose one of the following methods:

1. Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:
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

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.operationkeyboard.ugui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.

## Usage Examples

### Automatic Adaptation (Zero Panel Changes)

Install this package together with a platform channel package (e.g. `com.gameframex.unity.operationkeyboard.wechatminigame`) and the mini-game SDK. On WebGL mini-game builds, tapping an `InputField` / `TMP_InputField` now opens the platform keyboard; no component needs to be added to any panel.

```csharp
// Nothing to do in game code - the auto-binder handles discovery and binding.
// Business callbacks keep working unchanged:
inputField.onValueChanged.AddListener(value => { /* fires on keyboard input */ });
inputField.onEndEdit.AddListener(value => { /* fires on confirm / complete / blur */ });
```

### Manual Binding (Fallback)

Explicit mounting stays available (migration path for projects using per-field adapters):

```csharp
// Legacy InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.UGUIInputFieldKeyboardBinder>();
// TMP_InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.TMPInputFieldKeyboardBinder>();
```

### Event Flow Mapping

| Platform keyboard event | Field behavior |
|---|---|
| input | `text = value` (`onValueChanged` fires) + caret to end |
| confirm / complete | final value assigned, keyboard hidden, `onEndEdit` triggered |
| blur / window close | keyboard hidden, session detached (no duplicated `EndEdit`) |

### Main Classes

- **`UGUIInputFieldKeyboardBinder`** / **`TMPInputFieldKeyboardBinder`**: `IOperationKeyboardField` implementations; attach on focus, detach on blur, re-attach on re-tap.
- **`OperationKeyboardUGUIAutoBinder`**: bootstrap auto-binder; `DontDestroyOnLoad` + low-frequency `EventSystem` polling; relocates under the `[GameFrameX]/UI/Keyboard` group node once the runtime host exists.
- **`GameFrameXOperationKeyboardUGUICroppingHelper`**: code cropping helper for build optimization.

### Notes

- With no channel package registered (Editor / native platforms) the facade is a no-op: no event is intercepted and the native `TouchScreenKeyboard` path is fully preserved.
- `maxLength` is always passed as -1 by the core (mini-game keyboards count the pinyin composition string; a hard cap breaks Chinese input).

---

## Documentation & Resources

- **Documentation**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **Issue Tracker**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/issues)

---

## Community & Support

- **QQ Group**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **Feature Requests**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/discussions)

---

## Dependencies
| Package | Description |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard core |
| com.gameframex.unity | GameFrameX core |
| com.unity.ugui | probed via versionDefines |
| com.unity.textmeshpro | probed via versionDefines |

## Changelog

See [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases) for changelog.

## License

See [LICENSE.md](LICENSE.md) for license information.
