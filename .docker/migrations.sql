CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210903135012_InitialCreate') THEN
    CREATE TABLE "Sheets" (
        "Id" uuid NOT NULL,
        "PostAgeLimitInDays" integer NOT NULL DEFAULT 7,
        "AllowOver18" boolean NOT NULL,
        "AllowSpoilers" boolean NOT NULL,
        "AllowStickied" boolean NOT NULL,
        CONSTRAINT "PK_Sheets" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210903135012_InitialCreate') THEN
    CREATE TABLE "Subreddits" (
        "Name" text NOT NULL,
        "SheetId" uuid NOT NULL,
        "MaxPostCount" integer NOT NULL,
        "PostOrdering" integer NOT NULL,
        CONSTRAINT "PK_Subreddits" PRIMARY KEY ("SheetId", "Name"),
        CONSTRAINT "FK_Subreddits_Sheets_SheetId" FOREIGN KEY ("SheetId") REFERENCES "Sheets" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20210903135012_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20210903135012_InitialCreate', '5.0.8');
    END IF;
END $$;
COMMIT;

