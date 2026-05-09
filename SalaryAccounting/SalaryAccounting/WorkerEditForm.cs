using SalaryAccounting.Models;
using System;
using System.Windows.Forms;

namespace SalaryAccounting
{
    public partial class WorkerEditForm : Form
    {
        public Worker? ResultWorker { get; private set; }
        private Guid? _editingId;

        private ComboBox cbType = null!;
        private TextBox txtLastName = null!;
        private TextBox txtFirstName = null!;
        private TextBox txtPosition = null!;
        private TextBox txtBaseSalary = null!;
        private TextBox txtBonus = null!;

        public WorkerEditForm(Worker? worker = null)
        {
            InitializeComponent();
            BuildLayout();

            if (worker != null)
            {
                _editingId = worker.Id;
                LoadWorkerData(worker);
                this.Text = "Редагування працівника";
                cbType.Enabled = false; // Тип контракту зазвичай не змінюють після створення
            }
        }

        private void LoadWorkerData(Worker worker)
        {
            txtLastName.Text = worker.LastName;
            txtFirstName.Text = worker.FirstName;
            txtPosition.Text = worker.Position;
            txtBaseSalary.Text = worker.BaseSalary.ToString();
            txtBonus.Text = worker.Bonus.ToString();
            cbType.SelectedIndex = worker is FopWorker ? 0 : 1;
        }

        private void BuildLayout()
        {
            this.Text = "Дані працівника";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Використовуємо сітку для чіткого позиціонування без накладання
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(20),
                BackColor = Color.WhiteSmoke
            };

            // Налаштування колонок: 40% на підписи, 60% на поля вводу
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            // Ініціалізація контролів
            cbType = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cbType.Items.AddRange(new string[] { "ФОП", "Штатний працівник" });
            cbType.SelectedIndex = 0;

            txtLastName = new TextBox { Dock = DockStyle.Fill };
            txtFirstName = new TextBox { Dock = DockStyle.Fill };
            txtPosition = new TextBox { Dock = DockStyle.Fill };
            txtBaseSalary = new TextBox { Dock = DockStyle.Fill, Text = "0" };
            txtBonus = new TextBox { Dock = DockStyle.Fill, Text = "0" };

            Button btnSave = new Button
            {
                Text = "Зберегти",
                Dock = DockStyle.Fill,
                Height = 20,
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            // Додавання на форму
            AddRow(layout, "Тип контракту:", cbType, 0);
            AddRow(layout, "Прізвище:", txtLastName, 1);
            AddRow(layout, "Ім'я:", txtFirstName, 2);
            AddRow(layout, "Посада:", txtPosition, 3);
            AddRow(layout, "Базовий оклад:", txtBaseSalary, 4);
            AddRow(layout, "Додаткова премія:", txtBonus, 5);

            layout.Controls.Add(btnSave, 1, 6);

            this.Controls.Add(layout);
        }

        private void AddRow(TableLayoutPanel panel, string labelText, Control control, int row)
        {
            panel.Controls.Add(new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            panel.Controls.Add(control, 1, row);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (ValidateInputs())
                {
                    ResultWorker = cbType.SelectedIndex == 0 ? new FopWorker() : new FixedPayWorker();

                    if (_editingId.HasValue)
                    {
                        ResultWorker.Id = _editingId.Value; // Зберігаємо існуючий ID при редагуванні
                    }

                    ResultWorker.LastName = txtLastName.Text.Trim();
                    ResultWorker.FirstName = txtFirstName.Text.Trim();
                    ResultWorker.Position = txtPosition.Text.Trim();
                    ResultWorker.BaseSalary = decimal.Parse(txtBaseSalary.Text);
                    ResultWorker.Bonus = decimal.Parse(txtBonus.Text);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні даних: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Прізвище та ім'я обов'язкові!");
                return false;
            }
            if (!decimal.TryParse(txtBaseSalary.Text, out _) || !decimal.TryParse(txtBonus.Text, out _))
            {
                MessageBox.Show("Зарплата та премія мають бути числами!");
                return false;
            }
            return true;
        }
    }
}