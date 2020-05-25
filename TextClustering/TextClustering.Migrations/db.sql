IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(256) NOT NULL,
        [LastName] nvarchar(256) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [Password] nvarchar(256) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [Datasets] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NOT NULL,
        [Description] nvarchar(1024) NOT NULL,
        [CreatedById] int NOT NULL,
        CONSTRAINT [PK_Datasets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Datasets_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [DatasetClusterings] (
        [Id] int NOT NULL IDENTITY,
        [Description] nvarchar(1024) NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [CreatedById] int NOT NULL,
        [DatasetId] int NOT NULL,
        CONSTRAINT [PK_DatasetClusterings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DatasetClusterings_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DatasetClusterings_Datasets_DatasetId] FOREIGN KEY ([DatasetId]) REFERENCES [Datasets] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [DatasetTexts] (
        [Id] int NOT NULL IDENTITY,
        [DatasetId] int NOT NULL,
        [Key] nvarchar(256) NOT NULL,
        [Text] nvarchar(max) NULL,
        CONSTRAINT [PK_DatasetTexts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DatasetTexts_Datasets_DatasetId] FOREIGN KEY ([DatasetId]) REFERENCES [Datasets] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [Clusters] (
        [Id] int NOT NULL IDENTITY,
        [DatasetClusteringId] int NOT NULL,
        CONSTRAINT [PK_Clusters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Clusters_DatasetClusterings_DatasetClusteringId] FOREIGN KEY ([DatasetClusteringId]) REFERENCES [DatasetClusterings] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [ClusterDatasetTexts] (
        [DatasetTextId] int NOT NULL,
        [ClusterId] int NOT NULL,
        CONSTRAINT [PK_ClusterDatasetTexts] PRIMARY KEY ([ClusterId], [DatasetTextId]),
        CONSTRAINT [FK_ClusterDatasetTexts_Clusters_ClusterId] FOREIGN KEY ([ClusterId]) REFERENCES [Clusters] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ClusterDatasetTexts_DatasetTexts_DatasetTextId] FOREIGN KEY ([DatasetTextId]) REFERENCES [DatasetTexts] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [Topics] (
        [Id] int NOT NULL IDENTITY,
        [ClusterId] int NOT NULL,
        CONSTRAINT [PK_Topics] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Topics_Clusters_ClusterId] FOREIGN KEY ([ClusterId]) REFERENCES [Clusters] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE TABLE [TopicTokens] (
        [Id] int NOT NULL IDENTITY,
        [TopicId] int NOT NULL,
        [Token] nvarchar(max) NULL,
        CONSTRAINT [PK_TopicTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TopicTokens_Topics_TopicId] FOREIGN KEY ([TopicId]) REFERENCES [Topics] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_ClusterDatasetTexts_DatasetTextId] ON [ClusterDatasetTexts] ([DatasetTextId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_Clusters_DatasetClusteringId] ON [Clusters] ([DatasetClusteringId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_DatasetClusterings_CreatedById] ON [DatasetClusterings] ([CreatedById]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_DatasetClusterings_DatasetId] ON [DatasetClusterings] ([DatasetId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_Datasets_CreatedById] ON [Datasets] ([CreatedById]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_DatasetTexts_DatasetId] ON [DatasetTexts] ([DatasetId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE UNIQUE INDEX [IX_DatasetTexts_Key_DatasetId] ON [DatasetTexts] ([Key], [DatasetId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_Topics_ClusterId] ON [Topics] ([ClusterId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    CREATE INDEX [IX_TopicTokens_TopicId] ON [TopicTokens] ([TopicId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201130122115_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201130122115_Initial', N'3.1.10');
END;

GO

