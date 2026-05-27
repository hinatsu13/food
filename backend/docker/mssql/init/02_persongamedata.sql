USE [HRIS];
GO

-- PersonGameData keyed by PersonID (PersonDetail's existing PK). The API
-- accepts PersonCode from the client and translates to PersonID server-side.
CREATE TABLE dbo.PersonGameData (
    PersonID            numeric(18, 0) NOT NULL,
    FishSelectionScore  int            NOT NULL CONSTRAINT DF_PersonGameData_Selection  DEFAULT (0),
    FishPrepScore       int            NOT NULL CONSTRAINT DF_PersonGameData_Prep       DEFAULT (0),
    FishCheckTempScore  int            NOT NULL CONSTRAINT DF_PersonGameData_CheckTemp  DEFAULT (0),
    FishPackagingScore  int            NOT NULL CONSTRAINT DF_PersonGameData_Packaging  DEFAULT (0),
    StageCount          int            NOT NULL CONSTRAINT DF_PersonGameData_StageCount DEFAULT (0),
    TotalScore          AS (FishSelectionScore + FishPrepScore + FishCheckTempScore + FishPackagingScore) PERSISTED,
    LastUpdated         datetime       NOT NULL CONSTRAINT DF_PersonGameData_LastUpdated DEFAULT (GETDATE()),
    CONSTRAINT PK_PersonGameData PRIMARY KEY CLUSTERED (PersonID),
    CONSTRAINT FK_PersonGameData_PersonDetail
        FOREIGN KEY (PersonID) REFERENCES dbo.PersonDetail(PersonID)
);
GO
