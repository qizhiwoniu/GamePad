#include "customize.h"
#include "ui_customize.h"

Customize::Customize(QWidget *parent)
    : QDialog(parent)
    , ui(new Ui::Customize)
{
    ui->setupUi(this);
    setWindowIcon(QIcon(":/banlizai.ico"));
}

Customize::~Customize()
{
    delete ui;
}
