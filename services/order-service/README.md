# Order Service - E-commerce Nexus

## 📋 Descripción
Microservicio para gestión de pedidos y carritos de compra del sistema E-commerce Nexus, implementado en .NET 6 siguiendo principios DDD (Domain-Driven Design) y arquitectura limpia.

## 🏗️ Arquitectura

### Stack Tecnológico
- **.NET 6.0** - Framework principal
- **ASP.NET Core 6** - Web API
- **Entity Framework Core 6** - ORM para PostgreSQL
- **MongoDB.Driver** - Base de datos para carritos
- **MassTransit + RabbitMQ** - Mensajería y eventos
- **xUnit + FluentAssertions** - Testing
- **Serilog** - Logging estructurado
- **Prometheus.NET** - Métricas

### Bounded Context: Order Management

#### Agregados de Dominio
- **Order** (Aggregate Root) - Gestión de pedidos
- **Cart** (Aggregate Root) - Carritos de compra

#### Value Objects
- **Money** - Representación monetaria
- **OrderItem** - Items de pedidos
- **Address** - Direcciones de entrega

#### Eventos de Dominio
- OrderPlacedEvent
- OrderConfirmedEvent
- CartItemAddedEvent
- CartConvertedToOrderEvent

## 📁 Estructura del Proyecto

```
src/
├── OrderService.Domain/          # Capa de Dominio (DDD)
├── OrderService.Application/     # Capa de Aplicación (Use Cases)
├── OrderService.Infrastructure/  # Capa de Infraestructura
└── OrderService.API/            # Capa de Presentación (Web API)

tests/
├── OrderService.Domain.Tests/
└── OrderService.Application.Tests/
```

## 🚀 Getting Started

### Prerrequisitos
- .NET 6.0 SDK
- PostgreSQL 15
- MongoDB 6.0
- RabbitMQ 3

### Configuración Local
```bash
# Restaurar dependencias
dotnet restore

# Ejecutar tests
dotnet test

# Ejecutar aplicación
dotnet run --project src/OrderService.API
```

## 🔗 API Endpoints

### Orders
- `POST /api/v1/orders` - Crear pedido
- `GET /api/v1/orders/{id}` - Obtener pedido
- `GET /api/v1/orders` - Listar pedidos
- `PATCH /api/v1/orders/{id}/status` - Actualizar estado

### Carts
- `GET /api/v1/carts` - Obtener carrito
- `POST /api/v1/carts/items` - Agregar item
- `PUT /api/v1/carts/items/{id}` - Actualizar item
- `DELETE /api/v1/carts/items/{id}` - Remover item
- `POST /api/v1/carts/checkout` - Convertir a pedido

## 📊 Métricas y Observabilidad
- Health checks: `/health`
- Métricas Prometheus: `/metrics`
- Swagger UI: `/swagger`

## 🧪 Testing
- **Tests Unitarios**: xUnit + Moq
- **Tests de Integración**: TestContainers
- **Cobertura**: >90% target

---
*Order Service v1.0.0 | .NET 6 | DDD Architecture*