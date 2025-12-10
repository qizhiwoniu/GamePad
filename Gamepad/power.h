#ifndef POWER_H
#define POWER_H

#include <QDialog>

namespace Ui {
class Power;
}

class Power : public QDialog
{
    Q_OBJECT

public:
    explicit Power(QWidget *parent = nullptr);
    ~Power();
signals:
    void acceptedSignal();
    void rejectedSignal();
private slots:
    void on_buttonBox_accepted();
    void on_buttonBox_rejected();
private:
    Ui::Power *ui;
};

#endif // POWER_H
