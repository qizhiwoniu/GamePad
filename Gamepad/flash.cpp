#include "flash.h"
#include "ui_flash.h"
#include <QMenuBar>
#include <QFileDialog>
#include <QDir>
#include <QStringList>
#include <QProcess>

Flash::Flash(QWidget *parent)
    : QDialog(parent)
    , ui(new Ui::Flash)
{
    ui->setupUi(this);
    QVBoxLayout *layout = new QVBoxLayout(this);

    QMenuBar *menuBar = new QMenuBar(this);
    QFont menuFont("Segoe UI");
    menuFont.setPointSize(9);
    menuBar->setFont(menuFont);
    // 不要强行固定为过小的像素高度，使用 padding 控制项高度
    menuBar->setMinimumHeight(24);
    // 样式：调整菜单栏项与下拉菜单项的 padding 与选中高亮
    // 设置暗色模式样式表
    menuBar->setStyleSheet(
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
    QMenu *fileMenu = menuBar->addMenu("File(F)");
    QAction *exitAction =fileMenu->addAction("Exit");
    QMenu *CMenu = menuBar->addMenu("Configuration(C)");
    CMenu->addAction("Configuration");
    QMenu *DMenu = menuBar->addMenu("Driver(D)");
    QMenu *HelpMenu = menuBar->addMenu("Help(H)");
    HelpMenu->addAction("Check Updata");
    layout->setMenuBar(menuBar);

    connect(exitAction, &QAction::triggered, this, &QDialog::close);
    connect(ui->comboBox, &QComboBox::currentIndexChanged,
            this, &Flash::onComboChanged);
    connect(ui->radioButton, &QRadioButton::toggled,
            this, &Flash::onModeChanged);
    connect(ui->radioButton_2, &QRadioButton::toggled,
            this, &Flash::onModeChanged);
    connect(ui->radioButton_3, &QRadioButton::toggled,
            this, &Flash::onModeChanged);
}

Flash::~Flash()
{
    delete ui;
}

void Flash::on_pushButton_clicked()
{
    QString dir = QFileDialog::getExistingDirectory(this,QStringLiteral("选择文件夹"),"", QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks);

    if (!dir.isEmpty())
    {
        ui->lineEdit->setText(dir);
        // ===== 清空下拉框 =====
        ui->comboBox->clear();

        // ===== 扫描bat文件 =====
        QDir directory(dir);

        // 只筛选 .bat 文件
        QStringList filters;
        filters << "*.bat";

        QFileInfoList fileList = directory.entryInfoList(filters, QDir::Files);

        // ===== 加入到QComboBox =====
        for (const QFileInfo &fileInfo : std::as_const(fileList))
        {
            QString name = fileInfo.fileName();

            QString mode = "other";

            if (name.contains("flash_all_lock"))
                mode = "lock";
            else if (name.contains("flash_all_except"))
                mode = "except";
            else if (name.contains("flash_all"))
                mode = "all";

            // 显示名字 + 存 mode + 存路径
            int index = ui->comboBox->count();

            ui->comboBox->addItem(name);
            ui->comboBox->setItemData(index, mode);                    // 模式
            ui->comboBox->setItemData(index, fileInfo.absoluteFilePath(),
                                      Qt::UserRole + 1);             // 路径
        }
        // ===== 没有文件提示 =====
        if (fileList.isEmpty())
        {
            ui->comboBox->addItem("未找到bat文件");
        }
    }
}

void Flash::on_pushButton_2_clicked()
{
    QString batPath = ui->comboBox->currentData(Qt::UserRole + 1).toString();

    if (batPath.isEmpty())
        return;

    QProcess::startDetached("cmd.exe", QStringList() << "/c" << batPath);
}
void Flash::onComboChanged(int index)
{
    if (index < 0) return;

    QSignalBlocker b1(ui->radioButton);
    QSignalBlocker b2(ui->radioButton_2);
    QSignalBlocker b3(ui->radioButton_3);

    QString mode = ui->comboBox->itemData(index).toString();

    ui->radioButton->setChecked(mode == "all");
    ui->radioButton_2->setChecked(mode == "except");
    ui->radioButton_3->setChecked(mode == "lock");
}
void Flash::onModeChanged(bool checked)
{
    if (!checked) return;

    QSignalBlocker b(ui->comboBox);

    QString mode;

    if (ui->radioButton->isChecked())
        mode = "all";
    else if (ui->radioButton_2->isChecked())
        mode = "except";
    else if (ui->radioButton_3->isChecked())
        mode = "lock";

    for (int i = 0; i < ui->comboBox->count(); i++)
    {
        if (ui->comboBox->itemData(i).toString() == mode)
        {
            ui->comboBox->setCurrentIndex(i);
            break;
        }
    }
}
