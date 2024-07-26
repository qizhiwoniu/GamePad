using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Guna.UI2.WinForms;
using Microsoft.Toolkit.Uwp.Notifications;
using Nefarius.ViGEm.Client.Targets;
using Newtonsoft.Json.Linq;
using SharpDX.DirectInput;
using Windows.UI.Notifications;
using Button = System.Windows.Forms.Button;

namespace Game
{
    public partial class Form1 : Form
    {
        private CircularButton[] buttons;
        private bool[] buttonStates;
        private DirectInput directInput;
        private Joystick joystick;
        private JoystickState joystickState;
        private Guna2Panel acrylicPanel;
        private ContextMenuStrip sharedMenu;
        private ToolStripTextBox textBox;
        private Button currentButton; // 用于记录当前点击的按钮
        private NotifyIcon trayIcon;//任务栏图标
        private ContextMenu trayMenu;//任务栏图标菜单
        private Theme currentTheme;//主题
        private int sensitivityFactor = (int)500f; // 灵敏度因子，用于控制移动量
        private JoystickControl joystickControl; // 声明 JoystickControl 对象
        private const int MaxAxisValue = 65535; // 假设手柄轴的最大值
        private const float MovementSpeed = 0.01f; // 移动速度
        private IXbox360Controller xbox360Controller; // 类成员
        private Point crossPositionLeft = Point.Empty; // 左十字叉位置
        private Point crossPositionRight = Point.Empty; // 右十字叉位置
        private const int squareSize = 90;
        private const int crossSize = 10;
        private Timer timer;
        private List<JoystickState> recordedStates = new List<JoystickState>(); // 用于记录手柄状态
        private bool isRecording = false;
        private const int DeadZone = 1000; // 死区范围，防止微小抖动
        private float sensitivity = 0.01f; // 初始值可以根据需要调整
        public Form1()
        {
            string theme = Properties.Settings.Default.Theme;
            currentTheme = theme == "Dark" ? Themes.DarkTheme : Themes.LightTheme;

            InitializeComponent();
            InitializeAcrylicEffect();
            LoadImage();
            InitializeSharedMenu();

            #region 任务栏图标
            // 创建一个右键菜单
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Setting", OnSetting);
            trayMenu.MenuItems.Add("Brush Writing", OnBrush);
            trayMenu.MenuItems.Add("Plugins", OnPlugins);
            trayMenu.MenuItems.Add("Updata", OnUpdata);
            trayMenu.MenuItems.Add("About", OnAbout);
            trayMenu.MenuItems.Add("Exit", OnExit);

            // 创建系统托盘图标
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Game Pad Test";
            trayIcon.Icon = new Icon("C:\\Users\\Administrator\\Documents\\Game\\Game\\Resources\\banlizai.ico"); // 任务栏图标
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            // 添加双击事件处理程序
            trayIcon.DoubleClick += TrayIconDoubleClick;
            #endregion

            InitializeJoystick();
            //InitializeTimer();
            crossPositionLeft = new Point(groupBox2.Width / 4, groupBox2.Height / 2);
            crossPositionRight = new Point(3 * groupBox2.Width / 4, groupBox2.Height / 2);
            // 设置双缓冲
            SetDoubleBuffered(groupBox2);
        }

        #region 摇杆测试
        private void SetDoubleBuffered(Control control)
        {
            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(control, true, null);
        }
        private void InitializeJoystick()
        {
            directInput = new DirectInput();
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                joystickGuid = deviceInstance.InstanceGuid;
                Console.WriteLine($"Detected Gamepad: {deviceInstance.InstanceName} - {joystickGuid}");
            }

            if (joystickGuid == Guid.Empty)
            {
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                {
                    joystickGuid = deviceInstance.InstanceGuid;
                    Console.WriteLine($"Detected Joystick: {deviceInstance.InstanceName} - {joystickGuid}");
                }
            }

            if (joystickGuid != Guid.Empty)
            {
                joystick = new Joystick(directInput, joystickGuid);
                joystick.Properties.BufferSize = 128;
                joystick.Acquire();
            }
            else
            {
                MessageBox.Show("No joystick found.");
            }
        }

        /*private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Start();
        }*/

       /* private void Timer_Tick(object sender, EventArgs e)
        {
            if (joystick == null) return;

            joystick.Poll();
            //var datas = joystick.GetBufferedData();
            var state = joystick.GetCurrentState();
            Console.WriteLine($"X: {state.X}, Y: {state.Y}, RotationX: {state.RotationX}, RotationY: {state.RotationY}");
            // 更新左侧十字叉位置
            int xValue = state.X - 32767;
            int yValue = state.Y - 32767;
            if (Math.Abs(xValue) < DeadZone) xValue = 0;
            if (Math.Abs(yValue) < DeadZone) yValue = 0;

            crossPositionLeft.X = MapValueToRange(state.X, 0, 65535, 0, groupBox2.Width / 2);
            crossPositionLeft.Y = MapValueToRange(state.Y, 0, 65535, 0, groupBox2.Height);

            // 更新右侧十字叉位置
            int rotationXValue = state.RotationX - 32767;
            int rotationYValue = state.RotationY - 32767;
            if (Math.Abs(rotationXValue) < DeadZone) rotationXValue = 0;
            if (Math.Abs(rotationYValue) < DeadZone) rotationYValue = 0;

            crossPositionRight.X = MapValueToRange(state.RotationX, 0, 65535, groupBox2.Width / 2, groupBox2.Width);
            crossPositionRight.Y = MapValueToRange(state.RotationY, 0, 65535, 0, groupBox2.Height);

            // 限制十字叉位置在绘图区域内
            crossPositionLeft.X = Math.Max(0, Math.Min(groupBox2.Width / 2, crossPositionLeft.X));
            crossPositionLeft.Y = Math.Max(0, Math.Min(groupBox2.Height, crossPositionLeft.Y));
            crossPositionRight.X = Math.Max(groupBox2.Width / 2, Math.Min(groupBox2.Width, crossPositionRight.X));
            crossPositionRight.Y = Math.Max(0, Math.Min(groupBox2.Height, crossPositionRight.Y));

            
        
        foreach (var state in datas)
            {
                Console.WriteLine($"Offset: {state.Offset}, Value: {state.Value}"); // 添加调试输出

                // 校准和处理死区
                int value = state.Value - 32767;
                if (Math.Abs(value) < DeadZone) value = 0;
                // 根据偏移量更新对应的十字叉位置
                if (state.Offset == JoystickOffset.X)
                {
                    Console.WriteLine($"Updating crossPositionLeft.X to {state.Value}"); // 调试信息
                    crossPositionLeft.X = MapValueToRange(state.Value, 0, 65535, 0, groupBox2.Width / 2);
                    //crossPositionLeft.X += (int)(value * sensitivity);
                    //crossPositionLeft.X += (int)((state.Value - 32767) * sensitivity);
                    //crossPositionLeft.X = MapValueToRange(state.Value, 0, 65535, (groupBox2.Width / 4) - (squareSize / 2), (groupBox2.Width / 4) + (squareSize / 2));
                }
                if (state.Offset == JoystickOffset.Y)
                {
                    Console.WriteLine($"Updating crossPositionLeft.Y to {state.Value}"); // 调试信息
                    crossPositionLeft.Y = MapValueToRange(state.Value, 0, 65535, 0, groupBox2.Height);
                    //crossPositionLeft.Y += (int)(value * sensitivity);
                    //crossPositionLeft.Y += (int)((state.Value - 32767) * sensitivity);
                    //crossPositionLeft.Y += (int)(MapValueToRange(state.Value, 0, 65535, -1, 1) * sensitivity);
                    //crossPositionLeft.Y = MapValueToRange(state.Value, 0, 65535, (groupBox2.Height / 2) - (squareSize / 2), (groupBox2.Height / 2) + (squareSize / 2));
                }
                if (state.Offset == JoystickOffset.RotationX)
                {
                    Console.WriteLine($"Updating crossPositionRight.X to {state.Value}"); // 调试信息
                    crossPositionRight.X = MapValueToRange(state.Value, 0, 65535, groupBox2.Width / 2, groupBox2.Width);
                
                //crossPositionRight.X += (int)(value * sensitivity);
                //crossPositionRight.X = MapValueToRange(state.Value, 0, 65535, (3 * groupBox2.Width / 4) - (squareSize / 2), (3 * groupBox2.Width / 4) + (squareSize / 2));
                }
                if (state.Offset == JoystickOffset.RotationY)
                {
                    Console.WriteLine($"Updating crossPositionRight.Y to {state.Value}"); // 调试信息
                    crossPositionRight.Y = MapValueToRange(state.Value, 0, 65535, 0, groupBox2.Height);
                    //crossPositionRight.Y += (int)(value * sensitivity);
                    //crossPositionRight.Y = MapValueToRange(state.Value, 0, 65535, (groupBox2.Height / 2) - (squareSize / 2), (groupBox2.Height / 2) + (squareSize / 2));
                }
                // 限制十字叉位置在绘图区域内
                crossPositionLeft.X = Math.Max(0, Math.Min(groupBox2.Width / 2, crossPositionLeft.X));
                crossPositionLeft.Y = Math.Max(0, Math.Min(groupBox2.Height, crossPositionLeft.Y));
                crossPositionRight.X = Math.Max(groupBox2.Width / 2, Math.Min(groupBox2.Width, crossPositionRight.X));
                crossPositionRight.Y = Math.Max(0, Math.Min(groupBox2.Height, crossPositionRight.Y));

                // 如果正在录制，记录手柄状态
                if (isRecording)
                {
                    recordedStates.Add(joystick.GetCurrentState());
                }
            

            groupBox2.Invalidate();
        }*/
        private int MapValueToRange(int value, int inputMin, int inputMax, int outputMin, int outputMax)
        {
            return outputMin + (value - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);
        }
        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // 绘制左侧正方形
            Rectangle rectLeft = new Rectangle((groupBox2.Width / 4) - (squareSize / 2), (groupBox2.Height / 2) - (squareSize / 2), squareSize, squareSize);
            g.DrawRectangle(Pens.Black, rectLeft);

            // 绘制右侧正方形
            Rectangle rectRight = new Rectangle((3 * groupBox2.Width / 4) - (squareSize / 2), (groupBox2.Height / 2) - (squareSize / 2), squareSize, squareSize);
            g.DrawRectangle(Pens.Black, rectRight);

            // 绘制左侧十字叉
            g.DrawLine(Pens.Black, crossPositionLeft.X - crossSize / 2, crossPositionLeft.Y, crossPositionLeft.X + crossSize / 2, crossPositionLeft.Y);
            g.DrawLine(Pens.Black, crossPositionLeft.X, crossPositionLeft.Y - crossSize / 2, crossPositionLeft.X, crossPositionLeft.Y + crossSize / 2);

            // 绘制右侧十字叉
            g.DrawLine(Pens.Black, crossPositionRight.X - crossSize / 2, crossPositionRight.Y, crossPositionRight.X + crossSize / 2, crossPositionRight.Y);
            g.DrawLine(Pens.Black, crossPositionRight.X, crossPositionRight.Y - crossSize / 2, crossPositionRight.X, crossPositionRight.Y + crossSize / 2);

            // 绘制左侧文字
            string textLeft = "X 轴/Y 轴";
            SizeF textSizeLeft = g.MeasureString(textLeft, this.Font);
            PointF textPositionLeft = new PointF((groupBox2.Width / 4) - (textSizeLeft.Width / 2), (groupBox2.Height / 2) + (squareSize / 2) + 5);
            g.DrawString(textLeft, this.Font, Brushes.Black, textPositionLeft);

            // 绘制右侧文字
            string textRight = "X 轴/Y 轴";
            SizeF textSizeRight = g.MeasureString(textRight, this.Font);
            PointF textPositionRight = new PointF((3 * groupBox2.Width / 4) - (textSizeRight.Width / 2), (groupBox2.Height / 2) + (squareSize / 2) + 5);
            g.DrawString(textRight, this.Font, Brushes.Black, textPositionRight);

        }
        #endregion

        #region 主题
        public class Theme
        {
            public Color BackgroundColor
            {
                get; set;
            }
            public Color ForegroundColor
            {
                get; set;
            }
            public Color ButtonColor
            {
                get; set;
            }
            public Color TabControlBackColor
            {
                get; set;
            }
            public Color TabPageBackColor
            {
                get; set;
            }
            public Color TabForeColor
            {
                get; set;
            }
            public Color MenuStripBackColor
            {
                get; set;
            }
            public Color MenuStripForeColor
            {
                get; set;
            }
            public Color ToolStripMenuItemBackColor
            {
                get; set;
            }
            public Color ToolStripMenuItemForeColor
            {
                get; set;
            }
            // 其他控件的颜色
        }
        public static class Themes
        {
            public static Theme LightTheme = new Theme
            {
                BackgroundColor = Color.White,
                ForegroundColor = Color.Black,
                ButtonColor = Color.LightGray,
                TabControlBackColor = Color.White,
                TabPageBackColor = Color.White,
                TabForeColor = Color.Black,
                MenuStripBackColor = Color.White,
                MenuStripForeColor = Color.Black,
                ToolStripMenuItemBackColor = Color.White,
                ToolStripMenuItemForeColor = Color.Black
                // 其他颜色配置
            };

            public static Theme DarkTheme = new Theme
            {
                BackgroundColor = Color.FromArgb(45, 45, 48),
                ForegroundColor = Color.White,
                ButtonColor = Color.FromArgb(28, 28, 28),
                TabControlBackColor = Color.FromArgb(45, 45, 48),
                TabPageBackColor = Color.FromArgb(45, 45, 48),
                TabForeColor = Color.White,
                MenuStripBackColor = Color.FromArgb(45, 45, 48),
                MenuStripForeColor = Color.White,
                ToolStripMenuItemBackColor = Color.FromArgb(45, 45, 48),
                ToolStripMenuItemForeColor = Color.White
                // 其他颜色配置
            };
        }
       
        public class CustomToolStripRenderer : ToolStripRenderer
        {
            private Theme theme;
            private Color separatorColor = Color.Gray; // 设置分隔线颜色为灰色
            public CustomToolStripRenderer(Theme theme)
            {
                this.theme = theme;
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                base.OnRenderSeparator(e);
                using (var pen = new Pen(separatorColor))
                {
                    int y = e.Item.ContentRectangle.Height / 2;
                    e.Graphics.DrawLine(pen, e.Item.ContentRectangle.Left, y, e.Item.ContentRectangle.Right, y);
                }
               /*e.Graphics.FillRectangle(new SolidBrush(theme.MenuStripBackColor), e.Item.Bounds);
                e.Graphics.DrawLine(new Pen(theme.ForegroundColor),
                    e.Item.Bounds.Left, e.Item.Bounds.Height / 2,
                    e.Item.Bounds.Right, e.Item.Bounds.Height / 2);*/
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(theme.ButtonColor), e.Item.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(theme.MenuStripBackColor), e.Item.Bounds);
                }
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                e.Graphics.FillRectangle(new SolidBrush(theme.MenuStripBackColor), e.AffectedBounds);
            }
            
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                // 不绘制边框，实现边框透明
            }
        }
     
        public class CustomProfessionalColors : ProfessionalColorTable
        {
            public override Color ToolStripBorder => Color.Transparent; // 设置工具栏边框颜色为透明
        }
        public void ApplyTheme(Theme theme)
        {
            this.BackColor = theme.BackgroundColor;
            this.ForeColor = theme.ForegroundColor;

            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.BackColor = theme.ButtonColor;
                    control.ForeColor = ColorHelper.ContrastColor(theme.ButtonColor);

                }
                // 处理其他控件
                else if (control is TabControl tabControl)
                {
                    tabControl.BackColor = theme.TabControlBackColor;
                    tabControl.ForeColor = theme.TabForeColor;

                    foreach (TabPage tabPage in tabControl.TabPages)
                    {
                        tabPage.BackColor = theme.TabPageBackColor;
                        tabPage.ForeColor = theme.TabForeColor;
                    }
                }
                else if (control is MenuStrip menuStrip)
                {
                    menuStrip.BackColor = theme.MenuStripBackColor;
                    menuStrip.ForeColor = theme.MenuStripForeColor;
                    menuStrip.Renderer = new CustomToolStripRenderer(theme);
                    foreach (ToolStripMenuItem menuItem in menuStrip.Items)
                    {
                        ApplyMenuItemTheme(menuItem, theme);
                    }
                }
            }     
            this.Invalidate(true); // 强制窗体重新绘制
        }
        public static class ColorHelper
        {
            public static Color ContrastColor(Color backgroundColor)
            {
                // 计算颜色的亮度
                double brightness = (backgroundColor.R * 0.299 + backgroundColor.G * 0.587 + backgroundColor.B * 0.114) / 255;

                // 如果亮度小于 0.5，则返回白色；否则返回黑色
                return brightness < 0.5 ? Color.White : Color.Black;
            }
        }
        private void ApplyMenuItemTheme(ToolStripMenuItem menuItem, Theme theme)
        {
            menuItem.BackColor = theme.ToolStripMenuItemBackColor;
            menuItem.ForeColor = theme.ToolStripMenuItemForeColor;

            foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
            {
                if (dropDownItem is ToolStripMenuItem subMenuItem)
                {
                    ApplyMenuItemTheme(subMenuItem, theme);
                }
            }
        }  
        #endregion

        #region 任务栏图标
        // 双击系统托盘图标时触发的事件处理程序
        private void TrayIconDoubleClick(object sender, EventArgs e)
        {
            // 在此处添加双击图标时的操作
            // 例如，显示主窗体或者执行其他操作
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        // 点击退出菜单项时触发的事件处理程序
        private async void OnExit(object sender, EventArgs e)
        {
            try
            {      
                // 释放资源
                IDisposable disposableController = xbox360Controller as IDisposable;
                if (disposableController != null)
                {
                    disposableController.Dispose();
                }
            }
            catch (Exception ex)
            {
                // 处理异常，可以记录日志或者进行其他操作
                Console.WriteLine($"Exception on exit: {ex.Message}");
            }
            // 在此处添加退出操作
            System.Windows.Forms.Application.Exit();
        }
        private void OnBrush(object sender, EventArgs e)
        {
            Brush brush = new Brush();
            brush.Show();
        }
        private void OnSetting(object sender, EventArgs e)
        {
            Options options = new Options();
            options.Show();
        }
        private void OnPlugins(object sender, EventArgs e)
        {
            Plugins plugins = new Plugins();
            plugins.Show();
        }
        private void OnAbout(object sender, EventArgs e)
        {         
            About about = new About();
            about.Show();
        }
        private async void OnUpdata(object sender, EventArgs e)
        {
            string latestVersion = await Task.Run(() => GetLatestReleaseVersionAsync("qizhoward", "GamePad"));
            string localVersion = await Task.Run(() => GetLocalVersion());
            if (IsNewerVersion(localVersion, latestVersion))
            {
                await DownloadLatestReleaseAsync("qizhoward", "GamePad", latestVersion);
            }
            else
            {
                MessageBox.Show("You already have the latest version.");
            }
        }
        // 重写窗体关闭事件，以便将窗体最小化到系统托盘
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                ShowToastNotification("友好提示", "双击鼠标左键还原");
                e.Cancel = true;
                this.Hide();
            }
          
        }
        #region 消息提示

        private void ShowToastNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText(title)
                .AddText(message)
                .Show();
        }

        #endregion
        // 在窗体加载时显示系统托盘图标
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            trayIcon.Visible = true;
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            buttons = new CircularButton[20];
            buttonStates = new bool[20];
            int buttonPerRow = 8; // 每行按钮数
            int buttonWidth = 25; // 按钮宽度
            int buttonHeight = 25; // 按钮高度
            int spacing = 12; // 按钮之间的间距

            // 创建12个圆形按钮并添加到窗口中
            for (int i = 0; i < 20; i++)
            {
                CircularButton button = new CircularButton();
                button.Location = new Point(spacing + (i % buttonPerRow) * (buttonWidth + spacing), spacing + (i / buttonPerRow) * (buttonHeight + spacing));
                button.Size = new Size(buttonWidth, buttonHeight); // 设置按钮大小
                button.Text = (i + 1).ToString(); // 设置按钮文本为序号
                button.Font = new Font(button.Font.FontFamily, 12); // 设置按钮字体大小
                Console.WriteLine($"Button {i + 1}: Location ({button.Location.X}, {button.Location.Y}), Size ({button.Width}, {button.Height})");
                this.Controls.Add(button);
                buttons[i] = button;
                groupBox1.Controls.Add(button);
            }
           
            //右键删除图标
            InitializeContextMenuStrip();

            Theme theme = new Theme();
            // 创建自定义的 ToolStripRenderer
            CustomToolStripRenderer customToolStripRenderer = new CustomToolStripRenderer(theme);

            // 设置 MenuStrip 的 Renderer
            menuStrip1.Renderer = customToolStripRenderer;
            statusStrip1.Renderer = new CustomToolStripRenderer(theme);

            ApplyTheme(currentTheme);
            this.BackColor = currentTheme.BackgroundColor;

           #region 方向舵测试
            foreach (Control control in this.Controls)
            {
                if (control is JoystickControl)
                {
                    this.Controls.Remove(control);
                    break; // 因为只有一个 JoystickControl，找到后可以直接退出循环
                }
            }
            joystickControl = new JoystickControl();
            groupBox3.Controls.Add(joystickControl);
            joystickControl.Parent = groupBox3;
            int centerX = (groupBox3.Width - joystickControl.Width) / 2;
            int centerY = (groupBox3.Height - joystickControl.Height) / 2;

            // Set the location of joystickControl
            joystickControl.Location = new Point(centerX, centerY);
            #endregion
        }
    
        private void ProcessJoystickInput(JoystickState state)
        {
            // 根据手柄输入控制按钮高亮显示
            buttonStates[0] = state.Buttons[0];
            buttonStates[1] = state.Buttons[1];
            buttonStates[2] = state.Buttons[2];
            buttonStates[3] = state.Buttons[3];
            buttonStates[4] = state.Buttons[4];
            buttonStates[5] = state.Buttons[5];
            buttonStates[6] = state.Buttons[6];
            buttonStates[7] = state.Buttons[7];
            buttonStates[8] = state.Buttons[8];
            buttonStates[9] = state.Buttons[9];
            buttonStates[10] = state.Buttons[10];
            buttonStates[11] = state.Buttons[11];//false; // 这里根据需要设置其他按钮的状态
            buttonStates[12] = state.Buttons[12];
            buttonStates[13] = state.Buttons[13];
            buttonStates[14] = state.Buttons[14];
            buttonStates[15] = state.Buttons[15];
            buttonStates[16] = state.Buttons[16];
            buttonStates[17] = state.Buttons[17];
            buttonStates[18] = state.Buttons[18];
            buttonStates[19] = state.Buttons[19];
            for (int i = 0; i < 20; i++)
            {
                if (buttonStates[i])
                    buttons[i].BackColor = Color.LightGray;
                else
                    buttons[i].BackColor = Color.Gray;
            }
        }
        #region 按钮测试
        public class CircularButton : Control
        {
            private Color hoverColor = Color.LightGray;
            private Color defaultColor = Color.Gray;

            public CircularButton()
            {
                this.BackColor = defaultColor;
                this.Cursor = Cursors.Hand;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 绘制圆形按钮
                g.FillEllipse(new SolidBrush(this.BackColor), 0, 0, this.Width - 1, this.Height - 1);
                g.DrawEllipse(new Pen(Color.Black), 0, 0, this.Width - 1, this.Height - 1);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                this.BackColor = hoverColor;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                this.BackColor = defaultColor;
            }
        }
        #endregion

        #region 方向舵测试
        public class JoystickControl : Control
        {
            private int bigCircleRadius = 50; // 大圆圈半径
            private int smallCircleRadius = 10; // 小圆圈半径
            private Point bigCircleCenter; // 大圆中心点
            private Point smallCircleCenter; // 小圆中心点
            
            private bool isDragging = false; // 是否正在拖动小圆圈

            public JoystickControl()
            {
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                this.DoubleBuffered = true;
                this.Size = new Size(bigCircleRadius * 2, bigCircleRadius * 2);

                // 初始化大圆和小圆的中心位置
                bigCircleCenter = new Point(bigCircleRadius, bigCircleRadius);
                smallCircleCenter = new Point(bigCircleRadius, bigCircleRadius); // 初始位置在大圆中心
                this.MouseDown += JoystickControl_MouseDown;
                this.MouseMove += JoystickControl_MouseMove;
                this.MouseUp += JoystickControl_MouseUp;
                
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                
            base.OnPaint(e);

            // 绘制大圆圈
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // 绘制大圆
            Rectangle bigCircleRect = new Rectangle(bigCircleCenter.X - bigCircleRadius, bigCircleCenter.Y - bigCircleRadius, bigCircleRadius * 2, bigCircleRadius * 2);
            g.FillEllipse(Brushes.LightGray, bigCircleRect);
            g.DrawEllipse(Pens.Black, bigCircleRect);
            // 绘制小圆
            Rectangle smallCircleRect = new Rectangle(smallCircleCenter.X - smallCircleRadius, smallCircleCenter.Y - smallCircleRadius, smallCircleRadius * 2, smallCircleRadius * 2);
            g.FillEllipse(Brushes.White, smallCircleRect);
            g.DrawEllipse(Pens.Black, smallCircleRect);
        }
            private void JoystickControl_MouseDown(object sender, MouseEventArgs e)
            {
                isDragging = true;
                Console.WriteLine("Mouse Down at Small Circle");
            }

            private void JoystickControl_MouseMove(object sender, MouseEventArgs e)
            {
                if (isDragging)
                { 
                    // 计算新位置
                    int newX = e.X;
                    int newY = e.Y;
                    // 计算小圆新位置
                    int distance = (int)Math.Sqrt(Math.Pow(newX - bigCircleCenter.X, 2) + Math.Pow(newY - bigCircleCenter.Y, 2));
                    if (distance <= bigCircleRadius - smallCircleRadius)
                    {
                        smallCircleCenter = new Point(newX, newY);
                        Invalidate();
                    }
                }
            }

            private void JoystickControl_MouseUp(object sender, MouseEventArgs e)
            {
                isDragging = false;
                // 将小圆重置回初始位置
                smallCircleCenter = bigCircleCenter;
                Invalidate();
            }

            public void UpdateJoystickPosition(int deltaX, int deltaY)
            {
               
                // 根据手柄输入更新小圆位置
               
                  int newX = smallCircleCenter.X + deltaX;
                  int newY = smallCircleCenter.Y + deltaY;

                  // 确保小圆在大圆内部移动
                  int distance = (int)Math.Sqrt(Math.Pow(newX - bigCircleCenter.X, 2) + Math.Pow(newY - bigCircleCenter.Y, 2));
                  if (distance <= bigCircleRadius - smallCircleRadius)
                  {
                      smallCircleCenter = new Point(newX, newY);
                      Invalidate(); // 重绘控件
                  }
            }
        }
        #endregion
        
        #region 版本更新
        private async void updataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "检查更新中...";
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            try
            {
                string latestVersion = await Task.Run(() => GetLatestReleaseVersionAsync("qizhoward", "GamePad"));
                string localVersion = GetLocalVersion();

                if (!string.IsNullOrEmpty(latestVersion))
                {
                    toolStripStatusLabel1.Text = $"最新版本: {latestVersion}";
                    if (IsNewerVersion(localVersion, latestVersion))
                    {
                        toolStripStatusLabel1.Text = $"发现新版本: {latestVersion}，正在下载...";
                        await DownloadLatestReleaseAsync("qizhoward", "GamePad", latestVersion);
                        toolStripStatusLabel1.Text = "下载完成，请安装新版本。";
                    }
                    else
                    {
                        toolStripStatusLabel1.Text = "当前已是最新版本。";
                    }
                }
                else
                {
                    toolStripStatusLabel1.Text = "检查更新失败";
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = $"检查更新失败: {ex.Message}";
            }

            toolStripProgressBar1.Style = ProgressBarStyle.Blocks;


        }
        private async Task<string> GetLatestReleaseVersionAsync(string owner, string repo)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("request"); // GitHub API 需要用户代理

                string url = $"https://api.github.com/repos/qizhoward/GamePad/releases/latest";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);

                return json["tag_name"]?.ToString();
            }
        }
        private string GetLocalVersion()
        {
            // 获取当前执行的程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 获取程序集版本信息
            var version = assembly.GetName().Version;

            return version.ToString();
        }
        private bool IsNewerVersion(string localVersion, string latestVersion)
        {
            Version local = new Version(localVersion);
            Version latest = new Version(latestVersion);

            return latest.CompareTo(local) > 0;
        }
        private async Task DownloadLatestReleaseAsync(string owner, string repo, string version)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("request"); // GitHub API 需要用户代理

                string url = $"https://api.github.com/repos/qizhoward/GamePad/releases/tags/{version}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);

                string downloadUrl = json["assets"]?[0]?["browser_download_url"]?.ToString();
                if (downloadUrl != null)
                {
                    HttpResponseMessage downloadResponse = await client.GetAsync(downloadUrl);
                    downloadResponse.EnsureSuccessStatusCode();

                    byte[] fileBytes = await downloadResponse.Content.ReadAsByteArrayAsync();

                    // 保存文件
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GamePad_latest_release.exe");
                    await Task.Run(() => File.WriteAllBytes(filePath, fileBytes));
                    // 提示保存路径（可选）
                    MessageBox.Show($"新版本已下载到 {filePath}", "下载完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion
        
        private void LoadImage()
        {
            // 设置图片的路径
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "20240525a.png");
               
            // 加载图片
            pictureBox1.Image = System.Drawing.Image.FromFile(imagePath);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // 根据需要调整
            pictureBox1.BackColor = Color.DimGray;

        }

        #region 亚克力
        private void InitializeAcrylicEffect()
        {   
            // 初始化Guna2Panel
            acrylicPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent,
                FillColor = System.Drawing.Color.FromArgb(128, 255, 255, 255), // 半透明白色
                BorderColor = Color.Transparent,
                ShadowDecoration = { Parent = acrylicPanel }
            };

            pictureBox1.Controls.Add(acrylicPanel);
            acrylicPanel.BringToFront();
        }
        #endregion

        #region 菜单按钮
        private void InitializeSharedMenu()
        {
            // 创建共享菜单对象
            sharedMenu = new ContextMenuStrip();
            // 创建文本输入框
            textBox = new ToolStripTextBox();
            textBox.AutoSize = false;
            textBox.Width = 100;
            
            // 添加菜单项
            ToolStripMenuItem menuItem1 = new ToolStripMenuItem("显示:");
            ToolStripMenuItem menuItem2 = new ToolStripMenuItem("键盘设置");
            ToolStripMenuItem menuItem3 = new ToolStripMenuItem("鼠标移动");
            ToolStripMenuItem menuItem4 = new ToolStripMenuItem("鼠标点击");
            ToolStripSeparator separator1 = new ToolStripSeparator(); // 添加分隔符
            ToolStripMenuItem menuItem5 = new ToolStripMenuItem("震动");
            ToolStripMenuItem menuItem6 = new ToolStripMenuItem("按放切换");
            ToolStripSeparator separator2 = new ToolStripSeparator(); // 添加分隔符
            ToolStripMenuItem menuItem7 = new ToolStripMenuItem("复制键");
            ToolStripMenuItem menuItem8 = new ToolStripMenuItem("粘贴键");
            ToolStripMenuItem menuItem9 = new ToolStripMenuItem("清空键");
            menuItem1.DropDownItems.Add(textBox);
            menuItem1.Click += MenuItem_Click;
            menuItem2.Click += MenuItem2_Click;
            menuItem3.Click += MenuItem_Click;
            menuItem4.Click += MenuItem_Click;
            menuItem5.Click += MenuItem5_Click;
            menuItem6.Click += MenuItem_Click;
            menuItem7.Click += MenuItem7_Click;
            menuItem8.Click += MenuItem8_Click;
            menuItem9.Click += MenuItem9_Click;
            sharedMenu.Items.Add(menuItem1);
            sharedMenu.Items.Add(menuItem2);
            sharedMenu.Items.Add(menuItem3);
            sharedMenu.Items.Add(menuItem4);
            sharedMenu.Items.Add(separator1); // 添加分隔符
            sharedMenu.Items.Add(menuItem5);
            sharedMenu.Items.Add(menuItem6);
            sharedMenu.Items.Add(separator2); // 添加分隔符
            sharedMenu.Items.Add(menuItem7);
            sharedMenu.Items.Add(menuItem8);
            sharedMenu.Items.Add(menuItem9);
            // 将菜单与所有按钮关联
          
            button1.MouseDown += Button_MouseDown;
            button2.MouseDown += Button_MouseDown;
            button3.MouseDown += Button_MouseDown;
            button4.MouseDown += Button_MouseDown;
            button5.MouseDown += Button_MouseDown;
            button6.MouseDown += Button_MouseDown;
            button7.MouseDown += Button_MouseDown;
            button8.MouseDown += Button_MouseDown;
            button9.MouseDown += Button_MouseDown;
            button10.MouseDown += Button_MouseDown;
            button11.MouseDown += Button_MouseDown;
            button12.MouseDown += Button_MouseDown;
            button13.MouseDown += Button_MouseDown;
            button14.MouseDown += Button_MouseDown;
            button15.MouseDown += Button_MouseDown;
            button16.MouseDown += Button_MouseDown;
            button17.MouseDown += Button_MouseDown;
            button18.MouseDown += Button_MouseDown;
            button19.MouseDown += Button_MouseDown;
            button20.MouseDown += Button_MouseDown;
            button21.MouseDown += Button_MouseDown;
            button22.MouseDown += Button_MouseDown;
            button23.MouseDown += Button_MouseDown;
            button24.MouseDown += Button_MouseDown;
            button25.MouseDown += Button_MouseDown;
            button26.MouseDown += Button_MouseDown;
            button27.MouseDown += Button_MouseDown;
            button28.MouseDown += Button_MouseDown;
            button29.MouseDown += Button_MouseDown;
            button30.MouseDown += Button_MouseDown;
            button31.MouseDown += Button_MouseDown;
            // 监听文本输入框内容变化事件
            this.textBox.KeyDown += TextBox_KeyDown;
        }
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                currentButton = sender as System.Windows.Forms.Button;
                Point cursorPosition = System.Windows.Forms.Cursor.Position;
                sharedMenu.Show(cursorPosition);
            }
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // 检查是否按下了 Enter 键
            if (e.KeyCode == Keys.Enter)
            {
                // 更新当前按钮的文本为文本框中的内容
                if (currentButton != null)
                {
                    currentButton.Text = textBox.Text;
                }
                // 防止 Enter 键被控件默认处理
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
      
        private void MenuItem_Click(object sender, EventArgs e)
        {
            // 处理菜单项点击事件
            MessageBox.Show(((ToolStripMenuItem)sender).Text);
        }

        private void MenuItem2_Click(object sender, EventArgs e)
        {
            KeyboardSetting keyboardSetting = new KeyboardSetting();
            keyboardSetting.ShowDialog();
        }
        private void MenuItem5_Click(object sender, EventArgs e)
        {
            // 获取触发事件的菜单项
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            // 切换菜单项的选中状态
            menuItem.Checked = !menuItem.Checked;
        }
        private void MenuItem9_Click(object sender, EventArgs e)
        {
            // 弹出对话框询问用户是否清除内容
            DialogResult result = MessageBox.Show("是否清除内容？", "清空确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            // 如果用户点击 "是"，则清空文本框中的内容
            if (result == DialogResult.Yes)
            {
                currentButton.Text = string.Empty;
            }
        }
        private void MenuItem7_Click(object sender, EventArgs e)
        {
            if (currentButton != null)
            {
                Clipboard.SetText(currentButton.Text);
            }
        }
        private void MenuItem8_Click(object sender, EventArgs e)
        {
            if (currentButton != null && Clipboard.ContainsText())
            {
                currentButton.Text = Clipboard.GetText();
            }
        }
        #endregion

        List<PictureBox> pictureBoxes = new List<PictureBox>();
        ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
        int pictureBoxCount = 0;
        int iconsInCurrentRow = 0;
        int iconWidth = 60; // 图标宽度
        int iconHeight = 60; // 图标高度
        int iconSpacing = 10; // 图标间距
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件选择器的属性
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件 (*.*)|*.*|可执行文件 (*.exe)|*.exe|快捷方式 (*.lnk)|*.lnk";
            openFileDialog.Multiselect = false; // 只允许选择一个文件

            // 打开文件选择器并检查用户是否选择了文件
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件路径并进行处理
                string selectedFilePath = openFileDialog.FileName;

                // 在这里可以使用选定的文件路径进行后续操作，例如加载图片到 pictureBox 中等等
                Icon fileIcon = Icon.ExtractAssociatedIcon(selectedFilePath);
                if (fileIcon != null)
                {
                    // 将图标转换为位图
                    Bitmap bitmap = fileIcon.ToBitmap();

                    // 创建一个新的PictureBox控件
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Size = new Size(60, 60);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Image = bitmap;

                    /*int newX = pictureBox2.Location.X + pictureBox2.Width + 10; // 加上一些间隔
                      int newY = pictureBox2.Location.Y + (pictureBoxCount * (pictureBox.Height + 10)); // 根据控件数量计算Y位置
                    */
                    int newX;
                    int newY;
                    ///换行
                    if (iconsInCurrentRow >= 4)
                    {
                        // 换行
                        newX = pictureBox2.Location.X; // 放在第一列
                        newY = pictureBox2.Location.Y + iconHeight + iconSpacing; // 放在下一行
                        iconsInCurrentRow = 0; // 重置图标数量
                    }
                    else
                    {
                        // 不换行
                        newX = pictureBox2.Location.X + (iconWidth + iconSpacing) * iconsInCurrentRow;
                        newY = pictureBox2.Location.Y + (iconHeight + iconSpacing) * (iconsInCurrentRow / 4); // 更新 Y 坐标以实现换行
                    }
                    ///

                    // 设置新PictureBox的位置
                    pictureBox.Location = new Point(newX, newY);

                    // 将新的PictureBox添加到panel1中
                    panel1.Controls.Add(pictureBox);
                    pictureBox.Click += PictureBox_Click;
                    // 添加右键菜单
                    pictureBox.ContextMenuStrip = contextMenuStrip;

                    // 保存程序路径
                    pictureBox.Tag = selectedFilePath;
                    // 将新的PictureBox添加到集合中
                    pictureBoxes.Add(pictureBox);
                    //pictureBoxCount++;
                    iconsInCurrentRow++;
                    // 调整已有图标的位置
                    //AdjustPictureBoxesPosition();
                }
                else
                {
                    // 如果无法获取文件的图标，则显示默认图标或者给出提示
                    // 这里可以根据实际情况进行处理
                }

            }
        }
        private void AdjustPictureBoxesPosition()
        {
            int x = pictureBox2.Location.X + pictureBox2.Width + 10; // 初始 x 位置为第一个图标的右边加上间距
            int y = pictureBox2.Location.Y; // y 位置与第一个图标一致

            // 遍历已添加的 PictureBox 控件，调整它们的位置
            foreach (PictureBox pb in pictureBoxes)
            {
                pb.Location = new Point(x, y);
                x += pb.Width + 10; // 更新下一个 PictureBox 的 x 位置，考虑到间距
            }
        }
        //“删除”菜单
        private void InitializeContextMenuStrip()
        {
            // 创建右键菜单项
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("删除");
            deleteMenuItem.Click += DeleteMenuItem_Click;

            // 将菜单项添加到右键菜单
            contextMenuStrip.Items.Add(deleteMenuItem);
        }
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            // 获取触发右键菜单事件的菜单项
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                // 获取与菜单项关联的右键菜单
                ContextMenuStrip menu = menuItem.Owner as ContextMenuStrip;
                if (menu != null)
                {
                    // 获取触发右键菜单事件的PictureBox控件
                    PictureBox pictureBox = menu.SourceControl as PictureBox;
                    if (pictureBox != null)
                    {
                        // 从panel1中移除PictureBox控件
                        panel1.Controls.Remove(pictureBox);

                        // 进行其他清理操作，例如删除关联的文件等等

                        // 从集合中移除对应的PictureBox控件
                        pictureBoxes.Remove(pictureBox);
                    }
                }
            }
        }
      
        private void PictureBox_Click(object sender, EventArgs e)
        {
                // 获取点击的PictureBox控件
                PictureBox pictureBox = sender as PictureBox;

                // 获取程序路径
                string programPath = pictureBox.Tag.ToString();

                // 启动程序
                System.Diagnostics.Process.Start(programPath);
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/qizhoward/GamePad";

            try
            {
                // 使用默认浏览器打开URL
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // 如果出现错误，显示错误消息
                MessageBox.Show("无法打开浏览器: " + ex.Message);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.InitialDirectory = System.Windows.Forms.Application.ExecutablePath;
            openFile.Filter =
                "xml files (*.xml)|*.xml|" +
                "txt files (*.txt)|*.txt|" +
                "Game project files(*.Game)| *.game |" +
                "OK family files(*.ok)|*.ok |" +
                "YuPeng family template files(*.yupeng) | *.yupeng |" +
                "All files(=.=) | *.* ";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fullName = openFile.FileName;
                string fileName = Path.GetFileName(fullName);

            }
            string filePath = @"C:\Users\Administrator\Documents\settings.xml";
            if (File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    AppSettings settings = (AppSettings)serializer.Deserialize(reader);

                    Console.WriteLine("Setting1: " + settings.Name);
                    Console.WriteLine("Setting2: " + settings.Text);
                    Console.WriteLine("Setting4: " + settings.Note);
                    Console.WriteLine("Setting3: " + settings.Setting3);
                }
            }
            else
            {
                Console.WriteLine("设置文件不存在。");
            }
        }
       
        /// <summary>
        /// Save As 另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // 设置保存文件的筛选器
            saveFileDialog.Filter = "YuPeng family template files(*.yupeng)|*.yupeng|Text Files (*.txt)|*.txt|Game project files(*.Game)|*.game|All Files (*.*)|*.*";
            // 可以添加更多的筛选器，以支持不同类型的文件格式，格式为："描述1|扩展名1|描述2|扩展名2|..."

            // 设置默认的文件名
            saveFileDialog.FileName = "filenameVer2024.yupeng";

            // 显示对话框，并检查用户是否点击了保存按钮
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件名
                string filePath = saveFileDialog.FileName;
                try
                {

                    // 执行保存文件的逻辑
                    // 这里只是一个示例，实际保存文件的逻辑需要根据你的需求来实现
                    File.WriteAllText(filePath, "保存内容");

                    MessageBox.Show("文件保存成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存文件时出现错误：{ex.Message}");
                }
                // 执行保存文件的逻辑，例如保存文件到指定路径
                // 这里只是一个示例，实际保存文件的逻辑需要根据你的需求来实现
                // File.WriteAllText(filePath, "Content of the file");
            }

        }
        /// <summary>
        /// Save 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string filePath = @"C:\Users\Administrator\Documents\settings.xml";

            AppSettings settings = new AppSettings
            {
                Name = "Button23",
                Text = "1",
                Note = "备注",
                Setting3 = true
            };

            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, settings);
            }

            Console.WriteLine("设置已保存到XML文件。");
        }
        /// <summary>
        /// Exit 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Plugins 插件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pluginsPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Plugins plugins = new Plugins();
            plugins.Show();
        }
        /// <summary>
        /// SDK Download 扩展
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/qizhoward/GamePad";

            try
            {
                // 使用默认浏览器打开URL
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // 如果出现错误，显示错误消息
                MessageBox.Show("无法打开浏览器: " + ex.Message);
            }
        }
        /// <summary>
        /// Options 选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            Options options = new Options();
            options.Show(); 
        }
        /// <summary>
        /// CHM 打开帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            string helpFilePath = "C:\\Program Files (x86)\\Gamepad Tester\\Gamepad Tester help.chm";
            Help.ShowHelp(this, helpFilePath);
        }
        /// <summary>
        /// Theme 主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void themesTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTheme == Themes.LightTheme)
            {
                currentTheme = Themes.DarkTheme;
            }
            else
            {
                currentTheme = Themes.LightTheme;
            }

            ApplyTheme(currentTheme);
            // 保存主题选择
            Properties.Settings.Default.Theme = currentTheme == Themes.DarkTheme ? "Dark" : "Light";
            Properties.Settings.Default.Save();
            
        }
        /// <summary>
        /// 录制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recordingRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isRecording = !isRecording; // 切换录制状态

            if (isRecording)
            {
                recordedStates.Clear(); // 清空之前的记录
                ShowToastNotification("开始录制"); // 显示开始录制的气泡提示
            }
            else
            {
                // 在此处可以处理录制完成后的逻辑，比如保存记录数据到文件或者进行分析
                ShowToastNotification("停止录制"); // 显示停止录制的气泡提示
            }
        }
        private void ShowToastNotification(string message)
        {
            // 创建 ToastContentBuilder
            var builder = new ToastContentBuilder()
                .AddText("录制通知")
                .AddText(message);

            // 创建 ToastNotification
            var toast = new ToastNotification(builder.GetToastContent().GetXml());

            // 展示通知
            ToastNotificationManagerCompat.CreateToastNotifier().Show(toast);
        }
        /// <summary>
        /// Customize 自定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            Customize customize = new Customize();
            customize.Show();
        }
        /// <summary>
        /// about 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            About about = new About();
            about.Show();
        }
        /// <summary>
        /// BrushWriting 刷写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void brushWritingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Brush brush = new Brush();
            brush.Show();
        }
    }
}
