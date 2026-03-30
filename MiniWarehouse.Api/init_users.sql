-- Lagerarbeiter 1
INSERT INTO User (Email, PasswordHash, Name, PhoneNumber, Role)
SELECT 'lagerarbeiter_1@MiniWarehouse.com', 'StartPasswort123!', 'Hans Müller', '', 0 
WHERE NOT EXISTS (SELECT 1 FROM User WHERE Email = 'lagerarbeiter_1@MiniWarehouse.com');

-- Staplerfahrer 2
INSERT INTO User (Email, PasswordHash, Name, PhoneNumber, Role)
SELECT 'staplerfahrer_2@MiniWarehouse.com', 'StartPasswort123!', 'Staplerfahrer Klaus', '', 0 
WHERE NOT EXISTS (SELECT 1 FROM User WHERE Email = 'staplerfahrer_2@MiniWarehouse.com');

-- Erweiterung 1: Kommissionierer
INSERT INTO User (Email, PasswordHash, Name, PhoneNumber, Role)
SELECT 'picker_3@MiniWarehouse.com', 'StartPasswort123!', 'Sabine Schmidt', '', 0 
WHERE NOT EXISTS (SELECT 1 FROM User WHERE Email = 'picker_3@MiniWarehouse.com');

-- Erweiterung 2: Schichtleiter
INSERT INTO User (Email, PasswordHash, Name, PhoneNumber, Role)
SELECT 'lead_4@MiniWarehouse.com', 'StartPasswort123!', 'Christian Weber', '', 0 
WHERE NOT EXISTS (SELECT 1 FROM User WHERE Email = 'lead_4@MiniWarehouse.com');

-- Erweiterung 3: Azubi
INSERT INTO User (Email, PasswordHash, Name, PhoneNumber, Role)
SELECT 'azubi_5@MiniWarehouse.com', 'StartPasswort123!', 'Julia Fischer', '', 0 
WHERE NOT EXISTS (SELECT 1 FROM User WHERE Email = 'azubi_5@MiniWarehouse.com');