using ParkingSystem.ParkingTickets;
namespace ParkingSystem
{
    internal class Transaction
    {
        /// <summary>
        /// List of all of the expired transactions
        /// </summary>
        public static List<Transaction> expiredTransactions = new List<Transaction>();

        /// <summary>
        /// List of all of the currentlyActiveTransactions
        /// </summary>
        public static List<Transaction> activeTransactions = new List<Transaction>();

        /// <summary>
        /// List of all of the planned transactions
        /// </summary>
        public static List<Transaction> comingUpTransactions = new List<Transaction>();

        /// <summary>
        /// The current parking ticket, associated with the purchase
        /// </summary>
        private ParkingTicket parkingTicket;

        /// <summary>
        /// The current parking ticket, associated with the purchase
        /// </summary>
        public ParkingTicket ParkingTicket
        {
            get => parkingTicket;
            set => parkingTicket = value;
        }

        /// <summary>
        /// Used for assigning the parkingTicket
        /// </summary>
        /// <param name="parkingTicket">The parking ticket</param>
        public Transaction(ParkingTicket parkingTicket)
        {
            ParkingTicket = parkingTicket;
        }

        /// <summary>
        /// This static constructor is used for subscribing the CheckTransactions_OnDaysSwitched ONLY ONCE 
        /// </summary>
        static Transaction()
        {
            ParkingSys.OnDaySwitched += CheckTransactions_OnDaysSwitched;
        }

        /// <summary>
        /// This method daily checks if a transaction has expired and if so moves the transaction from activeTransactions to allTransactions
        /// There is no need to do anything else since the parkingSpot has no knoweledge of a car being there; The transaction class handles this
        /// </summary>
        static void CheckTransactions_OnDaysSwitched(object? sender, ParkingSys.DaySwitched e)
        {
            CheckComingUpTransactions(e);
            CheckActiveTransactions(e);
        }

        /// <summary>
        /// Checks if it is time for the plannedTickets to be activated
        /// </summary>
        /// <param name="e">Has information about the current date</param>
        private static void CheckComingUpTransactions(ParkingSys.DaySwitched e)
        {
            List<Transaction> temp = comingUpTransactions;
            foreach (Transaction transaction in temp.ToList())
            {
                if ((transaction.parkingTicket.StartingTicketTime - e.dateTime).TotalHours > 0)
                {
                    //There is still time until the ticket has to activate
                    return;
                }
                comingUpTransactions.Remove(transaction);
                activeTransactions.Add(transaction);
                Console.WriteLine($"Транзакцията на {transaction.parkingTicket.DesignatedParkingSpot} паркинг място беше активирана!");
            }
        }

        /// <summary>
        /// Checks if it the current date equals or it is more than ending date and removes if it is remove the car from the parking spot
        /// </summary>
        /// <param name="e">Has information about the current date</param>
        private static void CheckActiveTransactions(ParkingSys.DaySwitched e)
        {
            List<Transaction> temp = activeTransactions;
            foreach (Transaction transaction in temp.ToList())
            {
                if ((transaction.parkingTicket.EndingTicketTime - e.dateTime).TotalHours > 0)
                {
                    //There is still time until the ticket expires
                    continue;
                }
                activeTransactions.Remove(transaction);
                expiredTransactions.Add(transaction);
                Console.WriteLine($"Паркинг място {transaction.parkingTicket.DesignatedParkingSpot} беше освободено!");
            }
        }

        /// <summary>
        /// Reserves a parkingSpot for the car. Interactive so it is written which car where to, from when to when and  etc.
        /// </summary>
        /// <param name="parkingSpotAddress"></param>
        /// <param name="uniqueCarRegistration"></param>
        public static void TryReserveParkingSpot()
        {
            Console.WriteLine("Какъв тип билет искате? (Subscribtion, OnTheGo, InAdvance)");
            string typeOfTicket = Console.ReadLine();

            switch (typeOfTicket)
            {
                case "Subscribtion":
                    SubscriptionParkingTicket subscriptionParkingTicket = SubscriptionParkingTicket.CreateSubscribtionParkingTicket();
                    Transaction transaction = new Transaction(subscriptionParkingTicket);
                    //Have to check if there is a plannedParkingTicket on the parking spot at any time
                    activeTransactions.Add(transaction);
                    ParkingSys.CurrentAmountOfMoney += subscriptionParkingTicket.FullTicketPrice;
                    break;
                case "OnTheGo":
                    OnTheGoParkingTicket onTheGoParkingTicket = OnTheGoParkingTicket.CreateOnTheGoParkingTicket();
                    transaction = new Transaction(onTheGoParkingTicket);
                    activeTransactions.Add(transaction);
                    ParkingSys.CurrentAmountOfMoney += onTheGoParkingTicket.FullTicketPrice;
                    break;
                case "InAdvance":
                    InAdvanceParkingTicket inAdvanceParkingTicket = InAdvanceParkingTicket.CreateInAdvanceParkingTicket();
                    transaction = new Transaction(inAdvanceParkingTicket);
                    comingUpTransactions.Add(transaction);
                    ParkingSys.CurrentAmountOfMoney += inAdvanceParkingTicket.FullTicketPrice;
                    break;
                default:
                    throw new ArgumentException("Написахте невалиден тип билет!");
            }
            //The adding of money is a good idea to be done with events
        }

        /// <summary>
        /// Frees up the parking space earlier than the designated endingDate
        /// Gives 70% refund of the remaining time
        /// </summary>
        public static void TryFreeUpParkingSpaceEarlier()
        {
            Console.Write("Напишете парково място, което искате да бъде освободено: ");
            string designatedParkingSpotToFreeUp = Console.ReadLine();
            Console.Write("Напишете уникалния регистрационен номер на вашета кола : ");
            string uniqueCarRegistration = Console.ReadLine();

            Transaction transaction = null;
            double refund = 0;
            if (activeTransactions.Any(x => x.ParkingTicket.UniqueCarRegistration == uniqueCarRegistration))
            {
                transaction = activeTransactions.First(x => x.ParkingTicket.UniqueCarRegistration == uniqueCarRegistration);

                if (transaction.parkingTicket is SubscriptionParkingTicket)
                {
                    throw new ArgumentException("Вашета сума не може да бъде възтановена,тъй като билетът е тип абонамент(TransactionError)!");
                }

                activeTransactions.Remove(transaction);
                refund = Math.Round(Math.Round((transaction.ParkingTicket.EndingTicketTime - ParkingSys.currentTime).TotalHours) * transaction.ParkingTicket.HourlyTicketRate * 0.7, 2);
            }
            else if (comingUpTransactions.Any(x => x.ParkingTicket.UniqueCarRegistration == uniqueCarRegistration))
            {
                transaction = comingUpTransactions.First(x => x.ParkingTicket.UniqueCarRegistration == uniqueCarRegistration);
                comingUpTransactions.Remove(transaction);
                refund = Math.Round(transaction.ParkingTicket.FullTicketPrice * transaction.ParkingTicket.HourlyTicketRate * 0.7, 2);
            }
            if (transaction == null)
            {
                throw new ArgumentException("Вашета транзакция не беше намерена! (TransactionError)!");
            }
            expiredTransactions.Add(transaction);
            Console.WriteLine($"Възтановена е сума, равняваща се на {refund}$");
            ParkingSys.CurrentAmountOfMoney -= refund;
        }

        /// <summary>
        /// Shows all of the executed transactions and all of the current money
        /// </summary>
        public static void ShowAllTransactions()
        {
            Console.WriteLine("Предстоящи транзакции:");
            foreach (Transaction transaction in comingUpTransactions)
            {
                transaction.ParkingTicket.PrintOutTicketInfo();
            }
            Console.WriteLine("Сегашни транзакции:");
            foreach (Transaction transaction in activeTransactions)
            {
                transaction.ParkingTicket.PrintOutTicketInfo();
            }
            Console.WriteLine("Предишни транзакции:");
            foreach (Transaction transaction in expiredTransactions)
            {
                transaction.ParkingTicket.PrintOutTicketInfo();
            }
        }

        /// <summary>
        /// Checks the comingUpTransactions if there is a transaction that is going to be activated during the use of another parking ticket, which we want to create
        /// </summary>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        public static bool CheckIfSpotIsNOTReservedAheadOrNow(ParkingTicket parkingTicketToCreate)
        {
            if (!Parking.ParkingSpots.Contains(parkingTicketToCreate.DesignatedParkingSpot))
            {
                throw new ArgumentException("Паркинг мястото, където искате да запазите не съществува (TransactionError)");
            }
            bool comingUpTransactionCheck = CheckIfSpotIsNotReservedAheadHelper(parkingTicketToCreate, comingUpTransactions);
            bool activeTransactionCheck = CheckIfSpotIsNotReservedAheadHelper(parkingTicketToCreate, activeTransactions);
            if (comingUpTransactionCheck == false || activeTransactionCheck == false)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Helps the CheckIfSpotIsNotReserved (Saves code)
        /// </summary>
        /// <param name="parkingTicketToCreate"></param>
        /// <returns></returns>
        public static bool CheckIfSpotIsNotReservedAheadHelper(ParkingTicket parkingTicketToCreate, List<Transaction> listOfTransactionsToUse)
        {

            List<Transaction> transactions = listOfTransactionsToUse.Where(x => x.ParkingTicket.DesignatedParkingSpot == parkingTicketToCreate.DesignatedParkingSpot).ToList();

            foreach (Transaction plannedTransaction in transactions)
            {
                ParkingTicket parkingTicketPlanned = plannedTransaction.ParkingTicket;
                if ((parkingTicketPlanned.StartingTicketTime - parkingTicketToCreate.StartingTicketTime).TotalHours >= 0 && (parkingTicketPlanned.EndingTicketTime - parkingTicketToCreate.EndingTicketTime).TotalHours <= 0)
                {
                    return false;
                }
                else if ((parkingTicketPlanned.StartingTicketTime - parkingTicketToCreate.StartingTicketTime).TotalHours >= 0 && (parkingTicketPlanned.StartingTicketTime - parkingTicketToCreate.EndingTicketTime).TotalHours <= 0)
                {
                    return false;
                }
                else if ((parkingTicketPlanned.StartingTicketTime - parkingTicketToCreate.StartingTicketTime).TotalHours <= 0 && (parkingTicketPlanned.EndingTicketTime - parkingTicketToCreate.EndingTicketTime).TotalHours >= 0)
                {
                    return false;
                }
                else if ((parkingTicketPlanned.EndingTicketTime - parkingTicketToCreate.StartingTicketTime).TotalHours >= 0 && (parkingTicketPlanned.EndingTicketTime - parkingTicketToCreate.EndingTicketTime).TotalHours <= 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}