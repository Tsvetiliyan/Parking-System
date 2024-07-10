namespace ParkingSystem
{
    internal static class Parking
    {
        /// <summary>
        /// All of the parking spots
        /// </summary>
        private static string[] parkingSpots;

        /// <summary>
        /// All of the parking spots
        /// </summary>
        public static string[] ParkingSpots
        {
            get => parkingSpots;
            set => parkingSpots = value;
        }

        /// <summary>
        /// A static constructor of Parking class, which assigns default values to the parking spots
        /// </summary>
        static Parking()
        {
            parkingSpots = new string[5];
            for (int i = 0; i < 5; i++)
            {
                string parkingSpot = $"A{i + 1}";
                parkingSpots[i] = parkingSpot;
            }
        }

    }
}
