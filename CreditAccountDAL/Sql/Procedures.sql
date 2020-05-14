
IF OBJECT_ID ('dbo.GetBalance', 'P') IS NOT NULL   
    DROP PROCEDURE dbo.GetBalance;  
GO  
CREATE PROCEDURE dbo.GetBalance 
    @userId BIGINT,
    @resultStatus INT = 0 OUT
AS
BEGIN
	BEGIN TRANSACTION 
		IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @userId)
		BEGIN
			SET @resultStatus = 1;
			ROLLBACK TRANSACTION;
			RETURN
		END
		SELECT CurrencyCode, [Money] FROM Accounts WHERE UserId = @userId
	COMMIT TRANSACTION
END
GO

IF OBJECT_ID ('dbo.PutMoney', 'P') IS NOT NULL   
    DROP PROCEDURE dbo.PutMoney;  
GO  
CREATE PROCEDURE dbo.PutMoney 
    @userId BIGINT,
	@currencyCode INT,
	@money MONEY,
    @resultStatus INT = 0 OUT
AS
BEGIN
	BEGIN TRANSACTION 
		IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @userId)
		BEGIN
			SET @resultStatus = 1;
			ROLLBACK TRANSACTION;
			RETURN;
		END

		IF @money <= 0
		BEGIN
			SET @resultStatus = 2;
			ROLLBACK TRANSACTION;
			RETURN
		END

		MERGE INTO dbo.Account AS tgt
		USING (VALUES(@userId, @currencyCode, @money))
			AS src (UserId, CurrencyCode, [Money])
		ON	src.CurrencyCode = tgt.UserId AND src.CurrencyCode = tgt.CurrencyCode
		WHEN MATCHED THEN UPDATE 
			SET tgt.[Money] = tgt.[Money] + src.[Money]
		WHEN NOT MATCHED THEN INSERT
			VALUES(src.userId, src.currencyCode, tgt.[Money] + src.[money]);
	COMMIT TRANSACTION
END

IF OBJECT_ID ('dbo.WithdrawMoney', 'P') IS NOT NULL   
    DROP PROCEDURE dbo.WithdrawMoney;  
GO  
CREATE PROCEDURE dbo.WithdrawMoney 
    @userId BIGINT,
	@currencyCode INT,
	@money MONEY,
    @resultStatus INT = 0 OUT
AS
BEGIN
	BEGIN TRANSACTION 
		IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @userId)
		BEGIN
			SET @resultStatus = 1;
			ROLLBACK TRANSACTION;
			RETURN
		END

		IF @money <= 0
		BEGIN
			SET @resultStatus = 2;
			ROLLBACK TRANSACTION;
			RETURN
		END

		DECLARE @moneyInCurrency MONEY;
		SET  @moneyInCurrency = (SELECT [Money] FROM Accounts WHERE UserId = @userId AND CurrencyCode = @currencyCode);
		IF @moneyInCurrency IS NULL
		BEGIN
			SET @resultStatus = 3;
			ROLLBACK TRANSACTION;
			RETURN
		END

		IF @moneyInCurrency < @money
		BEGIN
			SET @resultStatus = 4;
			ROLLBACK TRANSACTION;
			RETURN
		END

		UPDATE Accounts SET [Money] = [Money] - @money WHERE UserId = @userId AND CurrencyCode = @currencyCode;
	COMMIT TRANSACTION;
END

GO

IF OBJECT_ID ('dbo.ChangeCurrency', 'P') IS NOT NULL   
    DROP PROCEDURE dbo.ChangeCurrency;  
GO  
CREATE PROCEDURE dbo.ChangeCurrency 
    @userId BIGINT,
	@fromCurrencyCode INT,
	@toCurrencyCode INT,
	@fromCurrencyMoney MONEY,
	@toCurrencyMoney MONEY,
    @resultStatus INT = 0 OUT
AS
BEGIN
	BEGIN TRANSACTION 
		EXEC dbo.WithdrawMoney @userId, @fromCurrencyCode, @fromCurrencyMoney, @resultStatus OUT;
		IF @resultStatus != 0
			RETURN;

		IF @toCurrencyMoney <= 0
		BEGIN
			SET @resultStatus = 5;
			ROLLBACK TRANSACTION;
			RETURN
		END

		MERGE INTO dbo.Account AS tgt
		USING (VALUES(@userId, @toCurrencyCode, @toCurrencyMoney))
			AS src (UserId, CurrencyCode, [Money])
		ON	src.CurrencyCode = tgt.UserId AND src.CurrencyCode = tgt.CurrencyCode
		WHEN MATCHED THEN UPDATE 
			SET tgt.[Money] = tgt.[Money] + src.[Money]
		WHEN NOT MATCHED THEN INSERT
			VALUES(src.userId, src.currencyCode, tgt.[Money] + src.[money]);
	COMMIT TRANSACTION;
END
GO

