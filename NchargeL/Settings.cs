using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NchargeL.Properties
{
    // 通过此类可以处理设置类的特定事件: 
    //  在更改某个设置的值之前将引发 SettingChanging 事件。
    //  在更改某个设置的值之后将引发 PropertyChanged 事件。
    //  在加载设置值之后将引发 SettingsLoaded 事件。
    //  在保存设置值之前将引发 SettingsSaving 事件。
    internal sealed partial class Settings
    {
        public Settings()
        {
            // // 若要为保存和更改设置添加事件处理程序，请取消注释下列行: 
            //
            this.SettingChanging += this.SettingChangingEventHandler;
            //
            this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
            // 在此处添加用于处理 SettingChangingEvent 事件的代码。
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
            JObject jObject =new JObject();
            jObject.Add("BodyColorS", BodyColorS.ToString());
            jObject.Add("NotificationSuccess", NotificationSuccess.ToString());
            jObject.Add("NotificationError", NotificationError.ToString());
            jObject.Add("NotificationWarning",NotificationWarning.ToString());
            jObject.Add("TextColor", TextColor.ToString());
            jObject.Add("ForegroundColor", ForegroundColor.ToString());
            jObject.Add("BackgroundColor", BackgroundColor.ToString());
            jObject.Add("DownloadSource", DownloadSource);
            jObject.Add("GameDir", GameDir);
            jObject.Add("User", User);
            jObject.Add("Java", Java);
            jObject.Add("RAM", RAM);
            // 在此处添加用于处理 SettingsSaving 事件的代码。
            string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            File.WriteAllText(ApplicationData+"\\NchargeL\\config.json", jObject.ToString(Formatting.Indented));

        }
    }
}