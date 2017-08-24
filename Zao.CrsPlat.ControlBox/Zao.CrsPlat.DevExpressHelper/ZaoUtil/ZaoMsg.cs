using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;


namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{

    public static class ZaoMsg {

        /// <summary>
        /// 显示一般的提示信息
        /// </summary>
        /// <param name="message">提示信息</param>
        public static DialogResult ShowTips(string message) {
            return XtraMessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示警告信息
        /// </summary>
        /// <param name="message">警告信息</param>
        public static DialogResult ShowWarning(string message) {
            return XtraMessageBox.Show(message, "警告信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public static DialogResult ShowError(string message) {
            return XtraMessageBox.Show(message, "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 显示询问用户信息，并显示错误标志
        /// </summary>
        /// <param name="message">错误信息</param>
        public static DialogResult ShowYesNoAndError(string message) {
            return XtraMessageBox.Show(message, "错误信息", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 显示询问用户信息，并显示提示标志
        /// </summary>
        /// <param name="message">错误信息</param>
        public static DialogResult ShowYesNoAndTips(string message) {
            return XtraMessageBox.Show(message, "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示询问用户信息，并显示警告标志
        /// </summary>
        /// <param name="message">警告信息</param>
        public static DialogResult ShowYesNoAndWarning(string message) {
            return XtraMessageBox.Show(message, "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 显示询问用户信息，并显示提示标志
        /// </summary>
        /// <param name="message">错误信息</param>
        public static DialogResult ShowYesNoCancelAndTips(string message) {
            return XtraMessageBox.Show(message, "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示一个YesNo选择对话框
        /// </summary>
        /// <param name="prompt">对话框的选择内容提示信息</param>
        /// <returns>如果选择Yes则返回true，否则返回false</returns>
        public static bool ConfirmYesNo(string prompt) {
            return XtraMessageBox.Show(prompt, "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// 显示一个YesNoCancel选择对话框
        /// </summary>
        /// <param name="prompt">对话框的选择内容提示信息</param>
        /// <returns>返回选择结果的的DialogResult值</returns>
        public static DialogResult ConfirmYesNoCancel(string prompt) {
            return XtraMessageBox.Show(prompt, "确认", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        public static DevExpress.XtraBars.Alerter.AlertControl alertControl1 = new DevExpress.XtraBars.Alerter.AlertControl();

        /// <summary>
        /// 弹出Winform提示框，需用户反馈后继续
        /// </summary>
        /// <param name="cap">标题标签</param>
        /// <param name="text">信息标签</param>
        /// <param name="msgType">提示类型，  0:成功 1:信息 2:警告 3:错误</param>
        /// <param name="args"></param>
        public static void MsgBox(string text, string cap, int msgType = 1, params string[] args) {
            Alert(null, cap, text, msgType, 0, 0, true, args);
        }

        /// <summary>
        /// 弹出Winform提示框，需用户反馈后继续
        /// </summary>
        /// <param name="cap">标题标签</param>
        /// <param name="text">信息标签</param>
        /// <param name="msgType">提示类型，  0:成功 1:信息 2:警告 3:错误</param>
        /// <param name="args"></param>
        public static void MsgBox(string text, params string[] args) {
            Alert(null, "提示", text, 1, 0, 0, true, args);
        }

        /// <summary>
        /// 弹出提示框,标题默认为提示，无需用户反馈，自动消失
        /// <param name="text">信息标签</param>
        /// <param name="msgType">提示类型，  0:成功 1:信息 2:警告 3:错误</param>
        /// <param name="cap">标题标签，默认为提示</param>
        /// <param name="msgdelay">自动关闭时间（单位：秒），默认3秒</param>
        /// <param name="position">提示框位置，0：右下角 1：左下角 2：左上角 3：右上角，默认：右下角</param>
        /// <param name="args"></param>
        /// </summary>
        public static void Alert(string text, int msgType, string cap = "提示", int msgdelay = 3, int position = 0, params string[] args) {
            Alert(null, cap, text, msgType, msgdelay, position, false, args);
        }

        /// <summary>
        /// 弹出提示框，无需用户反馈，自动消失
        /// </summary>
        /// <param name="ownerForm">父窗体,如果为空，则默认为主窗体</param>
        /// <param name="cap">标题标签</param>
        /// <param name="text">信息标签</param>
        /// <param name="msgType">提示类型，  0:成功 1:信息 2:警告 3:错误</param>
        /// <param name="msgdelay">自动关闭时间（单位：秒），默认5秒</param>
        /// <param name="position">提示框位置，0：右下角 1：左下角 2：左上角 3：右上角，默认：右下角</param>
        /// <param name="args"></param>
        public static void Alert(Form ownerForm, string cap, string text, int msgType, int msgdelay = 5, int position = 0, params string[] args) {
            Alert(ownerForm, cap, text, msgType, msgdelay, position, false, args);
        }

        /// <summary>
        /// 弹出消息，请用户选择
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="caption">标题</param>
        /// <returns></returns>
        public static bool Confirm(string text, string caption) {
            return (DevExpress.XtraEditors.XtraMessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            == DialogResult.Yes);
        }

        /// <summary>
        /// 弹出一个YesNoCancle的消息框(0否，1是，2取消或者直接关闭消息框)
        /// </summary>
        /// <param name="msg">消息框标题</param>
        /// <param name="str">提示的消息</param>
        /// <returns>返回选择状态</returns>
        public static int AskBox(string msg, string str) {
            var state = DevExpress.XtraEditors.XtraMessageBox.Show(str
                    , msg, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);
            if (state == DialogResult.No) {
                return 0;
            } else {
                if (state == DialogResult.Yes) {
                    return 1;
                } else {
                    return 2;
                }
            }
        }

        /// <summary>
        /// 弹出提示框
        /// </summary>
        /// <param name="ownerForm">父窗体</param>
        /// <param name="cap">标题标签</param>
        /// <param name="text">信息标签</param>
        /// <param name="msgType">  0：成功 1 信息 2 警告 3 错误</param>
        /// <param name="msgdelay">提示时间</param>
        /// <param name="position">提示框位置，0：右下角，1：左下角，2：左上角，3：右上角</param>
        /// <param name="ifwinMsg">是否强制使用windows提示</param>
        /// <param name="args"></param>
        private static void Alert(Form ownerForm, string cap, string text, int msgType, int msgdelay, int position, bool ifwinMsg, params string[] args) {
            if (ownerForm == null) {
            }

            for (var i = 0; i < args.Length; i++) {
                text += args[i];
            }

            if (ifwinMsg) {
                switch (msgType) {
                    case 0:
                        DevExpress.XtraEditors.XtraMessageBox.Show(text, cap, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    case 1:
                        DevExpress.XtraEditors.XtraMessageBox.Show(text, cap, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    case 2:
                        DevExpress.XtraEditors.XtraMessageBox.Show(text, cap, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 3:
                        DevExpress.XtraEditors.XtraMessageBox.Show(text, cap, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                return;
            }

            alertControl1.AllowHtmlText = true;
            switch (position) {
                case 0:
                    alertControl1.FormLocation =
                    DevExpress.XtraBars.Alerter.AlertFormLocation.BottomRight;
                    break;

                case 1:
                    alertControl1.FormLocation =
                    DevExpress.XtraBars.Alerter.AlertFormLocation.BottomLeft;
                    break;

                case 2:
                    alertControl1.FormLocation =
                    DevExpress.XtraBars.Alerter.AlertFormLocation.TopLeft;
                    break;

                case 3:
                    alertControl1.FormLocation =
                    DevExpress.XtraBars.Alerter.AlertFormLocation.TopRight;
                    break;
            }

            alertControl1.AutoFormDelay = Convert.ToInt32(msgdelay) * 1000;
            alertControl1.AutoHeight = true;
            alertControl1.FormShowingEffect =
            DevExpress.XtraBars.Alerter.AlertFormShowingEffect.SlideHorizontal;

            if (msgType == 0) {
                alertControl1.Show(ownerForm, cap, text);
            } else {
                if (msgType == 1) {
                    alertControl1.Show(ownerForm, cap, text);
                } else {
                    if (msgType == 2) {
                        alertControl1.Show(ownerForm, cap, text);
                    } else {
                        alertControl1.Show(ownerForm, cap, text);
                    }
                }
            }
        }

        /// <summary>
        /// 弹出等待窗口
        /// </summary>
        /// <param name="caption">标题，默认：“请稍候”</param>
        /// <param name="desc">内容，默认："正在加载……"</param>
        public static void ShowTheWaitForm(string caption = "请稍候", string desc = "正在加载……") {
        }

        /// <summary>
        /// 弹出等待窗口
        /// </summary>
        /// <param name="ParentForm">承载窗体</param>
        /// <param name="caption">标题标签</param>
        /// <param name="desc">介绍文字标签</param>
        public static void ShowTheWaitForm(Form ParentForm, string caption, string desc) {
            ShowTheWaitForm(ParentForm, true, true, caption, desc);
        }

        /// <summary>
        /// 关闭等待窗口
        /// </summary>
        public static void CloseTheWaitForm() {
            try {
                WaitFormManager.CloseWaitForm();
            } catch {
            }
        }

        private static DevExpress.XtraSplashScreen.SplashScreenManager WaitFormManager =
        new DevExpress.XtraSplashScreen.SplashScreenManager();

        /// <summary>
        /// 弹出等待窗口
        /// </summary>
        /// <param name="ParentForm">承载窗体</param>
        /// <param name="useFadein">是否使用淡入效果</param>
        /// <param name="useFadeOut">是否使用淡出效果</param>
        /// <param name="caption">标题</param>
        /// <param name="desc">介绍文字</param>
        private static void ShowTheWaitForm(Form ParentForm, bool useFadein, bool useFadeOut, string caption, string desc) {
            if (ParentForm == null) {
            }
            try {
                WaitFormManager =
                new DevExpress.XtraSplashScreen.SplashScreenManager(ParentForm, typeof(SysWait), useFadein, useFadeOut);

                WaitFormManager.ShowWaitForm();
                WaitFormManager.SetWaitFormCaption(caption);
                WaitFormManager.SetWaitFormDescription(desc);
            } catch {
            }
        }
    }
}