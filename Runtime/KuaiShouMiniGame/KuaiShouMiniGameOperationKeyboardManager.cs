#if UNITY_WEBGL && ENABLE_KUAISHOU_MINI_GAME && ENABLE_KUAISHOU_MINI_GAME_OPERATIONKEYBOARD

// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相关法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root of the source tree for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any disputes or liabilities arising from secondary development based on this project
//   本项目组织及贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:    https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using GameFrameX.OperationKeyboard.Runtime;

namespace GameFrameX.OperationKeyboard.KuaiShouMiniGame.Runtime
{
    /// <summary>
    /// 快手小游戏键盘管理器。
    /// </summary>
    /// <remarks>
    /// 直通平台键盘 API（KSWASM.KS 的 showKeyboard 家族）；Hide 时注销全部平台事件；
    /// 事件单槽回调由 <see cref="OperationKeyboard"/> 门面在字段会话内统一登记。
    /// This manager drives the KSWASM showKeyboard family directly, unregisters every platform event on Hide,
    /// and relies on the facade to register single-slot callbacks per field session.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class KuaiShouMiniGameOperationKeyboardManager : IOperationKeyboardManager
    {
        /// <summary>
        /// 渠道唯一标识
        /// </summary>
        public const string ChannelId = "kuaishouminigame";

        private Action<string> _onInput;
        private Action<string> _onConfirm;
        private Action<string> _onComplete;
        private Action<float> _onHeightChanged;

        private Action<KSWASM.OnKeyboardInputListenerResult> _onKeyboardInputListener;
        private Action<KSWASM.OnKeyboardInputListenerResult> _onKeyboardConfirmListener;
        private Action<KSWASM.OnKeyboardInputListenerResult> _onKeyboardCompleteListener;
        private Action<KSWASM.OnKeyboardHeightChangeListenerResult> _onKeyboardHeightChangeListener;

        private bool _listening;

        /// <summary>
        /// 拉起键盘并订阅平台键盘事件。
        /// </summary>
        /// <param name="option">键盘参数；<see cref="OperationKeyboardShowOption.MaxLength"/> 恒为 -1（不限长），直传平台。</param>
        [UnityEngine.Scripting.Preserve]
        public void Show(OperationKeyboardShowOption option)
        {
            EnsureListeners();

            KSWASM.KS.ShowKeyboard(new KSWASM.ShowKeyboardOption
            {
                defaultValue = option.DefaultValue,
                maxLength = option.MaxLength,
                multiple = option.Multiple,
                confirmHold = option.ConfirmHold,
                confirmType = option.ConfirmType,
            });
        }

        /// <summary>
        /// 收起键盘并注销全部平台事件。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void Hide()
        {
            if (!_listening)
            {
                return;
            }

            KSWASM.KS.HideKeyboard(new KSWASM.HideKeyboardOption());

            KSWASM.KS.OffKeyboardInput(_onKeyboardInputListener);
            KSWASM.KS.OffKeyboardConfirm(_onKeyboardConfirmListener);
            KSWASM.KS.OffKeyboardComplete(_onKeyboardCompleteListener);
            KSWASM.KS.OffKeyboardHeightChange(_onKeyboardHeightChangeListener);

            _onInput = null;
            _onConfirm = null;
            _onComplete = null;
            _onHeightChanged = null;
            _listening = false;
        }

        /// <summary>
        /// 更新键盘输入框内容（仅键盘处于拉起状态时有效）。
        /// </summary>
        /// <param name="value">当前输入框内容。</param>
        [UnityEngine.Scripting.Preserve]
        public void UpdateValue(string value)
        {
            KSWASM.KS.UpdateKeyboard(new KSWASM.UpdateKeyboardOption
            {
                value = value,
            });
        }

        /// <summary>
        /// 登记键盘输入事件回调（单槽，后设覆盖先设）。
        /// </summary>
        /// <param name="handler">回调：键盘输入的当前值。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnInput(Action<string> handler)
        {
            _onInput = handler;
        }

        /// <summary>
        /// 登记键盘确认事件回调（单槽，后设覆盖先设）。
        /// </summary>
        /// <param name="handler">回调：确认时的当前值。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnConfirm(Action<string> handler)
        {
            _onConfirm = handler;
        }

        /// <summary>
        /// 登记键盘完成（收起）事件回调（单槽，后设覆盖先设）。
        /// </summary>
        /// <param name="handler">回调：完成时的终值。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnComplete(Action<string> handler)
        {
            _onComplete = handler;
        }

        /// <summary>
        /// 登记键盘高度变化事件回调（供避让布局消费）。
        /// </summary>
        /// <param name="handler">回调：键盘高度（像素，0 表示收起）。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnHeightChanged(Action<float> handler)
        {
            _onHeightChanged = handler;
        }

        private void EnsureListeners()
        {
            if (_listening)
            {
                return;
            }

            _onKeyboardInputListener = result => _onInput?.Invoke(result.value ?? string.Empty);
            _onKeyboardConfirmListener = result => _onConfirm?.Invoke(result.value ?? string.Empty);
            _onKeyboardCompleteListener = result => _onComplete?.Invoke(result.value ?? string.Empty);
            _onKeyboardHeightChangeListener = result => _onHeightChanged?.Invoke(result.height);

            KSWASM.KS.OnKeyboardInput(_onKeyboardInputListener);
            KSWASM.KS.OnKeyboardConfirm(_onKeyboardConfirmListener);
            KSWASM.KS.OnKeyboardComplete(_onKeyboardCompleteListener);
            KSWASM.KS.OnKeyboardHeightChange(_onKeyboardHeightChangeListener);

            _listening = true;
        }
    }
}

#endif
