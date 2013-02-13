IF EXISTS(SELECT id FROM sysobjects WHERE name='PayPalTransactionResponse_Insert' AND type='P')
	DROP PROCEDURE PayPalTransactionResponse_Insert
go
CREATE PROCEDURE dbo.PayPalTransactionResponse_Insert
	@TransactionID uniqueidentifier,
	@txn_id nvarchar(17),
	@txn_type nvarchar(40) = null,
	@auth_amount money = null,
	@auth_id nvarchar(40) = null,
	@auth_exp datetime = null,
	@auth_status nvarchar(20) = null,
	@mc_gross_x money = null,
	@mc_handling_x money = null,
	@num_cart_items int = null,
	@parent_txn_id nvarchar(17) = null,
	@payment_date datetime = null,
	@payment_status nvarchar(40) = null,
	@payment_type nvarchar(20) = null,
	@pending_reason nvarchar(20) = null,
	@reason_code nvarchar(20) = null,
	@remaining_settle money = null,
	@transaction_entity nvarchar(20) = null,
	@invoice nvarchar(127) = null,
	@memo nvarchar(255) = null,
	@tax money = null,
	@business nvarchar(127) = null,
	@item_name nvarchar(127) = null,
	@item_number nvarchar(127) = null,
	@quantity int = null,
	@receiver_email nvarchar(127) = null,
	@receiver_id nvarchar(13) = null,
	@address_city nvarchar(40) = null,
	@address_country nvarchar(64) = null,
	@address_country_code nvarchar(2) = null,
	@address_name nvarchar(128) = null,
	@address_state nvarchar(40) = null,
	@address_status nvarchar(20) = null,
	@address_street nvarchar(200) = null,
	@address_zip nvarchar(20) = null,
	@first_name nvarchar(64) = null,
	@last_name nvarchar(64) = null,
	@payer_id nvarchar(13) = null,
	@payer_status nvarchar(20) = null,
	@residence_country nvarchar(2) = null,
	@exchange_rate money = null,
	@mc_fee money = null,
	@mc_gross money = null,
	@mc_handling# money = null,
	@mc_shipping# money = null,
	@payment_fee money = null,
	@payment_gross money = null,
	@settle_amount money = null,
	@settle_currency nvarchar(10) = null,
	@subscr_date datetime = null,
	@subscr_effective datetime = null,
	@period1 nvarchar(20) = null,
	@period2 nvarchar(64) = null,
	@period3 nvarchar(20) = null,
	@amount1 money = null,
	@amount2 money = null,
	@amount3 money = null,
	@mc_amount1 money = null,
	@mc_amount2 money = null,
	@mc_amount3 money = null,
	@mc_currency nvarchar(10) = null,
	@recurring bit = null,
	@reattempt bit = null,
	@retry_at datetime = null,
	@recur_times int = null,
	@username nvarchar(64) = null,
	@password nvarchar(127) = null,
	@subscr_id nvarchar(19) = null,
	@custom nvarchar(255) = null
AS
BEGIN
	INSERT INTO PayPalTransactionResponses
		(TransactionID, txn_id, txn_type, auth_amount, auth_id, auth_exp, auth_status, mc_gross_x, mc_handling_x, num_cart_items, parent_txn_id, payment_date, payment_status, payment_type, pending_reason, reason_code, remaining_settle, transaction_entity, invoice, memo, tax, business, item_name, item_number, quantity, receiver_email, receiver_id, address_city, address_country, address_country_code, address_name, address_state, address_status, address_street, address_zip, first_name, last_name, payer_id, payer_status, residence_country, exchange_rate, mc_fee, mc_gross, mc_handling#, mc_shipping#, payment_fee, payment_gross, settle_amount, settle_currency, subscr_date, subscr_effective, period1, period2, period3, amount1, amount2, amount3, mc_amount1, mc_amount2, mc_amount3, mc_currency, recurring, reattempt, retry_at, recur_times, username, password, subscr_id, custom)
	VALUES
		(@TransactionID, @txn_id, @txn_type, @auth_amount, @auth_id, @auth_exp, @auth_status, @mc_gross_x, @mc_handling_x, @num_cart_items, @parent_txn_id, @payment_date, @payment_status, @payment_type, @pending_reason, @reason_code, @remaining_settle, @transaction_entity, @invoice, @memo, @tax, @business, @item_name, @item_number, @quantity, @receiver_email, @receiver_id, @address_city, @address_country, @address_country_code, @address_name, @address_state, @address_status, @address_street, @address_zip, @first_name, @last_name, @payer_id, @payer_status, @residence_country, @exchange_rate, @mc_fee, @mc_gross, @mc_handling#, @mc_shipping#, @payment_fee, @payment_gross, @settle_amount, @settle_currency, @subscr_date, @subscr_effective, @period1, @period2, @period3, @amount1, @amount2, @amount3, @mc_amount1, @mc_amount2, @mc_amount3, @mc_currency, @recurring, @reattempt, @retry_at, @recur_times, @username, @password, @subscr_id, @custom)
END

go

IF EXISTS(SELECT id FROM sysobjects WHERE name='PayPalTransactionResponse_Update' AND type='P')
	DROP PROCEDURE PayPalTransactionResponse_Update
go
CREATE PROCEDURE dbo.PayPalTransactionResponse_Update
	@TransactionID uniqueidentifier,
	@txn_id nvarchar(17) = null,
	@txn_type nvarchar(40) = null,
	@auth_amount money = null,
	@auth_id nvarchar(40) = null,
	@auth_exp datetime = null,
	@auth_status nvarchar(20) = null,
	@mc_gross_x money = null,
	@mc_handling_x money = null,
	@num_cart_items int = null,
	@parent_txn_id nvarchar(17) = null,
	@payment_date datetime = null,
	@payment_status nvarchar(40) = null,
	@payment_type nvarchar(20) = null,
	@pending_reason nvarchar(20) = null,
	@reason_code nvarchar(20) = null,
	@remaining_settle money = null,
	@transaction_entity nvarchar(20) = null,
	@invoice nvarchar(127) = null,
	@memo nvarchar(255) = null,
	@tax money = null,
	@business nvarchar(127) = null,
	@item_name nvarchar(127) = null,
	@item_number nvarchar(127) = null,
	@quantity int = null,
	@receiver_email nvarchar(127) = null,
	@receiver_id nvarchar(13) = null,
	@address_city nvarchar(40) = null,
	@address_country nvarchar(64) = null,
	@address_country_code nvarchar(2) = null,
	@address_name nvarchar(128) = null,
	@address_state nvarchar(40) = null,
	@address_status nvarchar(20) = null,
	@address_street nvarchar(200) = null,
	@address_zip nvarchar(20) = null,
	@first_name nvarchar(64) = null,
	@last_name nvarchar(64) = null,
	@payer_id nvarchar(13) = null,
	@payer_status nvarchar(20) = null,
	@residence_country nvarchar(2) = null,
	@exchange_rate money = null,
	@mc_fee money = null,
	@mc_gross money = null,
	@mc_handling# money = null,
	@mc_shipping# money = null,
	@payment_fee money = null,
	@payment_gross money = null,
	@settle_amount money = null,
	@settle_currency nvarchar(10) = null,
	@subscr_date datetime = null,
	@subscr_effective datetime = null,
	@period1 nvarchar(20) = null,
	@period2 nvarchar(64) = null,
	@period3 nvarchar(20) = null,
	@amount1 money = null,
	@amount2 money = null,
	@amount3 money = null,
	@mc_amount1 money = null,
	@mc_amount2 money = null,
	@mc_amount3 money = null,
	@mc_currency nvarchar(10) = null,
	@recurring bit = null,
	@reattempt bit = null,
	@retry_at datetime = null,
	@recur_times int = null,
	@username nvarchar(64) = null,
	@password nvarchar(127) = null,
	@subscr_id nvarchar(19) = null,
	@custom nvarchar(255) = null
AS
BEGIN
	UPDATE PayPalTransactionResponses SET
		txn_id = COALESCE(@txn_id, txn_id),
		txn_type = COALESCE(@txn_type, txn_type),
		auth_amount = COALESCE(@auth_amount, auth_amount),
		auth_id = COALESCE(@auth_id, auth_id),
		auth_exp = COALESCE(@auth_exp, auth_exp),
		auth_status = COALESCE(@auth_status, auth_status),
		mc_gross_x = COALESCE(@mc_gross_x, mc_gross_x),
		mc_handling_x = COALESCE(@mc_handling_x, mc_handling_x),
		num_cart_items = COALESCE(@num_cart_items, num_cart_items),
		parent_txn_id = COALESCE(@parent_txn_id, parent_txn_id),
		payment_date = COALESCE(@payment_date, payment_date),
		payment_status = COALESCE(@payment_status, payment_status),
		payment_type = COALESCE(@payment_type, payment_type),
		pending_reason = COALESCE(@pending_reason, pending_reason),
		reason_code = COALESCE(@reason_code, reason_code),
		remaining_settle = COALESCE(@remaining_settle, remaining_settle),
		transaction_entity = COALESCE(@transaction_entity, transaction_entity),
		invoice = COALESCE(@invoice, invoice),
		memo = COALESCE(@memo, memo),
		tax = COALESCE(@tax, tax),
		business = COALESCE(@business, business),
		item_name = COALESCE(@item_name, item_name),
		item_number = COALESCE(@item_number, item_number),
		quantity = COALESCE(@quantity, quantity),
		receiver_email = COALESCE(@receiver_email, receiver_email),
		receiver_id = COALESCE(@receiver_id, receiver_id),
		address_city = COALESCE(@address_city, address_city),
		address_country = COALESCE(@address_country, address_country),
		address_country_code = COALESCE(@address_country_code, address_country_code),
		address_name = COALESCE(@address_name, address_name),
		address_state = COALESCE(@address_state, address_state),
		address_status = COALESCE(@address_status, address_status),
		address_street = COALESCE(@address_street, address_street),
		address_zip = COALESCE(@address_zip, address_zip),
		first_name = COALESCE(@first_name, first_name),
		last_name = COALESCE(@last_name, last_name),
		payer_id = COALESCE(@payer_id, payer_id),
		payer_status = COALESCE(@payer_status, payer_status),
		residence_country = COALESCE(@residence_country, residence_country),
		exchange_rate = COALESCE(@exchange_rate, exchange_rate),
		mc_fee = COALESCE(@mc_fee, mc_fee),
		mc_gross = COALESCE(@mc_gross, mc_gross),
		mc_handling# = COALESCE(@mc_handling#, mc_handling#),
		mc_shipping# = COALESCE(@mc_shipping#, mc_shipping#),
		payment_fee = COALESCE(@payment_fee, payment_fee),
		payment_gross = COALESCE(@payment_gross, payment_gross),
		settle_amount = COALESCE(@settle_amount, settle_amount),
		settle_currency = COALESCE(@settle_currency, settle_currency),
		subscr_date = COALESCE(@subscr_date, subscr_date),
		subscr_effective = COALESCE(@subscr_effective, subscr_effective),
		period1 = COALESCE(@period1, period1),
		period2 = COALESCE(@period2, period2),
		period3 = COALESCE(@period3, period3),
		amount1 = COALESCE(@amount1, amount1),
		amount2 = COALESCE(@amount2, amount2),
		amount3 = COALESCE(@amount3, amount3),
		mc_amount1 = COALESCE(@mc_amount1, mc_amount1),
		mc_amount2 = COALESCE(@mc_amount2, mc_amount2),
		mc_amount3 = COALESCE(@mc_amount3, mc_amount3),
		mc_currency = COALESCE(@mc_currency, mc_currency),
		recurring = COALESCE(@recurring, recurring),
		reattempt = COALESCE(@reattempt, reattempt),
		retry_at = COALESCE(@retry_at, retry_at),
		recur_times = COALESCE(@recur_times, recur_times),
		username = COALESCE(@username, username),
		password = COALESCE(@password, password),
		subscr_id = COALESCE(@subscr_id, subscr_id),
		custom = COALESCE(@custom, custom)
	WHERE TransactionID = @TransactionID
END

go

IF EXISTS(SELECT id FROM sysobjects WHERE name='PayPalTransactionResponse_UpdateExplicit' AND type='P')
	DROP PROCEDURE PayPalTransactionResponse_UpdateExplicit
go
CREATE PROCEDURE dbo.PayPalTransactionResponse_UpdateExplicit
	@TransactionID uniqueidentifier,
	@txn_id nvarchar(17),
	@txn_type nvarchar(40),
	@auth_amount money,
	@auth_id nvarchar(40),
	@auth_exp datetime,
	@auth_status nvarchar(20),
	@mc_gross_x money,
	@mc_handling_x money,
	@num_cart_items int,
	@parent_txn_id nvarchar(17),
	@payment_date datetime,
	@payment_status nvarchar(40),
	@payment_type nvarchar(20),
	@pending_reason nvarchar(20),
	@reason_code nvarchar(20),
	@remaining_settle money,
	@transaction_entity nvarchar(20),
	@invoice nvarchar(127),
	@memo nvarchar(255),
	@tax money,
	@business nvarchar(127),
	@item_name nvarchar(127),
	@item_number nvarchar(127),
	@quantity int,
	@receiver_email nvarchar(127),
	@receiver_id nvarchar(13),
	@address_city nvarchar(40),
	@address_country nvarchar(64),
	@address_country_code nvarchar(2),
	@address_name nvarchar(128),
	@address_state nvarchar(40),
	@address_status nvarchar(20),
	@address_street nvarchar(200),
	@address_zip nvarchar(20),
	@first_name nvarchar(64),
	@last_name nvarchar(64),
	@payer_id nvarchar(13),
	@payer_status nvarchar(20),
	@residence_country nvarchar(2),
	@exchange_rate money,
	@mc_fee money,
	@mc_gross money,
	@mc_handling# money,
	@mc_shipping# money,
	@payment_fee money,
	@payment_gross money,
	@settle_amount money,
	@settle_currency nvarchar(10),
	@subscr_date datetime,
	@subscr_effective datetime,
	@period1 nvarchar(20),
	@period2 nvarchar(64),
	@period3 nvarchar(20),
	@amount1 money,
	@amount2 money,
	@amount3 money,
	@mc_amount1 money,
	@mc_amount2 money,
	@mc_amount3 money,
	@mc_currency nvarchar(10),
	@recurring bit,
	@reattempt bit,
	@retry_at datetime,
	@recur_times int,
	@username nvarchar(64),
	@password nvarchar(127),
	@subscr_id nvarchar(19),
	@custom nvarchar(255)
AS
BEGIN
	UPDATE PayPalTransactionResponses SET
		txn_id = @txn_id,
		txn_type = @txn_type,
		auth_amount = @auth_amount,
		auth_id = @auth_id,
		auth_exp = @auth_exp,
		auth_status = @auth_status,
		mc_gross_x = @mc_gross_x,
		mc_handling_x = @mc_handling_x,
		num_cart_items = @num_cart_items,
		parent_txn_id = @parent_txn_id,
		payment_date = @payment_date,
		payment_status = @payment_status,
		payment_type = @payment_type,
		pending_reason = @pending_reason,
		reason_code = @reason_code,
		remaining_settle = @remaining_settle,
		transaction_entity = @transaction_entity,
		invoice = @invoice,
		memo = @memo,
		tax = @tax,
		business = @business,
		item_name = @item_name,
		item_number = @item_number,
		quantity = @quantity,
		receiver_email = @receiver_email,
		receiver_id = @receiver_id,
		address_city = @address_city,
		address_country = @address_country,
		address_country_code = @address_country_code,
		address_name = @address_name,
		address_state = @address_state,
		address_status = @address_status,
		address_street = @address_street,
		address_zip = @address_zip,
		first_name = @first_name,
		last_name = @last_name,
		payer_id = @payer_id,
		payer_status = @payer_status,
		residence_country = @residence_country,
		exchange_rate = @exchange_rate,
		mc_fee = @mc_fee,
		mc_gross = @mc_gross,
		mc_handling# = @mc_handling#,
		mc_shipping# = @mc_shipping#,
		payment_fee = @payment_fee,
		payment_gross = @payment_gross,
		settle_amount = @settle_amount,
		settle_currency = @settle_currency,
		subscr_date = @subscr_date,
		subscr_effective = @subscr_effective,
		period1 = @period1,
		period2 = @period2,
		period3 = @period3,
		amount1 = @amount1,
		amount2 = @amount2,
		amount3 = @amount3,
		mc_amount1 = @mc_amount1,
		mc_amount2 = @mc_amount2,
		mc_amount3 = @mc_amount3,
		mc_currency = @mc_currency,
		recurring = @recurring,
		reattempt = @reattempt,
		retry_at = @retry_at,
		recur_times = @recur_times,
		username = @username,
		password = @password,
		subscr_id = @subscr_id,
		custom = @custom
	WHERE TransactionID = @TransactionID
END

go

IF EXISTS(SELECT id FROM sysobjects WHERE name='PayPalTransactionResponse_Select' AND type='P')
	DROP PROCEDURE PayPalTransactionResponse_Select
go
CREATE PROCEDURE dbo.PayPalTransactionResponse_Select
	@TransactionID uniqueidentifier
AS
BEGIN
	SELECT *
	  FROM PayPalTransactionResponses
	 WHERE TransactionID = @TransactionID
END

go

IF EXISTS(SELECT id FROM sysobjects WHERE name='PayPalTransactionResponse_Delete' AND type='P')
	DROP PROCEDURE PayPalTransactionResponse_Delete
go
CREATE PROCEDURE dbo.PayPalTransactionResponse_Delete
	@TransactionID uniqueidentifier
AS
BEGIN
	DELETE
	  FROM PayPalTransactionResponses
	 WHERE TransactionID = @TransactionID
END

go

IF EXISTS(SELECT id FROM sysobjects WHERE name='PayPalTransactionResponse_Count' AND type='P')
	DROP PROCEDURE PayPalTransactionResponse_Count

go

CREATE PROCEDURE dbo.PayPalTransactionResponse_Count
	@TransactionID uniqueidentifier = null,
	@txn_id nvarchar(17) = null,
	@txn_type nvarchar(40) = null,
	@auth_amount money = null,
	@auth_id nvarchar(40) = null,
	@auth_exp_Min datetime = null,
	@auth_exp_Max datetime = null,
	@auth_status nvarchar(20) = null,
	@mc_gross_x money = null,
	@mc_handling_x money = null,
	@num_cart_items int = null,
	@parent_txn_id nvarchar(17) = null,
	@payment_date_Min datetime = null,
	@payment_date_Max datetime = null,
	@payment_status nvarchar(40) = null,
	@payment_type nvarchar(20) = null,
	@pending_reason nvarchar(20) = null,
	@reason_code nvarchar(20) = null,
	@remaining_settle money = null,
	@transaction_entity nvarchar(20) = null,
	@invoice nvarchar(127) = null,
	@memo nvarchar(255) = null,
	@tax money = null,
	@business nvarchar(127) = null,
	@item_name nvarchar(127) = null,
	@item_number nvarchar(127) = null,
	@quantity int = null,
	@receiver_email nvarchar(127) = null,
	@receiver_id nvarchar(13) = null,
	@address_city nvarchar(40) = null,
	@address_country nvarchar(64) = null,
	@address_country_code nvarchar(2) = null,
	@address_name nvarchar(128) = null,
	@address_state nvarchar(40) = null,
	@address_status nvarchar(20) = null,
	@address_street nvarchar(200) = null,
	@address_zip nvarchar(20) = null,
	@first_name nvarchar(64) = null,
	@last_name nvarchar(64) = null,
	@payer_id nvarchar(13) = null,
	@payer_status nvarchar(20) = null,
	@residence_country nvarchar(2) = null,
	@exchange_rate money = null,
	@mc_fee money = null,
	@mc_gross money = null,
	@mc_handling# money = null,
	@mc_shipping# money = null,
	@payment_fee money = null,
	@payment_gross money = null,
	@settle_amount money = null,
	@settle_currency nvarchar(10) = null,
	@subscr_date_Min datetime = null,
	@subscr_date_Max datetime = null,
	@subscr_effective_Min datetime = null,
	@subscr_effective_Max datetime = null,
	@period1 nvarchar(20) = null,
	@period2 nvarchar(64) = null,
	@period3 nvarchar(20) = null,
	@amount1 money = null,
	@amount2 money = null,
	@amount3 money = null,
	@mc_amount1 money = null,
	@mc_amount2 money = null,
	@mc_amount3 money = null,
	@mc_currency nvarchar(10) = null,
	@recurring bit = null,
	@reattempt bit = null,
	@retry_at_Min datetime = null,
	@retry_at_Max datetime = null,
	@recur_times int = null,
	@username nvarchar(64) = null,
	@password nvarchar(127) = null,
	@subscr_id nvarchar(19) = null,
	@custom nvarchar(255) = null,
	@Count int = NULL OUTPUT
AS
BEGIN
	SELECT @Count = COUNT(*)
	  FROM PayPalTransactionResponses
	 WHERE (@TransactionID IS NULL OR TransactionID = @TransactionID)
		   AND (@txn_id IS NULL OR txn_id = @txn_id)
		   AND (@txn_type IS NULL OR txn_type = @txn_type)
		   AND (@auth_amount IS NULL OR auth_amount = @auth_amount)
		   AND (@auth_id IS NULL OR auth_id = @auth_id)
		   AND ((@auth_exp_Min IS NULL AND @auth_exp_Max IS NULL) OR
			(@auth_exp_Min IS NULL AND @auth_exp_Max IS NOT NULL AND @auth_exp_Max >= auth_exp) OR
			(@auth_exp_Max IS NULL AND @auth_exp_Min IS NOT NULL AND @auth_exp_Min <= auth_exp) OR
			(@auth_exp_Max IS NOT NULL AND @auth_exp_Min IS NOT NULL AND auth_exp BETWEEN @auth_exp_Min AND @auth_exp_Max))
		   AND (@auth_status IS NULL OR auth_status = @auth_status)
		   AND (@mc_gross_x IS NULL OR mc_gross_x = @mc_gross_x)
		   AND (@mc_handling_x IS NULL OR mc_handling_x = @mc_handling_x)
		   AND (@num_cart_items IS NULL OR num_cart_items = @num_cart_items)
		   AND (@parent_txn_id IS NULL OR parent_txn_id = @parent_txn_id)
		   AND ((@payment_date_Min IS NULL AND @payment_date_Max IS NULL) OR
			(@payment_date_Min IS NULL AND @payment_date_Max IS NOT NULL AND @payment_date_Max >= payment_date) OR
			(@payment_date_Max IS NULL AND @payment_date_Min IS NOT NULL AND @payment_date_Min <= payment_date) OR
			(@payment_date_Max IS NOT NULL AND @payment_date_Min IS NOT NULL AND payment_date BETWEEN @payment_date_Min AND @payment_date_Max))
		   AND (@payment_status IS NULL OR payment_status = @payment_status)
		   AND (@payment_type IS NULL OR payment_type = @payment_type)
		   AND (@pending_reason IS NULL OR pending_reason = @pending_reason)
		   AND (@reason_code IS NULL OR reason_code = @reason_code)
		   AND (@remaining_settle IS NULL OR remaining_settle = @remaining_settle)
		   AND (@transaction_entity IS NULL OR transaction_entity = @transaction_entity)
		   AND (@invoice IS NULL OR invoice = @invoice)
		   AND (@memo IS NULL OR memo = @memo)
		   AND (@tax IS NULL OR tax = @tax)
		   AND (@business IS NULL OR business = @business)
		   AND (@item_name IS NULL OR item_name = @item_name)
		   AND (@item_number IS NULL OR item_number = @item_number)
		   AND (@quantity IS NULL OR quantity = @quantity)
		   AND (@receiver_email IS NULL OR receiver_email = @receiver_email)
		   AND (@receiver_id IS NULL OR receiver_id = @receiver_id)
		   AND (@address_city IS NULL OR address_city = @address_city)
		   AND (@address_country IS NULL OR address_country = @address_country)
		   AND (@address_country_code IS NULL OR address_country_code = @address_country_code)
		   AND (@address_name IS NULL OR address_name = @address_name)
		   AND (@address_state IS NULL OR address_state = @address_state)
		   AND (@address_status IS NULL OR address_status = @address_status)
		   AND (@address_street IS NULL OR address_street = @address_street)
		   AND (@address_zip IS NULL OR address_zip = @address_zip)
		   AND (@first_name IS NULL OR first_name = @first_name)
		   AND (@last_name IS NULL OR last_name = @last_name)
		   AND (@payer_id IS NULL OR payer_id = @payer_id)
		   AND (@payer_status IS NULL OR payer_status = @payer_status)
		   AND (@residence_country IS NULL OR residence_country = @residence_country)
		   AND (@exchange_rate IS NULL OR exchange_rate = @exchange_rate)
		   AND (@mc_fee IS NULL OR mc_fee = @mc_fee)
		   AND (@mc_gross IS NULL OR mc_gross = @mc_gross)
		   AND (@mc_handling# IS NULL OR mc_handling# = @mc_handling#)
		   AND (@mc_shipping# IS NULL OR mc_shipping# = @mc_shipping#)
		   AND (@payment_fee IS NULL OR payment_fee = @payment_fee)
		   AND (@payment_gross IS NULL OR payment_gross = @payment_gross)
		   AND (@settle_amount IS NULL OR settle_amount = @settle_amount)
		   AND (@settle_currency IS NULL OR settle_currency = @settle_currency)
		   AND ((@subscr_date_Min IS NULL AND @subscr_date_Max IS NULL) OR
			(@subscr_date_Min IS NULL AND @subscr_date_Max IS NOT NULL AND @subscr_date_Max >= subscr_date) OR
			(@subscr_date_Max IS NULL AND @subscr_date_Min IS NOT NULL AND @subscr_date_Min <= subscr_date) OR
			(@subscr_date_Max IS NOT NULL AND @subscr_date_Min IS NOT NULL AND subscr_date BETWEEN @subscr_date_Min AND @subscr_date_Max))
		   AND ((@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NULL) OR
			(@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NOT NULL AND @subscr_effective_Max >= subscr_effective) OR
			(@subscr_effective_Max IS NULL AND @subscr_effective_Min IS NOT NULL AND @subscr_effective_Min <= subscr_effective) OR
			(@subscr_effective_Max IS NOT NULL AND @subscr_effective_Min IS NOT NULL AND subscr_effective BETWEEN @subscr_effective_Min AND @subscr_effective_Max))
		   AND (@period1 IS NULL OR period1 = @period1)
		   AND (@period2 IS NULL OR period2 = @period2)
		   AND (@period3 IS NULL OR period3 = @period3)
		   AND (@amount1 IS NULL OR amount1 = @amount1)
		   AND (@amount2 IS NULL OR amount2 = @amount2)
		   AND (@amount3 IS NULL OR amount3 = @amount3)
		   AND (@mc_amount1 IS NULL OR mc_amount1 = @mc_amount1)
		   AND (@mc_amount2 IS NULL OR mc_amount2 = @mc_amount2)
		   AND (@mc_amount3 IS NULL OR mc_amount3 = @mc_amount3)
		   AND (@mc_currency IS NULL OR mc_currency = @mc_currency)
		   AND (@recurring IS NULL OR recurring = @recurring)
		   AND (@reattempt IS NULL OR reattempt = @reattempt)
		   AND ((@retry_at_Min IS NULL AND @retry_at_Max IS NULL) OR
			(@retry_at_Min IS NULL AND @retry_at_Max IS NOT NULL AND @retry_at_Max >= retry_at) OR
			(@retry_at_Max IS NULL AND @retry_at_Min IS NOT NULL AND @retry_at_Min <= retry_at) OR
			(@retry_at_Max IS NOT NULL AND @retry_at_Min IS NOT NULL AND retry_at BETWEEN @retry_at_Min AND @retry_at_Max))
		   AND (@recur_times IS NULL OR recur_times = @recur_times)
		   AND (@username IS NULL OR username = @username)
		   AND (@password IS NULL OR password = @password)
		   AND (@subscr_id IS NULL OR subscr_id = @subscr_id)
		   AND (@custom IS NULL OR custom = @custom)
END

go

IF EXISTS(SELECT id FROM sysobjects WHERE name='PayPalTransactionResponse_Filter' AND type='P')
	DROP PROCEDURE PayPalTransactionResponse_Filter

go

CREATE PROCEDURE dbo.PayPalTransactionResponse_Filter
	@TransactionID uniqueidentifier = null,
	@txn_id nvarchar(17) = null,
	@txn_type nvarchar(40) = null,
	@auth_amount money = null,
	@auth_id nvarchar(40) = null,
	@auth_exp_Min datetime = null,
	@auth_exp_Max datetime = null,
	@auth_status nvarchar(20) = null,
	@mc_gross_x money = null,
	@mc_handling_x money = null,
	@num_cart_items int = null,
	@parent_txn_id nvarchar(17) = null,
	@payment_date_Min datetime = null,
	@payment_date_Max datetime = null,
	@payment_status nvarchar(40) = null,
	@payment_type nvarchar(20) = null,
	@pending_reason nvarchar(20) = null,
	@reason_code nvarchar(20) = null,
	@remaining_settle money = null,
	@transaction_entity nvarchar(20) = null,
	@invoice nvarchar(127) = null,
	@memo nvarchar(255) = null,
	@tax money = null,
	@business nvarchar(127) = null,
	@item_name nvarchar(127) = null,
	@item_number nvarchar(127) = null,
	@quantity int = null,
	@receiver_email nvarchar(127) = null,
	@receiver_id nvarchar(13) = null,
	@address_city nvarchar(40) = null,
	@address_country nvarchar(64) = null,
	@address_country_code nvarchar(2) = null,
	@address_name nvarchar(128) = null,
	@address_state nvarchar(40) = null,
	@address_status nvarchar(20) = null,
	@address_street nvarchar(200) = null,
	@address_zip nvarchar(20) = null,
	@first_name nvarchar(64) = null,
	@last_name nvarchar(64) = null,
	@payer_id nvarchar(13) = null,
	@payer_status nvarchar(20) = null,
	@residence_country nvarchar(2) = null,
	@exchange_rate money = null,
	@mc_fee money = null,
	@mc_gross money = null,
	@mc_handling# money = null,
	@mc_shipping# money = null,
	@payment_fee money = null,
	@payment_gross money = null,
	@settle_amount money = null,
	@settle_currency nvarchar(10) = null,
	@subscr_date_Min datetime = null,
	@subscr_date_Max datetime = null,
	@subscr_effective_Min datetime = null,
	@subscr_effective_Max datetime = null,
	@period1 nvarchar(20) = null,
	@period2 nvarchar(64) = null,
	@period3 nvarchar(20) = null,
	@amount1 money = null,
	@amount2 money = null,
	@amount3 money = null,
	@mc_amount1 money = null,
	@mc_amount2 money = null,
	@mc_amount3 money = null,
	@mc_currency nvarchar(10) = null,
	@recurring bit = null,
	@reattempt bit = null,
	@retry_at_Min datetime = null,
	@retry_at_Max datetime = null,
	@recur_times int = null,
	@username nvarchar(64) = null,
	@password nvarchar(127) = null,
	@subscr_id nvarchar(19) = null,
	@custom nvarchar(255) = null,
	@OrderByFieldName nvarchar(150) = null,
	@OrderDirection bit = 1,
	@ResultsPerPage int = null,
	@PageNumber int = 1,
	@TotalResults int = 0 OUTPUT
AS
BEGIN
	IF @ResultsPerPage IS NULL OR @PageNumber IS NULL
	BEGIN
		IF @OrderDirection = 1
			SELECT *
			  FROM PayPalTransactionResponses
			 WHERE (@TransactionID IS NULL OR TransactionID = @TransactionID)
		   AND (@txn_id IS NULL OR txn_id = @txn_id)
		   AND (@txn_type IS NULL OR txn_type = @txn_type)
		   AND (@auth_amount IS NULL OR auth_amount = @auth_amount)
		   AND (@auth_id IS NULL OR auth_id = @auth_id)
		   AND ((@auth_exp_Min IS NULL AND @auth_exp_Max IS NULL) OR
			(@auth_exp_Min IS NULL AND @auth_exp_Max IS NOT NULL AND @auth_exp_Max >= auth_exp) OR
			(@auth_exp_Max IS NULL AND @auth_exp_Min IS NOT NULL AND @auth_exp_Min <= auth_exp) OR
			(@auth_exp_Max IS NOT NULL AND @auth_exp_Min IS NOT NULL AND auth_exp BETWEEN @auth_exp_Min AND @auth_exp_Max))
		   AND (@auth_status IS NULL OR auth_status = @auth_status)
		   AND (@mc_gross_x IS NULL OR mc_gross_x = @mc_gross_x)
		   AND (@mc_handling_x IS NULL OR mc_handling_x = @mc_handling_x)
		   AND (@num_cart_items IS NULL OR num_cart_items = @num_cart_items)
		   AND (@parent_txn_id IS NULL OR parent_txn_id = @parent_txn_id)
		   AND ((@payment_date_Min IS NULL AND @payment_date_Max IS NULL) OR
			(@payment_date_Min IS NULL AND @payment_date_Max IS NOT NULL AND @payment_date_Max >= payment_date) OR
			(@payment_date_Max IS NULL AND @payment_date_Min IS NOT NULL AND @payment_date_Min <= payment_date) OR
			(@payment_date_Max IS NOT NULL AND @payment_date_Min IS NOT NULL AND payment_date BETWEEN @payment_date_Min AND @payment_date_Max))
		   AND (@payment_status IS NULL OR payment_status = @payment_status)
		   AND (@payment_type IS NULL OR payment_type = @payment_type)
		   AND (@pending_reason IS NULL OR pending_reason = @pending_reason)
		   AND (@reason_code IS NULL OR reason_code = @reason_code)
		   AND (@remaining_settle IS NULL OR remaining_settle = @remaining_settle)
		   AND (@transaction_entity IS NULL OR transaction_entity = @transaction_entity)
		   AND (@invoice IS NULL OR invoice = @invoice)
		   AND (@memo IS NULL OR memo = @memo)
		   AND (@tax IS NULL OR tax = @tax)
		   AND (@business IS NULL OR business = @business)
		   AND (@item_name IS NULL OR item_name = @item_name)
		   AND (@item_number IS NULL OR item_number = @item_number)
		   AND (@quantity IS NULL OR quantity = @quantity)
		   AND (@receiver_email IS NULL OR receiver_email = @receiver_email)
		   AND (@receiver_id IS NULL OR receiver_id = @receiver_id)
		   AND (@address_city IS NULL OR address_city = @address_city)
		   AND (@address_country IS NULL OR address_country = @address_country)
		   AND (@address_country_code IS NULL OR address_country_code = @address_country_code)
		   AND (@address_name IS NULL OR address_name = @address_name)
		   AND (@address_state IS NULL OR address_state = @address_state)
		   AND (@address_status IS NULL OR address_status = @address_status)
		   AND (@address_street IS NULL OR address_street = @address_street)
		   AND (@address_zip IS NULL OR address_zip = @address_zip)
		   AND (@first_name IS NULL OR first_name = @first_name)
		   AND (@last_name IS NULL OR last_name = @last_name)
		   AND (@payer_id IS NULL OR payer_id = @payer_id)
		   AND (@payer_status IS NULL OR payer_status = @payer_status)
		   AND (@residence_country IS NULL OR residence_country = @residence_country)
		   AND (@exchange_rate IS NULL OR exchange_rate = @exchange_rate)
		   AND (@mc_fee IS NULL OR mc_fee = @mc_fee)
		   AND (@mc_gross IS NULL OR mc_gross = @mc_gross)
		   AND (@mc_handling# IS NULL OR mc_handling# = @mc_handling#)
		   AND (@mc_shipping# IS NULL OR mc_shipping# = @mc_shipping#)
		   AND (@payment_fee IS NULL OR payment_fee = @payment_fee)
		   AND (@payment_gross IS NULL OR payment_gross = @payment_gross)
		   AND (@settle_amount IS NULL OR settle_amount = @settle_amount)
		   AND (@settle_currency IS NULL OR settle_currency = @settle_currency)
		   AND ((@subscr_date_Min IS NULL AND @subscr_date_Max IS NULL) OR
			(@subscr_date_Min IS NULL AND @subscr_date_Max IS NOT NULL AND @subscr_date_Max >= subscr_date) OR
			(@subscr_date_Max IS NULL AND @subscr_date_Min IS NOT NULL AND @subscr_date_Min <= subscr_date) OR
			(@subscr_date_Max IS NOT NULL AND @subscr_date_Min IS NOT NULL AND subscr_date BETWEEN @subscr_date_Min AND @subscr_date_Max))
		   AND ((@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NULL) OR
			(@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NOT NULL AND @subscr_effective_Max >= subscr_effective) OR
			(@subscr_effective_Max IS NULL AND @subscr_effective_Min IS NOT NULL AND @subscr_effective_Min <= subscr_effective) OR
			(@subscr_effective_Max IS NOT NULL AND @subscr_effective_Min IS NOT NULL AND subscr_effective BETWEEN @subscr_effective_Min AND @subscr_effective_Max))
		   AND (@period1 IS NULL OR period1 = @period1)
		   AND (@period2 IS NULL OR period2 = @period2)
		   AND (@period3 IS NULL OR period3 = @period3)
		   AND (@amount1 IS NULL OR amount1 = @amount1)
		   AND (@amount2 IS NULL OR amount2 = @amount2)
		   AND (@amount3 IS NULL OR amount3 = @amount3)
		   AND (@mc_amount1 IS NULL OR mc_amount1 = @mc_amount1)
		   AND (@mc_amount2 IS NULL OR mc_amount2 = @mc_amount2)
		   AND (@mc_amount3 IS NULL OR mc_amount3 = @mc_amount3)
		   AND (@mc_currency IS NULL OR mc_currency = @mc_currency)
		   AND (@recurring IS NULL OR recurring = @recurring)
		   AND (@reattempt IS NULL OR reattempt = @reattempt)
		   AND ((@retry_at_Min IS NULL AND @retry_at_Max IS NULL) OR
			(@retry_at_Min IS NULL AND @retry_at_Max IS NOT NULL AND @retry_at_Max >= retry_at) OR
			(@retry_at_Max IS NULL AND @retry_at_Min IS NOT NULL AND @retry_at_Min <= retry_at) OR
			(@retry_at_Max IS NOT NULL AND @retry_at_Min IS NOT NULL AND retry_at BETWEEN @retry_at_Min AND @retry_at_Max))
		   AND (@recur_times IS NULL OR recur_times = @recur_times)
		   AND (@username IS NULL OR username = @username)
		   AND (@password IS NULL OR password = @password)
		   AND (@subscr_id IS NULL OR subscr_id = @subscr_id)
		   AND (@custom IS NULL OR custom = @custom)
		  ORDER BY CASE @OrderByFieldName
				WHEN 'Txn_id' THEN [txn_id]
			WHEN 'Txn_type' THEN [txn_type]
			WHEN 'Auth_amount' THEN [auth_amount]
			WHEN 'Auth_id' THEN [auth_id]
			WHEN 'Auth_exp' THEN Convert(varchar,[auth_exp],121)
			WHEN 'Auth_status' THEN [auth_status]
			WHEN 'Mc_gross_x' THEN [mc_gross_x]
			WHEN 'Mc_handling_x' THEN [mc_handling_x]
			WHEN 'Num_cart_items' THEN Convert(varchar,[num_cart_items])
			WHEN 'Parent_txn_id' THEN [parent_txn_id]
			WHEN 'Payment_date' THEN Convert(varchar,[payment_date],121)
			WHEN 'Payment_status' THEN [payment_status]
			WHEN 'Payment_type' THEN [payment_type]
			WHEN 'Pending_reason' THEN [pending_reason]
			WHEN 'Reason_code' THEN [reason_code]
			WHEN 'Remaining_settle' THEN [remaining_settle]
			WHEN 'Transaction_entity' THEN [transaction_entity]
			WHEN 'Invoice' THEN [invoice]
			WHEN 'Memo' THEN [memo]
			WHEN 'Tax' THEN [tax]
			WHEN 'Business' THEN [business]
			WHEN 'Item_name' THEN [item_name]
			WHEN 'Item_number' THEN [item_number]
			WHEN 'Quantity' THEN Convert(varchar,[quantity])
			WHEN 'Receiver_email' THEN [receiver_email]
			WHEN 'Receiver_id' THEN [receiver_id]
			WHEN 'Address_city' THEN [address_city]
			WHEN 'Address_country' THEN [address_country]
			WHEN 'Address_country_code' THEN [address_country_code]
			WHEN 'Address_name' THEN [address_name]
			WHEN 'Address_state' THEN [address_state]
			WHEN 'Address_status' THEN [address_status]
			WHEN 'Address_street' THEN [address_street]
			WHEN 'Address_zip' THEN [address_zip]
			WHEN 'First_name' THEN [first_name]
			WHEN 'Last_name' THEN [last_name]
			WHEN 'Payer_id' THEN [payer_id]
			WHEN 'Payer_status' THEN [payer_status]
			WHEN 'Residence_country' THEN [residence_country]
			WHEN 'Exchange_rate' THEN [exchange_rate]
			WHEN 'Mc_fee' THEN [mc_fee]
			WHEN 'Mc_gross' THEN [mc_gross]
			WHEN 'Mc_handlingamount' THEN [mc_handling#]
			WHEN 'Mc_shippingamount' THEN [mc_shipping#]
			WHEN 'Payment_fee' THEN [payment_fee]
			WHEN 'Payment_gross' THEN [payment_gross]
			WHEN 'Settle_amount' THEN [settle_amount]
			WHEN 'Settle_currency' THEN [settle_currency]
			WHEN 'Subscr_date' THEN Convert(varchar,[subscr_date],121)
			WHEN 'Subscr_effective' THEN Convert(varchar,[subscr_effective],121)
			WHEN 'Period1' THEN [period1]
			WHEN 'Period2' THEN [period2]
			WHEN 'Period3' THEN [period3]
			WHEN 'Amount1' THEN [amount1]
			WHEN 'Amount2' THEN [amount2]
			WHEN 'Amount3' THEN [amount3]
			WHEN 'Mc_amount1' THEN [mc_amount1]
			WHEN 'Mc_amount2' THEN [mc_amount2]
			WHEN 'Mc_amount3' THEN [mc_amount3]
			WHEN 'Mc_currency' THEN [mc_currency]
			WHEN 'Recurring' THEN Convert(varchar,[recurring])
			WHEN 'Reattempt' THEN Convert(varchar,[reattempt])
			WHEN 'Retry_at' THEN Convert(varchar,[retry_at],121)
			WHEN 'Recur_times' THEN Convert(varchar,[recur_times])
			WHEN 'Username' THEN [username]
			WHEN 'Password' THEN [password]
			WHEN 'Subscr_id' THEN [subscr_id]
			WHEN 'Custom' THEN [custom]
				ELSE NULL
			END ASC
		ELSE
			SELECT *
			  FROM PayPalTransactionResponses
			 WHERE (@TransactionID IS NULL OR TransactionID = @TransactionID)
		   AND (@txn_id IS NULL OR txn_id = @txn_id)
		   AND (@txn_type IS NULL OR txn_type = @txn_type)
		   AND (@auth_amount IS NULL OR auth_amount = @auth_amount)
		   AND (@auth_id IS NULL OR auth_id = @auth_id)
		   AND ((@auth_exp_Min IS NULL AND @auth_exp_Max IS NULL) OR
			(@auth_exp_Min IS NULL AND @auth_exp_Max IS NOT NULL AND @auth_exp_Max >= auth_exp) OR
			(@auth_exp_Max IS NULL AND @auth_exp_Min IS NOT NULL AND @auth_exp_Min <= auth_exp) OR
			(@auth_exp_Max IS NOT NULL AND @auth_exp_Min IS NOT NULL AND auth_exp BETWEEN @auth_exp_Min AND @auth_exp_Max))
		   AND (@auth_status IS NULL OR auth_status = @auth_status)
		   AND (@mc_gross_x IS NULL OR mc_gross_x = @mc_gross_x)
		   AND (@mc_handling_x IS NULL OR mc_handling_x = @mc_handling_x)
		   AND (@num_cart_items IS NULL OR num_cart_items = @num_cart_items)
		   AND (@parent_txn_id IS NULL OR parent_txn_id = @parent_txn_id)
		   AND ((@payment_date_Min IS NULL AND @payment_date_Max IS NULL) OR
			(@payment_date_Min IS NULL AND @payment_date_Max IS NOT NULL AND @payment_date_Max >= payment_date) OR
			(@payment_date_Max IS NULL AND @payment_date_Min IS NOT NULL AND @payment_date_Min <= payment_date) OR
			(@payment_date_Max IS NOT NULL AND @payment_date_Min IS NOT NULL AND payment_date BETWEEN @payment_date_Min AND @payment_date_Max))
		   AND (@payment_status IS NULL OR payment_status = @payment_status)
		   AND (@payment_type IS NULL OR payment_type = @payment_type)
		   AND (@pending_reason IS NULL OR pending_reason = @pending_reason)
		   AND (@reason_code IS NULL OR reason_code = @reason_code)
		   AND (@remaining_settle IS NULL OR remaining_settle = @remaining_settle)
		   AND (@transaction_entity IS NULL OR transaction_entity = @transaction_entity)
		   AND (@invoice IS NULL OR invoice = @invoice)
		   AND (@memo IS NULL OR memo = @memo)
		   AND (@tax IS NULL OR tax = @tax)
		   AND (@business IS NULL OR business = @business)
		   AND (@item_name IS NULL OR item_name = @item_name)
		   AND (@item_number IS NULL OR item_number = @item_number)
		   AND (@quantity IS NULL OR quantity = @quantity)
		   AND (@receiver_email IS NULL OR receiver_email = @receiver_email)
		   AND (@receiver_id IS NULL OR receiver_id = @receiver_id)
		   AND (@address_city IS NULL OR address_city = @address_city)
		   AND (@address_country IS NULL OR address_country = @address_country)
		   AND (@address_country_code IS NULL OR address_country_code = @address_country_code)
		   AND (@address_name IS NULL OR address_name = @address_name)
		   AND (@address_state IS NULL OR address_state = @address_state)
		   AND (@address_status IS NULL OR address_status = @address_status)
		   AND (@address_street IS NULL OR address_street = @address_street)
		   AND (@address_zip IS NULL OR address_zip = @address_zip)
		   AND (@first_name IS NULL OR first_name = @first_name)
		   AND (@last_name IS NULL OR last_name = @last_name)
		   AND (@payer_id IS NULL OR payer_id = @payer_id)
		   AND (@payer_status IS NULL OR payer_status = @payer_status)
		   AND (@residence_country IS NULL OR residence_country = @residence_country)
		   AND (@exchange_rate IS NULL OR exchange_rate = @exchange_rate)
		   AND (@mc_fee IS NULL OR mc_fee = @mc_fee)
		   AND (@mc_gross IS NULL OR mc_gross = @mc_gross)
		   AND (@mc_handling# IS NULL OR mc_handling# = @mc_handling#)
		   AND (@mc_shipping# IS NULL OR mc_shipping# = @mc_shipping#)
		   AND (@payment_fee IS NULL OR payment_fee = @payment_fee)
		   AND (@payment_gross IS NULL OR payment_gross = @payment_gross)
		   AND (@settle_amount IS NULL OR settle_amount = @settle_amount)
		   AND (@settle_currency IS NULL OR settle_currency = @settle_currency)
		   AND ((@subscr_date_Min IS NULL AND @subscr_date_Max IS NULL) OR
			(@subscr_date_Min IS NULL AND @subscr_date_Max IS NOT NULL AND @subscr_date_Max >= subscr_date) OR
			(@subscr_date_Max IS NULL AND @subscr_date_Min IS NOT NULL AND @subscr_date_Min <= subscr_date) OR
			(@subscr_date_Max IS NOT NULL AND @subscr_date_Min IS NOT NULL AND subscr_date BETWEEN @subscr_date_Min AND @subscr_date_Max))
		   AND ((@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NULL) OR
			(@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NOT NULL AND @subscr_effective_Max >= subscr_effective) OR
			(@subscr_effective_Max IS NULL AND @subscr_effective_Min IS NOT NULL AND @subscr_effective_Min <= subscr_effective) OR
			(@subscr_effective_Max IS NOT NULL AND @subscr_effective_Min IS NOT NULL AND subscr_effective BETWEEN @subscr_effective_Min AND @subscr_effective_Max))
		   AND (@period1 IS NULL OR period1 = @period1)
		   AND (@period2 IS NULL OR period2 = @period2)
		   AND (@period3 IS NULL OR period3 = @period3)
		   AND (@amount1 IS NULL OR amount1 = @amount1)
		   AND (@amount2 IS NULL OR amount2 = @amount2)
		   AND (@amount3 IS NULL OR amount3 = @amount3)
		   AND (@mc_amount1 IS NULL OR mc_amount1 = @mc_amount1)
		   AND (@mc_amount2 IS NULL OR mc_amount2 = @mc_amount2)
		   AND (@mc_amount3 IS NULL OR mc_amount3 = @mc_amount3)
		   AND (@mc_currency IS NULL OR mc_currency = @mc_currency)
		   AND (@recurring IS NULL OR recurring = @recurring)
		   AND (@reattempt IS NULL OR reattempt = @reattempt)
		   AND ((@retry_at_Min IS NULL AND @retry_at_Max IS NULL) OR
			(@retry_at_Min IS NULL AND @retry_at_Max IS NOT NULL AND @retry_at_Max >= retry_at) OR
			(@retry_at_Max IS NULL AND @retry_at_Min IS NOT NULL AND @retry_at_Min <= retry_at) OR
			(@retry_at_Max IS NOT NULL AND @retry_at_Min IS NOT NULL AND retry_at BETWEEN @retry_at_Min AND @retry_at_Max))
		   AND (@recur_times IS NULL OR recur_times = @recur_times)
		   AND (@username IS NULL OR username = @username)
		   AND (@password IS NULL OR password = @password)
		   AND (@subscr_id IS NULL OR subscr_id = @subscr_id)
		   AND (@custom IS NULL OR custom = @custom)
		  ORDER BY CASE @OrderByFieldName
				WHEN 'Txn_id' THEN [txn_id]
			WHEN 'Txn_type' THEN [txn_type]
			WHEN 'Auth_amount' THEN [auth_amount]
			WHEN 'Auth_id' THEN [auth_id]
			WHEN 'Auth_exp' THEN Convert(varchar,[auth_exp],121)
			WHEN 'Auth_status' THEN [auth_status]
			WHEN 'Mc_gross_x' THEN [mc_gross_x]
			WHEN 'Mc_handling_x' THEN [mc_handling_x]
			WHEN 'Num_cart_items' THEN Convert(varchar,[num_cart_items])
			WHEN 'Parent_txn_id' THEN [parent_txn_id]
			WHEN 'Payment_date' THEN Convert(varchar,[payment_date],121)
			WHEN 'Payment_status' THEN [payment_status]
			WHEN 'Payment_type' THEN [payment_type]
			WHEN 'Pending_reason' THEN [pending_reason]
			WHEN 'Reason_code' THEN [reason_code]
			WHEN 'Remaining_settle' THEN [remaining_settle]
			WHEN 'Transaction_entity' THEN [transaction_entity]
			WHEN 'Invoice' THEN [invoice]
			WHEN 'Memo' THEN [memo]
			WHEN 'Tax' THEN [tax]
			WHEN 'Business' THEN [business]
			WHEN 'Item_name' THEN [item_name]
			WHEN 'Item_number' THEN [item_number]
			WHEN 'Quantity' THEN Convert(varchar,[quantity])
			WHEN 'Receiver_email' THEN [receiver_email]
			WHEN 'Receiver_id' THEN [receiver_id]
			WHEN 'Address_city' THEN [address_city]
			WHEN 'Address_country' THEN [address_country]
			WHEN 'Address_country_code' THEN [address_country_code]
			WHEN 'Address_name' THEN [address_name]
			WHEN 'Address_state' THEN [address_state]
			WHEN 'Address_status' THEN [address_status]
			WHEN 'Address_street' THEN [address_street]
			WHEN 'Address_zip' THEN [address_zip]
			WHEN 'First_name' THEN [first_name]
			WHEN 'Last_name' THEN [last_name]
			WHEN 'Payer_id' THEN [payer_id]
			WHEN 'Payer_status' THEN [payer_status]
			WHEN 'Residence_country' THEN [residence_country]
			WHEN 'Exchange_rate' THEN [exchange_rate]
			WHEN 'Mc_fee' THEN [mc_fee]
			WHEN 'Mc_gross' THEN [mc_gross]
			WHEN 'Mc_handlingamount' THEN [mc_handling#]
			WHEN 'Mc_shippingamount' THEN [mc_shipping#]
			WHEN 'Payment_fee' THEN [payment_fee]
			WHEN 'Payment_gross' THEN [payment_gross]
			WHEN 'Settle_amount' THEN [settle_amount]
			WHEN 'Settle_currency' THEN [settle_currency]
			WHEN 'Subscr_date' THEN Convert(varchar,[subscr_date],121)
			WHEN 'Subscr_effective' THEN Convert(varchar,[subscr_effective],121)
			WHEN 'Period1' THEN [period1]
			WHEN 'Period2' THEN [period2]
			WHEN 'Period3' THEN [period3]
			WHEN 'Amount1' THEN [amount1]
			WHEN 'Amount2' THEN [amount2]
			WHEN 'Amount3' THEN [amount3]
			WHEN 'Mc_amount1' THEN [mc_amount1]
			WHEN 'Mc_amount2' THEN [mc_amount2]
			WHEN 'Mc_amount3' THEN [mc_amount3]
			WHEN 'Mc_currency' THEN [mc_currency]
			WHEN 'Recurring' THEN Convert(varchar,[recurring])
			WHEN 'Reattempt' THEN Convert(varchar,[reattempt])
			WHEN 'Retry_at' THEN Convert(varchar,[retry_at],121)
			WHEN 'Recur_times' THEN Convert(varchar,[recur_times])
			WHEN 'Username' THEN [username]
			WHEN 'Password' THEN [password]
			WHEN 'Subscr_id' THEN [subscr_id]
			WHEN 'Custom' THEN [custom]
				ELSE NULL
			END DESC
	END
	ELSE
	BEGIN
		CREATE TABLE #ids (n int unique identity, id uniqueidentifier)
		IF @OrderDirection = 1
			INSERT INTO #ids (id)
			SELECT TransactionID
			  FROM PayPalTransactionResponses
			 WHERE (@TransactionID IS NULL OR TransactionID = @TransactionID)
		   AND (@txn_id IS NULL OR txn_id = @txn_id)
		   AND (@txn_type IS NULL OR txn_type = @txn_type)
		   AND (@auth_amount IS NULL OR auth_amount = @auth_amount)
		   AND (@auth_id IS NULL OR auth_id = @auth_id)
		   AND ((@auth_exp_Min IS NULL AND @auth_exp_Max IS NULL) OR
			(@auth_exp_Min IS NULL AND @auth_exp_Max IS NOT NULL AND @auth_exp_Max >= auth_exp) OR
			(@auth_exp_Max IS NULL AND @auth_exp_Min IS NOT NULL AND @auth_exp_Min <= auth_exp) OR
			(@auth_exp_Max IS NOT NULL AND @auth_exp_Min IS NOT NULL AND auth_exp BETWEEN @auth_exp_Min AND @auth_exp_Max))
		   AND (@auth_status IS NULL OR auth_status = @auth_status)
		   AND (@mc_gross_x IS NULL OR mc_gross_x = @mc_gross_x)
		   AND (@mc_handling_x IS NULL OR mc_handling_x = @mc_handling_x)
		   AND (@num_cart_items IS NULL OR num_cart_items = @num_cart_items)
		   AND (@parent_txn_id IS NULL OR parent_txn_id = @parent_txn_id)
		   AND ((@payment_date_Min IS NULL AND @payment_date_Max IS NULL) OR
			(@payment_date_Min IS NULL AND @payment_date_Max IS NOT NULL AND @payment_date_Max >= payment_date) OR
			(@payment_date_Max IS NULL AND @payment_date_Min IS NOT NULL AND @payment_date_Min <= payment_date) OR
			(@payment_date_Max IS NOT NULL AND @payment_date_Min IS NOT NULL AND payment_date BETWEEN @payment_date_Min AND @payment_date_Max))
		   AND (@payment_status IS NULL OR payment_status = @payment_status)
		   AND (@payment_type IS NULL OR payment_type = @payment_type)
		   AND (@pending_reason IS NULL OR pending_reason = @pending_reason)
		   AND (@reason_code IS NULL OR reason_code = @reason_code)
		   AND (@remaining_settle IS NULL OR remaining_settle = @remaining_settle)
		   AND (@transaction_entity IS NULL OR transaction_entity = @transaction_entity)
		   AND (@invoice IS NULL OR invoice = @invoice)
		   AND (@memo IS NULL OR memo = @memo)
		   AND (@tax IS NULL OR tax = @tax)
		   AND (@business IS NULL OR business = @business)
		   AND (@item_name IS NULL OR item_name = @item_name)
		   AND (@item_number IS NULL OR item_number = @item_number)
		   AND (@quantity IS NULL OR quantity = @quantity)
		   AND (@receiver_email IS NULL OR receiver_email = @receiver_email)
		   AND (@receiver_id IS NULL OR receiver_id = @receiver_id)
		   AND (@address_city IS NULL OR address_city = @address_city)
		   AND (@address_country IS NULL OR address_country = @address_country)
		   AND (@address_country_code IS NULL OR address_country_code = @address_country_code)
		   AND (@address_name IS NULL OR address_name = @address_name)
		   AND (@address_state IS NULL OR address_state = @address_state)
		   AND (@address_status IS NULL OR address_status = @address_status)
		   AND (@address_street IS NULL OR address_street = @address_street)
		   AND (@address_zip IS NULL OR address_zip = @address_zip)
		   AND (@first_name IS NULL OR first_name = @first_name)
		   AND (@last_name IS NULL OR last_name = @last_name)
		   AND (@payer_id IS NULL OR payer_id = @payer_id)
		   AND (@payer_status IS NULL OR payer_status = @payer_status)
		   AND (@residence_country IS NULL OR residence_country = @residence_country)
		   AND (@exchange_rate IS NULL OR exchange_rate = @exchange_rate)
		   AND (@mc_fee IS NULL OR mc_fee = @mc_fee)
		   AND (@mc_gross IS NULL OR mc_gross = @mc_gross)
		   AND (@mc_handling# IS NULL OR mc_handling# = @mc_handling#)
		   AND (@mc_shipping# IS NULL OR mc_shipping# = @mc_shipping#)
		   AND (@payment_fee IS NULL OR payment_fee = @payment_fee)
		   AND (@payment_gross IS NULL OR payment_gross = @payment_gross)
		   AND (@settle_amount IS NULL OR settle_amount = @settle_amount)
		   AND (@settle_currency IS NULL OR settle_currency = @settle_currency)
		   AND ((@subscr_date_Min IS NULL AND @subscr_date_Max IS NULL) OR
			(@subscr_date_Min IS NULL AND @subscr_date_Max IS NOT NULL AND @subscr_date_Max >= subscr_date) OR
			(@subscr_date_Max IS NULL AND @subscr_date_Min IS NOT NULL AND @subscr_date_Min <= subscr_date) OR
			(@subscr_date_Max IS NOT NULL AND @subscr_date_Min IS NOT NULL AND subscr_date BETWEEN @subscr_date_Min AND @subscr_date_Max))
		   AND ((@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NULL) OR
			(@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NOT NULL AND @subscr_effective_Max >= subscr_effective) OR
			(@subscr_effective_Max IS NULL AND @subscr_effective_Min IS NOT NULL AND @subscr_effective_Min <= subscr_effective) OR
			(@subscr_effective_Max IS NOT NULL AND @subscr_effective_Min IS NOT NULL AND subscr_effective BETWEEN @subscr_effective_Min AND @subscr_effective_Max))
		   AND (@period1 IS NULL OR period1 = @period1)
		   AND (@period2 IS NULL OR period2 = @period2)
		   AND (@period3 IS NULL OR period3 = @period3)
		   AND (@amount1 IS NULL OR amount1 = @amount1)
		   AND (@amount2 IS NULL OR amount2 = @amount2)
		   AND (@amount3 IS NULL OR amount3 = @amount3)
		   AND (@mc_amount1 IS NULL OR mc_amount1 = @mc_amount1)
		   AND (@mc_amount2 IS NULL OR mc_amount2 = @mc_amount2)
		   AND (@mc_amount3 IS NULL OR mc_amount3 = @mc_amount3)
		   AND (@mc_currency IS NULL OR mc_currency = @mc_currency)
		   AND (@recurring IS NULL OR recurring = @recurring)
		   AND (@reattempt IS NULL OR reattempt = @reattempt)
		   AND ((@retry_at_Min IS NULL AND @retry_at_Max IS NULL) OR
			(@retry_at_Min IS NULL AND @retry_at_Max IS NOT NULL AND @retry_at_Max >= retry_at) OR
			(@retry_at_Max IS NULL AND @retry_at_Min IS NOT NULL AND @retry_at_Min <= retry_at) OR
			(@retry_at_Max IS NOT NULL AND @retry_at_Min IS NOT NULL AND retry_at BETWEEN @retry_at_Min AND @retry_at_Max))
		   AND (@recur_times IS NULL OR recur_times = @recur_times)
		   AND (@username IS NULL OR username = @username)
		   AND (@password IS NULL OR password = @password)
		   AND (@subscr_id IS NULL OR subscr_id = @subscr_id)
		   AND (@custom IS NULL OR custom = @custom)
		  ORDER BY CASE @OrderByFieldName
				WHEN 'Txn_id' THEN [txn_id]
			WHEN 'Txn_type' THEN [txn_type]
			WHEN 'Auth_amount' THEN [auth_amount]
			WHEN 'Auth_id' THEN [auth_id]
			WHEN 'Auth_exp' THEN Convert(varchar,[auth_exp],121)
			WHEN 'Auth_status' THEN [auth_status]
			WHEN 'Mc_gross_x' THEN [mc_gross_x]
			WHEN 'Mc_handling_x' THEN [mc_handling_x]
			WHEN 'Num_cart_items' THEN Convert(varchar,[num_cart_items])
			WHEN 'Parent_txn_id' THEN [parent_txn_id]
			WHEN 'Payment_date' THEN Convert(varchar,[payment_date],121)
			WHEN 'Payment_status' THEN [payment_status]
			WHEN 'Payment_type' THEN [payment_type]
			WHEN 'Pending_reason' THEN [pending_reason]
			WHEN 'Reason_code' THEN [reason_code]
			WHEN 'Remaining_settle' THEN [remaining_settle]
			WHEN 'Transaction_entity' THEN [transaction_entity]
			WHEN 'Invoice' THEN [invoice]
			WHEN 'Memo' THEN [memo]
			WHEN 'Tax' THEN [tax]
			WHEN 'Business' THEN [business]
			WHEN 'Item_name' THEN [item_name]
			WHEN 'Item_number' THEN [item_number]
			WHEN 'Quantity' THEN Convert(varchar,[quantity])
			WHEN 'Receiver_email' THEN [receiver_email]
			WHEN 'Receiver_id' THEN [receiver_id]
			WHEN 'Address_city' THEN [address_city]
			WHEN 'Address_country' THEN [address_country]
			WHEN 'Address_country_code' THEN [address_country_code]
			WHEN 'Address_name' THEN [address_name]
			WHEN 'Address_state' THEN [address_state]
			WHEN 'Address_status' THEN [address_status]
			WHEN 'Address_street' THEN [address_street]
			WHEN 'Address_zip' THEN [address_zip]
			WHEN 'First_name' THEN [first_name]
			WHEN 'Last_name' THEN [last_name]
			WHEN 'Payer_id' THEN [payer_id]
			WHEN 'Payer_status' THEN [payer_status]
			WHEN 'Residence_country' THEN [residence_country]
			WHEN 'Exchange_rate' THEN [exchange_rate]
			WHEN 'Mc_fee' THEN [mc_fee]
			WHEN 'Mc_gross' THEN [mc_gross]
			WHEN 'Mc_handlingamount' THEN [mc_handling#]
			WHEN 'Mc_shippingamount' THEN [mc_shipping#]
			WHEN 'Payment_fee' THEN [payment_fee]
			WHEN 'Payment_gross' THEN [payment_gross]
			WHEN 'Settle_amount' THEN [settle_amount]
			WHEN 'Settle_currency' THEN [settle_currency]
			WHEN 'Subscr_date' THEN Convert(varchar,[subscr_date],121)
			WHEN 'Subscr_effective' THEN Convert(varchar,[subscr_effective],121)
			WHEN 'Period1' THEN [period1]
			WHEN 'Period2' THEN [period2]
			WHEN 'Period3' THEN [period3]
			WHEN 'Amount1' THEN [amount1]
			WHEN 'Amount2' THEN [amount2]
			WHEN 'Amount3' THEN [amount3]
			WHEN 'Mc_amount1' THEN [mc_amount1]
			WHEN 'Mc_amount2' THEN [mc_amount2]
			WHEN 'Mc_amount3' THEN [mc_amount3]
			WHEN 'Mc_currency' THEN [mc_currency]
			WHEN 'Recurring' THEN Convert(varchar,[recurring])
			WHEN 'Reattempt' THEN Convert(varchar,[reattempt])
			WHEN 'Retry_at' THEN Convert(varchar,[retry_at],121)
			WHEN 'Recur_times' THEN Convert(varchar,[recur_times])
			WHEN 'Username' THEN [username]
			WHEN 'Password' THEN [password]
			WHEN 'Subscr_id' THEN [subscr_id]
			WHEN 'Custom' THEN [custom]
				ELSE NULL
			END ASC
		ELSE
			INSERT INTO #ids (id)
			SELECT TransactionID
			  FROM PayPalTransactionResponses
			 WHERE (@TransactionID IS NULL OR TransactionID = @TransactionID)
		   AND (@txn_id IS NULL OR txn_id = @txn_id)
		   AND (@txn_type IS NULL OR txn_type = @txn_type)
		   AND (@auth_amount IS NULL OR auth_amount = @auth_amount)
		   AND (@auth_id IS NULL OR auth_id = @auth_id)
		   AND ((@auth_exp_Min IS NULL AND @auth_exp_Max IS NULL) OR
			(@auth_exp_Min IS NULL AND @auth_exp_Max IS NOT NULL AND @auth_exp_Max >= auth_exp) OR
			(@auth_exp_Max IS NULL AND @auth_exp_Min IS NOT NULL AND @auth_exp_Min <= auth_exp) OR
			(@auth_exp_Max IS NOT NULL AND @auth_exp_Min IS NOT NULL AND auth_exp BETWEEN @auth_exp_Min AND @auth_exp_Max))
		   AND (@auth_status IS NULL OR auth_status = @auth_status)
		   AND (@mc_gross_x IS NULL OR mc_gross_x = @mc_gross_x)
		   AND (@mc_handling_x IS NULL OR mc_handling_x = @mc_handling_x)
		   AND (@num_cart_items IS NULL OR num_cart_items = @num_cart_items)
		   AND (@parent_txn_id IS NULL OR parent_txn_id = @parent_txn_id)
		   AND ((@payment_date_Min IS NULL AND @payment_date_Max IS NULL) OR
			(@payment_date_Min IS NULL AND @payment_date_Max IS NOT NULL AND @payment_date_Max >= payment_date) OR
			(@payment_date_Max IS NULL AND @payment_date_Min IS NOT NULL AND @payment_date_Min <= payment_date) OR
			(@payment_date_Max IS NOT NULL AND @payment_date_Min IS NOT NULL AND payment_date BETWEEN @payment_date_Min AND @payment_date_Max))
		   AND (@payment_status IS NULL OR payment_status = @payment_status)
		   AND (@payment_type IS NULL OR payment_type = @payment_type)
		   AND (@pending_reason IS NULL OR pending_reason = @pending_reason)
		   AND (@reason_code IS NULL OR reason_code = @reason_code)
		   AND (@remaining_settle IS NULL OR remaining_settle = @remaining_settle)
		   AND (@transaction_entity IS NULL OR transaction_entity = @transaction_entity)
		   AND (@invoice IS NULL OR invoice = @invoice)
		   AND (@memo IS NULL OR memo = @memo)
		   AND (@tax IS NULL OR tax = @tax)
		   AND (@business IS NULL OR business = @business)
		   AND (@item_name IS NULL OR item_name = @item_name)
		   AND (@item_number IS NULL OR item_number = @item_number)
		   AND (@quantity IS NULL OR quantity = @quantity)
		   AND (@receiver_email IS NULL OR receiver_email = @receiver_email)
		   AND (@receiver_id IS NULL OR receiver_id = @receiver_id)
		   AND (@address_city IS NULL OR address_city = @address_city)
		   AND (@address_country IS NULL OR address_country = @address_country)
		   AND (@address_country_code IS NULL OR address_country_code = @address_country_code)
		   AND (@address_name IS NULL OR address_name = @address_name)
		   AND (@address_state IS NULL OR address_state = @address_state)
		   AND (@address_status IS NULL OR address_status = @address_status)
		   AND (@address_street IS NULL OR address_street = @address_street)
		   AND (@address_zip IS NULL OR address_zip = @address_zip)
		   AND (@first_name IS NULL OR first_name = @first_name)
		   AND (@last_name IS NULL OR last_name = @last_name)
		   AND (@payer_id IS NULL OR payer_id = @payer_id)
		   AND (@payer_status IS NULL OR payer_status = @payer_status)
		   AND (@residence_country IS NULL OR residence_country = @residence_country)
		   AND (@exchange_rate IS NULL OR exchange_rate = @exchange_rate)
		   AND (@mc_fee IS NULL OR mc_fee = @mc_fee)
		   AND (@mc_gross IS NULL OR mc_gross = @mc_gross)
		   AND (@mc_handling# IS NULL OR mc_handling# = @mc_handling#)
		   AND (@mc_shipping# IS NULL OR mc_shipping# = @mc_shipping#)
		   AND (@payment_fee IS NULL OR payment_fee = @payment_fee)
		   AND (@payment_gross IS NULL OR payment_gross = @payment_gross)
		   AND (@settle_amount IS NULL OR settle_amount = @settle_amount)
		   AND (@settle_currency IS NULL OR settle_currency = @settle_currency)
		   AND ((@subscr_date_Min IS NULL AND @subscr_date_Max IS NULL) OR
			(@subscr_date_Min IS NULL AND @subscr_date_Max IS NOT NULL AND @subscr_date_Max >= subscr_date) OR
			(@subscr_date_Max IS NULL AND @subscr_date_Min IS NOT NULL AND @subscr_date_Min <= subscr_date) OR
			(@subscr_date_Max IS NOT NULL AND @subscr_date_Min IS NOT NULL AND subscr_date BETWEEN @subscr_date_Min AND @subscr_date_Max))
		   AND ((@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NULL) OR
			(@subscr_effective_Min IS NULL AND @subscr_effective_Max IS NOT NULL AND @subscr_effective_Max >= subscr_effective) OR
			(@subscr_effective_Max IS NULL AND @subscr_effective_Min IS NOT NULL AND @subscr_effective_Min <= subscr_effective) OR
			(@subscr_effective_Max IS NOT NULL AND @subscr_effective_Min IS NOT NULL AND subscr_effective BETWEEN @subscr_effective_Min AND @subscr_effective_Max))
		   AND (@period1 IS NULL OR period1 = @period1)
		   AND (@period2 IS NULL OR period2 = @period2)
		   AND (@period3 IS NULL OR period3 = @period3)
		   AND (@amount1 IS NULL OR amount1 = @amount1)
		   AND (@amount2 IS NULL OR amount2 = @amount2)
		   AND (@amount3 IS NULL OR amount3 = @amount3)
		   AND (@mc_amount1 IS NULL OR mc_amount1 = @mc_amount1)
		   AND (@mc_amount2 IS NULL OR mc_amount2 = @mc_amount2)
		   AND (@mc_amount3 IS NULL OR mc_amount3 = @mc_amount3)
		   AND (@mc_currency IS NULL OR mc_currency = @mc_currency)
		   AND (@recurring IS NULL OR recurring = @recurring)
		   AND (@reattempt IS NULL OR reattempt = @reattempt)
		   AND ((@retry_at_Min IS NULL AND @retry_at_Max IS NULL) OR
			(@retry_at_Min IS NULL AND @retry_at_Max IS NOT NULL AND @retry_at_Max >= retry_at) OR
			(@retry_at_Max IS NULL AND @retry_at_Min IS NOT NULL AND @retry_at_Min <= retry_at) OR
			(@retry_at_Max IS NOT NULL AND @retry_at_Min IS NOT NULL AND retry_at BETWEEN @retry_at_Min AND @retry_at_Max))
		   AND (@recur_times IS NULL OR recur_times = @recur_times)
		   AND (@username IS NULL OR username = @username)
		   AND (@password IS NULL OR password = @password)
		   AND (@subscr_id IS NULL OR subscr_id = @subscr_id)
		   AND (@custom IS NULL OR custom = @custom)
		  ORDER BY CASE @OrderByFieldName
				WHEN 'Txn_id' THEN [txn_id]
			WHEN 'Txn_type' THEN [txn_type]
			WHEN 'Auth_amount' THEN [auth_amount]
			WHEN 'Auth_id' THEN [auth_id]
			WHEN 'Auth_exp' THEN Convert(varchar,[auth_exp],121)
			WHEN 'Auth_status' THEN [auth_status]
			WHEN 'Mc_gross_x' THEN [mc_gross_x]
			WHEN 'Mc_handling_x' THEN [mc_handling_x]
			WHEN 'Num_cart_items' THEN Convert(varchar,[num_cart_items])
			WHEN 'Parent_txn_id' THEN [parent_txn_id]
			WHEN 'Payment_date' THEN Convert(varchar,[payment_date],121)
			WHEN 'Payment_status' THEN [payment_status]
			WHEN 'Payment_type' THEN [payment_type]
			WHEN 'Pending_reason' THEN [pending_reason]
			WHEN 'Reason_code' THEN [reason_code]
			WHEN 'Remaining_settle' THEN [remaining_settle]
			WHEN 'Transaction_entity' THEN [transaction_entity]
			WHEN 'Invoice' THEN [invoice]
			WHEN 'Memo' THEN [memo]
			WHEN 'Tax' THEN [tax]
			WHEN 'Business' THEN [business]
			WHEN 'Item_name' THEN [item_name]
			WHEN 'Item_number' THEN [item_number]
			WHEN 'Quantity' THEN Convert(varchar,[quantity])
			WHEN 'Receiver_email' THEN [receiver_email]
			WHEN 'Receiver_id' THEN [receiver_id]
			WHEN 'Address_city' THEN [address_city]
			WHEN 'Address_country' THEN [address_country]
			WHEN 'Address_country_code' THEN [address_country_code]
			WHEN 'Address_name' THEN [address_name]
			WHEN 'Address_state' THEN [address_state]
			WHEN 'Address_status' THEN [address_status]
			WHEN 'Address_street' THEN [address_street]
			WHEN 'Address_zip' THEN [address_zip]
			WHEN 'First_name' THEN [first_name]
			WHEN 'Last_name' THEN [last_name]
			WHEN 'Payer_id' THEN [payer_id]
			WHEN 'Payer_status' THEN [payer_status]
			WHEN 'Residence_country' THEN [residence_country]
			WHEN 'Exchange_rate' THEN [exchange_rate]
			WHEN 'Mc_fee' THEN [mc_fee]
			WHEN 'Mc_gross' THEN [mc_gross]
			WHEN 'Mc_handlingamount' THEN [mc_handling#]
			WHEN 'Mc_shippingamount' THEN [mc_shipping#]
			WHEN 'Payment_fee' THEN [payment_fee]
			WHEN 'Payment_gross' THEN [payment_gross]
			WHEN 'Settle_amount' THEN [settle_amount]
			WHEN 'Settle_currency' THEN [settle_currency]
			WHEN 'Subscr_date' THEN Convert(varchar,[subscr_date],121)
			WHEN 'Subscr_effective' THEN Convert(varchar,[subscr_effective],121)
			WHEN 'Period1' THEN [period1]
			WHEN 'Period2' THEN [period2]
			WHEN 'Period3' THEN [period3]
			WHEN 'Amount1' THEN [amount1]
			WHEN 'Amount2' THEN [amount2]
			WHEN 'Amount3' THEN [amount3]
			WHEN 'Mc_amount1' THEN [mc_amount1]
			WHEN 'Mc_amount2' THEN [mc_amount2]
			WHEN 'Mc_amount3' THEN [mc_amount3]
			WHEN 'Mc_currency' THEN [mc_currency]
			WHEN 'Recurring' THEN Convert(varchar,[recurring])
			WHEN 'Reattempt' THEN Convert(varchar,[reattempt])
			WHEN 'Retry_at' THEN Convert(varchar,[retry_at],121)
			WHEN 'Recur_times' THEN Convert(varchar,[recur_times])
			WHEN 'Username' THEN [username]
			WHEN 'Password' THEN [password]
			WHEN 'Subscr_id' THEN [subscr_id]
			WHEN 'Custom' THEN [custom]
				ELSE NULL
			END DESC
		
		 SELECT @TotalResults = COUNT(*) FROM #ids
		 DECLARE @firstRecord int, @lastRecord int
		 SET @firstRecord = (@PageNumber-1) * @ResultsPerPage + 1
		 SET @lastRecord = @firstRecord + @ResultsPerPage - 1
		 
		 SELECT r.*
		   FROM #ids i
	  LEFT JOIN PayPalTransactionResponses r
			 ON i.id = r.TransactionID
		  WHERE i.n BETWEEN @firstRecord AND @lastRecord
	END
END

go
