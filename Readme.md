2024.08.07 21:15 </br>
A Gameboard System Idea </br>

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