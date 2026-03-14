CREATE SCHEMA IF NOT EXISTS public;
SET search_path TO public;

CREATE TABLE cities (
    city_id SERIAL PRIMARY KEY,
    city_name VARCHAR(24) NOT NULL,
    country VARCHAR(26) NOT NULL,
    airport VARCHAR(26),
    language VARCHAR(16),
    country_iso_code CHAR(2)
);

CREATE TABLE flights (
    flight_id VARCHAR(6) NOT NULL,
    segment_number INTEGER NOT NULL,
    orig_airport CHAR(3),
    depart_time TIME,
    dest_airport CHAR(3),
    arrive_time TIME,
    meal CHAR(1),
    flying_time DOUBLE PRECISION,
    miles INTEGER,
    aircraft VARCHAR(6),
    CONSTRAINT flights_pk PRIMARY KEY (flight_id, segment_number),
    CONSTRAINT meal_constraint CHECK (meal IN ('B','L','D','S'))
);

CREATE TABLE flightavailability (
    flight_id VARCHAR(6) NOT NULL,
    segment_number INTEGER NOT NULL,
    flight_date DATE NOT NULL,
    economy_seats_taken INTEGER DEFAULT 0,
    business_seats_taken INTEGER DEFAULT 0,
    firstclass_seats_taken INTEGER DEFAULT 0,
    CONSTRAINT flightavail_pk PRIMARY KEY (flight_id, segment_number, flight_date),
    CONSTRAINT flights_fk2 FOREIGN KEY (flight_id, segment_number)
        REFERENCES flights(flight_id, segment_number)
);

CREATE TABLE flighthistory (
    id SERIAL PRIMARY KEY,
    username VARCHAR(26) NOT NULL,
    flight_id VARCHAR(6) NOT NULL,
    orig_airport CHAR(3) NOT NULL,
    dest_airport CHAR(3) NOT NULL,
    begin_date VARCHAR(12),
    class VARCHAR(12)
);

ALTER SEQUENCE flighthistory_id_seq RESTART WITH 6;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username text NOT NULL UNIQUE,
    password text
);

ALTER SEQUENCE users_id_seq RESTART WITH 9;