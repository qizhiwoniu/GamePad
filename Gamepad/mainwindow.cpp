#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "option.h"
#include "customize.h"
#include <QLabel>
#include <QVBoxLayout>
#include <QWidget>
#include <QFont>
#include <QTabWidget>
#include <QIcon>
#include <QEvent>
#include <QCloseEvent>
#include <QDebug>
#include <QTimer>
#include <QSystemTrayIcon>
#include <QProgressBar>
#include <QStatusBar>
#include <QVBoxLayout>
#include <QFileDialog>
#include <QProcess>
#include <QDesktopServices>
#include <QUrl>
#include <QFileInfo>
#include <QFile>
#include <QTextStream>
#include <QStringList>
#include <QColorDialog>
#include <QInputDialog>

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    setWindowTitle("Gamepad");
    setWindowIcon(QIcon("banlizai.ico"));
    setWindowFlag(Qt::WindowMaximizeButtonHint, false);
    setFixedSize(720, 480);
    initStatusBar();

   // 字体使用点数，便于高 DPI 下正确缩放
    QFont menuFont("Segoe UI");
    menuFont.setPointSize(9);
    ui->menubar->setFont(menuFont);
    // 不要强行固定为过小的像素高度，使用 padding 控制项高度
    ui->menubar->setMinimumHeight(24);
    // 样式：调整菜单栏项与下拉菜单项的 padding 与选中高亮
    // 设置暗色模式样式表
    ui->menubar->setStyleSheet(
    //    "QMenuBar {background-color: #2E2E2E;color: #FFFFFF;border: 1px solid #444444;}"
        "QMenuBar::item { background: transparent; padding: 2px 8px; margin: 0px; border: none; }"
        "QMenuBar::item:selected { background: rgba(255,255,255,0.04); }"
        "QMenuBar::item:pressed { background: rgba(255,255,255,0.06); }"
        // 下拉菜单：更紧凑的垂直间距，背景也采用深色透明风格
        "QMenu { background-color: rgba(37,37,38,0.95); color: #e6e6e6; border: 1px solid rgba(60,60,60,0.9); padding: 2px; }"
        "QMenu::item { padding: 4px 10px; }"
        "QMenu::item:selected { background-color: rgba(255,255,255,0.06); color: white; }"
        // 尝试减少菜单与菜单栏之间的视觉间距（部分风格可能忽略 margin）
        "QMenuBar::separator { height: 0px; }"
        );
    // 获取 tabWidget（优先 ui 指针，失败则 findChild）
    QTabWidget* tab = nullptr;
    if (!tab && ui->centralwidget) tab = ui->centralwidget->findChild<QTabWidget*>("tabWidget");
    if (tab && ui->centralwidget) {
        tab->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Expanding);

        QLayout* layout = ui->centralwidget->layout();
        if (!layout) {
            auto* v = new QVBoxLayout(ui->centralwidget);
            v->setContentsMargins(0, 0, 0, 0);
            v->setSpacing(0);
            v->addWidget(tab);
            ui->centralwidget->setLayout(v);
            qDebug() << "Created layout and added tabWidget";
        }
        else {
            layout->setContentsMargins(0, 0, 0, 0);
            if (layout->indexOf(tab) == -1) layout->addWidget(tab);
            qDebug() << "Ensured tabWidget is in centralwidget layout";
        }
    }
    else {
        qDebug() << "tabWidget not found. Check .ui name or re-run CMake to regenerate ui_*.h";
    }
    ui->tabWidget->setCurrentWidget(ui->pageHome);

    ui->tabWidget_2->setCurrentWidget(ui->tab_1);
    QFile file(":/Changes.txt");
    if (!file.open(QIODevice::ReadOnly | QIODevice::Text)) {
        ui->textEdit_changes->setPlainText("Cannot open Changes.txt");
    } else {
        ui->textEdit_changes->setPlainText(file.readAll());
        file.close();
    }
    QFile file_2(":/License.txt");
    if (!file_2.open(QIODevice::ReadOnly | QIODevice::Text)) {
        ui->textEdit_changes_2->setPlainText("Cannot open License.txt");
    } else {
        ui->textEdit_changes_2->setPlainText(file_2.readAll());
        file_2.close();
    }
    // 设置文本不可编辑
    ui->textEdit_changes->setReadOnly(true);
    ui->textEdit_changes_2->setReadOnly(true);
    ui->label_7->setTextInteractionFlags(Qt::TextBrowserInteraction);
    ui->label_7->setOpenExternalLinks(true);
    ui->label_8->setTextInteractionFlags(Qt::TextBrowserInteraction);
    ui->label_8->setOpenExternalLinks(true);
    ui->label_4->setScaledContents(true);
    ui->label_4->setAlignment(Qt::AlignCenter);

    QList<QPushButton*> btns = {
        ui->pushButton_28,
        ui->pushButton_27,
        ui->pushButton_17,
        ui->pushButton_16,
        ui->pushButton_18,
        ui->pushButton_19,
        ui->pushButton_5,
        ui->pushButton_4,
        ui->pushButton_2,
        ui->pushButton_3,
        ui->pushButton_9,
        ui->pushButton_11,
        ui->pushButton_8,
        ui->pushButton_10,
        ui->pushButton_12,
        ui->pushButton_13,
        ui->pushButton_14,
        ui->pushButton_15,
        ui->pushButton_35,
        ui->pushButton_36
    };
    for (QPushButton* btn : btns)
    {
        btn->installEventFilter(this);
    }
    breathTimer = new QTimer(this);

    connect(breathTimer, &QTimer::timeout, this, &MainWindow::updateBreathColor);
}

MainWindow::~MainWindow()
{
    delete ui;
}
void MainWindow::initStatusBar()
{
    ui->statusbar->setStyleSheet(""); // 清除样式表
    ui->statusbar->setSizeGripEnabled(false);

    statusLabel = new QLabel("状态", this);
    statusLabel->setMinimumWidth(100);
    ui->statusbar->addWidget(statusLabel, 0);

    progressBar = new QProgressBar(this);
    progressBar->setRange(0, 100);
    progressBar->setValue(0);
    progressBar->setTextVisible(false);
    progressBar->setFixedSize(120, 20);
    progressBar->setStyleSheet(R"(
        QProgressBar {
            border: 1px solid #555;
            border-radius: 5px;
            background-color:#2E2E2E
        }
        QProgressBar::chunk {
            background-color:#00FF00;
            border-radius: 5px;
        }
    )");
    ui->statusbar->addPermanentWidget(progressBar);
}
void MainWindow::setTrayIcon(QSystemTrayIcon *tray)
{
    m_tray = tray;
}
void MainWindow::closeEvent(QCloseEvent *event)
{
    event->ignore();  // 不退出程序
    this->hide();     // 隐藏窗口
    if (m_tray) {
        QTimer::singleShot(500, this, [this]() {
            m_tray->showMessage("提示",
                                "程序已在后台运行。",
                                QSystemTrayIcon::Information,
                                1000);
        });
    }
}
void MainWindow::on_actionExit_triggered()
{
    qApp->quit();
}


void MainWindow::on_pushButton_6_clicked()
{
    QString filter = "Image(*.png *.jpg)";
    QString path = QFileDialog::getOpenFileName(this,"选择图片","",filter);
    if(!path.isEmpty())
    {
        QPixmap pixmap(path);
        ui->label_4->setPixmap(pixmap.scaled(ui->label_4->size(), Qt::KeepAspectRatio, Qt::SmoothTransformation));
    }else{qDebug()<<"未选择";return;}

}
void MainWindow::on_actionoption_triggered()
{
    Option* option = new Option(this);
    option->setAttribute(Qt::WA_DeleteOnClose);
    Qt::WindowFlags flags = Qt::Window | Qt::WindowContextHelpButtonHint | Qt::WindowCloseButtonHint;
    option->setWindowFlags(flags);
    option->setWindowModality(Qt::NonModal);//允许主窗口交互
    option->setWindowFlag(Qt::WindowDoesNotAcceptFocus, true);
    option->setFixedSize(428, 312);
    QPoint parentPos = this->pos();
    int optionWidth = option->width(); // 或自己固定 400
    option->move(parentPos.x() - optionWidth - 10, parentPos.y());
    option->show();
}
void MainWindow::on_pushButton_7_clicked()
{
    QString systemPath = QDir::fromNativeSeparators(
        qgetenv("SystemRoot") + QStringLiteral("\\System32\\joy.cpl")
        );

    QProcess::startDetached("control.exe", { systemPath });
}
void MainWindow::on_actioncustomize_triggered()
{
    Customize* customize = new Customize(this);
    customize->setAttribute(Qt::WA_DeleteOnClose);
    Qt::WindowFlags flags = Qt::Window | Qt::WindowContextHelpButtonHint | Qt::WindowCloseButtonHint;
    customize->setWindowFlags(flags);
    customize->setWindowModality(Qt::NonModal);//允许主窗口交互
    customize->setWindowFlag(Qt::WindowDoesNotAcceptFocus, true);
    customize->setFixedSize(633, 451);
    QPoint parentPos = this->pos(); // 父窗口绝对坐标
    MainWindow mainWindow;
    int customizeWidth = mainWindow.width(); // 父窗口宽度
    customize->move(parentPos.x() + customizeWidth  + 10, parentPos.y());
    customize->show();
}
void MainWindow::on_actionCHM_triggered()
{
    QString filePath = "C:/Program Files/Gamepad/programm/Gamepad help.chm"; // 替换为你的文件路径
    QUrl url = QUrl::fromLocalFile(filePath);
    QDesktopServices::openUrl(url);
}
void MainWindow::on_actiontheme_triggered()
{
    static bool dark = true;
    QPalette palette;

    if (dark)
    {
        // 亮色主题
        palette.setColor(QPalette::Window, QColor("#FFFFFF"));
        palette.setColor(QPalette::WindowText, Qt::black);
        palette.setColor(QPalette::Base, QColor("#FFFFFF"));      // 输入框背景
        palette.setColor(QPalette::Text, Qt::black);
        palette.setColor(QPalette::Button, QColor("#F0F0F0"));
        palette.setColor(QPalette::ButtonText, Qt::black);
        ui->menubar->setStyleSheet(
            "QMenuBar {background-color: #FFFFFF;color: #2E2E2E;border: 0px solid #444444;}"
            "QMenuBar::item { background: transparent; padding: 2px 8px; margin: 0px; border: none; }"
            "QMenuBar::item:selected { background: rgba(255,255,255,0.04); }"
            "QMenuBar::item:pressed { background: rgba(255,255,255,0.06); }"
            // 下拉菜单：更紧凑的垂直间距，背景也采用深色透明风格
            "QMenu { background-color: rgba(37,37,38,0.95); color: #e6e6e6; border: 1px solid rgba(60,60,60,0.9); padding: 2px; }"
            "QMenu::item { padding: 4px 10px; }"
            "QMenu::item:selected { background-color: rgba(255,255,255,0.06); color: white; }"
            // 尝试减少菜单与菜单栏之间的视觉间距（部分风格可能忽略 margin）
            "QMenuBar::separator { height: 0px; }"
            );
        progressBar->setStyleSheet(R"(
        QProgressBar {
            border: 1px solid #555;
            border-radius: 5px;
            background-color:#FFFFFF
        }
        QProgressBar::chunk {
            background-color:#00FF00;
            border-radius: 5px;
        }
    )");
    }
    else
    {
        // 暗色主题
        palette.setColor(QPalette::Window, QColor("#2E2E2E"));
        palette.setColor(QPalette::WindowText, Qt::white);
        palette.setColor(QPalette::Base, QColor("#3A3A3A"));
        palette.setColor(QPalette::Text, Qt::white);
        palette.setColor(QPalette::Button, QColor("#444444"));
        palette.setColor(QPalette::ButtonText, Qt::white);
        ui->menubar->setStyleSheet(
            "QMenuBar {background-color: #2E2E2E;color: #FFFFFF;border: 1px solid #444444;}"
            "QMenuBar::item { background: transparent; padding: 2px 8px; margin: 0px; border: none; }"
            "QMenuBar::item:selected { background: rgba(255,255,255,0.04); }"
            "QMenuBar::item:pressed { background: rgba(255,255,255,0.06); }"
            // 下拉菜单：更紧凑的垂直间距，背景也采用深色透明风格
            "QMenu { background-color: rgba(37,37,38,0.95); color: #e6e6e6; border: 1px solid rgba(60,60,60,0.9); padding: 2px; }"
            "QMenu::item { padding: 4px 10px; }"
            "QMenu::item:selected { background-color: rgba(255,255,255,0.06); color: white; }"
            // 尝试减少菜单与菜单栏之间的视觉间距（部分风格可能忽略 margin）
            "QMenuBar::separator { height: 0px; }"
            );
        progressBar->setStyleSheet(R"(
        QProgressBar {
            border: 1px solid #555;
            border-radius: 5px;
            background-color:#2E2E2E
        }
        QProgressBar::chunk {
            background-color:#00FF00;
            border-radius: 5px;
        }
    )");
    }
    qApp->setPalette(palette);
    dark = !dark;


}


void MainWindow::on_pushButton_22_clicked()
{
    ui->pushButton_22->setText(
        ui->pushButton_22->text() == "OPEN" ? "CLOSE" : "OPEN"
        );
    ui->pushButton_22->setStyleSheet(
        "background-color: transparent;"
        "color: white;"      // 文字颜色
        "border: none;"
        );
}
void MainWindow::on_pushButton_24_clicked()
{
    ui->pushButton_24->setText(
        ui->pushButton_24->text() == "OPEN" ? "CLOSE" : "OPEN"
        );
    ui->pushButton_24->setStyleSheet(
        "background-color: transparent;"
        "color: white;"      // 文字颜色
        "border: none;"
        );
}
void MainWindow::on_pushButton_34_clicked()
{
    ui->pushButton_34->setText(
        ui->pushButton_34->text() == "OPEN" ? "CLOSE" : "OPEN"
        );
    ui->pushButton_34->setStyleSheet(
        "background-color: transparent;"
        "color: white;"      // 文字颜色
        "border: none;"
        );
}
void MainWindow::updateBreathColor()
{
    breathValue += direction * 2;

    if (breathValue >= 1536) {  // 256 * 6 段
        breathValue = 0;
    }

    int r, g, b;
    int v = breathValue;

    if (v < 256) {
        r = 255; g = v; b = 0;
    } else if (v < 512) {
        r = 511 - v; g = 255; b = 0;
    } else if (v < 768) {
        r = 0; g = 255; b = v - 512;
    } else if (v < 1024) {
        r = 0; g = 1023 - v; b = 255;
    } else if (v < 1280) {
        r = v - 1024; g = 0; b = 255;
    } else {
        r = 255; g = 0; b = 1535 - v;
    }

    QColor color(r, g, b);
    // 设置按钮背景
    ui->pushButton_23->setStyleSheet(
        QString("background-color: rgb(%1, %2, %3);")
            .arg(r).arg(g).arg(b)
        );

    // 更新 label RGB
    ui->label_rgb->setText(
        QString("R:%1  G:%2  B:%3")
            .arg(r).arg(g).arg(b)
        );
}
void MainWindow::on_pushButton_23_clicked()
{
    if (breathTimer->isActive()) {
        breathTimer->stop();
        ui->pushButton_23->setText("Start RGB");
    } else {
        breathTimer->start(30); // 越小越快，30 毫秒一次很顺滑
        ui->pushButton_23->setText("Stop RGB");
    }
}




bool MainWindow::eventFilter(QObject *obj, QEvent *event)
{
    // 检查是不是按钮
    QPushButton *btn = qobject_cast<QPushButton*>(obj);
    if (!btn) return false;
    // 右键修改文本
    if (event->type() == QEvent::ContextMenu) // 右键
    {
        bool ok;
        QString text = QInputDialog::getText(
            this,
            "修改按钮文字",
            "请输入新的按钮文字：",
            QLineEdit::Normal,
            btn->text(),
            &ok
            );
        if (ok && !text.isEmpty())
            btn->setText(text);
        return true; // 阻止默认菜单
    }
    // 鼠标按下（移动模式）
    if (moveMode && event->type() == QEvent::MouseButtonPress)
    {
        QMouseEvent *mouseEvent = static_cast<QMouseEvent*>(event);
        if (mouseEvent->button() == Qt::LeftButton)
        {
            movingBtn = btn;
            dragStartPos = mouseEvent->globalPos() - btn->pos();
            return true;
        }
    }
    // 鼠标移动
    if (moveMode && event->type() == QEvent::MouseMove && movingBtn)
    {
        QMouseEvent *mouseEvent = static_cast<QMouseEvent*>(event);
        QPoint newPos = mouseEvent->globalPos() - dragStartPos;
        movingBtn->move(newPos);
        return true;
    }
    // 鼠标释放
    if (moveMode && event->type() == QEvent::MouseButtonRelease && movingBtn)
    {
        movingBtn = nullptr;
        return true;
    }
    return QMainWindow::eventFilter(obj, event);
}
void MainWindow::on_pushButton_clicked()
{
    moveMode = !moveMode;

    if (moveMode)
        ui->pushButton->setText("退出移动模式");
    else
        ui->pushButton->setText("定义按钮位置");
}


void MainWindow::on_pushButton_32_clicked()
{

}

