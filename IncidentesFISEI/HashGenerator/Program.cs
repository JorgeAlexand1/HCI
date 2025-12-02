Console.WriteLine("=== Generador de Hashes BCrypt ===");
Console.WriteLine();

var passwords = new Dictionary<string, string>
{
    {"Admin", "Admin123!"},
    {"Supervisor", "Supervisor123!"},
    {"Tecnico", "Tecnico123!"},
    {"Docente", "Docente123!"},
    {"Estudiante", "Estudiante123!"}
};

foreach (var kvp in passwords)
{
    var hash = BCrypt.Net.BCrypt.HashPassword(kvp.Value);
    Console.WriteLine($"{kvp.Key}: {kvp.Value} -> {hash}");
}

Console.WriteLine();
Console.WriteLine("Presione cualquier tecla para salir...");
