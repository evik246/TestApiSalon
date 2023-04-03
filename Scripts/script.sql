/*
Создание схем
*/

CREATE SCHEMA IF NOT EXISTS salon_users_schema;

/*
Создание таблиц
*/

DROP TABLE IF EXISTS Appointment;
DROP TABLE IF EXISTS Skill;
DROP TABLE IF EXISTS Schedule;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS Comment;
DROP TABLE IF EXISTS Customer;
DROP TABLE IF EXISTS Salon;
DROP TABLE IF EXISTS City;
DROP TABLE IF EXISTS Service;
DROP TABLE IF EXISTS ServiceCategory;
DROP TABLE IF EXISTS salon_users_schema.Users;

DROP DOMAIN IF EXISTS email;
DROP DOMAIN IF EXISTS phone;

CREATE DOMAIN email as VARCHAR(65)
CHECK (VALUE SIMILAR TO '[a-z0-9._%-]+@[a-z0-9.-]+[a-z]{2,4}');

CREATE DOMAIN phone as CHAR(13) 
CHECK (VALUE SIMILAR TO '\+[0-9]{12}');

CREATE TABLE ServiceCategory (
    id SERIAL NOT NULL PRIMARY KEY,
    name VARCHAR(50) NOT NULL
);

CREATE TABLE Service (
    id SERIAL NOT NULL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    category_id INTEGER NOT NULL,
    price NUMERIC(6, 2) NOT NULL,
    execution_time INTERVAL NOT NULL CHECK (execution_time < '24 hour' AND execution_time > '0 hour'),
    FOREIGN KEY (category_id) references ServiceCategory (id) on delete restrict on update cascade
);

CREATE TABLE City (
    id SERIAL NOT NULL PRIMARY KEY,
    name VARCHAR(35) NOT NULL
);

CREATE TABLE Salon (
    id SERIAL NOT NULL PRIMARY KEY,
    address VARCHAR(100) NOT NULL,
    city_id INTEGER NOT NULL,
    phone phone UNIQUE NOT NULL,
    FOREIGN KEY (city_id) references City (id) on delete restrict on update cascade
);

CREATE TABLE Customer (
    id SERIAL NOT NULL PRIMARY KEY,
    name VARCHAR(40) NOT NULL,
    birthday DATE NULL CHECK (birthday >= '01.01.1950' AND birthday < CURRENT_DATE),
    email email UNIQUE NOT NULL,
    phone phone UNIQUE NOT NULL
);

CREATE TABLE Comment (
    id SERIAL NOT NULL PRIMARY KEY,
    salon_id INTEGER NOT NULL,
    customer_id INTEGER NOT NULL,
    published_date DATE NOT NULL DEFAULT CURRENT_DATE,
    review VARCHAR(250) NULL,
    rating SMALLINT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    FOREIGN KEY (salon_id) references Salon (id) on delete cascade on update cascade,
    FOREIGN KEY (customer_id) references Customer (id) on delete cascade on update cascade
);

CREATE TABLE Employee (
    id SERIAL NOT NULL PRIMARY KEY,
    name VARCHAR(40) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    salon_id INTEGER NULL,
    email email UNIQUE NOT NULL,
    role VARCHAR(15) NOT NULL CHECK (role in('Master', 'Manager', 'Admin')),
    photo_path VARCHAR(260) NULL,
    specialization VARCHAR(100) NULL,
    FOREIGN KEY (salon_id) references Salon (id) on delete restrict on update cascade
);

CREATE TABLE Schedule (
    id SERIAL NOT NULL PRIMARY KEY,
    weekday VARCHAR(15) NOT NULL CHECK (weekday in('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday')),
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    employee_id INTEGER NOT NULL,
    CONSTRAINT check_time CHECK (end_time - start_time <= '8 hour'),
    FOREIGN KEY (employee_id) references Employee (id) on delete restrict on update cascade
);

CREATE TABLE Skill (
    employee_id INTEGER NOT NULL,
    service_id INTEGER NOT NULL,
    PRIMARY KEY(employee_id, service_id),
    FOREIGN KEY (employee_id) references Employee (id) on delete restrict on update cascade,
    FOREIGN KEY (service_id) references Service (id) on delete restrict on update cascade
);

CREATE TABLE Appointment (
    id SERIAL NOT NULL PRIMARY KEY,
    date TIMESTAMP NOT NULL CHECK (date > CURRENT_TIMESTAMP),
    customer_id INTEGER NOT NULL,
    service_id INTEGER NOT NULL,
    employee_id INTEGER NOT NULL,
    status VARCHAR(15) NOT NULL CHECK (status in('Canceled', 'Active', 'Completed')) DEFAULT 'Active',
    FOREIGN KEY (customer_id) references Customer (id) on delete restrict on update cascade,
    FOREIGN KEY (employee_id) references Employee (id) on delete restrict on update cascade,
    FOREIGN KEY (service_id) references Service (id) on delete restrict on update cascade
);

CREATE TABLE salon_users_schema.Users (
	email email NOT NULL PRIMARY KEY,
	password varchar(255) NOT NULL,
	role varchar (50) NOT NULL CHECK (role in ('Admin', 'Manager', 'Master', 'Client'))
);

/*
Процедуры и триггеры
*/

CREATE OR REPLACE FUNCTION sha1(bytea)
RETURNS TEXT AS $$
BEGIN
	SELECT encode(public.digest($1, 'sha1'), 'hex');
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION hash_password()
RETURNS TRIGGER
AS $$
BEGIN
	IF (TG_OP = 'INSERT' OR NEW.password NOT LIKE OLD.password) THEN
		NEW.password = encode (public.digest(NEW.password, 'sha256'), 'hex');
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER make_password_hash
BEFORE INSERT OR UPDATE ON salon_users_schema.Users
FOR EACH ROW
EXECUTE PROCEDURE hash_password();

/*
Создание ролей и привилегий
*/

REVOKE CONNECT ON DATABASE beautysalon FROM guest, connect_user;

REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA public FROM guest;
REVOKE USAGE ON SCHEMA public FROM guest;

REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA salon_users_schema FROM connect_user;
REVOKE USAGE ON SCHEMA salon_users_schema FROM connect_user;

DROP ROLE IF EXISTS guest;
DROP ROLE IF EXISTS connect_user;

--
CREATE ROLE guest WITH LOGIN PASSWORD 'guestpassword';
CREATE ROLE connect_user WITH LOGIN PASSWORD 'connectpassword';

GRANT CONNECT ON DATABASE beautysalon TO guest, connect_user;

GRANT USAGE ON SCHEMA public to guest;
GRANT USAGE ON SCHEMA salon_users_schema to connect_user;

GRANT SELECT ON Service, ServiceCategory, Salon, City, Comment
TO guest;

GRANT SELECT ON salon_users_schema.Users
to connect_user;
