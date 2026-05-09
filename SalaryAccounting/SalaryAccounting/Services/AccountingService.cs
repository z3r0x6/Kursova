using SalaryAccounting.Models;

namespace SalaryAccounting.Services
{
    public class AccountingService
    {
        private List<Worker> _workers;

        public AccountingService()
        {
            _workers = FileManager.LoadFromFile();
        }

        public List<Worker> GetAll() => _workers;

        public void AddWorker(Worker worker)
        {
            _workers.Add(worker);
            FileManager.SaveToFile(_workers);
        }

        public void DeleteWorker(Guid id)
        {
            _workers.RemoveAll(w => w.Id == id);
            FileManager.SaveToFile(_workers);
        }

        // Сортування по убуванню чистої зарплати
        public List<Worker> GetSortedByNetSalary()
        {
            return _workers.OrderByDescending(w => w.CalculateNetSalary()).ToList();
        }

        // Пошук ФОПів з виводом їх чистої зарплати (повертаємо список анонімних об'єктів або кортежів)
        public List<(FopWorker Worker, decimal NetSalary)> GetFopsWithSalaries()
        {
            return _workers.OfType<FopWorker>()
                           .Select(f => (f, f.CalculateNetSalary()))
                           .ToList();
        }

        // Відбір працівників на окладі (FixedPay)
        public List<FixedPayWorker> GetFixedPayWorkers()
        {
            return _workers.OfType<FixedPayWorker>().ToList();
        }

        public List<Worker> SearchByPosition(string position)
        {
            return _workers.Where(w => w.Position.Contains(position, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Worker> SearchBySalaryMoreThan(decimal minSalary)
        {
            return _workers.Where(w => w.CalculateNetSalary() > minSalary).ToList();
        }

        // Обчислення середньої зарплати ФОПів
        public decimal GetAverageFopSalary()
        {
            var fops = _workers.OfType<FopWorker>().ToList();
            if (fops.Count == 0) return 0;
            return fops.Average(f => f.CalculateNetSalary());
        }
        public List<Worker> GetWorkersWithSalaryMoreThan(decimal minSalary)
        {
            return _workers.Where(w => w.CalculateNetSalary() > minSalary).ToList();
        }

        public List<string> GetFopsNetSalaries()
        {
            return _workers.OfType<FopWorker>()
                           .Select(f => $"{f.LastName} {f.FirstName} (ФОП): {f.CalculateNetSalary():F2} грн")
                           .ToList();
        }

        public void UpdateWorker(Worker updatedWorker)
        {
            var index = _workers.FindIndex(w => w.Id == updatedWorker.Id);
            if (index != -1)
            {
                _workers[index] = updatedWorker;
                FileManager.SaveToFile(_workers);
            }
        }
    }
}