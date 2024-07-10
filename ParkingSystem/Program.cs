class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Искате да сте admin или user?");
        string adminUser = Console.ReadLine();
        if (adminUser != "admin" && adminUser != "user")
        {
            throw new ArgumentException("Ролята ви трябва да е admin или user (ProgramError)");
        }
        while (true)
        {
            try
            {
                ParkingSystem.ParkingSys.TimeLoop(adminUser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}