# 🐼 BancoPanda API

API REST de BancoPanda hecha con **.NET 10** y **PostgreSQL**.

## Estructura del proyecto

Organizado por funcionalidad (*feature-first*):

```
bancopanda-api/
├── BancoPanda.slnx                 # Solución
└── BancoPanda.Api/
    ├── Data/
    │   └── AppDbContext.cs         # Conexión con PostgreSQL (EF Core)
    ├── Entities/                   # Tablas de la base de datos
    │   ├── Usuario.cs
    │   ├── Cuenta.cs
    │   ├── Tarjeta.cs
    │   ├── Movimiento.cs
    │   └── Enum.cs                 # TipoCuenta, TipoMovimiento, CategoriaMovimiento...
    ├── Common/                     # Errores de negocio y su respuesta JSON
    ├── Features/                   # Una carpeta por funcionalidad
    │   └── Auth/                   # Registro y login (JWT)
    │       ├── AuthController.cs   # Endpoints
    │       ├── AuthService.cs      # Reglas de negocio
    │       ├── AuthDtos.cs         # Datos de entrada y salida
    │       ├── TokenService.cs     # Generación del token JWT
    │       └── JwtOptions.cs       # Configuración del token
    ├── Migrations/                 # Migraciones generadas por EF Core
    ├── Properties/
    │   └── launchSettings.json     # Puertos de ejecución local
    ├── appsettings.json            # Configuración (sin contraseñas)
    └── Program.cs                  # Punto de entrada y registro de servicios
```

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [PostgreSQL 18](https://www.postgresql.org/download/)
- Herramienta de EF Core:

```bash
dotnet tool install --global dotnet-ef
```

## 1. Clonar el repositorio

```bash
git clone https://github.com/allanyanx/bancopanda-api.git
cd bancopanda-api/BancoPanda.Api
```

## 2. Crear la base de datos

Entra a PostgreSQL como administrador:

```bash
psql -U postgres
```

Crea el usuario y la base (cambia la contraseña):

```sql
CREATE USER bancopanda_user WITH PASSWORD 'TuClave';
CREATE DATABASE bancopanda OWNER bancopanda_user;
\q
```

## 3. Configurar los secretos

Las contraseñas y claves **no** se guardan en el repositorio. Se usa *User Secrets*.

Cadena de conexión a la base de datos:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=bancopanda;Username=bancopanda_user;Password=TuClave"
```

Clave para firmar los tokens JWT (cualquier texto de **32 caracteres o más**):

```bash
dotnet user-secrets set "Jwt:Clave" "escribe-aqui-una-clave-larga-y-aleatoria"
```

## 4. Crear las tablas

```bash
dotnet ef database update
```

## 5. Levantar la API

```bash
dotnet run
```

La API queda disponible en **http://localhost:5258**.

Para comprobar que funciona, abre: http://localhost:5258/openapi/v1.json

> 📱 Desde el emulador de Android la API se llama con **http://10.0.2.2:5258**

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/registro` | Crea el usuario con su cuenta de ahorros y su tarjeta. Devuelve la sesión iniciada |
| POST | `/api/auth/login` | Inicia sesión con correo o cédula. Devuelve el token JWT |

Hay peticiones de ejemplo en `BancoPanda.Api/BancoPanda.Api.http` (se ejecutan desde VS Code o Visual Studio).

Los errores se devuelven en formato *ProblemDetails*: el mensaje va en `detail` y los errores de validación en `errors`.

## Convenciones

- Las llaves primarias llevan el nombre de la entidad (`UsuarioId`, `CuentaId`...), nunca solo `Id`.
- Los campos se llaman igual en la base de datos (`usuario_id`), en el backend (`UsuarioId`) y en el JSON que recibe la app (`usuarioId`).
- La base de datos solo se modifica con migraciones de EF Core; no hay scripts SQL sueltos ni triggers.

## Problemas comunes

| Error | Solución |
|---|---|
| `Falta la clave del token` al arrancar | Configura `Jwt:Clave` (paso 3) |
| `dotnet ef` no se reconoce | Cierra y vuelve a abrir la terminal |
| `password authentication failed` | Revisa la contraseña con `dotnet user-secrets list` |
| `database "bancopanda" does not exist` | Repite el paso 2 |
