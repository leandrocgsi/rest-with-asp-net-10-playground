CREATE TABLE [dbo].[books] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [author] VARCHAR(MAX) NULL,
    [launch_date] DATETIME2(6) NOT NULL,
    [price] DECIMAL(18,2) NOT NULL,
    [title] VARCHAR(MAX) NULL
);