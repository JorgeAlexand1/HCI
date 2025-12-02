using FISEI.Incidentes.Infrastructure.Security;

// Script para generar hash de contraseña para usuario administrador
var password = "Admin123!";
var hash = PasswordHasher.HashPassword(password);

Console.WriteLine("=== Hash de Contraseña ===");
Console.WriteLine($"Contraseña: {password}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine();
Console.WriteLine("Copia este hash para usarlo en el script SQL.");
Console.WriteLine();

// Verificar que funciona
var isValid = PasswordHasher.Verify(password, hash);
Console.WriteLine($"Verificación: {(isValid ? "✓ OK" : "✗ Error")}");
