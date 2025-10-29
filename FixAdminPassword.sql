USE StudentRecordManagementSystem;
GO

-- Fix admin password hash to match C# SHA256 hash format EXACTLY
-- Password: admin123
-- C# hash: 240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9
-- This hash was verified by C# HashPassword method

-- Update admin password with the EXACT hash that C# generates
IF EXISTS (SELECT 1 FROM TblUser WHERE Email = 'admin@gmail.com')
BEGIN
    UPDATE TblUser 
    SET PasswordHash = '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9',
        IsDefaultPassword = 0
    WHERE Email = 'admin@gmail.com';
    
    PRINT 'Admin password hash updated successfully!';
    PRINT 'Using C# generated hash: 240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9';
END
ELSE
BEGIN
    INSERT INTO TblUser (Email, PasswordHash, UserRole, IsDefaultPassword)
    VALUES ('admin@gmail.com', '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9', 'Admin', 0);
    PRINT 'Admin user created successfully!';
END

PRINT '';
PRINT 'Verification:';
SELECT UserId, Email, UserRole, IsDefaultPassword, 
       PasswordHash as FullHash,
       CASE 
           WHEN PasswordHash = '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9' 
           THEN 'CORRECT - Matches C# hash'
           ELSE 'INCORRECT - Does not match C# hash'
       END as HashStatus
FROM TblUser 
WHERE Email = 'admin@gmail.com';

PRINT '';
PRINT '================================';
PRINT 'Login Credentials:';
PRINT 'Email: admin@gmail.com';
PRINT 'Password: admin123';
PRINT '================================';
GO
