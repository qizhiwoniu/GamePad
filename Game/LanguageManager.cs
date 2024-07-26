using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public delegate void LanguageChangedEventHandler(string langCode);
    public static class LanguageManager
    {
        public static event LanguageChangedEventHandler LanguageChanged;
      

        public static void ChangeLanguage(string langCode)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var newCulture = new CultureInfo(langCode);

            if (Thread.CurrentThread.CurrentUICulture.Name != newCulture.Name)
            {
                Thread.CurrentThread.CurrentUICulture = newCulture;
                Thread.CurrentThread.CurrentCulture = newCulture;
               

                LanguageChanged?.Invoke(langCode); // 触发语言改变事件

            }
        }
    }
}
