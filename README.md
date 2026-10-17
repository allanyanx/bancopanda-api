# 🐼 BancoPanda API

API REST de BancoPanda hecha con **.NET 10** y **PostgreSQL**.

## Estructura del proyecto

Organizado por funcionalidad (*feature-first*):

```
bancopanda-api/
├── BancoPanda.slnx                 # Solución
├── Dockerfile                      # Imagen para Render (aplica las migraciones al arrancar)
├── Database/
│   └── seed.sql                    # Datos de prueba (usuarios, cuentas, tarjetas, movimientos y beneficiarios)
└── BancoPanda.Api/
    ├── Data/
    │   ├── AppDbContext.cs         # Conexión con PostgreSQL (EF Core) y bancos precargados
    │   └── BloqueoCuentas.cs       # Bloquea las cuentas mientras se mueve dinero
    ├── Entities/                   # Tablas de la base de datos
    │   ├── Usuario.cs
    │   ├── Cuenta.cs
    │   ├── Tarjeta.cs              # Número de 16 dígitos
    │   ├── Movimiento.cs
    │   ├── Banco.cs                # Bancos para transferir (Banco Panda y otros del país)
    │   ├── Beneficiario.cs         # A quién transfirió cada usuario (frecuentes)
    │   ├── Transferencia.cs        # Comprobante de cada transferencia
    │   ├── EfectivoMovil.cs        # Retiro sin tarjeta con código
    │   └── Enum.cs                 # TipoCuenta, CategoriaMovimiento, EstadoEfectivoMovil...
    ├── Common/                     # Resultado<T>, ApiControllerBase, ReglasValidacion y ReglasMonto
    ├── Features/                   # Una carpeta por funcionalidad
    │   ├── Auth/                   # Registro y login (JWT)
    │   │   ├── AuthController.cs   # Endpoints
    │   │   ├── AuthService.cs      # Reglas de negocio
    │   │   ├── AuthDtos.cs         # Datos de entrada y salida
    │   │   ├── TokenService.cs     # Generación del token JWT
    │   │   └── JwtOptions.cs       # Configuración del token
    │   ├── Users/                  # Perfil del usuario
    │   ├── Accounts/               # Cuentas y saldos
    │   ├── Cards/                  # Tarjetas, sus movimientos y GeneradorTarjeta
    │   ├── Movements/              # Movimientos recientes
    │   ├── Banks/                  # Lista de bancos
    │   ├── Beneficiaries/          # Beneficiarios frecuentes
    │   ├── Transfers/              # Transferencias
    │   └── MobileCash/             # Efectivo móvil
    ├── Migrations/                 # Migraciones generadas por EF Core
    ├── Properties/
    │   └── launchSettings.json     # Puertos de ejecución local
    ├── BancoPanda.Api.http         # Peticiones de ejemplo
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

## 4. Crear o actualizar las tablas

```bash
dotnet ef database update
```

Aplica solo las migraciones que falten; **no hace falta borrar la base**. Para ver cuáles están aplicadas:

```bash
dotnet ef migrations list
```

## 5. Levantar la API

```bash
dotnet run
```

La API queda disponible en **http://localhost:5258**.

Para comprobar que funciona, abre: http://localhost:5258/openapi/v1.json

> 📱 Desde el emulador de Android la API se llama con **http://10.0.2.2:5258**

## 6. Cargar datos de prueba (opcional)

Un usuario recién registrado solo tiene su bono de bienvenida. Para ver el dashboard con varias cuentas, tarjetas, movimientos y beneficiarios, ejecuta el script desde la raíz del repositorio (después del paso 4):

```bash
psql -U bancopanda_user -d bancopanda -f Database/seed.sql
```

También puedes abrir `Database/seed.sql` en pgAdmin o DBeaver y ejecutarlo completo.

Crea dos usuarios (contraseña de ambos: `Panda2026`). Cada cuenta queda con **$100**:

| Usuario | Cédula | Cuentas | Datos |
|---|---|---|---|
| `andrea.demo` | `0912345678` | `5000900001` (ahorros), `5000900002` (corriente) | 2 tarjetas, 26 movimientos, 3 beneficiarios frecuentes |
| `carlos.demo` | `0923456781` | `5000900003` (ahorros) | 2 tarjetas (una inactiva), 8 movimientos |

Se puede ejecutar las veces que haga falta: borra lo que sembró antes (incluidas sus transferencias) y lo vuelve a crear con fechas recientes. No modifica a los usuarios registrados desde la app. Los bancos no están en el seed: los crea la migración.

## Endpoints

Todos, excepto los de `auth`, piden el token en la cabecera `Authorization: Bearer <token>` y solo devuelven datos del usuario del token.

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/registro` | Crea el usuario con su cuenta de ahorros, $100 de bienvenida y una tarjeta Visa de 16 dígitos. Devuelve la sesión iniciada |
| POST | `/api/auth/login` | Inicia sesión con el nombre de usuario. Devuelve el token JWT |
| GET | `/api/usuarios/me` | Datos del usuario |
| PUT | `/api/usuarios/me` | Actualiza nombre de usuario, nombres, apellidos, correo y celular |
| PUT | `/api/usuarios/me/contrasena` | Cambia la contraseña (pide la actual). Devuelve `204` |
| GET | `/api/cuentas` | Cuentas del usuario con su saldo |
| GET | `/api/tarjetas` | Tarjetas con su número completo, últimos 4 dígitos, cuenta asociada y saldo |
| GET | `/api/tarjetas/{tarjetaId}/movimientos?limite=20` | Movimientos de la cuenta de esa tarjeta. `404` si la tarjeta no es del usuario |
| GET | `/api/movimientos?limite=20` | Últimos movimientos de todas las cuentas del usuario |
| GET | `/api/bancos` | Bancos disponibles para transferir (Banco Panda primero) |
| GET | `/api/beneficiarios` | Beneficiarios frecuentes del usuario, el más reciente primero |
| POST | `/api/transferencias` | Transfiere a una cuenta de Banco Panda o de otro banco. Devuelve `201` con el comprobante |
| POST | `/api/efectivo-movil` | Descuenta el monto y genera dos códigos de 4 dígitos para retirar sin tarjeta (con el celular registrado o con otro). Devuelve `201` |

`limite` es opcional: por defecto 20, mínimo 1 y máximo 50.

Hay peticiones de ejemplo de todos los endpoints en `BancoPanda.Api/BancoPanda.Api.http` (se ejecutan desde VS Code con la extensión REST Client o desde Visual Studio).

Los errores se devuelven en formato *ProblemDetails*: el mensaje va en `detail` y los errores de validación en `errors`.

### Transferencias

Cuerpo de `POST /api/transferencias`:

```json
{
  "solicitudId": "1f0c6f7e-3b8a-4b8e-9a7d-2a4c1b5e9d10",
  "cuentaOrigenId": "c1000000-0000-4000-8000-000000000001",
  "bancoId": "b0000000-0000-4000-8000-000000000001",
  "numeroCuenta": "5000900003",
  "cedula": "0923456781",
  "nombre": "Carlos Mendoza",
  "monto": 12.50
}
```

- **Banco Panda** (`b0000000-0000-4000-8000-000000000001`): la cuenta debe existir y la cédula debe ser de su dueño; el dinero se descuenta del origen y **llega al destino** en la misma transacción. Si es otra cuenta del mismo usuario, cuenta como "entre mis cuentas".
- **Otros bancos**: solo se descuenta del origen (el envío se simula).
- El beneficiario se guarda o actualiza solo, y aparece luego en `GET /api/beneficiarios`.
- `nombre` es opcional; en Banco Panda se usa el nombre real del titular.
- `referencia` del comprobante = fecha de Ecuador + 6 dígitos (ej. `20261016158704`).

### Efectivo móvil

Cuerpo de `POST /api/efectivo-movil`:

```json
{
  "solicitudId": "8a2d1c4b-7e6f-4a3b-9c2d-1e0f5a6b7c8d",
  "cuentaId": "c1000000-0000-4000-8000-000000000001",
  "monto": 20
}
```

- `telefonoDestino` es **opcional**: si no se envía ("Mi número" en la app) se usa el **celular registrado** del usuario; si se envía ("Otro número") debe tener el formato `+5939XXXXXXXX`.
- Responde con **dos códigos de 4 dígitos** (`primerCodigo` y `segundoCodigo`), que se digitan en el cajero junto con el celular.
- Los códigos se devuelven **una sola vez**: en la base se guardan juntos y cifrados (`codigo_hash`, BCrypt).
- Vence a las **24 horas**. Estado inicial: `Pendiente` (otros estados: `Cobrado`, `Expirado`).
- Se registra un movimiento `Egreso` con categoría `EfectivoMovil`.

### Reglas del dinero

- Monto de **$0.01 a $5000**, con máximo 2 decimales, y la cuenta debe tener saldo suficiente.
- Todo ocurre en **una transacción** con las cuentas **bloqueadas** (`SELECT … FOR UPDATE`): si dos operaciones llegan a la vez, la segunda espera y ve el saldo real.
- **`solicitudId`** lo genera la app una vez por intento. Si llega repetido, la transferencia devuelve el mismo comprobante y el efectivo móvil responde `409`, así **nunca se cobra dos veces** al reintentar.

## Despliegue (Render + Supabase)

- El `Dockerfile` define `Database__AplicarMigraciones=true`: al arrancar en Render, la API aplica sola las migraciones pendientes en la base de Supabase. Basta con hacer *push* a la rama que vigila Render.
- La cadena de conexión y la clave JWT de producción están en las variables de entorno de Render (`ConnectionStrings__Default`, `Jwt__Clave`), nunca en el repositorio.
- Los datos de prueba **no** se cargan solos: si se quieren en Supabase, se pega `Database/seed.sql` en el **SQL Editor** de Supabase **después** de que Render termine de desplegar. Ojo: la contraseña de esos usuarios está en este repositorio.

## Convenciones

- Las llaves primarias llevan el nombre de la entidad (`UsuarioId`, `CuentaId`...), nunca solo `Id`.
- Los campos se llaman igual en la base de datos (`usuario_id`), en el backend (`UsuarioId`) y en el JSON que recibe la app (`usuarioId`).
- La estructura de la base de datos solo se modifica con migraciones de EF Core; no hay triggers. El único script SQL aparte es `Database/seed.sql`, que solo inserta datos de prueba.
- Los enums viajan como texto en el JSON (`"Egreso"`, `"EfectivoMovil"`, `"Visa"`), igual que se guardan en la base de datos.
- Los montos de los movimientos son siempre positivos; `tipo` (`Ingreso` o `Egreso`) indica si suman o restan.

## Problemas comunes

| Error | Solución |
|---|---|
| `Falta la clave del token` al arrancar | Configura `Jwt:Clave` (paso 3) |
| `dotnet ef` no se reconoce | Cierra y vuelve a abrir la terminal |
| `password authentication failed` | Revisa la contraseña con `dotnet user-secrets list` |
| `database "bancopanda" does not exist` | Repite el paso 2 |
| El build falla porque el archivo está en uso | Hay un `dotnet run` abierto: ciérralo con `Ctrl + C` antes de `dotnet ef` |
| `seed.sql` falla con `column ... does not exist` | Faltan migraciones: ejecuta primero `dotnet ef database update` |
