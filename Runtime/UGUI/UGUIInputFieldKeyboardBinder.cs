#if ENABLE_UGUI_OPERATIONKEYBOARD

// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相关法律法规与开源许可证之规定。
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

namespace GameFrameX.OperationKeyboard.UGUI.Runtime
{
    using GameFrameX.OperationKeyboard.Runtime;
    using KeyboardFacade = GameFrameX.OperationKeyboard.Runtime.OperationKeyboard;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// UGUI InputField 键盘绑定器。
    /// </summary>
    /// <remarks>
    /// 聚焦 Attach / 失焦 Detach：OnSelect / OnPointerClick 将本字段接入键盘门面，OnDeselect 解绑，销毁时兜底解绑；
    /// 绑定器从不拦截 InputField 原生事件——未注册渠道时门面 no-op，原生 TouchScreenKeyboard 路径完整保留；
    /// maxLength 恒 -1 由核心保证。绑定器为 public，游戏代码亦可显式挂载（手动模式始终可用）。
    /// Focus attaches the field to the OperationKeyboard facade and blur detaches it; the binder never suppresses
    /// native InputField events, and with no channel registered the facade is a no-op so the native keyboard path stays intact.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    [RequireComponent(typeof(InputField))]
    public sealed class UGUIInputFieldKeyboardBinder : MonoBehaviour, IOperationKeyboardField, ISelectHandler, IDeselectHandler, IPointerClickHandler
    {
        private InputField _inputField;

        /// <summary>
        /// 获取或设置字段当前文本。
        /// </summary>
        /// <remarks>
        /// 键盘 input 事件回灌时经此属性赋值（onValueChanged 照常触发）。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public string Text
        {
            get => _inputField != null ? _inputField.text : string.Empty;
            set
            {
                if (_inputField != null)
                {
                    _inputField.text = value ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// 获取字段当前是否聚焦。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public bool IsFocused
        {
            get => _inputField != null && _inputField.isFocused;
        }

        /// <summary>
        /// 获取是否多行输入。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public bool Multiline
        {
            get => _inputField != null && _inputField.lineType != InputField.LineType.SingleLine;
        }

        /// <summary>
        /// 获取键盘确认按钮类型。
        /// </summary>
        /// <remarks>
        /// UGUI 无确认类型概念，固定 done，可作为扩展点。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public string ConfirmType
        {
            get => "done";
        }

        /// <summary>
        /// 将光标移动到文本末尾。
        /// </summary>
        /// <remarks>
        /// 平台键盘 input 事件携带整串值，赋值后光标须置尾，避免光标停留在起始位置。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void MoveTextEnd()
        {
            if (_inputField != null)
            {
                _inputField.MoveTextEnd(false);
            }
        }

        /// <summary>
        /// 触发字段的结束编辑事件（等价于原生失焦 / 键盘确认后的 onEndEdit）。
        /// </summary>
        /// <remarks>
        /// DeactivateInputField 恰好触发一次 onEndEdit，与原生移动端 Done 后行为一致。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void TriggerEndEdit()
        {
            if (_inputField != null)
            {
                _inputField.DeactivateInputField();
            }
        }

        /// <summary>
        /// 缓存同物体上的 InputField（RequireComponent 保证存在，自动挂载时亦自动补齐组件）。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        private void Awake()
        {
            _inputField = GetComponent<InputField>();
        }

        /// <summary>
        /// 销毁兜底：字段持有键盘会话时解绑，避免会话悬挂。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        private void OnDestroy()
        {
            KeyboardFacade.Detach(this);
            _inputField = null;
        }

        /// <summary>
        /// 字段被选中：绑定键盘会话并拉起键盘（未注册渠道时门面 no-op）。
        /// </summary>
        /// <param name="eventData">事件数据。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnSelect(BaseEventData eventData)
        {
            KeyboardFacade.Attach(this);
        }

        /// <summary>
        /// 字段失焦：解绑键盘会话（不触发 EndEdit，失焦事件由字段自身链路产生）。
        /// </summary>
        /// <param name="eventData">事件数据。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnDeselect(BaseEventData eventData)
        {
            KeyboardFacade.Detach(this);
        }

        /// <summary>
        /// 再次点击已选中字段：补发 Attach（覆盖键盘已折叠但字段仍选中的场景；门面对同一字段幂等）。
        /// </summary>
        /// <param name="eventData">事件数据。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnPointerClick(PointerEventData eventData)
        {
            KeyboardFacade.Attach(this);
        }

        /// <summary>
        /// 字段已处于选中态时补发 Attach（AddComponent 不会为当前选中对象重放 ISelectHandler，由自动挂载器调用）。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        internal void HandleSelected()
        {
            KeyboardFacade.Attach(this);
        }
    }
}

#endif
