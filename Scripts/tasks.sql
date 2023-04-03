/*
Задачи клиента:
К1 - Регистрироваться
Входные данные: Имя, почта, пароль, номер телефона, день рождения
Выходные данные: Созданный клиент
*/

select * from customer;
select * from salon_users_schema.users;

CREATE OR REPLACE PROCEDURE register_customer(
	customer_name VARCHAR, 
	customer_email email, 
	user_password VARCHAR, 
	customer_phone phone, 
	customer_birthday DATE DEFAULT NULL
)
AS $$
BEGIN
	INSERT INTO customer (name, email, phone, birthday)
	VALUES (customer_name, customer_email, customer_phone, customer_birthday);
	
	INSERT INTO salon_users_schema.users (email, password, role)
	values (customer_email, user_password, 'Client');
END;
$$ LANGUAGE plpgsql;

CALL register_customer('Алла', 'test@gmail.com', '+380668940506', '1234');