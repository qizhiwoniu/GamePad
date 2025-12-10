#ifndef CUSTOMIZE_H
#define CUSTOMIZE_H

#include <QDialog>

namespace Ui {
class Customize;
}

class Customize : public QDialog
{
    Q_OBJECT

public:
    explicit Customize(QWidget *parent = nullptr);
    ~Customize();

private:
    Ui::Customize *ui;
};

#endif // CUSTOMIZE_H
