namespace ParkingSystem.ParkingTickets
{
    internal class InAdvanceParkingTicket : ParkingTicket
    {
        /// <summary>
        /// Constructor, which sets the startingTicketTime, endingTicketTime, designatedParkingSpot, uniqueCarRegistration, hourlyTicketRate(Depending on when was the ticket reserved) and calculates the fullTicketPrice
        /// </summary>
        /// <param name="startingTicketTime">The starting ticket date</param>
        /// <param name="endingTicketTime">The ending ticket date</param>
        /// <param name="designatedParkingSpot">The designated parking spot for the car</param>
        /// <param name="uniqueCarRegistration">Car's registration</param>
        public InAdvanceParkingTicket(DateTime startingTicketTime, DateTime endingTicketTime, string designatedParkingSpot, string uniqueCarRegistration) : base(startingTicketTime, endingTicketTime, designatedParkingSpot, uniqueCarRegistration)
        {
            if ((StartingTicketTime - ParkingSys.currentTime).TotalHours >= 24)
            {
                HourlyTicketRate = 1.2;
            }
            else
            {
                HourlyTicketRate = 1.0;
            }
            FullTicketPrice = CalculateTicketCost();
        }

        /// <summary>
        /// Used for when getting the tickets
        /// </summary>
        public InAdvanceParkingTicket() : base()
        {

        }

        /// <summary>
        /// Creates a ticket
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static InAdvanceParkingTicket CreateInAdvanceParkingTicket()
        {
            Console.Write("Напишете регистрационния номер на колата: ");
            string uniqueCarRegistration = Console.ReadLine();

            Console.Write("Напишете началното време на билета (час-ден-месец-година): ");
            int[] input = Console.ReadLine().Split('-').Select(x => Convert.ToInt32(x)).ToArray();
            DateTime startingDate = new DateTime(input[3], input[2], input[1], input[0], 0, 0);

            Console.Write("Напишете крайното време на билета (час-ден-месец-година): ");
            input = Console.ReadLine().Split('-').Select(x => Convert.ToInt32(x)).ToArray();
            DateTime endingDate = new DateTime(input[3], input[2], input[1], input[0], 0, 0);

            if ((startingDate - ParkingSys.currentTime).TotalDays > 7)
            {
                throw new ArgumentException("Не можеш да запазиш място, по-късно от една седмица!");
            }
            else if ((startingDate - ParkingSys.currentTime).TotalMinutes < 60)
            {
                throw new ArgumentException("Не можеш да запазиш място, по-рано от един час преди неговото началото!");
            }

            Console.Write("Напишете на кое парково място искате да сте: ");
            string designatedParkingSpot = Console.ReadLine();
            //if (!IsParkingSpotFreeAndContained(designatedParkingSpot))
            //{
            //    throw new ArgumentException("The designated parking spot you want to park in is either nonexistent or not free(InAdvanceParkingTicketError)");
            //}

            InAdvanceParkingTicket inAdvance = new InAdvanceParkingTicket(startingDate, endingDate, designatedParkingSpot, uniqueCarRegistration);
            if (!Transaction.CheckIfSpotIsNOTReservedAheadOrNow(inAdvance))
            {
                throw new ArgumentException("Избраното парково място вече е запазено за зададеното от вас време! (InAdvanceParkingTicketError)");
            }

            return inAdvance;
        }
    }
}