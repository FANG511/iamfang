ALTER TABLE ErpApHostDb ALTER COLUMN ErpSynResult NVARCHAR(MAX);
ALTER TABLE ErpApHostDb ADD ApUploadResult NVARCHAR(50);
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上傳結果欄位設定' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ErpApHostDb', @level2type=N'COLUMN',@level2name=N'ApUploadResult'