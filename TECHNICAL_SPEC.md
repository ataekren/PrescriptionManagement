# Technical Specification Document

## 1. Service Specifications

### 1.1 API Gateway
- **Technology**: .NET 7.0+ with Ocelot
- **Features**:
  - JWT validation and authentication
  - Request routing
  - Rate limiting
  - Request aggregation
  - Load balancing
- **Configuration**:
  ```json
  {
    "Routes": [
      {
        "DownstreamPathTemplate": "/api/v1/medicines/{everything}",
        "DownstreamScheme": "http",
        "DownstreamHostAndPorts": [
          {
            "Host": "medicineservice",
            "Port": 80
          }
        ],
        "UpstreamPathTemplate": "/api/v1/medicines/{everything}",
        "UpstreamHttpMethod": [ "GET", "POST", "PUT" ]
      }
    ]
  }
  ```

### 1.2 Medicine Service
- **Database**: MongoDB
- **Cache**: Redis
- **Key Components**:
  - Medicine Repository
  - Excel Parser for weekly updates
  - Search Service with autocomplete
- **Data Models**:
  ```csharp
  public class Medicine
  {
      public string Id { get; set; }
      public string Name { get; set; }
      public decimal Price { get; set; }
      public string Barcode { get; set; }
      public DateTime LastUpdated { get; set; }
      public bool IsActive { get; set; }
  }
  ```

### 1.3 Prescription Service
- **Database**: SQL Server
- **Authentication**: JWT
- **Key Components**:
  - Prescription Manager
  - Visit Recorder
  - TC Verification Service
- **Data Models**:
  ```csharp
  public class Prescription
  {
      public int Id { get; set; }
      public string PatientTc { get; set; }
      public string DoctorId { get; set; }
      public DateTime CreatedDate { get; set; }
      public List<PrescriptionItem> Items { get; set; }
      public PrescriptionStatus Status { get; set; }
  }
  ```

### 1.4 Notification Service
- **Message Queue**: RabbitMQ
- **Email Service**: SendGrid/SMTP
- **Key Components**:
  - Queue Consumer
  - Email Template Engine
  - Report Generator
- **Message Format**:
  ```json
  {
    "pharmacyId": "string",
    "prescriptionId": "string",
    "missingMedicines": ["string"],
    "notificationDate": "datetime"
  }
  ```

## 2. Data Flow Specifications

### 2.1 Prescription Creation Flow
1. Doctor authenticates via API Gateway
2. Creates prescription through Prescription Service
3. Service validates TC number
4. Prescription stored in database
5. Event published to RabbitMQ

### 2.2 Medicine Update Flow
1. Scheduled job triggers Sunday at 22:00
2. Downloads latest Excel file
3. Parses medicine data
4. Updates MongoDB
5. Invalidates Redis cache
6. Logs update statistics

### 2.3 Notification Flow
1. Scheduled job runs at 01:00
2. Queries incomplete prescriptions
3. Groups by pharmacy
4. Generates reports
5. Sends via email service

## 3. API Specifications

### 3.1 Medicine API
```yaml
openapi: 3.0.0
paths:
  /api/v1/medicines/search:
    get:
      parameters:
        - name: query
          in: query
          required: true
          schema:
            type: string
      responses:
        '200':
          description: Success
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Medicine'
```

### 3.2 Prescription API
```yaml
openapi: 3.0.0
paths:
  /api/v1/prescriptions:
    post:
      security:
        - bearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/PrescriptionCreate'
```

## 4. Security Specifications

### 4.1 Authentication
- JWT token structure:
  ```json
  {
    "iss": "auth-service",
    "sub": "user-id",
    "role": ["doctor"|"pharmacy"],
    "exp": 1735689600
  }
  ```

### 4.2 Authorization
- Role-based access control:
  - Doctors: Prescription creation
  - Pharmacies: Prescription submission
  - System: Internal service communication

### 4.3 Data Protection
- TLS 1.3 for all communications
- Data encryption at rest
- Secure key management
- Regular security audits

## 5. Infrastructure Specifications

### 5.1 Docker Configuration
```yaml
version: '3.8'
services:
  api-gateway:
    build: ./Infrastructure/APIGateway
    ports:
      - "80:80"
    depends_on:
      - medicine-service
      - prescription-service

  medicine-service:
    build: ./Services/MedicineService
    environment:
      - MONGODB_CONNECTION=mongodb://mongodb:27017
      - REDIS_CONNECTION=redis:6379

  prescription-service:
    build: ./Services/PrescriptionService
    environment:
      - SQL_CONNECTION=Server=sql-server;Database=Prescriptions
```

### 5.2 Monitoring
- Health check endpoints for each service
- Prometheus metrics collection
- Grafana dashboards
- ELK stack for logging

### 5.3 Scaling
- Horizontal scaling for services
- Redis cluster for caching
- MongoDB replication
- Load balancing configuration

## 6. Development Guidelines

### 6.1 Code Standards
- Clean Architecture principles
- SOLID principles
- REST API best practices
- Comprehensive unit testing
- Integration testing

### 6.2 Documentation
- API documentation with Swagger
- Code documentation
- Architecture decision records
- Deployment guides

### 6.3 CI/CD
- GitHub Actions workflow
- Automated testing
- Docker image building
- Deployment automation

## 7. Performance Requirements

### 7.1 Service Level Objectives
- API Response Time: < 200ms
- Search Autocomplete: < 100ms
- Availability: 99.9%
- Error Rate: < 0.1%

### 7.2 Caching Strategy
- Redis TTL configuration
- Cache invalidation rules
- Cache warming procedures
- Memory usage limits 