Screen
    
    屏幕型号	分辨率	接口	触控支持	常用触控芯片
    ILI9341 + XPT2046	320x240	SPI	✅ 电阻触控	XPT2046
    ST7789	240x240	SPI	❌ 不带触控（除非扩展）	无
    IPS 2.8寸/3.2寸 TFT	320x240 / 400x240 等	SPI	✅ 电阻触控	XPT2046
    3.5寸 TFT 屏（常见并口）	480x320	并口/SPI	✅ 电阻触控	XPT2046
    更高级别：FT6236 / FT6206	各种分辨率	I2C（电容触控）	✅ 电容触控（更灵敏）	FT6x06 系列

ILI9341 + XPT2046   </br>

屏幕型号：​ILI9341（320x240 分辨率）​</br>

触控芯片：​FT6236 或 FT6206（电容触控，I2C 接口）</br>
以下是 ESP32-S3 与 ILI9341 屏幕及 FT6236 触控芯片的典型连接方式：​</br>
ILI9341 屏幕（SPI 接口）：</br>
MOSI：​连接 ESP32-S3 的 GPIO 11​</br>
MISO：​连接 ESP32-S3 的 GPIO 13​</br>
SCK：​连接 ESP32-S3 的 GPIO 12​</br>
CS：​连接 ESP32-S3 的任意可用 GPIO（例如 GPIO 10）​</br>
DC/RS：​连接 ESP32-S3 的任意可用 GPIO（例如 GPIO 9）​</br>
RESET：​连接 ESP32-S3 的任意可用 GPIO（例如 GPIO 14），或直接接高电平​</br>
FT6236 触控芯片（I2C 接口）：</br>
SDA：​连接 ESP32-S3 的 GPIO 8（或其他支持 I2C 的 GPIO）​</br>
SCL：​连接 ESP32-S3 的 GPIO 18（或其他支持 I2C 的 GPIO）​</br>
INT：​连接 ESP32-S3 的任意可用 GPIO，用于触摸中断（例如 GPIO 7）</br>
