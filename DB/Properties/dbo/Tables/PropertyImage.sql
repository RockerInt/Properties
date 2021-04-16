CREATE TABLE [dbo].[PropertyImage] (
    [IdPropertyImage] UNIQUEIDENTIFIER NOT NULL,
    [IdProperty]      UNIQUEIDENTIFIER NOT NULL,
    [File]            IMAGE            NOT NULL,
    [Enabled]         BIT              NOT NULL,
    CONSTRAINT [PK_PropertyImage] PRIMARY KEY CLUSTERED ([IdPropertyImage] ASC),
    CONSTRAINT [FK_PropertyImage_Property] FOREIGN KEY ([IdProperty]) REFERENCES [dbo].[Property] ([IdProperty])
);

