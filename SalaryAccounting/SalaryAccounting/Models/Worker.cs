namespace SalaryAccounting.Models
{
    public abstract class Worker
    {
        public Guid Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal TaxRate { get; set; }

        public abstract string SalaryTypeMarker { get; }

        public Worker()
        {
            Id = Guid.NewGuid();
        }

        public abstract decimal CalculateNetSalary();

        public virtual string ToFileFormat()
        {
            return $"{SalaryTypeMarker}|{Id}|{LastName}|{FirstName}|{Position}|{BaseSalary}|{Bonus}|{TaxRate}";
        }
    }

    public class FopWorker : Worker
    {
        public override string SalaryTypeMarker => "FOP";

        public FopWorker()
        {
            // Встановлюємо ставку податку 22% (ЄСВ)
            TaxRate = 0.22m;
        }
        public override decimal CalculateNetSalary()
        {
            // Формула: (BaseSalary + Bonus) - 22% від BaseSalary
            decimal taxAmount = BaseSalary * TaxRate;
            return (BaseSalary + Bonus) - taxAmount;
        }
    }
    public class FixedPayWorker : Worker
    {
        public override string SalaryTypeMarker => "FIXED";

        public FixedPayWorker()
        {
            // Встановлюємо загальну ставку податку 19.5%
            TaxRate = 0.195m;
        }

        public override decimal CalculateNetSalary()
        {
            // За умовою премія становить 5% від BaseSalary. 
            // Відразу записуємо це значення у властивість Bonus для коректного збереження у файл.
            Bonus = BaseSalary * 0.05m;

            decimal taxAmount = BaseSalary * TaxRate;

            // Формула: BaseSalary + Премія - Податки
            return BaseSalary + Bonus - taxAmount;
        }
    }
}
