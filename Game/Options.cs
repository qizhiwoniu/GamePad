using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Game.Properties;
using System.Resources;
namespace Game
{
    public partial class Options : Form
    {
        
        public Options()
        {           
            InitializeComponent();
            InitializeLanguageComboBox();
            LanguageManager.LanguageChanged += new LanguageChangedEventHandler(LanguageManager_LanguageChanged);
            UpdateUI(Thread.CurrentThread.CurrentUICulture.Name);// 初始化时设置界面文本         
        }
        private void InitializeLanguageComboBox()
        {
            // 添加语言选项
            comboBox1.Items.Clear();
            comboBox1.Items.Add(new ComboBoxItem("English", "en-US"));
            comboBox1.Items.Add(new ComboBoxItem("Chinese", "zh-CN"));
            comboBox1.Items.Add(new ComboBoxItem("German", "de-DE"));
            comboBox1.Items.Add(new ComboBoxItem("Russian", "ru-RU"));
            // 设置默认选项
            SetComboBoxToCurrentLanguage("English");

            // 处理选项改变事件
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            // 获取选中的语言选项
            var selectedItem = comboBox1.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                // 切换应用程序语言
                ChangeLanguage(selectedItem.Value);
            }       
        }
        private void ChangeLanguage(string langCode)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var newCulture = new CultureInfo(langCode);

            if (currentCulture.Name != newCulture.Name)
            {
                Thread.CurrentThread.CurrentUICulture = newCulture;
                Thread.CurrentThread.CurrentCulture = newCulture;

                // 使用一个方法来重新初始化界面，而不是调用 InitializeComponent 以避免递归
                UpdateUI(langCode);
            }
            
        }
        private void LanguageManager_LanguageChanged(string langCode)
        {
            // 在语言切换事件中调用 ChangeLanguage 方法
            ChangeLanguage(langCode);
        }
        public void UpdateUI(string langCode)
        {   
            // 获取当前线程的文化信息
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            // 使用资源文件中的文本设置窗体标题
            if (culture.Name.ToLower() == "en-US")
            {   
                this.Text = Resources.Options_en_US;
                button4.Text = Resources.Options_en_US;
                label1.Text  = Resources.Options_en_US;
                button1.Text = Resources.Options_en_US;
                button2.Text = Resources.Options_en_US;
                button3.Text = Resources.Options_en_US;
                comboBox1.Text = Resources.Options_en_US;
          
            }
            else if (culture.Name.ToLower() == "zh-CN")
            {
                this.Text = Resources.Options_zh_CN;
                button4.Text = Resources.Options_zh_CN;
                label1.Text = Resources.Options_zh_CN;
                button1.Text = Resources.Options_zh_CN;
                button2.Text = Resources.Options_zh_CN;
                button3.Text = Resources.Options_zh_CN;
                comboBox1.Text = Resources.Options_zh_CN;
            }

            // 这里更新其他需要国际化的控件文本
           
        }
        private void SetComboBoxToCurrentLanguage(string language)
        {
            string currentLangCode = Thread.CurrentThread.CurrentUICulture.Name;
            // 根据当前语言设置 ComboBox 的选中项
            foreach (var item in comboBox1.Items)
            {
                var comboBoxItem = item as ComboBoxItem;
                if (comboBoxItem != null && comboBoxItem.Value == currentLangCode)
                {
                    comboBox1.SelectedItem = comboBoxItem;
                    break;
                }
            }
        }
        public class ComboBoxItem
        {
            public string Text
            {
                get; set;
            }
            public string Value
            {
                get; set;
            }

            public ComboBoxItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
