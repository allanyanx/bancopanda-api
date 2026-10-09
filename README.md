# 🐼 BancoPanda API

API REST de BancoPanda hecha con **.NET 10** y **PostgreSQL**.

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

## 3. Configurar la cadena de conexión

La contraseña **no** se guarda en el repositorio. Se usa *User Secrets*:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=bancopanda;Username=bancopanda_user;Password=TuClave"
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

## Problemas comunes

| Error | Solución |
|---|---|
| `dotnet ef` no se reconoce | Cierra y vuelve a abrir la terminal |
| `password authentication failed` | Revisa la contraseña con `dotnet user-secrets list` |
| `database "bancopanda" does not exist` | Repite el paso 2 |
