using System;
using System.IO;
using System.Windows.Forms;

namespace Game
{
    public partial class Brush : Form
    {
        public Brush()
        {
            InitializeComponent();
        }

        private void Brush_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件选择对话框的属性
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "二进制文件 (*.rom)|*.rom|所有文件 (*.*)|*.*";
            openFileDialog.Multiselect = false; // 只允许选择一个文件
            openFileDialog.InitialDirectory = @"C:\Users\Username\Desktop"; // 设置默认打开位置
            // 如果用户点击了“确定”按钮
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件路径数组
                string[] selectedFiles = openFileDialog.FileNames;
                // 清空ListBox中的内容
                listBox1.Items.Clear();
                // 将选中的文件路径添加到ListBox中
                foreach (string filePath in selectedFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    listBox1.Items.Add(filePath);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 模拟一个操作，比如保存文件或执行某些需要验证的逻辑
                bool operationSuccess = PerformOperation();

                if (operationSuccess)
                {
                    MessageBox.Show("操作成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("操作失败！作者正在努力写这个功能！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool PerformOperation()
        {
            Random random = new Random();
            int result = random.Next(1, 1); // 随机生成0或1

            return result == 0; // 假设返回1表示成功，0表示失败
        }
        /// <summary>
        /// 浏览2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件选择对话框的属性
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "视频文件 (*.mp4)|*.mp4|视频文件 (*.avi)|*.avi|所有文件 (*.*)|*.*";
            openFileDialog.Multiselect = false; // 只允许选择一个文件
            openFileDialog.InitialDirectory = @"C:\Users\Username\Desktop"; // 设置默认打开位置
            // 如果用户点击了“确定”按钮
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件路径数组
                string[] selectedFiles = openFileDialog.FileNames;
                // 清空ListBox中的内容
                listBox2.Items.Clear();
                // 将选中的文件路径添加到ListBox中
                foreach (string filePath in selectedFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    listBox2.Items.Add(filePath);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // 模拟一个操作，比如保存文件或执行某些需要验证的逻辑
                bool operationSuccess = PerformOperation();

                if (operationSuccess)
                {
                    MessageBox.Show("操作成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("操作失败！作者正在努力写这个功能！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 电池信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件选择对话框的属性
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "信息文件 (*.ifo)|*.ifo|所有文件 (*.*)|*.*";
            openFileDialog.Multiselect = false; // 只允许选择一个文件
            openFileDialog.InitialDirectory = @"C:\Users\Username\Desktop"; // 设置默认打开位置
            // 如果用户点击了“确定”按钮
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件路径数组
                string[] selectedFiles = openFileDialog.FileNames;
                // 清空ListBox中的内容
                listBox4.Items.Clear();
                // 将选中的文件路径添加到ListBox中
                foreach (string filePath in selectedFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    listBox4.Items.Add(filePath);
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // 确保 listBox4 中有至少一个路径
            if (listBox4.Items.Count == 0)
            {
                MessageBox.Show("列表为空，请添加文件路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 获取第一个文件路径
            string selectedFilePath = listBox4.Items[0].ToString();
            // 检查文件是否存在
            if (!string.IsNullOrEmpty(selectedFilePath) && System.IO.File.Exists(selectedFilePath))
            {
                // 打开记事本编辑当前文件
                System.Diagnostics.Process.Start("notepad.exe", selectedFilePath);
            }
            else
            {
                MessageBox.Show("文件不存在或未选择文件，请检查文件路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





    }
}
