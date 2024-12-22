功能：</br>
●呼吸LED </br>
● 1 x 摄像头 </br>
● 1 x mic </br>
● 1 x micro usb </br>
● 1 x 重力感应芯片 </br>
● 2 x 差速震动马达 </br>
● 2 x 喇叭 </br>
● 1 x LED ● 20 x 按键（1个复位键+19个功能键）</br>
● 2 x 摇杆（360度带确认键）</br>
● 1 x 1.54寸显示屏（240x240）/ ●LCD 1.44寸/1.8寸 </br>
● 1 x 锂电池（3.7V 1200mAh），板载充电电路 ● 1 x TPYE-C（下载/REPL调试/供电）</br>
●                 1800+mAh    
##
● 主控：STM32F103C8T6 </br>
●AP6212 备ESP8266</br>
A10 TXD </br>
A9 RXD </br>
●stm32f103c8t6怎么通过mini-USB烧录?</br>
##
● 主控：ESP32-S3-WROOM-1 （N8R2; Flash:4MBytes,RAM:2MBytes）支持WiFi/BLE (蓝牙和wifi的共存问题)</br>
* 1 x UART/I2C接口（XH-1.25MM-4P）</br>
* 1 x 拨码开关 </br>
##
chatgpt推荐：</br>
入门级手柄：</br>
MCU：STM32F103 或 RP2040</br>
通信：无</br>
传感器：无</br>
振动：DRV8833</br>
电源管理：TP4056</br>
中高端手柄：</br>
MCU：STM32F411 或 nRF52840</br>
通信：nRF52840（蓝牙）或 ESP32（蓝牙+Wi-Fi）</br>
传感器：MPU-6050 或 BMI160</br>
振动：DRV2605</br>
电源管理：BQ24074</br>
高端手柄（支持所有功能）：</br>
MCU：STM32H7 或 ESP32-S3</br>
通信：nRF52840</br>
传感器：MPU-9250 或 BMI270</br>
振动：DRV2605</br>
电源管理：BQ24074 + AP2112</br>
##
STM32F411CEU6</br>
应用：高性能手柄主控 MCU，支持高级振动算法和多按键处理。</br>
特点：ARM Cortex-M4 内核，100MHz，256KB Flash。</br>
STM32H743VI</br>
应用：用于复杂输入信号处理的高端手柄。</br>
特点：ARM Cortex-M7，480MHz，2MB Flash，支持高速外设。</br>
Nordic Semiconductor</br>
nRF52840-QIAA</br>
应用：无线手柄主控，集成蓝牙 5.0 和 BLE。</br>
特点：ARM Cortex-M4，支持 2.4GHz 通信，低功耗。</br>
nRF5340-QKAA</br>
应用：双核设计适合多任务游戏手柄。</br>
特点：主核 Cortex-M33（128MHz），辅助 Cortex-M33（64MHz）。</br>
##
传感器芯片</br>
Bosch Sensortec</br>
BNO055</br>
应用：9轴 IMU，用于体感手柄或动作捕捉。</br>
特点：集成陀螺仪、加速度计、磁力计和传感器融合算法。</br>
BMI160</br>
应用：6轴 IMU，适合常规手柄的体感功能。</br>
特点：低功耗，高精度。</br>
InvenSense</br>
MPU6050</br>
应用：经典 6轴 IMU，支持体感功能。</br>
特点：陀螺仪 + 加速度计，低延迟。</br>
##
电源管理芯片</br>
Analog Devices</br>
ADP5301</br>
应用：手柄低功耗电源管理。</br>
特点：支持宽电压范围输入，高效升降压。</br>
Texas Instruments</br>
TPS63060</br>
应用：锂电池供电手柄的升降压电源。</br>
特点：高效率、稳定输出。</br>
音频相关芯片</br>
Cirrus Logic</br>
CS42L42</br>
应用：带耳机插孔的高端手柄。</br>
特点：低功耗音频编解码器。</br>
Qualcomm</br>
CSRA64215</br>
应用：无线音频传输手柄。</br>
特点：集成蓝牙音频支持。</br>

