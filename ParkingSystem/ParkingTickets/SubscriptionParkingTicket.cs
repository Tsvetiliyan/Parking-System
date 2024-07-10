namespace ParkingSystem.ParkingTickets
{
    internal class SubscriptionParkingTicket : ParkingTicket
    {
        /// <summary>
        /// Constructor, which sets the designatedParkingSpot, the uniqueCarRegistration, sets the startingDate as current time and the endingDate as 1 month more than the startingDate. Also sets the fullTicketPrice and hourlyTicketRate
        /// </summary>
        /// <param name="designatedParkingSpot">The designated parking spot for the car</param>
        /// <param name="uniqueCarRegistration">The car's registration</param>
        public SubscriptionParkingTicket(string designatedParkingSpot, string uniqueCarRegistration) : base(ParkingSys.currentTime, ParkingSys.currentTime.AddMonths(1), designatedParkingSpot, uniqueCarRegistration)
        {
            HourlyTicketRate = 0.2333;
            FullTicketPrice = 168d;
        }

        /// <summary>
        /// Used for when getting the tickets
        /// </summary>
        public SubscriptionParkingTicket()
        {

        }

        /// <summary>
        /// Returns the fullTicketPrice, which with all subscribtion type tickets equalts to 168.00
        /// </summary>
        /// <returns>168$</returns>
        public override double CalculateTicketCost()
        {
            return FullTicketPrice;
        }

        /// <summary>
        /// Used for creating SubscriptionParkingTicket objects
        /// </summary>
        /// <returns>SubscriptionParkingTicket objects</returns>
        /// <exception cref="ArgumentException"></exception>
        public static SubscriptionParkingTicket CreateSubscribtionParkingTicket()
        {
            Console.Write("Напишете регистрационния номер на колата: ");
            string uniqueCarRegistration = Console.ReadLine();

            Console.Write("Напишете кое парково място искате да запазите: ");
            string designatedParkingSpot = Console.ReadLine();
            //if (!IsParkingSpotFreeAndContained(designatedParkingSpot))
            //{
            //    throw new ArgumentException("The designated parking spot you want to park in is either nonexistent or not free (SubscribtionParkingTicketError)");
            //}

            SubscriptionParkingTicket subscription = new SubscriptionParkingTicket(designatedParkingSpot, uniqueCarRegistration);

            if (!Transaction.CheckIfSpotIsNOTReservedAheadOrNow(subscription))
            {
                throw new ArgumentException("Избраното от вас място вече е запазено за зададенот от вас време (SubscriptionParkingTicketError)");
            }

            return subscription;
        }
    }
}