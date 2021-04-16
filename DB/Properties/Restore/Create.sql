USE [Properties]
GO

/****** Object:  Table [dbo].[Property]    Script Date: 2021-04-16 8:53:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Owner] (
    [IdOwner]  UNIQUEIDENTIFIER NOT NULL,
    [Name]     VARCHAR (100)    NOT NULL,
    [Address]   VARCHAR(100)  NOT NULL,
    [Photo]    IMAGE            NULL,
    [Birthday] DATE             NOT NULL,
    CONSTRAINT [PK_Owner] PRIMARY KEY CLUSTERED ([IdOwner] ASC)
);
GO

CREATE TABLE [dbo].[Property] (
    [IdProperty]   UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (100)   NOT NULL,
    [Address]      NVARCHAR (100)   NOT NULL,
    [Price]        DECIMAL (12, 2)  NOT NULL,
    [CodeInternal] INT              NOT NULL,
    [Year]         SMALLINT         NOT NULL,
    [IdOwner]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Property] PRIMARY KEY CLUSTERED ([IdProperty] ASC),
    CONSTRAINT [FK_Property_Owner] FOREIGN KEY ([IdOwner]) REFERENCES [dbo].[Owner] ([IdOwner])
);
GO

CREATE TABLE [dbo].[PropertyImage] (
    [IdPropertyImage] UNIQUEIDENTIFIER NOT NULL,
    [IdProperty]      UNIQUEIDENTIFIER NOT NULL,
    [File]            IMAGE            NOT NULL,
    [Enabled]         BIT              NOT NULL,
    CONSTRAINT [PK_PropertyImage] PRIMARY KEY CLUSTERED ([IdPropertyImage] ASC),
    CONSTRAINT [FK_PropertyImage_Property] FOREIGN KEY ([IdProperty]) REFERENCES [dbo].[Property] ([IdProperty])
);
GO

CREATE TABLE [dbo].[PropertyTrace] (
    [IdPropertyTrace] UNIQUEIDENTIFIER NOT NULL,
    [DateSale]        DATETIME         NOT NULL,
    [Name]            VARCHAR (100)    NOT NULL,
    [Value]           DECIMAL (12, 2)  NOT NULL,
    [Tax]             DECIMAL (12, 2)  NOT NULL,
    [IdProperty]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_PropertyTrace] PRIMARY KEY CLUSTERED ([IdPropertyTrace] ASC),
    CONSTRAINT [FK_PropertyTrace_Property] FOREIGN KEY ([IdProperty]) REFERENCES [dbo].[Property] ([IdProperty])
);
