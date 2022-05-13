Insert Into AppAccess (AccessName,AccessCode,AccessType,ScreenUrl,Active,CreatedBy,CreatedDate)
values ('Ledger Histroy','SCR_LEDGER_HISTROY','S','finance/ledger-Histroy','Y','5158d424-b0af-4f06-8302-69d767db6c9d','2021-10-14 20:40:00')



INSERT INTO [AppMenuMaster]([ModuleCode],[ModuleName],[ModuleDispOrder],[AppAccessId],[MainMenuName],[SubMenuName],[DispOrder],[MainMenuCode]
           ,[SubMenuCode],[MainMenuIcon],[SubMenuIcon],[ModuleIcon] ,[MainMenuDispOrd] ,[ShowFinYear],[ShowOrg],[Active] ,[CreatedBy]
           ,[CreatedDate])
     VALUES
           ('MDL_FINANCE_MGMT','Finance','1',(select top  1 id from AppAccess where AccessCode ='SCR_LEDGER_HISTROY'),'Ledger Histroy','2','MNU_FINANCE_REP','MNU_LEDGER_HISTROY',
		   'show_chart','','account_balance','2','True','True','Y','5158d424-b0af-4f06-8302-69d767db6c9d','2021-10-13 09:59:00')



INSERT INTO CodesDetail([Code],[Description],[DisplayOrder],[Active],[CreatedBy] ,[CreatedDate],[CodesMasterId])
     VALUES
        ('CLEARENCEORDERNO','Order No','2','Y','b04a242c-11fb-4596-8875-55007be62557','2021-10-25 19:52:00',(select top  1 id from CodesMaster where Code ='EXPORTHEADER'))






INSERT INTO CodesDetail([Code],[Description],[DisplayOrder],[Active],[CreatedBy] ,[CreatedDate],[CodesMasterId])
     VALUES
        ('CLEARENCEORDERDATE','Order Date','2','Y','b04a242c-11fb-4596-8875-55007be62557','2021-10-25 19:52:00',(select top  1 id from CodesMaster where Code ='EXPORTHEADER'))




INSERT INTO [CodesDetail]([Code],[Description],[DisplayOrder],[Active],[CreatedBy],[CreatedDate],[CodesMasterId])
     VALUES('EMBASSYCOMM','Embassy Commission Account','3','Y','5158d424-b0af-4f06-8302-69d767db6c9d','2021-10-25 19:52:00',(select top  1 id from CodesMaster where Code ='LDGACCUSEDFORCR'))
           

Insert Into AppAccess (AccessName,AccessCode,AccessType,Active,CreatedBy,CreatedDate)
values ('Email and Sms','SCR_EMAIL_SMS','C','Y','5158d424-b0af-4f06-8302-69d767db6c9d','2021-10-14 20:40:00')


INSERT INTO CodesDetail([Code],[Description],[DisplayOrder],[Active],[CreatedBy] ,[CreatedDate],[CodesMasterId])
     VALUES
        ('SHELVENO','ShelveNo','2','Y','b04a242c-11fb-4596-8875-55007be62557','2021-10-25 19:52:00',(select top  1 id from CodesMaster where Code ='EXPORTHEADER'))


INSERT INTO CodesDetail([Code],[Description],[DisplayOrder],[Active],[CreatedBy] ,[CreatedDate],[CodesMasterId])
     VALUES
        ('BARCODE','Barcode','2','Y','b04a242c-11fb-4596-8875-55007be62557','2021-10-25 19:52:00',(select top  1 id from CodesMaster where Code ='EXPORTHEADER'))




