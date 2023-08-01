--TABLES

create database JobManagmentSystemAuthService;


create table users
(
    user_id serial primary key,
    first_name varchar(100) not null,
    last_name varchar(100) not null,
    email varchar(100) not null unique,
    password text not null,
    is_employer bool not null default false,
    company_name varchar(100) default null
);
create table degrees
(
	degree_name varchar(100) primary key
);

create table education
(
	education_name varchar(100) primary key
);

create table profession
(
	proffesion_name varchar(100) primary key
);

create table skills
(
	skill_name varchar(100) primary key
);
--Helper QUERIES

select * from users;
select * from degrees;
select * from education;
select * from profession;
select * from skills;


update users set company_name = null where user_id = 1;