## 🔗 Links  
- [Demo Video Link](https://youtu.be/aRHAeW3X7GM)
- [Frontend Deployment](https://lylcwhwerzri68df.vercel.app/)
- [GitHub Repository Link](https://github.com/ataekren/PrescriptionManagement)

# Prescription Management System

A modern, microservices-based prescription management system designed to streamline the process of managing medical prescriptions, connecting patients, doctors, and pharmacies.

## Project Overview

The Prescription Management System is a comprehensive solution built using .NET microservices architecture with a modern frontend. The system facilitates secure prescription handling, user authentication, and seamless communication between healthcare providers and patients.

## 📊 Data Model

![ER](https://github.com/ataekren/PrescriptionManagement/blob/main/ER.png?raw=true)

## Architecture

The system follows a microservices architecture pattern with the following components:

### Backend Services
- **API Gateway**: Routes and manages requests to appropriate microservices
- **Authentication Service**: Handles user authentication, authorization, and user management
- **Medicine Service**: Manages medicine catalog, inventory, and medicine-related operations
- **Prescription Service**: Handles prescription creation, management, and tracking

### Frontend
- Modern web application built with Next.js
- Responsive design for optimal user experience

## Design

### User Roles
The system implements two distinct user roles:

**Doctor**
- Add medicines to prescription list and issue prescriptions

**Pharmacist**
- Process prescriptions
- Update prescription status

### Security
- JWT-based authentication with configurable token expiration
- Role-based authorization
- Data encryption for sensitive information
- Secure API communication between services

### Key Entities
- **Users**
- **Prescriptions**
- **Medications**
- **Prescription Items**
- **Pharmacies**
- **Submissions**

### Use Cases

**Prescription Management**
- Doctors create and issue prescriptions
- Pharmacists process prescriptions
- System tracks prescription status

**Medicine Management**
- Sync medicine database with external source
- Track medicine inventory

**User Management**
- User registration and authentication
- Role-based access control
- Profile management
- Password recovery

## Technology Stack

- **Backend**: 
  - .NET Core
  - Microservices Architecture
  - Docker Containerization
  - Entity Framework Core
  
- **Frontend**:
  - Next.js 14
  - React 18
  - Tailwind CSS
  - Radix UI Components
  - TypeScript

## Getting Started

### Prerequisites
- .NET SDK
- Docker and Docker Compose
- Your preferred IDE (Visual Studio recommended)

### Installation

1. Clone the repository:
```bash
git clone [repository-url]
```

2. Navigate to the project directory:
```bash
cd PrescriptionManagementSystem
```

3. Start the backend services using Docker Compose:
```bash
docker-compose up
```

4. Start the frontend development server:
```bash
cd frontend/prescriptionsystem
npm install
npm run dev
```

## Project Structure

```
PrescriptionManagementSystem/
├── .github/                    # GitHub workflows and CI/CD configurations
├── frontend/                   # Frontend application
│   └── prescriptionsystem/    # Next.js frontend application
├── src/
│   ├── Services/              # Microservices
│   │   ├── APIGateway/       # API Gateway for routing requests
│   │   ├── AuthService/      # Authentication and user management
│   │   ├── MedicineService/  # Medicine catalog and inventory
│   │   └── PrescriptionService/ # Prescription management
│   └── Shared/               # Shared libraries and utilities
├── docker-compose.yml        # Docker composition configuration
└── README.md                 # Project documentation
```

## Development

To run the project locally for development:

1. Start the backend services:
```bash
docker-compose up
```

2. Start the frontend development server:
```bash
cd frontend/prescriptionsystem
npm install
npm run dev
```

The frontend application will be available at `http://localhost:3000`
