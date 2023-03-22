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
    password CHAR(120) NOT NULL,
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
    password CHAR(120) NOT NULL,
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

INSERT INTO ServiceCategory (name) VALUES 
    ('Стрижки, укладки'),
    ('Окрашивание'),
    ('Уход за ногтями'),
    ('Визаж, дизайн бровей');
    
INSERT INTO Service (name, category_id, price, execution_time) VALUES
    ('Стрижка мужская', 1, 200, '30 min'),
    ('Стрижка женская', 1, 350, '1 hour 20 min'),
    ('Обычный маникюр', 3, 250, '1 hour 30 min'),
    ('Педикюр', 3, 210, '2 hour');
    
INSERT INTO City (name) VALUES 
    ('Одесса'),
    ('Кривой Рог');

INSERT INTO Salon (address, city_id, phone) VALUES 
    ('ул. Дерибасовская, 12', 1, '+380668986542'),
    ('ул. Водопроводная, 56', 1, '+380678976742'),
    ('ул. Широкая, 24', 2, '+380668986376');
    
INSERT INTO Customer (name, birthday, email, password, phone) VALUES
    ('Елена', '16.01.2002', 'test@test.com', '1234', '+380558940506'),
    ('Саша', '24.10.1995', 'abcd_d@test.com', '1234', '+380558940507'),
    ('Ольга', NULL, 'test2@test.com', '1234', '+380558940508'),
    ('Николай', NULL, 'nic@test.com', '1234', '+380558940523'),
    ('Мария', NULL, 'marisha@test.com', '1234', '+380558940575');
    
INSERT INTO Employee (name, last_name, email, password, role, salon_id, specialization) VALUES
    ('Максим', 'Соколов', 'maxxx@gmail.com', 'max1234', 'Admin', NULL, NULL),
    ('Александра', 'Кузнецова', 'test3@gmail.com', '1111111', 'Manager', 1, NULL),
    ('Георгий', 'Яковлев', 'ya1995@gmail.com', '1234', 'Manager', 2, NULL),
    ('Евгений', 'Йомих', 'evgeniy@gmail.com', 'evgeniy', 'Manager', 3, NULL),
    ('Мария', 'Грушевская', 'abcd111@gmail.com', 'parol', 'Master', 1, 'Парикмахер'),
    ('Валерия', 'Руденко', 'rudenko@gmail.com', 'jjhgjkghdgl', 'Master', 1, 'Маникюрщик'),
    ('Екатерина', 'Демчук', 'ekaterina@gmail.com', 'пароль1234', 'Master', 2, 'Маникюрщик, парикмахер'),
    ('Анна', 'Корч', 'anna287@gmail.com', '1111', 'Master', 2, 'Парикмахер');
    
INSERT INTO Skill (employee_id, service_id) VALUES
    (3, 1),
    (3, 2),
    (4, 3),
    (4, 4),
    (5, 3),
    (5, 4),
    (5, 2),
    (6, 2);

INSERT INTO Schedule (weekday, start_time, end_time, employee_id) VALUES
    ('Monday', '10:00', '17:00', 3),
    ('Tuesday', '12:00', '17:00', 3),
    ('Thursday', '10:00', '17:00', 4),
    ('Friday', '10:00', '15:00', 4);
    
INSERT INTO Appointment (date, customer_id, service_id, employee_id) VALUES
    ('20.11.2024 15:00', 1, 3, 4),
    ('27.10.2024 13:00', 2, 2, 3),
    ('12.11.2024 16:00', 3, 4, 4),
    ('21.11.2024 11:00', 3, 4, 4),
    ('18.11.2024 12:00', 4, 1, 3),
    ('18.11.2024 16:00', 5, 2, 3);
    
INSERT INTO Appointment (date, customer_id, service_id, employee_id, status) VALUES
    ('27.10.2024 12:00', 5, 4, 4, 'Completed'),
    ('22.10.2024 13:30', 5, 3, 4, 'Completed'),
    ('22.10.2024 15:30', 2, 3, 4, 'Completed'),
    ('28.10.2024 14:00', 2, 3, 4, 'Completed'),
    ('23.10.2024 10:30', 2, 2, 3, 'Completed'),
    ('24.10.2024 11:30', 2, 2, 3, 'Completed'),
    ('23.10.2024 15:30', 5, 2, 3, 'Completed'),
    ('23.10.2024 16:00', 1, 2, 3, 'Completed'),
    ('24.11.2024 16:00', 5, 3, 4, 'Canceled'),
    ('30.11.2024 14:00', 1, 4, 4, 'Canceled'),
    ('16.11.2024 11:00', 4, 1, 3, 'Canceled'),
    ('12.11.2024 16:00', 4, 3, 6, 'Canceled'),
    ('13.11.2024 12:00', 5, 4, 5, 'Canceled'),
    ('14.11.2024 13:00', 5, 4, 5, 'Canceled');
    
INSERT INTO Comment (salon_id, customer_id, review, rating) VALUES
    (1, 2, NULL, 4),
    (1, 1, 'Классный салон!!! Очень понравился. Всем советую!', 5);