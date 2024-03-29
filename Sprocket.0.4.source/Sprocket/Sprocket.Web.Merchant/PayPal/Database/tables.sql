if not exists (select id from sysobjects where name='PayPalTransactionResponses' and type='U')
create table dbo.PayPalTransactionResponses
(
	TransactionID			uniqueidentifier PRIMARY KEY,
	txn_id					varchar(17) NOT NULL,
	txn_type				varchar(40),
	auth_amount				money,
	auth_id					varchar(40),
	auth_exp				datetime,
	auth_status				varchar(20),
	mc_gross_x				money,
	mc_handling_x			money,
	num_cart_items			int,
	parent_txn_id			varchar(17),
	payment_date			datetime,
	payment_status			varchar(40),
	payment_type			varchar(20),
	pending_reason			varchar(20),
	reason_code				varchar(20),
	remaining_settle		money,
	transaction_entity		varchar(20),
	invoice					varchar(127),
	memo					varchar(255),
	tax						money,
	business				varchar(127),
	item_name				nvarchar(127),
	item_number				varchar(127),
	quantity				int,
	receiver_email			varchar(127),
	receiver_id				varchar(13),
	address_city			nvarchar(40),
	address_country			varchar(64),
	address_country_code	varchar(2),
	address_name			nvarchar(128),
	address_state			nvarchar(40),
	address_status			varchar(20),
	address_street			nvarchar(200),
	address_zip				nvarchar(20),
	first_name				nvarchar(64),
	last_name				nvarchar(64),
	payer_id				varchar(13),
	payer_status			varchar(20),
	residence_country		varchar(2),
	exchange_rate			money,
	mc_fee					money,
	mc_gross				money,
	[mc_handling#]			money,
	[mc_shipping#]			money,
	payment_fee				money,
	payment_gross			money,
	settle_amount			money,
	settle_currency			varchar(10),
	subscr_date				datetime,
	subscr_effective		datetime,
	period1					varchar(20),
	period2					varchar(64),
	period3					varchar(20),
	amount1					decimal,
	amount2					decimal,
	amount3					decimal,
	mc_amount1				decimal,
	mc_amount2				decimal,
	mc_amount3				decimal,
	mc_currency				varchar(10),
	recurring				bit,
	reattempt				bit,
	retry_at				datetime,
	recur_times				int,
	username				nvarchar(64),
	password				nvarchar(127),
	subscr_id				varchar(19),
	custom					nvarchar(255)
)