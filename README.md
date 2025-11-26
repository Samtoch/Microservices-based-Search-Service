# Microservices-based-Search-Service

<div align="center" id="Microservices Architecture">
	<a href="https://github.com/Samtoch/Microservices-based-Search-Service" title="Microservices Search Service/">
	  <img src="/Microservices Decorated nobg.drawio.png" alt="Microservices Architecture Banner" width="90%">
	</a>
</div>

This is a .Net 9 Microservices Project with implemetations for User Management, Email Service and Semantic Search Service. With YARP as the API Gateway

## API Gateway 
The API Gateway provides an entry point into the services, provides security (authentication and authorization), and manages rate limiting and caching.

## User Management
The User service has a collection of endpoints for user creation, update, deletion, query, signup, login, and password reset.
The user service communicates with the email service using the RabbitMQ message broker such that signup and password reset notifications are sent through the messaging queue, and a failure of the email service does not stop the user service, and whenever the email service is up, all the messages on the queue get delivered. 

## Email Service
The email service provides endpoints for sending mail to users on signup and password reset.

## Search Service
The Semantic Search service provides endpoints for document uploading, guided context searching, and direct search of LLM

### Highlight
Each of the services is independent and can be containerized in Docker and hosted on the Azure cloud or IIS.
To ensure security, the JWT was implemented to protect all the APIs, and all the sensitive properties are stored in an environmental variable instead of the traditional appSetting.json
To ensure performance, the Get APIs have rate limiting, cache, and pagination.

# Prerequisites for running the project locally
## User Management Service
- .NET 9 SDK
- SQL Server
- RabbitMQ Server running locally on docker running on default port 5672
- Visual Studio 2022 or later / VS Code
- Postman or any API testing tool
- Docker (optional, for containerization)
- Swagger (for API documentation)
- Entity Framework Core (for database interactions)
- ASP.NET Core Identity (for user authentication and authorization)
- AutoMapper (for object mapping)
- FluentValidation (for input validation)
- NLog (for logging)
- Moq (for unit testing)
- NUnit (for unit testing framework)
- Swashbuckle (for Swagger integration)

### SQL Script to create AppUsers table and insert sample data
CREATE TABLE [dbo].[AppUsers] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserName] NVARCHAR(100) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Phone] NVARCHAR(50) NULL,
    [Role] NVARCHAR(50) NULL,
    [FirstName] NVARCHAR(100) NULL,
    [LastName] NVARCHAR(100) NULL,
    [ModifiedDate] DATETIME NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL,
    [DateOfBirth] DATETIME NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

INSERT INTO [dbo].[AppUsers] (
    [Id], [UserName], [Password], [Email], [Phone], [Role],
    [FirstName], [LastName], [ModifiedDate], [CreatedDate],
    [IsActive], [DateOfBirth], [IsDeleted]
)
VALUES
(NEWID(), 'john_doe', 'Password123!', 'john.doe@example.com', '07123456789', 'Admin',
 'John', 'Doe', NULL, GETDATE(), 1, '1985-06-15', 0),

(NEWID(), 'jane_smith', 'SecurePass456!', 'jane.smith@example.com', '07987654321', 'User',
 'Jane', 'Smith', NULL, GETDATE(), 1, '1990-09-20', 0),

(NEWID(), 'alex_brown', 'MyPass789!', 'alex.brown@example.com', '07812345678', 'Manager',
 'Alex', 'Brown', NULL, GETDATE(), 1, '1988-03-10', 0);

SELECT * FROM APPUSERS;

## Email Service
- .NET 9 SDK
- SMTP Server (e.g., Gmail SMTP, SendGrid)
- Visual Studio 2022 or later / VS Code
- Postman or any API testing tool
- RabbitMQ Server running locally on docker running on default port 5672
- NUnit (for unit testing framework)
- Swashbuckle (for Swagger integration)
- NLog (for logging)

### Configuring RabbitMQ 
- Install Docker Desktop
- Pull RabbitMQ and map the ports using the below command
- docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
- browse rabbitmq GUI using http://localhost:15672
- login using username: guest and password: guest


## Search Service
- .NET 9 SDK
- Visual Studio 2022 or later / VS Code
- Postman or any API testing tool
- Docker (optional, for containerization)
- NUnit (for unit testing framework)
- Swashbuckle (for Swagger integration)
- NLog (for logging)
- Azure Cognitive Services (for semantic search capabilities)
- OpenAI API (for LLM integration)
- QdrantDB (for vector database storage)

## API Gateway
- .NET 9 SDK
- Visual Studio 2022 or later / VS Code
- Postman or any API testing tool
- YARP (for API Gateway)
- Swashbuckle (for Swagger integration)
- NLog (for logging)
- JWT (for secure token-based authentication)
- Health Checks (for monitoring application health)
- Rate Limiting Middleware (for controlling API request rates)
- Caching Middleware (for improving API response times)
- Swagger UI (for API documentation)
- OpenAPI (for API specification)

### API Gateway Login Credentials 
http://localhost:5083/swagger/index.html
{
  "username": "admin",
  "password": "password"
}

# Swagger UI for all the Services

<div align="center" id="Microservices Swagger Documentation">
	<a href="https://github.com/Samtoch/Microservices-based-Search-Service" title="Microservices Search Service/">
	  <img src="/Microservices Swagger UI.png" alt="Microservices UI" width="90%">
	</a>
</div>
