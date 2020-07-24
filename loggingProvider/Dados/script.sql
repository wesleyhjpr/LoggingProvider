CREATE TABLE [LogEvento] (
    [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
    [Category] VARCHAR(512) NOT NULL,
    [EventId] INT NULL,
    [LogLevel] VARCHAR(32) NOT NULL,
    [Message] VARCHAR(max) NOT NULL,
    [CreatedTime] DATETIME NOT NULL)