#include "power.h"
#include "ui_power.h"


Power::Power(QWidget *parent)
    : QDialog(parent)
    , ui(new Ui::Power)
{
    ui->setupUi(this);
    setWindowIcon(QIcon(":/Gamepadicon3.ico"));
}

Power::~Power()
{
    delete ui;
}

void Power::on_buttonBox_accepted()
{
    emit acceptedSignal();
}

void Power::on_buttonBox_rejected()
{
    emit rejectedSignal();
}
