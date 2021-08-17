IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210816220751_InitialCreate')
BEGIN
    CREATE TABLE [Sheets] (
        [Id] uniqueidentifier NOT NULL,
        [PostAgeLimitInDays] int NOT NULL DEFAULT 7,
        [AllowOver18] bit NOT NULL,
        [AllowSpoilers] bit NOT NULL,
        [AllowStickied] bit NOT NULL,
        CONSTRAINT [PK_Sheets] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210816220751_InitialCreate')
BEGIN
    CREATE TABLE [Subreddits] (
        [Name] nvarchar(450) NOT NULL,
        [SheetId] uniqueidentifier NOT NULL,
        [MaxPostCount] int NOT NULL,
        [PostOrdering] int NOT NULL,
        CONSTRAINT [PK_Subreddits] PRIMARY KEY ([SheetId], [Name]),
        CONSTRAINT [FK_Subreddits_Sheets_SheetId] FOREIGN KEY ([SheetId]) REFERENCES [Sheets] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210816220751_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210816220751_InitialCreate', N'5.0.8');
END;
GO

COMMIT;
GO

