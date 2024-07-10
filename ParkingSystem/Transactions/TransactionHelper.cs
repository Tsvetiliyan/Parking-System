using ParkingSystem.ParkingTickets;
namespace ParkingSystem.Transactions
{
    internal static class TransactionHelper
    {
        /// <summary>
        /// Base path of the current project
        /// </summary>
        private static string basePath = Directory.GetCurrentDirectory().Split(new string[] { "\\bin" }, StringSplitOptions.None)[0] + "\\Transactions";

        /// <summary>
        /// The path of the text file, which has all of the active and upcoming transactions
        /// </summary>
        private static string basePathActiveUpcomingTransactions = basePath + "\\ActiveUpcomingTransactions.txt";

        /// <summary>
        /// The path of the text file, which has all of the expired transactions
        /// </summary>
        private static string basePathExpiredTransactions = basePath + "\\ExpiredTransactions.txt";

        /// <summary>
        /// Gets the current date (it could be different because of the difference of time between the program and the real life)
        /// </summary>
        private static void GetCurrentDate()
        {
            using (StreamReader streamReader = new StreamReader(basePath.Replace("\\Transactions", "") + "\\CurrentDate.txt"))
            {
                string currentDate = streamReader.ReadLine();
                if (string.IsNullOrEmpty(currentDate))
                {
                    ParkingSys.currentTime = DateTime.Now;
                }
                else
                {
                    ///Switches the : with - in the hour:minute to make it work
                    currentDate = currentDate.Replace(':', '-');
                    int[] curDate = currentDate.Split("-").Select(x => Convert.ToInt32(x)).ToArray();
                    ParkingSys.currentTime = new DateTime(curDate[4], curDate[3], curDate[2], curDate[0], curDate[1], 0);
                }
            }
        }
        public static void SetCurrentDateToTextFile()
        {
            using (var file = File.Create(basePath.Replace("\\Transactions", "") + "\\CurrentDate.txt"))
            {
                using (StreamWriter streamWriter = new StreamWriter(file))
                {
                    streamWriter.WriteLine(ParkingSys.currentTime.ToString("HH:mm-d-M-yyyy"));
                }
            }
        }
        /// <summary>
        /// Gets all of the transactions
        /// </summary>
        /// <param name="expiredTransactions"></param>
        /// <param name="activeTransactions"></param>
        /// <param name="comingUpTransactions"></param>
        public static void GetAllTransactions(List<Transaction> expiredTransactions, List<Transaction> activeTransactions, List<Transaction> comingUpTransactions)
        {
            GetCurrentDate();

            ///The structure of each transaction : 
            ///TypeOfParkingTicket:startingTicketTime:endingTicketTime:hourlyTicketRate:fullTicketPrice:uniqueCarRegistration:designatedParkingSpot
            expiredTransactions.Clear();
            activeTransactions.Clear();
            comingUpTransactions.Clear();
            Transaction.expiredTransactions = ManuallyCreateListTransactions(basePathExpiredTransactions);
            List<Transaction> activeAndUpcomingTransactions = ManuallyCreateListTransactions(basePathActiveUpcomingTransactions);
            foreach (Transaction transaction in activeAndUpcomingTransactions)
            {
                ParkingTicket parkingTicket = transaction.ParkingTicket;
                ///Checks if the parkingTicket has expired
                if ((parkingTicket.EndingTicketTime - ParkingSys.currentTime).TotalMinutes < 0)
                {
                    Transaction.expiredTransactions.Add(transaction);
                }
                ///Checks if the starting date is after the currentDate -> means that it is still inAdvance type of ticket
                else if ((parkingTicket.StartingTicketTime - ParkingSys.currentTime).TotalMinutes > 0)
                {
                    Transaction.comingUpTransactions.Add(transaction);
                }
                else
                {
                    activeTransactions.Add(transaction);
                }
            }
        }

        /// <summary>
        /// Manually creates a parking ticket, using a text file
        /// </summary>
        /// <typeparam name="T">A type of Parking ticket</typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static ParkingTicket ManuallyCreateParkingTicket<T>(string[] transaction) where T : ParkingTicket, new()
        {
            int[] startingTicketTime = { Convert.ToInt32(transaction[1]) };
            startingTicketTime = startingTicketTime.Concat(transaction[2].Split('-').Select(x => Convert.ToInt32(x)).ToArray()).ToArray();
            int[] endingTicketTime = { Convert.ToInt32(transaction[3]) };
            endingTicketTime = endingTicketTime.Concat(transaction[4].Split('-').Select(x => Convert.ToInt32(x)).ToArray()).ToArray();
            T parkingTicket = new T()
            {
                StartingTicketTime = new DateTime(startingTicketTime[4], startingTicketTime[3], startingTicketTime[2], startingTicketTime[0], startingTicketTime[1], 0),
                EndingTicketTime = new DateTime(endingTicketTime[4], endingTicketTime[3], endingTicketTime[2], endingTicketTime[0], 0, 0),
                HourlyTicketRate = Convert.ToDouble(transaction[5]),
                FullTicketPrice = Convert.ToDouble(transaction[6]),
                UniqueCarRegistration = transaction[7],
                DesignatedParkingSpot = transaction[8]
            };
            return parkingTicket;
        }

        /// <summary>
        /// Manually creates a list of transactions, using the ManuallyCreateParkingTicket, from a text file; Adds the full ticket price of all transactions to the current money ParkinSyus
        /// </summary>
        /// <param name="directoryAddressTypeTransaction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static List<Transaction> ManuallyCreateListTransactions(string directoryAddressTypeTransaction)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (StreamReader streamReader = new StreamReader(directoryAddressTypeTransaction))
            {
                string transaction = streamReader.ReadLine();
                //The second check in the while is just for checking if the line is not null
                while (!string.IsNullOrWhiteSpace(transaction))
                {
                    string[] trans = transaction.Split(':');
                    switch (trans[0])
                    {
                        //basic formula -> casts the return of ManuallyCreateParkingTicket as the wanted type and as a parameter puts the new line from the text file; after that the transaction is added
                        case "OnTheGoParkingTicket":
                            OnTheGoParkingTicket onTheGoParkingTicket = (OnTheGoParkingTicket)ManuallyCreateParkingTicket<OnTheGoParkingTicket>(trans);
                            transactions.Add(new Transaction(onTheGoParkingTicket));
                            break;
                        case "InAdvanceParkingTicket":
                            InAdvanceParkingTicket inAdvanceParkingTicket = (InAdvanceParkingTicket)ManuallyCreateParkingTicket<InAdvanceParkingTicket>(trans);
                            transactions.Add(new Transaction(inAdvanceParkingTicket));
                            break;
                        case "SubscriptionParkingTicket":
                            SubscriptionParkingTicket subscriptionParkingTicket = (SubscriptionParkingTicket)ManuallyCreateParkingTicket<SubscriptionParkingTicket>(trans);
                            transactions.Add(new Transaction(subscriptionParkingTicket));
                            break;
                        default:
                            throw new ArgumentException("Грешка при създаването на транзакцията! (TransactionHelperError)");
                    }
                    transaction = streamReader.ReadLine();
                }
            }
            ParkingSys.CurrentAmountOfMoney += transactions.Sum(x => x.ParkingTicket.FullTicketPrice);
            return transactions;
        }

        /// <summary>
        /// When exiting the program, this method is started and it writes out all of the transactions to the text files used to store all of the transactions
        /// </summary>
        /// <param name="expiredTransactions"></param>
        /// <param name="activeTransactions"></param>
        /// <param name="comingUpTransactions"></param>
        public static void WriteTransactionsToTextFiles(List<Transaction> expiredTransactions, List<Transaction> activeTransactions, List<Transaction> comingUpTransactions)
        {
            ///The structure of each transaction : 
            ///TypeOfParkingTicket:startingTicketTime:endingTicketTime:hourlyTicketRate:fullTicketPrice:uniqueCarRegistration:designatedParkingSpot

            foreach (Transaction transaction in expiredTransactions)
            {
                WriteATransactionToTextFile(transaction, basePathExpiredTransactions);
            }
            foreach (Transaction transaction in activeTransactions)
            {
                WriteATransactionToTextFile(transaction, basePathActiveUpcomingTransactions);
            }
            foreach (Transaction transaction in comingUpTransactions)
            {
                WriteATransactionToTextFile(transaction, basePathActiveUpcomingTransactions);
            }
        }

        /// <summary>
        /// Used for writing one transaction to the text file used for storing all of the transactions
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="filePath"></param>
        private static void WriteATransactionToTextFile(Transaction transaction, string filePath)
        {
            ///The structure of each transaction : 
            ///TypeOfParkingTicket:startingTicketTime:endingTicketTime:hourlyTicketRate:fullTicketPrice:uniqueCarRegistration:designatedParkingSpot
            ///Clears the contents of all the text files
            ParkingTicket parkingTicket = transaction.ParkingTicket;
            string textLine = "";
            if (parkingTicket is OnTheGoParkingTicket)
            {
                textLine += "OnTheGoParkingTicket:";
            }
            else if (parkingTicket is InAdvanceParkingTicket)
            {
                textLine += "InAdvanceParkingTicket:";
            }
            else if (parkingTicket is SubscriptionParkingTicket)
            {
                textLine += "SubscriptionParkingTicket:";
            }
            ///Creates a text line in the designated way
            textLine += $"{parkingTicket.StartingTicketTime.ToString("HH:mm-d-M-yyyy")}:{parkingTicket.EndingTicketTime.ToString("HH:mm-d-M-yyyy")}:{parkingTicket.HourlyTicketRate}:{parkingTicket.FullTicketPrice}:{parkingTicket.UniqueCarRegistration}:{parkingTicket.DesignatedParkingSpot}";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(textLine);
            }
        }
    }
}
