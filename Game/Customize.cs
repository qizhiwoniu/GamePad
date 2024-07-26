using System;
using System.Windows.Forms;
using SharpDX.DirectInput;

namespace Game
{
    public partial class Customize : Form
    {
        private System.Windows.Forms.TrackBar[] trackBars;
        private System.Windows.Forms.TextBox[] textBoxes;
        private DirectInput directInput;
        private Joystick joystick;
        private Timer timer;

        public Customize()
        {
            InitializeComponent();

            InitializeJoystick();
            timer = new Timer();
            timer.Interval = 100; // Update every 100 ms
            timer.Tick += Timer_Tick;
            timer.Start(); 
        }


        private void InitializeJoystick()
        {
            try
            {
                directInput = new DirectInput();
                var joystickGuid = Guid.Empty;

                foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                {
                    joystickGuid = deviceInstance.InstanceGuid;
                    break;
                }

                // If Gamepad not found, look for a Joystick
                if (joystickGuid == Guid.Empty)
                {
                    foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    {
                        joystickGuid = deviceInstance.InstanceGuid;
                        break;
                    }
                }

                if (joystickGuid == Guid.Empty)
                {
                    MessageBox.Show("No joystick/Gamepad found.");
                    return;
                }

                joystick = new Joystick(directInput, joystickGuid);
                joystick.Properties.BufferSize = 128;
                joystick.Acquire();
                Console.WriteLine("Joystick acquired: " + joystick.Information.ProductName);
                foreach (var deviceObject in joystick.GetObjects())
                {
                    Console.WriteLine($"Object type: {deviceObject.ObjectType} - Name: {deviceObject.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing joystick: {ex.Message}");
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (joystick == null)
            {
                Console.WriteLine("Joystick not initialized.");
                return;
            }

            try
            {
                joystick.Poll();
                var state = joystick.GetCurrentState();
                var data = joystick.GetBufferedData();
                // Print debug info
                Console.WriteLine($"X Axis Value: {state.X}");
                Console.WriteLine($"Y Axis Value: {state.Y}");

                foreach (var stateItem in data)
                {
                    Console.WriteLine($"Buffered: {stateItem.Offset} = {stateItem.Value}");
                }
                // Update the UI
                trackBar1.Value = state.X;
                textBox1.Text = trackBar1.Value.ToString();
                trackBar2.Value = state.Y;
                textBox2.Text = trackBar2.Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading joystick state: {ex.Message}");
            }
        }






        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
            textBox2.Text = trackBar2.Value.ToString();
            textBox3.Text = trackBar3.Value.ToString();
            textBox4.Text = trackBar4.Value.ToString();
            textBox5.Text = trackBar5.Value.ToString();
            textBox6.Text = trackBar6.Value.ToString();
        }
    }
}
