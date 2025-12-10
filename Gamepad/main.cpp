#include "mainwindow.h"
#include <QMenu>
#include <QIcon>
#include <QSystemTrayIcon>
#include <QApplication>
#include <QEvent>
#include <QDebug>
#include <QTimer>
int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    MainWindow w;
    //Qt::WindowFlags flags = Qt::Window |
     //                       Qt::WindowSystemMenuHint |
     //                       Qt::WindowMinimizeButtonHint |
     //                       Qt::WindowCloseButtonHint |
                            //Qt::WindowContextHelpButtonHint;
    //flags &= ~Qt::WindowMaximizeButtonHint;

    //w.setWindowFlags(flags);

    bool startMinimized = false;
    for (int i = 1; i < argc; ++i) {
        if (QString(argv[i]).compare("/minimized", Qt::CaseInsensitive) == 0) {
            startMinimized = true;
            break;
        }
    }
    if (startMinimized) {
        QTimer::singleShot(0, [&]() {
            w.showMinimized();
        }); // 最小化到任务栏或托盘
    }
    else {
        w.show();  // 正常启动
    }
    ///////////////////////////////////系统托盘图标////////////////////////////////////////
    QSystemTrayIcon *trayIcon = new QSystemTrayIcon(&w);
    QIcon icon(":/banlizai.ico"); // 替换为您的图标路径
    trayIcon->setIcon(icon);
    trayIcon->setToolTip("Gamepad");
    trayIcon->setVisible(true);
    //trayIcon->showMessage("提示", "应用已最小化到托盘", QSystemTrayIcon::Information, 3000);
    // 创建右键菜单
    QMenu *menu = new QMenu();
    QAction *showAction = new QAction("打开主页", &w);
    QAction *updataAction = new QAction("检查更新", &w);
    QAction *quitAction = new QAction("退出", &w);
    menu->setStyleSheet(
        "QMenu { background-color: rgba(37,37,38,0.95); color: #e6e6e6; border: 1px solid rgba(60,60,60,0.9); padding: 2px; }"
        "QMenu::item { padding: 4px 10px; }"
        "QMenu::item:selected { background-color: rgba(255,255,255,0.06); color: white; }"
        );
    menu->addAction(showAction);
    menu->addSeparator();
    menu->addAction(updataAction);
    menu->addSeparator();
    menu->addAction(quitAction);
    trayIcon->setContextMenu(menu);
    // 连接事件
    QObject::connect(showAction, &QAction::triggered, &w,[&w](){
        w.showNormal();
        w.activateWindow();});
    QObject::connect(updataAction, &QAction::triggered, []() { });
    QObject::connect(quitAction, &QAction::triggered,&a,&QApplication::quit);
    QObject::connect(trayIcon, &QSystemTrayIcon::activated,&w,[&w](QSystemTrayIcon::ActivationReason reason) {
        if (reason == QSystemTrayIcon::DoubleClick) {
            w.showNormal();
            w.activateWindow();
        }
    });
    trayIcon->show();
    ///////////////////////////////////系统托盘图标////////////////////////////////////////
    w.setTrayIcon(trayIcon);
    return a.exec();
}
