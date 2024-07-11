namespace ParkingSystem.ParkingTickets
{
    internal class OnTheGoParkingTicket : ParkingTicket
    {
        /// <summary>
        /// Constructor, which assigns to the OnTheGoParkingTicket an endingTicketTimeDate, designatedParkingSpot, uniqueCarRegistration, assings the hourlyTicketRate as 1 lev and calculates the fullTicketPrice
        /// </summary>
        /// <param name="endingTicketTime">The ending date of the ticket</param>
        /// <param name="designatedParkingSpot">The designated parking spot for the ticket</param>
        /// <param name="uniqueCarRegistration">The car's registration, which reserved the spot</param>
        public OnTheGoParkingTicket(DateTime endingTicketTime, string designatedParkingSpot, string uniqueCarRegistration) : base(ParkingSys.currentTime, endingTicketTime, designatedParkingSpot, uniqueCarRegistration)
        {
            HourlyTicketRate = 1d;
            FullTicketPrice = CalculateTicketCost();
        }

        /// <summary>
        /// Used for when getting the tickets
        /// </summary>
        public OnTheGoParkingTicket() : base()
        {

        }

        /// <summary>
        /// Used for creating a OnTheGoParkingTicket object with user input; Does all of the nessecary validation
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static OnTheGoParkingTicket CreateOnTheGoParkingTicket()
        {
            Console.Write("Напишете регистрационния номер на колата: ");
            string uniqueCarRegistration = Console.ReadLine();

            DateTime startingDate = ParkingSys.currentTime;

            Console.Write("Напишете крайното време на билета (час-ден-месец-година): ");
            int[] input = Console.ReadLine().Split('-').Select(x => Convert.ToInt32(x)).ToArray();
            DateTime endingDate = new DateTime(input[3], input[2], input[1], input[0], 0, 0);

            //checks if the reservation follows the OnTheGoParkingTicket rules for reservation,i.e. The reservation has to be between 1 hour and 24 hours
            if ((endingDate - startingDate).TotalMinutes < 60)
            {
                throw new ArgumentException("Резервацията ви не може да е по-малко от 1 час! (OnTheGoParkingTicketError)");
            }
            else if ((endingDate - startingDate).TotalHours > 24)
            {
                // If the program goes to here, it means that the user has exceeded the 24maximum mark
                //But on a Saturday the user can reserve both days - Saturday and Sunday
                if (startingDate.DayOfWeek != DayOfWeek.Saturday || endingDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    throw new ArgumentException("Резервацията ви не може да бъде по-дълга от 1 ден. През уикенда по-дълги резерации са възможни. (OnTheGoParkingTicketError)");
                }
                else if ((endingDate - startingDate).TotalHours > 48)
                {
                    throw new ArgumentException("Вашета резервация през уикенда не може да бъде повече от 48 часа (OnTheGoParkingTicketError)");
                }
            }

            Console.Write("Напишете кое парково място искате да запазите : ");
            string designatedParkingSpot = Console.ReadLine();
            //if (!IsParkingSpotFreeAndContained(designatedParkingSpot))
            //{
            //    throw new ArgumentException("The designated parking spot you want to park in is either nonexistent or not free (OnTheGoParkingTicketError)");
            //}

            OnTheGoParkingTicket onTheGoParkingTicket = new OnTheGoParkingTicket(endingDate, designatedParkingSpot, uniqueCarRegistration);
            if (!Transaction.CheckIfSpotIsNOTReservedAheadOrNow(onTheGoParkingTicket))
            {
                throw new ArgumentException("Избраното от вас парково място е вече запазено за зададеното от вас време! (OnTheGoParkingTicket)");
            }

            return onTheGoParkingTicket;
        }
    }
}
