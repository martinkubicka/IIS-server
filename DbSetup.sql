DROP TABLE IF EXISTS Rating;
DROP TABLE IF EXISTS Post;
DROP TABLE IF EXISTS Thread;
DROP TABLE IF EXISTS JoinRequest;
DROP TABLE IF EXISTS ModeratorRequest;
DROP TABLE  IF EXISTS Member;
DROP TABLE  IF EXISTS `Groups`;
DROP TABLE  IF EXISTS Tokens;
DROP TABLE IF EXISTS Users; 
DROP PROCEDURE IF EXISTS DeleteUser;
DROP PROCEDURE IF EXISTS DeleteMember;
DROP PROCEDURE IF EXISTS CalculateRating;

-- TABLES

CREATE TABLE Users (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Handle VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(20) NOT NULL,
    Role INT NOT NULL,
    VisibilityRegistered BOOLEAN DEFAULT TRUE,
    VisibilityGuest BOOLEAN DEFAULT TRUE,
    VisibilityGroup BOOLEAN DEFAULT TRUE,
    Icon VARCHAR(255)
);

CREATE TABLE `Groups` (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL UNIQUE,
    Description VARCHAR(100),
    Name VARCHAR(20) NOT NULL,
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
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle) ON DELETE CASCADE
);

CREATE TABLE Thread (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Description VARCHAR(100),
    Handle VARCHAR(255) NOT NULL,
    Email VARCHAR(255),
    Name VARCHAR(50) NOT NULL,
    Date DATETIME NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email) ON DELETE SET NULL,
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle) ON DELETE CASCADE
);

CREATE TABLE Post (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    ThreadId VARCHAR(255) NOT NULL,
    Handle VARCHAR(255),
    Text VARCHAR(1024) NOT NULL,
    Date DATETIME NOT NULL,
    FOREIGN KEY (Handle) REFERENCES Users(Handle) ON DELETE SET NULL,
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

CREATE TABLE JoinRequest (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email) ON DELETE CASCADE,
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle) ON DELETE CASCADE
);

CREATE TABLE ModeratorRequest (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Handle VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email) ON DELETE CASCADE,
    FOREIGN KEY (Handle) REFERENCES `Groups`(Handle) ON DELETE CASCADE
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
    WHERE Rating.PostId = postId;
END //
DELIMITER ;

-- TRIGGERS

DELIMITER //
CREATE TRIGGER UpdateRoleToModerator
AFTER UPDATE ON Member
FOR EACH ROW
BEGIN
    IF NEW.GroupRole = 2 THEN
        DELETE FROM ModeratorRequest
        WHERE Handle = OLD.Handle AND Email = OLD.Email;
    END IF;
END;
//
DELIMITER ;

DELIMITER //
CREATE TRIGGER JoinMember
AFTER INSERT ON Member
FOR EACH ROW
BEGIN
    DELETE FROM JoinRequest WHERE Handle = NEW.Handle AND Email = NEW.Email;
END;
//
DELIMITER ;

DELIMITER //
CREATE TRIGGER UpdateGroupRole
BEFORE UPDATE ON Member
FOR EACH ROW
BEGIN
    DECLARE adminCount INT;

    SELECT COUNT(*)
    INTO adminCount
    FROM Member
    WHERE Handle = NEW.Handle AND GroupRole = 0;

    IF OLD.GroupRole = 0 AND adminCount = 1 THEN
        SIGNAL SQLSTATE '45002'
        SET MESSAGE_TEXT = 'Group must have at least one admin';
    END IF;
END;
//
DELIMITER ;

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
    DECLARE countGroupAdmins INT DEFAULT 0;
    
    SELECT COUNT(*) INTO isGroupAdmin FROM Member WHERE Email = userEmail AND Handle = groupHandle AND GroupRole = 0;
    SELECT COUNT(*) INTO countGroupAdmins FROM Member WHERE Handle = groupHandle AND GroupRole = 0;

    IF isGroupAdmin > 0 AND countGroupAdmins = 1 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'User is the only group admin and cannot be deleted';
    ELSE
        DELETE FROM ModeratorRequest WHERE Email = userEmail AND Handle = groupHandle;
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

-- Insert additional sample user data
INSERT INTO Users (Id, Email, Password, Handle, Name, Role, VisibilityRegistered, VisibilityGuest, VisibilityGroup, Icon)
VALUES
    ('85e619ff-1593-4fae-a4e1-8b5268c4c9a1', 'john.doe@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'john_doe', 'John Doe', 0, TRUE, TRUE, TRUE, 'smile'),
    ('b9d3a8e4-3a62-4b21-a5c8-5ebc0b3bf929', 'jane.smith@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'jane_smith', 'Jane Smith', 1, TRUE, TRUE, TRUE, 'star'),
    ('e7ac07c1-7dcd-4b36-b2a3-1f676f10a0ab', 'mark.johnson@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'mark_johnson', 'Mark Johnson', 1, TRUE, TRUE, TRUE, 'racoon'),
    ('f5e619ff-1593-4fae-a4e1-8b5268c4c9a1', 'emily.brown@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'emily_brown', 'Emily Brown', 1, TRUE, TRUE, TRUE, 'thinking'),
    ('b9d3a8e4-3a62-4b21-a5c8-5ebc0b3bf939', 'david.wilson@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'david_wilson', 'David Wilson', 1, TRUE, TRUE, TRUE, 'tennis'),
    ('e7ac07c1-7dcd-4b36-b2a3-1f676f11a0ab', 'linda.jones@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'linda_jones', 'Linda Jones', 1, TRUE, TRUE, TRUE, 'jamaica'),
    ('ce14b3a9-6d12-4fc2-98b5-4a1e35b6cdcb', 'sarah.miller@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'sarah_miller', 'Sarah Miller', 1, TRUE, TRUE, TRUE, 'smile'),
    ('a72b5d2c-52bc-4f8e-9a0a-3e1b6c9e5b3a', 'michael.jones@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'michael_jones', 'Michael Jones', 1, TRUE, TRUE, TRUE, 'shark'),
    ('bd7ca932-94ea-4ec8-8ad9-2a6d5f9e3a2b', 'lisa.martin@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'lisa_martin', 'Lisa Martin', 1, TRUE, TRUE, TRUE, 'sunflower'),
    ('8fe125d4-5a6d-42f9-86a5-7c1a9e4eabfc', 'jason.smith@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'jason_smith', 'Jason Smith', 1, TRUE, TRUE, TRUE, 'earth'),
    ('b9d3a8e4-3a62-4b21-a5c8-5ebc0b3bf923', 'olivia.wilson@example.com', '$2a$10$BBCx8vKl5YwlivXpqH/.r.jrE4r2DWE9FoNc9x/KwmLY4zOs5NekC', 'olivia_wilson', 'Olivia Wilson', 1, TRUE, TRUE, TRUE, 'flower');

-- Insert sample group data
INSERT INTO `Groups` (Id, Handle, Description, Name, VisibilityMember, VisibilityGuest, Icon)
VALUES
    ('50ca0201-3a6b-4d5b-9e6c-6899a0f0aa7f', 'cooking-enthusiasts', 'A group for cooking enthusiasts', 'Cooking Enthusiasts', TRUE, TRUE, 'man_cook'),
    ('da28ee79-bc45-4380-a0dd-7c8f0bfc8b6f', 'travel-lovers', 'Explore the world with us', 'Travel Lovers', TRUE, TRUE, 'airplane'),
    ('6f3b4120-5f63-482c-8b0c-8d94fbd8b2f6', 'tech-enthusiasts', 'A group for tech enthusiasts', 'Tech Enthusiasts', TRUE, TRUE, 'computer'),
    ('85c2f0d8-6f3d-48c7-9f5a-7c1e8e1f72f5', 'music-lovers', 'For those who love music', 'Music Lovers', TRUE, TRUE, 'musical_note');



-- Insert additional sample member data
INSERT INTO Member (Id, Handle, Email, GroupRole, Icon, Name)
VALUES
    ('ab9cfbc31-1c33-446d-bcae-8c045108b704', 'cooking-enthusiasts', 'emily.brown@example.com', 0, 'thinking', 'Emily Brown'),
    ('cf9d8f37c-69e5-4e64-a3f0-b006b1a88cb4', 'cooking-enthusiasts', 'david.wilson@example.com', 2, 'tennis', 'David Wilson'),
    ('8f5de2a3-7a0b-4f29-9842-5c2727e27483', 'cooking-enthusiasts', 'linda.jones@example.com', 1, 'jamaica', 'Linda Jones'),
    ('ab9cfbc31-1c33-446d-bcae-8c055108b704', 'travel-lovers', 'john.doe@example.com', 0, 'smile', 'John Doe'),
    ('cf9d8f37c-69e5-4e64-a3f0-b007b1a88cb4', 'travel-lovers', 'jane.smith@example.com', 2, 'star', 'Jane Smith'),
    ('a0b3bc17-1c33-4d6d-bc3e-8c4a5a9a8c04', 'tech-enthusiasts', 'michael.jones@example.com', 0, 'shark', 'Michael Jones'),
    ('c8f6b4c4-6e6e-4e6e-8e6e-1a7e6e7e7e7e', 'tech-enthusiasts', 'lisa.martin@example.com', 2, 'sun', 'Lisa Martin'),
    ('1b3c8e7e7e-7c7e-4c7b-7f7a-1f7e6e7e7e7e', 'music-lovers', 'jason.smith@example.com', 0, 'earth', 'Jason Smith'),
    ('2a6c3e5e5b-9a0e-4a6b-8b9e-3a9d1e7a8b7c', 'music-lovers', 'olivia.wilson@example.com', 2, 'flower', 'Olivia Wilson');

-- Insert sample thread data
INSERT INTO Thread (Id, Description, Handle, Email, Name, Date)
VALUES
    ('ecdf0794-667e-4de9-86a5-71f7e98d9b3a', 'Favorite Recipes', 'cooking-enthusiasts', 'emily.brown@example.com', 'Favorite Recipes', NOW()),
    ('73a1f07c-ee3d-4cc5-a3f5-17ee1d52e554', 'Europe Adventure', 'travel-lovers', 'john.doe@example.com', 'Europe Adventure', NOW()),
    ('4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'Baking Tips', 'cooking-enthusiasts', 'david.wilson@example.com', 'Baking Tips', NOW()),
    ('e63bc05c-9da3-45a0-85e7-8c4e32a9d8f1', 'Mediterranean Cuisine', 'cooking-enthusiasts', 'linda.jones@example.com', 'Mediterranean Cuisine', NOW()),
    ('1fcb8b12-25ea-468c-af4e-9f0c4f28f4df', 'Solo Travel Stories', 'travel-lovers', 'john.doe@example.com', 'Solo Travel Stories', NOW()),
    ('8e2b1e3e-3d7e-4c7b-8b7e-1f7e6e5e5b2e', 'Programming Languages', 'tech-enthusiasts', 'michael.jones@example.com', 'Programming Languages', NOW()),
    ('9d4f8f7e-4e6e-4a1b-9f3a-1f6e7e5e5b4e', 'Favorite Bands', 'music-lovers', 'jason.smith@example.com', 'Favorite Bands', NOW());

-- Insert sample post data
INSERT INTO Post (Id, ThreadId, Handle, Text, Date)
VALUES
    ('e43159aa-72d2-42f1-874b-4289e94036de', 'ecdf0794-667e-4de9-86a5-71f7e98d9b3a', 'emily_brown', 'I love making pasta!', NOW()),
    ('bcdfacab-c915-4a1b-b3a4-9ebe1fc77552', 'ecdf0794-667e-4de9-86a5-71f7e98d9b3a', 'david_wilson', 'Any tips for a perfect pizza?', NOW()),
    ('72b6884d-5f0f-4647-a742-2a0b05b14e02', '73a1f07c-ee3d-4cc5-a3f5-17ee1d52e554', 'john_doe', 'Exploring Italy next week!', NOW()),
    ('7cfb25e2-4d8e-4b8f-9a0b-3a7d4e7e7e1b', 'e63bc05c-9da3-45a0-85e7-8c4e32a9d8f1', 'emily_brown', 'I made a delicious Greek salad today.', NOW()),
    ('0b9e59e2-95e9-4850-8a4a-2c7e5b0b5b3e', 'e63bc05c-9da3-45a0-85e7-8c4e32a9d8f1', 'linda_jones', 'Share the recipe, please!', NOW()),
    ('c8e72a9d-2ea1-4ca1-bb3a-1f6ec3e5a2e9', '1fcb8b12-25ea-468c-af4e-9f0c4f28f4df', 'john_doe', 'My adventures in Paris were amazing!', NOW()),
    ('558be2c3-8f35-49fe-a8c9-23496d54b9a1', '8e2b1e3e-3d7e-4c7b-8b7e-1f7e6e5e5b2e', 'michael_jones', 'I love Python!', NOW()),
    ('f4bf64dd-9a03-4d2b-ac7c-f9fe600ab9fe', '8e2b1e3e-3d7e-4c7b-8b7e-1f7e6e5e5b2e', 'lisa_martin', 'What about Java?', NOW()),
    ('1b4c8a9b-7c7e-4c7b-7f7a-1f7e6gtgmkuh', '9d4f8f7e-4e6e-4a1b-9f3a-1f6e7e5e5b4e', 'jason_smith', 'The Beatles are classic!', NOW()),
    ('4a104b0a-e0d4-4ff6-9341-dc6858fe5a1c', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'I just made the best chocolate chip cookies!', '2023-10-11 08:30:00'),
    ('c96bf4b7-4c6d-466c-90b6-9f9358791176', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'michael_jones', 'Share the recipe, please!', '2023-10-11 08:35:00'),
    ('6097bd7d-d8fb-4cd7-945f-87359a856fdc', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Sure! Here it is:', '2023-10-11 08:40:00'),
    ('1f3a2aaf-b5ca-4f0e-a2bf-690f3fd36dcc', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Ingredients:\n- 1 cup butter\n- 1 cup granulated sugar\n- 2 cups all-purpose flour', '2023-10-11 08:45:00'),
    ('efbc0717-a93c-4459-8487-eb3e023b461d', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Instructions: Preheat oven to 350°F (175°C).. In a bowl, cream the butter and sugar together. Mix in the flour until a dough forms. Drop spoonfuls of dough onto baking sheets. Bake for 10-12 minutes or until the edges are golden brown.', '2023-10-11 08:50:00'),
    ('c34a1f05-4847-4c8b-9dc0-e71a1069b0f3', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'michael_jones', 'Thanks, Sarah! I cant wait to try this.', '2023-10-11 09:00:00'),
    ('21755d22-9fd4-4b73-a350-641657925f55', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'lisa_martin', 'I made these cookies last weekend. They were a hit!', '2023-10-11 09:10:00'),
    ('3af18d32-3215-4cbd-a019-0e739a4e8971', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'michael_jones', 'Do you have any other baking tips?', '2023-10-11 09:20:00'),
    ('f037a193-c8eb-4fa3-ac93-481c61e8d169', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Certainly! Here are a few more:', '2023-10-11 09:30:00'),
    ('0c464244-c930-4e7b-8b23-74c8886289c3', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', '1. Use room temperature butter for better texture.\n2. Add a pinch of salt to enhance the flavor.\n3. Try adding a mix of milk and dark chocolate chips for variety.', '2023-10-11 09:40:00'),
    ('2f59e893-9a93-4a9b-b9e9-2c79e8e9f3d4', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'linda_jones', 'I tried your cookie recipe, and it was amazing!', '2023-10-11 09:50:00'),
    ('9c8b21d4-0e34-4c7f-8c5e-8a9a1a0b8b13', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Im glad you liked it, Linda!', '2023-10-11 10:00:00'),
    ('5d0f01e7-4b68-4c91-9d36-8c4e0e7b4e5b', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'david_wilson', 'Any tips for a moist chocolate cake?', '2023-10-11 10:10:00'),
    ('1e5e8c6d-4b68-4c8b-9c3e-8c4a5a9a8c1f', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Sure, heres a chocolate cake recipe:', '2023-10-11 10:20:00'),
    ('7c5e7d9c-0e0e-4a8b-8c5e-8a9a1a0b8b6d', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Ingredients:\n- 2 cups all-purpose flour\n- 2 cups granulated sugar\n- 3/4 cup unsweetened cocoa powder\n- 1 1/2 tsp baking powder', '2023-10-11 10:30:00'),
    ('4db86920-34a2-4417-b484-792110de3dbd', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', '1/2 tsp baking soda\n- 1/2 tsp salt\n- 2 eggs\n- 1 cup milk\n- 1/2 cup vegetable oil', '2023-10-11 10:40:00'),
    ('6f6d0d9a-4c8a-4c6f-8c4a-8a9b1a0c8a4b', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', '2 tsp vanilla extract\n- 1 cup boiling water\n\nInstructions:', '2023-10-11 10:50:00'),
    ('f9c3a6b1-4e89-4b8c-8c5e-8a9a1a0b8c8e', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Preheat your oven to 350°F (175°C). Grease and flour two 9-inch round cake pans. In a large bowl, combine the flour, sugar, cocoa, baking powder, baking soda, and salt.', '2023-10-11 11:00:00'),
    ('e7d5a5e8-4b48-4c8e-8b5e-8c4b0e7a4e8c', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Add the eggs, milk, oil, and vanilla and mix until well combined. Stir in the boiling water. Pour the batter into the prepared pans.', '2023-10-11 11:10:00'),
    ('4c4a4a8a-4e48-4b4c-8a5e-8c4b8e9a4c6a', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'michael_jones', 'Thanks for the recipe, Sarah! I cant wait to try this chocolate cake.', '2023-10-11 11:20:00'),
    ('2a8d4a1a-0c3a-4c9f-9d3e-8a9b1a4c8e9a', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'lisa_martin', 'Im making the cookies and cake this weekend. Thanks for sharing!', '2023-10-11 11:30:00'),
    ('6c9e9e8b-0e9f-4b9f-8e9b-2c79e8e9f3d4', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Youre welcome, Lisa! Enjoy your baking adventures!', '2023-10-11 11:40:00'),
    ('9e0f0d9e-9a9f-4a9b-b9e9-8c9b1a0c8b9f', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'michael_jones', 'Sarah, do you have any tips for making gluten-free treats?', '2023-10-11 11:50:00'),
    ('3e0f3e5a-4b6a-4c8b-8c5e-8a9a1a0b8c8e', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Certainly, Michael! Here are some gluten-free baking tips:', '2023-10-11 12:00:00'),
    ('7c8e5a4c-4e8e-4a4b-b9e9-8c5e9a8c5e4e', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', '1. Use gluten-free flour blends in your recipes.\n2. Check the labels of ingredients to ensure they are gluten-free.\n3. Use separate utensils and equipment for gluten-free baking.', '2023-10-11 12:10:00'),
    ('4a9e0c8f-4b68-4c8b-8c5e-8a9a1a0b8c8b', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'linda_jones', 'Thanks, Sarah! Im always looking for gluten-free options.', '2023-10-11 12:20:00'),
    ('1a4e9c9f-4b8e-4f9a-8c5e-8a9a1a0b8f5a', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Youre welcome, Linda! Enjoy your gluten-free baking!', '2023-10-11 12:30:00'),
    ('5a8c3e9a-9a9b-4a9e-8c5e-8a9b1a4e8a3e', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'david_wilson', 'I made a cheesecake, and it cracked. Any tips for a smooth top?', '2023-10-11 12:40:00'),
    ('1a8e9f9c-4b8e-4f9b-8c5e-8a9a1a0c9b8e', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'To prevent cracks, David, try these tips:', '2023-10-11 12:50:00'),
    ('8c5e8e9c-4b8e-4f8e-8c9e-8a9a1a0b9b8e', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', '1. Dont overmix the cheesecake batter.\n2. Use a water bath when baking.\n3. Cool the cheesecake gradually by turning off the oven and cracking the door.\n4. Chill the cheesecake for several hours or overnight before serving.', '2023-10-11 13:00:00'),
    ('9a9e8b8c-4e8c-4c8b-8e9b-2c79e8e9f3d4', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'david_wilson', 'Thanks, Sarah! Ill give those a try next time.', '2023-10-11 13:10:00'),
    ('9a4e8c9b-4b8c-4c8b-8c9b-8a9b1a0b8c9b', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Youre welcome, David! I hope it helps.', '2023-10-11 13:20:00'),
    ('37dc003c-6417-4c63-b8e1-4e8365b9ce89', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'For those who enjoy a classic pie, try this apple pie recipe:', '2023-10-11 13:30:00'),
    ('1050a790-2cbf-42e9-9ecc-9ad902c382d4', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Ingredients:\n- 6-7 cups peeled and sliced apples\n- 3/4 cup granulated sugar\n- 1 tsp ground cinnamon', '2023-10-11 13:40:00'),
    ('27dabd0d-bd3e-41dd-8208-0bfa351e7422', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', '1/4 tsp salt\n- 2 tbsp all-purpose flour\n- 2 9-inch pie crusts', '2023-10-11 13:50:00'),
    ('988ea1a8-e666-401b-a358-775903d4bc4a', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Instructions: Preheat your oven to 425°F (220°C). In a large bowl, mix the apples, sugar, cinnamon, salt, and flour. Pour the mixture into one of the pie crusts.', '2023-10-11 14:00:00'),
    ('28865b13-06e6-47de-8c87-13bc05c8d684', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Cover the filling with the second pie crust. Trim and crimp the edges. Make a few small slits in the top crust to allow steam to escape.', '2023-10-11 14:10:00'),
    ('9e04e946-e15c-4662-aefd-25c0f85ce644', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'linda_jones', 'Im going to make the apple pie for Thanksgiving. Thanks, Sarah!', '2023-10-11 14:20:00'),
    ('d036bf32-77ef-4184-a68a-d9bcd3d1fcca', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Youre welcome, Linda! Itll be a great addition to your Thanksgiving table.', '2023-10-11 14:30:00'),
    ('08204a5a-b441-4ed2-8b18-89bb13507371', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'If you love muffins, try these blueberry muffins:', '2023-10-11 14:40:00'),
    ('97d3072c-eeb9-4d82-a7f9-7235718078e4', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Ingredients:\n- 1 1/2 cups all-purpose flour\n- 3/4 cup granulated sugar\n- 1/2 tsp salt', '2023-10-11 14:50:00'),
    ('7cea0eb5-776f-4b7e-b6dd-2b33519eacc5', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', '2 tsp baking powder\n- 1/3 cup vegetable oil\n- 1 egg\n- 1/3 cup milk\n- 1 cup fresh or frozen blueberries', '2023-10-11 15:00:00'),
    ('88a06a0a-afdc-4828-9231-76a1998f4d7d', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Instructions: Preheat your oven to 400°F (200°C). In a bowl, combine the flour, sugar, salt, and baking powder. Add the oil, egg, and milk. Fold in the blueberries.', '2023-10-11 15:10:00'),
    ('cca60704-fe14-42a2-b34f-aa8db7e48829', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Spoon the batter into muffin cups and bake for 20-25 minutes or until a toothpick comes out clean.', '2023-10-11 15:20:00'),
    ('5f8f7b5f-fbab-4398-bc3f-4fa76f6f6f49', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'michael_jones', 'I love blueberry muffins! Thanks for sharing, Sarah.', '2023-10-11 15:30:00'),
    ('1688020b-4337-4d27-9d7b-365a161728d2', '4a1ed285-7a0b-4f29-9842-5c27cf3e2793', 'sarah_miller', 'Youre welcome, Michael! Enjoy your muffins.', '2023-10-11 15:40:00');
    
   


    
-- Insert sample rating data
INSERT INTO Rating (Id, Rating, PostId, Email)
VALUES
    ('438e20e9-93e2-4a8f-b9a0-2b73d3727f79', TRUE, 'efbc0717-a93c-4459-8487-eb3e023b461d', 'david.wilson@example.com'),
    ('4b98861a-2a6e-4c7b-bf9f-32ad85a9bea9', TRUE, 'efbc0717-a93c-4459-8487-eb3e023b461d', 'jane.smith@example.com'),
    ('2b3e4e9a-9f3e-4b8b-8a9e-2b7b4b7b9f7f', TRUE, 'efbc0717-a93c-4459-8487-eb3e023b461d', 'lisa.martin@example.com'),
    ('4b7f8f7e-2a6e-4c7b-9f3a-3f4e9a9b7e9a', TRUE, 'efbc0717-a93c-4459-8487-eb3e023b461d', 'jason.smith@example.com');

