DROP TABLE Rating;
DROP TABLE Post;
DROP TABLE Thread;
DROP TABLE Member;
DROP TABLE Groups;
DROP TABLE Users;
DROP PROCEDURE DeleteUser;
DROP TRIGGER CheckDuplicateMembers;

-- TABLES

CREATE TABLE Users (
    Email VARCHAR(255) NOT NULL PRIMARY KEY,
    Password VARCHAR(255) NOT NULL,
    Handle VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Role INT NOT NULL,
    VisibilityRegistered BOOLEAN DEFAULT TRUE,
    VisibilityGuest BOOLEAN DEFAULT TRUE,
    VisibilityGroup BOOLEAN DEFAULT TRUE,
    Icon VARCHAR(255)
);

CREATE TABLE `Groups` (
    Handle VARCHAR(255) NOT NULL PRIMARY KEY,
    Description VARCHAR(255),
    Name VARCHAR(255) NOT NULL,
    Admin VARCHAR(255) NOT NULL,
    VisibilityMember BOOLEAN DEFAULT TRUE,
    VisibilityGuest BOOLEAN DEFAULT TRUE,
    Icon VARCHAR(255),
    FOREIGN KEY (Admin) REFERENCES Users(Email)
);

CREATE TABLE Member (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    GroupRole INT NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email),
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle)
);

CREATE TABLE Thread (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Date DATETIME NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email),
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle)
);

CREATE TABLE Post (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    ThreadId VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Text VARCHAR(255) NOT NULL,
    Date DATETIME NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email),
    FOREIGN KEY (ThreadId) REFERENCES Thread(Id)
);

CREATE TABLE Rating (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Rating BOOLEAN,  
    PostId VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email),
    FOREIGN KEY (PostId) REFERENCES Post(Id)
);

-- PROCEDURES

DELIMITER //

CREATE PROCEDURE DeleteUser(IN userEmail VARCHAR(255))
BEGIN
    DECLARE isAdmin INT DEFAULT 0;
    
    SELECT COUNT(*) INTO isAdmin FROM `Groups` WHERE Admin = userEmail;

    IF isAdmin > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'User is an admin and cannot be deleted';
    ELSE
        DELETE FROM Member WHERE Email = userEmail;

        DELETE FROM Users WHERE Email = userEmail;
    END IF;
END //

DELIMITER ;

-- TRIGGERS

DELIMITER //

CREATE TRIGGER CheckDuplicateMembers
BEFORE INSERT ON Member
FOR EACH ROW
BEGIN
    DECLARE handle_count INT;
    SET handle_count = (SELECT COUNT(*) FROM Member WHERE Handle = NEW.Handle AND Email = NEW.Email);
    
    IF handle_count > 0 THEN
        SIGNAL SQLSTATE '45001'
        SET MESSAGE_TEXT = 'User already exists in this group.';
    END IF;
END; //

DELIMITER ;
