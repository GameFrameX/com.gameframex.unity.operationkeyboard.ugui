<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (UGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（UGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（UGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（UGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(UGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.ugui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발 올인원 솔루션 · 인디 개발자의 꿈을 응원

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 목차

- [프로젝트 개요](#프로젝트-개요)
- [빠른 시작](#빠른-시작)
- [사용 예시](#사용-예시)
- [문서 및 자료](#문서-및-자료)
- [커뮤니티 및 지원](#커뮤니티-및-지원)
- [라이선스](#라이선스)

---

## 프로젝트 개요

GameFrameX OperationKeyboard(UGUI 바인딩)는 키보드 패밀리의 UI 바인딩 패키지입니다. `IOperationKeyboardField` 구현을 통해 Unity `InputField`와 `TMP_InputField`를 코어의 `OperationKeyboard` 파사드에 묶고, 런타임에 선택된 입력 필드를 자동 발견하는 자동 바인더를 동봉합니다. 패널 변경 없이 미니게임 키보드 자동 적응을 구현합니다.

### 주요 기능

- **듀얼 바인더** - `UGUIInputFieldKeyboardBinder`(`InputField`)와 `TMPInputFieldKeyboardBinder`(`TMP_InputField`)는 동등한 일급 시민입니다. 포커스 시 키보드 세션을 수립하고 블러 시 해제합니다
- **자가 부트스트랩 자동 바인더** - `DontDestroyOnLoad`로 `EventSystem.currentSelectedGameObject`를 저빈도 폴링하여 입력 필드를 발견하면 즉시 바인딩합니다(패널별 설정 불필요). 호스트 준비 후 부트스트랩 노드는 `[GameFrameX]/UI/Keyboard` 그룹 아래로 이동
- **네이티브 이벤트 흐름** - input 이벤트는 `text`에 대입하고(`onValueChanged`는 평소처럼 발생) 캐럿을 끝으로 이동. confirm / complete는 최종 값을 확정하고 `EndEdit`를 발생(네이티브 모바일과 동일). 블러는 `EndEdit`를 중복 발생시키지 않고 세션을 해제합니다
- **툴킷 탐색** - `versionDefines`가 `com.unity.ugui` / `com.unity.textmeshpro`를 탐색하며 각 바인더는 해당 툴킷이 있을 때만 컴파일됩니다(발견 곧 등록)

### 시스템 요구사항

- Unity 2019.4 이상
- `com.unity.ugui`(legacy `InputField` 바인더) 및 / 또는 `com.unity.textmeshpro`(`TMP_InputField` 바인더)

---

## 빠른 시작

### 설치

다음 방법 중 하나를 선택하세요:

1. Unity 프로젝트의 `Packages/manifest.json`을 편집하여 `scopedRegistries` 섹션을 추가합니다:
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

   `scopes`는 이 레지스트리를 통해 해석되는 패키지를 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies`에 다음을 추가합니다:
   ```json
   {
      "com.gameframex.unity.operationkeyboard.ugui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git"
   }
   ```
3. Unity의 `Package Manager`에서 `Git URL`로 추가합니다: `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.ugui.git`
4. 저장소를 클론하여 Unity 프로젝트의 `Packages` 디렉토리에 배치하면 자동으로 로드됩니다.

## 사용 예시

### 자동 적응(패널 변경 없음)

이 패키지를 플랫폼 채널 패키지(예: `com.gameframex.unity.operationkeyboard.wechatminigame`) 및 미니게임 SDK와 함께 설치합니다. WebGL 미니게임 빌드에서 `InputField` / `TMP_InputField`를 탭하면 플랫폼 키보드가 열립니다. 어떤 패널에도 컴포넌트를 추가할 필요가 없습니다.

```csharp
// 게임 코드에서 할 일이 없습니다. 자동 바인더가 발견과 바인딩을 담당합니다.
// 비즈니스 콜백은 평소처럼 동작합니다:
inputField.onValueChanged.AddListener(value => { /* 키보드 입력 시 발생 */ });
inputField.onEndEdit.AddListener(value => { /* confirm / complete / 블러 시 발생 */ });
```

### 수동 바인딩(폴백)

명시적 마운트도 사용 가능합니다(필드별 어댑터 프로젝트의 마이그레이션 경로):

```csharp
// legacy InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.UGUIInputFieldKeyboardBinder>();
// TMP_InputField
gameObject.GetOrAddComponent<GameFrameX.OperationKeyboard.UGUI.Runtime.TMPInputFieldKeyboardBinder>();
```

### 이벤트 흐름 매핑

| 플랫폼 키보드 이벤트 | 필드 동작 |
|---|---|
| input | `text = value`(`onValueChanged` 발생) + 캐럿 끝으로 |
| confirm / complete | 최종 값 확정, 키보드 접기, `onEndEdit` 발생 |
| 블러 / 창 닫기 | 키보드 접기, 세션 해제(`EndEdit` 중복 없음) |

### 주요 클래스

- **`UGUIInputFieldKeyboardBinder`** / **`TMPInputFieldKeyboardBinder`**: `IOperationKeyboardField` 구현. 포커스 시 Attach, 블러 시 Detach, 재탭 시 재 Attach.
- **`OperationKeyboardUGUIAutoBinder`**: 부트스트랩 자동 바인더. `DontDestroyOnLoad` + 저빈도 `EventSystem` 폴링. 호스트 준비 후 `[GameFrameX]/UI/Keyboard` 그룹 아래로 이동합니다.
- **`GameFrameXOperationKeyboardUGUICroppingHelper`**: 빌드 최적화용 코드 크로핑 헬퍼.

### 주의 사항

- 채널 패키지가 미등록(Editor / 네이티브 플랫폼)이면 파사드는 no-op입니다. 이벤트를 가로채지 않으며 네이티브 `TouchScreenKeyboard` 경로가 온전히 유지됩니다.
- `maxLength`는 코어가 항상 -1을 전달합니다(미니게임 키보드는 병음 조합 문자열을 기준으로 세므로 하드 상한은 중국어 입력을 자릅니다).

---

## 문서 및 자료

- **문서**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **이슈 제보**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/issues)

---

## 커뮤니티 및 지원

- **QQ 그룹**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **기능 제안**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/discussions)

---

## 의존성
| 패키지 | 설명 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard 코어 |
| com.gameframex.unity | GameFrameX 코어 |
| com.unity.ugui | versionDefines로 탐색 |
| com.unity.textmeshpro | versionDefines로 탐색 |

## 변경 로그

변경 이력은 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.ugui/releases)를 참조하세요.

## 라이선스

라이선스에 대한 자세한 내용은 [LICENSE.md](LICENSE.md)를 참조하세요.
