$SqlServer = ".\SQLEXPRESS"
$Database = "IncidentesFISEI_Dev"

$ConnectionString = "Server=$SqlServer;Database=$Database;Integrated Security=true;TrustServerCertificate=true;"
$Connection = New-Object System.Data.SqlClient.SqlConnection
$Connection.ConnectionString = $ConnectionString
$Connection.Open()

$Command = $Connection.CreateCommand()

# Delete existing services
$Command.CommandText = "DELETE FROM Servicios"
$Command.ExecuteNonQuery()

# Insert services with proper UTF-8 handling
$services = @(
    @{Nombre="Mantenimiento de Computadoras"; Descripcion="Reparación y mantenimiento de equipos de cómputo"; Codigo="SRV-HW-001"; CategoriaId=1; ResponsableArea="Infraestructura"; ContactoTecnico="soporte@ejemplo.com"; TiempoRespuesta=120; TiempoResolucion=480; Instrucciones="Contacte a soporte técnico"; RequiereAprobacion=0},
    @{Nombre="Reemplazo de Hardware"; Descripcion="Cambio e instalación de componentes de hardware"; Codigo="SRV-HW-002"; CategoriaId=1; ResponsableArea="Infraestructura"; ContactoTecnico="hardware@ejemplo.com"; TiempoRespuesta=240; TiempoResolucion=1440; Instrucciones="Solicite reemplazo de componentes"; RequiereAprobacion=1},
    @{Nombre="Impresoras y Periféricos"; Descripcion="Soporte para impresoras, escáneres y otros periféricos"; Codigo="SRV-HW-003"; CategoriaId=1; ResponsableArea="Tecnología"; ContactoTecnico="perifericos@ejemplo.com"; TiempoRespuesta=60; TiempoResolucion=240; Instrucciones="Reinicie el dispositivo"; RequiereAprobacion=0},
    @{Nombre="Instalación de Software"; Descripcion="Instalación y configuración de aplicaciones"; Codigo="SRV-SW-001"; CategoriaId=2; ResponsableArea="Desarrollo"; ContactoTecnico="software@ejemplo.com"; TiempoRespuesta=90; TiempoResolucion=360; Instrucciones="Proporcione licencias válidas"; RequiereAprobacion=1},
    @{Nombre="Actualización de Software"; Descripcion="Parches y actualizaciones de sistemas"; Codigo="SRV-SW-002"; CategoriaId=2; ResponsableArea="Tecnología"; ContactoTecnico="updates@ejemplo.com"; TiempoRespuesta=60; TiempoResolucion=300; Instrucciones="Se requiere planificación"; RequiereAprobacion=1},
    @{Nombre="Aplicaciones Empresariales"; Descripcion="Soporte a aplicaciones personalizadas corporativas"; Codigo="SRV-SW-003"; CategoriaId=2; ResponsableArea="Desarrollo"; ContactoTecnico="apps@ejemplo.com"; TiempoRespuesta=120; TiempoResolucion=600; Instrucciones="Abra ticket de soporte"; RequiereAprobacion=1},
    @{Nombre="Conectividad de Red"; Descripcion="Configuración y diagnóstico de conectividad"; Codigo="SRV-NET-001"; CategoriaId=3; ResponsableArea="Infraestructura"; ContactoTecnico="redes@ejemplo.com"; TiempoRespuesta=30; TiempoResolucion=180; Instrucciones="Verifique cables y switch"; RequiereAprobacion=0},
    @{Nombre="WiFi Corporativo"; Descripcion="Soporte a redes inalámbricas"; Codigo="SRV-NET-002"; CategoriaId=3; ResponsableArea="Infraestructura"; ContactoTecnico="wifi@ejemplo.com"; TiempoRespuesta=45; TiempoResolucion=240; Instrucciones="Reinicie enrutador"; RequiereAprobacion=0},
    @{Nombre="VPN y Acceso Remoto"; Descripcion="Acceso remoto y conectividad VPN"; Codigo="SRV-NET-003"; CategoriaId=3; ResponsableArea="Infraestructura"; ContactoTecnico="vpn@ejemplo.com"; TiempoRespuesta=45; TiempoResolucion=240; Instrucciones="Reinstale cliente VPN"; RequiereAprobacion=0},
    @{Nombre="Control de Acceso"; Descripcion="Gestión de permisos y acceso a recursos"; Codigo="SRV-ACC-001"; CategoriaId=4; ResponsableArea="Seguridad"; ContactoTecnico="acceso@ejemplo.com"; TiempoRespuesta=60; TiempoResolucion=240; Instrucciones="Solicite cambio de permisos"; RequiereAprobacion=1},
    @{Nombre="Seguridad y Antivirus"; Descripcion="Protección antivirus y antimalware"; Codigo="SRV-ACC-002"; CategoriaId=4; ResponsableArea="Seguridad"; ContactoTecnico="seguridad@ejemplo.com"; TiempoRespuesta=30; TiempoResolucion=120; Instrucciones="Ejecute escaneo completo"; RequiereAprobacion=0},
    @{Nombre="Backup y Recuperación"; Descripcion="Servicios de copia de seguridad y recuperación"; Codigo="SRV-ACC-003"; CategoriaId=4; ResponsableArea="Infraestructura"; ContactoTecnico="backup@ejemplo.com"; TiempoRespuesta=120; TiempoResolucion=1440; Instrucciones="Contacte a equipo de backup"; RequiereAprobacion=1},
    @{Nombre="Correo Corporativo"; Descripcion="Servicio de correo electrónico institucional"; Codigo="SRV-EMAIL-001"; CategoriaId=5; ResponsableArea="Tecnología"; ContactoTecnico="correo@ejemplo.com"; TiempoRespuesta=60; TiempoResolucion=240; Instrucciones="Verifique credenciales"; RequiereAprobacion=0},
    @{Nombre="Configuración de Outlook"; Descripcion="Configuración de clientes de correo"; Codigo="SRV-EMAIL-002"; CategoriaId=5; ResponsableArea="Tecnología"; ContactoTecnico="outlook@ejemplo.com"; TiempoRespuesta=45; TiempoResolucion=180; Instrucciones="Resetee contraseña si es necesario"; RequiereAprobacion=0},
    @{Nombre="Recuperación de Correo"; Descripcion="Recuperación de mensajes eliminados"; Codigo="SRV-EMAIL-003"; CategoriaId=5; ResponsableArea="Tecnología"; ContactoTecnico="recovery@ejemplo.com"; TiempoRespuesta=90; TiempoResolucion=480; Instrucciones="Contacte a administrador"; RequiereAprobacion=1}
)

foreach ($service in $services) {
    $Command.CommandText = @"
    INSERT INTO Servicios (Nombre, Descripcion, Codigo, IsActive, CategoriaId, ResponsableArea, ContactoTecnico, TiempoRespuestaMinutos, TiempoResolucionMinutos, Instrucciones, RequiereAprobacion, CreatedAt, UpdatedAt, IsDeleted)
    VALUES (@Nombre, @Descripcion, @Codigo, 1, @CategoriaId, @ResponsableArea, @ContactoTecnico, @TiempoRespuesta, @TiempoResolucion, @Instrucciones, @RequiereAprobacion, GETDATE(), GETDATE(), 0)
"@
    
    $Command.Parameters.Clear()
    $Command.Parameters.AddWithValue("@Nombre", $service.Nombre) > $null
    $Command.Parameters.AddWithValue("@Descripcion", $service.Descripcion) > $null
    $Command.Parameters.AddWithValue("@Codigo", $service.Codigo) > $null
    $Command.Parameters.AddWithValue("@CategoriaId", $service.CategoriaId) > $null
    $Command.Parameters.AddWithValue("@ResponsableArea", $service.ResponsableArea) > $null
    $Command.Parameters.AddWithValue("@ContactoTecnico", $service.ContactoTecnico) > $null
    $Command.Parameters.AddWithValue("@TiempoRespuesta", $service.TiempoRespuesta) > $null
    $Command.Parameters.AddWithValue("@TiempoResolucion", $service.TiempoResolucion) > $null
    $Command.Parameters.AddWithValue("@Instrucciones", $service.Instrucciones) > $null
    $Command.Parameters.AddWithValue("@RequiereAprobacion", $service.RequiereAprobacion) > $null
    
    $Command.ExecuteNonQuery() > $null
}

# Verify
$Command.CommandText = "SELECT COUNT(*) as Total FROM Servicios"
$result = $Command.ExecuteScalar()
Write-Host "Total servicios insertados: $result"

$Command.CommandText = "SELECT Id, Nombre, Codigo, CategoriaId FROM Servicios ORDER BY CategoriaId, Id"
$reader = $Command.ExecuteReader()
Write-Host ""
Write-Host "Id`tNombre`t`t`t`t`tCodigo`t`tCategoria"
Write-Host "-" * 80
while ($reader.Read()) {
    $id = $reader[0]
    $nombre = $reader[1]
    $codigo = $reader[2]
    $categoria = $reader[3]
    Write-Host "$id`t$nombre`t`t`t$codigo`t$categoria"
}
$reader.Close()

$Connection.Close()
