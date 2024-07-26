using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
//using Nefarius.ViGEm.Client;
//using Nefarius.ViGEm.Client.Targets;
//using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;

namespace Game
{
    internal static class Program
    { 
        // 定义XInput API的常量和结构
        [StructLayout(LayoutKind.Sequential)]
        public struct XInputState
        {
            public uint dwPacketNumber;
            public XInputGamepad Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XInputGamepad
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
        public static extern uint XInputGetState(uint dwUserIndex, ref XInputState pState);

        private static DirectInput directInput;
        private static Joystick joystick;
        private static JoystickState joystickState;
        //private static IXbox360Controller xbox360Controller;
        private static int sensitivityFactor = 10000;
        //private static JoystickControl joystickControl; // 你可以定义一个自定义的控制类
       
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           

            // 设置默认文化
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            //using (var client = new ViGEmClient())
            {
                //xbox360Controller = client.CreateXbox360Controller();
                //xbox360Controller.Connect();

                Console.WriteLine("Virtual Xbox 360 controller connected.");

                // 检查物理Xbox控制器连接状态
                //CheckXboxControllerConnection();

                // 发送初始状态以触发系统识别
               // SendInitialState(xbox360Controller);

                // 模拟按下 A 键
               // xbox360Controller.SetButtonState(Xbox360Button.A, true);
               // Console.WriteLine("A button pressed.");
                
                // 初始化摇杆
                //InitializeJoystick();
              

                Application.Run(new Form1());
            }   
        } 
        /*private static void CheckXboxControllerConnection()
            {
                XInputState state = new XInputState();
                if (XInputGetState(0, ref state) == 0) // 0表示ERROR_SUCCESS
                {
                    Console.WriteLine("Controller connected");
                }
                else
                {
                    Console.WriteLine("Controller not connected");
                }
            }
        private static void SendInitialState(IXbox360Controller xbox360Controller)
        {
            // 初始化所有按钮和轴的状态
            xbox360Controller.SetButtonState(Xbox360Button.A, false);
            xbox360Controller.SetButtonState(Xbox360Button.B, false);
            xbox360Controller.SetButtonState(Xbox360Button.X, false);
            xbox360Controller.SetButtonState(Xbox360Button.Y, false);

            // 发送初始报告
            xbox360Controller.SubmitReport();

            // 模拟一些输入变化以触发系统识别
            xbox360Controller.SetButtonState(Xbox360Button.A, true);
            xbox360Controller.SubmitReport();

            xbox360Controller.SetButtonState(Xbox360Button.A, false);
            xbox360Controller.SubmitReport();
        }
        private static void InitializeJoystick()
        {
            directInput = new DirectInput();

            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
            {
                joystickGuid = deviceInstance.InstanceGuid;
            }

            if (joystickGuid == Guid.Empty)
            {
                MessageBox.Show("未找到连接的手柄！");
                return;
            }

            joystick = new Joystick(directInput, joystickGuid);
            joystick.Acquire();

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick; // 添加事件处理程序
            timer.Start();
        }
        private static async void Timer_Tick(object sender, EventArgs e)
        {
            // 异步执行手柄输入检查
            //await CheckJoystickAsync();

            // 异步执行 Xbox 控制器状态更新
            //await UpdateControllerAsync(xbox360Controller);
        }
        // 异步执行手柄输入检查
        private static async Task CheckJoystickAsync()
        {

            await Task.Run(() =>
            {
                joystick.Poll();
                joystickState = joystick.GetCurrentState();
                //ProcessJoystickInput(joystickState);
            });
        } 
        // 异步执行 Xbox 控制器状态更新
        private static async Task UpdateControllerAsync(IXbox360Controller xbox360Controller)
        {
            await Task.Run(() => SimulateControllerActivity(xbox360Controller));

          
        }
        */
        // 示例：假设手柄的第一个轴控制 X 轴，第二个轴控制 Y 轴
        /*private static void ProcessJoystickInput(JoystickState state)
        {
            //int deltaX = state.X / sensitivityFactor;
            //int deltaY = state.Y / sensitivityFactor;

            // 确保 joystickControl 不为 null
            if (joystickControl != null)
            {
                // 更新 JoystickControl 中小圆的位置
                joystickControl.UpdateJoystickPosition(deltaX, deltaY);
            }
            

        }*/
     
/*      static void SimulateControllerActivity(IXbox360Controller xbox360Controller)
        {
            // 模拟控制器状态变化
            int leftThumbX = 0, leftThumbY = 0, rightThumbX = 0, rightThumbY = 0;
            byte leftTrigger = 0, rightTrigger = 0;
            bool buttonAState = false;

            for (int i = 0; i < 10; i++)
            {
                // 更新控制器状态
                leftThumbX += 100;
                leftThumbY -= 50;
                rightThumbX -= 50;
                rightThumbY += 100;
                leftTrigger += 5;
                rightTrigger -= 5;
                buttonAState = !buttonAState;

                UpdateControllerState(xbox360Controller, leftThumbX, leftThumbY, rightThumbX, rightThumbY, leftTrigger, rightTrigger, buttonAState);

                // 提交报告以更新状态
                xbox360Controller.SubmitReport();
            }
        }
        static void UpdateControllerState(IXbox360Controller xboxController, int leftThumbX, int leftThumbY, int rightThumbX, int rightThumbY, byte leftTrigger, byte rightTrigger, bool buttonAState)
        {
            xboxController.SetAxisValue(Xbox360Axis.LeftThumbX, (short)leftThumbX);
            xboxController.SetAxisValue(Xbox360Axis.LeftThumbY, (short)leftThumbY);
            xboxController.SetAxisValue(Xbox360Axis.RightThumbX, (short)rightThumbX);
            xboxController.SetAxisValue(Xbox360Axis.RightThumbY, (short)rightThumbY);
            xboxController.SetSliderValue(Xbox360Slider.LeftTrigger, leftTrigger);
            xboxController.SetSliderValue(Xbox360Slider.RightTrigger, rightTrigger);
            xboxController.SetButtonState(Xbox360Button.A, buttonAState);
        }
   */ }
}
