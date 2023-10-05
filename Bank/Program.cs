using System;
using System.Threading;

class Account
{
    private int balance;

    public void Deposit(decimal amount)
    {
        Interlocked.Add(ref balance, (int)amount);
    }

    public bool Snyatie(decimal amount)
    {
        decimal newBalance = Interlocked.Add(ref balance, -(int)amount);

        if (newBalance < 0)
        {
            Interlocked.Add(ref balance, (int)amount); // вернуть сумму обратно на счет, так как снятие невозможно
            return false;
        }

        return true;
    }

    public decimal GetBalance()
    {
        return balance;
    }

    public void WaitForBalance(decimal targetBalance)
    {
        while (balance < targetBalance)
        {
            Thread.Sleep(1000);
        }
    }
}

class Program
{
    static Account account = new Account();

    static void Main(string[] args)
    {
        // Создаем и запускаем поток для многократного пополнения баланса на случайную сумму
        Thread depositThread = new Thread(DepositThread);
        depositThread.Start();

        decimal withdrawalAmount = 10000; // Сумма, которую нужно накопить для снятия денег

        // Ожидаем, пока баланс достигнет требуемой суммы для снятия денег
        account.WaitForBalance(withdrawalAmount);

        // Если достаточно средств, снимаем деньги
        if (account.Snyatie(withdrawalAmount))
        {
            Console.WriteLine("Снятие денег: {0}", withdrawalAmount);
            Console.WriteLine("Остаток на балансе: {0}", account.GetBalance());
        }
        else
        {
            Console.WriteLine("Недостаточно средств на счете");
        }

        Console.ReadLine();
    }

    static void DepositThread()
    {
        Random random = new Random();

        while (true)
        {
            decimal depositAmount = random.Next(100, 1000); // случайная сумма для пополнения
            account.Deposit(depositAmount);

            Console.WriteLine("Пополнение баланса: {0}", depositAmount);
            Console.WriteLine("Остаток на балансе: {0}", account.GetBalance());

            Thread.Sleep(2000);
        }
    }
}