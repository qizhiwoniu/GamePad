#ifndef OPTION_H
#define OPTION_H

#include <QDialog>
#include <ui_option.h>

namespace Ui {
class Option;
}

class Option : public QDialog
{
    Q_OBJECT

public:
    explicit Option(QWidget *parent = nullptr);
    ~Option();

private:
    Ui::Option *ui;
private slots:
    void saveSettings();
    void applyRegisterCode(const QString& code);  // 注册后显示注册码
    void setAutoStart(bool enable);//开机启动
    bool isAutoStartEnabled() const;//开机启动
    void startMinimized(bool enable);//最小化启动
    void applyStartupSettings(bool enableStartup, bool enableMinimized);
    bool isMinimizedStartup() const;
};

#endif // OPTION_H
