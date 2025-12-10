#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QIcon>
#include <QSystemTrayIcon>
#include <QMessageBox>
#include <QCloseEvent>
#include <QWidget>
#include <QPainter>
#include <QProgressBar>
QT_BEGIN_NAMESPACE
namespace Ui {
class MainWindow;
}
QT_END_NAMESPACE

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();
    void setTrayIcon(QSystemTrayIcon *tray);
protected:
    void closeEvent(QCloseEvent *event) override;
private slots:
    void initStatusBar();
    void on_actionExit_triggered();
    void on_pushButton_6_clicked();

    void on_actionoption_triggered();

    void on_pushButton_7_clicked();

    void on_actioncustomize_triggered();

    void on_actionCHM_triggered();

    void on_actiontheme_triggered();

    void on_pushButton_22_clicked();

    void on_pushButton_24_clicked();

    void on_pushButton_23_clicked();
    void updateBreathColor();
    void on_pushButton_34_clicked();

    void on_pushButton_clicked();

    void on_pushButton_32_clicked();

private:
    Ui::MainWindow *ui;
    QSystemTrayIcon *m_tray = nullptr;
    QLabel *statusLabel;
    QProgressBar *progressBar;
    QTimer *breathTimer;
    int breathValue = 0;
    int direction = 1;   // 1 变亮，-1 变暗
    bool moveMode = false;        // 是否处于移动模式
    QPushButton *movingBtn = nullptr;
    QPoint dragStartPos;
    bool eventFilter(QObject *obj, QEvent *event) override;
};
#endif // MAINWINDOW_H
