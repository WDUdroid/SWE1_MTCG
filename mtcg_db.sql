--
-- PostgreSQL database dump
--

-- Dumped from database version 13.1
-- Dumped by pg_dump version 13.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: cards; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cards (
    name character varying(100) NOT NULL,
    type character varying(100) NOT NULL,
    element character varying(100) NOT NULL
);


ALTER TABLE public.cards OWNER TO postgres;

--
-- Name: credentials; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.credentials (
    id bigint NOT NULL,
    username character varying(50) NOT NULL,
    password character varying(128) NOT NULL,
    token character varying(128) NOT NULL
);


ALTER TABLE public.credentials OWNER TO postgres;

--
-- Name: credentials_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.credentials_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.credentials_id_seq OWNER TO postgres;

--
-- Name: credentials_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.credentials_id_seq OWNED BY public.credentials.id;


--
-- Name: deck; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.deck (
    personid integer NOT NULL,
    cardid character varying(100) NOT NULL
);


ALTER TABLE public.deck OWNER TO postgres;

--
-- Name: stack; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.stack (
    personid integer NOT NULL,
    cardid character varying(100) NOT NULL,
    name character varying(100) NOT NULL,
    damage integer NOT NULL
);


ALTER TABLE public.stack OWNER TO postgres;

--
-- Name: t_coins; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.t_coins (
    personid integer NOT NULL,
    coins integer NOT NULL
);


ALTER TABLE public.t_coins OWNER TO postgres;

--
-- Name: t_elo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.t_elo (
    personid integer NOT NULL,
    elo integer NOT NULL
);


ALTER TABLE public.t_elo OWNER TO postgres;

--
-- Name: t_package; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.t_package (
    packageid integer NOT NULL,
    cardid character varying(100) NOT NULL,
    name character varying(100) NOT NULL,
    damage integer NOT NULL
);


ALTER TABLE public.t_package OWNER TO postgres;

--
-- Name: t_page; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.t_page (
    personid integer NOT NULL,
    name character varying(50) NOT NULL,
    bio character varying(500) NOT NULL,
    image character varying(500) NOT NULL
);


ALTER TABLE public.t_page OWNER TO postgres;

--
-- Name: t_trade; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.t_trade (
    traderid integer NOT NULL,
    cardtoid character varying(100) NOT NULL,
    wantedtyp character varying(100) NOT NULL,
    minimumdamage integer NOT NULL,
    dealid character varying(100) NOT NULL
);


ALTER TABLE public.t_trade OWNER TO postgres;

--
-- Name: credentials id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.credentials ALTER COLUMN id SET DEFAULT nextval('public.credentials_id_seq'::regclass);


--
-- Data for Name: cards; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cards (name, type, element) FROM stdin;
Goblin	Monster	Normal
WaterGoblin	Monster	Water
Dragon	Monster	Fire
Ork	Monster	Normal
FireTroll	Monster	Fire
FireSpell	Spell	Fire
WaterSpell	Spell	Water
RegularSpell	Spell	Normal
Kraken	Monster	Water
Knight	Monster	Normal
Wizzard	Monster	Normal
FireElf	Monster	Normal
\.


--
-- Data for Name: credentials; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.credentials (id, username, password, token) FROM stdin;
\.


--
-- Data for Name: deck; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.deck (personid, cardid) FROM stdin;
\.


--
-- Data for Name: stack; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.stack (personid, cardid, name, damage) FROM stdin;
\.


--
-- Data for Name: t_coins; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.t_coins (personid, coins) FROM stdin;
\.


--
-- Data for Name: t_elo; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.t_elo (personid, elo) FROM stdin;
\.


--
-- Data for Name: t_package; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.t_package (packageid, cardid, name, damage) FROM stdin;
\.


--
-- Data for Name: t_page; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.t_page (personid, name, bio, image) FROM stdin;
\.


--
-- Data for Name: t_trade; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.t_trade (traderid, cardtoid, wantedtyp, minimumdamage, dealid) FROM stdin;
\.


--
-- Name: credentials_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.credentials_id_seq', 56, true);


--
-- Name: credentials credentials_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.credentials
    ADD CONSTRAINT credentials_pkey PRIMARY KEY (id);


--
-- PostgreSQL database dump complete
--

