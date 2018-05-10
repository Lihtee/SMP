
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/18/2018 14:39:27
-- Generated from EDMX file: D:\HSE\3Course\SMP\SMP\Models\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SMP];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_PersonTeam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Team] DROP CONSTRAINT [FK_PersonTeam];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectTeam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Team] DROP CONSTRAINT [FK_ProjectTeam];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectProject]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_ProjectProject];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectAddiction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Addiction] DROP CONSTRAINT [FK_ProjectAddiction];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectAddiction1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Addiction] DROP CONSTRAINT [FK_ProjectAddiction1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Project]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Project];
GO
IF OBJECT_ID(N'[dbo].[Person]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Person];
GO
IF OBJECT_ID(N'[dbo].[Team]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Team];
GO
IF OBJECT_ID(N'[dbo].[Addiction]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Addiction];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Project'
CREATE TABLE [dbo].[Project] (
    [IdProject] int IDENTITY(1,1) NOT NULL,
    [projectName] nvarchar(max)  NOT NULL,
    [startDateTime] datetime  NOT NULL,
    [endDateTime] datetime  NOT NULL,
    [description] nvarchar(max)  NULL,
    [isDone] bit  NOT NULL,
    [isClose] bit  NOT NULL,
    [plannedBudget] decimal(18,0)  NULL,
    [realBudget] decimal(18,0)  NULL,
    [parrentProject_IdProject] int  NULL
);
GO

-- Creating table 'Person'
CREATE TABLE [dbo].[Person] (
    [IdPerson] int IDENTITY(1,1) NOT NULL,
    [firstName] nvarchar(max)  NOT NULL,
    [surName] nvarchar(max)  NULL,
    [middleName] nvarchar(max)  NULL,
    [Position] int  NOT NULL,
    [login] nvarchar(max)  NOT NULL,
    [password] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Team'
CREATE TABLE [dbo].[Team] (
    [IdTeam] int IDENTITY(1,1) NOT NULL,
    [Person_IdPerson] int  NOT NULL,
    [Project_IdProject] int  NOT NULL
);
GO

-- Creating table 'Addiction'
CREATE TABLE [dbo].[Addiction] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [lastProject_IdProject] int  NOT NULL,
    [nextProject_IdProject] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [IdProject] in table 'Project'
ALTER TABLE [dbo].[Project]
ADD CONSTRAINT [PK_Project]
    PRIMARY KEY CLUSTERED ([IdProject] ASC);
GO

-- Creating primary key on [IdPerson] in table 'Person'
ALTER TABLE [dbo].[Person]
ADD CONSTRAINT [PK_Person]
    PRIMARY KEY CLUSTERED ([IdPerson] ASC);
GO

-- Creating primary key on [IdTeam] in table 'Team'
ALTER TABLE [dbo].[Team]
ADD CONSTRAINT [PK_Team]
    PRIMARY KEY CLUSTERED ([IdTeam] ASC);
GO

-- Creating primary key on [Id] in table 'Addiction'
ALTER TABLE [dbo].[Addiction]
ADD CONSTRAINT [PK_Addiction]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Person_IdPerson] in table 'Team'
ALTER TABLE [dbo].[Team]
ADD CONSTRAINT [FK_PersonTeam]
    FOREIGN KEY ([Person_IdPerson])
    REFERENCES [dbo].[Person]
        ([IdPerson])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PersonTeam'
CREATE INDEX [IX_FK_PersonTeam]
ON [dbo].[Team]
    ([Person_IdPerson]);
GO

-- Creating foreign key on [Project_IdProject] in table 'Team'
ALTER TABLE [dbo].[Team]
ADD CONSTRAINT [FK_ProjectTeam]
    FOREIGN KEY ([Project_IdProject])
    REFERENCES [dbo].[Project]
        ([IdProject])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectTeam'
CREATE INDEX [IX_FK_ProjectTeam]
ON [dbo].[Team]
    ([Project_IdProject]);
GO

-- Creating foreign key on [parrentProject_IdProject] in table 'Project'
ALTER TABLE [dbo].[Project]
ADD CONSTRAINT [FK_ProjectProject]
    FOREIGN KEY ([parrentProject_IdProject])
    REFERENCES [dbo].[Project]
        ([IdProject])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectProject'
CREATE INDEX [IX_FK_ProjectProject]
ON [dbo].[Project]
    ([parrentProject_IdProject]);
GO

-- Creating foreign key on [lastProject_IdProject] in table 'Addiction'
ALTER TABLE [dbo].[Addiction]
ADD CONSTRAINT [FK_ProjectAddiction]
    FOREIGN KEY ([lastProject_IdProject])
    REFERENCES [dbo].[Project]
        ([IdProject])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectAddiction'
CREATE INDEX [IX_FK_ProjectAddiction]
ON [dbo].[Addiction]
    ([lastProject_IdProject]);
GO

-- Creating foreign key on [nextProject_IdProject] in table 'Addiction'
ALTER TABLE [dbo].[Addiction]
ADD CONSTRAINT [FK_ProjectAddiction1]
    FOREIGN KEY ([nextProject_IdProject])
    REFERENCES [dbo].[Project]
        ([IdProject])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectAddiction1'
CREATE INDEX [IX_FK_ProjectAddiction1]
ON [dbo].[Addiction]
    ([nextProject_IdProject]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------