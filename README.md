# 🏗️ E-commerce Nexus: MVP de Arquitectura de Microservicios

**E-commerce Nexus** es un sistema de gestión de comercio electrónico diseñado para demostrar el dominio de la arquitectura de software moderna, microservicios y tecnologías full-stack. Este proyecto implementa un entorno de e-commerce completo siguiendo las mejores prácticas de la industria.

## 🎯 Visión del Sistema

Desarrollar una plataforma de e-commerce mínima pero integral que demuestre experiencia técnica en:
- **Arquitectura de Microservicios**
- **Domain-Driven Design (DDD)**
- **Test-Driven Development (TDD)**
- **Event-Driven Architecture**
- **Persistencia Políglota**
- **Observabilidad y Monitoreo**
- **Containerización y Orquestación**

## 🧩 Contextos de Dominio (DDD)

### Contextos Principales:
1. **Product Catalog** - Gestión de productos e inventario
2. **Order Management** - Carritos de compra y procesamiento de pedidos
3. **User Management** - Autenticación y perfiles de usuario
4. **Search & Discovery** - Búsqueda de productos via Elasticsearch

### Contextos de Soporte:
5. **Notification** - Notificaciones por email/SMS
6. **Analytics** - Métricas de usuario y comportamiento

## 🏛️ Arquitectura de Microservicios

### Servicios Principales:
- **Product Service** (Node.js/TypeScript) - CRUD de productos e inventario ✅
- **Order Service** (.NET 6) - Gestión de carritos y pedidos 🚧
- **User Service** (Go) - Autenticación y gestión de usuarios
- **Search Service** (Node.js/TypeScript) - Integración con Elasticsearch

### Servicios de Soporte:
- **Notification Service** (Go) - Mensajería y notificaciones
- **Analytics Service** (Go) - Métricas y seguimiento de comportamiento

## 🔌 API RESTful

### Endpoints Estandarizados:
- **Products**: `/api/v1/products` - CRUD completo + gestión de inventario ✅
- **Orders**: `/api/v1/orders` - Gestión de carritos y procesamiento de pedidos
- **Users**: `/api/v1/auth` & `/api/v1/users` - Autenticación y perfiles
- **Search**: `/api/v1/search` - Búsqueda de productos y facetas

## 🗄️ Persistencia Políglota

- **PostgreSQL** - Datos transaccionales (usuarios, productos, pedidos)
- **MongoDB** - Datos flexibles (carritos activos, comportamiento de usuario)
- **Elasticsearch** - Índice de búsqueda y analytics
- **Redis** - Cache y sesiones

## 🚀 Arquitectura Orientada a Eventos

### Topics de RabbitMQ:
- `product.inventory.updated`
- `order.created` → `order.confirmed`
- `user.registered`
- `search.query.executed`

## 📊 Stack de Observabilidad

- **Prometheus/Grafana** - Métricas y monitoreo
- **ELK Stack** - Logging y análisis
- **Kubernetes** - Orquestación de contenedores

## 🛠️ Stack Tecnológico

### Lenguajes y Frameworks:
- **Backend**: Node.js/TypeScript + .NET 6 + Go
- **Frameworks**: Express.js, ASP.NET Core, Gin, Fiber
- **Testing**: Jest, xUnit, FluentAssertions, Go testing
- **CI/CD**: GitHub Actions
- **Deployment**: Docker + Kubernetes

### Bases de Datos:
- **PostgreSQL 15** - Base de datos principal
- **MongoDB 6.0** - Documentos y datos flexibles
- **Elasticsearch 8.9** - Motor de búsqueda
- **Redis 7** - Cache en memoria

### Infraestructura:
- **RabbitMQ 3** - Message broker
- **Docker** - Containerización
- **Kubernetes** - Orquestación
- **Prometheus/Grafana** - Monitoreo

## 📁 Estructura del Proyecto

```
ecommerce-nexus/
├── services/                    # 6 microservicios
│   ├── product-service/         # Completado
│   ├── order-service/           # En desarrollo
│   ├── user-service/            # Pendiente
│   ├── search-service/          # Pendiente
│   ├── notification-service/    # Pendiente
│   └── analytics-service/       # Pendiente
├── shared/                      # Modelos y utilidades comunes
│   ├── models/
│   ├── utils/
│   └── config/
├── infrastructure/              # K8s, Docker, monitoreo
│   ├── docker/                  # Docker Compose
│   ├── k8s/                     # Configuraciones K8s
│   └── monitoring/              # Prometheus config
├── docs/                        # Especificaciones API y arquitectura
│   ├── api/
│   └── architecture/
└── tests/                       # Tests unitarios, integración, e2e
    ├── unit/
    ├── integration/
    └── e2e/
```

## 🚀 Inicio Rápido

### Prerrequisitos
- Docker & Docker Compose
- Node.js 18+
- .NET 6 SDK
- Go 1.21+
- Git

### 1. Clonar el Repositorio
```bash
git clone <repository-url>
cd ecommerce-nexus
```

### 2. Configurar Variables de Entorno
```bash
# Copiar archivos de ejemplo
cp services/product-service/.env.example services/product-service/.env
# Editar las variables según tu entorno
```

### 3. Iniciar Infraestructura
```bash
# Iniciar bases de datos y servicios de soporte
docker-compose -f infrastructure/docker/docker-compose.yml up -d postgres mongodb elasticsearch rabbitmq redis prometheus grafana
```

### 4. Ejecutar Product Service
```bash
cd services/product-service
npm install
npm run dev
```

### 5. Ejecutar Order Service (.NET 6)
```bash
cd services/order-service
dotnet restore
dotnet test  # Ejecutar tests TDD
dotnet run --project src/OrderService.API
```

### 6. Verificar Servicios
- **Product Service**: http://localhost:3001/health
- **Order Service**: http://localhost:3002/health
- **API Products**: http://localhost:3001/api/v1/products
- **API Orders**: http://localhost:3002/api/v1/orders
- **Métricas**: http://localhost:3001/metrics
- **Grafana**: http://localhost:3000 (admin/admin)
- **RabbitMQ Management**: http://localhost:15672 (admin/password)

## 🧪 Testing

### Ejecutar Tests - Product Service (Node.js)
```bash
cd services/product-service
# Tests unitarios
npm run test

# Tests con cobertura
npm run test:coverage

# Tests en modo watch
npm run test:watch
```

### Ejecutar Tests - Order Service (.NET 6)
```bash
cd services/order-service
# Tests unitarios
dotnet test

# Tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Tests en modo watch
dotnet watch test
```

### TDD Implementado
#### Product Service ✅
- ✅ Tests unitarios para entidades de dominio
- ✅ Tests de casos de uso con mocks
- ✅ Tests de integración para repositorios
- ✅ Cobertura de código completa

#### Order Service 🚧
- ✅ Tests TDD para Value Objects (Money)
- ✅ Tests de validación y reglas de negocio
- ✅ Arquitectura DDD con Clean Architecture
- 🚧 Tests para entidades y agregados (en desarrollo)

## 📊 Monitoreo y Observabilidad

### Métricas Disponibles:
- **HTTP Requests**: Latencia, throughput, códigos de estado
- **Business Metrics**: Productos creados, stock actualizado
- **Database Metrics**: Consultas, latencia de BD
- **System Metrics**: CPU, memoria, GC

### Dashboards:
- **Grafana**: Dashboards personalizados para cada servicio
- **Prometheus**: Métricas detalladas y alertas

## 🔧 API del Product Service

### Endpoints Principales:

#### Crear Producto
```bash
POST /api/v1/products
Content-Type: application/json

{
  "name": "iPhone 15 Pro",
  "description": "Latest iPhone with A17 Pro chip",
  "price": 999.99,
  "sku": "IPHONE-15-PRO-001",
  "category": "Electronics",
  "stock": 50
}
```

#### Listar Productos con Filtros
```bash
GET /api/v1/products?category=Electronics&inStock=true&page=1&limit=10
```

#### Actualizar Stock
```bash
PATCH /api/v1/products/{id}/stock
Content-Type: application/json

{
  "operation": "decrease",
  "quantity": 5
}
```

## 🔧 API del Order Service (.NET 6)

### Endpoints Principales:

#### Crear Pedido
```bash
POST /api/v1/orders
Content-Type: application/json

{
  "customerId": "c7f9d8e1-2345-6789-abcd-1234567890ab",
  "items": [
    {
      "productId": "product-123",
      "quantity": 2,
      "unitPrice": {
        "amount": 25.99,
        "currency": "USD"
      }
    }
  ]
}
```

#### Obtener Pedidos por Cliente
```bash
GET /api/v1/orders/customer/{customerId}?status=pending&page=1&limit=10
```

#### Actualizar Estado de Pedido
```bash
PATCH /api/v1/orders/{orderId}/status
Content-Type: application/json

{
  "status": "confirmed",
  "reason": "Payment processed successfully"
}
```

### Arquitectura DDD Implementada:
- **Domain Layer**: Value Objects (Money), Entities, Aggregates
- **Application Layer**: Use Cases, Commands, Queries
- **Infrastructure Layer**: Entity Framework Core, RabbitMQ
- **API Layer**: ASP.NET Core Web API

## ✅ Estado de Implementación

### Completado ✅
- [x] Estructura de proyecto con microservicios
- [x] Infraestructura Docker completa
- [x] Product Service con DDD y TDD (Node.js/TypeScript)
- [x] Order Service - Estructura .NET 6 con Clean Architecture
- [x] Order Service - Domain Layer: Value Objects (Money) con TDD
- [x] Order Service - Abstracciones comunes (Entity, ValueObject, IDomainEvent)
- [x] API RESTful con validaciones
- [x] PostgreSQL con migraciones
- [x] RabbitMQ para eventos
- [x] Métricas con Prometheus
- [x] Dockerización optimizada

### En Desarrollo 🚧
- [ ] Order Service (.NET 6) - Domain Layer completo con TDD
- [ ] User Service (Go)
- [ ] Search Service con Elasticsearch

### Pendiente ⏳
- [ ] Notification Service (Go)
- [ ] Analytics Service (Go)
- [ ] Configuraciones Kubernetes
- [ ] Pipeline CI/CD
- [ ] Tests E2E
- [ ] Documentación OpenAPI

## 📄 License

This project is licensed under the Apache-2.0 License - see the [LICENSE](LICENSE) file for details.

---

*E-commerce Nexus v1.0.0 | MVP de Arquitectura de Microservicios 
