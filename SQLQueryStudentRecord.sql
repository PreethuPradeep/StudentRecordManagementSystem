CREATE DATABASE StudentRecordManagementSystem;
GO
USE StudentRecordManagementSystem;
GO
---- Create tables
CREATE TABLE TblRole(
	RoleId INT PRIMARY KEY IDENTITY(1,1),
	RoleName NVARCHAR(20) NOT NULL UNIQUE,
	IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE TblStudent (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RollNumber INT NOT NULL UNIQUE,
    Name NVARCHAR(30) NOT NULL,
    Maths INT NOT NULL,
    Physics INT NOT NULL,
    Chemistry INT NOT NULL,
    English INT NOT NULL,
    Programming INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT CHK_Maths CHECK (Maths BETWEEN 1 AND 100),
    CONSTRAINT CHK_Physics CHECK (Physics BETWEEN 1 AND 100),
    CONSTRAINT CHK_Chemistry CHECK (Chemistry BETWEEN 1 AND 100),
    CONSTRAINT CHK_English CHECK (English BETWEEN 1 AND 100),
    CONSTRAINT CHK_Programming CHECK (Programming BETWEEN 1 AND 100)
);
GO

CREATE TABLE TblUser (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    UserRole NVARCHAR(20) NOT NULL,
    IsDefaultPassword BIT NOT NULL DEFAULT 1,
    StudentId INT NULL,
    CONSTRAINT FK_Users_Students FOREIGN KEY (StudentId) REFERENCES TblStudent(Id),
	CONSTRAINT FK_Users_Roles FOREIGN KEY (UserRole) REFERENCES TblRole(RoleName)
);
GO

INSERT INTO TblRole (RoleName) VALUES ('Admin'),('Invigilator'),('Student');
DECLARE @Password NVARCHAR(50) = 'admin123';
DECLARE @PasswordHash VARBINARY(32);
DECLARE @PasswordHashString NVARCHAR(256);

SET @PasswordHash = HASHBYTES('SHA2_256', @Password);
SET @PasswordHashString = CONVERT(NVARCHAR(256), @PasswordHash, 2);

-- insert the admin using the variable
INSERT INTO TblUser (Email, PasswordHash, UserRole, IsDefaultPassword)
VALUES (
    'admin@gmail.com',
    -- password: "admin123"
    @PasswordHashString,
    'Admin',
    0
);
GO