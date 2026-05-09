using SalaryAccounting.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SalaryAccounting.Services
{
    public static class FileManager
    {
        private static readonly string FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string FilePath = Path.Combine(FolderPath, "workers.txt");
        public static void SaveToFile(List<Worker> workers)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                using (StreamWriter writer = new StreamWriter(FilePath, false))
                {
                    foreach (var worker in workers)
                    {
                        writer.WriteLine(worker.ToFileFormat());
                    }
                }
            }
            catch (Exception ex)
            {
                // Логування помилки або виведення повідомлення користувачу
                MessageBox.Show($"Помилка при збереженні даних: {ex.Message}");

            }
        }
        public static List<Worker> LoadFromFile()
        {
            var workers = new List<Worker>();

            if (!File.Exists(FilePath)) return workers;

            string[] lines = File.ReadAllLines(FilePath);
            try
            {
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length < 8) continue;

                    string type = parts[0];
                    Worker worker = type == "FOP" ? new FopWorker() : new FixedPayWorker();

                    worker.Id = Guid.Parse(parts[1]);
                    worker.LastName = parts[2];
                    worker.FirstName = parts[3];
                    worker.Position = parts[4];
                    worker.BaseSalary = decimal.Parse(parts[5]);
                    worker.Bonus = decimal.Parse(parts[6]);
                    worker.TaxRate = decimal.Parse(parts[7]);

                    workers.Add(worker);
                }
            }
            catch { }
            

            return workers;
        }
    }
}