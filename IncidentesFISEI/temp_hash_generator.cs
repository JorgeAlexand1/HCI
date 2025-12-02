using BCrypt.Net;

class Program 
{
    static void Main() 
    {
        Console.WriteLine("Admin123! -> " + BCrypt.HashPassword("Admin123!"));
        Console.WriteLine("Supervisor123! -> " + BCrypt.HashPassword("Supervisor123!"));
        Console.WriteLine("Tecnico123! -> " + BCrypt.HashPassword("Tecnico123!"));
        Console.WriteLine("Docente123! -> " + BCrypt.HashPassword("Docente123!"));
        Console.WriteLine("Estudiante123! -> " + BCrypt.HashPassword("Estudiante123!"));
    }
}