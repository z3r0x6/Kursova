using SalaryAccounting.Models;
using SalaryAccounting.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SalaryAccounting
{
    public partial class MainForm : Form
    {
        private readonly AccountingService _service;
        private DataGridView _dgvWorkers = null!;

        public MainForm()
        {
            _service = new AccountingService();
            InitializeComponent();
            SetupAppearance();
            RefreshGrid(_service.GetAll());
        }

        private void SetupAppearance()
        {
            this.Text = "Система розрахунку зарплати IT";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48); // Темний фон

            // Налаштування таблиці
            _dgvWorkers = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(63, 63, 70), // Колір ліній сітки
                EnableHeadersVisualStyles = false
            };

            // Стиль заголовків
            _dgvWorkers.DefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            _dgvWorkers.DefaultCellStyle.ForeColor = Color.White; // БІЛИЙ ТЕКСТ
            _dgvWorkers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204); // Синій колір виділення
            _dgvWorkers.DefaultCellStyle.SelectionForeColor = Color.White;
            _dgvWorkers.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            _dgvWorkers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
            _dgvWorkers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _dgvWorkers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _dgvWorkers.ColumnHeadersHeight = 40;

            Panel container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            container.Controls.Add(_dgvWorkers);
            this.Controls.Add(_dgvWorkers);

            // Панель з кнопками
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel 
            {
                Dock = DockStyle.Bottom,
                Height = 110,
                BackColor = Color.FromArgb(25, 25, 25),
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight
            };

            Button btnAdd = CreateStyledButton("Додати працівника", Color.FromArgb(34, 139, 34));
            btnAdd.Click += (s, e) => OpenWorkerDialog();

            Button btnEdit = CreateStyledButton("Редагувати вибраного", Color.FromArgb(204, 153, 0));
            btnEdit.Click += (s, e) => EditSelectedWorker();

            Button btnDelete = CreateStyledButton("Видалити запис", Color.FromArgb(178, 34, 34));
            btnDelete.Click += (s, e) => DeleteSelectedWorker();

            Button btnSort = CreateStyledButton("Топ за зарплатою", Color.FromArgb(70, 130, 180));
            btnSort.Click += (s, e) => RefreshGrid(_service.GetSortedByNetSalary());

            Button btnAvg = CreateStyledButton("Середня з/п ФОП", Color.FromArgb(128, 0, 128));
            btnAvg.Click += (s, e) => MessageBox.Show($"Середня з/п ФОП: {_service.GetAverageFopSalary():F2} грн");

            Button btnFops = CreateStyledButton("Чиста з/п ФОП", Color.DarkCyan);
            btnFops.Click += btnShowFops_Click;

            Button btnConsole = CreateStyledButton("Штатні в консоль", Color.Gray);
            btnConsole.Click += btnFixedToConsole_Click;

            Button btnSearch = CreateStyledButton("Зарплата > ніж...", Color.IndianRed);
            btnSearch.Click += btnSearchBySalary_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnFops, btnConsole, btnSearch, btnSort }); 
            this.Controls.Add(buttonPanel);
        }

        private Button CreateStyledButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 50), // Збільшений розмір
                Margin = new Padding(5),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }
        private void EditSelectedWorker()
        {
            if (_dgvWorkers.SelectedRows.Count > 0)
            {
                // Отримуємо об'єкт з поточного рядка
                var worker = _dgvWorkers.SelectedRows[0].DataBoundItem as Worker;
                if (worker == null) return;

                using (var dialog = new WorkerEditForm(worker)) // Передаємо працівника для редагування
                {
                    if (dialog.ShowDialog() == DialogResult.OK && dialog.ResultWorker != null)
                    {
                        _service.UpdateWorker(dialog.ResultWorker);
                        RefreshGrid(_service.GetAll());
                    }
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть працівника в таблиці!");
            }
        }

        private void DeleteSelectedWorker()
        {
            if (_dgvWorkers.SelectedRows.Count > 0)
            {
                var worker = _dgvWorkers.SelectedRows[0].DataBoundItem as Worker;
                if (worker != null && MessageBox.Show("Видалити?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _service.DeleteWorker(worker.Id);
                    RefreshGrid(_service.GetAll());
                }
            }
        }

        private void btnShowFops_Click(object sender, EventArgs e)
        {
            var reports = _service.GetFopsNetSalaries();
            if (reports.Count == 0)
            {
                MessageBox.Show("ФОПів не знайдено.");
                return;
            }

            string message = string.Join("\n", reports);
            MessageBox.Show(message, "Чиста зарплата ФОПів");
        }

        // 2. Кнопка "Штатні в консоль"
        private void btnFixedToConsole_Click(object sender, EventArgs e)
        {
            var fixedWorkers = _service.GetFixedPayWorkers();

            Console.WriteLine("\n--- СПИСОК ПРАЦІВНИКІВ НА ОКЛАДІ ---");
            foreach (var w in fixedWorkers)
            {
                Console.WriteLine($"{w.LastName} {w.FirstName} | Посада: {w.Position} | Оклад: {w.BaseSalary}");
            }
            Console.WriteLine("------------------------------------\n");

            MessageBox.Show("Дані виведено в консоль (дивись вікно Output або консольне вікно)!");
        }

        // 3. Кнопка "Пошук за сумою"
        private void btnSearchBySalary_Click(object sender, EventArgs e)
        {
            // Простий діалог для введення суми
            string input = Microsoft.VisualBasic.Interaction.InputBox("Введіть мінімальну чисту зарплату:", "Пошук", "0");

            if (decimal.TryParse(input, out decimal minSalary))
            {
                var results = _service.GetWorkersWithSalaryMoreThan(minSalary);
                _dgvWorkers.DataSource = null;
                _dgvWorkers.DataSource = results;

                if (results.Count == 0)
                    MessageBox.Show("Працівників з такою зарплатою не знайдено.");
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректне число.");
            }
        }

        private void RefreshGrid(object dataSource)
        {
            _dgvWorkers.DataSource = null;
            _dgvWorkers.DataSource = dataSource;
        }

        private void OpenWorkerDialog(Worker? worker = null)
        {
            using (var dialog = new WorkerEditForm())
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.ResultWorker != null)
                {
                    _service.AddWorker(dialog.ResultWorker);
                    RefreshGrid(_service.GetAll());
                }
            }
        }
    }
}