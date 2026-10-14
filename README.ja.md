<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (UGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（UGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（UGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（UGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(UGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディーゲーム開発のオールインワンソリューション · インディーデベロッパーの夢を応援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## 目次

- [プロジェクト概要](#プロジェクト概要)
- [クイックスタート](#クイックスタート)
- [使用例](#使用例)
- [ドキュメントとリソース](#ドキュメントとリソース)
- [コミュニティとサポート](#コミュニティとサポート)
- [ライセンス](#ライセンス)

---

## プロジェクト概要

GameFrameX OperationKeyboard（UGUI バインディング）はキーボードファミリーの UI バインディングパッケージです。`IOperationKeyboardField` 実装を通じて Unity の `InputField` と `TMP_InputField` をコアの `OperationKeyboard` ファサードに束ね、実行時に選択された入力フィールドを自動発見する自動バインダーを同梱します。パネル変更ゼロでミニゲームキーボードの自動適応を実現します。

### 主な機能

- **デュアルバインダー** - `UGUIInputFieldKeyboardBinder`（`InputField`）と `TMPInputFieldKeyboardBinder`（`TMP_InputField`）は同等のファーストクラス扱い。フォーカスでキーボードセッションを確立し、ブラーで解除します
- **自己ブートストラップ自動バインダー** - `DontDestroyOnLoad` で `EventSystem.currentSelectedGameObject` を低頻度ポーリングし、入力フィールドを発見したら即バインドします（パネルごとの設定は不要）
- **ネイティブイベントフロー** - input イベントは `text` に代入し（`onValueChanged` は通常どおり発火）、キャレットを末尾へ。confirm / complete は最終値を確定して `EndEdit` を発火（ネイティブモバイルと同じ）。ブラーは `EndEdit` を重複させずにセッションを解除します
- **ツールキット探索** - `versionDefines` が `com.unity.ugui` / `com.unity.textmeshpro` を探索し、各バインダーは対応ツールキットが存在する场合のみコンパイルされます（発見即登録）

### システム要件

- Unity 2019.4 以上
- `com.unity.ugui`（legacy `InputField` バインダー）および / または `com.unity.textmeshpro`（`TMP_InputField` バインダー）

---

## クイックスタート

### インストール

以下のいずれかの方法を選択してください：

1. Unity プロジェクトの `Packages/manifest.json` を編集し、`scopedRegistries` セクションを追加します：
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

   `scopes` はこのレジストリで解決されるパッケージを制御します。`com.gameframex` で始まるパッケージのみこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に以下を追加します：
   ```json
   {
      "com.gameframex.unity.operationkeyboard.ugui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git"
   }
   ```
3. Unity の `Package Manager` で `Git URL` を使って追加します： `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git`
4. リポジトリをクローンして Unity プロジェクトの `Packages` ディレクトリに配置すると自動的に読み込まれます。

## 使用例

### 自動適応（パネル変更ゼロ）

本パッケージをプラットフォームチャネルパッケージ（例: `com.gameframex.unity.operationkeyboard.wechatminigame`）およびミニゲーム SDK と一緒にインストールします。WebGL ミニゲームビルドでは、`InputField` / `TMP_InputField` をタップすると平台キーボードが開きます。パネルにコンポーネントを追加する必要は一切ありません。

```csharp
// ゲームコードでの作業は不要です。自動バインダーが発見とバインドを担います。
// ビジネスコールバックはこれまでどおり動作します：
inputField.onValueChanged.AddListener(value => { /* キーボード入力で発火 */ });
inputField.onEndEdit.AddListener(value => { /* confirm / complete / ブラーで発火 */ });
```

### 手動バインド（フォールバック）

明示的なマウントも利用できます（フィールドごとのアダプタからの移行パス）：

```csharp
// legacy InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.UGUIInputFieldKeyboardBinder>();
// TMP_InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.TMPInputFieldKeyboardBinder>();
```

### イベントフローのマッピング

| 平台キーボードイベント | フィールドの動作 |
|---|---|
| input | `text = value`（`onValueChanged` 発火）+ キャレットを末尾へ |
| confirm / complete | 最終値の確定、キーボードを畳む、`onEndEdit` 発火 |
| ブラーウィンドウ | キーボードを畳む、セッション解除（`EndEdit` の重複なし） |

### 主要なクラス

- **`UGUIInputFieldKeyboardBinder`** / **`TMPInputFieldKeyboardBinder`**：`IOperationKeyboardField` 実装。フォーカスで Attach、ブラーで Detach、再タップで再 Attach。
- **`OperationKeyboardUGUIAutoBinder`**：ブートストラップ自動バインダー。`DontDestroyOnLoad` + 低頻度 `EventSystem` ポーリング。
- **`GameFrameXOperationKeyboardUGUICroppingHelper`**：ビルド最適化用のコードクリッピングヘルパー。

### 注意事項

- チャネルパッケージが未登録（Editor / ネイティブ平台）の場合、ファサードは no-op です。イベントを一切インターセプトせず、ネイティブ `TouchScreenKeyboard` の経路が完全に保持されます。
- `maxLength` はコアから常に -1 が渡されます（ミニゲームキーボードはピンイン組成文字列をカウントするため、ハード上限は中国語入力を切断します）。

---

## ドキュメントとリソース

- **ドキュメント**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **課題報告**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/issues)

---

## コミュニティとサポート

- **QQ グループ**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **機能リクエスト**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/discussions)

---

## 依存関係
| パッケージ | 説明 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard コア |
| com.gameframex.unity | GameFrameX コア |
| com.unity.ugui | versionDefines で探索 |
| com.unity.textmeshpro | versionDefines で探索 |

## 変更履歴

更新履歴は [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases) を参照してください。

## ライセンス

ライセンスの詳細は [LICENSE.md](LICENSE.md) を参照してください。
