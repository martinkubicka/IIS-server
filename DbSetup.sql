DROP TABLE Rating;
DROP TABLE Post;
DROP TABLE Thread;
DROP TABLE Member;
DROP TABLE `Groups`;
DROP TABLE Tokens;
DROP TABLE Users;
DROP PROCEDURE DeleteUser;
DROP PROCEDURE DeleteMember;

-- TABLES

CREATE TABLE Users (
    Email VARCHAR(255) NOT NULL PRIMARY KEY,
    Password VARCHAR(255) NOT NULL,
    Handle VARCHAR(255) NOT NULL UNIQUE,
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
    VisibilityMember BOOLEAN DEFAULT TRUE,
    VisibilityGuest BOOLEAN DEFAULT TRUE,
    Icon VARCHAR(255)
);

CREATE TABLE Member (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    GroupRole INT NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email),
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle) ON DELETE CASCADE
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
        SUM(CASE WHEN Rating = TRUE THEN 1 ELSE -1 END) AS RatingCount,
    INTO
        ratingCount
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
-- Insert data into Users table
INSERT INTO Users (Email, Password, Handle, Name, Role, VisibilityRegistered, VisibilityGuest, VisibilityGroup, Icon)
VALUES
    ('user1@example.com', 'password1', 'user1', 'User 1', 1, TRUE, TRUE, TRUE, 'user1_icon.jpg'),
    ('user2@example.com', 'password2', 'user2', 'User 2', 1, TRUE, TRUE, TRUE, 'user2_icon.jpg'),
    ('admin@example.com', 'adminpassword', 'admin', 'Admin', 0, TRUE, TRUE, TRUE, 'admin_icon.jpg'),
    ('user3@example.com', 'password3', 'user3', 'User 3', 1, TRUE, TRUE, TRUE, 'user3_icon.jpg'),
    ('user4@example.com', 'password4', 'user4', 'User 4', 1, TRUE, TRUE, TRUE, 'user4_icon.jpg');

-- Insert data into Groups table
INSERT INTO `Groups` (Handle, Description, Name, VisibilityMember, VisibilityGuest, Icon)
VALUES
    ('group1', 'Group 1 Description', 'Group 1', TRUE, TRUE, 'group1_icon.jpg'),
    ('group2', 'Group 2 Description', 'Group 2', TRUE, TRUE, 'group2_icon.jpg'),
    ('group3', 'Group 3 Description', 'Group 3', TRUE, TRUE, 'group3_icon.jpg'),
    ('group4', 'Group 4 Description', 'Group 4', TRUE, TRUE, 'group4_icon.jpg');

-- Insert data into Member table
INSERT INTO Member (Id, Handle, Email, GroupRole)
VALUES
    ('1f4b5c1f-5d3a-4b19-b7bf-60b92ca8421f', 'group1', 'user1@example.com', 1),
    ('2b3e69a8-e76c-47d1-a5f7-c7f23a9e48c3', 'group2', 'user2@example.com', 1),
    ('3a8c84a6-33d5-4f28-b8f4-90f124a28fc0', 'group1', 'admin@example.com', 0),
    ('4f4b5c1f-5d3a-4b19-b7bf-60b92ca8421f', 'group3', 'user3@example.com', 1),
    ('5b3e69a8-e76c-47d1-a5f7-c7f23a9e48c3', 'group4', 'user4@example.com', 1),
    ('6c8c84a6-33d5-4f28-b8f4-90f124a28fc0', 'group3', 'admin@example.com', 0);

-- Insert data into Thread table
INSERT INTO Thread (Id, Description, Handle, Email, Name, Date)
VALUES
    ('7d9a319a-0c5c-4c60-875b-6f2e80d7ef93', 'thread 1 description', 'group1', 'user1@example.com', 'Thread 1', NOW()),
    ('8f8e5a7b-77e0-47d7-b22a-1f29a3c6e065', 'thread 2 description', 'group2', 'user2@example.com', 'Thread 2', NOW()),
    ('8f8e5a7b-77e0-47d7-b22a-1f29a3c6e066', 'thread 3 description', 'group2', 'user2@example.com', 'Thread 2', NOW()),
    ('8f8e5a7b-77e0-47d7-b22a-1f29a3c6e067', 'thread 4 description', 'group2', 'user2@example.com', 'Thread 2', NOW()),
    ('9d9a319a-0c5c-4c60-875b-6f2e80d7ef94', 'thread 5 description', 'group3', 'user3@example.com', 'Thread 3', NOW()),
    ('1e8e5a7b-77e0-47d7-b22a-1f29a3c6e066', 'thread 6 description', 'group4', 'user4@example.com', 'Thread 4', NOW());
    
-- Insert data into Post table
INSERT INTO Post (Id, ThreadId, Email, Text, Date)
VALUES
    ('3eefc84c-165f-4a0d-9abf-78b5c4421d2d', '7d9a319a-0c5c-4c60-875b-6f2e80d7ef93', 'user1@example.com', 'Post 1', NOW()),
    ('6b9f0de1-69ab-4c8f-953c-872607d56204', '8f8e5a7b-77e0-47d7-b22a-1f29a3c6e065', 'user2@example.com', 'Post 2', NOW()),
    ('9eefc84c-165f-4a0d-9abf-78b5c4421d2e', '9d9a319a-0c5c-4c60-875b-6f2e80d7ef94', 'user3@example.com', 'Post 3', NOW()),
    ('8b9f0de1-69ab-4c8f-953c-872607d56205', '1e8e5a7b-77e0-47d7-b22a-1f29a3c6e066', 'user4@example.com', 'Post 4', NOW());

-- Insert data into Rating table
INSERT INTO Rating (Id, Rating, PostId, Email)
VALUES
    ('76c776a6-0d3b-40db-bb94-19f6f6ee961e', TRUE, '3eefc84c-165f-4a0d-9abf-78b5c4421d2d', 'user2@example.com'),
    ('09acfa4b-7c1d-4e67-8a46-b33df2eac846', FALSE, '6b9f0de1-69ab-4c8f-953c-872607d56204', 'user1@example.com'),
    ('86c776a6-0d3b-40db-bb94-19f6f6ee961f', TRUE, '9eefc84c-165f-4a0d-9abf-78b5c4421d2e', 'user4@example.com'),
    ('29acfa4b-7c1d-4e67-8a46-b33df2eac847', FALSE, '8b9f0de1-69ab-4c8f-953c-872607d56205', 'user3@example.com');

