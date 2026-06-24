# MonoMarket API

Backend de facturación, desarrollado en .NET 9. Escanea facturas vencidas, las avanza por etapas de recordatorio (primer recordatorio, segundo recordatorio, desactivación) y notifica al cliente por correo.

## TECNOLOGÍAS UTILIZADAS

.NET 9 - ASP.NET Core Web API - MongoDB - CQRS (MediatR) - FluentValidation - Polly - Swagger - Docker / Docker Compose

## PRERREQUISITOS

- Docker
- Docker Compose
- .NET 9 SDK (solo para correr la API en local en modo desarrollo)

## ARQUITECTURA

```
MonoMarket.Commons         
MonoMarket.Domain        
MonoMarket.Application
MonoMarket.Infrastructure  
MonoMarket.WebApi        
docker/ (Mongo + API)
```

Descripción de capas:

- **Domain**: entidades y reglas de negocio.
- **Application**: casos de uso (Queries), validaciones y lógica orquestadora de recordatorios.
- **Infrastructure**: acceso a datos (MongoDB) y envío de correo.
- **WebApi**: Controllers, middleware y job en background.

Patrones implementados:
- CQRS
- Strategy Pattern (etapas de recordatorio)
- Factory Pattern (resolución dinámica de etapa y de plantilla de correo)
- Adapter + Decorator (envío de correo con reintentos)
- Middleware global de excepciones

## CONFIGURACIÓN DE BASE DE DATOS

La base de datos es MongoDB y se crea automáticamente al levantar el contenedor (colecciones, índices y datos de prueba), no requiere scripts manuales.

## EJECUCIÓN DEL PROYECTO

**Desarrollo** (Mongo + API, dos contenedores para ejecución completa):

```bash
docker compose up -d
cd mono.market
dotnet restore
dotnet run --project MonoMarket.WebApi
```

**Producción** (Mongo + API, dos contenedores):

```bash
cp .env.example .env
docker compose up -d
```

Swagger estará disponible en:

```
http://localhost:8080/swagger
```

## SIMULACIÓN DE ENVÍO DE CORREOS

El envío de correo se prueba contra el servidor público de **Yopmail** (`smtp.yopmail.com`, sin autenticación). Cualquier correo enviado a una dirección `@yopmail.com` puede verse de inmediato en `https://yopmail.com/` con esa misma dirección. Los clientes de la data de prueba ya usan este tipo de correo (ej. `monomarket-pruebas-beta@yopmail.com`).

## REGLAS DE NEGOCIO

- Una factura avanza `Pending` → `FirstReminder` → `SecondReminder` → `Deactivated` según días transcurridos desde su emisión (30 / 60 / 90 días).
- `Pending → FirstReminder` es silenciosa (sin correo); las demás transiciones envían correo con su plantilla.
- El estado solo avanza si no dependía de correo, o si el correo se envió con éxito; si falla, se reintenta en la siguiente corrida.
- El job de escaneo corre solo al iniciar la app y se repite cada día; también se puede controlar manualmente vía `/api/jobs/invoice-reminder-scan`.

## NOTAS

- No se implementa autenticación ni autorización (fuera del alcance).
- No se implementa paginación en los endpoints públicos de consulta.
- No se configuran health checks, CORS ni rate limiting.
