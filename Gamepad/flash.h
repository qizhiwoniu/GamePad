#ifndef FLASH_H
#define FLASH_H

#include <QDialog>

namespace Ui {
class Flash;
}

class Flash : public QDialog
{
    Q_OBJECT

public:
    explicit Flash(QWidget *parent = nullptr);
    ~Flash();

private slots:
    void on_pushButton_clicked();

    void on_pushButton_2_clicked();
    void onComboChanged(int index);
    void onModeChanged(bool checked);
private:
    Ui::Flash *ui;
};

#endif // FLASH_H
