# Microservices-based-Search-Service
This is a .Net 9 Microservices Project with implemetations for User Management, Email Service and Semantic Search Service. With YARP as the API Gateway


CREATE TABLE [dbo].[Users] (
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

INSERT INTO [dbo].[Users] (
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

 SELECT * FROM USERS;
