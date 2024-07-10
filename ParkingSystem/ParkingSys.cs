using ParkingSystem.ParkingTickets;
using ParkingSystem.Transactions;
namespace ParkingSystem
{
    internal static class ParkingSys
    {
        public static event EventHandler<DaySwitched> OnDaySwitched;
        public class DaySwitched : EventArgs
        {
            public DateTime dateTime;
        }

        public static DateTime currentTime;

        private static double currentAmountOfMoney = 0;
        public static double CurrentAmountOfMoney
        {
            get => currentAmountOfMoney;
            set => currentAmountOfMoney = value;
        }

        static ParkingSys()
        {
            TransactionHelper.GetAllTransactions(Transaction.expiredTransactions, Transaction.activeTransactions, Transaction.comingUpTransactions);
            Console.WriteLine($"В момента часът е : {currentTime}");
        }

        /// <summary>
        /// This is used to start the program and for the user to use it
        /// </summary>
        public static void TimeLoop(string adminUser)
        {
            string input = "";
            Console.WriteLine("Имате ли нужда от помощ с командите? Ако да, напиши 'help'!");
            input = Console.ReadLine();
            if (input.ToLower() == "help")
            {
                WriteOutAvailableCommands(adminUser);
            }
            do
            {
                Console.WriteLine("Напиши командата, която искаш да се изпълни!");
                input = Console.ReadLine();
                switch (input)
                {
                    case "reserve-spot":
                        if (adminUser == "admin")
                        {
                            Console.WriteLine("Тази команда не е валидна за администратор!");
                            break;
                        }
                        Transaction.TryReserveParkingSpot();
                        break;
                    case "free-spot":
                        if (adminUser == "admin")
                        {
                            Console.WriteLine("Тази команда не е валидна за администратор!");
                            break;
                        }
                        Transaction.TryFreeUpParkingSpaceEarlier();
                        break;
                    case "park-spot-info":
                        WriteOutParkingSpotInfo();
                        break;
                    case "show-transactions":
                        if (adminUser == "user")
                        {
                            Console.WriteLine("Тази команда не е валидна за потребител!");
                            break;
                        }
                        Transaction.ShowAllTransactions();
                        break;
                    case "exit":
                        TransactionHelper.WriteTransactionsToTextFiles(Transaction.expiredTransactions, Transaction.activeTransactions, Transaction.comingUpTransactions);
                        TransactionHelper.SetCurrentDateToTextFile();
                        Environment.Exit(0);
                        break;
                    case "skip-day":
                        break;
                    default:
                        Console.WriteLine("Невалидна команда!");
                        break;
                }
                Console.WriteLine("Ако искате да отидете до следващия ден, напишете 'switch-day'.");
                input = Console.ReadLine();
            } while (input != "switch-day");
            Console.Clear();
            SwitchDays();

        }

        /// <summary>
        /// Writes out all of the available commands that the user han use
        /// </summary>
        private static void WriteOutAvailableCommands(string adminUser)
        {
            switch (adminUser)
            {
                case "admin":
                    Console.WriteLine("park-spot-info (Показва информация за всички паркови места)");
                    Console.WriteLine("show-transactions (Показва всички транзакции)");
                    Console.WriteLine("skip-day (Пропуска сегашния ден, без да прави промени)");
                    Console.WriteLine("exit (Изход от програмата)");
                    break;
                case "user":
                    Console.WriteLine("reserve-spot (Резервира парково място)");
                    Console.WriteLine("free-spot (Освобождава парково място, преди да дойде крайната дата)");
                    Console.WriteLine("park-spot-info (Показва информацията за всички паркови места)");
                    Console.WriteLine("skip-day (Пропуска деня без да прави промени)");
                    Console.WriteLine("exit (Напуска програмата)");
                    break;
                default:
                    throw new ArgumentException("Трябва да бъдете или user, или admin! (WriteOutAvailableCommands.ParkingSysError)");
            }
        }

        /// <summary>
        /// Prints the information about all of the parking spots (The parking spot, if there is a car inside, from when to when, etc.
        /// </summary>
        private static void WriteOutParkingSpotInfo()
        {
            foreach (string parkingSpot in Parking.ParkingSpots)
            {
                if (Transaction.activeTransactions != null && Transaction.activeTransactions.Any(x => x.ParkingTicket.DesignatedParkingSpot == parkingSpot))
                {
                    ParkingTicket parkingTicket = Transaction.activeTransactions.FirstOrDefault(x => x.ParkingTicket.DesignatedParkingSpot == parkingSpot).ParkingTicket;
                    parkingTicket.PrintOutTicketInfo();
                }
                else
                {
                    Console.WriteLine($"{parkingSpot} - свободно");
                }
            }
        }

        /// <summary>
        /// Switches days and fires an event, which all of the transactions are supposed to be subscribed to.
        /// </summary>
        private static void SwitchDays()
        {
            Console.WriteLine("Започна нов ден!");
            currentTime = currentTime.AddDays(1);
            Console.WriteLine();
            Console.WriteLine($"В момента в банката на паркинг системата има : {CurrentAmountOfMoney}$!");
            Console.WriteLine();
            Console.WriteLine($"Сегашната дата е : {currentTime}!");
            OnDaySwitched?.Invoke(string.Empty, new DaySwitched { dateTime = currentTime });
        }
    }
}