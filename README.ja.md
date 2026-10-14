<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (Kuaishou Mini Game)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（快手小游戏）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（快手小遊戲）', 'ja': 'GameFrameX OperationKeyboard（Kuaishou ミニゲーム）', 'ko': 'GameFrameX OperationKeyboard(콰이쇼우 미니게임)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.kuaishouminigame)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.kuaishouminigame/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.kuaishouminigame)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.kuaishouminigame/releases)
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

GameFrameX OperationKeyboard（Kuaishou ミニゲーム） は Kuaishou ミニゲーム 平台向けキーボード適応チャネルパッケージです。`IOperationKeyboardManager` を実装し（`KuaiShouMiniGameOperationKeyboardManager`、`KS.ShowKeyboard` / `KS.HideKeyboard` / `KS.UpdateKeyboard` plus `On/OffKeyboardInput/Confirm/Complete/HeightChange` (WeChat-isomorphic) を直通）、`KuaiShouMiniGameOperationKeyboardBootstrap` が `BeforeSceneLoad` 段階でメインパッケージの `OperationKeyboardRegistry` に自己登録します（マクロ `UNITY_WEBGL && ENABLE_KUAISHOU_MINI_GAME && ENABLE_KUAISHOU_MINI_GAME_OPERATIONKEYBOARD` が有効でない場合コードは除去され、登録も発生しません。参照即登録）。設定面はありません。キーボードオプションはコアファサードがフィールド契約から導出します。

### 主な機能

- **Kuaishou ミニゲーム キーボード** - ネイティブ SDK の showKeyboard 系と単一スロットコールバックイベントを統合
- **自己登録** - `BeforeSceneLoad` 段階で `OperationKeyboardRegistry` に登録（参照即登録）
- **ゼロ結合** - コアパッケージはチャネル SDK を参照しません。SDK は対象プロジェクトが用意します

### システム要件

- Unity 2019.4+

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
       "com.gameframex.unity.operationkeyboard.kuaishouminigame": "1.0.0"
     }
   }
   ```

   `scopes` はこのレジストリで解決されるパッケージを制御します。`com.gameframex` で始まるパッケージのみこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に以下を追加します：
   ```json
   {
      "com.gameframex.unity.operationkeyboard.kuaishouminigame": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.kuaishouminigame.git"
   }
   ```
3. Unity の `Package Manager` で `Git URL` を使って追加します： `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.kuaishouminigame.git`
4. リポジトリをクローンして Unity プロジェクトの `Packages` ディレクトリに配置すると自動的に読み込まれます。

## 使用例

### チャネル登録（Bootstrap 自己登録）

起動時の設定は不要です。本パッケージを Kuaishou ミニゲーム SDK（`com.kuaishou.minigame`）と一緒にインストールするとチャネルは自動登録されます。UI 入力フィールドはコアファサード経由で駆動されます（`com.gameframex.unity.operationkeyboard.ugui` をインストールすると `InputField` / `TMP_InputField` を自動バインド）。

```csharp
using GameFrameX.OperationKeyboard.Runtime;

// UI バインディングパッケージがフォーカス / ブラー時に自動呼び出し。手動利用（カスタム IOperationKeyboardField）：
// OperationKeyboard.Attach(field);  /  OperationKeyboard.Detach(field);
OperationKeyboard.UpdateValue("プログラム値");
OperationKeyboard.OnHeightChanged(height => { /* 回避レイアウト */ });
```

チャネルの `ChannelId` 定数は `"kuaishouminigame"` です。`OperationKeyboardRegistry.ResolveManager()` は登録順に解決し、`GetManager("kuaishouminigame")` で明示的に取得することもできます。

### プロジェクト構成

```
Runtime/
├── KuaiShouMiniGame/
│   ├── KuaiShouMiniGameOperationKeyboardManager.cs  # Kuaishou ミニゲーム キーボードマネージャー
│   └── KuaiShouMiniGameOperationKeyboardBootstrap.cs  # チャネル起動登録（自己登録）
└── GameFrameXOperationKeyboardKuaiShouMiniGameCroppingHelper.cs  # コードクリッピングヘルパー
```

### 主要なクラス

- **`KuaiShouMiniGameOperationKeyboardManager`**：Kuaishou ミニゲーム キーボード SDK 向けの `IOperationKeyboardManager` 実装（`public const string ChannelId`）。`Hide` 时に全ての平台イベントを解除。
- **`KuaiShouMiniGameOperationKeyboardBootstrap`**：チャネル起動登録。`BeforeSceneLoad` 段階でメインパッケージの `OperationKeyboardRegistry` に自己登録。
- **`GameFrameXOperationKeyboardKuaiShouMiniGameCroppingHelper`**：ビルド最適化用のコードクリッピングヘルパー。

### 注意事項

- イベント面は微信と同型で、キーボード高度変化を含みます。
- `maxLength` は -1 をそのまま渡します。ハード上限はピンイン組成を切断します。

---

## ドキュメントとリソース

- **ドキュメント**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **課題報告**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.kuaishouminigame/issues)

---

## コミュニティとサポート

- **QQ グループ**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **機能リクエスト**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.kuaishouminigame/discussions)

---

## 依存関係
| パッケージ | 説明 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard コア |
| com.gameframex.unity | GameFrameX コア |
| com.kuaishou.minigame | 対象プロジェクトが用意 |

## 変更履歴

更新履歴は [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.kuaishouminigame/releases) を参照してください。

## ライセンス

ライセンスの詳細は [LICENSE.md](LICENSE.md) を参照してください。
