-- Lokaal script: maakt editor@test.nl Editor en admin@test.nl Admin.
-- Eerst beide gebruikers registreren via POST /api/auth/register.
-- Er staan hier GEEN wachtwoorden in.

USE PortfolioDb;

-- 1. Id's van de gebruikers opzoeken
DECLARE @EditorUserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE NormalizedEmail = 'EDITOR@TEST.NL');
DECLARE @AdminUserId  NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE NormalizedEmail = 'ADMIN@TEST.NL');

-- 2. Id's van de rollen opzoeken
DECLARE @EditorRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'EDITOR');
DECLARE @AdminRoleId  NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN');

-- 3. Oude rol (Gast) van deze twee gebruikers weghalen
DELETE FROM AspNetUserRoles WHERE UserId IN (@EditorUserId, @AdminUserId);

-- 4. Nieuwe rol toekennen
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@EditorUserId, @EditorRoleId);
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@AdminUserId,  @AdminRoleId);

-- 5. Controleren: wie heeft welke rol?
SELECT u.Email, r.Name AS Rol
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r      ON r.Id = ur.RoleId;