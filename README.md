# Prescription Management System

## Overview
A comprehensive prescription and doctor visit management system for Saglik Bakanligi pharmacies. The system facilitates prescription creation by doctors, prescription management by pharmacies, and includes medicine lookup capabilities with automated notifications for incomplete prescriptions.

## System Architecture
The system is built using a microservices architecture with the following components:

### Core Services
1. **API Gateway**
   - Single entry point for all client requests
   - Routes requests to appropriate microservices
   - Handles authentication and cross-cutting concerns
   - Implements versioning and pagination

2. **Medicine Service**
   - Manages medicine database
   - Provides medicine search with autocomplete
   - Weekly updates from Saglik Bakanligi (Sundays at 22:00)
   - NoSQL storage for medicine data

3. **Prescription Service**
   - Handles prescription creation by doctors
   - Manages patient visit records
   - JWT authentication for doctors
   - Prescription submission for pharmacies

4. **Notification Service**
   - Monitors incomplete prescriptions
   - Sends daily reports to pharmacies at 01:00
   - Queue-based asynchronous processing

### Infrastructure Components
- **Redis Cache**: For medicine names and frequently accessed data
- **RabbitMQ**: Message queue for asynchronous operations
- **NoSQL Database**: For medicine data storage
- **JWT Authentication**: For secure API access

## Key Features

### For Doctors
- Secure prescription creation
- Patient visit recording
- JWT-based authentication
- TC number verification via mock API

### For Pharmacies
- Prescription submission system
- Medicine search with autocomplete
- Authentication system
- Incomplete prescription notifications
- Price management for medicines

### System Features
- Automated medicine list updates
- Nightly prescription status notifications
- Caching for performance optimization
- Queue-based processing
- API versioning and pagination

## Technical Requirements

### Development
- .NET Core latest version
- Docker support
- Cloud-ready architecture
- API Gateway implementation
- Caching with Redis
- Message Queue (RabbitMQ/Azure Messaging)
- NoSQL database for medicine data

### Authentication
- JWT-based authentication
- Role-based access control
- Secure API endpoints

### API Design
- RESTful API architecture
- Versioning support
- Pagination implementation
- Swagger/OpenAPI documentation

## Project Structure
```
PrescriptionManagementSystem/
├── Services/
│   ├── MedicineService/
│   │   └── [Medicine management implementation]
│   ├── PrescriptionService/
│   │   └── [Prescription handling implementation]
│   └── NotificationService/
│       └── [Notification system implementation]
├── Infrastructure/
│   ├── APIGateway/
│   │   └── [API Gateway configuration]
│   ├── RedisCacheService/
│   │   └── [Caching implementation]
│   └── RabbitMQService/
│       └── [Message queue implementation]
├── Shared/
│   └── SharedKernel/
│       └── [Shared components and utilities]
└── AuthService/
    └── [Authentication implementation]
```

## Setup and Deployment

### Prerequisites
- .NET Core SDK
- Docker
- Redis
- RabbitMQ
- NoSQL Database (MongoDB recommended)

### Development Setup
1. Clone the repository
2. Install dependencies
3. Configure environment variables
4. Start required services (Redis, RabbitMQ)
5. Run the application

### Docker Deployment
```bash
docker-compose up -d
```

## API Documentation

### Medicine Service API
- `GET /api/v1/medicines/search` - Search medicines with autocomplete
- `POST /api/v1/medicines/update` - Update medicine database

### Prescription Service API
- `POST /api/v1/prescriptions` - Create new prescription
- `GET /api/v1/prescriptions` - List prescriptions
- `PUT /api/v1/prescriptions/{id}` - Update prescription

### Authentication API
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/verify` - Verify token

## Security Considerations
- JWT token-based authentication
- Role-based access control
- Secure API gateway
- Data encryption
- Rate limiting

## Monitoring and Maintenance
- Logging implementation
- Performance monitoring
- Error tracking
- Regular backups
- System health checks

## Contributing
[Contributing guidelines]

## License
[License information] 