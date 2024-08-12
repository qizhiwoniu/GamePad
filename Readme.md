2024.08.07 21:15 </br>
A Gameboard System Idea </br>

          YuOS
    power by Qizhiwoniu

    time run
    ----------timeline
      |boot
      |Hard disk partition
      |---------system partition (Unable to change)
      |    |
      |    |--------
      |    |    |boot file (driver files)
      |    |    |Functional area definition(File)
      |    |    
      |
      |--------User partition (Partially hidden)
      |    |root file (Super permissions)
      |    |----------
      |    |    |boot up guidance(loading)
      |    |    |----------
      |    |    |    |Run cursor flashing 
      |    |    |    |Building a file system (system go show ui)
      |    |    |    |User store files （Normally accessible）
      |    |    |    |
      |    |    |    
      |    |
      |
      |

*Power-on Self-Test 硬件自检 </br>
*1.检查RAM 2.检查I/O端口 3.检查系统时钟 4.检查EEPROM </br>
*多按键处理：支持多个按钮同时按下（如方向键的组合），确保在快速按键情况下，系统能准确检测到按键组合。</br>
●碳纤维版</br>

●摄像头 </br>
●呼吸LED </br>
●micro usb </br>
●重力感应芯片 </br>

●STM32F103C8T6 </br>
●AP6212 备ESP8266</br>
A10 TXD </br>
A9 RXD </br>
●stm32f103c8t6怎么通过mini-USB烧录?</br>

● 主控：ESP32-S3-WROOM-1 （N8R2; Flash:4MBytes,RAM:2MBytes）支持WiFi/BLE </br>
● 1 x LED ● 13 x 按键（1个复位键+12个功能键）</br>
● 2 x 摇杆（360度带确认键）</br>
● 1 x 1.54寸显示屏（240x240）/ ●LCD 1.44寸 1.8寸 </br>
● 1 x UART/I2C接口（XH-1.25MM-4P）</br>
● 1 x 锂电池（3.7V 1200mAh），板载充电电路 ● 1 x TPYE-C（下载/REPL调试/供电）</br>
● 1 x 拨码开关 </br>
