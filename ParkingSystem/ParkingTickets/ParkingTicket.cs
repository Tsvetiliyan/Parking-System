namespace ParkingSystem.ParkingTickets
{
    internal abstract class ParkingTicket
    {
        /// <summary>
        /// The date at which the ticket is activated
        /// </summary>
        private DateTime startingTicketTime;

        /// <summary>
        /// The last date at which the ticket is activated
        /// </summary>
        private DateTime endingTicketTime;

        /// <summary>
        /// The hourly price that the user has to pay
        /// </summary>
        private double hourlyTicketRate;

        /// <summary>
        /// The full price that the user has to pay
        /// </summary>
        private double fullTicketPrice;

        /// <summary>
        /// The car's registration
        /// </summary>
        private string uniqueCarRegistration;

        /// <summary>
        /// The parking spot, which the car wants to park at
        /// </summary>
        private string designatedParkingSpot;

        /// <summary>
        /// This is the start date of the parking ticket
        /// </summary>
        public DateTime StartingTicketTime
        {
            get => startingTicketTime;
            init => startingTicketTime = value;
        }

        /// <summary>
        /// This is the reserved spot by the ticket
        /// </summary>
        public string DesignatedParkingSpot
        {
            get => designatedParkingSpot;
            init => designatedParkingSpot = value;
        }

        /// <summary>
        /// This is the end date of the parking ticket
        /// </summary>
        public DateTime EndingTicketTime
        {
            get => endingTicketTime;
            init => endingTicketTime = value;
        }

        /// <summary>
        /// The hourly rate of the ticket, i.e. the price, which is charged hourly to the car
        /// The rate is different for each type of ticket and CONSTANT
        /// </summary>
        public double HourlyTicketRate
        {
            get => hourlyTicketRate;
            init => hourlyTicketRate = value;
        }

        /// <summary>
        /// The full price of the ticket
        /// </summary>
        public double FullTicketPrice
        {
            get => fullTicketPrice;
            init => fullTicketPrice = value;
        }

        /// <summary>
        /// This is the car's unique car registration. The car is the one that reserved the parking spot
        /// </summary>
        public string UniqueCarRegistration
        {
            get => uniqueCarRegistration;
            init => uniqueCarRegistration = value;
        }

        /// <summary>
        /// Used for when getting the tickets
        /// </summary>
        public ParkingTicket()
        {

        }

        /// <summary>
        /// Contructor, which assing all of the parameters. Also does the following checks : the the starting date is after the ending date; if the starting date is before the current time
        /// </summary>
        /// <param name="startingTicketTime"></param>
        /// <param name="endingTicketTime"></param>
        /// <param name="designatedParkingSpot"></param>
        /// <param name="uniqueCarRegistration"></param>
        /// <exception cref="ArgumentException"></exception>
        public ParkingTicket(DateTime startingTicketTime, DateTime endingTicketTime, string designatedParkingSpot, string uniqueCarRegistration)
        {
            if ((startingTicketTime - endingTicketTime).TotalHours > 0)
            {
                throw new ArgumentException("Началната дате не може да бъде зададена след крайната дата! (ParkingTicketError)");
            }
            else if ((startingTicketTime - ParkingSys.currentTime).TotalHours < 0)
            {
                throw new ArgumentException("Началната дате не може да бъде зададена след преди текущата дата! (ParkingTicketError)");
            }
            StartingTicketTime = startingTicketTime;
            EndingTicketTime = endingTicketTime;
            DesignatedParkingSpot = designatedParkingSpot;
            UniqueCarRegistration = uniqueCarRegistration;
        }


        /// <summary>
        /// Calculates the ticket cost. The price is received by multiplying the hourly rate by the whole amount of hours
        /// </summary>
        /// <returns>The full ticket cost</returns>
        public virtual double CalculateTicketCost()
        {
            double hourSpan = Math.Round((EndingTicketTime - StartingTicketTime).TotalHours, 2);
            return Math.Round(HourlyTicketRate * hourSpan, 2);
        }

        /// <summary>
        /// Prints out the following info of the parking ticket : DesignatedParkingSpot; UniqueCarRegistration; StartingTicketTime; EndingTicketTime; FullTicketPrice
        /// </summary>
        public void PrintOutTicketInfo()
        {
            Console.WriteLine($"{DesignatedParkingSpot} - {UniqueCarRegistration} - {StartingTicketTime} - {EndingTicketTime} - {FullTicketPrice}$");
        }
    }
}