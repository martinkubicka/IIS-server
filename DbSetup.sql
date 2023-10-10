DROP TABLE Rating;
DROP TABLE Post;
DROP TABLE Thread;
DROP TABLE Member;
DROP TABLE `Groups`;
DROP TABLE Tokens;
DROP TABLE Users; 
DROP PROCEDURE DeleteUser;
DROP PROCEDURE DeleteMember;
DROP PROCEDURE CalculateRating;

-- TABLES

CREATE TABLE Users (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Handle VARCHAR(255) NOT NULL UNIQUE,
    Name VARCHAR(255) NOT NULL,
    Role INT NOT NULL,
    VisibilityRegistered BOOLEAN DEFAULT TRUE,
    VisibilityGuest BOOLEAN DEFAULT TRUE,
    VisibilityGroup BOOLEAN DEFAULT TRUE,
    Icon VARCHAR(255),
    UNIQUE KEY (Icon),
    UNIQUE KEY (Name)
);

CREATE TABLE `Groups` (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL UNIQUE,
    Description VARCHAR(255),
    Name VARCHAR(255) NOT NULL,
    VisibilityMember BOOLEAN DEFAULT TRUE,
    VisibilityGuest BOOLEAN DEFAULT TRUE,
    Icon VARCHAR(255)
);

CREATE TABLE Member (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    GroupRole INT NOT NULL,
    Icon VARCHAR(255),
    FOREIGN KEY (Email) REFERENCES Users(Email),
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle) ON DELETE CASCADE,
    FOREIGN KEY (Icon) REFERENCES Users(Icon),
    FOREIGN KEY (Name) REFERENCES Users(Name)
);

CREATE TABLE Thread (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Description VARCHAR(255),
    Handle VARCHAR(255) NOT NULL,
    Email VARCHAR(255),
    Name VARCHAR(255) NOT NULL,
    Date DATETIME NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email) ON DELETE SET NULL,
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle) ON DELETE CASCADE
);

CREATE TABLE Post (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    ThreadId VARCHAR(255) NOT NULL,
    Email VARCHAR(255),
    Text VARCHAR(255) NOT NULL,
    Date DATETIME NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email) ON DELETE SET NULL,
    FOREIGN KEY (ThreadId) REFERENCES Thread(Id) ON DELETE CASCADE
);

CREATE TABLE Rating (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Rating BOOLEAN,  
    PostId VARCHAR(255) NOT NULL,
    Email VARCHAR(255),
    FOREIGN KEY (Email) REFERENCES Users(Email) ON DELETE SET NULL,
    FOREIGN KEY (PostId) REFERENCES Post(Id) ON DELETE CASCADE
);

CREATE TABLE Tokens (
    Token VARCHAR(255) NOT NULL PRIMARY KEY,
    Email VARCHAR(255),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (Email) REFERENCES Users(Email) ON DELETE CASCADE
);

-- PROCEDURES

DELIMITER //

CREATE PROCEDURE DeleteUser(IN userEmail VARCHAR(255))
BEGIN
    DECLARE isGroupAdmin INT DEFAULT 0;
    DECLARE isSystemAdmin INT DEFAULT 0;
    
    SELECT COUNT(*) INTO isGroupAdmin FROM Member WHERE Email = userEmail AND GroupRole = 0;
    SELECT COUNT(*) INTO isSystemAdmin FROM Users WHERE Role = 0 AND Email = userEmail;

    IF isGroupAdmin > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'User is an group admin and cannot be deleted';
    ELSEIF isSystemAdmin > 0 THEN
        SIGNAL SQLSTATE '45001'
        SET MESSAGE_TEXT = 'User is an system admin and cannot be deleted';
    ELSE
        DELETE FROM Member WHERE Email = userEmail;

        DELETE FROM Users WHERE Email = userEmail;
    END IF;
END //

DELIMITER ;

DELIMITER //
CREATE PROCEDURE CalculateRating(
    IN postId VARCHAR(255),
    OUT ratingCount INT
)
BEGIN
    SELECT 
        SUM(CASE WHEN Rating = TRUE THEN 1 ELSE -1 END) INTO ratingCount
    FROM Rating
    WHERE PostId = postId;
END //
DELIMITER ;

-- TRIGGERS

DELIMITER //
CREATE TRIGGER DeleteThread
AFTER DELETE ON Thread
FOR EACH ROW
BEGIN
    DELETE FROM Post
    WHERE Post.ThreadId = OLD.Id;
END;
//
DELIMITER ;

DELIMITER //
CREATE TRIGGER DeletePost
AFTER DELETE ON Post
FOR EACH ROW
BEGIN
    DELETE FROM Rating
    WHERE Rating.PostId = OLD.Id;
END;
//
DELIMITER //

CREATE PROCEDURE DeleteMember(IN userEmail VARCHAR(255), IN groupHandle VARCHAR(255))
BEGIN
    DECLARE isGroupAdmin INT DEFAULT 0;
    
    SELECT COUNT(*) INTO isGroupAdmin FROM Member WHERE Email = userEmail AND Handle = groupHandle AND GroupRole = 0;

    IF isGroupAdmin > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'User is an group admin and cannot be deleted';
    ELSE
        DELETE FROM Member WHERE Email = userEmail AND Handle = groupHandle;
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
        SIGNAL SQLSTATE '45002'
        SET MESSAGE_TEXT = 'User already exists in this group.';
    END IF;
END; //

DELIMITER ;

-- INSERTS
-- Insert additional sample user data to make at least six users
INSERT INTO Users (Id, Email, Password, Handle, Name, Role, VisibilityRegistered, VisibilityGuest, VisibilityGroup, Icon)
VALUES
    ('85e619ff-1593-4fae-a4e1-8b5268c4c9a1', 'user1@example.com', '$2a$10$YWX0i1FngYLmVmpoBMzZZujAP0dnRDmGRjtcfNa11ryxEezzQepBa', 'user1_handle', 'User 1', 1, TRUE, TRUE, TRUE, 'user1-icon'),
    ('b9d3a8e4-3a62-4b21-a5c8-5ebc0b3bf929', 'user2@example.com', '$2a$10$YWX0i1FngYLmVmpoBMzZZujAP0dnRDmGRjtcfNa11ryxEezzQepBa', 'user2_handle', 'User 2', 1, TRUE, TRUE, TRUE, 'user2-icon'),
    ('e7ac07c1-7dcd-4b36-b2a3-1f676f10a0ab', 'user3@example.com', '$2a$10$YWX0i1FngYLmVmpoBMzZZujAP0dnRDmGRjtcfNa11ryxEezzQepBa', 'user3_handle', 'User 3', 1, TRUE, TRUE, TRUE, 'user3-icon'),
    ('f5e619ff-1593-4fae-a4e1-8b5268c4c9a1', 'user4@example.com', '$2a$10$YWX0i1FngYLmVmpoBMzZZujAP0dnRDmGRjtcfNa11ryxEezzQepBa', 'user4_handle', 'User 4', 1, TRUE, TRUE, TRUE, 'user4-icon'),
    ('b9d3a8e4-3a62-4b21-a5c8-5ebc0b3bf939', 'user5@example.com', '$2a$10$YWX0i1FngYLmVmpoBMzZZujAP0dnRDmGRjtcfNa11ryxEezzQepBa', 'user5_handle', 'User 5', 1, TRUE, TRUE, TRUE, 'user5-icon'),
    ('e7ac07c1-7dcd-4b36-b2a3-1f676f11a0ab', 'user6@example.com', '$2a$10$YWX0i1FngYLmVmpoBMzZZujAP0dnRDmGRjtcfNa11ryxEezzQepBa', 'user6_handle', 'User 6', 1, TRUE, TRUE, TRUE, 'user6-icon');

-- Insert sample group data
INSERT INTO `Groups` (Id, Handle, Description, Name, VisibilityMember, VisibilityGuest, Icon)
VALUES
    ('50ca0201-3a6b-4d5b-9e6c-6899a0f0aa7f', 'group1', 'Description for Group 1', 'Group 1', TRUE, TRUE, 'group1-icon'),
    ('da28ee79-bc45-4380-a0dd-7c8f0bfc8b6f', 'group2', 'Description for Group 2', 'Group 2', TRUE, TRUE, 'group2-icon');

-- Insert additional sample member data to make at least six members
INSERT INTO Member (Id, Handle, Email, GroupRole, Icon, Name)
VALUES
    ('ab9cfbc31-1c33-446d-bcae-8c045108b704', 'group1', 'user4@example.com', 1, 'user4-icon', 'User 4'),
    ('cf9d8f37c-69e5-4e64-a3f0-b006b1a88cb4', 'group1', 'user5@example.com', 2, 'user5-icon', 'User 5'),
    ('8f5de2a3-7a0b-4f29-9842-5c2727e27483', 'group1', 'user6@example.com', 1, 'user6-icon', 'User 6'),
    ('ab9cfbc31-1c33-446d-bcae-8c055108b704', 'group2', 'user1@example.com', 1, 'user1-icon', 'User 1'),
    ('cf9d8f37c-69e5-4e64-a3f0-b007b1a88cb4', 'group2', 'user2@example.com', 2, 'user2-icon', 'User 2'),
    ('8f5de2a3-7a0b-4f29-9842-5c2727e27473', 'group2', 'user3@example.com', 1, 'user3-icon', 'User 3'),
    ('de2a2355c-9da3-45a0-85e7-8c4e24a9d8f6', 'group2', 'user4@example.com', 1, 'user4-icon', 'User 4'),
    ('c8196b4db-5f06-4f3e-9e63-2a752d3b1ef5', 'group2', 'user5@example.com', 2, 'user5-icon', 'User 5'),
    ('0e12a7cf-25ea-468c-af4e-9f0b4f18f4de', 'group2', 'user6@example.com', 2, 'user6-icon', 'User 6');


-- Insert sample thread data
INSERT INTO Thread (Id, Description, Handle, Email, Name, Date)
VALUES
    ('ecdf0794-667e-4de9-86a5-71f7e98d9b3a', 'Description for Thread 1', 'group1', 'user1@example.com', 'Thread 1', NOW()),
    ('73a1f07c-ee3d-4cc5-a3f5-17ee1d52e554', 'Description for Thread 2', 'group1', 'user2@example.com', 'Thread 2', NOW()),
    ('bca3ad1f-d8c2-4c9d-b07f-ffbcfaec072e', 'Description for Thread 3', 'group2', 'user3@example.com', 'Thread 3', NOW());

-- Insert sample post data
INSERT INTO Post (Id, ThreadId, Email, Text, Date)
VALUES
    ('e43159aa-72d2-42f1-874b-4289e94036de', 'ecdf0794-667e-4de9-86a5-71f7e98d9b3a', 'user1@example.com', 'Post 1 in Thread 1', NOW()),
    ('bcdfacab-c915-4a1b-b3a4-9ebe1fc77552', 'ecdf0794-667e-4de9-86a5-71f7e98d9b3a', 'user2@example.com', 'Post 2 in Thread 1', NOW()),
    ('72b6884d-5f0f-4647-a742-2a0b05b14e02', '73a1f07c-ee3d-4cc5-a3f5-17ee1d52e554', 'user2@example.com', 'Post 1 in Thread 2', NOW()),
    ('f905fd6e-0057-428d-884b-59ff419a222e', '73a1f07c-ee3d-4cc5-a3f5-17ee1d52e554', 'user3@example.com', 'Post 2 in Thread 2', NOW());

-- Insert sample rating data
INSERT INTO Rating (Id, Rating, PostId, Email)
VALUES
    ('438e20e9-93e2-4a8f-b9a0-2b73d3727f79', TRUE, 'e43159aa-72d2-42f1-874b-4289e94036de', 'user2@example.com'),
    ('4b98861a-2a6e-4c7b-bf9f-32ad85a9bea9', TRUE, 'bcdfacab-c915-4a1b-b3a4-9ebe1fc77552', 'user3@example.com'),
    ('8d7f8081-87a9-490c-8b96-5d4c25b38d42', FALSE, 'e43159aa-72d2-42f1-874b-4289e94036de', 'user3@example.com');
