create TABLE [Log] (
    [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
    [Categoria] VARCHAR(512) NOT NULL,
    [IdEvento] INT NULL,
    [LogLevel] VARCHAR(32) NOT NULL,
    [Mensagem] VARCHAR(max) NOT NULL,
    [DataCadastro] DATETIME NOT NULL)