-- PhotoShop Database Script
-- Pokrenite na SQL Server Management Studio (bez CREATE DATABASE / USE naredbi)

CREATE TABLE Photographer (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Bio NVARCHAR(500) NULL,
    Email NVARCHAR(200) NULL
);

CREATE TABLE AppUser (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    FirstName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NULL,
    Phone NVARCHAR(50) NULL
);

CREATE TABLE Photo (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    ImageUrl NVARCHAR(MAX) NOT NULL,
    Price DECIMAL(10,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PhotographerId INT NOT NULL,
    CONSTRAINT FK_Photo_Photographer FOREIGN KEY (PhotographerId) REFERENCES Photographer(Id)
);

CREATE TABLE Tag (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE PhotoTag (
    PhotoId INT NOT NULL,
    TagId INT NOT NULL,
    CONSTRAINT PK_PhotoTag PRIMARY KEY (PhotoId, TagId),
    CONSTRAINT FK_PhotoTag_Photo FOREIGN KEY (PhotoId) REFERENCES Photo(Id),
    CONSTRAINT FK_PhotoTag_Tag FOREIGN KEY (TagId) REFERENCES Tag(Id)
);

CREATE TABLE Download (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PhotoId INT NOT NULL,
    AppUserId INT NOT NULL,
    DownloadedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Download_Photo FOREIGN KEY (PhotoId) REFERENCES Photo(Id),
    CONSTRAINT FK_Download_AppUser FOREIGN KEY (AppUserId) REFERENCES AppUser(Id)
);

CREATE TABLE Log (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Level NVARCHAR(50) NOT NULL DEFAULT 'Info',
    Message NVARCHAR(MAX) NOT NULL
);

-- =====================
-- SEED DATA
-- =====================

INSERT INTO Photographer (Name, Bio, Email) VALUES
('Ante Kovač', 'Profesionalni fotograf specijaliziran za prirodu i krajoliku.', 'ante.kovac@photoshop.hr'),
('Maja Horvat', 'Urbana fotografija i arhitektura, Zagreb.', 'maja.horvat@photoshop.hr'),
('Ivan Perić', 'Putopisna fotografija s 15 godina iskustva.', 'ivan.peric@photoshop.hr');

INSERT INTO Tag (Name) VALUES
('nature'), ('travel'), ('urban'), ('portrait'), ('architecture'), ('adventure'), ('wildlife'), ('landscape');

INSERT INTO Photo (Name, Description, ImageUrl, Price, PhotographerId) VALUES
('Plitvička jezera', 'Prekrasni slapovi Plitvičkih jezera u proljeće.', 'https://upload.wikimedia.org/wikipedia/commons/thumb/5/5b/Plitvice_Lakes_National_Park_%281%29.jpg/1280px-Plitvice_Lakes_National_Park_%281%29.jpg', 15.99, 1),
('Zagreb noću', 'Panorama Zagreba snimljena s Medvednice.', 'https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Zagreb_night_panorama_from_Sljeme.jpg/1280px-Zagreb_night_panorama_from_Sljeme.jpg', 12.50, 2),
('Dubrovnik iz zraka', 'Stari grad Dubrovnik iz zraka.', 'https://upload.wikimedia.org/wikipedia/commons/thumb/9/9e/Dubrovnik_-_Old_town_walls_-_%28panoramio%29.jpg/1280px-Dubrovnik_-_Old_town_walls_-_%28panoramio%29.jpg', 18.00, 3),
('Šuma u jeseni', 'Šarene šume Like u listopadu.', 'https://upload.wikimedia.org/wikipedia/commons/thumb/f/f8/Autumn_Forest_in_Muenster%2C_North-Rhine_Westfalia.jpg/1280px-Autumn_Forest_in_Muenster%2C_North-Rhine_Westfalia.jpg', 9.99, 1),
('Jadransko more', 'Kristalno čisto Jadransko more kod Hvara.', 'https://upload.wikimedia.org/wikipedia/commons/thumb/e/e3/Hvar_harbor_Croatia.jpg/1280px-Hvar_harbor_Croatia.jpg', 14.00, 3);

INSERT INTO PhotoTag (PhotoId, TagId) VALUES
(1,1),(1,2),(1,6),(2,3),(2,5),(3,2),(3,5),(4,1),(4,8),(5,2),(5,8);

-- NAPOMENA: Za kreiranje admin korisnika, registrirajte se kroz aplikaciju
-- pa pokrenite ovu SQL naredbu:
-- UPDATE AppUser SET Role = 'Admin' WHERE Username = 'vase_korisnicko_ime';

UPDATE AppUser SET Role = 'Admin' WHERE Username = 'bronka'

select * from Photo

select * from AppUser