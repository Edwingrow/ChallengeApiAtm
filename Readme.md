# 🏧 Challenge API ATM

Una API REST para operaciones de cajero automático (ATM) desarrollada con .NET 8

## 🛠️ Tecnologías Utilizadas


### **Backend Framework**
- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - API REST

### **Base de Datos**
- **PostgreSQL 14** - Base de datos relacional
- **Entity Framework Core 8.0** - ORM para base de datos
- **EFCore.NamingConventions** - Convención snake_case para PostgreSQL
- **Entity Framework Migrations** - Control de versiones de BD

### **Autenticación y Seguridad**
- **JWT Bearer Authentication** - Autenticación con tokens
- **Microsoft.IdentityModel.Tokens** - Validación de tokens JWT
- **System.IdentityModel.Tokens.Jwt** - Generación de tokens JWT
- **BCrypt.Net-Next** - Hash seguro de PINs

### **Validación y Documentación**
- **FluentValidation** - Validación de modelos y DTOs
- **Swagger/OpenAPI** - Documentación interactiva de API
- **Swashbuckle.AspNetCore** - Generación de documentación Swagger

### **Arquitectura y Patrones**
- **Clean Architecture** - Separación por capas (Domain, Application, Infrastructure, API)
- **Repository Pattern** - Abstracción de acceso a datos
- **Dependency Injection** - Inversión de control nativa de .NET
- **CQRS-like** - Separación de comandos y consultas

### **Middleware y Servicios**
- **Middleware personalizado** - Manejo centralizado de errores
- **CORS** - Configuración para desarrollo
- **Health Checks** - Monitoreo del estado de la aplicación
- **HTTP Context Services** - Gestión de contexto de usuario

### **Dockerización**
- **Docker & Docker Compose** - Contenedorización
- **Dockerfile** - Imagen personalizada de la aplicación
- **PostgreSQL Container** - Base de datos en contenedor

## 📁 Estructura del Proyecto

```
ChallengeApiAtm/
├── ChallengeApiAtm.Api/             # Capa de Presentación (Controllers, Middleware)
├── ChallengeApiAtm.Application/     # Capa de Aplicación (Services, DTOs, Interfaces)
├── ChallengeApiAtm.Domain/          # Capa de Dominio (Entities, Enums, Exceptions)
├── ChallengeApiAtm.Infrastructure/  # Capa de Infraestructura (Repositories, Security)
├── docker-compose.yml               # Configuración Docker Compose
├── Dockerfile                       # Imagen Docker de la aplicación
└── README.md                        # Este archivo
└── Diagrama_DER_challengeApiAtm.jpg # Diagrama DER de la base de datos

```

## 🚀 Instalación y Ejecución

### Prerequisitos

- [Docker](https://www.docker.com/get-started) (Obligatorio)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Obligatorio)

## ⚙️ Configuración

### Variables de Entorno

```Enviroments
# Base de datos
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=postgres;Database=atm_db;Username=postgres;Password=admin123

# JWT
Jwt__SecretKey=AtmSecretKey2024!@#$%^&*()_+1234567890SuperSecureKey
Jwt__Issuer=ChallengeApiAtm
Jwt__Audience=ChallengeApiAtm-Users
Jwt__ExpirationMinutes=60

```

### 🐳 Opción 1: Ejecutar con Docker (Recomendado)

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/edwingrow16/ChallengeApiAtm.git
   cd ChallengeApiAtm
   ```

2. **Ejecutar con Docker Compose**
   ```bash
   docker-compose up --build -d
   ```

3. **Verificar que está funcionando**
   - API: http://localhost:3000
   - Swagger: http://localhost:3000/swagger
   - Base de datos PostgreSQL: localhost:5432

### 💻 Opción 2: Ejecutar Localmente
1. **Configurar la base de datos**
   ```bash
   # Iniciar PostgreSQL (usando Docker)
   docker-compose up -d postgres 
   ```

2. **Configurar la cadena de conexión**
   
   Editar `ChallengeApiAtm.Api/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=challengeapiatm;Username=postgres;Password=postgres;"
     }
   }
   ```

3. **Ejecutar la aplicación**
   ```bash
   cd ChallengeApiAtm.Api
   dotnet run --project ChallengeApiAtm.Api // dotnet watch --project ChallengeApiAtm.Api
   ```

4. **Aplicar migraciones** (se hace automáticamente al iniciar)
   ```bash
   dotnet ef database update
   ```

## 📚 Documentación de la API

### 🌐 Acceso a Swagger

Una vez que la aplicación esté ejecutándose, puedes acceder a la documentación interactiva de Swagger en:

**URL:** http://localhost:3000/swagger

Swagger proporciona:
- 📖 Documentación completa de todos los endpoints
- 🧪 Interfaz interactiva para probar los endpoints
- 📋 Esquemas de request/response
- 🔐 Configuración de autenticación Bearer

### 🔑 Datos de Prueba

La aplicación crea automáticamente datos de prueba al iniciar:

```
👤 Usuario: Edwin Garcia (Documento: 96069288)
💳 Tarjeta: 400123456789010
🔢 PIN: 8033
🏦 Cuenta: 1000000001
💰 Saldo inicial: $1,000.00
```

## 🛡️ Endpoints de la API

### Base URL
```
http://localhost:3000/api
```

---

### 1. 👤 Registro de Usuario

**POST** `/user/register`

**Descripción:** Registra un nuevo usuario con tarjeta personalizada.

**Request Body:**
```json
{
  "firstName": "Edwin",
  "lastName": "Garcia",
  "documentNumber": "96069288",
  "cardNumber": "4111111111111111",
  "expiryDate": "12/2026",
  "pin": "123",
  "confirmPin": "123",
  "initialBalance": 1000000
}

**Response (201 Created):**
```json
{
  "success": true,
  "timestamp": "2025-06-30T12:23:06.0023284Z",
  "data": {
    "userId": "f43d62fb-380c-4655-9d19-a10a89ec0a65",
    "accountNumber": "2028586839",
    "cardNumber": "****-****-****-1111",
    "firstName": "Edwin",
    "lastName": "Garcia",
    "initialBalance": 1000000,
    "expiryDate": "12/2026",
    "registrationDate": "2025-06-30T12:23:06.0017721Z",
    "message": "Usuario registrado exitosamente"
  },
  "message": "Usuario registrado exitosamente"
}
```

**cURL:**
```bash
curl -X 'POST' \
  'http://localhost:3000/api/user/register' \
  -H 'Content-Type: application/json' \
  -d '{

    "lastName": "Garcia",
    "documentNumber": "96069288",
    "cardNumber": "4111111111111111",
    "expiryDate": "12/2026",
    "pin": "123",
    "confirmPin": "123",
    "initialBalance": 1000000
  }'
```

---

### 2. 🔐 Autenticación (Login)

**POST** `/auth/login`

**Descripción:** Autentica un usuario y devuelve un token JWT.

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "pin": "123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "timestamp": "2025-06-30T12:26:56.3040252Z",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJmNDNkNjJmYi0zODBjLTQ2NTUtOWQxOS1hMTBhODllYzBhNjUiLCJqdGkiOiJjOWFlNmZlMi1mZDMxLTQxMTgtOGFkMi0zMDU3NjBlNjFlMTEiLCJpYXQiOjE3NTEyODY0MTYsImNhcmRfbnVtYmVyIjoiNDExMTExMTExMTExMTExMSIsImFjY291bnRfbnVtYmVyIjoiMjAyODU4NjgzOSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiZjQzZDYyZmItMzgwYy00NjU1LTlkMTktYTEwYTg5ZWMwYTY1IiwiZXhwIjoxNzUxMjkwMDE2LCJpc3MiOiJDaGFsbGVuZ2VBcGlBdG0iLCJhdWQiOiJDaGFsbGVuZ2VBcGlBdG0tVXNlcnMifQ.icYM0Krrpvg8F6POn4gRIq4ptxEA1uNmlOyiwhmubVA",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "userInfo": {
      "fullName": "Edwin Garcia",
      "accountNumber": "2028586839",
      "cardNumber": "****-****-****-1111"
    }
  },
  "message": "Autenticación exitosa"
}
```

**cURL:**
```bash
curl -X 'POST' \
  'http://localhost:3000/api/auth/login' \
  -H 'Content-Type: application/json' \
  -d '{
    "cardNumber": "4111111111111111",
    "pin": "123"
  }'
```

---

### 3. 💰 Consulta de Saldo

**POST** `/account/balance`
🔒 **Requiere autenticación**

**Descripción:** Consulta el saldo actual de la cuenta.

**Headers:**
```
Authorization: Bearer <token_jwt>
Content-Type: application/json
```

**Request Body:**
```json
{
  "cardNumber": "4111111111111111"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "timestamp": "2025-06-30T12:28:14.016078Z",
  "data": {
    "userName": "Edwin Garcia",
    "accountNumber": "2028586839",
    "currentBalance": 1000000,
    "lastWithdrawalDate": null,
    "consultationDate": "2025-06-30T12:28:14.0154004Z"
  },
  "message": "Consulta de saldo realizada exitosamente"
}
```

**cURL:**
```bash
curl -X 'POST' \
  'http://localhost:3000/api/account/balance' \
  -H 'Authorization: Bearer <TU_TOKEN_AQUI>' \
  -H 'Content-Type: application/json' \
  -d '{
    "cardNumber": "4111111111111111"
  }'
```

---

### 4. 💸 Retiro de Dinero

**POST** `/account/withdraw`
🔒 **Requiere autenticación**

**Descripción:** Realiza un retiro de dinero de la cuenta.

**Headers:**
```
Authorization: Bearer <token_jwt>
Content-Type: application/json
```

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "amount": 500000
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "timestamp": "2025-06-30T12:28:54.1958291Z",
  "data": {
    "transactionId": "e221e7b5-ca41-45d4-b93d-cef086ef2b34",
    "withdrawnAmount": 500000,
    "previousBalance": 1000000,
    "newBalance": 500000,
    "transactionDate": "2025-06-30T12:28:54.1808504Z",
    "accountNumber": "2028586839"
  },
  "message": "Retiro de $500000.00 realizado exitosamente"
}
```

**cURL:**
```bash
curl -X 'POST' \
  'http://localhost:3000/api/account/withdraw' \
  -H 'Authorization: Bearer <TU_TOKEN_AQUI>' \
  -H 'Content-Type: application/json' \
  -d '{
    "cardNumber": "4111111111111111",
    "amount": 500000
  }'
```

---

### 5. 📊 Historial de Operaciones

**POST** `/account/operations`
🔒 **Requiere autenticación**

**Descripción:** Obtiene el historial paginado de transacciones.

**Headers:**
```
Authorization: Bearer <token_jwt>
Content-Type: application/json
```
**Query Parameters:**
```
pageNumber: 1
pageSize: 10
```

**Request Body:**
```json
{
  "cardNumber": "4111111111111111"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "timestamp": "2025-06-30T12:29:57.0695528Z",
  "data": {
    "transactions": [
      {
        "id": "e221e7b5-ca41-45d4-b93d-cef086ef2b34",
        "type": 1,
        "typeDescription": "Retiro",
        "amount": 500000,
        "description": "Retiro ATM - Confirmado",
        "date": "2025-06-30T12:28:54.18085Z",
        "balanceAfterTransaction": 500000,
        "status": 1
      },
      {
        "id": "bcbbd761-c9e6-475f-9ddb-629abbf1fb2b",
        "type": 2,
        "typeDescription": "Consulta de Saldo",
        "amount": 0,
        "description": "Consulta de saldo",
        "date": "2025-06-30T12:28:13.970634Z",
        "balanceAfterTransaction": null,
        "status": 1
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalRecords": 2,
      "totalPages": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    },
    "accountInfo": {
      "accountNumber": "2028586839",
      "accountHolderName": "Edwin Garcia",
      "currentBalance": 500000
    }
  },
  "message": "Historial de operaciones obtenido exitosamente"
}
```

**cURL:**
```bash
curl -X 'POST' \
  'http://localhost:3000/api/account/operations?pageNumber=1&pageSize=10' \
  -H 'Authorization: Bearer <TU_TOKEN_AQUI>' \
  -H 'Content-Type: application/json' \
  -d '{ "cardNumber": "4111111111111111" }'
```


### 6. 🔓 Desbloquear Tarjeta

**POST** `/auth/unblock`
🔒 **Requiere autenticación**

**Descripción:** Desbloquea una tarjeta previamente bloqueada.

**Headers:**
```
Authorization: Bearer <token_jwt>
Content-Type: application/json
```

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "documentNumber": "96069288",
  "newPin": "123",
  "confirmNewPin": "123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "timestamp": "2025-06-30T13:09:00.2614373Z",
  "data": {
    "cardNumber": "****-****-****-1111",
    "cardHolderName": "Edwin Garcia",
    "cardStatus": "Activa",
    "unblockDate": "2025-06-30T13:09:00.2611231Z",
    "message": "Tarjeta desbloqueada exitosamente"
  }
}
```



## 🔒 Autenticación y Seguridad

### Flujo de Autenticación

1. **Login**: El usuario envía número de tarjeta y PIN
2. **Token JWT**: Si es válido, recibe un token JWT con información de la tarjeta
3. **Requests autenticados**: Cada request debe incluir:
   - **Header**: `Authorization: Bearer <token>`
   - **Body**: Número de tarjeta (debe coincidir con el del token)

### Validación Dual

Todos los endpoints protegidos validan:
1. **Token JWT válido** en el header Authorization
2. **Número de tarjeta** en el body que coincida con el del token
3. **Tarjeta activa** y no bloqueada

### Bloqueo de Tarjetas

- Después de **4 intentos fallidos** de PIN, la tarjeta se bloquea automáticamente
- Las tarjetas bloqueadas no pueden realizar operaciones
- Se puede desbloquear usando el endpoint `/auth/unblock`


## 🚨 Códigos de Error

| Código | Descripción |
|--------|-------------|
| 400 | Bad Request - Datos de entrada inválidos |
| 401 | Unauthorized - Token faltante o inválido |
| 403 | Forbidden - Acceso a tarjeta no autorizada |
| 404 | Not Found - Recurso no encontrado |
| 409 | Conflict - Tarjeta o documento ya existe |
| 500 | Internal Server Error - Error interno |

### Mensajes de Error Comunes

```json
{
  "success": false,
  "timestamp": "2025-06-30T13:34:01.4929725Z",
  "error": "OPERACION_INVALIDA",
  "message": "Tarjeta no válida o inactiva",
  "details": {}
}
```

```json
{
  "success": false,
  "timestamp": "2025-06-29T15:56:12.1078861Z",
  "error": "PIN_INCORRECTO",
  "message": "PIN inválido. Intentos restantes: 3",
  "details": {
    "remainingAttempts": 3
  }
}
```
```json
{
  "success": false,
  "timestamp": "2025-06-30T13:33:42.8727733Z",
  "error": "TARJETA_BLOQUEADA",
  "message": "La tarjeta 4111111111111111 está bloqueada por múltiples intentos fallidos",
  "details": {
    "cardNumber": "4111111111111111"
  }
}
```
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ConfirmNewPin": [
      "La confirmación del PIN no coincide con el nuevo PIN"
    ],
    "DocumentNumber": [
      "El documento debe tener al menos 8 caracteres"
    ]
  },
  "traceId": "00-8fa0cd78028b34159c84811501920f12-1c337726b046701a-00"
}
{
  "success": false,
  "timestamp": "2025-06-30T13:36:04.2698798Z",
  "error": "NO_AUTORIZADO",
  "message": "No tiene autorización para realizar esta operación",
  "details": {}
}
```

### Flujo de Prueba Completo

1. **Registro de usuario**
   ```bash
   POST /api/user/register
   ```

2. **Login para obtener token**
   ```bash
   POST /api/auth/login
   ```

3. **Consultar saldo** (con token)
   ```bash
   POST /api/account/balance
   ```

4. **Realizar retiro** (con token)
   ```bash
   POST /api/account/withdraw
   ```

5. **Ver historial** (con token)
   ```bash
   POST /api/account/operations
   ```

## 🧪 Testing🧪 

### Health Check
```bash
curl http://
```bash
curl http://localhost:3000/localhost:3000/api/healthapi/health
```
**Descripción:** Verifica el estado de la API.

**Response (200 OK):**
```json
{
  "status": "Healthy",
  "timestamp": "2025-06-30T13:42:21.0985971Z",
  "version": "1.0.0",
  "environment": "Development",
  "checks": {
    "database": "Connected",
    "api": "Running"
  }
}
```

### Comandos Útiles

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run --project ChallengeApiAtm.Api

# Aplicar migraciones
dotnet ef database update --project ChallengeApiAtm.Infrastructure --startup-project ChallengeApiAtm.Api

# Crear nueva migración
dotnet ef migrations add <NombreMigracion> --project ChallengeApiAtm.Infrastructure --startup-project ChallengeApiAtm.Api
```

## 📊 Monitoreo y Logs

### Logs Disponibles

- **Intentos de login**: Exitosos y fallidos
- **Operaciones ATM**: Retiros, consultas de saldo
- **Seguridad**: Intentos de acceso no autorizados
- **Errores**: Stack traces completos
- **Performance**: Tiempo de respuesta de operaciones

### Acceso a Logs

```bash
# Ver logs en tiempo real
docker-compose logs -f api

# Ver logs de base de datos
docker-compose logs -f postgres

# Ver logs específicos
docker-compose logs api | grep "ERROR"
```

---

## 🗃️ Base de Datos

### PostgreSQL con Snake Case
- Usa la convención `snake_case` para nombres de tablas y columnas
- Tablas: `users`, `accounts`, `cards`, `transactions`
- Columnas: `first_name`, `last_name`, `account_id`, `card_id`, etc.

### Inicialización Automática
La aplicación maneja automáticamente la inicialización de la base de datos:

#### 🔄 **Migraciones (Todos los Ambientes)**
- Se aplican automáticamente al inicio de la aplicación
- Funciona tanto en **Development** como en **Production**

#### 📊 **Datos de Prueba (Solo Development)**
Los datos de prueba se cargan **ÚNICAMENTE** en ambiente `Development`:

**✅ Development (`ASPNETCORE_ENVIRONMENT=Development`)**
- Aplica migraciones + carga datos de prueba
- 4 usuarios, 4 cuentas, 4 tarjetas, 60 transacciones

**❌ Production (`ASPNETCORE_ENVIRONMENT=Production`)**
- Solo aplica migraciones
- NO carga datos de prueba

### Control de Ambiente
Para cambiar el comportamiento, modifica la variable de ambiente:

```bash
# Development - con datos de prueba
ASPNETCORE_ENVIRONMENT=Development

# Production - sin datos de prueba  
ASPNETCORE_ENVIRONMENT=Production
```

### Recrear Base de Datos
Si quieres empezar desde cero:

```bash
# Eliminar todo (BD + volúmenes)
docker-compose down -v

# Recrear desde 0
docker-compose up --build
```

## 👨‍💻 Autor

**Edwin Garcia**
- GitHub: [@edwingrow](https://github.com/Edwingrow)
- Email: edwingrow16@gmail.com
