#include "option.h"
#include "ui_option.h"
#include <QSettings>
#include <QDir>
#include <QMessageBox>

Option::Option(QWidget *parent)
    : QDialog(parent)
    , ui(new Ui::Option)
{
    ui->setupUi(this);
    setWindowIcon(QIcon(":/banlizai.ico"));

    QSettings option("YuPeng", "Gamepad");
    ui->checkBox->setChecked(option.value("checkBox", true).toBool());
    ui->checkBox_2->setChecked(option.value("checkBox_2", true).toBool());
    bool autoStart = isAutoStartEnabled();
    ui->checkBox_3->setChecked(autoStart);
    ui->checkBox_4->setChecked(isMinimizedStartup());
    ui->checkBox_4->setEnabled(autoStart);
    //—— 监听开机启动（checkBox_3） ——
    connect(ui->checkBox_3, &QCheckBox::toggled, this, [&](bool checked){
        ui->checkBox_4->setEnabled(checked);

        if (!checked)
            ui->checkBox_4->setChecked(false);

        applyStartupSettings(checked, ui->checkBox_4->isChecked());
    });

    //—— 监听最小化启动（checkBox_4） ——
    connect(ui->checkBox_4, &QCheckBox::toggled, this, [&](bool checked){
        applyStartupSettings(ui->checkBox_3->isChecked(), checked);
    });

    ui->textEdit->setPlainText(QString("Gamepad version: %1").arg(APP_VERSION));
    //ui->textEdit->setPlainText("Gaming version 1.0.0");
    ui->textEdit->setEnabled(false);
    // 连接 SAVE 按钮
    QPushButton* saveButton = ui->buttonBox_2->button(QDialogButtonBox::Save);
    connect(saveButton, &QPushButton::clicked, this, [=]() {
        saveSettings();
        this->close();
    });
    // 连接 OK 按钮
    connect(ui->buttonBox, &QDialogButtonBox::accepted, this, [=]() {
        this->close();   // 关闭窗口
    });
}
void Option::saveSettings()
{
    QSettings option("YuPeng", "Gamepad");  // 可自定义公司和应用名

    // 保存 checkBox 状态
    option.setValue("checkBox", ui->checkBox->isChecked());
    option.setValue("checkBox_2",ui->checkBox_2->isChecked());
    option.setValue("checkBox_3", ui->checkBox_3->isChecked());
    option.setValue("checkBox_4", ui->checkBox_4->isChecked());

    // 保存 textEdit 内容
    option.setValue("textEdit", ui->textEdit->toPlainText());
    // TODO: 实际保存逻辑，例如写入文件或配置

    option.sync();
}

void Option::applyStartupSettings(bool enableStartup, bool enableMinimized)
{
    QString key = "Gamepad";
    QString appPath = "\"" + QDir::toNativeSeparators(QCoreApplication::applicationFilePath()) + "\"";

    QSettings reg("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                  QSettings::NativeFormat);

    if (!enableStartup) {
        reg.remove(key);
        qDebug() << "[Startup] removed";
        return;
    }

    QString value = appPath;
    if (enableMinimized)
        value += " /minimized";

    reg.setValue(key, value);

    qDebug() << "[Startup] enabled:" << value;
}
bool Option::isAutoStartEnabled() const
{
    QSettings reg("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                  QSettings::NativeFormat);

    return reg.contains("Gamepad");
}
bool Option::isMinimizedStartup() const
{
    QSettings reg("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                  QSettings::NativeFormat);

    QString val = reg.value("Gamepad", "").toString();
    return val.contains("/minimized", Qt::CaseInsensitive);
}
void Option::startMinimized(bool enable)
{
    QString appName = "Gamepad";
    QString appPath = "\"" + QDir::toNativeSeparators(QCoreApplication::applicationFilePath()) + "\"";

    QSettings reg("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                  QSettings::NativeFormat);

    if (enable) {
        reg.setValue(appName, appPath + " /minimized");
        qDebug() << "startup mini";
    }
    else {
        reg.setValue(appName, appPath);
        qDebug() << "startup normal";
    }
}
void Option::setAutoStart(bool enable)
{
    QString appName = "Gamepad";
    QString appPath = "\"" + QDir::toNativeSeparators(QCoreApplication::applicationFilePath()) + "\"";

    QSettings reg("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                  QSettings::NativeFormat);

    if (enable)
        reg.setValue(appName, appPath);
    else
        reg.remove(appName);
}

// 注册后显示注册码
void Option::applyRegisterCode(const QString& code)
{
    ui->textEdit->setPlainText("注册码: " + code);
}
Option::~Option()
{
    delete ui;
}
