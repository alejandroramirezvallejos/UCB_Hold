--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4
-- Dumped by pg_dump version 17.4

-- Started on 2026-06-05 22:38:43

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 6 (class 2615 OID 22789)
-- Name: hangfire; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA hangfire;


ALTER SCHEMA hangfire OWNER TO postgres;

--
-- TOC entry 7 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO postgres;

--
-- TOC entry 5398 (class 0 OID 0)
-- Dependencies: 7
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS '';


--
-- TOC entry 2 (class 3079 OID 22790)
-- Name: pgcrypto; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;


--
-- TOC entry 5400 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION pgcrypto; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION pgcrypto IS 'cryptographic functions';


--
-- TOC entry 1010 (class 1247 OID 22828)
-- Name: estado_disponibilidad; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.estado_disponibilidad AS ENUM (
    'disponible',
    'mantenimiento',
    'ocupado'
);


ALTER TYPE public.estado_disponibilidad OWNER TO postgres;

--
-- TOC entry 1013 (class 1247 OID 22836)
-- Name: estado_equipo; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.estado_equipo AS ENUM (
    'operativo',
    'parcialmente_operativo',
    'inoperativo'
);


ALTER TYPE public.estado_equipo OWNER TO postgres;

--
-- TOC entry 1016 (class 1247 OID 22844)
-- Name: estado_prestamo; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.estado_prestamo AS ENUM (
    'pendiente',
    'rechazado',
    'aprobado',
    'activo',
    'finalizado',
    'cancelado',
    'atrasado'
);


ALTER TYPE public.estado_prestamo OWNER TO postgres;

--
-- TOC entry 1019 (class 1247 OID 22860)
-- Name: tipo_mantenimiento; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.tipo_mantenimiento AS ENUM (
    'correctivo',
    'preventivo'
);


ALTER TYPE public.tipo_mantenimiento OWNER TO postgres;

--
-- TOC entry 1022 (class 1247 OID 22866)
-- Name: tipo_usuario; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.tipo_usuario AS ENUM (
    'docente',
    'administrador',
    'estudiante'
);


ALTER TYPE public.tipo_usuario OWNER TO postgres;

--
-- TOC entry 285 (class 1255 OID 22873)
-- Name: actualizar_accesorio(integer, character varying, character varying, character varying, integer, text, double precision, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_accesorio(IN p_id_accesorio_actualizar integer, IN p_nombre_nuevo character varying DEFAULT NULL::character varying, IN p_modelo_nuevo character varying DEFAULT NULL::character varying, IN p_tipo_nuevo character varying DEFAULT NULL::character varying, IN p_codigo_imt_nuevo integer DEFAULT NULL::integer, IN p_descripcion_nueva text DEFAULT NULL::text, IN p_precio_nuevo double precision DEFAULT NULL::double precision, IN p_url_data_sheet_nueva text DEFAULT NULL::text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_equipo_para_actualizar INTEGER;
    v_accesorio_existe BOOLEAN;
BEGIN
    SELECT EXISTS (
        SELECT 1
        FROM public.accesorios
        WHERE id_accesorio = p_id_accesorio_actualizar AND estado_eliminado = FALSE
    ) INTO v_accesorio_existe;

    IF NOT v_accesorio_existe THEN
        RAISE EXCEPTION 'No se encontró un accesorio activo con ID = % para actualizar.', p_id_accesorio_actualizar;
    END IF;

    IF p_codigo_imt_nuevo IS NOT NULL THEN
        SELECT e.id_equipo
          INTO v_id_equipo_para_actualizar
          FROM public.equipos AS e
         WHERE e.codigo_imt = p_codigo_imt_nuevo
           AND e.estado_eliminado = FALSE;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'No se encontró un equipo activo con código IMT = % para asociar al accesorio.', p_codigo_imt_nuevo;
        END IF;
    END IF;

    UPDATE public.accesorios
       SET
           nombre         = COALESCE(p_nombre_nuevo, nombre),
           modelo         = COALESCE(p_modelo_nuevo, modelo),
           tipo           = COALESCE(p_tipo_nuevo, tipo),
           id_equipo      = COALESCE(v_id_equipo_para_actualizar, id_equipo),
           descripcion    = COALESCE(p_descripcion_nueva, descripcion),
           precio         = COALESCE(p_precio_nuevo, precio),
           url_data_sheet = COALESCE(p_url_data_sheet_nueva, url_data_sheet)
     WHERE id_accesorio = p_id_accesorio_actualizar; 
	 
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error de violación de unicidad al actualizar el accesorio. Verifique que los nuevos datos no dupliquen información existente: %', SQLERRM;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar el accesorio: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.actualizar_accesorio(IN p_id_accesorio_actualizar integer, IN p_nombre_nuevo character varying, IN p_modelo_nuevo character varying, IN p_tipo_nuevo character varying, IN p_codigo_imt_nuevo integer, IN p_descripcion_nueva text, IN p_precio_nuevo double precision, IN p_url_data_sheet_nueva text) OWNER TO postgres;

--
-- TOC entry 392 (class 1255 OID 22874)
-- Name: actualizar_cantidad_grupos_equipos(); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_cantidad_grupos_equipos()
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_grupo_id integer;
    v_cantidad_actual integer;
    v_cantidad_esperada integer;
    v_total_actualizado integer := 0;
BEGIN
    -- Iterar sobre todos los grupos de equipos
    FOR v_grupo_id IN SELECT id_grupo_equipo FROM public.grupos_equipos ORDER BY id_grupo_equipo LOOP
        
        -- Contar equipos activos (no eliminados) en este grupo
        SELECT COUNT(*)
        INTO v_cantidad_esperada
        FROM public.equipos
        WHERE id_grupo_equipo = v_grupo_id
          AND estado_eliminado = FALSE;
        
        -- Obtener la cantidad actual registrada
        SELECT cantidad
        INTO v_cantidad_actual
        FROM public.grupos_equipos
        WHERE id_grupo_equipo = v_grupo_id;
        
        -- Si no coinciden, actualizar
        IF v_cantidad_actual != v_cantidad_esperada THEN
            UPDATE public.grupos_equipos
            SET cantidad = v_cantidad_esperada
            WHERE id_grupo_equipo = v_grupo_id;
            
            v_total_actualizado := v_total_actualizado + 1;
            
            RAISE NOTICE 'Grupo % actualizado: % -> % equipos', 
                v_grupo_id, v_cantidad_actual, v_cantidad_esperada;
        END IF;
    END LOOP;
    
    RAISE NOTICE 'Total de grupos actualizados: %', v_total_actualizado;
END;
$$;


ALTER PROCEDURE public.actualizar_cantidad_grupos_equipos() OWNER TO postgres;

--
-- TOC entry 305 (class 1255 OID 22875)
-- Name: actualizar_carrera(integer, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_carrera(IN p_id_carrera_actualizar integer, IN p_nombre_nuevo character varying DEFAULT NULL::character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_carrera_existe BOOLEAN;
    v_nombre_actual character varying;
    v_id_existente integer;
BEGIN
    SELECT EXISTS (
        SELECT 1
        FROM public.carreras
        WHERE id_carrera = p_id_carrera_actualizar AND estado_eliminado = FALSE
    ) INTO v_carrera_existe;

    IF NOT v_carrera_existe THEN
        RAISE EXCEPTION 'No se encontró una carrera activa con ID = % para actualizar.', p_id_carrera_actualizar;
    END IF;

    IF p_nombre_nuevo IS NOT NULL THEN
        IF trim(p_nombre_nuevo) = '' THEN
            RAISE EXCEPTION 'El nuevo nombre de la carrera no puede estar vacío.';
        END IF;

        -- Verificar si ya existe una carrera activa con ese nombre (diferente al ID actual)
        IF EXISTS (
            SELECT 1
            FROM public.carreras
            WHERE nombre = p_nombre_nuevo
              AND estado_eliminado = FALSE
              AND id_carrera <> p_id_carrera_actualizar
        ) THEN
            RAISE EXCEPTION 'Ya existe otra carrera con el nombre "%".', p_nombre_nuevo;
        END IF;

        -- Si existe una carrera eliminada con el mismo nombre, reactivarla y eliminar lógicamente la actual
        SELECT id_carrera INTO v_id_existente
        FROM public.carreras
        WHERE nombre = p_nombre_nuevo
          AND estado_eliminado = TRUE
        LIMIT 1;

        IF v_id_existente IS NOT NULL THEN
            -- Reactivar la carrera eliminada
            UPDATE public.carreras
            SET estado_eliminado = FALSE
            WHERE id_carrera = v_id_existente;

            -- Eliminar lógicamente la carrera actual
            UPDATE public.carreras
            SET estado_eliminado = TRUE
            WHERE id_carrera = p_id_carrera_actualizar;

            RETURN;
        END IF;
    END IF;

    -- Actualización normal si no hay conflictos de nombre
    UPDATE public.carreras
    SET nombre = COALESCE(p_nombre_nuevo, nombre)
    WHERE id_carrera = p_id_carrera_actualizar
      AND estado_eliminado = FALSE;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe otra carrera con el nombre "%".', COALESCE(p_nombre_nuevo, (SELECT nombre FROM public.carreras WHERE id_carrera = p_id_carrera_actualizar));
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar la carrera: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.actualizar_carrera(IN p_id_carrera_actualizar integer, IN p_nombre_nuevo character varying) OWNER TO postgres;

--
-- TOC entry 363 (class 1255 OID 22876)
-- Name: actualizar_categoria(integer, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_categoria(IN p_id_categoria_actualizar integer, IN p_nombre_nuevo_raw character varying DEFAULT NULL::character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_categoria_existe   BOOLEAN;
    v_nombre_nuevo_procesado TEXT;
    v_id_existente       integer;
BEGIN
    -- 1) Verificar que la categoría exista y esté activa
    SELECT EXISTS (
        SELECT 1
        FROM public.categorias
        WHERE id_categoria = p_id_categoria_actualizar
          AND estado_eliminado = FALSE
    ) INTO v_categoria_existe;

    IF NOT v_categoria_existe THEN
        RAISE EXCEPTION 'No se encontró una categoría activa con ID = % para actualizar.', p_id_categoria_actualizar;
    END IF;

    -- 2) Procesar nuevo nombre si se proporcionó
    IF p_nombre_nuevo_raw IS NOT NULL THEN
        v_nombre_nuevo_procesado := TRIM(both ' ' FROM p_nombre_nuevo_raw);

        IF v_nombre_nuevo_procesado = '' THEN
            RAISE EXCEPTION 'El nuevo nombre de la categoría no puede estar vacío.';
        END IF;

        -- 3) Verificar conflicto con otra categoría activa distinta
        IF EXISTS (
            SELECT 1
            FROM public.categorias
            WHERE nombre = v_nombre_nuevo_procesado
              AND estado_eliminado = FALSE
              AND id_categoria <> p_id_categoria_actualizar
        ) THEN
            RAISE EXCEPTION 'Ya existe otra categoría con el nombre "%".', v_nombre_nuevo_procesado;
        END IF;

        -- 4) Si hay una categoría eliminada con el mismo nombre, reactivar esa
        SELECT id_categoria
        INTO   v_id_existente
        FROM   public.categorias
        WHERE  nombre = v_nombre_nuevo_procesado
          AND  estado_eliminado = TRUE
        LIMIT 1;

        IF v_id_existente IS NOT NULL THEN
            -- Reactivar la existente
            UPDATE public.categorias
               SET estado_eliminado = FALSE
             WHERE id_categoria = v_id_existente;

            -- Eliminar lógicamente la actual
            UPDATE public.categorias
               SET estado_eliminado = TRUE
             WHERE id_categoria = p_id_categoria_actualizar;

            RETURN;
        END IF;
    ELSE
        v_nombre_nuevo_procesado := NULL;
    END IF;

    -- 5) Actualización normal si no hubo reactivación ni conflicto
    UPDATE public.categorias
       SET nombre = COALESCE(v_nombre_nuevo_procesado, nombre)
     WHERE id_categoria = p_id_categoria_actualizar
       AND estado_eliminado = FALSE;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe otra categoría con el nombre "%".',
            COALESCE(
              v_nombre_nuevo_procesado,
              (SELECT nombre FROM public.categorias WHERE id_categoria = p_id_categoria_actualizar)
            );
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar la categoría: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.actualizar_categoria(IN p_id_categoria_actualizar integer, IN p_nombre_nuevo_raw character varying) OWNER TO postgres;

--
-- TOC entry 304 (class 1255 OID 22877)
-- Name: actualizar_componente(integer, character varying, character varying, character varying, integer, text, double precision, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_componente(IN p_id_componente_actualizar integer, IN p_nombre_nuevo character varying DEFAULT NULL::character varying, IN p_modelo_nuevo character varying DEFAULT NULL::character varying, IN p_tipo_nuevo character varying DEFAULT NULL::character varying, IN p_codigo_imt_nuevo integer DEFAULT NULL::integer, IN p_descripcion_nueva text DEFAULT NULL::text, IN p_precio_referencia_nuevo double precision DEFAULT NULL::double precision, IN p_url_data_sheet_nueva text DEFAULT NULL::text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_equipo_para_actualizar INTEGER; 
    v_componente_existe BOOLEAN;
BEGIN
    SELECT EXISTS (
        SELECT 1
        FROM public.componentes
        WHERE id_componente = p_id_componente_actualizar AND estado_eliminado = FALSE
    ) INTO v_componente_existe;

    IF NOT v_componente_existe THEN
        RAISE EXCEPTION 'No se encontró un componente activo con ID = % para actualizar.', p_id_componente_actualizar;
    END IF;

     IF p_codigo_imt_nuevo IS NOT NULL THEN
        SELECT e.id_equipo
          INTO v_id_equipo_para_actualizar
          FROM public.equipos AS e
         WHERE e.codigo_imt = p_codigo_imt_nuevo
           AND e.estado_eliminado = FALSE;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'No se encontró un equipo activo con código IMT = % para asociar al componente.', p_codigo_imt_nuevo;
        END IF;
    END IF; 
   
    UPDATE public.componentes
       SET
           nombre            = COALESCE(p_nombre_nuevo, nombre),
           modelo            = COALESCE(p_modelo_nuevo, modelo),
           tipo              = COALESCE(p_tipo_nuevo, tipo),
           id_equipo         = COALESCE(v_id_equipo_para_actualizar, id_equipo),
           descripcion       = COALESCE(p_descripcion_nueva, descripcion),
           precio_referencia = COALESCE(p_precio_referencia_nuevo, precio_referencia),
           url_data_sheet    = COALESCE(p_url_data_sheet_nueva, url_data_sheet)
     WHERE id_componente = p_id_componente_actualizar;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error de violación de unicidad al actualizar el componente. Verifique que los nuevos datos no dupliquen información existente (ej. nombre+modelo+id_equipo si existe tal restricción): %', SQLERRM;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar el componente: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.actualizar_componente(IN p_id_componente_actualizar integer, IN p_nombre_nuevo character varying, IN p_modelo_nuevo character varying, IN p_tipo_nuevo character varying, IN p_codigo_imt_nuevo integer, IN p_descripcion_nueva text, IN p_precio_referencia_nuevo double precision, IN p_url_data_sheet_nueva text) OWNER TO postgres;

--
-- TOC entry 373 (class 1255 OID 22878)
-- Name: actualizar_empresa_mantenimiento(integer, character varying, character varying, character varying, character varying, text, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_empresa_mantenimiento(IN p_id_empresa_actualizar integer, IN p_nombre_nuevo character varying DEFAULT NULL::character varying, IN p_nombre_responsable_nuevo character varying DEFAULT NULL::character varying, IN p_apellido_responsable_nuevo character varying DEFAULT NULL::character varying, IN p_telefono_nuevo character varying DEFAULT NULL::character varying, IN p_direccion_nueva text DEFAULT NULL::text, IN p_nit_nuevo character varying DEFAULT NULL::character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_empresa_existe   BOOLEAN;
    v_nombre_trimmed   TEXT;
BEGIN
    -- 1) Verificar que la empresa exista y esté activa
    SELECT EXISTS (
        SELECT 1
          FROM public.empresas_mantenimiento
         WHERE id_empresa_mantenimiento = p_id_empresa_actualizar
           AND estado_eliminado = FALSE
    ) INTO v_empresa_existe;

    IF NOT v_empresa_existe THEN
        RAISE EXCEPTION 'No se encontró una empresa de mantenimiento activa con ID = % para actualizar.', p_id_empresa_actualizar;
    END IF;

    -- 2) Lógica de duplicados solo para el nombre
    IF p_nombre_nuevo IS NOT NULL THEN
        v_nombre_trimmed := TRIM(both ' ' FROM p_nombre_nuevo);

        -- 2a) Reactivar si existe eliminada lógicamente
        UPDATE public.empresas_mantenimiento
           SET estado_eliminado = FALSE
         WHERE nombre = v_nombre_trimmed
           AND estado_eliminado = TRUE;
        IF FOUND THEN
            -- Eliminar lógicamente la actual y terminar
            UPDATE public.empresas_mantenimiento
               SET estado_eliminado = TRUE
             WHERE id_empresa_mantenimiento = p_id_empresa_actualizar;
            RETURN;
        END IF;

        -- 2b) Verificar si otro registro activo ya tiene ese nombre
        IF EXISTS (
            SELECT 1
              FROM public.empresas_mantenimiento
             WHERE nombre = v_nombre_trimmed
               AND estado_eliminado = FALSE
               AND id_empresa_mantenimiento <> p_id_empresa_actualizar
        ) THEN
            RAISE EXCEPTION 'Error de violación de unicidad al actualizar la empresa de mantenimiento. Verifique que los nuevos datos (ej. NIT o nombre) no dupliquen información existente: %', SQLERRM;
        END IF;
    END IF;

    -- 3) Actualización normal de todos los campos
    UPDATE public.empresas_mantenimiento
       SET
           nombre               = COALESCE(v_nombre_trimmed, nombre),
           nombre_responsable   = COALESCE(p_nombre_responsable_nuevo, nombre_responsable),
           apellido_responsable = COALESCE(p_apellido_responsable_nuevo, apellido_responsable),
           telefono             = COALESCE(p_telefono_nuevo, telefono),
           direccion            = COALESCE(p_direccion_nueva, direccion),
           nit                  = COALESCE(p_nit_nuevo, nit)
     WHERE id_empresa_mantenimiento = p_id_empresa_actualizar
       AND estado_eliminado = FALSE;

EXCEPTION
    WHEN unique_violation THEN
         RAISE EXCEPTION 'Error de violación de unicidad al actualizar la empresa de mantenimiento. Verifique que los nuevos datos (ej. NIT o nombre) no dupliquen información existente: %', SQLERRM;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar la empresa de mantenimiento: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.actualizar_empresa_mantenimiento(IN p_id_empresa_actualizar integer, IN p_nombre_nuevo character varying, IN p_nombre_responsable_nuevo character varying, IN p_apellido_responsable_nuevo character varying, IN p_telefono_nuevo character varying, IN p_direccion_nueva text, IN p_nit_nuevo character varying) OWNER TO postgres;

--
-- TOC entry 377 (class 1255 OID 22879)
-- Name: actualizar_equipo(integer, character varying, character varying, character varying, character varying, text, character varying, character varying, character varying, double precision, integer, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_equipo(IN p_id_equipo_actualizar integer, IN p_nombre_grupo_equipo_nuevo character varying DEFAULT NULL::character varying, IN p_modelo_grupo_equipo_nuevo character varying DEFAULT NULL::character varying, IN p_marca_grupo_equipo_nuevo character varying DEFAULT NULL::character varying, IN p_codigo_ucb_nuevo character varying DEFAULT NULL::character varying, IN p_descripcion_nueva text DEFAULT NULL::text, IN p_numero_serial_nuevo character varying DEFAULT NULL::character varying, IN p_ubicacion_nueva character varying DEFAULT NULL::character varying, IN p_procedencia_nueva character varying DEFAULT NULL::character varying, IN p_costo_referencia_nuevo double precision DEFAULT NULL::double precision, IN p_tiempo_maximo_prestamo_nuevo integer DEFAULT NULL::integer, IN p_nombre_gavetero_nuevo character varying DEFAULT NULL::character varying, IN p_estado_equipo_nuevo character varying DEFAULT NULL::character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_grupo_equipo_actual        INTEGER;
    v_id_categoria_actual           INTEGER;
    v_id_grupo_equipo_para_actualizar INTEGER;
    v_id_categoria_para_actualizar  INTEGER;
    v_id_gavetero_para_actualizar     INTEGER;
BEGIN
    SELECT
        e.id_grupo_equipo,
        ge.id_categoria
    INTO
        v_id_grupo_equipo_actual,
        v_id_categoria_actual
    FROM public.equipos e
    JOIN public.grupos_equipos ge ON e.id_grupo_equipo = ge.id_grupo_equipo
    WHERE e.id_equipo = p_id_equipo_actualizar AND e.estado_eliminado = FALSE AND ge.estado_eliminado = FALSE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un equipo activo con ID = % o su grupo asociado no existe/está eliminado.', p_id_equipo_actualizar;
    END IF;

    IF p_nombre_grupo_equipo_nuevo IS NOT NULL and p_modelo_grupo_equipo_nuevo is not null and p_marca_grupo_equipo_nuevo is not null THEN
        SELECT ge.id_grupo_equipo, ge.id_categoria
          INTO v_id_grupo_equipo_para_actualizar, v_id_categoria_para_actualizar
          FROM public.grupos_equipos AS ge
         WHERE ge.nombre           = p_nombre_grupo_equipo_nuevo
		   AND ge.modelo 			= p_modelo_grupo_equipo_nuevo
		   AND ge.marca				= p_marca_grupo_equipo_nuevo
           AND ge.estado_eliminado = FALSE;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'No se encontró el grupo de equipos con nombre = % para asociar.', p_nombre_grupo_equipo_nuevo;
        END IF;
    ELSE
        v_id_grupo_equipo_para_actualizar := NULL;
    END IF;

    IF p_nombre_gavetero_nuevo IS NOT NULL THEN
        SELECT g.id_gavetero
          INTO v_id_gavetero_para_actualizar
          FROM public.gaveteros AS g
         WHERE g.nombre            = p_nombre_gavetero_nuevo
           AND g.estado_eliminado  = FALSE;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'No se encontró el gavetero con nombre = % para asociar.', p_nombre_gavetero_nuevo;
        END IF;
    ELSE
        v_id_gavetero_para_actualizar := NULL;
    END IF;

    IF p_estado_equipo_nuevo IS NOT NULL THEN
        IF lower(p_estado_equipo_nuevo) NOT IN ('operativo', 'inoperativo', 'parcialmente_operativo') THEN
            RAISE EXCEPTION 'Valor inválido para estado_equipo: "%". Debe ser ''operativo'', ''inoperativo'', o ''parcialmente_operativo''.', p_estado_equipo_nuevo;
        END IF;
    END IF;

    UPDATE public.equipos
       SET
           id_grupo_equipo     = COALESCE(v_id_grupo_equipo_para_actualizar, id_grupo_equipo),
           id_gavetero         = COALESCE(v_id_gavetero_para_actualizar, id_gavetero),
           codigo_ucb          = COALESCE(p_codigo_ucb_nuevo, codigo_ucb),
           descripcion         = COALESCE(p_descripcion_nueva, descripcion),
           numero_serial       = COALESCE(p_numero_serial_nuevo, numero_serial),
           ubicacion           = COALESCE(p_ubicacion_nueva, ubicacion),
           procedencia         = COALESCE(p_procedencia_nueva, procedencia),
           costo_referencia    = COALESCE(p_costo_referencia_nuevo, costo_referencia),
           tiempo_max_prestamo = COALESCE(p_tiempo_maximo_prestamo_nuevo, tiempo_max_prestamo),
           estado_equipo       = COALESCE(CAST(lower(p_estado_equipo_nuevo) AS public.estado_equipo), estado_equipo) -- CORRECCIÓN AQUÍ
     WHERE id_equipo = p_id_equipo_actualizar;

EXCEPTION
    WHEN unique_violation THEN
        IF SQLERRM LIKE '%unique_codigo_imt%' THEN
            RAISE EXCEPTION 'Error: El código IMT generado ("%") para el nuevo grupo de equipo ya está en uso por otro equipo. Esto ocurre si se cambió a una categoría diferente y el código IMT resultante ya existe. (Detalle: %)', v_codigo_imt_a_usar, SQLERRM;
        ELSIF SQLERRM LIKE '%equipos_codigo_ucb_key%' OR SQLERRM LIKE '%unique_codigo_ucb%' THEN
            RAISE EXCEPTION 'Error: El código UCB "%" ya existe para otro equipo. (Detalle: %)', p_codigo_ucb_nuevo, SQLERRM;
        ELSIF SQLERRM LIKE '%equipos_numero_serial_key%' OR SQLERRM LIKE '%unique_numero_serial%' THEN
            RAISE EXCEPTION 'Error: El número de serie "%" ya existe para otro equipo. (Detalle: %)', p_numero_serial_nuevo, SQLERRM;
        ELSE
            RAISE EXCEPTION 'Error de violación de unicidad al actualizar el equipo. (Detalle: %)', SQLERRM;
        END IF;
    WHEN OTHERS THEN
         RAISE EXCEPTION 'Error inesperado al actualizar el equipo: % (SQLSTATE: %)', SQLERRM, SQLSTATE;
END;
$$;


ALTER PROCEDURE public.actualizar_equipo(IN p_id_equipo_actualizar integer, IN p_nombre_grupo_equipo_nuevo character varying, IN p_modelo_grupo_equipo_nuevo character varying, IN p_marca_grupo_equipo_nuevo character varying, IN p_codigo_ucb_nuevo character varying, IN p_descripcion_nueva text, IN p_numero_serial_nuevo character varying, IN p_ubicacion_nueva character varying, IN p_procedencia_nueva character varying, IN p_costo_referencia_nuevo double precision, IN p_tiempo_maximo_prestamo_nuevo integer, IN p_nombre_gavetero_nuevo character varying, IN p_estado_equipo_nuevo character varying) OWNER TO postgres;

--
-- TOC entry 292 (class 1255 OID 22881)
-- Name: actualizar_estado_prestamo(integer, public.estado_prestamo); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_estado_prestamo(IN p_id_prestamo integer, IN p_estado_prestamo_input public.estado_prestamo)
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- 1. Validar que el nuevo estado esté dentro de los permitidos
    IF p_estado_prestamo_input NOT IN (
        'pendiente',
        'rechazado',
        'aprobado',
        'activo',
        'finalizado',
        'cancelado'
    ) THEN
        RAISE EXCEPTION 
            'Estado inválido: “%”. Solo se permiten pendiente, rechazado, aprobado, activo, finalizado o cancelado.',
            p_estado_prestamo_input
        USING ERRCODE = '22023';  -- invalid_parameter_value
    END IF;

    -- 2. Intentar la actualización
    BEGIN
        UPDATE public.prestamos
           SET estado_prestamo = p_estado_prestamo_input
         WHERE id_prestamo    = p_id_prestamo;

        -- 3. Verificar que realmente se haya actualizado alguna fila
        IF NOT FOUND THEN
            RAISE EXCEPTION 
                'No existe préstamo con id % o el estado ya era el mismo.',
                p_id_prestamo
            USING ERRCODE = 'P0002';  -- no_data_found-like
        END IF;

    EXCEPTION
        WHEN OTHERS THEN
            RAISE EXCEPTION 
                'Error inesperado (%s) al actualizar estado del préstamo %: %',
                SQLSTATE, p_id_prestamo, SQLERRM;
    END;
END;
$$;


ALTER PROCEDURE public.actualizar_estado_prestamo(IN p_id_prestamo integer, IN p_estado_prestamo_input public.estado_prestamo) OWNER TO postgres;

--
-- TOC entry 314 (class 1255 OID 22882)
-- Name: actualizar_gavetero(integer, character varying, character varying, character varying, double precision, double precision, double precision); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_gavetero(IN p_id_gavetero_actualizar integer, IN p_nombre_nuevo character varying DEFAULT NULL::character varying, IN p_tipo_nuevo character varying DEFAULT NULL::character varying, IN p_nombre_mueble_nuevo character varying DEFAULT NULL::character varying, IN p_longitud_nueva double precision DEFAULT NULL::double precision, IN p_profundidad_nueva double precision DEFAULT NULL::double precision, IN p_altura_nueva double precision DEFAULT NULL::double precision)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_mueble_para_actualizar INTEGER;
    v_gavetero_existe BOOLEAN;
    v_nombre_actual character varying;
BEGIN
    SELECT g.nombre, TRUE
      INTO v_nombre_actual, v_gavetero_existe
      FROM public.gaveteros g
     WHERE g.id_gavetero = p_id_gavetero_actualizar AND g.estado_eliminado = FALSE;

    IF NOT v_gavetero_existe THEN
        RAISE EXCEPTION 'No se encontró un gavetero activo con ID = % para actualizar.', p_id_gavetero_actualizar;
    END IF;

    IF p_nombre_mueble_nuevo IS NOT NULL THEN
        SELECT m.id_mueble
          INTO v_id_mueble_para_actualizar
          FROM public.muebles AS m
         WHERE m.nombre           = p_nombre_mueble_nuevo
           AND m.estado_eliminado = FALSE;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'No se encontró el mueble activo con nombre = % para asociar al gavetero.', p_nombre_mueble_nuevo;
        END IF;
    ELSE
        v_id_mueble_para_actualizar := NULL; 
    END IF;

 
    IF p_nombre_nuevo IS NOT NULL AND p_nombre_nuevo IS DISTINCT FROM v_nombre_actual THEN
        IF EXISTS (
            SELECT 1
              FROM public.gaveteros
             WHERE nombre = p_nombre_nuevo
               AND estado_eliminado = FALSE
               
        ) THEN
            RAISE EXCEPTION 'Ya existe otro gavetero activo con el nombre = %.', p_nombre_nuevo;
        END IF;
    END IF;

    UPDATE public.gaveteros
       SET
           nombre      = COALESCE(p_nombre_nuevo, nombre),
           tipo        = COALESCE(p_tipo_nuevo, tipo),
           id_mueble   = COALESCE(v_id_mueble_para_actualizar, id_mueble),
           longitud    = COALESCE(p_longitud_nueva, longitud),
           profundidad = COALESCE(p_profundidad_nueva, profundidad),
           altura      = COALESCE(p_altura_nueva, altura)
     WHERE id_gavetero = p_id_gavetero_actualizar;

EXCEPTION
    WHEN unique_violation THEN
         RAISE EXCEPTION 'Violación de unicidad al intentar actualizar el gavetero con nombre "%". (Detalle: %)', COALESCE(p_nombre_nuevo, v_nombre_actual), SQLERRM;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar el gavetero: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.actualizar_gavetero(IN p_id_gavetero_actualizar integer, IN p_nombre_nuevo character varying, IN p_tipo_nuevo character varying, IN p_nombre_mueble_nuevo character varying, IN p_longitud_nueva double precision, IN p_profundidad_nueva double precision, IN p_altura_nueva double precision) OWNER TO postgres;

--
-- TOC entry 277 (class 1255 OID 22883)
-- Name: actualizar_grupo_equipo(integer, character varying, character varying, character varying, text, character varying, text, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_grupo_equipo(IN p_id_grupo_equipo_actualizar integer, IN p_nombre_nuevo character varying DEFAULT NULL::character varying, IN p_modelo_nuevo character varying DEFAULT NULL::character varying, IN p_marca_nueva character varying DEFAULT NULL::character varying, IN p_descripcion_nueva text DEFAULT NULL::text, IN p_nombre_categoria_nuevo character varying DEFAULT NULL::character varying, IN p_url_data_sheet_nuevo text DEFAULT NULL::text, IN p_url_imagen_nuevo text DEFAULT NULL::text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_categoria_actual            INTEGER; 
    v_id_categoria_para_actualizar   INTEGER;
    v_grupo_equipo_existe            BOOLEAN;
    v_nombre_actual                  character varying;
    v_modelo_actual                  character varying;
    v_marca_actual                   character varying;
    v_nombre_final_para_verificar    character varying;
    v_modelo_final_para_verificar    character varying;
    v_marca_final_para_verificar     character varying;
BEGIN
    SELECT
        ge.nombre,
        ge.modelo,
        ge.marca,
        ge.id_categoria 
    INTO
        v_nombre_actual,
        v_modelo_actual,
        v_marca_actual,
        v_id_categoria_actual 
    FROM public.grupos_equipos ge
    WHERE ge.id_grupo_equipo = p_id_grupo_equipo_actualizar AND ge.estado_eliminado = FALSE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un grupo de equipos activo con ID = % para actualizar.', p_id_grupo_equipo_actualizar;
    END IF;

    IF p_nombre_categoria_nuevo IS NOT NULL THEN
        SELECT c.id_categoria
          INTO v_id_categoria_para_actualizar
          FROM public.categorias AS c
         WHERE c.nombre           = p_nombre_categoria_nuevo
           AND c.estado_eliminado = FALSE;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'No se encontró la categoría activa con nombre = "%" para asociar al grupo de equipos.', p_nombre_categoria_nuevo;
        END IF;
    ELSE

        v_id_categoria_para_actualizar := NULL;
    END IF;

    v_nombre_final_para_verificar := COALESCE(p_nombre_nuevo, v_nombre_actual);
    v_modelo_final_para_verificar := COALESCE(p_modelo_nuevo, v_modelo_actual);
    v_marca_final_para_verificar  := COALESCE(p_marca_nueva, v_marca_actual);

    IF (v_nombre_final_para_verificar IS DISTINCT FROM v_nombre_actual) OR
       (v_modelo_final_para_verificar IS DISTINCT FROM v_modelo_actual) OR
       (v_marca_final_para_verificar IS DISTINCT FROM v_marca_actual)
    THEN
        IF EXISTS (
            SELECT 1
              FROM public.grupos_equipos AS ge
             WHERE ge.nombre            = v_nombre_final_para_verificar
               AND ge.modelo            = v_modelo_final_para_verificar
               AND ge.marca             = v_marca_final_para_verificar
               AND ge.estado_eliminado  = FALSE
               AND ge.id_grupo_equipo   != p_id_grupo_equipo_actualizar 
        ) THEN
            RAISE EXCEPTION 'Ya existe otro grupo de equipos activo con la combinación: Nombre="%", Modelo="%", Marca="%".',
                v_nombre_final_para_verificar, v_modelo_final_para_verificar, v_marca_final_para_verificar;
        END IF;
    END IF;

    UPDATE public.grupos_equipos
       SET
           nombre         = COALESCE(p_nombre_nuevo, nombre),
           modelo         = COALESCE(p_modelo_nuevo, modelo),
           marca          = COALESCE(p_marca_nueva, marca),
           descripcion    = COALESCE(p_descripcion_nueva, descripcion),
           id_categoria   = COALESCE(v_id_categoria_para_actualizar, id_categoria), 
           url_data_sheet = COALESCE(p_url_data_sheet_nuevo, url_data_sheet),
           url_imagen     = COALESCE(p_url_imagen_nuevo, url_imagen)
     WHERE id_grupo_equipo = p_id_grupo_equipo_actualizar;

EXCEPTION
    WHEN unique_violation THEN
          RAISE EXCEPTION 'Error: La combinación Nombre="%", Modelo="%", Marca="%" ya está en uso por otro grupo de equipos. (Detalle: %)',
            v_nombre_final_para_verificar, v_modelo_final_para_verificar, v_marca_final_para_verificar, SQLERRM;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar el grupo de equipos: % (SQLSTATE: %)', SQLERRM, SQLSTATE;
END;
$$;


ALTER PROCEDURE public.actualizar_grupo_equipo(IN p_id_grupo_equipo_actualizar integer, IN p_nombre_nuevo character varying, IN p_modelo_nuevo character varying, IN p_marca_nueva character varying, IN p_descripcion_nueva text, IN p_nombre_categoria_nuevo character varying, IN p_url_data_sheet_nuevo text, IN p_url_imagen_nuevo text) OWNER TO postgres;

--
-- TOC entry 386 (class 1255 OID 22884)
-- Name: actualizar_mueble(integer, character varying, character varying, double precision, character varying, double precision, double precision, double precision); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_mueble(IN p_id_mueble_actual integer, IN p_nombre_nuevo character varying DEFAULT NULL::character varying, IN p_tipo_nuevo character varying DEFAULT NULL::character varying, IN p_costo_nuevo double precision DEFAULT NULL::double precision, IN p_ubicacion_nueva character varying DEFAULT NULL::character varying, IN p_longitud_nueva double precision DEFAULT NULL::double precision, IN p_profundidad_nueva double precision DEFAULT NULL::double precision, IN p_altura_nueva double precision DEFAULT NULL::double precision)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.muebles
     WHERE id_mueble = p_id_mueble_actual
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un mueble activo con ID = %.', p_id_mueble_actual;
    END IF;

    UPDATE public.muebles
       SET
           nombre      = COALESCE(p_nombre_nuevo, nombre),
           tipo        = COALESCE(p_tipo_nuevo, tipo),
           costo       = COALESCE(p_costo_nuevo, costo),
           ubicacion   = COALESCE(p_ubicacion_nueva, ubicacion),
           longitud    = COALESCE(p_longitud_nueva, longitud),
           profundidad = COALESCE(p_profundidad_nueva, profundidad),
           altura      = COALESCE(p_altura_nueva, altura)
     WHERE id_mueble = p_id_mueble_actual
       AND estado_eliminado = FALSE;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error: Ya existe un mueble con el nombre "%" en la base de datos.', p_nombre_nuevo;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar mueble: % (SQLSTATE %)', SQLERRM, SQLSTATE;
END;
$$;


ALTER PROCEDURE public.actualizar_mueble(IN p_id_mueble_actual integer, IN p_nombre_nuevo character varying, IN p_tipo_nuevo character varying, IN p_costo_nuevo double precision, IN p_ubicacion_nueva character varying, IN p_longitud_nueva double precision, IN p_profundidad_nueva double precision, IN p_altura_nueva double precision) OWNER TO postgres;

--
-- TOC entry 284 (class 1255 OID 22885)
-- Name: actualizar_usuario(character varying, character varying, character varying, character varying, character varying, text, public.tipo_usuario, character varying, character varying, character varying, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.actualizar_usuario(IN p_carnet_actual character varying, IN p_nombre_nuevo character varying DEFAULT NULL::character varying, IN p_apellido_paterno_nuevo character varying DEFAULT NULL::character varying, IN p_apellido_materno_nuevo character varying DEFAULT NULL::character varying, IN p_email_nuevo character varying DEFAULT NULL::character varying, IN p_contrasena_nueva text DEFAULT NULL::text, IN p_rol_nuevo public.tipo_usuario DEFAULT NULL::public.tipo_usuario, IN p_carrera_nueva character varying DEFAULT NULL::character varying, IN p_telefono_nuevo character varying DEFAULT NULL::character varying, IN p_telefono_ref_nuevo character varying DEFAULT NULL::character varying, IN p_nombre_ref_nuevo character varying DEFAULT NULL::character varying, IN p_email_ref_nuevo character varying DEFAULT NULL::character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_carrera_actual integer;
    v_id_carrera_para_actualizar integer;
BEGIN
    SELECT id_carrera
      INTO v_id_carrera_actual
    FROM public.usuarios
    WHERE carnet = p_carnet_actual
      AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No existe usuario activo con carnet = %', p_carnet_actual;
    END IF;

    IF p_carrera_nueva IS NOT NULL THEN
        SELECT id_carrera
          INTO v_id_carrera_para_actualizar
        FROM public.carreras
        WHERE nombre = p_carrera_nueva
          AND estado_eliminado = FALSE;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'Carrera no encontrada o eliminada: %', p_carrera_nueva;
        END IF;
    ELSE
        v_id_carrera_para_actualizar := v_id_carrera_actual;
    END IF;

    IF p_rol_nuevo IS NOT NULL THEN
        IF p_rol_nuevo NOT IN ('administrador','estudiante') THEN
            RAISE EXCEPTION 'Rol inválido: %. Debe ser administrador o estudiante.', p_rol_nuevo;
        END IF;
    END IF;

    UPDATE public.usuarios
       SET
         nombre            = COALESCE(p_nombre_nuevo, nombre),
         apellido_paterno  = COALESCE(p_apellido_paterno_nuevo, apellido_paterno),
         apellido_materno  = COALESCE(p_apellido_materno_nuevo, apellido_materno),
         email             = COALESCE(p_email_nuevo, email),
         contrasena        = COALESCE(p_contrasena_nueva, contrasena),
         rol               = COALESCE(p_rol_nuevo, rol),
         id_carrera        = v_id_carrera_para_actualizar,
         telefono          = COALESCE(p_telefono_nuevo, telefono),
         telefono_referencia = COALESCE(p_telefono_ref_nuevo, telefono_referencia),
         nombre_referencia = COALESCE(p_nombre_ref_nuevo, nombre_referencia),
         email_referencia  = COALESCE(p_email_ref_nuevo, email_referencia)
     WHERE carnet = p_carnet_actual
       AND estado_eliminado = FALSE;

EXCEPTION
    WHEN unique_violation THEN
        IF SQLERRM LIKE '%usuarios_carnet_key%' THEN
            RAISE EXCEPTION 'Error: El carnet "%" ya está en uso.', p_carnet_actual;
        ELSIF SQLERRM LIKE '%usuarios_email_key%' THEN
            RAISE EXCEPTION 'Error: El email "%" ya está en uso.', p_email_nuevo;
        ELSE
            RAISE EXCEPTION 'Violación de unicidad: %', SQLERRM;
        END IF;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inesperado al actualizar usuario: % (SQLSTATE %)', SQLERRM, SQLSTATE;
END;
$$;


ALTER PROCEDURE public.actualizar_usuario(IN p_carnet_actual character varying, IN p_nombre_nuevo character varying, IN p_apellido_paterno_nuevo character varying, IN p_apellido_materno_nuevo character varying, IN p_email_nuevo character varying, IN p_contrasena_nueva text, IN p_rol_nuevo public.tipo_usuario, IN p_carrera_nueva character varying, IN p_telefono_nuevo character varying, IN p_telefono_ref_nuevo character varying, IN p_nombre_ref_nuevo character varying, IN p_email_ref_nuevo character varying) OWNER TO postgres;

--
-- TOC entry 324 (class 1255 OID 22886)
-- Name: eliminar_accesorio(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_accesorio(IN p_id_accesorio integer)
    LANGUAGE plpgsql
    AS $$
BEGIN

    -- 2) Bloquear la fila y verificar existencia de accesorio activo
    PERFORM 1
      FROM public.accesorios
     WHERE id_accesorio    = p_id_accesorio
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un accesorio activo con ID = %.', p_id_accesorio;
    END IF;

    -- 3) Marcar como eliminado lógicamente
    UPDATE public.accesorios
       SET estado_eliminado = TRUE
     WHERE id_accesorio = p_id_accesorio;

EXCEPTION
    WHEN OTHERS THEN
        -- Capturar cualquier error inesperado
        RAISE EXCEPTION 'Error al eliminar lógicamente el accesorio (ID = %): %',
                        p_id_accesorio, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_accesorio(IN p_id_accesorio integer) OWNER TO postgres;

--
-- TOC entry 320 (class 1255 OID 22887)
-- Name: eliminar_carrera(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_carrera(IN p_id_carrera integer)
    LANGUAGE plpgsql
    AS $$
BEGIN

    PERFORM 1
      FROM public.carreras
     WHERE id_carrera      = p_id_carrera
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró una carrera activa con ID = %.', p_id_carrera;
    END IF;

    UPDATE public.carreras
       SET estado_eliminado = TRUE
     WHERE id_carrera = p_id_carrera;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente la carrera (ID = %): %',
                        p_id_carrera, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_carrera(IN p_id_carrera integer) OWNER TO postgres;

--
-- TOC entry 297 (class 1255 OID 22888)
-- Name: eliminar_categoria(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_categoria(IN p_id_categoria integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.categorias
     WHERE id_categoria    = p_id_categoria
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró una categoría activa con ID = %.', p_id_categoria;
    END IF;

    UPDATE public.categorias
       SET estado_eliminado = TRUE
     WHERE id_categoria = p_id_categoria;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente la categoría (ID = %): %',
                        p_id_categoria, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_categoria(IN p_id_categoria integer) OWNER TO postgres;

--
-- TOC entry 281 (class 1255 OID 22889)
-- Name: eliminar_componente(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_componente(IN p_id_componente integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.componentes
     WHERE id_componente   = p_id_componente
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un componente activo con ID = %.', p_id_componente;
    END IF;

    UPDATE public.componentes
       SET estado_eliminado = TRUE
     WHERE id_componente = p_id_componente;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el componente (ID = %): %',
                        p_id_componente, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_componente(IN p_id_componente integer) OWNER TO postgres;

--
-- TOC entry 370 (class 1255 OID 22890)
-- Name: eliminar_empresas_mantenimiento(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_empresas_mantenimiento(IN p_id_empresa_mantenimiento integer)
    LANGUAGE plpgsql
    AS $$
BEGIN

    PERFORM 1
      FROM public.empresas_mantenimiento
     WHERE id_empresa_mantenimiento = p_id_empresa_mantenimiento
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un registro activo en empresas_mantenimiento con ID = %.', 
                        p_id_empresa_mantenimiento;
    END IF;

    UPDATE public.empresas_mantenimiento
       SET estado_eliminado = TRUE
     WHERE id_empresa_mantenimiento = p_id_empresa_mantenimiento;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente empresas_mantenimiento (ID = %): %',
                        p_id_empresa_mantenimiento, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_empresas_mantenimiento(IN p_id_empresa_mantenimiento integer) OWNER TO postgres;

--
-- TOC entry 333 (class 1255 OID 22891)
-- Name: eliminar_equipo(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_equipo(IN p_id_equipo integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.equipos
     WHERE id_equipo = p_id_equipo
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un equipo activo con ID = %.', p_id_equipo;
    END IF;

    UPDATE public.equipos
       SET estado_eliminado = TRUE
     WHERE id_equipo = p_id_equipo;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el equipo (ID = %): %',
                        p_id_equipo, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_equipo(IN p_id_equipo integer) OWNER TO postgres;

--
-- TOC entry 291 (class 1255 OID 22892)
-- Name: eliminar_gavetero(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_gavetero(IN p_id_gavetero integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.gaveteros
     WHERE id_gavetero = p_id_gavetero
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un gavetero activo con ID = %.', p_id_gavetero;
    END IF;

    UPDATE public.gaveteros
       SET estado_eliminado = TRUE
     WHERE id_gavetero = p_id_gavetero;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el gavetero (ID = %): %',
                        p_id_gavetero, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_gavetero(IN p_id_gavetero integer) OWNER TO postgres;

--
-- TOC entry 337 (class 1255 OID 22893)
-- Name: eliminar_grupo_equipo(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_grupo_equipo(IN p_id_grupo_equipo integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.grupos_equipos
     WHERE id_grupo_equipo = p_id_grupo_equipo
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un grupo de equipos activo con ID = %.', p_id_grupo_equipo;
    END IF;

    UPDATE public.grupos_equipos
       SET estado_eliminado = TRUE
     WHERE id_grupo_equipo = p_id_grupo_equipo;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el grupo de equipos (ID = %): %',
                        p_id_grupo_equipo, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_grupo_equipo(IN p_id_grupo_equipo integer) OWNER TO postgres;

--
-- TOC entry 288 (class 1255 OID 22894)
-- Name: eliminar_mantenimiento(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_mantenimiento(IN p_id_mantenimiento integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.mantenimientos
     WHERE id_mantenimiento = p_id_mantenimiento
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un mantenimiento activo con ID = %.', p_id_mantenimiento;
    END IF;

    UPDATE public.mantenimientos
       SET estado_eliminado = TRUE
     WHERE id_mantenimiento = p_id_mantenimiento;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el mantenimiento (ID = %): %',
                        p_id_mantenimiento, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_mantenimiento(IN p_id_mantenimiento integer) OWNER TO postgres;

--
-- TOC entry 360 (class 1255 OID 22895)
-- Name: eliminar_mueble(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_mueble(IN p_id_mueble integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.muebles
     WHERE id_mueble = p_id_mueble
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un mueble activo con ID = %.', p_id_mueble;
    END IF;

    UPDATE public.muebles
       SET estado_eliminado = TRUE
     WHERE id_mueble = p_id_mueble;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el mueble (ID = %): %', 
                        p_id_mueble, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_mueble(IN p_id_mueble integer) OWNER TO postgres;

--
-- TOC entry 321 (class 1255 OID 22896)
-- Name: eliminar_prestamo(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_prestamo(IN p_id_prestamo integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.prestamos
     WHERE id_prestamo     = p_id_prestamo
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un préstamo activo con ID = %.', p_id_prestamo;
    END IF;

    UPDATE public.prestamos
       SET estado_eliminado = TRUE
     WHERE id_prestamo = p_id_prestamo;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el préstamo (ID = %): %', 
                        p_id_prestamo, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_prestamo(IN p_id_prestamo integer) OWNER TO postgres;

--
-- TOC entry 287 (class 1255 OID 22897)
-- Name: eliminar_usuario(character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.eliminar_usuario(IN p_carnet character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    PERFORM 1
      FROM public.usuarios
     WHERE carnet = p_carnet
       AND estado_eliminado = FALSE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un usuario activo con carnet = %.', p_carnet;
    END IF;

    UPDATE public.usuarios
       SET estado_eliminado = TRUE
     WHERE carnet = p_carnet;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al eliminar lógicamente el usuario (carnet = %): %', 
                        p_carnet, SQLERRM;
END;
$$;


ALTER PROCEDURE public.eliminar_usuario(IN p_carnet character varying) OWNER TO postgres;

--
-- TOC entry 354 (class 1255 OID 22898)
-- Name: fn_actualizar_cantidad_equipo_por_estado(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_actualizar_cantidad_equipo_por_estado() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
  IF OLD.estado_eliminado = FALSE AND NEW.estado_eliminado = TRUE THEN
    UPDATE public.grupos_equipos
       SET cantidad = GREATEST(0, COALESCE(cantidad, 0) - 1) 
     WHERE id_grupo_equipo = NEW.id_grupo_equipo;

  ELSIF OLD.estado_eliminado = TRUE AND NEW.estado_eliminado = FALSE THEN
    UPDATE public.grupos_equipos
       SET cantidad = COALESCE(cantidad, 0) + 1
     WHERE id_grupo_equipo = NEW.id_grupo_equipo;
  END IF;

  RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_actualizar_cantidad_equipo_por_estado() OWNER TO postgres;

--
-- TOC entry 353 (class 1255 OID 22899)
-- Name: fn_actualizar_cantidad_tras_update_equipos(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_actualizar_cantidad_tras_update_equipos() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Este trigger se activa cuando OLD.id_grupo_equipo es diferente de NEW.id_grupo_equipo.
    -- Solo se realizarán ajustes si el estado_eliminado del equipo no cambia en la misma transacción,
    -- ya que se asume que otro trigger maneja los cambios de cantidad cuando estado_eliminado cambia.

    IF OLD.estado_eliminado = NEW.estado_eliminado THEN
        -- El estado de eliminación no cambió, así que esto es un "movimiento puro".
        IF OLD.estado_eliminado = FALSE THEN
            -- El equipo se movió mientras estaba activo.
            -- Decrementar del grupo antiguo.
            IF OLD.id_grupo_equipo IS NOT NULL THEN
                UPDATE public.grupos_equipos
                SET cantidad = cantidad - 1
                WHERE id_grupo_equipo = OLD.id_grupo_equipo;
            END IF;

            -- Incrementar en el grupo nuevo.
            IF NEW.id_grupo_equipo IS NOT NULL THEN
                UPDATE public.grupos_equipos
                SET cantidad = cantidad + 1
                WHERE id_grupo_equipo = NEW.id_grupo_equipo;
            END IF;
        END IF;
        -- Si el equipo se movió mientras estaba inactivo (OLD.estado_eliminado = TRUE),
        -- no se afectan las cantidades de equipos activos.
    END IF;
    -- Si OLD.estado_eliminado != NEW.estado_eliminado, el trigger que maneja
    -- los cambios de estado_eliminado se encargará de los ajustes de cantidad.

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_actualizar_cantidad_tras_update_equipos() OWNER TO postgres;

--
-- TOC entry 326 (class 1255 OID 22900)
-- Name: fn_actualizar_conteo_gaveteros_por_estado(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_actualizar_conteo_gaveteros_por_estado() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF OLD.estado_eliminado IS DISTINCT FROM NEW.estado_eliminado THEN

        IF OLD.estado_eliminado = TRUE AND NEW.estado_eliminado = FALSE THEN
            UPDATE public.muebles
               SET numero_gaveteros = COALESCE(numero_gaveteros, 0) + 1
             WHERE id_mueble = NEW.id_mueble;

        ELSIF OLD.estado_eliminado = FALSE AND NEW.estado_eliminado = TRUE THEN
            UPDATE public.muebles
               SET numero_gaveteros = GREATEST(0, COALESCE(numero_gaveteros, 0) - 1)
             WHERE id_mueble = NEW.id_mueble;
        END IF;
    END IF;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_actualizar_conteo_gaveteros_por_estado() OWNER TO postgres;

--
-- TOC entry 294 (class 1255 OID 22901)
-- Name: fn_actualizar_costo_promedio_grupo(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_actualizar_costo_promedio_grupo() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Cuando se inserta, actualiza o elimina un equipo, recalcula el promedio del grupo
    UPDATE public.grupos_equipos
    SET costo_promedio = (
        SELECT COALESCE(AVG(costo_referencia), 0)
        FROM public.equipos
        WHERE id_grupo_equipo = COALESCE(NEW.id_grupo_equipo, OLD.id_grupo_equipo)
          AND estado_eliminado = FALSE
          AND estado_equipo = 'operativo'
    )
    WHERE id_grupo_equipo = COALESCE(NEW.id_grupo_equipo, OLD.id_grupo_equipo);
    
    RETURN COALESCE(NEW, OLD);
END;
$$;


ALTER FUNCTION public.fn_actualizar_costo_promedio_grupo() OWNER TO postgres;

--
-- TOC entry 357 (class 1255 OID 22902)
-- Name: fn_actualizar_gavetero_tras_update_mueble(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_actualizar_gavetero_tras_update_mueble() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF OLD.estado_eliminado = NEW.estado_eliminado THEN
        IF OLD.estado_eliminado = FALSE THEN

            IF OLD.id_mueble IS NOT NULL THEN
                UPDATE public.muebles
                SET numero_gaveteros = numero_gaveteros - 1
                WHERE id_mueble = OLD.id_mueble;
            END IF;

            IF NEW.id_mueble IS NOT NULL THEN
                UPDATE public.muebles
                SET numero_gaveteros = numero_gaveteros + 1
                WHERE id_mueble = NEW.id_mueble;
            END IF;
        END IF;
    END IF;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_actualizar_gavetero_tras_update_mueble() OWNER TO postgres;

--
-- TOC entry 296 (class 1255 OID 22903)
-- Name: fn_estado_eliminado_mantenimiento_a_detalle(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_estado_eliminado_mantenimiento_a_detalle() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN

    IF OLD.estado_eliminado IS DISTINCT FROM NEW.estado_eliminado THEN
        UPDATE public.detalles_mantenimientos
           SET estado_eliminado = NEW.estado_eliminado 
         WHERE id_mantenimiento = NEW.id_mantenimiento;
    END IF;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_estado_eliminado_mantenimiento_a_detalle() OWNER TO postgres;

--
-- TOC entry 366 (class 1255 OID 22904)
-- Name: fn_estado_eliminado_prestamo_a_detalle(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_estado_eliminado_prestamo_a_detalle() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN

    IF OLD.estado_eliminado IS DISTINCT FROM NEW.estado_eliminado THEN
        UPDATE public.detalles_prestamos
           SET estado_eliminado = NEW.estado_eliminado 
         WHERE id_prestamo = NEW.id_prestamo;
    END IF;


    RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_estado_eliminado_prestamo_a_detalle() OWNER TO postgres;

--
-- TOC entry 307 (class 1255 OID 22905)
-- Name: fn_incrementar_cantidad_equipos(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_incrementar_cantidad_equipos() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
  -- Aumenta en 1 la cantidad en grupos_equipos
  UPDATE grupos_equipos
     SET cantidad = cantidad + 1
   WHERE id_grupo_equipo = NEW.id_grupo_equipo;
  RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_incrementar_cantidad_equipos() OWNER TO postgres;

--
-- TOC entry 329 (class 1255 OID 22906)
-- Name: fn_incrementar_numero_gaveteros(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_incrementar_numero_gaveteros() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
  UPDATE public.muebles
     SET numero_gaveteros = COALESCE(numero_gaveteros, 0) + 1
   WHERE id_mueble = NEW.id_mueble;

  RETURN NEW;
EXCEPTION
  WHEN OTHERS THEN
    RAISE EXCEPTION 'Error al actualizar numero_gaveteros para mueble %: %',
      NEW.id_mueble, SQLERRM;
END;
$$;


ALTER FUNCTION public.fn_incrementar_numero_gaveteros() OWNER TO postgres;

--
-- TOC entry 298 (class 1255 OID 22907)
-- Name: insertar_accesorios(character varying, character varying, character varying, integer, text, double precision, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_accesorios(IN p_nombre character varying, IN p_modelo character varying, IN p_tipo character varying, IN p_codigo_imt integer, IN p_descripcion text DEFAULT NULL::text, IN p_precio double precision DEFAULT NULL::double precision, IN p_url_data_sheet text DEFAULT NULL::text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_equipo INTEGER;
BEGIN
    SELECT id_equipo
      INTO v_id_equipo
      FROM equipos
     WHERE codigo_imt = p_codigo_imt
       AND estado_eliminado = FALSE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró el equipo con código IMT = %', p_codigo_imt;
    END IF;

    INSERT INTO accesorios(
        nombre,
        descripcion,
        modelo,
        url_data_sheet,
        precio,
        id_equipo,
        tipo,
        estado_eliminado
    )
    VALUES(
        p_nombre,
        p_descripcion,
        p_modelo,
        p_url_data_sheet,
        p_precio,
        v_id_equipo,
        p_tipo,
        FALSE
    );
    
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe un accesorio con esos datos.';
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar accesorio: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_accesorios(IN p_nombre character varying, IN p_modelo character varying, IN p_tipo character varying, IN p_codigo_imt integer, IN p_descripcion text, IN p_precio double precision, IN p_url_data_sheet text) OWNER TO postgres;

--
-- TOC entry 371 (class 1255 OID 22908)
-- Name: insertar_carrera(character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_carrera(IN p_nombre character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF trim(p_nombre) = '' THEN
        RAISE EXCEPTION 'El nombre de la carrera no puede estar vacío';
    END IF;

    -- Intentamos actualizar si existe una carrera eliminada lógicamente
    UPDATE public.carreras
    SET estado_eliminado = FALSE
    WHERE nombre = p_nombre AND estado_eliminado = TRUE;

    -- Si se actualizó alguna fila, terminamos el procedimiento
    IF FOUND THEN
        RETURN;
    END IF;

    -- Verificamos si ya existe una carrera activa con ese nombre
    IF EXISTS (
        SELECT 1 FROM public.carreras
        WHERE nombre = p_nombre AND estado_eliminado = FALSE
    ) THEN
        RAISE EXCEPTION 'Ya existe una carrera con el nombre "%" ', p_nombre;
    END IF;

    -- Insertamos si no existe ningún registro (ni activo ni eliminado)
    INSERT INTO public.carreras (
        nombre,
        estado_eliminado
    )
    VALUES (
        p_nombre,
        FALSE
    );

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar carrera: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_carrera(IN p_nombre character varying) OWNER TO postgres;

--
-- TOC entry 378 (class 1255 OID 22909)
-- Name: insertar_categoria(character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_categoria(IN p_nombre_raw character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_nombre TEXT := TRIM(both ' ' FROM p_nombre_raw);
BEGIN
    IF v_nombre = '' THEN
        RAISE EXCEPTION 'El nombre de la categoría no puede estar vacío';
    END IF;

    -- Intentar reactivar si existe una categoría eliminada lógicamente
    UPDATE public.categorias
    SET estado_eliminado = FALSE
    WHERE nombre = v_nombre AND estado_eliminado = TRUE;

    IF FOUND THEN
        RETURN;
    END IF;

    -- Verificar si ya existe una categoría activa con ese nombre
    IF EXISTS (
        SELECT 1
        FROM public.categorias
        WHERE nombre = v_nombre AND estado_eliminado = FALSE
    ) THEN
        RAISE EXCEPTION 'Ya existe una categoría con el nombre "%"', v_nombre;
    END IF;

    -- Insertar si no existe
    INSERT INTO public.categorias (
        nombre,
        estado_eliminado
    )
    VALUES (
        v_nombre,
        FALSE
    );

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe una categoría con el nombre "%"', v_nombre;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar categoría: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_categoria(IN p_nombre_raw character varying) OWNER TO postgres;

--
-- TOC entry 359 (class 1255 OID 22910)
-- Name: insertar_componente(character varying, character varying, character varying, integer, text, double precision, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_componente(IN p_nombre character varying, IN p_modelo character varying, IN p_tipo character varying, IN p_codigo_imt integer, IN p_descripcion text DEFAULT NULL::text, IN p_precio_referencia double precision DEFAULT NULL::double precision, IN p_url_data_sheet text DEFAULT NULL::text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_equipo INTEGER;
BEGIN
    SELECT id_equipo
      INTO v_id_equipo
      FROM equipos
     WHERE codigo_imt      = p_codigo_imt
       AND estado_eliminado = FALSE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró el equipo con código IMT = %', p_codigo_imt;
    END IF;

    INSERT INTO componentes (
        nombre,
        modelo,
        tipo,
        descripcion,
        url_data_sheet,
        precio_referencia,
        id_equipo,
        estado_eliminado
    )
    VALUES (
        p_nombre,
        p_modelo,
        p_tipo,
        p_descripcion,
        p_url_data_sheet,
        p_precio_referencia,
        v_id_equipo,
        FALSE
    );

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe un componente con esos datos.';
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar componente: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_componente(IN p_nombre character varying, IN p_modelo character varying, IN p_tipo character varying, IN p_codigo_imt integer, IN p_descripcion text, IN p_precio_referencia double precision, IN p_url_data_sheet text) OWNER TO postgres;

--
-- TOC entry 279 (class 1255 OID 22911)
-- Name: insertar_empresa_mantenimiento(character varying, character varying, character varying, character varying, text, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_empresa_mantenimiento(IN p_nombre character varying, IN p_nombre_responsable character varying, IN p_apellido_responsable character varying, IN p_telefono character varying, IN p_direccion text, IN p_nit character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_nombre_trimmed TEXT := TRIM(both ' ' FROM p_nombre);
BEGIN
    -- 1) Reactivar si ya había una empresa con ese nombre eliminada lógicamente
    UPDATE public.empresas_mantenimiento
       SET estado_eliminado = FALSE
     WHERE nombre = v_nombre_trimmed
       AND estado_eliminado = TRUE;
    IF FOUND THEN
        RETURN;
    END IF;

    -- 2) Verificar si ya existe una empresa activa con ese nombre
    IF EXISTS (
        SELECT 1
          FROM public.empresas_mantenimiento
         WHERE nombre = v_nombre_trimmed
           AND estado_eliminado = FALSE
    ) THEN
        RAISE EXCEPTION 'Ya existe una empresa de mantenimiento con esos datos.';
    END IF;

    -- 3) Insertar nueva empresa
    INSERT INTO public.empresas_mantenimiento (
        nombre,
        nombre_responsable,
        apellido_responsable,
        telefono,
        direccion,
        nit,
        estado_eliminado
    )
    VALUES (
        v_nombre_trimmed,
        p_nombre_responsable,
        p_apellido_responsable,
        p_telefono,
        p_direccion,
        p_nit,
        FALSE
    );

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe una empresa de mantenimiento con esos datos.';
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar empresa de mantenimiento: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_empresa_mantenimiento(IN p_nombre character varying, IN p_nombre_responsable character varying, IN p_apellido_responsable character varying, IN p_telefono character varying, IN p_direccion text, IN p_nit character varying) OWNER TO postgres;

--
-- TOC entry 323 (class 1255 OID 22912)
-- Name: insertar_equipo(character varying, character varying, character varying, character varying, text, character varying, character varying, character varying, double precision, integer, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_equipo(IN p_nombre_grupo_equipo character varying, IN p_modelo character varying, IN p_marca character varying, IN p_codigo_ucb character varying, IN p_descripcion text, IN p_numero_serial character varying, IN p_ubicacion character varying, IN p_procedencia character varying, IN p_costo_referencia double precision, IN p_tiempo_maximo_prestamo integer, IN p_nombre_gavetero character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_grupo_equipo INTEGER;
    v_id_gavetero     INTEGER;
    v_codigo_imt      INTEGER;
BEGIN
    SELECT ge.id_grupo_equipo
      INTO v_id_grupo_equipo
      FROM grupos_equipos AS ge
     WHERE ge.nombre           = p_nombre_grupo_equipo
	 	and ge.modelo			=p_modelo
		 and ge.marca 			=p_marca
       AND ge.estado_eliminado = FALSE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró el grupo de equipos con nombre = %', p_nombre_grupo_equipo;
    END IF;
	
    IF p_nombre_gavetero IS NOT NULL THEN
    	SELECT g.id_gavetero
      INTO v_id_gavetero
      FROM gaveteros AS g
     WHERE g.nombre = p_nombre_gavetero
       AND g.estado_eliminado = FALSE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontro el gavetero con nombre = %', p_nombre_gavetero;
    END IF;
	ELSE
    v_id_gavetero := NULL;
	END IF;

    v_codigo_imt := public.obtener_codigo_imt(v_id_grupo_equipo);

    INSERT INTO equipos (
        codigo_imt,
        codigo_ucb,
        descripcion,
        numero_serial,
        ubicacion,
        procedencia,
        costo_referencia,
        tiempo_max_prestamo,
        id_gavetero,
        id_grupo_equipo,
        estado_eliminado
    )
    VALUES (
        v_codigo_imt,
        p_codigo_ucb,
        p_descripcion,
        p_numero_serial,
        p_ubicacion,
        p_procedencia,
        p_costo_referencia,
        p_tiempo_maximo_prestamo,
        v_id_gavetero,
        v_id_grupo_equipo,
        FALSE
    );

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe un equipo con ese código UCB o número serial.';
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar equipo: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_equipo(IN p_nombre_grupo_equipo character varying, IN p_modelo character varying, IN p_marca character varying, IN p_codigo_ucb character varying, IN p_descripcion text, IN p_numero_serial character varying, IN p_ubicacion character varying, IN p_procedencia character varying, IN p_costo_referencia double precision, IN p_tiempo_maximo_prestamo integer, IN p_nombre_gavetero character varying) OWNER TO postgres;

--
-- TOC entry 375 (class 1255 OID 22913)
-- Name: insertar_gavetero(character varying, character varying, character varying, double precision, double precision, double precision); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_gavetero(IN p_nombre character varying, IN p_tipo character varying, IN p_nombre_mueble character varying, IN p_longitud double precision, IN p_profundidad double precision, IN p_altura double precision)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_mueble INTEGER;
BEGIN
    SELECT m.id_mueble
      INTO v_id_mueble
      FROM muebles AS m
     WHERE m.nombre           = p_nombre_mueble
       AND m.estado_eliminado = FALSE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró el mueble con nombre = %', p_nombre_mueble;
    END IF;

    IF EXISTS (
        SELECT 1
          FROM gaveteros
         WHERE nombre = p_nombre
           AND estado_eliminado = FALSE
    ) THEN
        RAISE EXCEPTION 'Ya existe un gavetero con nombre = %', p_nombre;
    END IF;

    INSERT INTO gaveteros (
        nombre,
        tipo,
        id_mueble,
        longitud,
        profundidad,
        altura,
        estado_eliminado
    )
    VALUES (
        p_nombre,
        p_tipo,
        v_id_mueble,
        p_longitud,
        p_profundidad,
        p_altura,
        FALSE
    );

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Violación de unicidad al intentar insertar gavetero: %', p_nombre;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar gavetero: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_gavetero(IN p_nombre character varying, IN p_tipo character varying, IN p_nombre_mueble character varying, IN p_longitud double precision, IN p_profundidad double precision, IN p_altura double precision) OWNER TO postgres;

--
-- TOC entry 317 (class 1255 OID 22914)
-- Name: insertar_grupo_equipo(character varying, character varying, character varying, text, character varying, text, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_grupo_equipo(IN p_nombre character varying, IN p_modelo character varying, IN p_marca character varying, IN p_descripcion text, IN p_nombre_categoria character varying, IN p_url_data_sheet text, IN p_url_imagen text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_categoria INTEGER;
BEGIN
    SELECT c.id_categoria
      INTO v_id_categoria
      FROM categorias AS c
     WHERE c.nombre           = p_nombre_categoria
       AND c.estado_eliminado = FALSE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró la categoría con nombre = %', p_nombre_categoria;
    END IF;

    IF EXISTS (
        SELECT 1
          FROM grupos_equipos AS ge
         WHERE ge.nombre            = p_nombre
           AND ge.modelo            = p_modelo
           AND ge.marca             = p_marca
           AND ge.estado_eliminado  = FALSE
    ) THEN
        RAISE EXCEPTION 'Ya existe un grupo de equipos con nombre = %, modelo = %, marca = %',
            p_nombre, p_modelo, p_marca;
    END IF;

    INSERT INTO grupos_equipos (
        nombre,
        modelo,
        marca,
        descripcion,
        id_categoria,
        url_data_sheet,
        url_imagen,
        estado_eliminado
    )
    VALUES (
        p_nombre,
        p_modelo,
        p_marca,
        p_descripcion,
        v_id_categoria,
        p_url_data_sheet,
        p_url_imagen,
        FALSE
    );

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Violación de unicidad al intentar insertar grupo de equipos: % / % / %',
            p_nombre, p_modelo, p_marca;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar grupo de equipos: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_grupo_equipo(IN p_nombre character varying, IN p_modelo character varying, IN p_marca character varying, IN p_descripcion text, IN p_nombre_categoria character varying, IN p_url_data_sheet text, IN p_url_imagen text) OWNER TO postgres;

--
-- TOC entry 361 (class 1255 OID 22915)
-- Name: insertar_mantenimiento(date, date, character varying, double precision, text, integer[], character varying[], text[]); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_mantenimiento(IN p_fecha_mantenimiento date, IN p_fecha_final_mantenimiento date, IN p_nombre_empresa character varying, IN p_costo double precision, IN p_descripcion text, IN p_codigos_imt integer[], IN p_tipos_mantenimiento character varying[], IN p_descripciones_equipo text[])
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_mantenimiento         INTEGER;
    v_id_empresa_mantenimiento INTEGER;
    v_id_equipo                INTEGER;
    i                          INTEGER;
BEGIN
    SELECT em.id_empresa_mantenimiento
      INTO v_id_empresa_mantenimiento
      FROM empresas_mantenimiento AS em
     WHERE em.nombre           = p_nombre_empresa
       AND em.estado_eliminado = FALSE;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró la empresa con nombre = %', p_nombre_empresa;
    END IF;

    INSERT INTO mantenimientos (
        fecha_mantenimiento,
        fecha_final_mantenimiento,
        id_empresa,
        costo,
        descripcion,
        estado_eliminado
    )
    VALUES (
        p_fecha_mantenimiento,
        p_fecha_final_mantenimiento,
        v_id_empresa_mantenimiento,
        p_costo,
        p_descripcion,
        FALSE
    )
    RETURNING id_mantenimiento
    INTO v_id_mantenimiento;

    IF array_length(p_codigos_imt, 1) IS NULL
       OR array_length(p_codigos_imt, 1) <> array_length(p_tipos_mantenimiento, 1)
       OR array_length(p_codigos_imt, 1) <> array_length(p_descripciones_equipo, 1)
    THEN
        RAISE EXCEPTION 'Los arreglos de equipos deben tener la misma longitud';
    END IF;

    FOR i IN 1..array_length(p_codigos_imt, 1) LOOP
        SELECT e.id_equipo
          INTO v_id_equipo
          FROM equipos AS e
         WHERE e.codigo_imt       = p_codigos_imt[i]
           AND e.estado_eliminado = FALSE;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'Equipo no encontrado con código IMT = %', p_codigos_imt[i];
        END IF;

        INSERT INTO detalles_mantenimientos (
            id_mantenimiento,
            id_equipo,
            tipo_mantenimiento,
            descripcion,
            estado_eliminado
        )
        VALUES (
            v_id_mantenimiento,
            v_id_equipo,
            p_tipos_mantenimiento[i],  
            p_descripciones_equipo[i],
            FALSE
        );
    END LOOP;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Violación de unicidad al insertar mantenimiento';
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar mantenimiento: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_mantenimiento(IN p_fecha_mantenimiento date, IN p_fecha_final_mantenimiento date, IN p_nombre_empresa character varying, IN p_costo double precision, IN p_descripcion text, IN p_codigos_imt integer[], IN p_tipos_mantenimiento character varying[], IN p_descripciones_equipo text[]) OWNER TO postgres;

--
-- TOC entry 330 (class 1255 OID 22916)
-- Name: insertar_mueble(character varying, character varying, double precision, character varying, double precision, double precision, double precision); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_mueble(IN p_nombre character varying, IN p_tipo character varying, IN p_costo double precision, IN p_ubicacion character varying, IN p_longitud double precision, IN p_profundidad double precision, IN p_altura double precision)
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO muebles (
        nombre,
        tipo,
        costo,
        ubicacion,
        longitud,
        profundidad,
        altura,
        estado_eliminado
    )
    VALUES (
        p_nombre,
        p_tipo,
        p_costo,
        p_ubicacion,
        p_longitud,
        p_profundidad,
        p_altura,
        FALSE
    );
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Ya existe un mueble con el mismo nombre.';
   	WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al insertar mueble: %', SQLERRM;
END;
$$;


ALTER PROCEDURE public.insertar_mueble(IN p_nombre character varying, IN p_tipo character varying, IN p_costo double precision, IN p_ubicacion character varying, IN p_longitud double precision, IN p_profundidad double precision, IN p_altura double precision) OWNER TO postgres;

--
-- TOC entry 331 (class 1255 OID 22917)
-- Name: insertar_prestamo(integer[], timestamp without time zone, timestamp without time zone, text, character varying, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_prestamo(IN id_grupos_equipo_input integer[], IN fecha_prestamo_esperada_input timestamp without time zone, IN fecha_devolucion_esperada_input timestamp without time zone, IN observacion_input text, IN carnet_input character varying, IN id_contrato_input text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    id_prestamo_nuevo integer;
    id_grupo integer;
    id_equipo_disponible integer;
    equipos_ya_asignados_a_este_prestamo integer[] := ARRAY[]::integer[];
    contador integer := 0;
BEGIN
    -- Primera pasada: verificar disponibilidad para todos los grupos
    FOREACH id_grupo IN ARRAY id_grupos_equipo_input LOOP
        SELECT COUNT(e.id_equipo) INTO contador
        FROM public.equipos e
        WHERE e.id_grupo_equipo = id_grupo
          AND e.estado_eliminado = FALSE
          AND e.estado_equipo = 'operativo'
          AND NOT EXISTS (
              SELECT 1 FROM public.detalles_prestamos dp
              INNER JOIN public.prestamos p ON dp.id_prestamo = p.id_prestamo
              WHERE dp.id_equipo = e.id_equipo
                AND p.estado_eliminado = FALSE
                AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                AND fecha_prestamo_esperada_input::date BETWEEN p.fecha_prestamo_esperada::date AND p.fecha_devolucion_esperada::date
          )
          AND NOT EXISTS (
              SELECT 1 FROM public.detalles_mantenimientos dm
              INNER JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
              WHERE dm.id_equipo = e.id_equipo
                AND m.estado_eliminado = FALSE
                AND fecha_prestamo_esperada_input::date BETWEEN m.fecha_mantenimiento AND m.fecha_final_mantenimiento
          );

        IF contador = 0 THEN
            RAISE EXCEPTION 'No hay equipos disponibles para el grupo %', id_grupo;
        END IF;
    END LOOP;

    -- Insertar el préstamo principal
    INSERT INTO public.prestamos (
        carnet,
        fecha_solicitud,
        fecha_prestamo_esperada,
        fecha_devolucion_esperada,
        observacion,
        estado_prestamo,
        id_contrato,
        estado_eliminado
    )
    VALUES (
        carnet_input,
        (now() AT TIME ZONE 'America/La_Paz')::timestamp without time zone,
        fecha_prestamo_esperada_input,
        fecha_devolucion_esperada_input,
        observacion_input,
        'pendiente'::estado_prestamo,
        id_contrato_input,
        FALSE
    )
    RETURNING id_prestamo INTO id_prestamo_nuevo;

    -- Segunda pasada: asignar equipos específicos
    FOREACH id_grupo IN ARRAY id_grupos_equipo_input LOOP
        SELECT e.id_equipo INTO id_equipo_disponible
        FROM public.equipos e
        WHERE e.id_grupo_equipo = id_grupo
          AND e.estado_eliminado = FALSE
          AND e.estado_equipo = 'operativo'
          AND NOT (e.id_equipo = ANY(equipos_ya_asignados_a_este_prestamo))
          AND NOT EXISTS (
              SELECT 1 FROM public.detalles_prestamos dp
              INNER JOIN public.prestamos p ON dp.id_prestamo = p.id_prestamo
              WHERE dp.id_equipo = e.id_equipo
                AND p.estado_eliminado = FALSE
                AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                AND fecha_prestamo_esperada_input::date BETWEEN p.fecha_prestamo_esperada::date AND p.fecha_devolucion_esperada::date
          )
          AND NOT EXISTS (
              SELECT 1 FROM public.detalles_mantenimientos dm
              INNER JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
              WHERE dm.id_equipo = e.id_equipo
                AND m.estado_eliminado = FALSE
                AND fecha_prestamo_esperada_input::date BETWEEN m.fecha_mantenimiento AND m.fecha_final_mantenimiento
          )
        LIMIT 1;

        IF id_equipo_disponible IS NOT NULL THEN
            INSERT INTO public.detalles_prestamos (id_prestamo, id_equipo, estado_eliminado)
            VALUES (id_prestamo_nuevo, id_equipo_disponible, FALSE);

            equipos_ya_asignados_a_este_prestamo := array_append(equipos_ya_asignados_a_este_prestamo, id_equipo_disponible);
        END IF;
    END LOOP;
END;
$$;


ALTER PROCEDURE public.insertar_prestamo(IN id_grupos_equipo_input integer[], IN fecha_prestamo_esperada_input timestamp without time zone, IN fecha_devolucion_esperada_input timestamp without time zone, IN observacion_input text, IN carnet_input character varying, IN id_contrato_input text) OWNER TO postgres;

--
-- TOC entry 339 (class 1255 OID 22918)
-- Name: insertar_prestamo(integer[], timestamp with time zone, timestamp with time zone, text, character varying, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_prestamo(IN id_grupos_equipo_input integer[], IN fecha_prestamo_esperada_input timestamp with time zone, IN fecha_devolucion_esperada_input timestamp with time zone, IN observacion_input text, IN carnet_input character varying, IN id_contrato_input text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    id_grupo integer;
    id_equipo_disponible integer;
    id_prestamo_general integer;  
    equipos_ya_asignados_a_este_prestamo integer[] := ARRAY[]::integer[];
    equipo_tmp integer;
    dias_solicitados integer;
    tiempo_maximo_permitido integer;
BEGIN
    -- Calcular días solicitados
    dias_solicitados := (fecha_devolucion_esperada_input::date - fecha_prestamo_esperada_input::date);
    
    -- PRIMERA PASADA: Verificación de disponibilidad para todos los grupos
    FOREACH id_grupo IN ARRAY id_grupos_equipo_input LOOP
        IF NOT EXISTS(
            SELECT 1 
              FROM public.grupos_equipos ge 
             WHERE ge.id_grupo_equipo = id_grupo
               AND ge.estado_eliminado = FALSE
        ) THEN
            RAISE EXCEPTION 'Grupo ID % no existe o está eliminado. No se puede continuar con la asignación de equipos para este grupo.', id_grupo;
        END IF;

        SELECT e.id_equipo
          INTO equipo_tmp
          FROM public.equipos e
         WHERE e.id_grupo_equipo = id_grupo
           AND e.estado_eliminado = FALSE
           AND e.estado_equipo = 'operativo'
           AND dias_solicitados <= e.tiempo_max_prestamo
           AND NOT EXISTS ( 
               SELECT 1
                 FROM public.detalles_prestamos dp
                 JOIN public.prestamos pr ON dp.id_prestamo = pr.id_prestamo
                WHERE dp.id_equipo = e.id_equipo
                  AND pr.estado_eliminado = FALSE
                  AND pr.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                  AND (
                      (fecha_prestamo_esperada_input < pr.fecha_devolucion_esperada AND fecha_devolucion_esperada_input > pr.fecha_prestamo_esperada)
                  )
           )
           AND NOT EXISTS ( 
               SELECT 1
                 FROM public.detalles_mantenimientos dm
                 JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
                WHERE dm.id_equipo = e.id_equipo
                  AND m.estado_eliminado = FALSE
                  AND (
                      (fecha_prestamo_esperada_input < m.fecha_final_mantenimiento AND fecha_devolucion_esperada_input > m.fecha_mantenimiento)
                  )
           )
         ORDER BY e.id_equipo 
         LIMIT 1;

        IF equipo_tmp IS NULL THEN
            RAISE EXCEPTION 'No se encontró equipo disponible para el grupo ID % en las fechas solicitadas (o los equipos exceden el tiempo máximo de préstamo permitido).', id_grupo;
        END IF;
    END LOOP;

    -- SEGUNDA PASADA: Inserción del préstamo y detalles
    BEGIN
        INSERT INTO public.prestamos (
            fecha_prestamo_esperada,
            fecha_devolucion_esperada,
            observacion,
            carnet,
            id_contrato
        ) VALUES (
            fecha_prestamo_esperada_input,
            fecha_devolucion_esperada_input,
            observacion_input,
            carnet_input,
            id_contrato_input
        )
        RETURNING id_prestamo INTO id_prestamo_general;

    EXCEPTION
        WHEN unique_violation THEN
            RAISE EXCEPTION 'Error al crear préstamo general: Conflicto de llave única. %', SQLERRM;
        WHEN OTHERS THEN
            RAISE EXCEPTION 'Error inesperado (%s) al crear el préstamo general: %', SQLSTATE, SQLERRM;
    END;

    IF id_prestamo_general IS NULL THEN
        RAISE EXCEPTION 'Fallo crítico: No se pudo obtener el ID del préstamo general creado.';
    END IF;

    FOREACH id_grupo IN ARRAY id_grupos_equipo_input LOOP
        id_equipo_disponible := NULL; 

        SELECT e.id_equipo
          INTO id_equipo_disponible
          FROM public.equipos e
         WHERE e.id_grupo_equipo = id_grupo
           AND e.estado_eliminado = FALSE
           AND e.estado_equipo = 'operativo'
           AND dias_solicitados <= e.tiempo_max_prestamo
           AND (equipos_ya_asignados_a_este_prestamo IS NULL OR NOT (e.id_equipo = ANY(equipos_ya_asignados_a_este_prestamo)))
           AND NOT EXISTS ( 
               SELECT 1
                 FROM public.detalles_prestamos dp
                 JOIN public.prestamos pr ON dp.id_prestamo = pr.id_prestamo
                WHERE dp.id_equipo = e.id_equipo
                  AND pr.id_prestamo <> id_prestamo_general 
                  AND pr.estado_eliminado = FALSE
                  AND pr.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                  AND (
                      (fecha_prestamo_esperada_input < pr.fecha_devolucion_esperada AND fecha_devolucion_esperada_input > pr.fecha_prestamo_esperada)
                  )
           )
           AND NOT EXISTS ( 
               SELECT 1
                 FROM public.detalles_mantenimientos dm
                 JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
                WHERE dm.id_equipo = e.id_equipo
                  AND m.estado_eliminado = FALSE
                  AND (
                      (fecha_prestamo_esperada_input < m.fecha_final_mantenimiento AND fecha_devolucion_esperada_input > m.fecha_mantenimiento)
                  )
           )
         ORDER BY e.id_equipo 
         LIMIT 1;

        IF id_equipo_disponible IS NULL THEN
            RAISE EXCEPTION 'No se encontró equipo disponible para el grupo ID % en las fechas solicitadas (o los equipos exceden el tiempo máximo de préstamo permitido).', id_grupo;
        END IF;

        BEGIN
            INSERT INTO public.detalles_prestamos (
                id_prestamo,
                id_equipo
            ) VALUES (
                id_prestamo_general, 
                id_equipo_disponible
            );

             equipos_ya_asignados_a_este_prestamo := array_append(equipos_ya_asignados_a_este_prestamo, id_equipo_disponible);

        EXCEPTION
            WHEN unique_violation THEN
                RAISE EXCEPTION 'Conflicto de llave única al crear detalle para PréstamoID %, GrupoID %, EquipoID %: %. Esto podría indicar un intento de asignar el mismo equipo dos veces si la tabla detalles_prestamos tiene una restricción de unicidad en (id_prestamo, id_equipo).',
                    id_prestamo_general, id_grupo, id_equipo_disponible, SQLERRM;
            WHEN check_violation THEN
                RAISE EXCEPTION 'Violación de restricción CHECK al crear detalle para PréstamoID %, GrupoID %, EquipoID %: %.',
                    id_prestamo_general, id_grupo, id_equipo_disponible, SQLERRM;
            WHEN foreign_key_violation THEN
                RAISE EXCEPTION 'Violación de llave foránea al crear detalle para PréstamoID %, GrupoID %, EquipoID %: %.',
                    id_prestamo_general, id_grupo, id_equipo_disponible, SQLERRM;
            WHEN OTHERS THEN
                RAISE EXCEPTION 'Error inesperado (%s) al crear detalle para PréstamoID %, GrupoID %, EquipoID %: %.',
                    SQLSTATE, id_prestamo_general, id_grupo, id_equipo_disponible, SQLERRM;
        END; 
    END LOOP; 
END;
$$;


ALTER PROCEDURE public.insertar_prestamo(IN id_grupos_equipo_input integer[], IN fecha_prestamo_esperada_input timestamp with time zone, IN fecha_devolucion_esperada_input timestamp with time zone, IN observacion_input text, IN carnet_input character varying, IN id_contrato_input text) OWNER TO postgres;

--
-- TOC entry 344 (class 1255 OID 22920)
-- Name: insertar_usuario(character varying, character varying, character varying, character varying, public.tipo_usuario, character varying, text, character varying, character varying, character varying, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insertar_usuario(IN carnet_input character varying, IN nombre_input character varying, IN apellido_paterno_input character varying, IN apellido_materno_input character varying, IN rol_input public.tipo_usuario, IN email_input character varying, IN contrasena_input text, IN carrera_input character varying, IN telefono_input character varying, IN telefono_referencia_input character varying, IN nombre_referencia_input character varying, IN email_referencia_input character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DECLARE
        id_carrera_input INTEGER;
    BEGIN
        SELECT
            id_carrera INTO id_carrera_input
        FROM
            carreras as c
        WHERE
            nombre = carrera_input and
			c.estado_eliminado=false;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'No se encontró la carrera con nombre: %',
            carrera_input;
        END IF;
        INSERT INTO usuarios (
            carnet,
            nombre,
            apellido_paterno,
            apellido_materno,
            rol,
            email,
            contrasena,
            id_carrera,
            telefono,
            telefono_referencia,
            nombre_referencia,
            email_referencia,
            estado_eliminado
        )
        VALUES
            (
                carnet_input,
                nombre_input,
                apellido_paterno_input,
                apellido_materno_input,
                rol_input,
                email_input,
                contrasena_input,
                id_carrera_input,
                telefono_input,
                telefono_referencia_input,
                nombre_referencia_input,
                email_referencia_input,
                FALSE
            );
    EXCEPTION
        WHEN UNIQUE_VIOLATION THEN
            RAISE EXCEPTION 'Error: El carnet o email ya está registrado en la base de datos.';
        WHEN OTHERS THEN
            RAISE EXCEPTION 'Hubo un error inesperado durante el proceso de inserción: %',
            SQLERRM;
    END;
END;
$$;


ALTER PROCEDURE public.insertar_usuario(IN carnet_input character varying, IN nombre_input character varying, IN apellido_paterno_input character varying, IN apellido_materno_input character varying, IN rol_input public.tipo_usuario, IN email_input character varying, IN contrasena_input text, IN carrera_input character varying, IN telefono_input character varying, IN telefono_referencia_input character varying, IN nombre_referencia_input character varying, IN email_referencia_input character varying) OWNER TO postgres;

--
-- TOC entry 355 (class 1255 OID 22921)
-- Name: insertar_y_obtener_prestamo(integer[], timestamp without time zone, timestamp without time zone, text, character varying, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.insertar_y_obtener_prestamo(id_grupos_equipo_input integer[], fecha_prestamo_esperada_input timestamp without time zone, fecha_devolucion_esperada_input timestamp without time zone, observacion_input text, carnet_input character varying, id_contrato_input text) RETURNS TABLE(id_prestamo integer, id_equipo integer, codigo_imt character varying, codigo_serial character varying, nombre character varying, modelo character varying, marca character varying, id_grupo_equipo integer)
    LANGUAGE plpgsql
    AS $$
DECLARE
    id_grupo_actual integer;
    id_equipo_disponible integer;
    id_prestamo_general integer;
    dias_solicitados integer;
    equipos_asignados integer[] := ARRAY[]::integer[];
    fecha_prestamo_ajustada timestamp without time zone;
    fecha_devolucion_ajustada timestamp without time zone;
BEGIN
    -- VALIDACIONES INICIALES
    -- Validar que el usuario existe
    IF NOT EXISTS(SELECT 1 FROM public.usuarios WHERE carnet = carnet_input AND estado_eliminado = FALSE) THEN
        RAISE EXCEPTION 'Usuario con carnet % no existe o está eliminado.', carnet_input;
    END IF;

    -- Las fechas se usan tal cual vienen del frontend
    -- Si fecha_inicio = fecha_fin → 1 día
    -- Si fecha_fin = fecha_inicio + 1 → 2 días
    -- etc.
    fecha_prestamo_ajustada := fecha_prestamo_esperada_input;
    fecha_devolucion_ajustada := fecha_devolucion_esperada_input;

    -- Validar que la fecha de devolución no sea anterior a la de préstamo
    IF fecha_devolucion_ajustada::date < fecha_prestamo_ajustada::date THEN
        RAISE EXCEPTION 'La fecha de devolución no puede ser anterior a la fecha de préstamo.';
    END IF;

    -- Calcular días solicitados (diferencia + 1 = días totales ocupados)
    dias_solicitados := (fecha_devolucion_ajustada::date - fecha_prestamo_ajustada::date) + 1;

    -- REMOVIDO: Verificación de solapamiento por grupo (causaba rechazos innecesarios)
    -- Ahora confiamos en la búsqueda de equipos libres para manejar conflictos

    -- Crear el préstamo
    INSERT INTO public.prestamos (
        fecha_prestamo_esperada,
        fecha_devolucion_esperada,
        observacion,
        carnet,
        id_contrato
    ) VALUES (
        fecha_prestamo_ajustada,
        fecha_devolucion_ajustada,
        observacion_input,
        carnet_input,
        id_contrato_input
    )
    RETURNING prestamos.id_prestamo INTO id_prestamo_general;

    -- Para cada grupo, encontrar y asignar un equipo disponible
    FOREACH id_grupo_actual IN ARRAY id_grupos_equipo_input LOOP
        -- Validar que el grupo existe (mover aquí, después de crear préstamo)
        IF NOT EXISTS(SELECT 1 FROM public.grupos_equipos ge WHERE ge.id_grupo_equipo = id_grupo_actual AND ge.estado_eliminado = FALSE) THEN
            -- Eliminar el préstamo creado si falla
            DELETE FROM public.prestamos WHERE prestamos.id_prestamo = id_prestamo_general;
            RAISE EXCEPTION 'Grupo de equipo ID % no existe o está eliminado.', id_grupo_actual;
        END IF;

        -- Buscar equipo disponible para este grupo
        SELECT e.id_equipo
        INTO id_equipo_disponible
        FROM public.equipos e
        WHERE e.id_grupo_equipo = id_grupo_actual
          AND e.estado_eliminado = FALSE
          AND e.estado_equipo = 'operativo'
          AND dias_solicitados <= COALESCE(e.tiempo_max_prestamo, 9999)
          -- Excluir equipos ya asignados en este préstamo
          AND NOT (e.id_equipo = ANY(equipos_asignados))
          -- Excluir equipos ocupados en otros préstamos (verificación diaria)
          AND NOT EXISTS (
              SELECT 1
              FROM generate_series(fecha_prestamo_ajustada::date, fecha_devolucion_ajustada::date, INTERVAL '1 day') AS gs(fecha_dia)
              WHERE EXISTS (
                  SELECT 1
                  FROM public.detalles_prestamos dp
                  INNER JOIN public.prestamos pr ON dp.id_prestamo = pr.id_prestamo
                  WHERE dp.id_equipo = e.id_equipo
                    AND pr.estado_eliminado = FALSE
                    AND pr.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                    AND gs.fecha_dia BETWEEN pr.fecha_prestamo_esperada::date AND pr.fecha_devolucion_esperada::date
              )
          )
          -- Excluir equipos en mantenimiento
          AND NOT EXISTS (
              SELECT 1
              FROM public.detalles_mantenimientos dm
              JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
              WHERE dm.id_equipo = e.id_equipo
                AND m.estado_eliminado = FALSE
                AND (fecha_prestamo_ajustada::date <= m.fecha_final_mantenimiento
                     AND fecha_devolucion_ajustada::date >= m.fecha_mantenimiento)
          )
        ORDER BY e.id_equipo
        LIMIT 1;

        -- Si no hay equipo disponible, hacer rollback y error
        IF id_equipo_disponible IS NULL THEN
            -- Eliminar el préstamo creado (cascade eliminará detalles)
            DELETE FROM public.prestamos WHERE prestamos.id_prestamo = id_prestamo_general;
            RAISE EXCEPTION 'No se encontró equipo disponible para el grupo ID % en las fechas solicitadas.', id_grupo_actual;
        END IF;

        -- Asignar el equipo al préstamo
        INSERT INTO public.detalles_prestamos (id_prestamo, id_equipo)
        VALUES (id_prestamo_general, id_equipo_disponible);

        -- Agregar a la lista de equipos asignados
        equipos_asignados := array_append(equipos_asignados, id_equipo_disponible);
    END LOOP;

    -- RETORNAR los equipos asignados con sus datos
    RETURN QUERY
    SELECT
        dp.id_prestamo,
        e.id_equipo,
        e.codigo_imt::character varying,
        e.numero_serial,
        ge.nombre,
        ge.modelo,
        ge.marca,
        e.id_grupo_equipo
    FROM public.detalles_prestamos dp
    INNER JOIN public.equipos e ON dp.id_equipo = e.id_equipo
    INNER JOIN public.grupos_equipos ge ON ge.id_grupo_equipo = e.id_grupo_equipo
    WHERE dp.id_prestamo = id_prestamo_general
    ORDER BY e.id_grupo_equipo, e.id_equipo;
END;
$$;


ALTER FUNCTION public.insertar_y_obtener_prestamo(id_grupos_equipo_input integer[], fecha_prestamo_esperada_input timestamp without time zone, fecha_devolucion_esperada_input timestamp without time zone, observacion_input text, carnet_input character varying, id_contrato_input text) OWNER TO postgres;

--
-- TOC entry 379 (class 1255 OID 22923)
-- Name: obtener_accesorios(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_accesorios() RETURNS TABLE(id_accesorio integer, nombre_accesorio character varying, modelo_accesorio character varying, tipo_accesorio character varying, precio_accesorio double precision, nombre_equipo_asociado character varying, codigo_imt_equipo_asociado integer, descripcion_accesorio text, url_data_sheet_accesorio text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        a.id_accesorio,
        a.nombre AS nombre_accesorio,
        a.modelo AS modelo_accesorio,
        a.tipo AS tipo_accesorio,
        a.precio AS precio_accesorio,
        ge.nombre AS nombre_equipo_asociado,   
        e.codigo_imt AS codigo_imt_equipo_asociado, 
        a.descripcion AS descripcion_accesorio,
		a.url_data_sheet as url_data_sheet_accesorio
    FROM
        public.accesorios AS a
    LEFT JOIN 
        public.equipos AS e ON e.id_equipo = a.id_equipo
                           AND e.estado_eliminado = FALSE
    LEFT JOIN
        public.grupos_equipos AS ge ON ge.id_grupo_equipo = e.id_grupo_equipo
                                  AND ge.estado_eliminado = FALSE
    WHERE
        a.estado_eliminado = FALSE; 
END;
$$;


ALTER FUNCTION public.obtener_accesorios() OWNER TO postgres;

--
-- TOC entry 362 (class 1255 OID 22924)
-- Name: obtener_carreras(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_carreras() RETURNS TABLE(id_carrera integer, nombre_carrera character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		c.id_carrera,
        c.nombre -- Referencing the 'nombre' column from the 'carreras' table
    FROM
        public.carreras AS c -- Aliasing the table for clarity, though not strictly necessary for a single table
    WHERE
        c.estado_eliminado = FALSE;
END;
$$;


ALTER FUNCTION public.obtener_carreras() OWNER TO postgres;

--
-- TOC entry 382 (class 1255 OID 22925)
-- Name: obtener_categorias(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_categorias() RETURNS TABLE(id_categoria integer, categoria character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
	SELECT 
	c.id_categoria,
	c.nombre
	from categorias as c
	where c.estado_eliminado=false;
END;
$$;


ALTER FUNCTION public.obtener_categorias() OWNER TO postgres;

--
-- TOC entry 327 (class 1255 OID 22926)
-- Name: obtener_codigo_imt(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_codigo_imt(p_id_grupo_equipo integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_id_categoria  integer;
    v_max_sufijo    integer;
    v_codigo_imt    integer;
BEGIN
    -- 1) Obtener la categoría del grupo
    SELECT ge.id_categoria
      INTO v_id_categoria
      FROM grupos_equipos AS ge
     WHERE ge.id_grupo_equipo = p_id_grupo_equipo;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró el grupo de equipos con id = %', p_id_grupo_equipo;
    END IF;

    -- 2) Calcular el mayor sufijo ya usado para esa categoría
    SELECT COALESCE(MAX(e.codigo_imt % 10000000), 0)
      INTO v_max_sufijo
      FROM grupos_equipos AS ge
      JOIN equipos          AS e
        ON e.id_grupo_equipo = ge.id_grupo_equipo
     WHERE ge.id_categoria = v_id_categoria;

    -- 3) Generar el siguiente código IMT
    v_codigo_imt := v_id_categoria * 10000000 + (v_max_sufijo + 1);

    RETURN v_codigo_imt;
END;
$$;


ALTER FUNCTION public.obtener_codigo_imt(p_id_grupo_equipo integer) OWNER TO postgres;

--
-- TOC entry 308 (class 1255 OID 22927)
-- Name: obtener_componentes(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_componentes() RETURNS TABLE(id_componente integer, nombre_componente character varying, modelo_componente character varying, tipo_componente character varying, descripcion_componente text, precio_referencia_componente double precision, nombre_equipo character varying, codigo_imt_equipo integer, url_data_sheet_equipo text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		c.id_componente,
        c.nombre,
        c.modelo,
        c.tipo,
        c.descripcion,
        c.precio_referencia,
        ge.nombre AS nombre_equipo, 
        e.codigo_imt,
		c.url_data_sheet
    FROM
        public.componentes AS c
    left JOIN
        public.equipos AS e 
		ON c.id_equipo = e.id_equipo
		and e.estado_eliminado=false
    left JOIN
        public.grupos_equipos AS ge 
		ON ge.id_grupo_equipo = e.id_grupo_equipo
		and ge.estado_eliminado=false
    WHERE
        c.estado_eliminado = FALSE; 
                                  
END;
$$;


ALTER FUNCTION public.obtener_componentes() OWNER TO postgres;

--
-- TOC entry 349 (class 1255 OID 22928)
-- Name: obtener_disponibilidad_equipos_por_fechas_y_id_grupos_equipos(timestamp without time zone, timestamp without time zone, integer[]); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_disponibilidad_equipos_por_fechas_y_id_grupos_equipos(fecha_inicio timestamp without time zone, fecha_fin timestamp without time zone, p_array_ids integer[]) RETURNS TABLE(fecha timestamp without time zone, id_grupo_equipo integer, cantidad_disponible bigint)
    LANGUAGE plpgsql
    AS $$
DECLARE
    fecha_actual date;
    grupo_id integer;
    disponibilidad bigint;
    dias_solicitados integer;
BEGIN
    -- Calcular días solicitados (para verificación de tiempo_max_prestamo)
    dias_solicitados := (fecha_fin::date - fecha_inicio::date) + 1;

    -- Iterar por cada día en el rango solicitado (de inicio a fin, ambos inclusivos)
    FOR fecha_actual IN SELECT generate_series(fecha_inicio::date, fecha_fin::date, INTERVAL '1 day')::date LOOP
        -- Iterar por cada grupo en el array
        FOREACH grupo_id IN ARRAY p_array_ids LOOP

            -- Contar equipos disponibles: total operativos - ocupados - en mantenimiento
            SELECT COUNT(*)
            INTO disponibilidad
            FROM public.equipos e
            WHERE e.id_grupo_equipo = grupo_id
              AND e.estado_eliminado = FALSE
              AND e.estado_equipo = 'operativo'
              AND dias_solicitados <= COALESCE(e.tiempo_max_prestamo, 9999)
              -- Excluir equipos ocupados en reservas activas ese día
              AND NOT EXISTS (
                  SELECT 1
                  FROM public.detalles_prestamos dp
                  INNER JOIN public.prestamos p ON dp.id_prestamo = p.id_prestamo
                  WHERE dp.id_equipo = e.id_equipo
                    AND p.estado_eliminado = FALSE
                    AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                    AND fecha_actual BETWEEN p.fecha_prestamo_esperada::date AND p.fecha_devolucion_esperada::date
              )
              -- Excluir equipos en mantenimiento ese día
              AND NOT EXISTS (
                  SELECT 1
                  FROM public.detalles_mantenimientos dm
                  INNER JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
                  WHERE dm.id_equipo = e.id_equipo
                    AND m.estado_eliminado = FALSE
                    AND fecha_actual BETWEEN m.fecha_mantenimiento::date AND m.fecha_final_mantenimiento::date
              );

            -- Retornar la fecha con su disponibilidad
            RETURN QUERY SELECT (fecha_actual || ' 00:00:00')::timestamp without time zone, grupo_id, disponibilidad;
        END LOOP;
    END LOOP;
END;
$$;


ALTER FUNCTION public.obtener_disponibilidad_equipos_por_fechas_y_id_grupos_equipos(fecha_inicio timestamp without time zone, fecha_fin timestamp without time zone, p_array_ids integer[]) OWNER TO postgres;

--
-- TOC entry 280 (class 1255 OID 22929)
-- Name: obtener_empresas_mantenimiento(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_empresas_mantenimiento() RETURNS TABLE(id_empresa_mantenimiento integer, nombre_empresa character varying, nombre_responsable_empresa character varying, apellido_responsable_empresa character varying, telefono_empresa character varying, nit_empresa character varying, direccion_empresa character varying)
    LANGUAGE plpgsql ROWS 100
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		em.id_empresa_mantenimiento,
        em.nombre,
        em.nombre_responsable,
        em.apellido_responsable,
        em.telefono,
        em.nit,
        em.direccion
    FROM
        public.empresas_mantenimiento AS em
    WHERE
        em.estado_eliminado = FALSE; 
                                    
END;
$$;


ALTER FUNCTION public.obtener_empresas_mantenimiento() OWNER TO postgres;

--
-- TOC entry 390 (class 1255 OID 22930)
-- Name: obtener_equipos(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_equipos() RETURNS TABLE(id_equipo integer, nombre_grupo_equipo character varying, modelo_equipo character varying, marca_equipo character varying, codigo_imt_equipo integer, codigo_ucb_equipo character varying, numero_serial_equipo character varying, estado_equipo_equipo public.estado_equipo, ubicacion_equipo character varying, nombre_gavetero_equipo character varying, costo_referencia_equipo double precision, descripcion_equipo text, tiempo_max_prestamo_equipo integer, procedencia_equipo character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		e.id_equipo,
        ge.nombre AS nombre_grupo_equipo,
		ge.modelo as modelo_equipo,
		ge.marca as marca_equipo,
        e.codigo_imt AS codigo_imt_equipo,
        e.codigo_ucb AS codigo_ucb_equipo,
        e.numero_serial AS numero_serial_equipo,
        e.estado_equipo AS estado_equipo_equipo,
        e.ubicacion AS ubicacion_equipo,
        g.nombre AS nombre_gavetero_equipo, 
        e.costo_referencia AS costo_referencia_equipo,
        e.descripcion AS descripcion_equipo,
        e.tiempo_max_prestamo AS tiempo_max_prestamo_equipo,
        e.procedencia AS procedencia_equipo
    FROM
        public.equipos AS e
    left JOIN
        public.grupos_equipos AS ge 
		ON e.id_grupo_equipo = ge.id_grupo_equipo
        AND ge.estado_eliminado = FALSE  
    LEFT JOIN
        public.gaveteros AS g 
		ON e.id_gavetero = g.id_gavetero 
        and g.estado_eliminado=false                       
    WHERE
        e.estado_eliminado = FALSE;
END;
$$;


ALTER FUNCTION public.obtener_equipos() OWNER TO postgres;

--
-- TOC entry 389 (class 1255 OID 22931)
-- Name: obtener_equipos_necesitan_mantenimiento(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_equipos_necesitan_mantenimiento() RETURNS TABLE(id_equipo integer, codigo_imt integer, nombre character varying, estado_equipo public.estado_equipo, ubicacion character varying, ultima_fecha_mantenimiento date)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		e.id_equipo,
        e.codigo_imt,
        ge.nombre,
        e.estado_equipo,
        e.ubicacion,
        COALESCE(MAX(m.fecha_mantenimiento), e.fecha_ingreso_equipo) AS ultima_fecha_mantenimiento
    FROM equipos e
    LEFT JOIN detalles_mantenimientos as dm
        ON dm.id_equipo = e.id_equipo AND dm.estado_eliminado = false
    INNER JOIN grupos_equipos as ge
        ON ge.id_grupo_equipo = e.id_grupo_equipo
    LEFT JOIN mantenimientos as m
        ON m.id_mantenimiento = dm.id_mantenimiento AND m.estado_eliminado = false
    WHERE e.estado_eliminado = false
    GROUP BY
        e.codigo_imt, ge.nombre, e.estado_equipo, e.ubicacion, e.fecha_ingreso_equipo
    HAVING
        (
            
            e.estado_equipo IN ('parcialmente_operativo', 'inoperativo')
            OR
            
            (MAX(m.fecha_mantenimiento) IS NOT NULL AND EXTRACT(MONTH FROM AGE(CURRENT_DATE, MAX(m.fecha_mantenimiento))) > 4)
            OR
            
            (MAX(m.fecha_mantenimiento) IS NULL AND EXTRACT(MONTH FROM AGE(CURRENT_DATE, e.fecha_ingreso_equipo)) > 4)
        )
    ORDER BY
        ultima_fecha_mantenimiento DESC;

END;
$$;


ALTER FUNCTION public.obtener_equipos_necesitan_mantenimiento() OWNER TO postgres;

--
-- TOC entry 301 (class 1255 OID 22932)
-- Name: obtener_fechas_no_disponibles_por_id_grupos_equipos(timestamp without time zone, timestamp without time zone, jsonb); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_fechas_no_disponibles_por_id_grupos_equipos(fecha_inicio timestamp without time zone, fecha_fin timestamp without time zone, json_input jsonb) RETURNS TABLE(id_grupo_equipo integer, fecha_no_disponible timestamp without time zone, cantidad_disponible bigint)
    LANGUAGE plpgsql
    AS $$
DECLARE
    fecha_actual date;
    grupo_key text;
    cantidad_requerida integer;
    disponibilidad bigint;
    grupo_array text[];
    dias_solicitados integer;
BEGIN
    -- Convertir jsonb_object_keys a array correctamente y ordenar
    grupo_array := array_agg(key ORDER BY key::integer) FROM jsonb_object_keys(json_input) AS t(key);
    
    -- Calcular días solicitados
    dias_solicitados := (fecha_fin::date - fecha_inicio::date);
    
    -- Iterar por cada día en el rango
    FOR fecha_actual IN SELECT generate_series(fecha_inicio::date, fecha_fin::date, INTERVAL '1 day')::date LOOP
        -- Iterar por cada grupo en el JSON usando el array pre-convertido
        FOREACH grupo_key IN ARRAY grupo_array LOOP
            cantidad_requerida := (json_input ->> grupo_key)::integer;
            
            -- Contar equipos disponibles para este grupo en esta fecha
            -- que CUMPLEN con: tiempo_solicitado <= tiempo_max_prestamo
            SELECT COUNT(*)
            INTO disponibilidad
            FROM (
                -- Obtener todos los equipos activos del grupo que cumplen restricción de tiempo
                SELECT e.id_equipo
                FROM public.equipos e
                WHERE e.id_grupo_equipo = grupo_key::integer
                  AND e.estado_eliminado = FALSE
                  AND e.estado_equipo = 'operativo'
                  AND dias_solicitados <= e.tiempo_max_prestamo
                
                EXCEPT
                
                -- Restar equipos ocupados en préstamos activos en esta fecha
                SELECT DISTINCT dp.id_equipo
                FROM public.detalles_prestamos dp
                INNER JOIN public.prestamos p ON dp.id_prestamo = p.id_prestamo
                WHERE p.estado_eliminado = FALSE
                  AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                  AND fecha_actual BETWEEN p.fecha_prestamo_esperada::date AND p.fecha_devolucion_esperada::date
                
                EXCEPT
                
                -- Restar equipos en mantenimiento en esta fecha
                SELECT DISTINCT dm.id_equipo
                FROM public.detalles_mantenimientos dm
                INNER JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
                WHERE m.estado_eliminado = FALSE
                  AND fecha_actual BETWEEN m.fecha_mantenimiento AND m.fecha_final_mantenimiento
            ) equipos_disponibles;
            
            -- Si no hay suficientes equipos disponibles, registrar esta fecha como no disponible
            IF disponibilidad < cantidad_requerida THEN
                RETURN QUERY SELECT grupo_key::integer, (fecha_actual || ' 00:00:00')::timestamp without time zone, disponibilidad;
            END IF;
        END LOOP;
    END LOOP;
END;
$$;


ALTER FUNCTION public.obtener_fechas_no_disponibles_por_id_grupos_equipos(fecha_inicio timestamp without time zone, fecha_fin timestamp without time zone, json_input jsonb) OWNER TO postgres;

--
-- TOC entry 283 (class 1255 OID 22933)
-- Name: obtener_gaveteros(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_gaveteros() RETURNS TABLE(id_gavetero integer, nombre_gavetero character varying, tipo_gavetero character varying, nombre_mueble character varying, longitud_gavetero double precision, profundidad_gavetero double precision, altura_gavetero double precision)
    LANGUAGE plpgsql ROWS 100
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		g.id_gavetero,
        g.nombre AS nombre_gavetero,
        g.tipo AS tipo_gavetero,
        m.nombre AS nombre_mueble,
        g.longitud AS longitud_gavetero,
        g.profundidad AS profundidad_gavetero,
        g.altura AS altura_gavetero
    FROM
        public.gaveteros AS g
    left JOIN
        public.muebles AS m ON g.id_mueble = m.id_mueble
								and m.estado_eliminado=false
    WHERE
        g.estado_eliminado = FALSE;

END;
$$;


ALTER FUNCTION public.obtener_gaveteros() OWNER TO postgres;

--
-- TOC entry 299 (class 1255 OID 22934)
-- Name: obtener_grupo_equipo_especifico_por_id(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_grupo_equipo_especifico_por_id(id_grupo_equipo_input integer) RETURNS TABLE(id_grupo_equipo integer, nombre_grupo_equipo character varying, modelo_grupo_equipo character varying, marca_grupo_equipo character varying, descripcion_grupo_equipo text, url_data_sheet_grupo_equipo text, nombre_categoria character varying, url_imagen_grupo_equipo text, cantidad_grupo_equipo integer, costo_promedio numeric)
    LANGUAGE plpgsql
    AS $$
BEGIN

    RETURN QUERY
    SELECT 
        ge.id_grupo_equipo,
        ge.nombre,
        ge.modelo,
        ge.marca,
        ge.descripcion,
        ge.url_data_sheet,
        c.nombre as categoria,
        ge.url_imagen,
        ge.cantidad,
        ge.costo_promedio
    FROM grupos_equipos as ge
    INNER JOIN categorias as c
        ON c.id_categoria = ge.id_categoria
    WHERE 
        ge.id_grupo_equipo = id_grupo_equipo_input
    LIMIT 1;
END;
$$;


ALTER FUNCTION public.obtener_grupo_equipo_especifico_por_id(id_grupo_equipo_input integer) OWNER TO postgres;

--
-- TOC entry 351 (class 1255 OID 22935)
-- Name: obtener_grupos_equipos(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_grupos_equipos() RETURNS TABLE(id_grupo_equipo integer, nombre_grupo_equipo character varying, modelo_grupo_equipo character varying, marca_grupo_equipo character varying, nombre_categoria character varying, cantidad_grupo_equipo integer, descripcion_grupo_equipo text, url_data_sheet_grupo_equipo text, url_imagen_grupo_equipo text, costo_promedio numeric)
    LANGUAGE plpgsql ROWS 100
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		ge.id_grupo_equipo as id_grupo_equipo,
        ge.nombre AS nombre_grupo_equipo,
        ge.modelo AS modelo_grupo_equipo,
        ge.marca AS marca_grupo_equipo,
        c.nombre AS nombre_categoria,
        ge.cantidad AS cantidad_grupo_equipo,
        ge.descripcion AS descripcion_grupo_equipo,
		ge.url_data_sheet as url_data_sheet_grupo_equipo,
		ge.url_imagen as url_imagen_grupo_equipo,
        ge.costo_promedio as costo_promedio
    FROM
        public.grupos_equipos AS ge
    Left JOIN
        public.categorias AS c 
		ON ge.id_categoria = c.id_categoria
                             AND c.estado_eliminado = FALSE 
    WHERE
        ge.estado_eliminado = FALSE; 
END;
$$;


ALTER FUNCTION public.obtener_grupos_equipos() OWNER TO postgres;

--
-- TOC entry 387 (class 1255 OID 22936)
-- Name: obtener_grupos_equipos_por_nombre_y_categoria(text, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_grupos_equipos_por_nombre_y_categoria(nombre_grupo_equipo_input text, categoria_input text) RETURNS TABLE(id_grupo_equipo integer, nombre_grupo_equipo character varying, modelo_grupo_equipo character varying, marca_grupo_equipo character varying, nombre_categoria character varying, url_imagen_grupo_equipo text, url_data_sheet_grupo_equipo text, descripcion_grupo_equipo text, cantidad_grupo_equipo integer, costo_promedio numeric)
    LANGUAGE plpgsql
    AS $$
BEGIN

    RETURN QUERY
    SELECT 
        ge.id_grupo_equipo,
        ge.nombre,
        ge.modelo,
        ge.marca,
        c.nombre as categoria,
        ge.url_imagen,
        ge.url_data_sheet,
        ge.descripcion,
        ge.cantidad,
        ge.costo_promedio
    FROM grupos_equipos as ge
    INNER JOIN categorias as c
        ON c.id_categoria = ge.id_categoria
    WHERE 
        (REPLACE(LOWER(ge.nombre),' ','') LIKE '%' || REPLACE(LOWER(nombre_grupo_equipo_input),' ','') || '%' OR nombre_grupo_equipo_input is NULL)  
        AND (REPLACE(LOWER(c.nombre),' ','') LIKE '%' || REPLACE(LOWER(categoria_input),' ','') || '%' OR categoria_input is NULL)  
        AND ge.estado_eliminado = false;
END;
$$;


ALTER FUNCTION public.obtener_grupos_equipos_por_nombre_y_categoria(nombre_grupo_equipo_input text, categoria_input text) OWNER TO postgres;

--
-- TOC entry 347 (class 1255 OID 22937)
-- Name: obtener_mantenimientos(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_mantenimientos() RETURNS TABLE(id_mantenimiento integer, nombre_empresa_mantenimiento character varying, fecha_mantenimiento date, fecha_final_mantenimiento date, costo_mantenimiento double precision, descripcion_mantenimiento text, tipo_detalle_mantenimiento character varying, nombre_grupo_equipo character varying, codigo_imt_equipo integer, descripcion_equipo text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		m.id_mantenimiento,
        em.nombre AS nombre_empresa_mantenimiento,
        m.fecha_mantenimiento,
        m.fecha_final_mantenimiento,
        m.costo AS costo_mantenimiento,
        m.descripcion AS descripcion_mantenimiento,
        dm.tipo_mantenimiento AS tipo_detalle_mantenimiento,
        ge.nombre AS nombre_grupo_equipo,
        e.codigo_imt AS codigo_imt_equipo,
		dm.descripcion as descripcion_equipo
    FROM
        public.mantenimientos AS m
    LEFT JOIN
        public.empresas_mantenimiento AS em
            ON m.id_empresa = em.id_empresa_mantenimiento AND em.estado_eliminado = FALSE
    LEFT JOIN
        public.detalles_mantenimientos AS dm
            ON m.id_mantenimiento = dm.id_mantenimiento AND dm.estado_eliminado = FALSE
    LEFT JOIN
        public.equipos AS e
            ON dm.id_equipo = e.id_equipo AND e.estado_eliminado = FALSE
    LEFT JOIN
        public.grupos_equipos AS ge
            ON e.id_grupo_equipo = ge.id_grupo_equipo AND ge.estado_eliminado = FALSE
    WHERE
        m.estado_eliminado = FALSE
	order by m.id_mantenimiento; 
END;
$$;


ALTER FUNCTION public.obtener_mantenimientos() OWNER TO postgres;

--
-- TOC entry 313 (class 1255 OID 22938)
-- Name: obtener_muebles(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_muebles() RETURNS TABLE(id_mueble integer, nombre_mueble character varying, numero_gaveteros_mueble integer, ubicacion_mueble character varying, tipo_mueble character varying, costo_mueble double precision, longitud_mueble double precision, profundidad_mueble double precision, altura_mueble double precision)
    LANGUAGE plpgsql ROWS 100
    AS $$
BEGIN
    RETURN QUERY
    SELECT
		m.id_mueble,
        m.nombre AS nombre_mueble,
        m.numero_gaveteros AS numero_gaveteros_mueble,
        m.ubicacion AS ubicacion_mueble,
        m.tipo AS tipo_mueble,
        m.costo AS costo_mueble,
        m.longitud AS longitud_mueble,
        m.profundidad AS profundidad_mueble,
        m.altura AS altura_mueble
    FROM
        public.muebles AS m
    WHERE
        m.estado_eliminado = FALSE;
END;
$$;


ALTER FUNCTION public.obtener_muebles() OWNER TO postgres;

--
-- TOC entry 289 (class 1255 OID 22939)
-- Name: obtener_numero_equipos_disponibles_por_id_y_fechas(integer, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_numero_equipos_disponibles_por_id_y_fechas(id_grupo_equipo_input integer, fecha_prestamo_esperada_input timestamp without time zone, fecha_devolucion_esperada_input timestamp without time zone) RETURNS TABLE(cantidad_disponible bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT COUNT(e.id_equipo) AS cantidad_equipos_disponibles
    FROM public.equipos e
    WHERE
        e.id_grupo_equipo = id_grupo_equipo_input

        AND e.estado_eliminado = FALSE
        AND e.estado_equipo = 'operativo'

        AND NOT EXISTS (
            SELECT 1
            FROM public.detalles_prestamos dp
            JOIN public.prestamos pr ON dp.id_prestamo = pr.id_prestamo
            WHERE dp.id_equipo = e.id_equipo
              AND pr.estado_eliminado = FALSE
              AND pr.estado_prestamo IN ('pendiente', 'aprobado', 'activo') 
              AND (
                  
                  fecha_prestamo_esperada_input < pr.fecha_devolucion_esperada AND
                  fecha_devolucion_esperada_input > pr.fecha_prestamo_esperada
              )
        )

        AND NOT EXISTS (
            SELECT 1
            FROM public.detalles_mantenimientos dm
            JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
            WHERE dm.id_equipo = e.id_equipo
              AND m.estado_eliminado = FALSE 
              AND (
                  
                  fecha_prestamo_esperada_input < m.fecha_final_mantenimiento AND
                  fecha_devolucion_esperada_input > m.fecha_mantenimiento
              )
        );
END;
$$;


ALTER FUNCTION public.obtener_numero_equipos_disponibles_por_id_y_fechas(id_grupo_equipo_input integer, fecha_prestamo_esperada_input timestamp without time zone, fecha_devolucion_esperada_input timestamp without time zone) OWNER TO postgres;

--
-- TOC entry 350 (class 1255 OID 22940)
-- Name: obtener_prestamos(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_prestamos() RETURNS TABLE(id_prestamo integer, carnet character varying, nombre character varying, apellido_paterno character varying, telefono character varying, nombre_grupo_equipo character varying, codigo_imt integer, fecha_solicitud timestamp without time zone, fecha_prestamo_esperada timestamp without time zone, fecha_prestamo timestamp without time zone, fecha_devolucion_esperada timestamp without time zone, fecha_devolucion timestamp without time zone, observacion text, estado_prestamo public.estado_prestamo, ubicacion_equipo character varying, nombre_gavetero character varying, nombre_mueble character varying, ubicacion_mueble character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
	SELECT
	p.id_prestamo,
	p.carnet,
	u.nombre,
	u.apellido_paterno,
	u.telefono,
	ge.nombre as nombre_grupo_equipo,
	e.codigo_imt,
	p.fecha_solicitud, 
	p.fecha_prestamo_esperada,
	p.fecha_prestamo,
	p.fecha_devolucion_esperada,
	p.fecha_devolucion,
	p.observacion, 
	p.estado_prestamo,
	e.ubicacion as ubicacion_equipo,
	ga.nombre as nombre_gavetero,
	mu.nombre as nombre_mueble,
	mu.ubicacion as ubicacion_mueble
	FROM public.prestamos as p
	left join detalles_prestamos as dp
	on dp.id_prestamo=p.id_prestamo
	and dp.estado_eliminado=false
	left join equipos as e
	on dp.id_equipo=e.id_equipo
	and e.estado_eliminado=false
	left join grupos_equipos as ge
	on ge.id_grupo_equipo=e.id_grupo_equipo
	and ge.estado_eliminado=false
	left join usuarios as u
	on u.carnet=p.carnet
	and u.estado_eliminado=false
	left join gaveteros as ga
	on ga.id_gavetero=e.id_gavetero
	and ga.estado_eliminado=false
	left join muebles as mu
	on mu.id_mueble=ga.id_mueble
	and mu.estado_eliminado=false
	
	where p.estado_eliminado=false
	order by fecha_solicitud desc;
END;
$$;


ALTER FUNCTION public.obtener_prestamos() OWNER TO postgres;

--
-- TOC entry 335 (class 1255 OID 22941)
-- Name: obtener_prestamos_por_carnet_y_estado_prestamo(character varying, public.estado_prestamo); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_prestamos_por_carnet_y_estado_prestamo(p_carnet_input character varying, p_estado_input public.estado_prestamo) RETURNS TABLE(id_prestamo integer, carnet character varying, nombre character varying, apellido_paterno character varying, telefono character varying, nombre_grupo_equipo character varying, codigo_imt integer, fecha_solicitud timestamp without time zone, fecha_prestamo_esperada timestamp without time zone, fecha_prestamo timestamp without time zone, fecha_devolucion_esperada timestamp without time zone, fecha_devolucion timestamp without time zone, observacion text, estado_prestamo public.estado_prestamo, ubicacion_equipo character varying, nombre_gavetero character varying, nombre_mueble character varying, ubicacion_mueble character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        p.id_prestamo                          AS id_prestamo,
        p.carnet                               AS carnet,
        u.nombre                               AS nombre,
        u.apellido_paterno                     AS apellido_paterno,
        u.telefono                             AS telefono,
        ge.nombre                              AS nombre_grupo_equipo,
        e.codigo_imt                           AS codigo_imt,
        p.fecha_solicitud                      AS fecha_solicitud,
        p.fecha_prestamo_esperada              AS fecha_prestamo_esperada,
        p.fecha_prestamo                       AS fecha_prestamo,
        p.fecha_devolucion_esperada            AS fecha_devolucion_esperada,
        p.fecha_devolucion                     AS fecha_devolucion,
        p.observacion                          AS observacion,
        p.estado_prestamo                      AS estado_prestamo,
		e.ubicacion								AS ubicacion_equipo,
		ga.nombre								AS nombre_gavetero,
		mu.nombre								AS nombre_mueble,
		mu.ubicacion							AS ubicacion_mueble
    FROM public.prestamos p
    INNER JOIN public.detalles_prestamos dp
        ON dp.id_prestamo = p.id_prestamo
    INNER JOIN public.equipos e
        ON dp.id_equipo = e.id_equipo
    INNER JOIN public.grupos_equipos ge
        ON e.id_grupo_equipo = ge.id_grupo_equipo
    INNER JOIN public.usuarios u
        ON u.carnet = p.carnet
	LEFT JOIN public.gaveteros as ga
		ON ga.id_gavetero=e.id_gavetero
	LEFT JOIN public.muebles as mu
		ON mu.id_mueble=ga.id_mueble
    WHERE NOT p.estado_eliminado
      AND p.carnet = p_carnet_input
      AND p.estado_prestamo = p_estado_input
    ORDER BY p.fecha_solicitud DESC;
END;
$$;


ALTER FUNCTION public.obtener_prestamos_por_carnet_y_estado_prestamo(p_carnet_input character varying, p_estado_input public.estado_prestamo) OWNER TO postgres;

--
-- TOC entry 376 (class 1255 OID 22942)
-- Name: obtener_ubicaciones_grupos_equipos_por_nombre(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_ubicaciones_grupos_equipos_por_nombre(nombre_grupo_equipo_input text) RETURNS TABLE(id_grupo_equipo integer, codigo_imt integer, nombre character varying, modelo character varying, marca character varying, ubicacion character varying, categoria character varying, url_imagen text)
    LANGUAGE plpgsql
    AS $$
BEGIN

    RETURN QUERY
    SELECT 
		ge.id_grupo_equipo,
		e.codigo_imt,
        ge.nombre,
        ge.modelo,
        ge.marca,
		e.ubicacion,
        c.nombre as categoria,
        ge.url_imagen
    FROM grupos_equipos as ge
	inner join equipos as e
	on e.id_grupo_equipo=ge.id_grupo_equipo
    INNER JOIN categorias as c
    ON c.id_categoria = ge.id_categoria
	Left join gaveteros as ga
	on e.id_gavetero=ga.id_gavetero
	inner join muebles as mu
	on mu.id_mueble=ga.id_mueble
    WHERE 
        (REPLACE(LOWER(ge.nombre),' ','') LIKE '%' || REPLACE(LOWER(nombre_grupo_equipo_input),' ','') || '%');
END;
$$;


ALTER FUNCTION public.obtener_ubicaciones_grupos_equipos_por_nombre(nombre_grupo_equipo_input text) OWNER TO postgres;

--
-- TOC entry 302 (class 1255 OID 22943)
-- Name: obtener_usuario_iniciar_sesion(character varying, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_usuario_iniciar_sesion(email_input character varying, contrasena_input text) RETURNS TABLE(carnet character varying, nombre character varying, apellido_paterno character varying, apellido_materno character varying, rol public.tipo_usuario, carrera character varying, email character varying, telefono character varying, telefono_referencia character varying, nombre_referencia character varying, email_referencia character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u.carnet,
		u.nombre,
		u.apellido_paterno,
		u.apellido_materno,
        u.rol,
        c.nombre,
		u.email,
        u.telefono,
        u.telefono_referencia,
        u.nombre_referencia,
        u.email_referencia
    FROM usuarios as u
	inner join carreras as c
	on c.id_carrera=u.id_carrera
    WHERE 
	u.email=email_input and
	u.contrasena=contrasena_input and 
	u.estado_eliminado=false;
END;
$$;


ALTER FUNCTION public.obtener_usuario_iniciar_sesion(email_input character varying, contrasena_input text) OWNER TO postgres;

--
-- TOC entry 391 (class 1255 OID 22944)
-- Name: obtener_usuario_por_carnet(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_usuario_por_carnet(carnet_input text) RETURNS TABLE(carnet character varying, nombre character varying, apellido_paterno character varying, apellido_materno character varying, rol public.tipo_usuario, carrera character varying, email character varying, contrasena text, telefono character varying, telefono_referencia character varying, nombre_referencia character varying, email_referencia character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u.carnet,
        u.nombre,
        u.apellido_paterno,
        u.apellido_materno,
        u.rol,
        c.nombre,
		u.email,
		u.contrasena,
        u.telefono,
        u.telefono_referencia,
        u.nombre_referencia,
        u.email_referencia
    FROM usuarios as u
	inner join carreras as c
	on c.id_carrera=u.id_carrera
    WHERE u.carnet = carnet_input
	and u.estado_eliminado=false;
END;
$$;


ALTER FUNCTION public.obtener_usuario_por_carnet(carnet_input text) OWNER TO postgres;

--
-- TOC entry 338 (class 1255 OID 22945)
-- Name: obtener_usuarios(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.obtener_usuarios() RETURNS TABLE(carnet character varying, nombre character varying, apellido_paterno character varying, apellido_materno character varying, carrera character varying, rol public.tipo_usuario, email character varying, telefono character varying, telefono_referencia character varying, nombre_referencia character varying, email_referencia character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
	SELECT u.carnet, u.nombre, u.apellido_paterno, u.apellido_materno, c.nombre, u.rol, u.email, u.telefono, u.telefono_referencia, u.nombre_referencia, u.email_referencia
	FROM public.usuarios as u
	left join carreras as c
	on c.id_carrera=u.id_carrera
	and c.estado_eliminado=false
	where u.estado_eliminado=false;
END;
$$;


ALTER FUNCTION public.obtener_usuarios() OWNER TO postgres;

--
-- TOC entry 384 (class 1255 OID 22946)
-- Name: recalcular_costo_promedio_todos_grupos(); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.recalcular_costo_promedio_todos_grupos()
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_grupo_id integer;
    v_costo_promedio numeric(10,2);
BEGIN
    -- Iterar sobre todos los grupos de equipos
    FOR v_grupo_id IN SELECT id_grupo_equipo FROM public.grupos_equipos ORDER BY id_grupo_equipo LOOP
        
        -- Calcular promedio de equipos operativos no eliminados
        SELECT COALESCE(AVG(costo_referencia), 0)
        INTO v_costo_promedio
        FROM public.equipos
        WHERE id_grupo_equipo = v_grupo_id
          AND estado_eliminado = FALSE
          AND estado_equipo = 'operativo';
        
        -- Actualizar el grupo
        UPDATE public.grupos_equipos
        SET costo_promedio = v_costo_promedio
        WHERE id_grupo_equipo = v_grupo_id;
        
        RAISE NOTICE 'Grupo % actualizado: costo_promedio = %', v_grupo_id, v_costo_promedio;
    END LOOP;
    
    RAISE NOTICE 'Todos los costos promedios han sido recalculados.';
END;
$$;


ALTER PROCEDURE public.recalcular_costo_promedio_todos_grupos() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 22947)
-- Name: aggregatedcounter; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.aggregatedcounter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp with time zone
);


ALTER TABLE hangfire.aggregatedcounter OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 22952)
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.aggregatedcounter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.aggregatedcounter_id_seq OWNER TO postgres;

--
-- TOC entry 5401 (class 0 OID 0)
-- Dependencies: 220
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.aggregatedcounter_id_seq OWNED BY hangfire.aggregatedcounter.id;


--
-- TOC entry 221 (class 1259 OID 22953)
-- Name: counter; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.counter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp with time zone
);


ALTER TABLE hangfire.counter OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 22958)
-- Name: counter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.counter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.counter_id_seq OWNER TO postgres;

--
-- TOC entry 5402 (class 0 OID 0)
-- Dependencies: 222
-- Name: counter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.counter_id_seq OWNED BY hangfire.counter.id;


--
-- TOC entry 223 (class 1259 OID 22959)
-- Name: hash; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.hash (
    id bigint NOT NULL,
    key text NOT NULL,
    field text NOT NULL,
    value text,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.hash OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 22965)
-- Name: hash_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.hash_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.hash_id_seq OWNER TO postgres;

--
-- TOC entry 5403 (class 0 OID 0)
-- Dependencies: 224
-- Name: hash_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.hash_id_seq OWNED BY hangfire.hash.id;


--
-- TOC entry 225 (class 1259 OID 22966)
-- Name: job; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.job (
    id bigint NOT NULL,
    stateid bigint,
    statename text,
    invocationdata jsonb NOT NULL,
    arguments jsonb NOT NULL,
    createdat timestamp with time zone NOT NULL,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.job OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 22972)
-- Name: job_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.job_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.job_id_seq OWNER TO postgres;

--
-- TOC entry 5404 (class 0 OID 0)
-- Dependencies: 226
-- Name: job_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.job_id_seq OWNED BY hangfire.job.id;


--
-- TOC entry 227 (class 1259 OID 22973)
-- Name: jobparameter; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.jobparameter (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    value text,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobparameter OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 22979)
-- Name: jobparameter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.jobparameter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.jobparameter_id_seq OWNER TO postgres;

--
-- TOC entry 5405 (class 0 OID 0)
-- Dependencies: 228
-- Name: jobparameter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.jobparameter_id_seq OWNED BY hangfire.jobparameter.id;


--
-- TOC entry 229 (class 1259 OID 22980)
-- Name: jobqueue; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.jobqueue (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    queue text NOT NULL,
    fetchedat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobqueue OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 22986)
-- Name: jobqueue_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.jobqueue_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.jobqueue_id_seq OWNER TO postgres;

--
-- TOC entry 5406 (class 0 OID 0)
-- Dependencies: 230
-- Name: jobqueue_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.jobqueue_id_seq OWNED BY hangfire.jobqueue.id;


--
-- TOC entry 231 (class 1259 OID 22987)
-- Name: list; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.list (
    id bigint NOT NULL,
    key text NOT NULL,
    value text,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.list OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 22993)
-- Name: list_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.list_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.list_id_seq OWNER TO postgres;

--
-- TOC entry 5407 (class 0 OID 0)
-- Dependencies: 232
-- Name: list_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.list_id_seq OWNED BY hangfire.list.id;


--
-- TOC entry 233 (class 1259 OID 22994)
-- Name: lock; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.lock (
    resource text NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL,
    acquired timestamp with time zone
);


ALTER TABLE hangfire.lock OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 23000)
-- Name: schema; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.schema (
    version integer NOT NULL
);


ALTER TABLE hangfire.schema OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 23003)
-- Name: server; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.server (
    id text NOT NULL,
    data jsonb,
    lastheartbeat timestamp with time zone NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.server OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 23009)
-- Name: set; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.set (
    id bigint NOT NULL,
    key text NOT NULL,
    score double precision NOT NULL,
    value text NOT NULL,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.set OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 23015)
-- Name: set_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.set_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.set_id_seq OWNER TO postgres;

--
-- TOC entry 5408 (class 0 OID 0)
-- Dependencies: 237
-- Name: set_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.set_id_seq OWNED BY hangfire.set.id;


--
-- TOC entry 238 (class 1259 OID 23016)
-- Name: state; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.state (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    reason text,
    createdat timestamp with time zone NOT NULL,
    data jsonb,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.state OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 23022)
-- Name: state_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.state_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.state_id_seq OWNER TO postgres;

--
-- TOC entry 5409 (class 0 OID 0)
-- Dependencies: 239
-- Name: state_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.state_id_seq OWNED BY hangfire.state.id;


--
-- TOC entry 240 (class 1259 OID 23023)
-- Name: accesorios; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.accesorios (
    id_accesorio integer NOT NULL,
    nombre character varying(255) NOT NULL,
    descripcion text,
    modelo character varying(255) NOT NULL,
    url_data_sheet text,
    precio double precision,
    id_equipo integer NOT NULL,
    tipo character varying(255),
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.accesorios OWNER TO postgres;

--
-- TOC entry 5410 (class 0 OID 0)
-- Dependencies: 240
-- Name: COLUMN accesorios.id_accesorio; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.accesorios.id_accesorio IS 'Código del accesorio';


--
-- TOC entry 241 (class 1259 OID 23029)
-- Name: Accesorio_Id_Accesorio_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.accesorios ALTER COLUMN id_accesorio ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Accesorio_Id_Accesorio_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 242 (class 1259 OID 23030)
-- Name: categorias; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.categorias (
    id_categoria integer NOT NULL,
    nombre character varying(255) NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.categorias OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 23034)
-- Name: Categoria_ID_Categoria_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.categorias ALTER COLUMN id_categoria ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Categoria_ID_Categoria_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 244 (class 1259 OID 23035)
-- Name: componentes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.componentes (
    id_componente integer NOT NULL,
    descripcion text,
    modelo character varying(255) NOT NULL,
    url_data_sheet text,
    tipo character varying(255),
    precio_referencia double precision,
    nombre character varying(255) NOT NULL,
    id_equipo integer NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.componentes OWNER TO postgres;

--
-- TOC entry 5411 (class 0 OID 0)
-- Dependencies: 244
-- Name: COLUMN componentes.id_componente; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.componentes.id_componente IS 'Código del componente';


--
-- TOC entry 245 (class 1259 OID 23041)
-- Name: Componente_Id_Componente_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.componentes ALTER COLUMN id_componente ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Componente_Id_Componente_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 246 (class 1259 OID 23042)
-- Name: empresas_mantenimiento; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.empresas_mantenimiento (
    id_empresa_mantenimiento integer NOT NULL,
    nombre character varying(255) NOT NULL,
    direccion character varying(512),
    telefono character varying(64),
    nit character varying(255),
    estado_eliminado boolean DEFAULT false NOT NULL,
    nombre_responsable character varying(64),
    apellido_responsable character varying(64)
);


ALTER TABLE public.empresas_mantenimiento OWNER TO postgres;

--
-- TOC entry 5412 (class 0 OID 0)
-- Dependencies: 246
-- Name: COLUMN empresas_mantenimiento.id_empresa_mantenimiento; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.empresas_mantenimiento.id_empresa_mantenimiento IS 'Código empresa';


--
-- TOC entry 247 (class 1259 OID 23048)
-- Name: Empresa_Mantenimiento_Id_Empresa_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.empresas_mantenimiento ALTER COLUMN id_empresa_mantenimiento ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Empresa_Mantenimiento_Id_Empresa_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 248 (class 1259 OID 23049)
-- Name: equipos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.equipos (
    id_equipo integer NOT NULL,
    id_grupo_equipo integer NOT NULL,
    codigo_imt integer NOT NULL,
    descripcion text,
    estado_equipo public.estado_equipo DEFAULT 'operativo'::public.estado_equipo NOT NULL,
    numero_serial character varying(255),
    ubicacion character varying(255),
    costo_referencia double precision DEFAULT 0,
    tiempo_max_prestamo integer DEFAULT 9999,
    procedencia character varying(255),
    id_gavetero integer,
    estado_eliminado boolean DEFAULT false NOT NULL,
    fecha_ingreso_equipo date DEFAULT CURRENT_DATE NOT NULL,
    codigo_ucb character varying
);


ALTER TABLE public.equipos OWNER TO postgres;

--
-- TOC entry 249 (class 1259 OID 23059)
-- Name: Equipo_Id_equipo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.equipos ALTER COLUMN id_equipo ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Equipo_Id_equipo_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 250 (class 1259 OID 23060)
-- Name: gaveteros; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.gaveteros (
    id_gavetero integer NOT NULL,
    nombre character varying(255) NOT NULL,
    tipo character varying(255),
    estado_eliminado boolean DEFAULT false NOT NULL,
    id_mueble integer NOT NULL,
    longitud double precision,
    profundidad double precision,
    altura double precision
);


ALTER TABLE public.gaveteros OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 23066)
-- Name: Gavetero_Id_Gavetero_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.gaveteros ALTER COLUMN id_gavetero ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Gavetero_Id_Gavetero_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 252 (class 1259 OID 23067)
-- Name: grupos_equipos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.grupos_equipos (
    id_grupo_equipo integer NOT NULL,
    nombre character varying(256) NOT NULL,
    modelo character varying(512) NOT NULL,
    url_data_sheet text,
    cantidad integer DEFAULT 0 NOT NULL,
    marca character varying(256) NOT NULL,
    id_categoria integer NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL,
    url_imagen text NOT NULL,
    descripcion text NOT NULL,
    costo_promedio numeric(10,2) DEFAULT 0
);


ALTER TABLE public.grupos_equipos OWNER TO postgres;

--
-- TOC entry 5413 (class 0 OID 0)
-- Dependencies: 252
-- Name: COLUMN grupos_equipos.descripcion; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.grupos_equipos.descripcion IS 'Esto se mostrar en la pagina web';


--
-- TOC entry 253 (class 1259 OID 23075)
-- Name: Grupo_Equipo_Id_Grupo_equipo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.grupos_equipos ALTER COLUMN id_grupo_equipo ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Grupo_Equipo_Id_Grupo_equipo_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 254 (class 1259 OID 23076)
-- Name: mantenimientos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.mantenimientos (
    id_mantenimiento integer NOT NULL,
    descripcion text,
    costo double precision,
    fecha_mantenimiento date NOT NULL,
    id_empresa integer NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL,
    fecha_final_mantenimiento date NOT NULL
);


ALTER TABLE public.mantenimientos OWNER TO postgres;

--
-- TOC entry 255 (class 1259 OID 23082)
-- Name: Mantenimiento_Id_Mantenimiento_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.mantenimientos ALTER COLUMN id_mantenimiento ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Mantenimiento_Id_Mantenimiento_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 256 (class 1259 OID 23083)
-- Name: muebles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.muebles (
    id_mueble integer NOT NULL,
    nombre character varying(255) NOT NULL,
    tipo character varying(255),
    ubicacion character varying(255),
    numero_gaveteros integer DEFAULT 0 NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL,
    longitud double precision,
    profundidad double precision,
    altura double precision,
    costo double precision
);


ALTER TABLE public.muebles OWNER TO postgres;

--
-- TOC entry 5414 (class 0 OID 0)
-- Dependencies: 256
-- Name: COLUMN muebles.id_mueble; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.muebles.id_mueble IS 'Código del mueble';


--
-- TOC entry 257 (class 1259 OID 23090)
-- Name: Mueble_Id_Mueble_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.muebles ALTER COLUMN id_mueble ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Mueble_Id_Mueble_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 258 (class 1259 OID 23091)
-- Name: prestamos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.prestamos (
    id_prestamo integer NOT NULL,
    fecha_solicitud timestamp without time zone DEFAULT (now() AT TIME ZONE 'America/La_Paz'::text) NOT NULL,
    fecha_prestamo timestamp without time zone,
    fecha_devolucion_esperada timestamp without time zone NOT NULL,
    observacion text,
    estado_prestamo public.estado_prestamo DEFAULT 'pendiente'::public.estado_prestamo NOT NULL,
    carnet character varying(64) NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL,
    fecha_devolucion timestamp without time zone,
    fecha_prestamo_esperada timestamp without time zone NOT NULL,
    id_contrato integer
);


ALTER TABLE public.prestamos OWNER TO postgres;

--
-- TOC entry 5415 (class 0 OID 0)
-- Dependencies: 258
-- Name: COLUMN prestamos.id_prestamo; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.prestamos.id_prestamo IS 'Código del préstamo';


--
-- TOC entry 259 (class 1259 OID 23099)
-- Name: Prestamo_Id_Prestamo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.prestamos ALTER COLUMN id_prestamo ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Prestamo_Id_Prestamo_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 274 (class 1259 OID 23372)
-- Name: audit_logs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.audit_logs (
    id integer NOT NULL,
    admin_carnet character varying(20) NOT NULL,
    admin_nombre text NOT NULL,
    accion character varying(50) NOT NULL,
    entidad character varying(100) NOT NULL,
    entidad_id text,
    detalle text,
    "timestamp" timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.audit_logs OWNER TO postgres;

--
-- TOC entry 273 (class 1259 OID 23371)
-- Name: audit_logs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.audit_logs_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.audit_logs_id_seq OWNER TO postgres;

--
-- TOC entry 5416 (class 0 OID 0)
-- Dependencies: 273
-- Name: audit_logs_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.audit_logs_id_seq OWNED BY public.audit_logs.id;


--
-- TOC entry 260 (class 1259 OID 23100)
-- Name: carreras; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.carreras (
    id_carrera integer NOT NULL,
    nombre character varying(255) NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.carreras OWNER TO postgres;

--
-- TOC entry 261 (class 1259 OID 23104)
-- Name: carrera_id_carrera_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.carrera_id_carrera_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.carrera_id_carrera_seq OWNER TO postgres;

--
-- TOC entry 5417 (class 0 OID 0)
-- Dependencies: 261
-- Name: carrera_id_carrera_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.carrera_id_carrera_seq OWNED BY public.carreras.id_carrera;


--
-- TOC entry 262 (class 1259 OID 23105)
-- Name: carreras_id_carrera_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.carreras ALTER COLUMN id_carrera ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.carreras_id_carrera_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 263 (class 1259 OID 23106)
-- Name: contratos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.contratos (
    id integer NOT NULL,
    contrato text
);


ALTER TABLE public.contratos OWNER TO postgres;

--
-- TOC entry 264 (class 1259 OID 23111)
-- Name: detalles_mantenimientos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.detalles_mantenimientos (
    id_detalle_mantenimiento integer NOT NULL,
    id_mantenimiento integer NOT NULL,
    descripcion text,
    id_equipo integer NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL,
    tipo_mantenimiento character varying
);


ALTER TABLE public.detalles_mantenimientos OWNER TO postgres;

--
-- TOC entry 265 (class 1259 OID 23117)
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.detalles_mantenimientos_id_detalle_mantenimiento_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.detalles_mantenimientos_id_detalle_mantenimiento_seq OWNER TO postgres;

--
-- TOC entry 5418 (class 0 OID 0)
-- Dependencies: 265
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.detalles_mantenimientos_id_detalle_mantenimiento_seq OWNED BY public.detalles_mantenimientos.id_detalle_mantenimiento;


--
-- TOC entry 266 (class 1259 OID 23118)
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq1; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.detalles_mantenimientos ALTER COLUMN id_detalle_mantenimiento ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.detalles_mantenimientos_id_detalle_mantenimiento_seq1
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 267 (class 1259 OID 23119)
-- Name: detalles_prestamos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.detalles_prestamos (
    id_detalle_prestamo integer NOT NULL,
    id_equipo integer,
    id_prestamo integer NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL,
    id_grupo_equipo integer NOT NULL,
    estado_equipo_retorno public.estado_equipo
);


ALTER TABLE public.detalles_prestamos OWNER TO postgres;

--
-- TOC entry 268 (class 1259 OID 23123)
-- Name: detalles_prestamos_id_detalle_prestamo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.detalles_prestamos ALTER COLUMN id_detalle_prestamo ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.detalles_prestamos_id_detalle_prestamo_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 269 (class 1259 OID 23124)
-- Name: nombre_de_tu_tabla_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.contratos ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.nombre_de_tu_tabla_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 270 (class 1259 OID 23125)
-- Name: usuarios; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.usuarios (
    carnet character varying(64) NOT NULL,
    nombre character varying(64) NOT NULL,
    apellido_paterno character varying(64) NOT NULL,
    apellido_materno character varying(64) NOT NULL,
    rol public.tipo_usuario DEFAULT 'estudiante'::public.tipo_usuario NOT NULL,
    contrasena text NOT NULL,
    email character varying(255) NOT NULL,
    telefono character varying(32) NOT NULL,
    telefono_referencia character varying(32),
    nombre_referencia character varying(32),
    email_referencia character varying(255),
    estado_eliminado boolean DEFAULT false NOT NULL,
    id_carrera integer NOT NULL,
    imagen_frente_carnet bytea,
    imagen_atras_carnet bytea,
    refresh_token text,
    refresh_token_expiry timestamp with time zone
);


ALTER TABLE public.usuarios OWNER TO postgres;

--
-- TOC entry 271 (class 1259 OID 23132)
-- Name: vw_equipos_necesitan_mantenimiento; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.vw_equipos_necesitan_mantenimiento AS
 SELECT e.codigo_imt,
    ge.nombre AS grupo_equipo,
    e.estado_equipo,
    e.ubicacion,
    COALESCE(max(m.fecha_mantenimiento), e.fecha_ingreso_equipo) AS ultima_fecha_mantenimiento
   FROM (((public.equipos e
     LEFT JOIN public.detalles_mantenimientos dm ON (((dm.id_equipo = e.id_equipo) AND (dm.estado_eliminado = false))))
     JOIN public.grupos_equipos ge ON ((ge.id_grupo_equipo = e.id_grupo_equipo)))
     LEFT JOIN public.mantenimientos m ON (((m.id_mantenimiento = dm.id_mantenimiento) AND (m.estado_eliminado = false))))
  WHERE (e.estado_eliminado = false)
  GROUP BY e.codigo_imt, ge.nombre, e.estado_equipo, e.ubicacion, e.fecha_ingreso_equipo
 HAVING ((e.estado_equipo = ANY (ARRAY['parcialmente_operativo'::public.estado_equipo, 'inoperativo'::public.estado_equipo])) OR ((max(m.fecha_mantenimiento) IS NOT NULL) AND (EXTRACT(month FROM age((CURRENT_DATE)::timestamp with time zone, (max(m.fecha_mantenimiento))::timestamp with time zone)) > (4)::numeric)) OR ((max(m.fecha_mantenimiento) IS NULL) AND (EXTRACT(month FROM age((CURRENT_DATE)::timestamp with time zone, (e.fecha_ingreso_equipo)::timestamp with time zone)) > (4)::numeric)));


ALTER VIEW public.vw_equipos_necesitan_mantenimiento OWNER TO postgres;

--
-- TOC entry 272 (class 1259 OID 23137)
-- Name: vw_ubicaciones_grupos_equipos; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.vw_ubicaciones_grupos_equipos AS
 SELECT ge.id_grupo_equipo,
    e.codigo_imt,
    ge.nombre,
    ge.modelo,
    ge.marca,
    e.ubicacion,
    c.nombre AS categoria,
    ge.url_imagen
   FROM ((((public.grupos_equipos ge
     JOIN public.equipos e ON ((e.id_grupo_equipo = ge.id_grupo_equipo)))
     JOIN public.categorias c ON ((c.id_categoria = ge.id_categoria)))
     LEFT JOIN public.gaveteros ga ON ((e.id_gavetero = ga.id_gavetero)))
     JOIN public.muebles mu ON ((mu.id_mueble = ga.id_mueble)))
  WHERE ((ge.estado_eliminado = false) AND (e.estado_eliminado = false));


ALTER VIEW public.vw_ubicaciones_grupos_equipos OWNER TO postgres;

--
-- TOC entry 5007 (class 2604 OID 23142)
-- Name: aggregatedcounter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter ALTER COLUMN id SET DEFAULT nextval('hangfire.aggregatedcounter_id_seq'::regclass);


--
-- TOC entry 5008 (class 2604 OID 23143)
-- Name: counter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.counter ALTER COLUMN id SET DEFAULT nextval('hangfire.counter_id_seq'::regclass);


--
-- TOC entry 5009 (class 2604 OID 23144)
-- Name: hash id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash ALTER COLUMN id SET DEFAULT nextval('hangfire.hash_id_seq'::regclass);


--
-- TOC entry 5011 (class 2604 OID 23145)
-- Name: job id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.job ALTER COLUMN id SET DEFAULT nextval('hangfire.job_id_seq'::regclass);


--
-- TOC entry 5013 (class 2604 OID 23146)
-- Name: jobparameter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter ALTER COLUMN id SET DEFAULT nextval('hangfire.jobparameter_id_seq'::regclass);


--
-- TOC entry 5015 (class 2604 OID 23147)
-- Name: jobqueue id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobqueue ALTER COLUMN id SET DEFAULT nextval('hangfire.jobqueue_id_seq'::regclass);


--
-- TOC entry 5017 (class 2604 OID 23148)
-- Name: list id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.list ALTER COLUMN id SET DEFAULT nextval('hangfire.list_id_seq'::regclass);


--
-- TOC entry 5021 (class 2604 OID 23149)
-- Name: set id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set ALTER COLUMN id SET DEFAULT nextval('hangfire.set_id_seq'::regclass);


--
-- TOC entry 5023 (class 2604 OID 23150)
-- Name: state id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state ALTER COLUMN id SET DEFAULT nextval('hangfire.state_id_seq'::regclass);


--
-- TOC entry 5049 (class 2604 OID 23375)
-- Name: audit_logs id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.audit_logs ALTER COLUMN id SET DEFAULT nextval('public.audit_logs_id_seq'::regclass);


--
-- TOC entry 5339 (class 0 OID 22947)
-- Dependencies: 219
-- Data for Name: aggregatedcounter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.aggregatedcounter (id, key, value, expireat) FROM stdin;
1	stats:succeeded:2026-06-01	2	2026-06-30 22:50:00.66743-04
191	stats:succeeded:2026-06-05-01	1	2026-06-05 21:00:09.658571-04
236	stats:succeeded:2026-06-05-23	3	2026-06-06 19:57:26.730095-04
189	stats:succeeded:2026-06-05	22	2026-07-05 19:57:25.730095-04
196	stats:succeeded:2026-06-05-02	4	2026-06-05 22:50:08.181604-04
201	stats:succeeded:2026-06-05-03	1	2026-06-05 23:00:09.024999-04
7	stats:succeeded:2026-06-03	9	2026-07-03 19:02:15.151647-04
243	stats:succeeded:2026-06-06	4	2026-07-05 20:40:07.197675-04
244	stats:succeeded:2026-06-06-00	4	2026-06-06 20:40:08.197675-04
203	stats:succeeded:2026-06-05-05	2	2026-06-06 01:30:04.782276-04
3	stats:succeeded	93	\N
210	stats:succeeded:2026-06-05-14	5	2026-06-06 10:50:11.306911-04
224	stats:succeeded:2026-06-05-15	2	2026-06-06 11:10:14.627454-04
230	stats:succeeded:2026-06-05-17	2	2026-06-06 13:10:09.510761-04
24	stats:succeeded:2026-06-04	56	2026-07-04 19:30:05.605239-04
\.


--
-- TOC entry 5341 (class 0 OID 22953)
-- Dependencies: 221
-- Data for Name: counter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.counter (id, key, value, expireat) FROM stdin;
282	stats:succeeded:2026-06-06	1	2026-07-05 20:56:46.823885-04
283	stats:succeeded:2026-06-06-00	1	2026-06-06 20:56:47.823885-04
284	stats:succeeded	1	\N
285	stats:succeeded:2026-06-06	1	2026-07-05 21:00:08.616837-04
286	stats:succeeded:2026-06-06-01	1	2026-06-06 21:00:09.616837-04
287	stats:succeeded	1	\N
\.


--
-- TOC entry 5343 (class 0 OID 22959)
-- Dependencies: 223
-- Data for Name: hash; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.hash (id, key, field, value, expireat, updatecount) FROM stdin;
1	recurring-job:estado-prestamo	Queue	default	\N	0
2	recurring-job:estado-prestamo	Cron	*/10 * * * *	\N	0
3	recurring-job:estado-prestamo	TimeZoneId	UTC	\N	0
4	recurring-job:estado-prestamo	Job	{"Type":"IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Method":"Execute","ParameterTypes":"[]","Arguments":"[]"}	\N	0
5	recurring-job:estado-prestamo	CreatedAt	2026-06-01T02:32:59.4235310Z	\N	0
7	recurring-job:estado-prestamo	V	2	\N	0
8	recurring-job:estado-prestamo	LastExecution	2026-06-06T01:00:09.4008529Z	\N	0
6	recurring-job:estado-prestamo	NextExecution	2026-06-06T01:10:00.0000000Z	\N	0
9	recurring-job:estado-prestamo	LastJobId	95	\N	0
\.


--
-- TOC entry 5345 (class 0 OID 22966)
-- Dependencies: 225
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
77	231	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 05:30:04.664948-04	2026-06-06 01:30:04.782276-04	0
86	258	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 17:10:09.386128-04	2026-06-06 13:10:09.510761-04	0
90	270	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-06 00:00:08.166542-04	2026-06-06 20:00:08.265892-04	0
87	261	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 23:22:22.265641-04	2026-06-06 19:22:26.839317-04	0
93	279	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-06 00:40:08.133061-04	2026-06-06 20:40:08.197675-04	0
80	240	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 14:30:08.378233-04	2026-06-06 10:30:08.565193-04	0
83	249	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 15:00:12.977705-04	2026-06-06 11:00:13.04507-04	0
71	213	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 02:05:22.317033-04	2026-06-05 22:05:36.096666-04	0
84	252	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 15:10:14.564079-04	2026-06-06 11:10:14.627454-04	0
73	219	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 02:47:52.682387-04	2026-06-05 22:47:57.403451-04	0
75	225	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 03:00:08.959712-04	2026-06-05 23:00:09.024999-04	0
85	255	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 17:00:51.459838-04	2026-06-06 13:01:07.598527-04	0
91	273	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-06 00:10:08.906021-04	2026-06-06 20:10:08.971653-04	0
78	234	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 14:10:34.133724-04	2026-06-06 10:10:48.854726-04	0
88	264	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 23:30:05.062658-04	2026-06-06 19:30:05.453887-04	0
94	282	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-06 00:56:38.631419-04	2026-06-06 20:56:47.823885-04	0
81	243	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 14:40:09.269285-04	2026-06-06 10:40:09.428081-04	0
92	276	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-06 00:34:37.144778-04	2026-06-06 20:34:47.671223-04	0
70	210	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 01:00:09.509939-04	2026-06-05 21:00:09.658571-04	0
72	216	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 02:10:08.339701-04	2026-06-05 22:10:08.449383-04	0
79	237	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 14:20:07.104274-04	2026-06-06 10:20:07.249461-04	0
89	267	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 23:57:22.778369-04	2026-06-06 19:57:26.730095-04	0
74	222	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 02:50:08.125702-04	2026-06-05 22:50:08.181604-04	0
95	285	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-06 01:00:09.424749-04	2026-06-06 21:00:09.616837-04	0
76	228	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 05:25:03.493836-04	2026-06-06 01:25:09.712295-04	0
82	246	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-05 14:50:11.147571-04	2026-06-06 10:50:11.306911-04	0
\.


--
-- TOC entry 5347 (class 0 OID 22973)
-- Dependencies: 227
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
313	79	RecurringJobId	"estado-prestamo"	0
314	79	Time	1780669207	0
315	79	CurrentCulture	"es-MX"	0
316	79	CurrentUICulture	"es-MX"	0
325	82	RecurringJobId	"estado-prestamo"	0
326	82	Time	1780671011	0
327	82	CurrentCulture	"es-MX"	0
328	82	CurrentUICulture	"es-MX"	0
341	86	RecurringJobId	"estado-prestamo"	0
342	86	Time	1780679409	0
343	86	CurrentCulture	"es-MX"	0
344	86	CurrentUICulture	"es-MX"	0
345	87	RecurringJobId	"estado-prestamo"	0
346	87	Time	1780701741	0
347	87	CurrentCulture	"es-MX"	0
348	87	CurrentUICulture	"es-MX"	0
349	88	RecurringJobId	"estado-prestamo"	0
350	88	Time	1780702204	0
351	88	CurrentCulture	"es-MX"	0
352	88	CurrentUICulture	"es-MX"	0
361	91	RecurringJobId	"estado-prestamo"	0
362	91	Time	1780704608	0
363	91	CurrentCulture	"es-MX"	0
364	91	CurrentUICulture	"es-MX"	0
373	94	RecurringJobId	"estado-prestamo"	0
374	94	Time	1780707397	0
375	94	CurrentCulture	"es-MX"	0
376	94	CurrentUICulture	"es-MX"	0
317	80	RecurringJobId	"estado-prestamo"	0
318	80	Time	1780669808	0
319	80	CurrentCulture	"es-MX"	0
320	80	CurrentUICulture	"es-MX"	0
329	83	RecurringJobId	"estado-prestamo"	0
330	83	Time	1780671612	0
331	83	CurrentCulture	"es-MX"	0
332	83	CurrentUICulture	"es-MX"	0
333	84	RecurringJobId	"estado-prestamo"	0
334	84	Time	1780672214	0
335	84	CurrentCulture	"es-MX"	0
336	84	CurrentUICulture	"es-MX"	0
353	89	RecurringJobId	"estado-prestamo"	0
354	89	Time	1780703842	0
355	89	CurrentCulture	"es-MX"	0
356	89	CurrentUICulture	"es-MX"	0
365	92	RecurringJobId	"estado-prestamo"	0
366	92	Time	1780706076	0
367	92	CurrentCulture	"es-MX"	0
368	92	CurrentUICulture	"es-MX"	0
377	95	RecurringJobId	"estado-prestamo"	0
378	95	Time	1780707609	0
379	95	CurrentCulture	"es-MX"	0
380	95	CurrentUICulture	"es-MX"	0
357	90	RecurringJobId	"estado-prestamo"	0
358	90	Time	1780704008	0
359	90	CurrentCulture	"es-MX"	0
360	90	CurrentUICulture	"es-MX"	0
369	93	RecurringJobId	"estado-prestamo"	0
370	93	Time	1780706408	0
371	93	CurrentCulture	"es-MX"	0
372	93	CurrentUICulture	"es-MX"	0
277	70	RecurringJobId	"estado-prestamo"	0
278	70	Time	1780621209	0
279	70	CurrentCulture	"es-MX"	0
280	70	CurrentUICulture	"es-MX"	0
281	71	RecurringJobId	"estado-prestamo"	0
282	71	Time	1780625121	0
283	71	CurrentCulture	"es-MX"	0
284	71	CurrentUICulture	"es-MX"	0
285	72	RecurringJobId	"estado-prestamo"	0
286	72	Time	1780625408	0
287	72	CurrentCulture	"es-MX"	0
288	72	CurrentUICulture	"es-MX"	0
289	73	RecurringJobId	"estado-prestamo"	0
290	73	Time	1780627672	0
291	73	CurrentCulture	"es-MX"	0
292	73	CurrentUICulture	"es-MX"	0
293	74	RecurringJobId	"estado-prestamo"	0
294	74	Time	1780627808	0
295	74	CurrentCulture	"es-MX"	0
296	74	CurrentUICulture	"es-MX"	0
297	75	RecurringJobId	"estado-prestamo"	0
298	75	Time	1780628408	0
299	75	CurrentCulture	"es-MX"	0
300	75	CurrentUICulture	"es-MX"	0
301	76	RecurringJobId	"estado-prestamo"	0
302	76	Time	1780637102	0
303	76	CurrentCulture	"es-MX"	0
304	76	CurrentUICulture	"es-MX"	0
305	77	RecurringJobId	"estado-prestamo"	0
306	77	Time	1780637404	0
307	77	CurrentCulture	"es-MX"	0
308	77	CurrentUICulture	"es-MX"	0
309	78	RecurringJobId	"estado-prestamo"	0
310	78	Time	1780668633	0
311	78	CurrentCulture	"es-MX"	0
312	78	CurrentUICulture	"es-MX"	0
321	81	RecurringJobId	"estado-prestamo"	0
322	81	Time	1780670409	0
323	81	CurrentCulture	"es-MX"	0
324	81	CurrentUICulture	"es-MX"	0
337	85	RecurringJobId	"estado-prestamo"	0
338	85	Time	1780678850	0
339	85	CurrentCulture	"es-MX"	0
340	85	CurrentUICulture	"es-MX"	0
\.


--
-- TOC entry 5349 (class 0 OID 22980)
-- Dependencies: 229
-- Data for Name: jobqueue; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.jobqueue (id, jobid, queue, fetchedat, updatecount) FROM stdin;
\.


--
-- TOC entry 5351 (class 0 OID 22987)
-- Dependencies: 231
-- Data for Name: list; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.list (id, key, value, expireat, updatecount) FROM stdin;
\.


--
-- TOC entry 5353 (class 0 OID 22994)
-- Dependencies: 233
-- Data for Name: lock; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.lock (resource, updatecount, acquired) FROM stdin;
\.


--
-- TOC entry 5354 (class 0 OID 23000)
-- Dependencies: 234
-- Data for Name: schema; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.schema (version) FROM stdin;
23
\.


--
-- TOC entry 5355 (class 0 OID 23003)
-- Dependencies: 235
-- Data for Name: server; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.server (id, data, lastheartbeat, updatecount) FROM stdin;
\.


--
-- TOC entry 5356 (class 0 OID 23009)
-- Dependencies: 236
-- Data for Name: set; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.set (id, key, score, value, expireat, updatecount) FROM stdin;
1	recurring-jobs	1780708200	estado-prestamo	\N	0
\.


--
-- TOC entry 5358 (class 0 OID 23016)
-- Dependencies: 238
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
235	79	Enqueued	Triggered by recurring job scheduler	2026-06-05 14:20:07.182325-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T14:20:07.1816079Z"}	0
236	79	Processing	\N	2026-06-05 14:20:07.224747-04	{"ServerId": "x:31320:407015c8-be13-4110-8d5a-68237354e5b7", "WorkerId": "ca98c61b-68da-408a-bc06-4059146f2189", "StartedAt": "2026-06-05T14:20:07.2208908Z"}	0
237	79	Succeeded	\N	2026-06-05 14:20:07.250551-04	{"Latency": "125", "SucceededAt": "2026-06-05T14:20:07.2431133Z", "PerformanceDuration": "13"}	0
245	82	Processing	\N	2026-06-05 14:50:11.266342-04	{"ServerId": "x:31320:407015c8-be13-4110-8d5a-68237354e5b7", "WorkerId": "cdca388d-abf7-40f7-a29b-3fdbc9cebf11", "StartedAt": "2026-06-05T14:50:11.2487198Z"}	0
246	82	Succeeded	\N	2026-06-05 14:50:11.310247-04	{"Latency": "126", "SucceededAt": "2026-06-05T14:50:11.2959942Z", "PerformanceDuration": "21"}	0
247	83	Enqueued	Triggered by recurring job scheduler	2026-06-05 15:00:12.988711-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T15:00:12.9883419Z"}	0
249	83	Succeeded	\N	2026-06-05 15:00:13.046861-04	{"Latency": "46", "SucceededAt": "2026-06-05T15:00:13.0371785Z", "PerformanceDuration": "12"}	0
250	84	Enqueued	Triggered by recurring job scheduler	2026-06-05 15:10:14.574947-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T15:10:14.5742141Z"}	0
252	84	Succeeded	\N	2026-06-05 15:10:14.629257-04	{"Latency": "39", "SucceededAt": "2026-06-05T15:10:14.6202066Z", "PerformanceDuration": "16"}	0
256	86	Enqueued	Triggered by recurring job scheduler	2026-06-05 17:10:09.435613-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T17:10:09.4346897Z"}	0
257	86	Processing	\N	2026-06-05 17:10:09.463848-04	{"ServerId": "x:35048:b96573b6-0ea9-4568-80f2-6e530eff0a30", "WorkerId": "2a8f7dfb-70af-4832-9f4a-ec2732954d99", "StartedAt": "2026-06-05T17:10:09.4561063Z"}	0
258	86	Succeeded	\N	2026-06-05 17:10:09.514979-04	{"Latency": "84", "SucceededAt": "2026-06-05T17:10:09.4956958Z", "PerformanceDuration": "24"}	0
259	87	Enqueued	Triggered by recurring job scheduler	2026-06-05 23:22:22.570947-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T23:22:22.5164658Z"}	0
262	88	Enqueued	Triggered by recurring job scheduler	2026-06-05 23:30:05.182354-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T23:30:05.1359947Z"}	0
268	90	Enqueued	Triggered by recurring job scheduler	2026-06-06 00:00:08.183167-04	{"Queue": "default", "EnqueuedAt": "2026-06-06T00:00:08.1818743Z"}	0
275	92	Processing	\N	2026-06-06 00:34:37.692986-04	{"ServerId": "x:21460:dae83fbb-5621-48ed-9f6c-81a8d0df8e0c", "WorkerId": "d492b76a-56e6-4d2f-a492-26ea32cd7eb3", "StartedAt": "2026-06-06T00:34:37.6273368Z"}	0
281	94	Processing	\N	2026-06-06 00:56:39.167293-04	{"ServerId": "x:3380:ad2a0071-e820-493b-96b2-53ea4afec94d", "WorkerId": "197bb6fb-1854-40ca-acab-8fc8f13b8a89", "StartedAt": "2026-06-06T00:56:39.0684340Z"}	0
211	71	Enqueued	Triggered by recurring job scheduler	2026-06-05 02:05:22.704047-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T02:05:22.6206545Z"}	0
238	80	Enqueued	Triggered by recurring job scheduler	2026-06-05 14:30:08.425143-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T14:30:08.4242228Z"}	0
239	80	Processing	\N	2026-06-05 14:30:08.492876-04	{"ServerId": "x:31320:407015c8-be13-4110-8d5a-68237354e5b7", "WorkerId": "194d2f57-db93-4603-a480-76c4cde6e6b1", "StartedAt": "2026-06-05T14:30:08.4813322Z"}	0
240	80	Succeeded	\N	2026-06-05 14:30:08.57098-04	{"Latency": "127", "SucceededAt": "2026-06-05T14:30:08.5501374Z", "PerformanceDuration": "43"}	0
242	81	Processing	\N	2026-06-05 14:40:09.396474-04	{"ServerId": "x:31320:407015c8-be13-4110-8d5a-68237354e5b7", "WorkerId": "4653b7bb-82cd-4968-a795-5f38dfeb3693", "StartedAt": "2026-06-05T14:40:09.3912711Z"}	0
248	83	Processing	\N	2026-06-05 15:00:13.017824-04	{"ServerId": "x:31320:407015c8-be13-4110-8d5a-68237354e5b7", "WorkerId": "feb21417-5748-4585-8c67-d66cc57c685f", "StartedAt": "2026-06-05T15:00:13.0090589Z"}	0
251	84	Processing	\N	2026-06-05 15:10:14.597915-04	{"ServerId": "x:31320:407015c8-be13-4110-8d5a-68237354e5b7", "WorkerId": "1be3efa9-7e03-4eb9-b0e8-8f61248c6380", "StartedAt": "2026-06-05T15:10:14.5919775Z"}	0
253	85	Enqueued	Triggered by recurring job scheduler	2026-06-05 17:00:52.356899-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T17:00:51.9118562Z"}	0
260	87	Processing	\N	2026-06-05 23:22:22.825151-04	{"ServerId": "x:9820:27062f87-ceea-426b-87a9-4d0de4410ffd", "WorkerId": "67a36f2f-bfd6-4455-ba9e-d8835e506add", "StartedAt": "2026-06-05T23:22:22.7700852Z"}	0
263	88	Processing	\N	2026-06-05 23:30:05.320018-04	{"ServerId": "x:24064:f424b45e-bdf4-4b05-875b-a931a72ca120", "WorkerId": "31bf7ff0-4192-494a-8e6c-9d66ef0b05bb", "StartedAt": "2026-06-05T23:30:05.2766842Z"}	0
269	90	Processing	\N	2026-06-06 00:00:08.217633-04	{"ServerId": "x:14488:018b7a8d-ba1b-4aae-ac7b-d45cffad97d3", "WorkerId": "033b1504-e865-4e03-8cc4-8d0dd82cbae0", "StartedAt": "2026-06-06T00:00:08.2028529Z"}	0
276	92	Succeeded	\N	2026-06-06 00:34:47.711135-04	{"Latency": "564", "SucceededAt": "2026-06-06T00:34:47.5827820Z", "PerformanceDuration": "9870"}	0
282	94	Succeeded	\N	2026-06-06 00:56:47.832952-04	{"Latency": "556", "SucceededAt": "2026-06-06T00:56:47.7554317Z", "PerformanceDuration": "8565"}	0
241	81	Enqueued	Triggered by recurring job scheduler	2026-06-05 14:40:09.317018-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T14:40:09.3164149Z"}	0
243	81	Succeeded	\N	2026-06-05 14:40:09.432019-04	{"Latency": "132", "SucceededAt": "2026-06-05T14:40:09.4202000Z", "PerformanceDuration": "18"}	0
254	85	Processing	\N	2026-06-05 17:00:53.955904-04	{"ServerId": "x:35048:b96573b6-0ea9-4568-80f2-6e530eff0a30", "WorkerId": "2d3eca80-101f-48ad-83f9-36717aabf478", "StartedAt": "2026-06-05T17:00:52.8518935Z"}	0
261	87	Succeeded	\N	2026-06-05 23:22:26.863878-04	{"Latency": "573", "SucceededAt": "2026-06-05T23:22:26.8133981Z", "PerformanceDuration": "3974"}	0
264	88	Succeeded	\N	2026-06-05 23:30:05.461827-04	{"Latency": "269", "SucceededAt": "2026-06-05T23:30:05.4139157Z", "PerformanceDuration": "81"}	0
270	90	Succeeded	\N	2026-06-06 00:00:08.269713-04	{"Latency": "58", "SucceededAt": "2026-06-06T00:00:08.2430873Z", "PerformanceDuration": "18"}	0
277	93	Enqueued	Triggered by recurring job scheduler	2026-06-06 00:40:08.145805-04	{"Queue": "default", "EnqueuedAt": "2026-06-06T00:40:08.1453827Z"}	0
283	95	Enqueued	Triggered by recurring job scheduler	2026-06-06 01:00:09.455921-04	{"Queue": "default", "EnqueuedAt": "2026-06-06T01:00:09.4548446Z"}	0
265	89	Enqueued	Triggered by recurring job scheduler	2026-06-05 23:57:22.911152-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T23:57:22.8775911Z"}	0
271	91	Enqueued	Triggered by recurring job scheduler	2026-06-06 00:10:08.932894-04	{"Queue": "default", "EnqueuedAt": "2026-06-06T00:10:08.9326352Z"}	0
273	91	Succeeded	\N	2026-06-06 00:10:08.975139-04	{"Latency": "48", "SucceededAt": "2026-06-06T00:10:08.9651764Z", "PerformanceDuration": "10"}	0
278	93	Processing	\N	2026-06-06 00:40:08.170631-04	{"ServerId": "x:21460:dae83fbb-5621-48ed-9f6c-81a8d0df8e0c", "WorkerId": "d492b76a-56e6-4d2f-a492-26ea32cd7eb3", "StartedAt": "2026-06-06T00:40:08.1650488Z"}	0
284	95	Processing	\N	2026-06-06 01:00:09.521947-04	{"ServerId": "x:3380:ad2a0071-e820-493b-96b2-53ea4afec94d", "WorkerId": "197bb6fb-1854-40ca-acab-8fc8f13b8a89", "StartedAt": "2026-06-06T01:00:09.4957739Z"}	0
266	89	Processing	\N	2026-06-05 23:57:23.018937-04	{"ServerId": "x:14488:018b7a8d-ba1b-4aae-ac7b-d45cffad97d3", "WorkerId": "033b1504-e865-4e03-8cc4-8d0dd82cbae0", "StartedAt": "2026-06-05T23:57:22.9849559Z"}	0
272	91	Processing	\N	2026-06-06 00:10:08.950491-04	{"ServerId": "x:14488:018b7a8d-ba1b-4aae-ac7b-d45cffad97d3", "WorkerId": "6d4266ce-9d99-46b8-9caa-1a199b39ac6a", "StartedAt": "2026-06-06T00:10:08.9429983Z"}	0
279	93	Succeeded	\N	2026-06-06 00:40:08.200188-04	{"Latency": "40", "SucceededAt": "2026-06-06T00:40:08.1868864Z", "PerformanceDuration": "12"}	0
285	95	Succeeded	\N	2026-06-06 01:00:09.622372-04	{"Latency": "112", "SucceededAt": "2026-06-06T01:00:09.5845591Z", "PerformanceDuration": "47"}	0
208	70	Enqueued	Triggered by recurring job scheduler	2026-06-05 01:00:09.531607-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T01:00:09.5311910Z"}	0
209	70	Processing	\N	2026-06-05 01:00:09.565465-04	{"ServerId": "x:43252:25152a7a-7b9e-4dbb-9eb2-6905f1868fe4", "WorkerId": "45be1f11-d43f-4440-a963-19202cdc4144", "StartedAt": "2026-06-05T01:00:09.5569348Z"}	0
210	70	Succeeded	\N	2026-06-05 01:00:09.689366-04	{"Latency": "61", "SucceededAt": "2026-06-05T01:00:09.6392211Z", "PerformanceDuration": "67"}	0
212	71	Processing	\N	2026-06-05 02:05:22.963265-04	{"ServerId": "x:18716:8503ec37-fa78-4e6c-bb5e-5c3a60428939", "WorkerId": "f9c60ed8-b0e5-4dbf-8988-f8500cb645cb", "StartedAt": "2026-06-05T02:05:22.8476746Z"}	0
213	71	Succeeded	\N	2026-06-05 02:05:36.106421-04	{"Latency": "681", "SucceededAt": "2026-06-05T02:05:35.9518721Z", "PerformanceDuration": "12949"}	0
214	72	Enqueued	Triggered by recurring job scheduler	2026-06-05 02:10:08.36018-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T02:10:08.3594488Z"}	0
215	72	Processing	\N	2026-06-05 02:10:08.418648-04	{"ServerId": "x:18716:8503ec37-fa78-4e6c-bb5e-5c3a60428939", "WorkerId": "f9c60ed8-b0e5-4dbf-8988-f8500cb645cb", "StartedAt": "2026-06-05T02:10:08.4050261Z"}	0
216	72	Succeeded	\N	2026-06-05 02:10:08.45126-04	{"Latency": "84", "SucceededAt": "2026-06-05T02:10:08.4374604Z", "PerformanceDuration": "12"}	0
217	73	Enqueued	Triggered by recurring job scheduler	2026-06-05 02:47:52.823552-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T02:47:52.7804234Z"}	0
218	73	Processing	\N	2026-06-05 02:47:53.014036-04	{"ServerId": "x:12768:6d308ed7-4012-46f2-aacd-7e12578b0993", "WorkerId": "7b1684dc-96c9-43a8-bf50-c1bf94f5cb34", "StartedAt": "2026-06-05T02:47:52.9727295Z"}	0
219	73	Succeeded	\N	2026-06-05 02:47:57.407739-04	{"Latency": "344", "SucceededAt": "2026-06-05T02:47:57.3652549Z", "PerformanceDuration": "4338"}	0
220	74	Enqueued	Triggered by recurring job scheduler	2026-06-05 02:50:08.135829-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T02:50:08.1352193Z"}	0
221	74	Processing	\N	2026-06-05 02:50:08.15003-04	{"ServerId": "x:12768:6d308ed7-4012-46f2-aacd-7e12578b0993", "WorkerId": "7b1684dc-96c9-43a8-bf50-c1bf94f5cb34", "StartedAt": "2026-06-05T02:50:08.1429316Z"}	0
222	74	Succeeded	\N	2026-06-05 02:50:08.18378-04	{"Latency": "28", "SucceededAt": "2026-06-05T02:50:08.1678892Z", "PerformanceDuration": "13"}	0
223	75	Enqueued	Triggered by recurring job scheduler	2026-06-05 03:00:08.975448-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T03:00:08.9751617Z"}	0
224	75	Processing	\N	2026-06-05 03:00:09.004884-04	{"ServerId": "x:12768:6d308ed7-4012-46f2-aacd-7e12578b0993", "WorkerId": "f5a8a738-bf61-4faf-86b1-be30733e9b6a", "StartedAt": "2026-06-05T03:00:08.9996507Z"}	0
225	75	Succeeded	\N	2026-06-05 03:00:09.026587-04	{"Latency": "48", "SucceededAt": "2026-06-05T03:00:09.0171433Z", "PerformanceDuration": "8"}	0
226	76	Enqueued	Triggered by recurring job scheduler	2026-06-05 05:25:03.698322-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T05:25:03.6679502Z"}	0
227	76	Processing	\N	2026-06-05 05:25:03.878767-04	{"ServerId": "x:18504:f246a0f2-7e72-495b-9db7-ac33e34b5ae1", "WorkerId": "c6fd1b80-b452-4472-b736-b4bd030134a4", "StartedAt": "2026-06-05T05:25:03.8273837Z"}	0
228	76	Succeeded	\N	2026-06-05 05:25:09.742163-04	{"Latency": "402", "SucceededAt": "2026-06-05T05:25:09.6288528Z", "PerformanceDuration": "5731"}	0
229	77	Enqueued	Triggered by recurring job scheduler	2026-06-05 05:30:04.72578-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T05:30:04.7251948Z"}	0
230	77	Processing	\N	2026-06-05 05:30:04.750854-04	{"ServerId": "x:18504:f246a0f2-7e72-495b-9db7-ac33e34b5ae1", "WorkerId": "c6fd1b80-b452-4472-b736-b4bd030134a4", "StartedAt": "2026-06-05T05:30:04.7404487Z"}	0
231	77	Succeeded	\N	2026-06-05 05:30:04.784007-04	{"Latency": "91", "SucceededAt": "2026-06-05T05:30:04.7736324Z", "PerformanceDuration": "16"}	0
232	78	Enqueued	Triggered by recurring job scheduler	2026-06-05 14:10:34.862783-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T14:10:34.6626703Z"}	0
233	78	Processing	\N	2026-06-05 14:10:35.254106-04	{"ServerId": "x:31320:407015c8-be13-4110-8d5a-68237354e5b7", "WorkerId": "864b1985-8332-45cb-9fb3-35f0ead58837", "StartedAt": "2026-06-05T14:10:35.1559857Z"}	0
234	78	Succeeded	\N	2026-06-05 14:10:48.897552-04	{"Latency": "1143", "SucceededAt": "2026-06-05T14:10:48.7954689Z", "PerformanceDuration": "13481"}	0
244	82	Enqueued	Triggered by recurring job scheduler	2026-06-05 14:50:11.17653-04	{"Queue": "default", "EnqueuedAt": "2026-06-05T14:50:11.1758300Z"}	0
255	85	Succeeded	\N	2026-06-05 17:01:07.607288-04	{"Latency": "2619", "SucceededAt": "2026-06-05T17:01:07.4987735Z", "PerformanceDuration": "13410"}	0
267	89	Succeeded	\N	2026-06-05 23:57:26.73533-04	{"Latency": "250", "SucceededAt": "2026-06-05T23:57:26.6918822Z", "PerformanceDuration": "3661"}	0
274	92	Enqueued	Triggered by recurring job scheduler	2026-06-06 00:34:37.482223-04	{"Queue": "default", "EnqueuedAt": "2026-06-06T00:34:37.3777149Z"}	0
280	94	Enqueued	Triggered by recurring job scheduler	2026-06-06 00:56:38.934941-04	{"Queue": "default", "EnqueuedAt": "2026-06-06T00:56:38.8518402Z"}	0
\.


--
-- TOC entry 5360 (class 0 OID 23023)
-- Dependencies: 240
-- Data for Name: accesorios; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.accesorios (id_accesorio, nombre, descripcion, modelo, url_data_sheet, precio, id_equipo, tipo, estado_eliminado) FROM stdin;
2	cable usb	dasd	C-123	https://datasheet.example.com/c123.pdf	15.99	5	Electrónico	t
3	string	string	string	\N	777	7	string	t
16	FER2		FER	dsd	1	5	FER	t
17	aaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaa	https://aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	111	6	aaaaaaaaaaaaaaaaaaaaaaaaa	t
1	string	string	string	string	0	6	string	t
4	aaaa	string	string	\N	0	5	string	t
5	string	string	string	\N	0	5	string	t
6	string	string	string	\N	0	5	string	t
7	string	\N	string	\N	\N	5	\N	t
8	string	string	string	string	0	5	string	t
9	string	string	string	string	0	5	string	t
12	FER		FER	dsd	1	5	FER	t
13	FER		FER	dsd	1	5	FER	t
14	FER		FER	dsd	450	5	FER	t
10	Accesorio1	string	default		23	30	default	f
15	Accesorio3		default		590	45	default	f
\.


--
-- TOC entry 5392 (class 0 OID 23372)
-- Dependencies: 274
-- Data for Name: audit_logs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.audit_logs (id, admin_carnet, admin_nombre, accion, entidad, entidad_id, detalle, "timestamp") FROM stdin;
1	sistema		AtrasadoAutomatico	PrestamoEntity	230	Auto-rechazado por exceder fecha de inicio	2026-06-03 05:04:57.968706-04
2	sistema		AtrasadoAutomatico	PrestamoEntity	228	Auto-rechazado por exceder fecha de inicio	2026-06-03 05:04:59.092436-04
3	sistema		AtrasadoAutomatico	PrestamoEntity	229	Auto-rechazado por exceder fecha de inicio	2026-06-03 05:04:59.11666-04
4	12890061	Fernando	Recoger	PrestamoEntity	231	\N	2026-06-03 13:00:39.314541-04
5	12890061	Fernando	Devolver	PrestamoEntity	231	\N	2026-06-03 13:00:45.566309-04
6	12890061	Fernando	Crear	CarreraEntity	48	\N	2026-06-03 13:03:42.730319-04
7	12890061	Fernando	Eliminar	Prestamo	228	\N	2026-06-03 17:37:04.053889-04
8	12890061	Fernando	Eliminar	Carrera	48	\N	2026-06-03 17:38:08.003556-04
9	12890061	Fernando	Crear	PrestamoEntity	232	\N	2026-06-03 17:38:57.042305-04
10	12890061	Fernando	Aprobar	PrestamoEntity	232	\N	2026-06-03 17:39:06.976268-04
11	12890061	Fernando	Recoger	PrestamoEntity	232	\N	2026-06-03 17:39:10.612578-04
12	12890061	Fernando	Devolver	PrestamoEntity	232	\N	2026-06-03 17:39:18.705133-04
13	12890061	Fernando	Crear	Prestamo	233	\N	2026-06-03 21:45:36.983285-04
14	12890061	Fernando	Aprobar	Prestamo	233	\N	2026-06-03 21:46:02.761909-04
15	12890061	Fernando	Recoger	Prestamo	233	\N	2026-06-03 21:46:19.010525-04
16	12890061	Fernando	Devolver	Prestamo	233	\N	2026-06-04 00:11:09.650793-04
17	12890061	Fernando	Crear	Prestamo	234	\N	2026-06-04 00:15:32.301454-04
18	sistema		Rechazar	PrestamoEntity	234	Auto-rechazado por exceder fecha de inicio	2026-06-04 00:20:15.116688-04
19	12890061	Fernando	Crear	Prestamo	235	\N	2026-06-04 02:46:59.304736-04
20	sistema		Rechazar	PrestamoEntity	235	Auto-rechazado por exceder fecha de inicio	2026-06-04 02:50:08.212638-04
21	12890061	Fernando	Crear	Prestamo	236	\N	2026-06-04 12:00:25.755302-04
22	sistema		Rechazar	PrestamoEntity	236	Auto-rechazado por exceder fecha de inicio	2026-06-05 00:46:39.66447-04
23	12890061	Fernando	Crear	Prestamo	237	\N	2026-06-05 00:53:53.418886-04
24	12890061	Fernando	Editar	Equipo	125	\N	2026-06-05 00:58:28.563629-04
25	12890061	Fernando	Crear	Prestamo	238	\N	2026-06-05 00:58:47.950576-04
26	sistema		Rechazar	PrestamoEntity	237	Auto-rechazado por exceder fecha de inicio	2026-06-05 01:00:09.609708-04
27	sistema		Rechazar	PrestamoEntity	238	Auto-rechazado por exceder fecha de inicio	2026-06-05 01:00:09.62739-04
28	12890061	Fernando	Crear	Prestamo	239	\N	2026-06-05 01:01:31.412742-04
29	12890061	Fernando	Rechazar	Prestamo	239	\N	2026-06-05 01:01:41.984142-04
30	12890061	Fernando	Crear	Prestamo	240	\N	2026-06-05 02:12:03.17475-04
31	sistema		Rechazar	PrestamoEntity	240	Auto-rechazado por exceder fecha de inicio	2026-06-05 02:47:56.741735-04
32	12890061	Fernando	Crear	Prestamo	241	\N	2026-06-05 02:57:34.024847-04
33	12890061	Fernando	Editar	Equipo	131	\N	2026-06-05 03:01:05.48879-04
34	12890061	Fernando	Rechazar	Prestamo	241	\N	2026-06-05 17:14:34.372209-04
35	12890061	Fernando	Rechazar	Prestamo	242	\N	2026-06-05 17:14:38.436154-04
36	12890061	Fernando	Rechazar	Prestamo	243	\N	2026-06-05 17:14:43.640293-04
37	12890061	Fernando	Rechazar	Prestamo	244	\N	2026-06-05 23:30:00.415131-04
38	12890061	Fernando	Aprobar	Prestamo	245	\N	2026-06-05 23:30:03.865809-04
39	12890061	Fernando	Recoger	Prestamo	245	\N	2026-06-05 23:30:09.630514-04
40	12890061	Fernando	Devolver	Prestamo	245	\N	2026-06-05 23:31:53.954576-04
41	12890061	Fernando	Crear	Prestamo	246	\N	2026-06-06 00:02:54.910616-04
42	12890061	Fernando	Aprobar	Prestamo	246	\N	2026-06-06 00:03:05.913046-04
43	12890061	Fernando	Recoger	Prestamo	246	\N	2026-06-06 00:03:09.728353-04
44	12890061	Fernando	Devolver	Prestamo	246	\N	2026-06-06 00:03:18.454845-04
45	12890061	Fernando	Crear	Prestamo	247	\N	2026-06-06 00:07:57.664393-04
46	12890061	Fernando	Aprobar	Prestamo	247	\N	2026-06-06 00:08:05.194095-04
47	12890061	Fernando	Recoger	Prestamo	247	\N	2026-06-06 00:08:08.817232-04
48	12890061	Fernando	Devolver	Prestamo	247	\N	2026-06-06 00:08:15.985716-04
\.


--
-- TOC entry 5380 (class 0 OID 23100)
-- Dependencies: 260
-- Data for Name: carreras; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.carreras (id_carrera, nombre, estado_eliminado) FROM stdin;
3	Inteligencia Artificial	f
4	Industrial	f
5	Civil	f
6	Biotecnología	f
7	Arquitectura	f
8	Diseño Digital	f
9	Agronomía y Zootecnia	f
10	Agronegocios	f
11	Administración de Empresas	f
12	Marketing y Medios Digitales	f
13	Comercial	f
14	Financiera	f
15	Medicina	f
16	Odontología	f
17	Kinesiología y Fisioterapia	f
18	Psicología	f
19	Fonoaudiología	f
20	Derecho	f
21	Filosofía y Letras	f
22	Trabajo Social	f
24	Ciencias Religiosas	f
25	Fer	t
32	prueba	t
38	W!=0f#5+!zw:g!ow-x<R2k}<*@>F:ezA*GDw(+c|(i38q}#mX^	t
39	W!=0f#5+!zw:g!ow-x<R2k}<*@>F:ezA*GDw(+c|(i38q}#mX^D	t
40	Ahh	t
41	AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA	t
42	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	t
1	Mecatronica	f
23	JJJJ	t
28	Fernando	t
27	JJ	t
2	Software	f
29	1234	t
26	EEEE	t
30	pruebaass	t
\.


--
-- TOC entry 5362 (class 0 OID 23030)
-- Dependencies: 242
-- Data for Name: categorias; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.categorias (id_categoria, nombre, estado_eliminado) FROM stdin;
8	prueba prueba	t
2	Fuente de alimentacion	t
11	JJJ	t
12	JJJJ	t
5	string	t
17	x	t
18	prueba2	t
4	Prueba	t
10	update_prueba	t
14	prueba	t
1	Impresora	t
19	Tecnologia	t
20	Mecanica	t
21	Automatizacion	t
3	Repuestos	t
22	Computadoras	t
23	Mecánica	f
24	Electrónica	f
25	Proyecto	f
26	Aeronáutica	f
27	Otros	f
28	Programación	f
29	Control	f
\.


--
-- TOC entry 5364 (class 0 OID 23035)
-- Dependencies: 244
-- Data for Name: componentes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.componentes (id_componente, descripcion, modelo, url_data_sheet, tipo, precio_referencia, nombre, id_equipo, estado_eliminado) FROM stdin;
2	bla bla bla	prueba	\N	prueba	\N	PRUEBA	5	t
5	ASD	ASD	https:ASDASD.com	ASD	0	111	5	t
3	\N	PRE	\N	MODULAR	\N	PRE	7	t
8		FERRR		FER	\N	Maquinas	5	t
11	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	11	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	5	t
4	stringaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	straingaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	https://sdasdasdasdasd	stringaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	11	stringaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	5	t
1	string	string	string	string	9.01	string	5	t
7		Terra		maquina	23	Maquina2	1	t
9		VDAS		enganche	0	Maquina3	4	t
6		Terra		maquina	200	Engranaje	45	t
10		ada-7		almacen	\N	Maquina1	37	t
\.


--
-- TOC entry 5383 (class 0 OID 23106)
-- Dependencies: 263
-- Data for Name: contratos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.contratos (id, contrato) FROM stdin;
1	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 2 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>FERRR </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 2 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 0 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td>\n          <td>0</td>\n          <td>0</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="5" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>0</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 4 días del mes de 5 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <div class="signature">\n    <div>\n\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBA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JBChLl6x1OM+UMf/H4E6wrRZ6nVFi8AcvvEodrujrLP0bgIECHwtIEAbCAQIVBKYzlDn36fty1Ad/1/OVguFjyuY29PFEyPjYTdeBAgQILCRgAC9EaTTECCwm0AG6fhzuo46L2im+mP66Zpz29PtNiydmACBKwsI0Feuvr4T6FcgQuI/a639vQ+zq/cz1Tkjncs/Rpl9zX4+mnGP2eZ4xa4n8YrlGm7M7Hd8azkBAsUFBOjiBdI8AgRmCUS4zP96X0+d/chQ/GjWfYqSgTofXCM4zxoyDiJAgMDrAgL063beSYBAbYE566mjBxk4f/bGTXYZUON88ff88x+01v7HhGD6b/cBN2+YzP8/Q3L871yaEn/P0Hz//mhjtO//3q6ZbbL+u/YY1DoCBAYVEKAHLaxuESDwicD9LPWjmxTPYpsuO4mgHIE5t/Y7q02uS4AAAQJvCAjQhgYBAlcXyNnh+z+fufzybTZ4Orucf//s9uYIw/GK/51/z/NmQLbk4pm0fydAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gL/H0fl0PpGZZTJAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>FERRR</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div></div></div>
2	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 15 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernando </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 0 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 1000 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td>\n          <td>1000</td>\n          <td>1000</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="5" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>1000</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 18 días del mes de 5 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <div class="signature">\n    <div>\n\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBA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JBChLl6x1OM+UMf/H4E6wrRZ6nVFi8AcvvEodrujrLP0bgIECHwtIEAbCAQIVBKYzlDn36fty1Ad/1/OVguFjyuY29PFEyPjYTdeBAgQILCRgAC9EaTTECCwm0AG6fhzuo46L2im+mP66Zpz29PtNiydmACBKwsI0Feuvr4T6FcgQuI/a639vQ+zq/cz1Tkjncs/Rpl9zX4+mnGP2eZ4xa4n8YrlGm7M7Hd8azkBAsUFBOjiBdI8AgRmCUS4zP96X0+d/chQ/GjWfYqSgTofXCM4zxoyDiJAgMDrAgL063beSYBAbYE566mjBxk4f/bGTXYZUON88ff88x+01v7HhGD6b/cBN2+YzP8/Q3L871yaEn/P0Hz//mhjtO//3q6ZbbL+u/YY1DoCBAYVEKAHLaxuESDwicD9LPWjmxTPYpsuO4mgHIE5t/Y7q02uS4AAAQJvCAjQhgYBAlcXyNnh+z+fufzybTZ4Orucf//s9uYIw/GK/51/z/NmQLbk4pm0fydAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gL/H0fl0PpGZZTJAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernando</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div></div></div>
3	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 16 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernando </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 0 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 1000 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td>\n          <td>1000</td>\n          <td>1000</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="5" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>1000</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 19 días del mes de 5 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <div class="signature">\n    <div>\n\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBA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JBChLl6x1OM+UMf/H4E6wrRZ6nVFi8AcvvEodrujrLP0bgIECHwtIEAbCAQIVBKYzlDn36fty1Ad/1/OVguFjyuY29PFEyPjYTdeBAgQILCRgAC9EaTTECCwm0AG6fhzuo46L2im+mP66Zpz29PtNiydmACBKwsI0Feuvr4T6FcgQuI/a639vQ+zq/cz1Tkjncs/Rpl9zX4+mnGP2eZ4xa4n8YrlGm7M7Hd8azkBAsUFBOjiBdI8AgRmCUS4zP96X0+d/chQ/GjWfYqSgTofXCM4zxoyDiJAgMDrAgL063beSYBAbYE566mjBxk4f/bGTXYZUON88ff88x+01v7HhGD6b/cBN2+YzP8/Q3L871yaEn/P0Hz//mhjtO//3q6ZbbL+u/YY1DoCBAYVEKAHLaxuESDwicD9LPWjmxTPYpsuO4mgHIE5t/Y7q02uS4AAAQJvCAjQhgYBAlcXyNnh+z+fufzybTZ4Orucf//s9uYIw/GK/51/z/NmQLbk4pm0fydAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gL/H0fl0PpGZZTJAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernando</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div></div></div>
4	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 16 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernan </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 1 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 1000 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td>\n          <td>1000</td>\n          <td>1000</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="5" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>1000</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 19 días del mes de 5 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <div class="signature">\n    <div>\n\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBA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JBChLl6x1OM+UMf/H4E6wrRZ6nVFi8AcvvEodrujrLP0bgIECHwtIEAbCAQIVBKYzlDn36fty1Ad/1/OVguFjyuY29PFEyPjYTdeBAgQILCRgAC9EaTTECCwm0AG6fhzuo46L2im+mP66Zpz29PtNiydmACBKwsI0Feuvr4T6FcgQuI/a639vQ+zq/cz1Tkjncs/Rpl9zX4+mnGP2eZ4xa4n8YrlGm7M7Hd8azkBAsUFBOjiBdI8AgRmCUS4zP96X0+d/chQ/GjWfYqSgTofXCM4zxoyDiJAgMDrAgL063beSYBAbYE566mjBxk4f/bGTXYZUON88ff88x+01v7HhGD6b/cBN2+YzP8/Q3L871yaEn/P0Hz//mhjtO//3q6ZbbL+u/YY1DoCBAYVEKAHLaxuESDwicD9LPWjmxTPYpsuO4mgHIE5t/Y7q02uS4AAAQJvCAjQhgYBAlcXyNnh+z+fufzybTZ4Orucf//s9uYIw/GK/51/z/NmQLbk4pm0fydAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gL/H0fl0PpGZZTJAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernan</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div></div></div>
5	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 16 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernan </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 1 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 1000 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Series</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td>Por definirse</td>\n          <td>\n          <strong>Rueda plana con diamante</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td>Por definirse</td>\n          <td>1</td>\n          <td>1000</td>\n          <td>1000</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="5" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>1000</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 31 días del mes de 5 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <div class="signature">\n    <div>\n\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBA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JBChLl6x1OM+UMf/H4E6wrRZ6nVFi8AcvvEodrujrLP0bgIECHwtIEAbCAQIVBKYzlDn36fty1Ad/1/OVguFjyuY29PFEyPjYTdeBAgQILCRgAC9EaTTECCwm0AG6fhzuo46L2im+mP66Zpz29PtNiydmACBKwsI0Feuvr4T6FcgQuI/a639vQ+zq/cz1Tkjncs/Rpl9zX4+mnGP2eZ4xa4n8YrlGm7M7Hd8azkBAsUFBOjiBdI8AgRmCUS4zP96X0+d/chQ/GjWfYqSgTofXCM4zxoyDiJAgMDrAgL063beSYBAbYE566mjBxk4f/bGTXYZUON88ff88x+01v7HhGD6b/cBN2+YzP8/Q3L871yaEn/P0Hz//mhjtO//3q6ZbbL+u/YY1DoCBAYVEKAHLaxuESDwicD9LPWjmxTPYpsuO4mgHIE5t/Y7q02uS4AAAQJvCAjQhgYBAlcXyNnh+z+fufzybTZ4Orucf//s9uYIw/GK/51/z/NmQLbk4pm0fydAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gL/H0fl0PpGZZTJAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernan</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div></div></div>
8	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 31 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernando </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">Por definirse</td>\n          <td class="ucb-code" data-grupo-id="24">Por definirse</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">Por definirse</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 2 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 499999.5 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">Por definirse</td>\n          <td class="ucb-code" data-grupo-id="24">Por definirse</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">Por definirse</td>\n          <td>1</td>\n          <td>499999.5</td>\n          <td>499999.5</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="6" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>499999.5</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 3 días del mes de 6 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAQPv/AY78Qo9U6GvfAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernando</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div>
6	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 31 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernando </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">230000001</td>\n          <td class="ucb-code" data-grupo-id="24">0000as</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">804V06A0</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 1 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 499999.5 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">230000001</td>\n          <td class="ucb-code" data-grupo-id="24">0000as</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">804V06A0</td>\n          <td>1</td>\n          <td>499999.5</td>\n          <td>499999.5</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="6" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>499999.5</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 2 días del mes de 6 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAQPv/AY78Qo9U6GvfAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernando</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div>
7	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 31 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernando </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">230000002</td>\n          <td class="ucb-code" data-grupo-id="24">0001</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">801E33A6</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 2 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 499999.5 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">230000002</td>\n          <td class="ucb-code" data-grupo-id="24">0001</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">801E33A6</td>\n          <td>1</td>\n          <td>499999.5</td>\n          <td>499999.5</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="6" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>499999.5</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 3 días del mes de 6 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAQPv/AY78Qo9U6GvfAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernando</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div>
9	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 31 días del mes de mayo de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernando </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">230000001</td>\n          <td class="ucb-code" data-grupo-id="24">0000as</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">804V06A0</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 1 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 499999.5 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="24">230000001</td>\n          <td class="ucb-code" data-grupo-id="24">0000as</td>\n          <td>\n          <strong>Batería Litio‑Ion 12V max</strong>\n          <p>Marca:  Default </p>\n          <p>Modelo:  Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="24">804V06A0</td>\n          <td>1</td>\n          <td>499999.5</td>\n          <td>499999.5</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="6" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>499999.5</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 4 días del mes de 6 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQmAj8ndZa/Pc/b//BIUDgIgIC9EUKrZsECBAgsEggw3H+GW/+7BaY4+//eHK2H374+79ddHYHEyDQtYAA3XX5NJ4AAQIENhSIsPy91toPnpwzZ5zjz3/SWvtbrbVvmYXesBJORaC4gABdvECaR4AAAQKHCMQMcgbnCMZf3q46XZ7xaKlGvu+/tda+c0hLXYQAgdMFBOjTS6ABBAgQIHCiQMw6/9FtSUYE5N/68PcIw0teP70t7bCUY4maYwl0LCBAd1w8TSdAgACBVQKxjvm/3s6wJvxGCI/zxJ9rzrOqM95MgMBxAgL0cdauRIAAAQJ1BGLWOdY7vzrrfN+TCONxTiG6To21hMBuAgL0brROTOASArbxukSZh+rkdMnG1uuWpyHaTYVDDRudIfCxgABtRBAgsFRgerNVvjfWjf7x0hM5nsDBAkcstciZbV8TBxfX5QgcKSBAH6ntWgT6F4h1nl/dwnLMtn23tfZPbzdeCdD913fkHuR6562WbLxlFctCIkRbCz3yaNK3ywsI0JcfAgAIzBaImef4yHu6Q0GGBbNtsxkdeIJAfmoS4Tm2mos/93rl10T8QhlfF14ECAwoIEAPWFRdIrCDQASQCAT3wSNn9cy27YDulJsI/Ki19sVt/B4RaAXoTcrmJARqCwjQteujdQTOFog1o7/fWvuNNxqSAdps29mVcv1HAjk+/7S19o8OIvI1cRC0yxA4U0CAPlPftQnUFoggEE9me+/pahGw4yESAnTtWl6xdXnDYPQ9dsQ46pUz0Fvv8HFU+12HAIEZAgL0DCSHELigQCzZiFf++R7BX9/WRXuM8QUHSuEuxw2v8UtgjMulTxZc0y0Beo2e9xLoRECA7qRQmkngIIHcIzfWNM8NHfkYY99PDiqSyzwVOCs8R8PswvG0PA4g0L+AH3j911APCGwlkA+BiBut5obnuLYAvVUFnGcLgQzPZ+0MI0BvUUXnIFBcQIAuXiDNI3CQQPzQ//zJeue3mhJLOOLl+8lBxXKZNwXODs/RsN+93Xj7k9ba99WKAIExBfzAG7OuekVgiUA89OFnH94wZ73z/XnzJsLY3u7IG7WW9M+x1xCo8gTA3HPajbXXGHd6eVEBAfqihddtAh92zsj1zl+ueAy3sGAoVRA4c83zff/tA11hRGgDgZ0FBOidgZ2eQFGBV9c733cnZ/08SKVooS/QrErhObhzH2jb2F1g8OnidQUE6OvWXs+vK7BmvfO9Wq5/PnqrsOtWT8+nAtXCc7QtlzXF32NZ056PDTcaCBA4SUCAPgneZQmcJLBkf+dnTZwGBd9Lnmn5960FKobn7GPuTCNAb1115yNQRMAPvSKF0AwCOwtssd75vom5/tkNhDsXz+k/EcixV/WTjwzQVdtnSBEgsFJAgF4J6O0EOhDYar3zfVczJFj/3MEgGKiJucb4rH2e51BW2E5vTjsdQ4DAiwIC9Itw3kagE4Et1ztPuzxdvlE5yHRSJs1cIBDhNF6VHx2fN9faym5BYR1KoCcBAbqnamkrgWUC8UM8gu4eQeNHrbUvbs3xfWRZXRz9ukAu3ag+5jJA24nj9Vp7J4HSAtW/CZXG0zgCRQVyvfNXH9r3ysNR5nTrz1prv9pa+8vW2q/MeYNjCKwUyE89elhXnHtBC9Ari+7tBKoKCNBVK6NdBF4TiJARH3HHsor44b3Ha7p8w/rnPYTnnTPWAscNnFfZJm3PT1Tmic8/KgO0G2znmzmSQFcCAnRX5dJYAu8K7HWz4P1F82P0+P9t03X8oIw6/4fW2t+9/aIU62xHf/U0+xy1yBsd4+9+zo4+OvXvkgK+sC9Zdp0eUCBC7bd3Wu98z5U7DJhdO24gRSD7/LamPf6erx6WM2yhFGMuxlt8stLDyx7pPVRJGwmsEBCgV+B5K4EiAkeHi3z6oOUbxwyACMw/+PDLUaxp/+z2ZwS0+P+u8D08Z3N7+mXB0wiP+dpwFQKnCVzhm+9puC5MYGeBPR6O8qzJ04+mewo0z/pV9d/f2oYwPwUY/Xt4D3s+Pxo7AnTVryjtIrCRwOjffDdichoC5QTOWhPq6YPHDYUIz1HnRzupxENs4hVr0Ed95RjvdSeL/KTGfQKjjlD9urSAAH3p8ut8pwJH3Sz4iMfyjWMGTdQ4AuRbNwhGgI41wXvs8X1MD59fpfc+CtDPa+wIAt0KCNDdlk7DLypw5M2C98SePnjMoAvn+O+9bQgjnI38lLtYohIGvc6wu4nwmK8VVyFwmoAAfRq9CxNYLHD2PrjT7et871hcvtlvCOf3HoDT+9KGZxC5vrvnNfYC9LMq+3cCnQv4Idh5ATX/MgJH77TxCNb2dfsPt/gl6dlWbRnORpyBHiE8xyjJGtnqcf+vGVcgcIqAAH0Ku4sSmC1wxk4bjxrn6YOzS/bSgbHm+V+21n5jxrtHDdD59L74BaL3h8Pk7iEC9IwB7RACPQoI0D1WTZuvInDWThuPfDPcxL/1/NF6xbGT+zzPvSFwxACd42uUvcWzP73uIFLx60SbCJQSEKBLlUNjCPxc4MydNh6VwfZ1+wzOXOv83prn+yuPFqBztnaksJlfL6P8QrDP6HdWAh0LCNAdF0/ThxWI2at4ylzMSMZHwBVeuSXXiOtuz/BdszQnA+cI4WzE8BzjSYA+46vKNQkcKCBAH4jtUgRmCJy5Td1bzbP+eUbhFhyy9tOFUcLZyDfaxc2g8YvwCOu5FwxthxK4joAAfZ1a62l9gbO3qXtLaLp9naeqrRtHbz2ae8lZsx69r0UfZceNR7UbuW9LxqpjCQwrIEAPW1od60ygwjZ1zwK0HQXWDaqtfkHK5TQ9f/8ePWBm/3qu0brR7t0EBhfwxT14gXWvvECuhf3qQ0uX3Eh2ZMesf16vHYFqixqPsOxh9PAcoyW+ZvzCuf7rxhkIlBUQoMuWRsMuILDmRrKjeKx/XicdfhEYYy3se4/mnnuV3tc/Z3ge4QbI92omQM8d0Y4j0KmAAN1p4TS7e4HcfaD6Otbp/s/WPy8bdrmbylbhOa7+09tT7qqPm0dSeWPd6Du5jLbN4LJR72gCFxEQoC9SaN0sJdBLeA40+z+/NnRipjWCVPzSsdWr5+UbV3qwSNZp9Fn2rca18xDoUkCA7rJsGt2xQG87KORH7qPPGm41pHJZTpxv7pMF5147x05vDxwZda/nt+qW/f1Ja+37c4vrOAIE+hIQoPuql9b2LZAfYffy8bv1z8vG295BMZdv9DSzubfJsgodc3TOtv9ea+0PjrmkqxAgcLSAAH20uOtdVSA/0t9yPezelhl+4jrWP7+vvffTAae/zPTyfTtNrrYbRW+fMu39fcT5CQwp0Ms34iHxdeoyAhmeKz2aew6+9c9zlH6xTnzPTxZ6q8U0PPc27udV/e2j7AG9VtD7CXQgIEB3UCRN7FogfpjGa+v1sEegXGXLsTWWR32ykHtx97B8Yxqee/rEZc04mL43atXbOvWt+u48BC4jIEBfptQ6erBA3kwWH19HiOjx1VNoO9p3erNg1DfqvNerp+UbVw/PduDY66vAeQkUExCgixVEc4YQGGEf2J5C29GD5ugb43pZvpEPjYk/91zOcnS9l1wvbyCMX6pi5xovAgQGFRCgBy2sbp0mkOGq9x+gvYS2owud9T1qW79edkIRnr8ZiW4gPPor0vUInCQgQJ8E77JDCuy9E8ORaNY/f6p9xmO0f9Ra++LWlMrfr3O8XHXmOUeLGwiP/C7lWgROFKj8DflEFpcmsFigp6cLzulcrn++eiC6D0ZHe+Tez3/ZWvuVOYU7+JhcCx7j/2ibg7s663LxdXO1bftmwTiIwGgCAvRoFdWfMwRiZvLz282Ccfd976+eHxm9h/1Zs6vT5RtVlwSdZbNHndee0w2EawW9n0BHAgJ0R8XS1JICo4XnQD5jqULF4ubsavx5xnZs+eTKqjOawvPHo3akJVwVvx61iUApAQG6VDk0pjOBCDj50fWe25gdzZLBqOqs5xEe0/AcT2E8+lX95kFj5NMR4QbCo79KXI/AiQIC9In4Lt21QM8PSHkP3vKNb34pivqe+TCMyrugmHl+/BXkBsKuv6VrPIFlAgL0Mi9HEwiBUcNz9K1CeDxzlFXpf9WH2Nid5e3R6QbCM79yXZvAwQIC9MHgLte1QO51GzOTvT5d8FkBrrz+OR+CcebMc9QnaxB/j+UjVZYHZXg+ag/sZ2O10r+P8PCkSp7aQqC8gABdvkQaWEQgZiZjzfOXt4BTpFmbNyNnPq/2vaFKeI6C5tZ1lW4ejNAcO80Iz4+/5HL8/HDw7w+bf8NxQgK9Clzth2SvddLucwVGebrgM8WcRTt7BvZZO7f+90qz7hW3rsvdQP6ktfbdrfEHOZ8bCAcppG4QmCsgQM+VctxVBUZ7QMp7dawUJI8abxkOq+w4Uu3mwfS52i9VS8efGwiXijmeQOcCAnTnBdT8XQWuFJ4D8mohoNpuEtW2rsswLzw//zbjBsLnRo4gMJSAAD1UOXVmQ4GrheegixBwlTWc1cJz+E9vHjz7e7OZ5/nfTDyBcL6VIwkMI3D2N+lhIHVkKIErhuerrPOezrR/57bXc5XBW2XruupPQKxSr2yH9c/VKqI9BA4QEKAPQHaJrgSuFCSnhclHkp/x1L2jBsjZj+Z+r5/T5RtnBnszz8tH49WWPi0X8g4CAwoI0AMWVZdeFrjyR7GjL9+YPmExAmqVvZVzsFaY9a3Qhpe/eE96oyd3ngTvsgTOFhCgz66A61cRyB+EVXZjONIl+/7PW2v/6cgLH3StKk8XfKu7FW4erLb7x0FDY/VlcmzFvQOxR3a8op7xin/L12eTf8tf3uK4/PvP3mhJ/Pv0l724odOLAIECAgJ0gSJowukCV3+K2MjLN6qH5xj8Z988OJ15rjg7f/o3iLtgHA9NyVc8XCaDcAbdryb/ngE4/pwG5vzf08D9qJ8ZxuPPCOF5jvvwHf8W151er4qbdhAYUkCAHrKsOrVA4KprnqdE8eS7EZ+wWOnpgu8NyXzy4BlP+ROeP65MBNPpDHKG1jwqw3EG1Vj/fPYTI6chOwJ9fC3HnzGrHWOq2nKlBd+eHUqgroAAXbc2Wra/wNVnnnP2KwLcaEtXclb3jFC6ZOSe+eTBvPktAlbUf/TlAdNwPP37tyehOcJnvDJ05rKMt2aH42vnKls/LhnXjiUwvIAAPXyJdfANAeH5G5gMmiN9L+glPE/9j57FzJnnaMOZu36s+QY1Xc4wnYWNQByzr/Fn/pI4XUsc/zb9ZeHVXxxG/NpZUw/vJXApgZF+aF6qcDq7SuDKu23cw8UMWgSImIEc4VXxASnvuZ6x9/N02UYPM8/x9RpLreLPCMURhuN/x8xvhuRYWpGBOsZz/P3VYDz368DTB+dKOY7AgAIC9IBF1aV3BaZ3zccM0pVfo+080lt4nt48GPtvH7FWteqyjVxSkTtXZFDO3SlyBnnvUDz3+0HWbrSlT3P77zgClxcQoC8/BC4FIDx/XO6RPoLuLTxHJY7eOq5CeM6gnH/m+uMIxnmDXvz9iF8m1nzzi1n8+H4y8oOH1vh4L4HhBQTo4UusgzcB4fnToRCBKoJMzyEg2h9hJv7sYTlCVuHovZ+PDs/TGeVcZhF9j2Ccyy0iKFeZUV7yjdISsCVajiUwqIAAPWhhdesjAVvVPR4QvT99MIJMBMN49bZ/8VHLN/IXjPga2ONGxemMcmydFq9cixxBudeQ/N630KzdUctufDsnQKCggABdsCiatKmA8PyYM116DQE9PCBlTgjbI9ROZ7nzU4a115nujZzLLvL/i5Aca5XjGu9t+7bpF/aJJ4sbb+PV8yc3J/K5NIExBAToMeqoF++HRDf6fOrT89MHew/PUY29d98Io1zaEgE3Zujnvu6XX+SNfRGQc5/kqz6gw/KNuaPIcQQGFxCgBy/whbtn5vn94ve6fKOXpwu+pz9d/7zHHsw59qMNz8Lze2E5Z5ZHXIbx6rdGNw++Kud9BAYTEKAHK6jufC2QAWKPcDICcQa43nx+t7X2+wM8NXG6/nnr78HT8PzoKYxR+/glJF6xZjnXK+fNfcLy21/hZp9H+O6nDwQ2Etj6m/dGzXIaAi8LmHl+Ttfj8o0Mnb/XWvuD510sfcT0QSZbrqPN2fnofD5eOgNzrFu2FGPdsHDz4Do/7yYwlIAAPVQ5L98Z4XneEOjt6YMZOEdZy55bymXInVe194+aPpo71il/9iAwm11eJ/2/W2v/z82D6xC9m8AoAgL0KJXUD/s8zxsDvX0MPeIT37a8gTDq+V9aa39/Uv682U9gnvc1Meeo/Lr5V621H895g2MIEBhbQIAeu75X6V1vofDMuvT09MHRZp6j7lvcQJjLMnINc5z3r1prfzJZ33zmGBvx2j0uexqxDvpEoIyAAF2mFBryokAGkkc3TL14yqHf1ssuAj0+mnvOwJne5Lfk+2+G5h/cLpKPus6bAHt6CuMcp2rHjPDUzmqm2kOga4El38C77qjGDysQP9giTESA8Hou0MP2dSPOPGdlMkDPebBJHBv/TUNzrG+O2dAftda+2Onpgs9H0fWO6OHr5npV0WMCJwoI0Cfiu/RqgeljnFef7AIn6GF7vwzPvW2xN3f45E4ZbwXoRzPNGZrzGrkMJ5ZsfHfuhR33skCv2z6+3GFvJEDguYAA/dzIETUFhOfldam+jnP08BwVy/A7fcDJnNB8H57j/fGpSy7lWD4avGOuQE/3Dcztk+MIEFgpIECvBPT2UwR6Wcd7Cs47F628fd2oa57vy5Fh7Ce3G/+mDzO5n2m+f2/OhArPx35l+X5zrLerEehCQIDuokwaORHIj8BH/Yh/r2JX3qnkKuE5avvfW2u/fitybjcXN8A+m0nO+uV6/wjRXscIWP98jLOrEOhKQIDuqlyXb2wPa3irFqnqx9BXCM/3SzRijCx5KIzwfN5XlfXP59m7MoHSAgJ06fJo3ERAeF43HCqufx55t42oVprnVnOxRCN21JizA0dWO96bW6j51GXd18Ar7676i+crffEeAgQ2FBCgN8R0qt0EchZoyazdbo3p9MTVPoYeNTzfzzbH47rziYDTNcwRhue8rjBDP8fhrGOsfz5L3nUJFBcQoIsXSPO+FrDjxrqBUO1j6JzVi3AZfx/h9Wi2+b5vuX5/br8zPPvF8bwRUu0Xz/MkXJkAgY8EBGgDorqA8Ly+QpU+hs6lOCM8OfLRbPN7NwRmHb4146bBDM9zw/b6UeIM9wLVfvFUIQIECgkI0IWKoSmfCFRct9tjmao8hjjD83QP5B4958w2P+pXbCMYrwjQ771yeYvwfO7oqPSL57kSrk6AwCcCArRBUVUgwlYEiVgr+myLr6p9qNKuCh9DT3eSeBYgq7hN23E/2xwzzV+11uLPOa+52wjmzPMIM/RzXCof49OvytXRNgInCwjQJxfA5R8K2HFju4FR5WPonAWP9bw97WEcfrFzRqxfzn2bX1m3nbOZ7+2kkTPPwvN24//VM839hefV83sfAQKdCwjQnRdwwOZXCXyj0Fb4GLq3nSQePVo7llPMnW1+NHaeLUey5rnWV9yS9eq1Wq41BAgcIiBAH8LsIjMFMjybgZsJNuOws9c/9/TkyEezzXOeEjijDO3RMpqwiVc8zjs+dbHmeY7kMcec/XVzTC9dhQCBlwUE6JfpvHFjgXxgRHxMPneP3I2bMOTpzlz/nEtxKgfDnG2OEDt94MkryzTeGkDTT1XimFgSEjb5ipAeD1npaWnLkF8sk06d+XUzuq3+ERhCQIAeooxDdCJ2KBCety3lmcthqv9CNF2mkWubt5ptvq9iLt+I60RwFpi3Hedbn809GFuLOh+BAQUE6AGL2mGXfFy6T9HOXP+ca3rn7Hm8T+8/PesRs82P+vIXt9ntv2qt9XYT5VG1qXSdZ+vVK7VVWwgQOElAgD4J3mV/LtDbDWY9le6sIFCtpkfONt+Pj/wU4Cette/3NHgu3FbLNy5cfF0nMFdAgJ4r5bg9BKoFrT36eOY5zwgCVdY9nzXb/Kjeubb6zLHg2vME7L4xz8lRBC4vIEBffgicBiA870t/xj62ec0znzS49PHa+1bB2XsTiL2445fAHh/205u19hLoWkCA7rp83Tbeo4r3L90ZNxCetZa90mzz/pV1hb0Ezvilc6++OC8BAjsLCNA7Azv9JwK5L7C9nvcdHEevf57zpL2te2y2eWvRa5/vzJtury2v9wQ6FBCgOyxax03O8HzmR/wd8y1q+pEfRcfNcf/+tn/3EXsZR9D59u2j9jWP114E6uDhBWIrzRi/sVOKFwECBN4VEKANkKME8uYy4fkY8VhO8dWHS235QJC3Wh43K/55a+3Xduya2eYdcZ3664foRICO8Lzmke0oCRC4iIAAfZFCn9zNDM8xW+jmnP2LcWQYOGLXgh+31n7n9qCdeGLfEb8U7F8lV6gkcOQnNpX6rS0ECLwoIEC/COdtswUyPMcb4hHdR3zEP7txgx541A2Ee990FefPYGPsDDpYC3Rr73FcoIuaQIDA1gIC9NaizjcVEJ7PGQ9H3QyVWxHu8X0kxk6E53h5et854+gqVz36hturuOongaEF9vjBNzSYzs0WEJ5nU21+4BGBYM8HpuQvANbLbz40nPBOwOyzIUGAwEsCAvRLbN70RCB/KMVhPno/frgccQNh3Di49Zr26ZIN2xweP26ueMUjftm8oqs+ExheQIAevsSHd3Aant3Rfjj/1xfc+xHee+z5PP3Ewrg5Z9xc7apmn69Wcf0lsKGAAL0hplP9fCuooDCDeM6A2PsGwj22I8xAHjPa1jufM26ueFWzz1esuj4T2EhAgN4I0mk+Cs/Wrp43IDLg7vW1veXjuiPs5/n8wnXemLnilc0+X7Hq+kxgQ4G9fshu2ESn6kBgumxDeD63YHvOquVM8Q8/dHHtXszTWec4n4dXnDturnZ1+z5freL6S2BjAQF6Y9ALnm5645fwfP4AyMdcx82bW762WroxnXXOxybH0g0vAkcJmH0+Stp1CAwsIEAPXNwDuiY8H4C88BJ77MAx/YQhniT5auDNWefo0haz2AtpHE7ga4EtlyEhJUDgogIC9EULv0G3hecNEHdHGRTqAAAVyUlEQVQ4xR47cOQDU14NvdMdNmLWOc7jiZQ7FN8pnwrkWLS95lMqBxAg8J6AAG18vCIwDc9b7wX8Snu85xuBPXbgWPNQk+k4MetslFYQ+OntE5StlzhV6Js2ECBwoIAAfSD2QJfKGUnhuVZRM0Bv9XWds3Wv1Hk662x7ulrj5KqtyV8G1yxDuqqdfhMgcCew1Q9asNcQMPNcu85b7sCx5mmS1jrXHidXbd0ey5uuaqnfBC4vIEBffggsApjOPHvgxSK6Qw7OreXWbjE33Slj6brnHCPRYetMDym7i8wQ2PKXyxmXcwgBAqMLCNCjV3i7/gnP21nudaatduDIWi/dlnA6RiI8v7pbx14+zntNAdvWXbPuek1gVwEBelfeYU5uVrGPUsZH1GtnfeMBE9+7hd9YKzr39Wronnt+xxF4VcDs86ty3keAwJsCArTB8UxAeH4mVOPft7iBcPp0wCUzyMJzjTGgFZ8K5NdFLDnztEsjhACBzQQE6M0ohzyR8NxPWWPXi5g9XjJrPO3ddNeMJbPYdmTpZ4xcsaVmn69YdX0mcICAAH0AcqeXyI/yo/lLAlWn3e2+2Wse4T3dcWPJTYPTGetXg3v38DpQVmCPfdHLdlbDCBA4VkCAPta7l6vFGtgI0MJzLxX7pl4/+9DcpTtwTHfciI+446PuOa8129zNOb9jCKwViK+JGKcemrJW0vsJEPhEQIA2KO4Fpnv4WjfYz/iIpRRfvrDO89UlGGsf792PrJb2KJC/4HloSo/V02YCHQgI0B0U6cAmvroO9sAmutQbAvGI4qXLKF5dgrHm8d4KSOAIgVc/kTmiba5BgMAAAgL0AEXcqAvT8LxkHexGl3eaFQK5DGNJgH51CcZ0nJjdW1E0b91NwOzzbrROTIBACgjQxkIICM99j4Oo3+cvrl9e+suSpRt9j5UrtN7s8xWqrI8EThYQoE8uQIHLT8Pz0ifPFWi+JnwQiCUV8dS/ufvcvrpvc95capwYdlUFttgPvWrftIsAgUICAnShYpzQlOnH+ELRCQXY6JIRoKN+8d+zV4bgCNxLlnzEeWOdde5qMOdaz9ri3wlsLbDV4+y3bpfzESAwmIAAPVhBF3Rnun2Z8LwAruChERrmbNX16rrn6HLeOLhkq7uCVJo0sIDZ54GLq2sEqgkI0NUqckx7hOdjnI+4ypIbCHPpxish+K9vnXHj4BFVdY1XBHIP9KV7ob9yLe8hQODiAgL09QZABK6/aK39jdba/2qt/e3rEQzV47k3EL66ZV1g5VMpXwneQ2HrTFmBJb9Ilu2EhhEg0I+AAN1PrbZo6XTm+f+01v7mFid1jlMF5sy6rVm6MX2v7xenltrF3xGY83UAkAABApsJ+IG4GWX5E02DUDQ21sy6Eax82Z42cM4NhGu2nsvZ56Xb3T1tuAMIbCjwyoOENry8UxEgcDUBAfoaFZ9uVSc8j1XzZzcQZu1f2XXD7PNYY2XU3ph9HrWy+kWgsIAAXbg4GzXtPjybSdwItshpngXovPnvlU8czD4XKbJmvCkQv+T9x9barzEiQIDAkQIC9JHax1/rPjy7Cez4Gux5xQgPsa/zo10H4t/+TWvtt1trr/7SlOHb94k9q+jcawTiF8gY35ajrVH0XgIEFgv4wbiYrJs33Idnez13U7rZDX1r/XPUPmaPI0T/pLX2/dln/MWBZp9fQPOWQwU8svtQbhcjQGAqIECPOR4y/GTvXln/OqbMWL2KOv/WXZemTxqMf3t1Zs7s81hjZbTexC+JP5j5AKHR+q4/BAgUEBCgCxRh4ybkjgvT8BzrXyNEe40l8J9ba7/8IUR89WG2OZbnRHiOULF2qc4frlz6MZay3lQTsOdztYpoD4ELCgjQYxX9PjxH7165eWwslbF7E2Eil2xET2PWOQL0q6/cecM+4a8Ket/eAtY97y3s/AQIPBUQoJ8SdXFAhJ74OD+C1PQlPHdRvk0aGWNgi08Z1uwZvUlHnITAOwK2rDM8CBAoISBAlyjDqkbc3yyYJxOeV7Fe8s3T9dPfuqSATlcWsO65cnW0jcDFBATovgseszGx5vX+JTz3XdczWr/mcd9ntNc1ryVg3fO16q23BMoLCNDlS/RmA+932jDz3G8tK7Q8fxlbewNihb5ow3gC1j2PV1M9ItC1gADdZ/ke3SwYPTHz3Gc9z251zj7b7vDsSrj+IwHrno0LAgTKCQjQ5UryboPeWu8cwWfNnr99KWjt1gJuHNxa1Pm2ErDueStJ5yFAYFMBAXpTzl1P9l54dsPXrvRDnzyXbph9HrrMXXbOuucuy6bRBK4hIED3Uee3bhb0eO4+6le5lfnEQct/Klfpmm2z7vmadddrAl0ICND1y/TWeucffmh6rg2s3wstrCiQv5j5Raxida7dJuuer11/vSdQXkCArluitx6OEi02W1i3bj217KcfnloY4yyWAG3xEJae+q6tdQWse65bGy0jQOAmIEDXHApuFqxZl5FaZeeNkao5Vl/iF7uYJPBL3Vh11RsCQwkI0PXK+ePW2u88aJaP2evVqucW5fINS4F6ruJ4bbfuebya6hGBIQUE6DplfW/Jhodb1KnTKC3Jmwd9Dxilov33I36p++y2JWf/vdEDAgSGFvDDs0Z5v/fhB0c8WfDRK/Z3jgDtRWArAcs3tpJ0nq0EbFm3laTzECBwiIAAfQjzuxd5a5eNWP/3m621Pz2/iVowmIDHdg9W0AG6E+uePQxqgELqAoGrCAjQ51XaLhvn2V/9ytY/X30E1Op/TCJ86ZO2WkXRGgIE3hcQoM8ZIW/tsuGGrnPqcbWr5qcetkO8WuXr9deWdfVqokUECMwQEKBnIG18SKx1jjXP01fssBHhOf70IrC3gP2f9xZ2/jkCEZ7j+2HsQ+5FgACBrgQE6OPK9daSDbPOx9XAlb4RiB04Yo294GJEnClgy7oz9V2bAIFVAgL0Kr7Zb360ZMOs82w+B24oYAeODTGd6mUB4fllOm8kQKCCgAC9bxXMOu/r6+zLBfKXOQ/mWW7nHdsIxE2s3749bXCbMzoLAQIEDhYQoPcDf7S3c3xsbqum/cyd+blAjksP53lu5YjtBdw0uL2pMxIgcIKAAL09ev6AiD+nL2udt7d2xuUCP2qtfXG7aTVmAr0IHCWQy4di7X1MJngRIECgWwEBervSvbVcw1rn7Yydab3An7XWfvVDgPa1v97SGZYJWPe8zMvRBAgUFvBDdJviPNqaLmZYYtbZY7i3MXaW9QJuIFxv6AyvCXhYymtu3kWAQFEBAXpdYf6wtfbbD04R65wF53W23r29QK5/tpxoe1tnfFsglgp9drv/gxMBAgSGEBCg15Ux9tOdvgSTdZ7eva9APsLb1/2+zs7+CwE3DRoNBAgMKeAH6bqyxvrmf9ha+/PW2m+6MWYdpnfvLhAfo8cyDg9Q2Z3aBT48WVV4NgwIEBhWQIAetrQ6RuAjgVz/7FMSA+MIAY/pPkLZNQgQOE1AgD6N3oUJHCogQB/KfemLxViLTzt80nHpYaDzBMYWEKDHrq/eEZgKxJp9N7gaE3sKZHj2wKg9lZ2bAIHTBQTo00ugAQQOE/jpbZ3+dw67ogtdSUB4vlK19ZXAxQUE6IsPAN2/lECsS82HWXgK4aVKv3tnhefdiV2AAIFKAgJ0pWpoC4H9BXIrO49T3t/6KlcQnq9Saf0kQODnAgK0wUDgegKWclyv5nv1OHfbiGVB8fRVLwIECFxCQIC+RJl1ksBHArmUI0JP7GXuReAVAVvVvaLmPQQIDCEgQA9RRp0gsFjgj24PVXFD4WI6b/CQFGOAAIGrCwjQVx8B+n9VgZyFjo/dbTl21VHwWr+/11r7vLXml6/X/LyLAIEBBAToAYqoCwReFPj11tq/u80mRpCOpxT+8Yvn8rZrCMRNqJ/dfum6Ro/1kgABAg8EBGjDggCB2EXhBx9uAouZRUHaeHhLILZA/OrDP9oC0RghQODyAgL05YcAAAI/FxCkDYZHAjEuYs18fELhplNjhAABAh++IQrQhgEBAvcCGZhinbQZ6WuPj/yl6kvh+doDQe8JEPhYQIA2IggQeEsgAnQs7cgg7WbDa40Vezxfq956S4DAAgEBegGWQwlcVCDWRkeQjtnI+AjfR/njDwQ7bYxfYz0kQGCFgAC9As9bCVxIIMJzhqr4e+zWEUHa0+fGGwR22hivpnpEgMDGAgL0xqBOR2BwgQzSMSMd4TnWxkaYFqTHKHzstJE1HaNHekGAAIEdBAToHVCdksAFBATpsYpsp42x6qk3BAjsLCBA7wzs9AQGF7gP0rlG2ox0P4V3s2A/tdJSAgSKCAjQRQqhGQQ6F7jfQzqCtK3P6hc1d1rxWO76tdJCAgQKCQjQhYqhKQQGEJjOSEd3cp10BGoP4ahV4HyioCcL1qqL1hAg0IGAAN1BkTSRQIcCEaRjdvPz258Zps1Mn19M653Pr4EWECDQuYAA3XkBNZ9ABwI5K/3tB2H6q9sstdnpYwoZWxH+i9bav/aJwDHgrkKAwJgCAvSYddUrAlUFcmY6wnSEuXzFUo/4LwK15R7bVy9nncPXko3tfZ2RAIGLCQjQFyu47hIoJBChLl6x1OM+UMf/H4E6wrRZ6nVFi8AcvvEodrujrLP0bgIECHwtIEAbCAQIVBKYzlDn36fty1Ad/1/OVguFjyuY29PFEyPjYTdeBAgQILCRgAC9EaTTECCwm0AG6fhzuo46L2im+mP66Zpz29PtNiydmACBKwsI0Feuvr4T6FcgQuI/a639vQ+zq/cz1Tkjncs/Rpl9zX4+mnGP2eZ4xa4n8YrlGm7M7Hd8azkBAsUFBOjiBdI8AgRmCUS4zP96X0+d/chQ/GjWfYqSgTofXCM4zxoyDiJAgMDrAgL063beSYBAbYE566mjBxk4f/bGTXYZUON88ff88x+01v7HhGD6b/cBN2+YzP8/Q3L871yaEn/P0Hz//mhjtO//3q6ZbbL+u/YY1DoCBAYVEKAHLaxuESDwicD9LPWjmxTPYpsuO4mgHIE5t/Y7q02uS4AAAQJvCAjQhgYBAlcXyNnh+z+fufzybTZ4Orucf//s9uYIw/GK/51/z/NmQLbk4pm0fydAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gL/H0fl0PpGZZTJAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernando</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div>
\.


--
-- TOC entry 5384 (class 0 OID 23111)
-- Dependencies: 264
-- Data for Name: detalles_mantenimientos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.detalles_mantenimientos (id_detalle_mantenimiento, id_mantenimiento, descripcion, id_equipo, estado_eliminado, tipo_mantenimiento) FROM stdin;
3	10	\N	7	t	preventivo
4	10	\N	6	t	correctivo
2	7	asdasd	2	t	preventivo
1	1	\N	7	t	preventivo
5	11	string	4	t	preventivo
6	12	string	4	t	preventivo
7	13	jjjbbjl	1	t	preventivo
8	13	 l kkkn k;ml;l llm	30	t	preventivo
9	13	null	3	t	correctivo
10	14	m,lmlml	1	t	correctivo
11	14	jnknknl	6	t	correctivo
12	14	 nlkmlmlm;,	4	t	preventivo
\.


--
-- TOC entry 5387 (class 0 OID 23119)
-- Dependencies: 267
-- Data for Name: detalles_prestamos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.detalles_prestamos (id_detalle_prestamo, id_equipo, id_prestamo, estado_eliminado, id_grupo_equipo, estado_equipo_retorno) FROM stdin;
178	131	224	f	28	\N
179	\N	225	f	24	\N
181	120	227	f	24	\N
180	121	226	f	24	\N
184	\N	230	f	24	\N
183	121	229	f	24	\N
185	120	231	f	24	\N
182	120	228	t	24	\N
186	659	232	f	23	\N
187	131	233	f	28	\N
188	\N	234	f	23	\N
189	\N	235	f	23	\N
190	\N	236	f	23	\N
191	\N	237	f	59	\N
192	\N	238	f	27	\N
193	\N	239	f	23	\N
194	\N	239	f	27	\N
195	\N	240	f	28	\N
196	\N	241	f	155	\N
197	\N	241	f	155	\N
198	\N	242	f	28	\N
199	\N	243	f	28	\N
200	\N	244	f	28	\N
201	131	245	f	28	\N
202	659	246	f	23	\N
203	132	247	f	29	\N
\.


--
-- TOC entry 5366 (class 0 OID 23042)
-- Dependencies: 246
-- Data for Name: empresas_mantenimiento; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.empresas_mantenimiento (id_empresa_mantenimiento, nombre, direccion, telefono, nit, estado_eliminado, nombre_responsable, apellido_responsable) FROM stdin;
6	JJJJJJJJJ111	string	string	string	t	string	string
8	Aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	111111111111	111111111111111	t	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
3	stringaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	JJJJJ11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111	111111111111	111111111111111	t	stringaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	stringaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
2	Electronics	Av. Alemana	774643247	123123	t	Fernando	Terrazas
1	Alura	string	745635345	23413123	t	Josue	Balbontin
7	LatamDC	Av. Beni 	735234123	231245	t	Alejandro	Ramirez
10	olas	\N	12345678	\N	t	eee	VALLEJOS
12	empresa	\N	1222222222	\N	f	Alejandro	Ramírez Vallejos
13	Estructuras de Datoss	\N	1111111111	\N	t	Alejandro	Ramírez Vallejos
14	Electronicsas	a	98328234	81238	t	Alejandro	Ramírez Vallejos
\.


--
-- TOC entry 5368 (class 0 OID 23049)
-- Dependencies: 248
-- Data for Name: equipos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.equipos (id_equipo, id_grupo_equipo, codigo_imt, descripcion, estado_equipo, numero_serial, ubicacion, costo_referencia, tiempo_max_prestamo, procedencia, id_gavetero, estado_eliminado, fecha_ingreso_equipo, codigo_ucb) FROM stdin;
77	11	10000087	\N	operativo	13125	Frente al laboratorio	0	9999	\N	1	t	2025-06-25	31245
78	14	40000009	\N	operativo	123145	Frente al laboratorio	0	9999	\N	1	t	2025-06-25	31245613
59	2	10000079	\N	operativo	64534	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	7456456
60	4	10000080	\N	operativo	84534	Frente al laboratorio	0	9999	\N	3	t	2025-06-25	846345
61	15	10000081	\N	operativo	123132	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	12345
62	10	30000007	\N	operativo	123132	Frente al laboratorio	0	9999	\N	1	t	2025-06-25	12345
63	10	30000008	\N	operativo	6412341	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	312356
64	11	10000082	\N	operativo	312356	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	312356
65	13	30000009	\N	operativo	231256	Frente al laboratorio	0	9999	\N	3	t	2025-06-25	312356
66	14	40000008	\N	operativo	62132155	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	3123
67	13	30000010	\N	operativo	31578	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	32131257
68	10	30000011	\N	operativo	4236788	4234679	0	9999	\N	7	t	2025-06-25	423467
69	15	10000083	\N	operativo	4123125	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	423423577
70	7	10000084	\N	operativo	4124123	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	3125
71	13	30000012	\N	operativo	312578	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	31231246
80	6	10000089	\N	operativo	312355	Frente al laboratorio	0	9999	\N	1	t	2025-06-25	31235
81	14	40000010	\N	operativo	12321456	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	312455
82	7	10000090	\N	operativo	1231245	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	312456
83	14	40000011	\N	operativo	4123125	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	3125213
84	7	10000091	\N	operativo	1231245	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	312456
85	14	40000012	\N	operativo	123123	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	3125123
86	11	10000092	\N	operativo	1231245	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	312456
89	12	20000004	\N	operativo	312567	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	5613215
87	12	20000002	\N	operativo	52135	123125	0	9999	Donado	2	t	2025-06-25	312312
88	12	20000003	\N	operativo	123164	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	1231245
90	8	20000005	\N	operativo	31231	Frente al laboratorio	0	9999	Donado	5	t	2025-06-25	312321
91	8	20000006	\N	operativo	412312	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	3123
92	8	20000007	\N	operativo	13123	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	3123
93	8	20000008	\N	operativo	54131	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	12312
3	1	34		operativo	13246123	Pared derecha lab	0	9999	Donado	2	t	2025-04-28	A412355
119	20	220000001		operativo	0000	Default	0	9999	Donado	7	t	2025-07-12	0000
126	27	240000005	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	16690
127	27	240000006	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	16691
128	27	240000007	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	16692
129	27	240000008	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	16693
130	27	240000009	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	16694
133	30	240000012	Lámpara de aumento Takema, nueva y sin uso. Ideal para trabajos de precisión gracias a su lente ampliadora y luz integrada.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	0000
132	29	240000011	Mini Dron XT FLYER con control remoto, cable de carga y 3 hélices. Ideal para principiantes, con diseño ligero y fácil de operar para vuelos cortos.	operativo	XT001B201603010387	Mueble Ventana - Part. Superior	0	9999	Default	9	f	2025-07-13	0000
122	27	240000001	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	t	2025-07-13	16685
123	27	240000002	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9	s	9	f	2025-07-13	16686
124	27	240000003	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	t	2025-07-13	16688
134	30	240000013	Lámpara de aumento Takema, nueva y sin uso. Ideal para trabajos de precisión gracias a su lente ampliadora y luz integrada.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	0000
1	16	40000006	string	operativo	451235	oficina del jefe de carrera	0	9999	string	3	t	2025-04-28	21351
79	11	10000088	\N	operativo	1231245	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	31245513
95	8	20000010	\N	operativo	62341	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	5234
96	8	20000011	\N	operativo	31234	Frente al laboratorio	0	9999	Donado	7	t	2025-06-25	3124
131	28	240000010	Mini Dron XT FLYER con control remoto, 2 baterías, cable USB, 4 hélices y manual incluidos. Compacto y fácil de manejar, perfecto para vuelos recreativos.	operativo	XT001B201603010372	Mueble Ventana - Part. Superior	3000	99	Default	9	f	2025-07-13	0000
97	10	30000015	\N	operativo	14123	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	3123
98	4	10000093	\N	operativo	54132	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	4213123
100	16	200000009	\N	operativo	31256	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	312356
101	13	30000016	\N	operativo	6143132	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	43123
102	12	20000012	\N	operativo	d3125143	Frente al laboratorio	0	9999	Donado	5	t	2025-06-25	e3124
103	12	20000013	\N	operativo	312456	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	31235
104	4	10000095	\N	operativo	123125	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	543125
94	8	20000009	\N	operativo	6134	Frente al laboratorio	0	9999	Donado	7	t	2025-06-25	31231
99	7	10000094	\N	operativo	41235	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	421356
105	4	10000096	\N	operativo	131256	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	31256
106	2	10000097	\N	operativo	3156	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	31246
107	2	10000098	\N	operativo	312456	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	31245
108	2	10000099	\N	operativo	31234	Frente al laboratorio	0	9999	Donado	5	t	2025-06-25	4123214
109	9	30000017	\N	operativo	31245	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	31245
110	9	30000018	\N	operativo	31235	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	431245
111	9	30000019	\N	operativo	4123123	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	4123124
112	9	30000020	\N	operativo	512312	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	3123
113	6	10000100	\N	operativo	531235	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	53123
114	6	10000101	\N	operativo	7534	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	5123125
115	6	10000102	\N	operativo	75645	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	8567567
116	6	10000103	\N	operativo	86435347	Frente al laboratorio	0	9999	Donado	1	t	2025-06-25	8745654
117	6	10000104	\N	operativo	86345	Frente al laboratorio	0	9999	Donado	2	t	2025-06-25	8635
118	6	10000105	\N	operativo	75345	Frente al laboratorio	0	9999	Donado	7	t	2025-06-25	86345
135	30	240000014	Lámpara de aumento Takema, nueva y sin uso. Ideal para trabajos de precisión gracias a su lente ampliadora y luz integrada.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	0000
136	31	240000015	Lámpara de aumento Takema, en buen estado de funcionamiento. Perfecta para tareas detalladas que requieren iluminación y aumento preciso.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-13	0000
138	38	240000017	Sensor de pinza de corriente de carga Kyoritsu, ideal para medir corriente sin interrumpir el circuito. Compatible con analizadores eléctricos para monitoreo preciso.	operativo	29565	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
139	38	240000018	Sensor de pinza de corriente de carga Kyoritsu, ideal para medir corriente sin interrumpir el circuito. Compatible con analizadores eléctricos para monitoreo preciso.	operativo	29570	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
140	37	240000019	Sensor de pinza de corriente de carga Kyoritsu, ideal para medir corriente sin interrumpir el circuito. Compatible con analizadores eléctricos para monitoreo preciso.	operativo	29571	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
141	39	240000020	Cables de prueba Kyoritsu diseñados para garantizar conexiones seguras y precisas en mediciones eléctricas. Resistentes y compatibles con instrumentos de la marca.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
402	133	230000040	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
143	43	230000003	Soporte de pared Premium para pantalla, funcional y resistente, ideal para una instalación segura y estable en entornos profesionales o domésticos.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
144	43	230000004	Soporte de pared Premium para pantalla, funcional y resistente, ideal para una instalación segura y estable en entornos profesionales o domésticos.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
145	43	230000005	Soporte de pared Premium para pantalla, funcional y resistente, ideal para una instalación segura y estable en entornos profesionales o domésticos.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
146	43	230000006	Soporte de pared Premium para pantalla, funcional y resistente, ideal para una instalación segura y estable en entornos profesionales o domésticos.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
147	43	230000007	Soporte de pared Premium para pantalla, funcional y resistente, ideal para una instalación segura y estable en entornos profesionales o domésticos.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
148	46	230000008	Motor a pasos JKongmotor usado, adecuado para aplicaciones que requieren control preciso de movimiento en proyectos y maquinaria.	operativo	180105	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
149	46	230000009	Motor a pasos JKongmotor usado, adecuado para aplicaciones que requieren control preciso de movimiento en proyectos y maquinaria.	operativo	180105	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
150	46	230000010	Motor a pasos JKongmotor usado, adecuado para aplicaciones que requieren control preciso de movimiento en proyectos y maquinaria.	operativo	180105	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
151	51	240000022	Driver para motor a pasos, dispositivo que controla la corriente y dirección del motor para movimientos precisos en aplicaciones electrónicas y robóticas.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
405	133	230000043		operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
495	154	240000190	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
152	51	240000023	Driver para motor a pasos, dispositivo que controla la corriente y dirección del motor para movimientos precisos en aplicaciones electrónicas y robóticas.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
499	154	240000194	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
500	154	240000195	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
501	154	240000196	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
503	154	240000198	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
504	154	240000199	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
153	51	240000024	Driver para motor a pasos, dispositivo que controla la corriente y dirección del motor para movimientos precisos en aplicaciones electrónicas y robóticas.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
154	51	240000025	Driver para motor a pasos, dispositivo que controla la corriente y dirección del motor para movimientos precisos en aplicaciones electrónicas y robóticas.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
155	51	240000026	Driver para motor a pasos, dispositivo que controla la corriente y dirección del motor para movimientos precisos en aplicaciones electrónicas y robóticas.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
156	51	240000027	Driver para motor a pasos, dispositivo que controla la corriente y dirección del motor para movimientos precisos en aplicaciones electrónicas y robóticas.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
157	52	240000028	Sensor capacitivo RHOMBERG.BRASLER con borneras para conexión, ideal para detección sin contacto de objetos sólidos o líquidos en aplicaciones industriales.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
158	52	240000029	Sensor capacitivo RHOMBERG.BRASLER con borneras para conexión, ideal para detección sin contacto de objetos sólidos o líquidos en aplicaciones industriales.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
159	52	240000030	Sensor capacitivo RHOMBERG.BRASLER con borneras para conexión, ideal para detección sin contacto de objetos sólidos o líquidos en aplicaciones industriales.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
160	54	240000031	Cable MicroLogix Allen Bradley, utilizado para la programación y comunicación entre PLCs MicroLogix y computadoras. Esencial para automatización industrial.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
161	54	240000032	Cable MicroLogix Allen Bradley, utilizado para la programación y comunicación entre PLCs MicroLogix y computadoras. Esencial para automatización industrial.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
162	55	240000033	Cable banana a cocodrilo, ideal para conexiones rápidas y seguras en pruebas eléctricas y de laboratorio. Versátil y fácil de usar.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
163	55	240000034	Cable banana a cocodrilo, ideal para conexiones rápidas y seguras en pruebas eléctricas y de laboratorio. Versátil y fácil de usar.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
164	55	240000035	Cable banana a cocodrilo, ideal para conexiones rápidas y seguras en pruebas eléctricas y de laboratorio. Versátil y fácil de usar.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
165	55	240000036	Cable banana a cocodrilo, ideal para conexiones rápidas y seguras en pruebas eléctricas y de laboratorio. Versátil y fácil de usar.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
352	124	260000026	\N	operativo	9450	G	0	9999	\N	18	f	2025-07-28	0000
354	124	260000028	\N	operativo	9450	G	0	9999	\N	18	f	2025-07-28	0000
166	56	240000037	Cable banana a punta, diseñado para realizar mediciones eléctricas precisas con multímetros y equipos de prueba. Seguro y fácil de manipular.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
167	56	240000038	Cable banana a punta, diseñado para realizar mediciones eléctricas precisas con multímetros y equipos de prueba. Seguro y fácil de manipular.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
168	57	240000039	Cable de alimentación 250V - 10A, adecuado para suministrar energía a equipos eléctricos de alta demanda. Robusto y seguro para uso industrial o doméstico.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
169	59	240000040	Adaptador verde 250V - 20A, diseñado para conexiones eléctricas seguras en aplicaciones de alta potencia. Robusto y fácil de instalar.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
170	59	240000041	Adaptador verde 250V - 20A, diseñado para conexiones eléctricas seguras en aplicaciones de alta potencia. Robusto y fácil de instalar.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
171	61	240000042	UNI-SOLVER de Smith & Nephew, removedor de adhesivos médico quirúrgicos, ideal para eliminar residuos de forma suave y efectiva sin dañar la piel.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
172	61	240000043	UNI-SOLVER de Smith & Nephew, removedor de adhesivos médico quirúrgicos, ideal para eliminar residuos de forma suave y efectiva sin dañar la piel.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
173	66	240000044	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
174	66	240000045	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
175	66	240000046	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
176	66	240000047	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
177	66	240000048	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
178	66	240000049	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
179	66	240000050	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
406	133	230000044	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
180	66	240000051	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
502	154	240000197	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
181	66	240000052	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
182	66	240000053	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
183	66	240000054	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
184	66	240000055	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
185	66	240000056	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
186	66	240000057	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
187	66	240000058	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
188	66	240000059	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
189	66	240000060	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
190	66	240000061	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
191	66	240000062	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
192	66	240000063	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
193	66	240000064	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
194	66	240000065	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
195	66	240000066	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
196	66	240000067	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
197	66	240000068	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
198	66	240000069	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
199	66	240000070	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
200	66	240000071	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
201	66	240000072	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
202	66	240000073	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
203	66	240000074	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
204	66	240000075	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
205	66	240000076	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
206	66	240000077	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
207	66	240000078	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
208	66	240000079	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
209	66	240000080	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
407	133	230000045	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
210	66	240000081	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
211	79	260000001	Dron UCB de 4 hélices desarmado, ideal para ensamblaje personalizado y aprendizaje práctico. Perfecto para aficionados y entusiastas de la tecnología.	operativo	0000	Mueble Pared Arriba	0	9999	\N	10	f	2025-07-15	0000
212	79	260000002	Dron UCB de 4 hélices desarmado, ideal para ensamblaje personalizado y aprendizaje práctico. Perfecto para aficionados y entusiastas de la tecnología.	operativo	0000	Mueble Pared Arriba	0	9999	\N	10	f	2025-07-15	17293
213	79	260000003	Dron UCB de 4 hélices desarmado, ideal para ensamblaje personalizado y aprendizaje práctico. Perfecto para aficionados y entusiastas de la tecnología.	operativo	0000	Mueble Pared Arriba	0	9999	\N	10	f	2025-07-15	17294
215	86	280000001	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
216	86	280000002	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
217	86	280000003	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
218	86	280000004	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
219	86	280000005	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
220	86	280000006	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
221	86	280000007	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
222	86	280000008	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
223	86	280000009	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	operativo	22x22x9	G	0	9999	\N	11	f	2025-07-27	S16539
224	88	240000082	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
225	88	240000083	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
226	88	240000084	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
227	88	240000085	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
228	88	240000086	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
353	124	260000027	\N	operativo	9450	G	0	9999	\N	18	f	2025-07-28	0000
229	88	240000087	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
230	88	240000088	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
232	88	240000090	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
233	89	240000091	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
234	89	240000092	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
237	89	240000095	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
239	89	240000097	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
231	88	240000089	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	operativo	DA2BAOF	G	0	9999	\N	12	f	2025-07-27	S16702
235	89	240000093	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
236	89	240000094	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
238	89	240000096	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
240	89	240000098	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	operativo	201230095316	G	0	9999	\N	12	f	2025-07-27	0000
241	90	240000099	Relé Temporizador ideal para automatizar procesos eléctricos con control de tiempo preciso, perfecto para aplicaciones industriales, domótica y proyectos electrónicos.	operativo	294675	G	0	9999	\N	13	f	2025-07-27	0000
242	90	240000100	Relé Temporizador ideal para automatizar procesos eléctricos con control de tiempo preciso, perfecto para aplicaciones industriales, domótica y proyectos electrónicos.	operativo	294675	G	0	9999	\N	13	f	2025-07-27	0000
243	90	240000101	Relé Temporizador ideal para automatizar procesos eléctricos con control de tiempo preciso, perfecto para aplicaciones industriales, domótica y proyectos electrónicos.	operativo	294675	G	0	9999	\N	13	f	2025-07-27	0000
244	90	240000102	Relé Temporizador ideal para automatizar procesos eléctricos con control de tiempo preciso, perfecto para aplicaciones industriales, domótica y proyectos electrónicos.	operativo	294675	G	0	9999	\N	13	f	2025-07-27	0000
245	90	240000103	Relé Temporizador ideal para automatizar procesos eléctricos con control de tiempo preciso, perfecto para aplicaciones industriales, domótica y proyectos electrónicos.	operativo	294675	G	0	9999	\N	13	f	2025-07-27	0000
246	90	240000104	Relé Temporizador ideal para automatizar procesos eléctricos con control de tiempo preciso, perfecto para aplicaciones industriales, domótica y proyectos electrónicos.	operativo	294675	G	0	9999	\N	13	f	2025-07-27	0000
247	91	240000105	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16308
248	91	240000106	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16309
249	91	240000107	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16310
250	91	240000108	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16311
251	91	240000109	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16312
252	91	240000110	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16313
253	91	240000111	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16317
254	91	240000112	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16320
255	91	240000113	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16322
256	91	240000114	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16324
257	91	240000115	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16325
258	91	240000116	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	operativo	411D171110	G	0	9999	\N	13	f	2025-07-27	S16327
259	92	240000117	Generador de señales versátil para crear formas de onda precisas, ideal para pruebas, desarrollo y diagnóstico en electrónica y telecomunicaciones.	operativo	36SC17131	G	0	9999	\N	14	f	2025-07-27	S16255
260	92	240000118	Generador de señales versátil para crear formas de onda precisas, ideal para pruebas, desarrollo y diagnóstico en electrónica y telecomunicaciones.	operativo	36SC17131	G	0	9999	\N	14	f	2025-07-27	S16256
261	92	240000119	Generador de señales versátil para crear formas de onda precisas, ideal para pruebas, desarrollo y diagnóstico en electrónica y telecomunicaciones.	operativo	36SC17131	G	0	9999	\N	14	f	2025-07-27	S16257
387	128	260000061	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
388	129	260000062	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
262	92	240000120	Generador de señales versátil para crear formas de onda precisas, ideal para pruebas, desarrollo y diagnóstico en electrónica y telecomunicaciones.	operativo	36SC17131	G	0	9999	\N	14	f	2025-07-27	S16275
263	92	240000121	Generador de señales versátil para crear formas de onda precisas, ideal para pruebas, desarrollo y diagnóstico en electrónica y telecomunicaciones.	operativo	36SC17131	G	0	9999	\N	14	f	2025-07-27	S16276
264	97	270000002	Lapicero inteligente 3D que permite crear figuras tridimensionales con precisión y facilidad, ideal para diseño, arte, educación y proyectos creativos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
265	97	270000003	Lapicero inteligente 3D que permite crear figuras tridimensionales con precisión y facilidad, ideal para diseño, arte, educación y proyectos creativos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
266	97	270000004	Lapicero inteligente 3D que permite crear figuras tridimensionales con precisión y facilidad, ideal para diseño, arte, educación y proyectos creativos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
267	98	270000005	Filamento 3D premium de alta calidad que garantiza impresiones precisas, resistentes y con excelente acabado, ideal para proyectos profesionales y creativos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
302	111	230000021	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
268	98	270000006	Filamento 3D premium de alta calidad que garantiza impresiones precisas, resistentes y con excelente acabado, ideal para proyectos profesionales y creativos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
269	98	270000007	Filamento 3D premium de alta calidad que garantiza impresiones precisas, resistentes y con excelente acabado, ideal para proyectos profesionales y creativos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
270	98	270000008	Filamento 3D premium de alta calidad que garantiza impresiones precisas, resistentes y con excelente acabado, ideal para proyectos profesionales y creativos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
271	99	270000009	Filamento 3D PLA ecológico y fácil de usar, ideal para impresiones detalladas con excelente adherencia y acabado, perfecto para usuarios de todos los niveles.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
272	99	270000010	Filamento 3D PLA ecológico y fácil de usar, ideal para impresiones detalladas con excelente adherencia y acabado, perfecto para usuarios de todos los niveles.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
273	99	270000011	Filamento 3D PLA ecológico y fácil de usar, ideal para impresiones detalladas con excelente adherencia y acabado, perfecto para usuarios de todos los niveles.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
274	99	270000012	Filamento 3D PLA ecológico y fácil de usar, ideal para impresiones detalladas con excelente adherencia y acabado, perfecto para usuarios de todos los niveles.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
275	99	270000013	Filamento 3D PLA ecológico y fácil de usar, ideal para impresiones detalladas con excelente adherencia y acabado, perfecto para usuarios de todos los niveles.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
276	102	240000122	Conector confiable y duradero, ideal para asegurar uniones eléctricas firmes en proyectos electrónicos, industriales o de automatización.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
277	102	240000123	Conector confiable y duradero, ideal para asegurar uniones eléctricas firmes en proyectos electrónicos, industriales o de automatización.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
278	103	240000124	Mangueras de alimentación resistentes y seguras, ideales para transportar energía eléctrica en instalaciones industriales, comerciales o proyectos técnicos.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
279	104	230000011	Pernos y arandelas esenciales para uniones mecánicas firmes y seguras, ideales en ensamblajes industriales, proyectos de construcción y electrónica.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
280	105	240000125	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
281	105	240000126	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
282	105	240000127	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
283	105	240000128	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
284	105	240000129	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
285	105	240000130	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
286	105	240000131	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
287	105	240000132	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
288	105	240000133	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
289	105	240000134	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
290	105	240000135	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
291	105	240000136	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	operativo	0000	G	0	9999	\N	15	f	2025-07-27	0000
292	106	230000012	Kit de excavadoras interactivo y educativo que permite armar modelos funcionales, ideal para aprender principios de mecánica, robótica y construcción de forma divertida.	operativo	0000	G	0	9999	\N	16	f	2025-07-27	0000
293	106	230000013	Kit de excavadoras interactivo y educativo que permite armar modelos funcionales, ideal para aprender principios de mecánica, robótica y construcción de forma divertida.	operativo	0000	G	0	9999	\N	16	f	2025-07-27	0000
389	129	260000063	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
390	129	260000064	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
294	107	230000014	Kit para Armar Reloj – Crea tu propio reloj personalizado con este completo kit. Incluye mecanismo, manecillas y todo lo necesario para ensamblarlo fácilmente en casa.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
295	107	230000015	Kit para Armar Reloj – Crea tu propio reloj personalizado con este completo kit. Incluye mecanismo, manecillas y todo lo necesario para ensamblarlo fácilmente en casa.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
296	108	240000137	Tubos termocontraíbles\nIdeales para aislar y proteger cables eléctricos. Se ajustan con calor para un acabado seguro y duradero.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
297	109	230000016	Kit para armar un reloj – Nuevo (ya armado)\nIncluye todas las piezas para crear tu propio reloj decorativo. Ya viene ensamblado, listo para usar o personalizar.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
403	133	230000041	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
298	109	230000017	Kit para armar un reloj – Nuevo (ya armado)\nIncluye todas las piezas para crear tu propio reloj decorativo. Ya viene ensamblado, listo para usar o personalizar.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
299	110	230000018	Juego de tornillos.\nSet completo de tornillos de distintos tamaños, ideal para reparaciones, bricolaje o proyectos domésticos.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
300	111	230000019	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
301	111	230000020	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
303	111	230000022	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
305	111	230000024	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
306	111	230000025	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
304	111	230000023	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
307	111	230000026	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
308	111	230000027	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
309	111	230000028	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
310	112	230000029	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
311	112	230000030	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
312	112	230000031	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
313	112	230000032	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
314	112	230000033	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
315	112	230000034	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
316	112	230000035	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
317	112	230000036	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
318	112	230000037	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
319	112	230000038	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
320	113	240000138	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
321	113	240000139	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
322	113	240000140	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
323	113	240000141	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
324	113	240000142	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
325	113	240000143	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
326	113	240000144	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
327	113	240000145	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	operativo	0000	G	0	9999	\N	16	f	2025-07-28	0000
328	114	260000004	Kit para armar dron JounyRC – Usado\nIncluye componentes esenciales para ensamblar un dron JounyRC. En buen estado y listo para volar con tus ajustes.	operativo	0000	G	0	9999	\N	17	f	2025-07-28	0000
329	114	260000005	Kit para armar dron JounyRC – Usado\nIncluye componentes esenciales para ensamblar un dron JounyRC. En buen estado y listo para volar con tus ajustes.	operativo	0000	G	0	9999	\N	17	f	2025-07-28	0000
391	129	260000065	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
392	129	260000066	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
330	114	260000006	Kit para armar dron JounyRC – Usado\nIncluye componentes esenciales para ensamblar un dron JounyRC. En buen estado y listo para volar con tus ajustes.	operativo	0000	G	0	9999	\N	17	f	2025-07-28	0000
331	117	240000146	Cargador de baterías IMAX – Nuevo\nCargador eficiente y confiable para baterías recargables, compatible con múltiples tipos y tamaños. Ideal para uso doméstico y profesional.	operativo	SKU:598000005-D	G	0	9999	\N	17	f	2025-07-28	0000
332	118	240000147	Cargador de baterías AC IMAX – Seminuevo\nCargador versátil y eficiente para baterías recargables, en excelente estado y listo para usar. Compatible con diversos tipos y tamaños.	operativo	CH-9-006-S	G	0	9999	\N	17	f	2025-07-28	0000
333	120	260000007	Hélices rojas (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
334	120	260000008	Hélices rojas (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
408	133	230000046	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
335	120	260000009	Hélices rojas (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
336	120	260000010	Hélices rojas (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
337	121	260000011	Hélices negras (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
338	121	260000012	Hélices negras (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
339	121	260000013	Hélices negras (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
340	121	260000014	Hélices negras (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
351	124	260000025	\N	operativo	9450	G	0	9999	\N	18	f	2025-07-28	0000
341	121	260000015	Hélices negras (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	operativo	1045R	G	0	9999	\N	18	f	2025-07-28	0000
342	122	260000016	Hélices negras (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
343	122	260000017	Hélices negras (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
344	122	260000018	Hélices negras (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
345	122	260000019	Hélices negras (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
346	122	260000020	Hélices negras (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
347	123	260000021	Hélices rojas (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
348	123	260000022	Hélices rojas (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
349	123	260000023	Hélices rojas (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
350	123	260000024	Hélices rojas (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	operativo	1045	G	0	9999	\N	18	f	2025-07-28	0000
355	124	260000029	\N	operativo	9450	G	0	9999	\N	18	f	2025-07-28	0000
356	124	260000030	\N	operativo	9450	G	0	9999	\N	18	f	2025-07-28	0000
357	125	260000031	\N	operativo	9450cw	G	0	9999	\N	18	f	2025-07-28	0000
358	125	260000032	\N	operativo	9450cw	G	0	9999	\N	18	f	2025-07-28	0000
359	125	260000033	\N	operativo	9450cw	G	0	9999	\N	18	f	2025-07-28	0000
360	125	260000034	\N	operativo	9450cw	G	0	9999	\N	18	f	2025-07-28	0000
361	125	260000035	\N	operativo	9450cw	G	0	9999	\N	18	f	2025-07-28	0000
362	125	260000036	\N	operativo	9450cw	G	0	9999	\N	18	f	2025-07-28	0000
363	126	260000037	\N	operativo	9450	G	0	9999	\N	18	f	2025-07-28	0000
364	127	260000038	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
365	127	260000039	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
366	127	260000040	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
367	127	260000041	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
368	128	260000042	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
369	128	260000043	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
370	128	260000044	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
371	128	260000045	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
372	128	260000046	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
373	128	260000047	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
374	128	260000048	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
375	128	260000049	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
376	128	260000050	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
377	128	260000051	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
378	128	260000052	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
379	128	260000053	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
380	128	260000054	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
381	128	260000055	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
382	128	260000056	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
383	128	260000057	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
384	128	260000058	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
385	128	260000059	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
386	128	260000060	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-28	0000
393	129	260000067	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
394	129	260000068	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
395	129	260000069	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
396	130	260000070	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
397	130	260000071	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
398	131	240000148	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
399	132	290000001	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
404	133	230000042	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
409	133	230000047	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
410	133	230000048	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
411	133	230000049	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
412	133	230000050	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
413	133	230000051	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
414	133	230000052	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
415	133	230000053	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
416	133	230000054	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
417	133	230000055	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
418	133	230000056	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
419	133	230000057	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
420	133	230000058	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
421	133	230000059	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
422	133	230000060	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
423	133	230000061	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
424	133	230000062	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
444	135	230000082	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
425	134	230000063	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
427	134	230000065	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
429	134	230000067	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
431	134	230000069	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
432	134	230000070	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
434	134	230000072	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
435	134	230000073	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
437	134	230000075	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
439	134	230000077	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
440	134	230000078	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
426	134	230000064	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
428	134	230000066	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
430	134	230000068	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
433	134	230000071	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
436	134	230000074	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
438	134	230000076	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
441	134	230000079	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
442	134	230000080	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
443	134	230000081	\N	operativo	0000	G	0	9999	Default	19	f	2025-07-29	0000
445	135	230000083	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
446	135	230000084	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
447	135	230000085	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
448	135	230000086	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
449	135	230000087	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
450	135	230000088	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
451	135	230000089	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
452	135	230000090	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
453	135	230000091	\N	operativo	0000	G	0	9999	\N	19	f	2025-07-29	0000
457	39	240000152	\N	operativo	0000	C	0	9999	\N	20	f	2025-07-29	0000
456	39	240000151	\N	operativo	0000	C	0	9999	Default	20	f	2025-07-29	0000
455	39	240000150	\N	operativo	0000	C	0	9999	Default	20	f	2025-07-29	0000
454	39	240000149	\N	operativo	0000	C	0	9999	Default	20	f	2025-07-29	0000
458	39	240000153	\N	operativo	0000	C	0	9999	\N	20	f	2025-07-29	0000
459	39	240000154	\N	operativo	0000	C	0	9999	\N	20	f	2025-07-29	0000
460	39	240000155	\N	operativo	0000	C	0	9999	\N	20	f	2025-07-29	0000
461	146	240000156	\N	operativo	138F17136	C	0	9999	\N	21	f	2025-07-29	1658
462	146	240000157	\N	operativo	138F17136	C	0	9999	\N	21	f	2025-07-29	16231
463	146	240000158	\N	operativo	138F17136	C	0	9999	\N	21	f	2025-07-29	16232
464	146	240000159	\N	operativo	138F17136	C	0	9999	\N	21	f	2025-07-29	16234
465	146	240000160	\N	operativo	138F17136	C	0	9999	\N	21	f	2025-07-29	16581
466	146	240000161	\N	operativo	138F17136	C	0	9999	\N	21	f	2025-07-29	16582
467	147	240000162	\N	operativo	0000	C	0	9999	\N	22	f	2025-07-29	6049
468	147	240000163	\N	operativo	0000	C	0	9999	\N	22	f	2025-07-29	6050
469	147	240000164	\N	operativo	0000	C	0	9999	\N	22	f	2025-07-29	6004
470	148	240000165	\N	operativo	886877	C	0	9999	\N	22	f	2025-07-29	6036
471	148	240000166	\N	operativo	886877	C	0	9999	\N	22	f	2025-07-29	6039
472	148	240000167	\N	operativo	886877	C	0	9999	\N	22	f	2025-07-29	6042
473	150	240000168	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
474	150	240000169	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
475	150	240000170	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
476	150	240000171	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
477	150	240000172	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
478	150	240000173	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
479	150	240000174	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
480	150	240000175	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
481	150	240000176	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
482	150	240000177	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
483	154	240000178	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
484	154	240000179	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
485	154	240000180	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
486	154	240000181	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
487	154	240000182	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
488	154	240000183	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
489	154	240000184	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
490	154	240000185	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
491	154	240000186	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
492	154	240000187	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
493	154	240000188	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
494	154	240000189	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
496	154	240000191	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
497	154	240000192	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
498	154	240000193	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
505	154	240000200	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
506	154	240000201	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
507	154	240000202	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
508	154	240000203	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
509	154	240000204	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
510	154	240000205	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
511	154	240000206	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
512	154	240000207	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
513	154	240000208	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
514	154	240000209	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
515	154	240000210	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
516	154	240000211	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
517	154	240000212	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
518	154	240000213	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
519	154	240000214	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
520	154	240000215	\N	operativo	0000	C	0	9999	\N	23	f	2025-07-29	0000
521	155	240000216	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
522	155	240000217	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
523	155	240000218	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
524	155	240000219	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
525	155	240000220	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
526	155	240000221	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
527	155	240000222	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
528	155	240000223	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
529	155	240000224	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
530	155	240000225	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
531	155	240000226	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
532	155	240000227	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
533	155	240000228	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
534	155	240000229	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
535	155	240000230	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
536	155	240000231	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
537	155	240000232	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
538	155	240000233	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
539	155	240000234	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
540	155	240000235	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
541	155	240000236	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
542	155	240000237	\N	operativo	0000	C	0	9999	\N	24	f	2025-07-29	0000
543	159	240000238	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
544	159	240000239	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
545	159	240000240	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
546	159	240000241	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
547	159	240000242	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
548	159	240000243	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
549	159	240000244	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
554	159	240000249	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
550	159	240000245	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
551	159	240000246	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
552	159	240000247	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
553	159	240000248	\N	operativo	0000	C	0	9999	\N	26	f	2025-07-29	0000
555	162	240000250	\N	operativo	3300016038420	C	0	9999	\N	26	f	2025-07-29	0000
556	162	240000251	\N	operativo	3300016038420	C	0	9999	\N	26	f	2025-07-29	0000
557	162	240000252	\N	operativo	3300016038420	C	0	9999	\N	26	f	2025-07-29	0000
558	162	240000253	\N	operativo	3300016038420	C	0	9999	\N	26	f	2025-07-29	0000
559	162	240000254	\N	operativo	3300016038420	C	0	9999	\N	26	f	2025-07-29	0000
560	167	230000092	\N	operativo	7807808874887	C	0	9999	\N	28	f	2025-07-29	0000
561	168	230000093	\N	operativo	0000	C	0	9999	\N	28	f	2025-07-29	0000
562	169	240000255	\N	operativo	7891265619617	C	0	9999	\N	28	f	2025-07-29	0000
563	169	240000256	\N	operativo	7891265619617	C	0	9999	\N	28	f	2025-07-29	0000
564	171	240000257	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
565	171	240000258	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
566	171	240000259	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
567	171	240000260	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
568	171	240000261	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
569	171	240000262	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
570	171	240000263	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
571	171	240000264	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
572	171	240000265	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
573	171	240000266	\N	operativo	0000	C	0	9999	\N	29	f	2025-07-29	0000
574	172	240000267	\N	operativo	0000	C	0	9999	\N	30	f	2025-07-29	0000
575	172	240000268	\N	operativo	0000	C	0	9999	\N	30	f	2025-07-29	0000
142	39	240000021	\N	operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
28	16	4	sdfsdfs	operativo	dfsdfsdf	\N	0	9999	\N	\N	t	2025-06-09	sdfsdfsfsdw4
27	5	5	\N	operativo	dfgfdg	\N	0	9999	\N	\N	t	2025-06-09	222222222222
13	5	7	PRUEBA PRUEBA	operativo	PRUEBA PRUEBA	PRUEBA PRUEBA	0	9999	PRUEBA PRUEBA	1	t	2025-05-17	\N
656	120	290000003	aaa	operativo	aaaaaa	Mueble Ventana - Part. Superior	111	11	111	24	f	2026-05-15	99999999
46	19	10000073	1aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	operativo	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	0	9999	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	8	t	2025-06-23	aaaaaaaaaaaaaaaaaaaa
11	5	6	PRUEBA PRUEBA	operativo	PRUEBA PRUEBA	PRUEBA PRUEBA	0	9999	PRUEBA PRUEBA	1	t	2025-05-17	\N
7	5	3	Prueba	inoperativo	\N	No existe	0	9999	\N	1	t	2025-04-28	\N
2	1	33	Impresora	operativo	\N	Pared derecha lab	0	9999	\N	\N	t	2025-04-28	JJJJJJ
5	4	1	Cable de potencia rojo y negro	operativo	56135	Pared de frente lab	0	9999	Donado	1	t	2025-04-28	A35612332
6	2	2	Soldamatic	operativo	312356	Pared derecha lab	0	9999	Donado	1	t	2025-04-28	51235
400	132	290000002	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
576	172	240000269	\N	operativo	0000	C	0	9999	\N	30	f	2025-07-29	0000
577	172	240000270	\N	operativo	0000	C	0	9999	\N	30	f	2025-07-29	0000
578	173	240000271	\N	operativo	0000	C	0	9999	\N	31	f	2025-07-29	0000
579	173	240000272	\N	operativo	0000	C	0	9999	\N	31	f	2025-07-29	0000
580	173	240000273	\N	operativo	0000	C	0	9999	\N	31	f	2025-07-29	0000
581	173	240000274	\N	operativo	0000	C	0	9999	\N	31	f	2025-07-29	0000
582	173	240000275	\N	operativo	0000	C	0	9999	\N	31	f	2025-07-29	0000
598	175	240000291	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
599	175	240000292	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
600	175	240000293	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
601	175	240000294	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
602	175	240000295	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
603	175	240000296	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
604	175	240000297	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
605	175	240000298	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
606	172	240000299	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
607	172	240000300	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
608	172	240000301	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
609	172	240000302	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
610	172	240000303	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
611	172	240000304	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
612	172	240000305	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
613	172	240000306	\N	operativo	0000	C	0	9999	\N	34	f	2025-07-29	0000
614	177	240000307	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
615	177	240000308	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
616	177	240000309	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
617	177	240000310	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
618	177	240000311	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
619	177	240000312	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
620	177	240000313	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
621	177	240000314	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
622	177	240000315	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
623	177	240000316	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
624	177	240000317	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
625	178	240000318	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
626	178	240000319	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
627	178	240000320	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
628	178	240000321	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
629	173	240000322	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
630	173	240000323	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
631	173	240000324	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
632	173	240000325	\N	operativo	0000	C	0	9999	\N	35	f	2025-07-29	0000
633	179	240000326	\N	operativo	0000	C	0	9999	\N	36	f	2025-07-29	0000
634	179	240000327	\N	operativo	0000	C	0	9999	\N	36	f	2025-07-29	0000
635	188	230000094	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
636	188	230000095	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
637	189	230000096	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
638	189	230000097	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
639	189	230000098	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
640	189	230000099	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
641	190	230000100	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
642	191	230000101	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
643	191	230000102	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
644	191	230000103	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
645	192	230000104	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
646	192	230000105	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
647	192	230000106	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
648	192	230000107	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
649	192	230000108	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
651	193	230000110	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
652	193	230000111	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
650	192	230000109	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
653	97	270000014	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
654	97	270000015	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
655	97	270000016	\N	operativo	0000	C	0	9999	\N	37	f	2025-07-29	0000
72	8	20000001	\N	operativo	561312	Frente al laboratorio	0	9999	\N	1	t	2025-06-25	4234123
4	16	10000006	string	operativo	string	Frente al laboratorio	0	9999	Donado	1	t	2025-04-28	21312
73	7	10000085	\N	operativo	131246	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	312564
45	1	10000072		operativo	123456	Frente al laboratorio	0	9999	Donado	3	t	2025-06-22	12353
37	1	10000007		operativo	ABCD	Frente al laboratorio	0	9999	Donado	3	t	2025-06-22	ABCD
30	16	40000007		operativo	1234	Frente al laboratorio	0	9999	Donado	3	t	2025-06-21	1234
29	1	70		operativo	4234123	Frente al laboratorio	0	9999	Donado	1	t	2025-06-18	4234123
74	13	30000013	\N	operativo	2312315	Frente al laboratorio	0	9999	\N	7	t	2025-06-25	312356
44	1	10000071		operativo	123563	Frente al laboratorio	0	9999	Donado	3	t	2025-06-22	34213
47	3	30000001	\N	operativo	45123245	Frente al laboratorio	0	9999	\N	1	t	2025-06-25	4213341
48	3	30000002	\N	operativo	5613125	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	63414
49	3	30000003	\N	operativo	12312	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	31233
50	3	30000004	\N	operativo	412312	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	41234123
51	3	30000005	\N	operativo	31231	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	31235
52	15	10000074	\N	operativo	61235	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	5123
53	15	10000075	\N	operativo	31256123	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	42341A
54	15	10000076	\N	operativo	5672341	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	6234123
55	15	10000077	\N	operativo	664123	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	234123
56	10	30000006	\N	operativo	1231256	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	312355
57	16	200000008	\N	operativo	31232	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	312321
58	1	10000078	\N	operativo	567234	Frente al laboratorio	0	9999	\N	2	t	2025-06-25	75234
75	9	30000014	\N	operativo	312315	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	312356
76	11	10000086	\N	operativo	5123125	Frente al laboratorio	0	9999	\N	5	t	2025-06-25	45123
214	85	270000001	\N	operativo	HC5712017	G	0	9999	\N	11	f	2025-07-15	0000
401	133	230000039	\N	operativo	0000	G	0	9999	\N	18	f	2025-07-29	0000
583	173	240000276	\N	operativo	0000	C	0	9999	\N	31	f	2025-07-29	0000
584	173	240000277	\N	operativo	0000	C	0	9999	\N	31	f	2025-07-29	0000
585	172	240000278	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
586	172	240000279	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
587	172	240000280	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
588	172	240000281	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
589	172	240000282	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
590	172	240000283	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
591	172	240000284	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
592	172	240000285	\N	operativo	0000	C	0	9999	\N	32	f	2025-07-29	0000
593	175	240000286	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
594	175	240000287	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
595	175	240000288	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
596	175	240000289	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
597	175	240000290	\N	operativo	0000	C	0	9999	\N	33	f	2025-07-29	0000
137	32	240000016	Lámpara de aumento Takema. Solo se incluye la caja, sin contenido en su interior.	operativo	0000	Mueble Ventana - Part. Superior	200	99	ss	9	t	2025-07-13	0000
657	21	290000004	as	operativo	451235	as	1000	12	1	24	f	2026-05-16	12222
121	24	230000002	Batería Litio-Ion 12V max Makita, en buen estado de funcionamiento y lista para su uso. Muestra desgaste estético leve propio del uso regular.	operativo	801E33A6	Mueble Ventana - Part. Superior	0	9	1111111111	9	f	2025-07-13	0001
658	25	290000005	as	inoperativo	i128381238	s	12	12	s	26	f	2026-05-16	i12i3i123i
659	23	290000006	12	operativo	812838213	a	12	12	as	\N	f	2026-05-16	128238
660	40	290000007	a	parcialmente_operativo	0129382	a	12	12	a	20	f	2026-05-16	91239821939
120	24	230000001	Batería Litio-Ion 12V max Makita, en buen estado de funcionamiento y lista para su uso. Muestra desgaste estético leve propio del uso regular.	operativo	804V06A0	Mueble Ventana - Part. Superior	999999	9	s	9	f	2025-07-13	0000as
125	27	240000004	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	operativo	0000	Mueble Ventana - Part. Superior	1002	9	s	9	f	2025-07-13	16689
\.


--
-- TOC entry 5370 (class 0 OID 23060)
-- Dependencies: 250
-- Data for Name: gaveteros; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.gaveteros (id_gavetero, nombre, tipo, estado_eliminado, id_mueble, longitud, profundidad, altura) FROM stdin;
4	aaAAAaa	string	t	3	\N	\N	\N
6	PRUEBA	ehh	t	6	1	1	1
8	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	t	8	1111	1111	111
1	M3	default	t	6	1	1	1
2	M5	default	t	6	1.4	2	0.5
5	M4	default	t	6	2	2	2
7	M2	default	t	6	1	1	1
3	M1	default	t	4	1.4	3.1	1.1
9	Mueble Ventana - Part. Superior	Default	f	9	\N	\N	\N
10	Mueble Pared Arriba	Default	f	10	\N	\N	\N
11	G01	Default	f	11	\N	\N	\N
12	G02	Default	f	11	\N	\N	\N
13	G03	Default	f	11	\N	\N	\N
14	G04	Default	f	11	\N	\N	\N
15	G06	Default	f	11	\N	\N	\N
16	G08	Default	f	11	\N	\N	\N
17	G09	Default	f	11	\N	\N	\N
18	G10	Default	f	11	\N	\N	\N
19	G12	Default	f	11	\N	\N	\N
20	C01	Default	f	12	\N	\N	\N
21	C02	Default	f	12	\N	\N	\N
22	C03	Default	f	12	\N	\N	\N
23	C04	Default	f	12	\N	\N	\N
24	C05	Default	f	12	\N	\N	\N
25	C06	Default	f	12	\N	\N	\N
26	C07	Default	f	12	\N	\N	\N
27	C08	Default	f	12	\N	\N	\N
28	C09	Default	f	12	\N	\N	\N
29	C10	Default	f	12	\N	\N	\N
30	C11	Default	f	12	\N	\N	\N
31	C12	Default	f	12	\N	\N	\N
32	C13	Default	f	12	\N	\N	\N
33	C14	Default	f	12	\N	\N	\N
34	C15	Default	f	12	\N	\N	\N
35	C16	Default	f	12	\N	\N	\N
36	C17	Default	f	12	\N	\N	\N
37	C18	Default	f	12	\N	\N	\N
\.


--
-- TOC entry 5372 (class 0 OID 23067)
-- Dependencies: 252
-- Data for Name: grupos_equipos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.grupos_equipos (id_grupo_equipo, nombre, modelo, url_data_sheet, cantidad, marca, id_categoria, estado_eliminado, url_imagen, descripcion, costo_promedio) FROM stdin;
17	string	string	string	0	string	5	t	string	string	0.00
18	prueba	TC-2022	http:prueba	0	DELL	3	t	http:prueba	LOREM	0.00
19	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	https://aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaasssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaa	0	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	1	t	https://dsdasddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	0.00
5	prueba	prueba	https::prueba	0	prueba	4	t	https://th.bing.com/th/id/OIP.u6bg7Q6XQdd5ZCfumbYt9AHaD4?cb=iwp1&rs=1&pid=ImgDetMain	prueba	0.00
3	Fuente de alimentación DC	   prueba		0	   prueba	3	t	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR9iF-fVeQ04UmJkJrtRgqMR5PZ8g_BqFwDwg&s	Aqui entra un texto descriptivo del equipo	0.00
22	Maletín con cajones	Default	https://www.makita.es/product/194884-7.html	0	Default	23	f	https://fi.makitamedia.com/images/3_Makita/304_accessories_GS1/30410_JPG_zoom/194884-7_C1C0.jpg	Maletín con cajones, funcional y práctico para organizar herramientas. Incluye brocas, aunque el juego está incompleto.	0.00
26	Taladro atornillador	Default	https://makita.bo/categoria/herramientas/atornilladores/atornilladores-atornilladores/	0	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTsyvnmRxMb8jOCIChXwLPKD50DtVmy1fFi5Q&s	Taladro atornillador Makita, funcional y confiable para trabajos de precisión. Presenta señales de uso superficial sin afectar su operación.	0.00
33	Analizador de calidad de energía y conjunto de sensores de pinza modelo	Default	\N	0	Default	24	t	https://techmasterdemexico.com/wp-content/uploads/KEW-6315-03.jpg	Analizador de calidad de energía Kyoritsu con sensores de pinza incluidos, ideal para monitorear y diagnosticar parámetros eléctricos. Contiene todos los componentes necesarios para su instalación y u	0.00
34	Analizador de Calidad de Energía	Default	\N	0	Default	24	f	https://techmasterdemexico.com/wp-content/uploads/KEW-6315-03.jpg	Analizador de Calidad de Energía Kyoritsu diseñado para medir y registrar parámetros eléctricos críticos. Ideal para mantenimiento preventivo y análisis de redes eléctricas.	0.00
41	Tarjeta SD	Default	https://www.kew-ltd.co.jp/en/products/detail/01236/	0	Default	24	f	https://www.kew-ltd.co.jp/files/co/photo_accessory/8326-02.jpg	Tarjeta SD Kyoritsu para almacenamiento de datos de mediciones eléctricas. Observación: el componente falta en el conjunto.	0.00
42	Bolsa	Default	https://www.kew-ltd.co.jp/en/products/detail/00468/	0	Default	24	f	https://www.kew-ltd.co.jp/files/co/photo_accessory/9125.jpg	Bolsa Kyoritsu resistente y práctica, diseñada para transportar y proteger instrumentos y accesorios de medición.	0.00
44	Soporte de pared para pantalla 	Default	\N	0	Default	23	f	https://hauscenter.com.bo/_next/image?url=https%3A%2F%2Fwww.dashboard.hauscenter.com.bo%2Fuploads%2F1_LGACLUTVCB_00001_1550bf8d4e.jpg&w=3840&q=75	Soporte de pared para pantalla DuraTech, nuevo y sellado, ofrece instalación segura y duradera. Ideal para uso residencial o comercial.	0.00
47	Potenciómetro de 10k	Default	\N	0	Default	24	f	https://www.330ohms.com/cdn/shop/products/photo_OS-10710_Potenciometro10K_01_700x700.png?v=1598042025	Potenciómetro de 10k, componente electrónico utilizado para ajustar niveles de voltaje o resistencia en circuitos eléctricos.	0.00
48	Potenciómetro de 50k	Default	\N	0	Default	24	f	https://www.steren.com.mx/media/catalog/product/cache/0236bbabe616ddcff749ccbc14f38bf2/image/151731252/potenciometro-miniatura-sin-switch-de-50-kohms.jpg	Potenciómetro de 50k, utilizado para controlar y ajustar señales eléctricas en diversos circuitos y dispositivos.	0.00
49	Potenciómetro de 100k	Default	\N	0	Default	24	f	https://i2celectronica.com/713/ptenciometro-de-audio-100k.jpg	Potenciómetro de 100k, ideal para regular voltajes y controlar parámetros eléctricos en circuitos electrónicos.	0.00
50	CNC shield V3	Default	https://tienda.sawers.com.bo/cnc-shield-controlador-para-ramps?search=CNC%20shield	0	Default	24	f	https://tienda.sawers.com.bo/image/cache/catalog/00034-500x500.jpg	CNC Shield V3, placa de expansión compatible con controladores Arduino para facilitar el manejo de motores paso a paso en proyectos de máquinas CNC.	0.00
102	Conectores	 Default		0	 Default	24	f	https://tienda.sawers.com.bo/image/cache/catalog/00657-500x500.jpg	Conector confiable y duradero, ideal para asegurar uniones eléctricas firmes en proyectos electrónicos, industriales o de automatización.	\N
25	Taladro llave de impacto	Default	https://www.lectura-specs.es/es/modelo/herramientas/herramientas-electricas-sin-cable-impulsoras-y-llaves-de-impacto-sin-cable-makita/td090d-11766255	1	Default	23	f	https://www.lectura-specs.es/models/renamed/orig/impulsoras-y-llaves-de-impacto-sin-cable-td090d-makita.jpg	Taladro con llave de impacto Makita, totalmente funcional y apto para tareas exigentes. Conserva marcas de uso moderado que no comprometen su desempeño.	12.00
32	Lámpara de Aumento	Default	\N	0	Default	24	f	https://irelectronics.pe/wp-content/uploads/2025/02/ZD-129ALED-0.webp	Lámpara de aumento Takema. Solo se incluye la caja, sin contenido en su interior.	0.00
40	Cable USB	Default	https://www.kew-ltd.co.jp/en/products/detail/01022/	1	Default	24	f	https://www.kew-ltd.co.jp/files/co/photo_accessory/7219.jpg	Cable USB Kyoritsu para transferencia de datos entre analizadores y computadoras. Facilita la descarga y gestión de mediciones eléctricas.	12.00
23	Cargador Litio‑Ion 7.2V ‑ 12V max	Default	https://www.makitatools.com/es/products/details/DC10WB	1	Default	23	f	https://cdn.makitatools.com/apps/cms/img/dc1/a8a1c3b5-b3a8-4033-858a-9945514d8fd1_dc10wb_p_1500px.png	Cargador Litio-Ion Makita 7.2V – 12V max, completamente funcional y compatible con baterías compactas. Presenta señales de uso que no afectan su rendimiento.	12.00
53	Interruptor de presión de latón	Default	\N	0	Default	24	f	https://tameson.es/cdn/shop/files/psl-b-n-1-10-c-fp_04.7cdb6fb5.jpg?v=1719653520	Interruptor de presión de latón Omega, diseñado para activar o desactivar circuitos eléctricos según la presión del sistema. Robusto y confiable para aplicaciones industriales.	0.00
58	Cable de calibre 18 (+2m)	Default	\N	0	Default	24	f	https://yorobotics.co/wp-content/uploads/2021/05/D_NQ_NP_910189-MCO31091395374_062019-O.jpg	Cable de calibre 18 (+2 m), ideal para conexiones eléctricas de baja corriente. Flexible y resistente, adecuado para proyectos electrónicos y de automatización.	0.00
60	Cable rojo calibre 18, 300V (+1 m)	Default	\N	0	Default	24	f	https://static.grainger.com/rp/s/is/image/Grainger/3GRL5_GC01	Cable rojo calibre 18, 300V (+1 m), ideal para conexiones eléctricas de baja a media tensión. Flexible, resistente y apto para aplicaciones electrónicas.	0.00
62	Nuprep 25g	Default	\N	0	Default	24	f	https://5.imimg.com/data5/SELLER/Default/2023/11/359232999/PN/MF/TI/6317077/nuprep-skin-prep-gel.jpg	Nuprep 25g de Weaver, pasta abrasiva para preparación de piel en procedimientos médicos, que mejora la conductividad y adhesión de electrodos.	0.00
64	Parche Biosensor	Default	\N	0	Default	24	f	https://www.lifesignals.com/wp-content/uploads/2024/04/2A-Ubiqvue-by-lifesignals-Biosensor-1.png	Parche Biosensor Life Signal, diseñado para monitoreo biomédico continuo. Estado: desarmado.	0.00
71	F.O. cable de 62.5mm (+5m) Delgado	Default	\N	0	Default	24	f	https://m.media-amazon.com/images/I/41mnq4IWACL.jpg	Cable de fibra óptica 62.5 mm Amp Netconnect, delgado y flexible, con longitud superior a 5 metros. Ideal para conexiones de alto rendimiento en redes de datos.	0.00
72	F.O. cable de 62.5mm (+5m) Grueso	Default	\N	0	Default	24	f	https://m.media-amazon.com/images/I/61Gn2UiJmfL._UF1000,1000_QL80_.jpg	Cable de fibra óptica 62.5 mm Amp Netconnect, versión gruesa y robusta, con más de 5 metros de longitud. Diseñado para instalaciones exigentes y entornos de alto tráfico de datos.	0.00
73	Cable LAN a Jack de audio	Default	\N	0	Default	24	f	https://media.cablematic.com/__sized__/images_1000/sh00000-01-thumbnail-1080x1080-70.jpg	Cable adaptador de LAN a jack de audio, ideal para aplicaciones específicas de integración de red y sonido. Permite la conexión entre dispositivos de red y sistemas de audio.	0.00
74	Cable LAN azul (1.33m)	Default	\N	0	Default	24	f	https://cdn-reichelt.de/resize/600%2F-/web/xxl_ws/E910%2FMK6001-0-15BL_01.png?type=ProductXxl&resize=600%252F-&	Cable LAN azul de 1.33 metros, ideal para conexiones de red estables en hogares u oficinas. Proporciona transmisión rápida y confiable de datos.	0.00
75	Cable telefónico plomo (0.97m)	Default	\N	0	Default	24	f	https://static.compreloadomicilio.com/dmraccesorios/products/026185/65e7611a886a4062396911.webp	Cable telefónico color plomo de 0.97 metros, ideal para conexiones cortas entre teléfonos y tomas de pared. Ofrece señal clara y conexión segura.	0.00
76	Cable telefónico negro (1.85m)	Default	\N	0	Default	24	f	https://ae01.alicdn.com/kf/Hef5d1ed8a19745639ef4c882b90faa45k.jpg_640x640q90.jpg	Cable telefónico negro de 1.85 metros, perfecto para conexiones confiables entre teléfonos y tomas de pared en espacios domésticos o de oficina.	0.00
77	Cable LAN rojo (1.37m)	Default	\N	0	Default	24	f	https://media.cablematic.com/__sized__/images_1000/rl00400-03-thumbnail-1080x1080-70.jpg	Cable LAN rojo de 1.37 metros, ideal para conexiones de red rápidas y seguras en entornos domésticos o laborales. Garantiza una transmisión de datos eficiente y estable.	0.00
95	Borneras de 4	Default	\N	0	Default	24	f	https://electronilab.co/wp-content/uploads/2022/04/Terminal-Bornera-4P-5.08mm-1.jpg	Borneras de 4 prácticas y seguras para conexiones eléctricas ordenadas, ideales para instalaciones industriales, residenciales y proyectos eléctricos.	0.00
96	Osciloscopio	Default	\N	0	Default	24	f	https://www.finaltest.com.mx/v/vspfiles/assets/images/osciloscopio-digital-tektronix.jpg	Osciloscopio esencial para visualizar y analizar señales eléctricas en tiempo real, ideal para diagnóstico, desarrollo y pruebas en electrónica.	0.00
100	Filamento PLA 3D rojo	Default	\N	0	Default	27	f	https://grilon3.com.ar/wp-content/uploads/2020/09/pla_roj2.jpg	Filamento PLA 3D rojo de alta calidad, fácil de imprimir y con un acabado brillante, ideal para crear piezas resistentes y visualmente impactantes.	0.00
101	Generador de funciones	Default	\N	0	Default	24	f	https://www.valiometro.pe/wp-content/uploads/2024/01/generador_de_funciones_formas_de_onda_2_canales_10mhz_peru_valiometro_1.jpg	Generador de funciones ideal para producir señales eléctricas con diferentes formas de onda, perfecto para pruebas, diseño y diagnóstico en electrónica.	0.00
115	Receptor con sistema	Default	\N	0	Default	24	f	https://ejemplo.com/imagen.jpg	Receptor con sistema – Nuevo\nReceptor completo con sistema integrado para control remoto. Ideal para proyectos de radiofrecuencia o modelismo.	0.00
116	Sensor de huella digital	Default	\N	0	Default	24	f	https://www.kimaldi.com/wp-content/uploads/2018/04/Biomini-slim-2-suprema_web-500x500.jpg	Sensor de huella digital – Nuevo\nPermite la identificación biométrica rápida y segura. Ideal para proyectos de seguridad, acceso y automatización.	0.00
119	Batería de alta capacidad	Default	\N	0	Default	24	f	https://ejemplo.com/imagen.jpg	Batería de alta capacidad – Buen estado\nBatería potente y duradera, ideal para dispositivos de alto consumo. Se encuentra en buen estado y lista para funcionar.	0.00
136	Llave de collets	Default	\N	0	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR0Q55Nk7eAhw8sk6T6ySkUG9yjxrjQRNsG9Q&s	Llave de collets – Nuevo\nHerramienta práctica y precisa para ajustar o retirar collets fácilmente. Ideal para mantenimiento de drones y motores RC.	0.00
137	Kit de láminas	Default	\N	0	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ9MpN8i1yU0CAF-3Fwk4URRaxa8EfFs0_mmg&s	Kit de láminas Carbide – Nuevo\nLáminas de carburo de alta resistencia para cortes precisos y duraderos. Perfectas para trabajos técnicos y de modelado.	0.00
138	Broca (Base de caja celeste)	Default	\N	0	Default	23	f	https://m.media-amazon.com/images/I/51wL0+QVP8L.jpg	Broca (Base de caja celeste) – Nuevo\nBroca de alta calidad presentada en caja celeste para mayor protección y fácil almacenamiento. Ideal para trabajos de precisión.	0.00
139	Broca (base de caja transparente)	Default	\N	0	Default	23	f	https://i5.walmartimages.com/asr/2ceae8af-4210-4f6b-9cfa-120d8b997c62.116f43459dda1b9a9b83760ed9fc81f0.jpeg?odnHeight=612&odnWidth=612&odnBg=FFFFFF	Broca (base de caja transparente) – Nuevo\nBroca duradera y precisa, presentada en caja transparente para fácil identificación y almacenamiento seguro. Perfecta para trabajos detallados.	0.00
140	Spindle to morse taper	Default	\N	0	Default	23	f	https://www.sherline.com/wp-content/uploads/2024/05/40272_pic.jpg	Spindle to Morse taper – Nuevo\nAdaptador de alta precisión para acoplar husillos a conos Morse, garantizando un montaje seguro y estable en maquinaria. Ideal para talleres y proyectos industriales.	0.00
141	ND-C tools	Default	\N	0	Default	23	f	https://i.ebayimg.com/images/g/~B4AAOSwHglfV0Ii/s-l1600.jpg	ND-C Tools – Nuevo\nHerramientas profesionales de alta calidad diseñadas para trabajos precisos y duraderos en electrónica y mecánica. Perfectas para uso técnico y especializado.	0.00
142	Marcador de números para metal	Default	\N	0	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcScfDdXM6EuOdzl9So8ow1u6mYfR4L4THUP1A&s	Marcador de números para metal – Nuevo\nHerramienta precisa para grabar números en superficies metálicas, ideal para identificación y marcado duradero. Fácil de usar y resistente.	0.00
165	Pulidora neumática	 Default		0	 Default	23	f	https://urrea.com/media/catalog/product/U/P/UP869P.jpeg?auto=webp&format=pjpg&fit=cover	Pulidora neumática – Buen estado\nHerramienta de aire comprimido ideal para trabajos de pulido y acabado en metal, madera o plástico. Funciona correctamente y está en buen estado.	0.00
143	Caja de herramientas para el torno (color azul)	Default	\N	0	Default	23	f	https://www.travers.com.mx/media/catalog/product/agility/img/78-008-657.jpg?optimize=high&fit=bounds&height=500&width=500&canvas=500:500	Caja de herramientas para torno (color azul) – Usado\nCaja resistente y práctica para organizar herramientas de torno, con señales de uso pero en buen estado funcional. Ideal para taller o aficionado.	0.00
144	USB	Default	\N	0	Default	23	f	https://sofmat.com.bo/wp-content/uploads/2023/10/HP-x770w-Memoria-Flash-USB-2.jpg	USB – Nuevo\nDispositivo de almacenamiento portátil y confiable para transferir y guardar tus archivos fácilmente. Compatible con múltiples dispositivos.	0.00
145	Collets kit	Default	\N	0	Default	23	f	https://m.media-amazon.com/images/I/61TuLEzJS7L._UF894,1000_QL80_.jpg	Collets kit – Usado\nSet de collets en buen estado, ideal para asegurar hélices y ejes en drones o modelos RC. Perfecto para proyectos de mantenimiento y reparación.	0.00
149	Motoreductor	Default	\N	0	Default	24	f	https://www.roydisa.es/wp-content/uploads/2012/12/CHA.jpg	Motoreductor – Buen estado\nMotor con reductor en óptimas condiciones, ideal para aplicaciones que requieren control de velocidad y torque. Listo para usar en proyectos industriales o robóticos.	0.00
151	Medidor de calor	Default	\N	0	Default	24	f	https://launchparaguay.com/wp-content/uploads/2017/02/medidor-de-temperatura-termometro-digital-infrarrojo-termometro-a-distancia.jpg	Medidor de calor – Buen estado\nDispositivo preciso para medir temperatura y calor, en buen estado y listo para uso en laboratorios o procesos industriales.	0.00
152	Kit de relojería (80 piezas)	Default	\N	0	Default	27	f	https://m.media-amazon.com/images/I/71iR1dn30uL.jpg	Kit de relojería 80 piezas – Buen estado\nCompleto set de herramientas para reparación y ajuste de relojes, ideal para aficionados o profesionales. Incluye estuche organizado y piezas en buen estado.	0.00
153	Puntas y Gancho multiuso	Default	\N	0	Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRNs0i9MyARnudiJ48QjO6ijcyUbgQIbDg5oA&s	Puntas y gancho multiuso – Buen estado\nHerramientas versátiles para manipulación, limpieza o reparación en espacios reducidos. Resistentes y en buen estado, ideales para trabajos de precisión.	0.00
160	Joystick antiguo	Default	\N	0	Default	24	f	https://m.media-amazon.com/images/I/61lXf75z8KL._SL1500_.jpg	Joystick antiguo\nJoystick usado con signos de antigüedad, aún funcional. Ideal para pruebas, proyectos electrónicos o piezas de repuesto.	0.00
166	Maguera para compresor	Default	\N	0	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR3RFYqGPC6BBHUbMgc_VvYoBWIewXUCuBdjw&s	Manguera para compresor – Nuevo\nManguera resistente y flexible, ideal para conectar herramientas neumáticas a compresores. Lista para uso industrial o doméstico.	0.00
170	Base magnética	Default	\N	0	Default	24	f	https://wakoimportaciones.com/wp-content/uploads/2020/04/02897.jpg	Base magnética Starrett – Nuevo\nSoporte magnético de alta precisión ideal para instrumentos de medición. Fijación firme y estable, perfecta para trabajos de inspección y mecanizado.	0.00
174	Cable para arduino	Default	\N	0	Default	24	f	https://apmelectronica.com/wp-content/uploads/2023/09/s-l1200-1.jpg	Cable para Arduino – Buen estado\nCable compatible y funcional para conexiones entre Arduino y sensores o módulos. En buen estado, ideal para proyectos de electrónica y prototipado.	0.00
176	Cable de poder tipo jack de 12V - 1.5A	Default	\N	0	Default	24	f	https://ferretronica.com/cdn/shop/products/FuentedeVoltaje-AdaptadordeCorriente12V-1.5A-1500mAparacamarasdeseguridad_dispositivoselectricosyelectronicos_modulosyTajetasArduino_Ferretronica.jpg?v=15904	Cable de poder tipo jack 12V - 1.5A – Buen estado\nCable resistente y funcional para alimentación de dispositivos electrónicos con conector jack. En buen estado, ideal para fuentes de 12V y 1.5A.	0.00
180	Bloque de Alumnio UCB	Default	\N	0	Default	23	f	https://m.media-amazon.com/images/I/41Xj-bCYfjL.jpg	Bloque de Aluminio UCB – Buen estado\nBloque de aluminio resistente y de alta calidad, ideal para proyectos industriales o mecánicos. En buen estado y listo para su uso.	0.00
181	Bloque de madera UCB	Default	\N	0	Default	23	f	https://master.opitec.com/out/pictures/master/product/1/667433-000-000-VO-01-z.jpg	Bloque de madera UCB – Buen estado\nBloque de madera sólida y resistente, ideal para trabajos de carpintería o proyectos educativos. En buen estado y listo para usar.	0.00
182	Cilindro de metal UCB	Cilindro de metal UCB – Buen estado Cilindro metál	\N	0	Default	23	f	https://img.freepik.com/vetores-premium/cilindro-metalico-brilhante-pedestal_275806-1118.jpg	Cilindro de metal UCB – Buen estado\nCilindro metálico robusto, ideal para aplicaciones industriales y mecánicas. En buen estado y listo para su uso.	0.00
183	Cilindro de Aluminio	Default	\N	0	Default	23	f	https://www.3bscientific.com/imagelibrary/U30071/U30071_01_Cilindro-calorimetrico-Aluminio.jpg	Cilindro de aluminio – Buen estado\nCilindro ligero y resistente, ideal para proyectos mecánicos e industriales. En buen estado y listo para su uso.	0.00
184	Sensor Mecánico	Default	\N	0	Default	24	f	https://img.interempresas.net/FotosArtProductos/P171652.jpg	Sensor mecánico – Actualmente no disponible\nProducto fuera de stock o temporalmente no disponible. Consulta para futuras reposiciones o alternativas.	0.00
185	Herramienta corte para torno	Default	\N	0	Default	23	f	https://www.runsom.com/wp-content/uploads/2023/03/HSS-Lathe-Tool.jpg	Herramienta de corte para torno – Nuevo\nHerramienta precisa y resistente para operaciones de corte en tornos. Ideal para trabajos industriales y de fabricación.	0.00
187	Llave especial naranja	Default	\N	0	Default	23	f	https://cahema.pe/75256-large_default/llave-ajustable-8-naranja-asaki-ask04002.jpg	Llave especial naranja – Nuevo\nHerramienta especializada con diseño ergonómico y acabado en color naranja para fácil identificación. Ideal para ajustes precisos y trabajos específicos.	0.00
194	Herramienta taladro de precisión	Default	\N	0	Default	23	f	https://m.media-amazon.com/images/I/51SM3tLNhzL._UF894,1000_QL80_.jpg	Herramienta taladro de precisión – Usado (con óxido)\nHerramienta para taladro de precisión con signos de uso y presencia de óxido, recomendable para trabajos no críticos o repuestos.	0.00
195	Llave tipo T hexagonal	Default	\N	0	Default	23	f	https://urrea.com/media/catalog/product/4/6/46420LBGP.jpeg?auto=webp&format=pjpg&fit=cover	Llave tipo T hexagonal – Nuevo\nHerramienta ergonómica en forma de T para un ajuste firme y cómodo en tornillos hexagonales. Ideal para trabajos mecánicos y de precisión.	0.00
15	Access Point WiFi	   UniFi AP-AC	https://example.com/datasheet/uap-ac.pdf	0	   Ubiquiti	1	t	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTyXCiA1GcmhsoSoze7vIN5LbWSWYxc2a2r6g&s	Aqui entra un texto descriptivo del equipo	0.00
14	Workstation Móvil	 Precision 5550	https://example.com/datasheet/5550.pdf	0	 Dell	21	t	https://hp.widen.net/content/iphnzbqotl/png/iphnzbqotl.png?w=800&h=600&dpi=72&color=ffffff00	Laptop workstation con Xeon y Quadro RTX	0.00
1	Impresora	   prueba		0	   prueba	1	t	https://mediaserver.goepson.com/ImConvServlet/imconv/0b6b6f6b5bccbd9b2a89b0b1117c730e3bcab3a1/1200Wx1200H?use=banner&hybrisId=B2C&assetDescr=20Lio2_MBL_blk_01	Aqui entra un texto descriptivo del equipo	0.00
20	Laptop	  Latitud		0	  DEL	22	t	https://intecsa.com.bo/wp-content/uploads/2024/07/DELL-NB-LATITUDE-7420-2.jpg	Laptop Latitud DEL.	0.00
29	Mini Dron (de 3 hélices)	 Default		1	 Default	24	f	https://i.ebayimg.com/images/g/3TwAAOSwQv5i4wK4/s-l400.jpg	Mini Dron XT FLYER con control remoto, cable de carga y 3 hélices. Ideal para principiantes, con diseño ligero y fácil de operar para vuelos cortos.	0.00
85	Cable conector	Default	\N	1	Default	27	f	https://duraled.com.mx/wp-content/uploads/2024/02/CONECTOR-TIRA-DE-LED-DURALED-127V-2835-IP44.jpg	Cable conector XP, diseñado para conexiones seguras y eficientes entre dispositivos electrónicos. Ideal para diversas aplicaciones tecnológicas.	0.00
11	Disco Duro Externo	 WD Elements	https://example.com/datasheet/wd-elements.pdf	0	 Western Digital	1	t	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSQoZVZOZxYTo_V5do0MhUGBfmIRuEIt_Xupg&s	Disco duro portátil de 2TB USB 3.0	0.00
10	Teclado Mecánico	  K95 RGB	https://example.com/datasheet/k95rgb.pdf	0	  Corsair	3	t	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSZbULX5JQZLIaU8iORvsX7hKSA9sSO9R1fTQ&s	Teclado gaming mecánico con switches Cherry MX	0.00
16	Equipo1	  TC-2022	https://www.alldatasheet.com/	0	  Dell	20	t	https://cursodeinstalador.com/wp-content/uploads/2020/12/pexels-pixabay-159298-scaled.jpg.webp	Aqui entra un texto descriptivo del equipo	0.00
13	Tablet Profesional	 Tab S7+	https://example.com/datasheet/tab-s7plus.pdf	0	 Samsung	3	t	https://i.blogs.es/78a4ac/lenovo-tab-m11/650_1200.jpg	Tablet Android con S Pen y pantalla Super AMOLED	0.00
12	Impresora Laser	 LaserJet Pro	https://example.com/datasheet/laserjet-pro.pdf	0	 HP	1	t	https://santacruz.solutekla.com/photo/1/hp/impresoras_laser_multifuncionales_monocromaticas/impresora_multifuncin_hp_laserjet_pro_m428fdn/impresora_multifuncin_hp_laserjet_pro_m428fdn_0001	Impresora láser monocromática 25ppm	0.00
8	Servidor Rack	 PowerEdge R740	https://example.com/datasheet/r740.pdf	0	 Dell	19	t	https://www.eabel.com/wp-content/uploads/2024/07/Advanced-server-racks-in-a-high-tech-data-center.webp	Servidor rackeable de 2U con doble procesador	0.00
7	Switch Gigabit	 GS308	https://example.com/datasheet/gs308.pdf	0	 Netgear	1	t	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSMNWDigUnYzWSuVhWeF36dwbDXvbR_qzaSEA&s	Switch de 8 puertos 10/100/1000 Mbps	0.00
4	Cables de potencia rojo y negro	   prueba		0	   prueba	1	t	https://cotzul.com/wp-content/uploads/2024/11/Cable-calibre-18-CCA-parlante-rojo-negro-England-Electronics.png	Aqui entra un texto descriptivo del equipo	0.00
2	Soldamatics	   prueba		0	   prueba	1	t	https://seaberyat.com/wp-content/uploads/2023/09/Soldamatic-5.0-360F.png	Aqui entra un texto descriptivo del equipo	0.00
9	Monitor Profesional	 P27h-20	https://example.com/datasheet/p27h-20.pdf	0	 Lenovo	3	t	https://www.lg.com/content/dam/channel/wcms/pe/images/monitores/27gr93u-b_awf_espr_pe_c/gallery/medium01.jpg	Monitor IPS de 27" QHD con USB-C	0.00
6	Router Inalámbrico	 RT-AC68U	https://example.com/datasheet/rt-ac68u.pdf	0	 Asus	1	t	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTud4pzZOu4n9oHF4g_pUVmMJSaJfdeSCmY2g&s	Router dual-band con velocidades de hasta 1900 Mbps	0.00
30	Lámpara de Aumento (nueva)	 Default		3	 Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRIf8qCb15jvfLjyCbhBqACLGCI-98c1_bLbQ&s	Lámpara de aumento Takema, nueva y sin uso. Ideal para trabajos de precisión gracias a su lente ampliadora y luz integrada.	0.00
31	Lámpara de Aumento (Funcional)	Default	\N	1	Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRIf8qCb15jvfLjyCbhBqACLGCI-98c1_bLbQ&s	Lámpara de aumento Takema, en buen estado de funcionamiento. Perfecta para tareas detalladas que requieren iluminación y aumento preciso.	0.00
38	Sensor de pinza de corriente de carga	Default	\N	2	 Default	24	f	https://techmasterdemexico.com/wp-content/uploads/MODEL-8125.jpg	Sensor de pinza de corriente de carga Kyoritsu, ideal para medir corriente sin interrumpir el circuito. Compatible con analizadores eléctricos para monitoreo preciso.	0.00
37	Analizador de calidad de energía y conjunto de sensores de pinza modelo 	Default	\N	1	Default	24	f	https://ce8dc832c.cloudimg.io/v7/_cdn_/D2/8B/C0/00/0/833581_1.jpg?width=640&height=480&wat=1&wat_url=_tme-wrk_%2Ftme_new.png&wat_scale=100p&ci_sign=bc22f13dc0f37d3aca621fcba33ceddc202d5728	Analizador de Calidad de Energía Kyoritsu diseñado para medir y registrar parámetros eléctricos críticos. Ideal para mantenimiento preventivo y análisis de redes eléctricas.	0.00
43	Soporte para pantalla en la pared	Default	\N	5	Default	23	f	https://lumiproduct.oss-cn-hongkong.aliyuncs.com/2022/08/12/62f65222327ffa0002aba657.jpg	Soporte de pared Premium para pantalla, funcional y resistente, ideal para una instalación segura y estable en entornos profesionales o domésticos.	0.00
46	Motor a pasos	 Default		3	 Default	23	f	https://tienda.sawers.com.bo/image/cache/catalog/00556-500x500.jpg	Motor a pasos JKongmotor usado, adecuado para aplicaciones que requieren control preciso de movimiento en proyectos y maquinaria.	0.00
51	Driver para motor a pasos	Default	\N	6	Default	24	f	https://www.steren.com.mx/media/catalog/product/cache/295a12aacdcb0329a521cbf9876b29e7/image/19452484b/tarjeta-de-control-para-motor-a-pasos.jpg	Driver para motor a pasos, dispositivo que controla la corriente y dirección del motor para movimientos precisos en aplicaciones electrónicas y robóticas.	0.00
52	Sensor Capacitivo	Default	\N	3	Default	24	f	https://i.ebayimg.com/images/g/sSoAAOSwnGRjq9a9/s-l400.jpg	Sensor capacitivo RHOMBERG.BRASLER con borneras para conexión, ideal para detección sin contacto de objetos sólidos o líquidos en aplicaciones industriales.	0.00
28	Mini Dron	 Default		1	 Default	24	f	https://i.ebayimg.com/images/g/3TwAAOSwQv5i4wK4/s-l400.jpg	Mini Dron XT FLYER con control remoto, 2 baterías, cable USB, 4 hélices y manual incluidos. Compacto y fácil de manejar, perfecto para vuelos recreativos.	3000.00
54	Cable MicroLogix	Default	https://es.rs-online.com/web/p/accesorios-para-controladores-y-automatas/7140085	2	Default	24	f	https://assetcloud.roccommerce.net/w458-h458-cpad/_smcelectric/6/7/9/rockwell_automation_1761_cbl_pm02.jpg	Cable MicroLogix Allen Bradley, utilizado para la programación y comunicación entre PLCs MicroLogix y computadoras. Esencial para automatización industrial.	0.00
55	Cable banana - cocodrilo	Default	\N	4	Default	24	f	https://images.ledbox.es/subproductos/10519-51/grande/10519-51.jpg	Cable banana a cocodrilo, ideal para conexiones rápidas y seguras en pruebas eléctricas y de laboratorio. Versátil y fácil de usar.	0.00
56	Cable banana - punta	Default	\N	2	Default	24	f	https://cdtechnologia.net/34328-large_default/cable-para-pruebas-punta-banana-a-banana-1-metro.jpg	Cable banana a punta, diseñado para realizar mediciones eléctricas precisas con multímetros y equipos de prueba. Seguro y fácil de manipular.	0.00
57	Cable de alimentación 250V - 10A	Default	\N	1	Default	24	f	https://ascentoptics.com/blog/wp-content/uploads/2024/09/6.2-4.png	Cable de alimentación 250V - 10A, adecuado para suministrar energía a equipos eléctricos de alta demanda. Robusto y seguro para uso industrial o doméstico.	0.00
59	Adaptador verde 250V - 20A	Default	\N	2	Default	24	f	https://m.media-amazon.com/images/I/61-u5zY-BcL._UF894,1000_QL80_.jpg	Adaptador verde 250V - 20A, diseñado para conexiones eléctricas seguras en aplicaciones de alta potencia. Robusto y fácil de instalar.	0.00
61	UNI-SOLVER	Default	\N	2	Default	24	f	https://m.media-amazon.com/images/I/51wT-k7B0dS.jpg	UNI-SOLVER de Smith & Nephew, removedor de adhesivos médico quirúrgicos, ideal para eliminar residuos de forma suave y efectiva sin dañar la piel.	0.00
66	Aseguradores de cable LAN	Default	\N	38	Default	24	f	https://roams.es/images/post/es_ES_telco/companias-telefonicas-blog-tecnologia-conexion-ethernet.jpg	Aseguradores de cable LAN, accesorios para fijar y proteger cables de red, evitando desconexiones accidentales y mejorando la organización.	0.00
79	Dron UCB 4 hélices	Default	\N	3	Default	26	f	https://m.media-amazon.com/images/I/51ZMU00wbeL._UF894,1000_QL80_.jpg	Dron UCB de 4 hélices desarmado, ideal para ensamblaje personalizado y aprendizaje práctico. Perfecto para aficionados y entusiastas de la tecnología.	0.00
86	NI myRIO	Default	\N	9	Default	28	f	https://www.smsic.com.bo/myRIO.jpg	NI myRIO es un dispositivo compacto y potente ideal para estudiantes e ingenieros, que permite desarrollar proyectos de control y adquisición de datos en tiempo real con LabVIEW.	0.00
88	Protoboard	Default	\N	9	Default	24	f	https://i2celectronica.com/157-large_default/protoboard-400-puntos.jpg	Protoboard ideal para el diseño y prueba rápida de circuitos electrónicos sin necesidad de soldar, perfecta para estudiantes, makers y profesionales.	0.00
89	Portacautín con lampara	Default	\N	8	Default	24	f	https://ja-bots.com/wp-content/uploads/2024/07/s-l1200.jpg	Portacautín con lámpara multifuncional que ofrece soporte seguro para el cautín e iluminación precisa para trabajos de soldadura detallados y seguros.	0.00
90	Relé Temporizador	Default	\N	6	Default	24	f	https://assets.tramontina.com.br/upload/tramon/imagens/ELT/58015285PDM001G.jpg	Relé Temporizador ideal para automatizar procesos eléctricos con control de tiempo preciso, perfecto para aplicaciones industriales, domótica y proyectos electrónicos.	0.00
91	Fuente de poder	Default	\N	12	Default	24	f	https://compubit.com.co/wp-content/uploads/2023/10/FUENTE-DE-PODER-PARA-PC-DE-750W_2-1-1024x1024.jpg	Fuente de poder confiable y ajustable, ideal para alimentar circuitos electrónicos y proyectos de laboratorio con voltaje y corriente controlados.	0.00
92	Generador de señales	Default	\N	5	Default	24	f	https://upload.wikimedia.org/wikipedia/commons/thumb/e/ed/Leader_LSG-15_signal_generator.jpg/800px-Leader_LSG-15_signal_generator.jpg	Generador de señales versátil para crear formas de onda precisas, ideal para pruebas, desarrollo y diagnóstico en electrónica y telecomunicaciones.	0.00
98	Filamento 3D premium	Default	\N	4	Default	27	f	https://www.3dmarket.mx/wp-content/uploads/2016/08/filamento3d-filamentosimpresora3d-filamentoplapla-purple-3dmarket.jpg	Filamento 3D premium de alta calidad que garantiza impresiones precisas, resistentes y con excelente acabado, ideal para proyectos profesionales y creativos.	0.00
99	Filamento 3D PLA	Default	\N	5	Default	27	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSD7xAmMeZ3o97rnQYqCrS9HjSqOjYNp-_E_w&s	Filamento 3D PLA ecológico y fácil de usar, ideal para impresiones detalladas con excelente adherencia y acabado, perfecto para usuarios de todos los niveles.	0.00
103	Mangueras de alimentación	Default	\N	1	Default	24	f	https://entaban.es/10754-product_default/manguera-pvc-limpieza-alimentaria-rollo.jpg	Mangueras de alimentación resistentes y seguras, ideales para transportar energía eléctrica en instalaciones industriales, comerciales o proyectos técnicos.	0.00
104	Pernos y arandelas	Default	\N	1	Default	23	f	https://kingsunmachining.com/wp-content/uploads/2024/11/1-72.png	Pernos y arandelas esenciales para uniones mecánicas firmes y seguras, ideales en ensamblajes industriales, proyectos de construcción y electrónica.	0.00
105	Fusibles	Default	\N	12	Default	24	f	https://sdindustrial.com.mx/wp-content/uploads/2021/08/fusibles-1024x576.jpeg	Fusibles de protección confiable para circuitos eléctricos, ideales para prevenir daños por sobrecorriente en sistemas electrónicos e industriales.	0.00
106	Kit de excavadoras	Default	\N	2	Default	23	f	https://tienda.sawers.com.bo/image/cache/catalog/02883-228x228.jpg	Kit de excavadoras interactivo y educativo que permite armar modelos funcionales, ideal para aprender principios de mecánica, robótica y construcción de forma divertida.	0.00
107	Kit para armar reloj	Default	\N	2	Default	23	f	https://tienda.sawers.com.bo/image/cache/catalog/00297-1-500x500.jpg	Kit para Armar Reloj – Crea tu propio reloj personalizado con este completo kit. Incluye mecanismo, manecillas y todo lo necesario para ensamblarlo fácilmente en casa.	0.00
108	Tubos termocontraibles	Default	\N	1	Default	24	f	https://magnani.com.ar/images/product_image/24385/0?dpr=2.625&fit=contain&h=400&q=80&version=fbc7a&w=400	Tubos termocontraíbles\nIdeales para aislar y proteger cables eléctricos. Se ajustan con calor para un acabado seguro y duradero.	0.00
109	Kit para armar reloj (armado)	Default	\N	2	Default	23	f	https://electronicahl.com/wp-content/uploads/2021/03/20-33.png	Kit para armar un reloj – Nuevo (ya armado)\nIncluye todas las piezas para crear tu propio reloj decorativo. Ya viene ensamblado, listo para usar o personalizar.	0.00
110	Juego de tornillos	Default	\N	1	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQLxURGyUXQSjqNvFRdFxRWrazwlVV-N60vhA&s	Juego de tornillos.\nSet completo de tornillos de distintos tamaños, ideal para reparaciones, bricolaje o proyectos domésticos.	0.00
111	Brocas de fresa	Default	\N	10	Default	23	f	https://m.media-amazon.com/images/I/61eSgrFfZiL.jpg	Brocas de fresa\nBrocas de corte precisas ideales para madera, plástico o metal. Perfectas para trabajos de carpintería y mecanizado.	0.00
112	Brocas para minidrill	Default	\N	10	Default	23	f	https://epyelectronica.com/wp-content/uploads/Brocas-Milimetricas-para-PCB-1.2mm.jpg	Brocas para minidrill\nJuego de brocas finas y precisas, ideal para trabajos detallados con minitaladro en madera, metal o plástico.	0.00
113	Electrodos adhesivos	Default	\N	8	Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ0pdSBh1-mEl1bvVUCjfonCleMUcbFFYC75w&s	Electrodos adhesivos Leadwire\nElectrodos reutilizables con conexión tipo leadwire. Ofrecen excelente adherencia y conducción para terapias TENS o EMS.	0.00
114	Kit para armar dron	Default	\N	3	Default	26	f	https://www.educaciontrespuntocero.com/wp-content/uploads/2022/03/kit-drones.jpg	Kit para armar dron JounyRC – Usado\nIncluye componentes esenciales para ensamblar un dron JounyRC. En buen estado y listo para volar con tus ajustes.	0.00
117	Cargador de baterías	Default	\N	1	Default	24	f	https://tienda.sawers.com.bo/image/cache/catalog/00127-1-500x500.jpg	Cargador de baterías IMAX – Nuevo\nCargador eficiente y confiable para baterías recargables, compatible con múltiples tipos y tamaños. Ideal para uso doméstico y profesional.	0.00
118	Cagador de baterias AC	Default	\N	1	Default	24	f	https://naylampmechatronics.com/1016-superlarge_default/cargador-de-bateria-imax-b6ac-compatible.jpg	Cargador de baterías AC IMAX – Seminuevo\nCargador versátil y eficiente para baterías recargables, en excelente estado y listo para usar. Compatible con diversos tipos y tamaños.	0.00
121	Hélices negras	Default	\N	5	Default	26	f	https://http2.mlstatic.com/D_NQ_NP_885774-MEC79233029829_092024-O.webp	Hélices negras (sentido horario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	0.00
122	Hélices negras (sentido antihorario)	Default	\N	5	Default	26	f	https://eu.robotshop.com/cdn/shop/files/multi-rotor-8x45-cw-propeller-pair.webp?v=1720502010&width=500	Hélices negras (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	0.00
123	Hélices rojas (sentido antihorario)	Default	\N	4	Default	26	f	https://www.maisondudrone.com/wp-content/uploads/2020/11/24-He%CC%81lices-3-Pales-CW-Clockwise-CCW-Counter-Clockwise-DALPROP-CYCLONE-T5045C-PRO-5045-ROUGE-CRYSTAL--300x300.jpg	Hélices rojas (sentido antihorario) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	0.00
124	Hélices blancas (sentido antihorario) (punto plateado)	Default	\N	6	Default	26	f	https://ejemplo.com/imagen.jpg	Hélices blancas (sentido antihorario) (punto plateado) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido antihorario. Perfectas para drones y vehículos aéreos.	0.00
125	Helices blancas (sentido horario) (punto negro)	Default	\N	6	Default	26	f	https://ejemplo.com/imagen.jpg	Hélices blancas (sentido horario) (punto negro) – Nuevo\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	0.00
126	Hélices grises (sentido antihorario) (punto plateado)	Default	\N	1	Default	26	f	https://ejemplo.com/imagen.jpg	Hélices grises (sentido antihorario) – Usadas\nHélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	0.00
127	Patas para el dron	Default	\N	4	Default	26	f	https://m.media-amazon.com/images/I/415bZPrv+IL.jpg	Soportes resistentes que brindan estabilidad y protección durante el aterrizaje. Compatibles con diversos modelos de dron.	0.00
128	Adaptador para hélices	Default	\N	20	Default	26	f	https://ejemplo.com/imagen.jpg	Adaptador para hélices – Nuevo\nPermite una fijación segura de las hélices al motor. Ideal para drones y modelos aéreos de alta precisión.	0.00
129	Bases de goma para dron	Default	\N	8	Default	26	f	https://m.media-amazon.com/images/I/51wCYrSs-TL.jpg	Bases de goma para dron – Nuevo\nProtege tu dron con estas bases de goma duraderas que reducen el impacto en aterrizajes. Compatibles con múltiples modelos.	0.00
130	Placa de distribución	Default	\N	2	Default	26	f	https://tienda.sawers.com.bo/image/cache/catalog/03710-1-228x228.jpg	Placa de distribución nueva.\nDistribuye eficientemente la energía entre los componentes de tu dron con esta placa compacta y confiable. Perfecta para configuraciones personalizadas.	0.00
131	Jumpers de conexión	Default	\N	1	Default	24	f	https://sumador.com/cdn/shop/products/Jumper_hembra-hembra_10cm.jpg?v=1577419718	Jumpers de conexión – Nuevo\nConecta fácilmente componentes electrónicos con estos jumpers flexibles y duraderos. Ideales para proyectos de drones y circuitos DIY.	0.00
132	Controlador de veolcidad electrónico (ESC)	Default	\N	2	Default	29	f	https://m.media-amazon.com/images/I/71W0Il2EGML._UF350,350_QL80_.jpg	Controlador de velocidad electrónico (ESC) Readtosky – Nuevo\nOptimiza el rendimiento de tu dron con este ESC de alta eficiencia. Compatible con motores brushless y fácil de instalar.	0.00
133	Tornillos hexagonales	Default	\N	24	Default	23	f	https://m.media-amazon.com/images/I/51DOI04d85L.jpg	Tornillos hexagonales – Nuevo\nJuego de tornillos hexagonales resistentes, ideales para ensamblar y asegurar piezas de drones u otros proyectos electrónicos.	0.00
134	Set de collets	Default	\N	19	Default	23	f	https://www.creativo3d.com/wp-content/uploads/2025/06/Set-de-Collets-ER11-para-CNC-de-15-Piezas.jpg	Set de collets – Nuevo\nAsegura hélices y ejes con este set de collets de alta precisión. Compatible con diversos motores para drones y modelos RC.	0.00
135	Perno cabeza Allen	Default	\N	10	Default	23	f	https://www.privarsa.com.mx/wp-content/uploads/2019/02/Tornillo_Socket_Cabeza_Allen.jpg	Perno cabeza Allen – Usado\nPerno resistente con cabeza Allen, ideal para fijaciones seguras en estructuras metálicas o electrónicas. En buen estado y totalmente funcional.	0.00
39	Cables de prueba	Default	https://www.kew-ltd.co.jp/en/products/detail/00131/	9	Default	24	f	https://www.kew-ltd.co.jp/files/co/photo_accessory/7141B.jpg	Cables de prueba Kyoritsu diseñados para garantizar conexiones seguras y precisas en mediciones eléctricas. Resistentes y compatibles con instrumentos de la marca.	0.00
146	Multímetro	Default	\N	6	Default	24	f	https://tienda.sawers.com.bo/image/cache/catalog/04019-1-500x500.jpg	Multímetro BK Precision – Buen estado\nInstrumento versátil para medir voltaje, corriente y resistencia con precisión. Funciona correctamente y es ideal para tareas eléctricas y electrónicas.	0.00
147	Protoboard (mal estado)	Default	\N	3	Default	24	f	https://i2celectronica.com/157-large_default/protoboard-400-puntos.jpg	Protoboard – Mal estado\nProtoboard usado con señales de desgaste y conexiones defectuosas, recomendable solo para repuestos o proyectos no críticos.	0.00
148	Multímetro amarillo caja	Default	\N	3	Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSZ6JHarxssgqgN6ju9Lyo5y4RDVIVSqHxGyw&s	Multímetro amarillo (sin cable)\nMultímetro funcional con carcasa amarilla, ideal para mediciones básicas. No incluye cables de prueba.	0.00
150	Cable macho/hembra	Default	\N	10	Default	24	f	https://i1.wp.com/oxdea.gt/wp-content/uploads/2019/01/JUMPER20CM.png?w=600&ssl=1	Cable macho/hembra – Buen estado\nCable de conexión macho a hembra en buen estado, perfecto para enlazar dispositivos electrónicos con seguridad y estabilidad. Ideal para proyectos y reparaciones.	0.00
154	Cabeza banana hembra	Default	\N	38	Default	24	f	https://ccmtiendadelsonido.com/wp-content/uploads/2024/01/10104-1.jpg	Cabeza banana hembra – Buen estado\nConector tipo banana hembra en buen estado, ideal para equipos de medición y pruebas eléctricas. Asegura conexiones firmes y seguras.	0.00
155	Protoboard doble	Default	\N	22	Default	24	f	https://edutronicas.com/wp-content/uploads/2024/12/imagen_2024-06-27_173920330-2.png	Protoboard doble – Buen estado\nPlaca de pruebas con doble área de conexión, ideal para desarrollar y testear circuitos electrónicos. En buen estado y lista para usar.	0.00
159	Botones	Default	\N	12	Default	24	f	https://i0.wp.com/www.teslaelectronic.com.pe/wp-content/uploads/2019/10/Pulsador.1.jpg?fit=600%2C600&ssl=1	Botones\nBotones electrónicos ideales para proyectos de control o prototipado. Compatibles con placas de desarrollo y fáciles de instalar.	0.00
162	Batería de litio 3.8V 2200 mA	Default	\N	5	Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR8xXNJe5GvSD0ON8lIyCpKmtqJx-U16QJzJg&s	Batería de litio 3.8V 2200 mA TURNIGX – Buen estado\nBatería recargable de alta calidad, perfecta para drones y dispositivos electrónicos. Marca TURNIGX, en buen estado y lista para uso inmediato.	0.00
167	Kit de discos de sierra (para minidrill)	Default	\N	1	Default	23	f	https://mvelectronica.s3.us-east-2.amazonaws.com/productos/102328/63e7e12c786cb.webp	Kit de discos de sierra (para minidrill) – Nuevo\nSet de discos de corte para minitaladro, ideal para trabajar con madera, plástico y metal. Incluye varios tamaños para mayor versatilidad.	0.00
168	Kit de sierra (para minidrill)	Default	\N	1	Default	23	f	https://http2.mlstatic.com/D_NQ_NP_842429-MLM47085362202_082021-O.webp	Kit de sierra (para minidrill) – Nuevo\nConjunto de sierras circulares para minitaladro, perfecto para cortes precisos en diversos materiales. Ideal para trabajos de detalle y bricolaje.	0.00
169	Manómetro	Default	\N	2	Default	24	f	https://suministrosenmetrologia.com/wp-content/uploads/2023/08/manometro.jpg	Manómetro – Nuevo\nInstrumento de medición preciso para controlar la presión en sistemas neumáticos o hidráulicos. Ideal para uso industrial o doméstico.	0.00
171	Conectores caimán (par)	Default	\N	10	Default	24	f	https://www.makersgonnamake.com.mx/cdn/shop/products/s-l225_1.jpg?v=1532385201	Conectores caimán – Buen estado\nPinzas caimán en buen estado, ideales para pruebas eléctricas y conexiones temporales. Aseguran contacto firme y seguro en proyectos electrónicos.	0.00
175	Cables para osciloscopio	Default	\N	13	Default	24	f	https://i2celectronica.com/1898/cable-de-osciloscopio-bnc-con-caimanes.jpg	Cables para osciloscopio – Buen estado\nCables de alta calidad para conexión segura y precisa con osciloscopios. En buen estado, perfectos para mediciones y análisis electrónicos.	0.00
172	Cables de poder	Default	\N	20	Default	24	f	https://pvl.com.bo/wp-content/uploads/2019/12/CAB-PO-NEMA-CABLE-PODER-NEMA_TITULO.jpg	Cables de poder – Buen estado\nCables resistentes y funcionales para suministro eléctrico en diversos dispositivos. En buen estado, ideales para uso doméstico o de laboratorio.	0.00
177	Cables para fuente de voltaje	Default	\N	11	Default	24	f	https://toolroommexico.mx/cdn/shop/products/5f492502c041b40006f987f6_17454203135578674899.jpg?v=1599332850	Cables para fuente de voltaje – Buen estado\nCables duraderos y funcionales para conectar fuentes de voltaje a dispositivos electrónicos. En buen estado, ideales para laboratorio y proyectos eléctricos	0.00
178	Cables doble caimán	Default	\N	4	Default	24	f	https://i5-mx.walmartimages.com/mg/gm/3pp/asr/60ed2185-343b-4e4c-b75f-1bde379edd6f.42ca1dd35882262bc237a8a5ed4976ff.jpeg?odnHeight=612&odnWidth=612&odnBg=FFFFFF	Cables doble caimán – Buen estado\nCables con pinzas caimán en ambos extremos, perfectos para conexiones temporales y pruebas eléctricas. En buen estado y listos para usar.	0.00
173	Cables generador de señal	Default	\N	11	Default	24	f	https://apmelectronica.com/wp-content/uploads/2019/05/Sonda_Cable_de_Pruebas_BNC_a_Caimanes_para_Osciloscopio_y_Generador_de_Funciones_Puntas_de_Prueba_Ferretronica_54bfb075-83d3-4853-aa4b-dfc1faeb432	Cables para generador de señal – Buen estado\nCables confiables para conectar y transmitir señales eléctricas con precisión. En buen estado, ideales para laboratorios y pruebas electrónicas.	0.00
179	Cable de alimentación 3 pines	Default	\N	2	Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT_17MbCfvcBh4kHkRAq608hoRDGY8DTmP6Cw&s	Cable de alimentación 3 pines – Buen estado\nCable robusto con conector de 3 pines, ideal para suministrar energía segura a dispositivos electrónicos y equipos eléctricos. En buen estado y listo para u	0.00
188	Llave Allen grande y chica	Default	\N	2	Default	23	f	https://upload.wikimedia.org/wikipedia/commons/c/c3/Allen_keys.jpg	Llave Allen grande y chica – Buen estado\nJuego de llaves Allen en tamaños grande y pequeño, resistentes y funcionales para diversas aplicaciones mecánicas y electrónicas. En buen estado y listas para 	0.00
190	Herramienta de corte torno/incerto	Default	\N	1	Default	23	f	https://www.runsom.com/wp-content/uploads/2023/03/Ceramic-Lathe-Tool.jpg	Herramienta de corte torno/incerto – Nuevo\nHerramienta de corte intercambiable para torno, diseñada para trabajos precisos y duraderos en mecanizado. Ideal para uso profesional e industrial.	0.00
191	Inserto para herramiento de torno	Default	\N	3	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTrtAT2oAip0C6tBzaSbyk6hmSbvYAksjXypQ&s	Inserto para herramienta de torno – Nuevo\nPieza de repuesto resistente y precisa para herramientas de corte en tornos. Garantiza acabados de alta calidad en mecanizados.	0.00
193	Broca de punta para CNC	Default	\N	2	Default	23	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRY8a0PRPQjQYRbKF5vAiIMtfYCijPe6AqTEg&s	Broca de punta para CNC – Nuevo\nBroca de alta precisión diseñada para máquinas CNC, ideal para cortes limpios y detallados en diversos materiales. Duradera y eficiente.	0.00
45	Robot Delta	Default	\N	0	Default	25	f	https://cl.urany.net/assets/img/delta-con-reductor-2-1.webp	Robot Delta de alta velocidad y precisión, ideal para tareas de ensamblaje, empaquetado y manipulación en líneas de producción automatizadas.	0.00
63	Cable USB A - USB Micro	Default	\N	0	Default	24	f	https://santacruz.solutekla.com/photo/1/solutek/cables/usb_a_micro_usb_5pin_2a/usb_a_micro_usb_5pin_2a_0001	Cable USB A a USB Micro, ideal para conectar y cargar dispositivos electrónicos con puerto Micro USB. Compacto y fácil de usar.	0.00
65	Material para la clase de redes	Default	\N	0	Default	24	f	https://ejemplo.com/imagen.jpg	Material para la clase de redes, incluye recursos y herramientas educativas para el aprendizaje de conceptos y prácticas en redes de comunicación.	0.00
67	Cable VGA macho - macho	Default	\N	0	Default	24	f	https://sofmat.com.bo/wp-content/uploads/2021/07/Cable-VGA-para-monitor.jpg	Cable VGA macho a macho ideal para conectar monitores, proyectores o pantallas a computadoras. Ofrece transmisión de video analógica de alta calidad.	0.00
68	Cable VGA hembra - hembra	Default	\N	0	Default	24	f	https://electronicamorelos.com/image/cache/data/SKU%20RECICLADOS/SKUS%20RECICLADOS%202/4700-001-1500x1500.jpg	Cable VGA hembra a hembra diseñado para unir dos cables VGA macho. Ideal para extender la conexión entre dispositivos de video.	0.00
69	Caja eléctrica	Default	\N	0	Default	24	f	https://ejemplo.com/imagen.jpg	Caja eléctrica Asanno resistente y segura, ideal para instalaciones eléctricas residenciales o comerciales. Fabricada con materiales duraderos para una larga vida útil.	0.00
70	LAN - Óptico (+3m)	Default	\N	0	Default	24	f	https://www.furukawalatam.com/sfc/servlet.shepherd/version/download/0686100000299kbAAA?asPdf=false&	Cable LAN a óptico Furukawa de más de 3 metros, ideal para conexiones de alta velocidad y larga distancia. Garantiza transmisión de datos estable y eficiente en redes avanzadas.	0.00
78	Cable LAN plomo, conector AMP	Default	\N	0	Default	24	f	https://i.ebayimg.com/images/g/JGoAAOSw42dZI952/s-l400.jpg	Cable LAN color plomo con conector AMP, diseñado para conexiones de red confiables y de alta calidad en oficinas o hogares. Fácil de instalar y compatible con múltiples dispositivos.	0.00
80	Dron de 6 hélices	Default	\N	0	Default	26	f	https://www.midronedecarreras.com/wp-content/uploads/2017/10/hexacoptero.png	Dron de 6 hélices desarmado, perfecto para montaje personalizado y proyectos de aeromodelismo avanzado. Ideal para entusiastas que buscan mayor estabilidad y potencia.	0.00
81	Helicoptero grande - 2 hélices de 2 aspas	Default	\N	0	Default	26	f	https://ejemplo.com/imagen.jpg	Helicóptero grande desarmado con 2 hélices de 2 aspas, ideal para ensamblaje y personalización. Perfecto para aficionados que buscan un modelo robusto y detallado.	0.00
82	Heelicopteor chico - hélice de 2 aspas	Default	\N	0	Default	26	f	https://ejemplo.com/imagen.jpg	Helicóptero pequeño U.LIKE desarmado con hélice de 2 aspas, ideal para ensamblaje fácil y aprendizaje básico. Perfecto para principiantes y entusiastas jóvenes.	0.00
83	Control Remoto	Default	\N	0	Default	27	f	https://www.maisondudrone.com/wp-content/uploads/2020/10/201412181750039428.jpg	Control remoto SYMA en perfecto estado y funcionando, compatible con drones y juguetes de la marca. Fácil de usar y confiable para un manejo preciso.	0.00
84	Cable de carga	Default	\N	0	Default	27	f	https://samsung-bolivia.s3.amazonaws.com/product-family-item-image-image/square/product-family-item-image-image_jwFl2OWd3ziuam7fLJZD.png	Cable de carga universal, compatible con múltiples dispositivos para una recarga rápida y segura. Ideal para uso diario en casa, oficina o viaje.	0.00
87	Equipo de medidas y testeo	Default	\N	0	Default	24	f	https://ejemplo.com/imagen.jpg	Equipo de medidas y testeo diseñado para realizar mediciones precisas y confiables en aplicaciones eléctricas, electrónicas e industriales, ideal para laboratorios y mantenimiento técnico.	0.00
93	Protoboard profesional	Default	\N	0	Default	24	f	https://diotronic.com/39733-large_default/bp006-protoboard-profesional.jpg	Protoboard profesional de alta calidad y durabilidad, ideal para el desarrollo avanzado de circuitos electrónicos con mayor espacio y mejor conectividad.	0.00
94	Tablero demostración	Default	\N	0	Default	24	f	https://ejemplo.com/imagen.jpg	Tablero de demostración diseñado para enseñar y mostrar el funcionamiento de sistemas eléctricos o electrónicos, ideal para laboratorios educativos y presentaciones técnicas.	0.00
156	Puntas para taladro	 Default		0	 Default	23	f	https://placacenter.com.bo/wp-content/uploads/2023/12/1046_Puntas-Para-Desarmador-Pozidriv-Largo-2-PZ2_2.jpg	Puntas para taladro KWD – Completo\nSet completo de puntas para taladro, ideal para perforar diversos materiales con precisión. Incluye variedad de tamaños y tipos.	0.00
157	Puntas para taladro (estuche dañado)	Default	\N	0	Default	23	f	https://placacenter.com.bo/wp-content/uploads/2023/12/1046_Puntas-Para-Desarmador-Pozidriv-Largo-2-PZ2_2.jpg	Puntas para taladro KWD – Estuche dañado\nSet de puntas KWD funcional y completo, ideal para trabajos de perforación. El estuche presenta daños, pero las herramientas están en buen estado.	0.00
192	Herramienta de corte para fresadora	Default	\N	6	Default	23	f	https://ejemplo.com/imagen.jpg	Herramienta de corte para fresadora – Nuevo\nHerramienta resistente y precisa para trabajos de fresado en diversos materiales. Ideal para uso industrial y proyectos de alta precisión.	0.00
97	Lapicero inteligente 3D	Default	\N	6	Default	27	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQk0xzGndHZOLPrETHAVKSbCoeL8E6HYQRpEg&s	Lapicero inteligente 3D que permite crear figuras tridimensionales con precisión y facilidad, ideal para diseño, arte, educación y proyectos creativos.	0.00
158	Engrapadora	Default	\N	0	Default	27	f	https://www.truper.com/media/product/3a2/engrapadora-clavadora-tipo-pistola-uso-rudo-truper-e5c.jpg	Engrapadora TRUPPER – Buen estado\nHerramienta resistente y funcional, ideal para trabajos de carpintería, tapicería o bricolaje. Marca TRUPPER, en buen estado y lista para usar.	0.00
161	Batería de litio 3.7V 2600mA	Default	\N	0	Default	24	f	https://motoma.com/web/userfiles/product/big/LCR18650-2600mAh-1.jpg	Batería de litio 3.7V 2600mA – Buen estado\nBatería recargable de alta capacidad, ideal para dispositivos electrónicos y drones. En buen estado y lista para usar.	0.00
163	Batería de litio 3.8V 2200 mA (usada)	Default	\N	0	Default	24	f	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR8xXNJe5GvSD0ON8lIyCpKmtqJx-U16QJzJg&s	Batería de litio 3.8V 2200 mA TURNIGX – Usada\nBatería recargable de alta calidad, perfecta para drones y dispositivos electrónicos. Marca TURNIGX, en buen estado y lista para uso inmediato.	0.00
164	Circuito de inducción magnética	Default	\N	0	Default	24	f	https://ejemplo.com/imagen.jpg	Circuito de inducción magnética – Buen estado\nMódulo funcional para experimentos y proyectos de electromagnetismo, ideal para educación y desarrollo técnico. En buen estado y listo para usar.	0.00
186	Llaves Allen chicas	Default	\N	0	Default	23	f	https://www.infobae.com/resizer/v2/BCO7N7P3LNEQRPEJA5GTW4P4WM.jpg?auth=3cb0b5e9b4f81caf9ed16e1b967899d3ea649b914384b9bca80b5177857b1376&smart=true&width=1200&height=900&quality=85	Llaves Allen chicas – Nuevo\nJuego de llaves Allen pequeñas, perfectas para ajustes precisos en electrónica, mecánica y bricolaje. Resistentes y fáciles de usar.	0.00
120	Hélices rojas	Default	\N	5	Default	26	f	https://m.media-amazon.com/images/I/61x4iziPywL._AC_UF1000,1000_QL80_.jpg	Hélices resistentes y livianas diseñadas para rotación en sentido horario. Perfectas para drones y vehículos aéreos.	22.20
189	Rueda plana con diamante	Default	\N	4	Default	23	f	https://http2.mlstatic.com/D_NQ_NP_963252-MLA80342885673_102024-O.webp	Rueda plana con diamante – Nuevo\nRueda abrasiva con recubrimiento de diamante, ideal para pulir y cortar materiales duros con precisión. Herramienta duradera y eficiente.	1000.00
21	Combo de Atornillador y Llave de Impacto de 10,8 V	 Default	https://makita.com.ar/producto/121-combo-de-atornillador-y-llave-de-impacto-de-10-8-v/	1	 Default	23	f	https://makita.com.ar/wp-content/uploads/2024/08/combo-de-atornillador-y-llave-de-impacto-de-10-8-v-makita-lct204w.jpg	Combo de Atornillador y Llave de Impacto Makita de 10,8 V, ideal para trabajos de ensamblaje y mantenimiento. Compacto, funcional y con componentes incluidos para mayor versatilidad.	1000.00
24	Batería Litio‑Ion 12V max	Default	https://www.makitatools.com/es/products/details/BL1014	2	Default	23	f	https://cdn.makitatools.com/apps/cms/img/bl1/3aca4543-aae2-41ee-81a6-30497bbf6c54_bl1014_p_1500px.png	Batería Litio-Ion 12V max Makita, en buen estado de funcionamiento y lista para su uso. Muestra desgaste estético leve propio del uso regular.	499999.50
27	Estación de soldadura y aire caliente	Default	\N	7	Default	24	f	https://pcell.pe/wp-content/uploads/2023/10/Post-de-facebook-de-frase-o-versiculo-para-iglesia-en-color-negro-y-amarillo-47.png	Estación de soldadura y aire caliente KADA, totalmente funcional y equipada con fuente, cautín, soporte, pistola de aire y 3 boquillas. Ideal para trabajos electrónicos de precisión.	143.14
\.


--
-- TOC entry 5374 (class 0 OID 23076)
-- Dependencies: 254
-- Data for Name: mantenimientos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.mantenimientos (id_mantenimiento, descripcion, costo, fecha_mantenimiento, id_empresa, estado_eliminado, fecha_final_mantenimiento) FROM stdin;
10	string	50	2025-07-10	6	t	2025-08-10
7	asdadasd	100	2027-01-01	1	t	2027-05-05
1	\N	\N	2025-04-04	1	t	2025-01-01
2	\N	\N	2026-01-01	1	t	2026-02-02
11	string	1	2025-07-18	2	t	2025-08-18
12	string	1	2025-09-18	2	t	2025-10-18
13		0	2025-06-05	2	t	2025-06-27
14	lnknkn	0	2025-06-25	1	t	2025-06-25
15	\N	66	2026-05-15	10	t	2026-05-16
16	\N	1	2026-05-15	10	t	2026-05-16
17	\N	1	2026-05-16	12	t	2026-05-17
18	\N	1	2026-05-16	12	t	2026-05-16
19	\N	1	2026-05-16	12	t	2026-05-17
\.


--
-- TOC entry 5376 (class 0 OID 23083)
-- Dependencies: 256
-- Data for Name: muebles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.muebles (id_mueble, nombre, tipo, ubicacion, numero_gaveteros, estado_eliminado, longitud, profundidad, altura, costo) FROM stdin;
5	12345	string	string	0	t	\N	\N	\N	\N
8	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	0	t	11	11	11	111
7	Mueble4	Almacen	Frente al laboratorio	0	t	1	1	1	70
3	Mueble2 	Almacen	A la derecha de la oficina del jefe de carrera	0	t	\N	\N	\N	50
6	Mueble3	Almacen	En la entrada del laboratorio de meca	0	t	1	1	1	300
4	Mueble1	Almacen	A la izqueirda del laboratorio	0	t	1.4	4.1	0.5	100
9	Mueble Ventana	\N	\N	1	f	\N	\N	\N	\N
10	Mueble Pared	\N	\N	1	f	\N	\N	\N	\N
11	Mueble G	\N	\N	9	f	\N	\N	\N	\N
12	Mueble C	\N	\N	18	f	\N	\N	\N	\N
\.


--
-- TOC entry 5378 (class 0 OID 23091)
-- Dependencies: 258
-- Data for Name: prestamos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.prestamos (id_prestamo, fecha_solicitud, fecha_prestamo, fecha_devolucion_esperada, observacion, estado_prestamo, carnet, estado_eliminado, fecha_devolucion, fecha_prestamo_esperada, id_contrato) FROM stdin;
113	2025-10-28 10:22:09.267169	\N	2025-11-07 00:00:00	string	pendiente	12890061	t	\N	2025-10-30 00:00:00	\N
109	2025-09-21 16:37:29.633172	\N	2025-09-25 00:00:00	string	aprobado	12890061	t	\N	2025-09-21 00:00:00	\N
110	2025-09-21 16:37:31.821564	\N	2025-09-25 00:00:00	string	rechazado	12890061	t	\N	2025-09-21 00:00:00	\N
114	2025-10-30 02:04:12.829238	\N	2025-10-31 00:00:00	string	rechazado	12890061	t	\N	2025-10-30 00:00:00	\N
115	2025-10-30 02:11:09.426403	\N	2025-10-31 00:00:00	string	rechazado	12890061	t	\N	2025-10-30 00:00:00	\N
116	2025-10-30 02:32:04.93934	\N	2025-10-31 00:00:00	string	cancelado	12890061	t	\N	2025-10-30 00:00:00	\N
120	2025-11-01 15:19:16.577537	\N	2025-11-08 00:00:00	string	rechazado	12890061	t	\N	2025-11-06 00:00:00	\N
121	2025-11-01 15:22:22.184594	\N	2025-11-04 00:00:00	string	finalizado	12890061	t	\N	2025-11-02 00:00:00	\N
122	2025-11-01 15:49:11.842186	\N	2025-11-01 15:48:22.075	string	rechazado	string	t	\N	2025-11-01 15:48:22.074	\N
123	2025-11-02 11:05:06.043099	\N	2025-11-05 00:00:00	string	finalizado	12890061	t	\N	2025-11-04 00:00:00	\N
124	2025-11-02 11:14:24.895437	\N	2025-11-08 00:00:00	string	rechazado	12890061	t	\N	2025-11-04 00:00:00	\N
125	2025-11-05 20:55:04.809459	\N	2025-11-10 00:00:00	string	rechazado	12890061	t	\N	2025-11-10 00:00:00	\N
126	2025-11-05 20:55:46.719742	\N	2025-11-10 00:00:00	string	rechazado	12890061	t	\N	2025-11-10 00:00:00	\N
168	2025-11-06 23:44:20.20598	\N	2025-11-04 23:59:59	Test lunes a martes	pendiente	7777777	t	\N	2025-11-03 00:00:00	\N
169	2025-11-06 23:44:23.715645	\N	2025-11-05 23:59:59	Test martes a miércoles	pendiente	7777777	t	\N	2025-11-04 00:00:00	\N
158	2025-11-06 21:22:02.878784	\N	2025-11-04 00:00:00	Test monday	rechazado	7777777	t	\N	2025-11-03 00:00:00	\N
160	2025-11-06 21:22:35.593569	\N	2025-11-05 00:00:00	Test tuesday	rechazado	7777777	t	\N	2025-11-04 00:00:00	\N
163	2025-11-06 22:25:49.356201	\N	2025-11-06 00:00:00	Test wednesday	rechazado	7777777	t	\N	2025-11-05 00:00:00	\N
164	2025-11-06 22:26:01.777404	\N	2025-11-07 23:59:59	Test thursday-friday	rechazado	7777777	t	\N	2025-11-06 00:00:00	\N
171	2025-11-06 23:44:37.042296	\N	2025-11-05 23:59:59	Test lunes a miércoles	pendiente	7777777	t	\N	2025-11-03 00:00:00	\N
129	2025-11-05 21:05:17.243004	\N	2025-11-09 00:00:00	string	rechazado	12890061	t	\N	2025-11-08 00:00:00	\N
133	2025-11-05 21:32:05.642537	\N	2025-11-08 00:00:00	string	rechazado	12890061	t	\N	2025-11-07 00:00:00	\N
137	2025-11-05 21:42:13.244738	\N	2025-11-14 00:00:00	string	finalizado	12890061	t	\N	2025-11-13 00:00:00	\N
139	2025-11-05 21:44:16.090997	\N	2025-11-13 00:00:00	string	rechazado	12890061	t	\N	2025-11-12 00:00:00	\N
140	2025-11-05 21:44:33.583188	\N	2025-11-07 00:00:00	string	rechazado	12890061	t	\N	2025-11-06 00:00:00	\N
141	2025-11-05 21:46:22.319465	\N	2025-11-06 00:00:00	string	rechazado	12890061	t	\N	2025-11-05 00:00:00	\N
142	2025-11-05 21:55:47.924574	\N	2025-11-30 00:00:00	string	rechazado	12890061	t	\N	2025-11-29 00:00:00	\N
145	2025-11-05 22:06:27.498381	\N	2025-12-01 08:00:00	Test timezone Bolivia	rechazado	6	t	\N	2025-12-01 06:00:00	\N
147	2025-11-05 22:11:35.919342	\N	2025-11-25 00:00:00	string	rechazado	12890061	t	\N	2025-11-24 00:00:00	\N
148	2025-11-05 22:15:56.516917	\N	2025-11-07 00:00:00	string	rechazado	12890061	t	\N	2025-11-06 00:00:00	\N
149	2025-11-05 22:48:58.010853	\N	2025-11-07 00:00:00	string	rechazado	12890061	t	\N	2025-11-06 00:00:00	\N
150	2025-11-05 23:26:57.682644	\N	2025-11-19 00:00:00	string	rechazado	12890061	t	\N	2025-11-18 00:00:00	\N
151	2025-11-05 23:59:12.317308	\N	2025-11-08 00:00:00	string	rechazado	12890061	t	\N	2025-11-07 00:00:00	\N
152	2025-11-06 00:07:02.947481	\N	2025-11-07 00:00:00	string	cancelado	12890061	t	\N	2025-11-06 00:00:00	\N
128	2025-11-05 21:00:30.980122	\N	2025-11-12 06:00:00	Prueba de función corregida	rechazado	7777777	t	\N	2025-11-10 06:00:00	\N
135	2025-11-05 21:37:03.811826	\N	2025-11-16 00:00:00	Prueba reserva todo el día 15 nov	rechazado	7777777	t	\N	2025-11-15 00:00:00	\N
143	2025-11-05 21:58:43.890987	\N	2025-12-01 08:00:00	Test timezone	rechazado	7777777	t	\N	2025-12-01 06:00:00	\N
155	2025-11-06 20:44:24.232792	\N	2025-11-04 23:59:59	Test reservation	pendiente	7777777	t	\N	2025-11-03 00:00:00	\N
173	2025-11-06 23:44:42.932007	\N	2025-11-07 23:59:59	Test jueves a viernes	pendiente	7777777	t	\N	2025-11-06 00:00:00	\N
174	2025-11-06 23:48:18.133175	\N	2025-12-12 23:59:59	Test 11-12 dic	rechazado	7777777	t	\N	2025-12-11 00:00:00	\N
167	2025-11-06 23:41:12.511819	\N	2025-11-08 00:00:00	string	rechazado	12890061	t	\N	2025-11-07 00:00:00	\N
166	2025-11-06 23:18:57.577265	\N	2025-11-07 00:00:00	string	rechazado	12890061	t	\N	2025-11-06 00:00:00	\N
180	2025-11-06 23:52:40.913983	\N	2025-11-12 00:00:00	string	rechazado	12890061	t	\N	2025-11-11 00:00:00	\N
193	2025-11-07 00:06:51.5879	\N	2025-11-11 00:00:00	string	rechazado	12890061	t	\N	2025-11-11 00:00:00	\N
194	2025-11-07 00:07:19.190591	\N	2025-11-07 00:00:00	string	rechazado	12890061	t	\N	2025-11-07 00:00:00	\N
195	2025-11-07 00:10:52.576307	\N	2025-11-26 00:00:00	Prueba 1 día (26)	rechazado	7777777	t	\N	2025-11-26 00:00:00	\N
196	2025-11-07 00:11:20.436696	\N	2025-11-28 00:00:00	Prueba 2 días (27-28)	rechazado	7777777	t	\N	2025-11-27 00:00:00	\N
197	2025-11-07 00:13:53.281586	\N	2025-12-01 00:00:00	Prueba 3 días (29nov-1dic)	rechazado	7777777	t	\N	2025-11-29 00:00:00	\N
198	2025-11-07 00:14:33.719374	\N	2025-12-02 00:00:00	Prueba día libre	rechazado	7777777	t	\N	2025-12-02 00:00:00	\N
199	2025-11-07 00:16:18.947408	\N	2025-11-11 00:00:00	string	rechazado	12890061	t	\N	2025-11-11 00:00:00	\N
200	2025-11-07 00:17:56.149385	\N	2025-11-08 00:00:00	string	rechazado	12890061	t	\N	2025-11-07 00:00:00	\N
201	2025-11-07 00:19:13.446894	\N	2025-11-25 00:00:00	string	rechazado	12890061	t	\N	2025-11-25 00:00:00	\N
177	2025-11-06 23:51:30.265957	\N	2025-11-07 00:00:00	string	rechazado	12890061	t	\N	2025-11-06 00:00:00	\N
181	2025-11-06 23:52:54.403637	\N	2025-11-11 00:00:00	string	rechazado	12890061	t	\N	2025-11-10 00:00:00	\N
182	2025-11-06 23:58:03.110976	\N	2025-11-07 00:00:00	Test miércoles	rechazado	7777777	t	\N	2025-11-06 00:00:00	\N
185	2025-11-07 00:00:50.786823	\N	2025-11-03 00:00:00	Prueba 2 días	rechazado	7777777	t	\N	2025-11-01 00:00:00	\N
188	2025-11-07 00:02:07.27192	\N	2025-11-14 00:00:00	string	rechazado	12890061	t	\N	2025-11-13 00:00:00	\N
189	2025-11-07 00:02:29.61482	\N	2025-11-20 00:00:00	string	rechazado	12890061	t	\N	2025-11-19 00:00:00	\N
190	2025-11-07 00:02:52.667594	\N	2025-11-25 00:00:00	string	rechazado	12890061	t	\N	2025-11-24 00:00:00	\N
191	2025-11-07 00:05:22.270785	\N	2025-11-24 00:00:00	Prueba 1 día	rechazado	7777777	t	\N	2025-11-24 00:00:00	\N
192	2025-11-07 00:05:36.456174	\N	2025-11-25 00:00:00	Prueba 1 día 25	rechazado	7777777	t	\N	2025-11-25 00:00:00	\N
202	2025-11-07 07:52:37.418804	\N	2025-12-08 00:00:00	string	cancelado	12890061	t	\N	2025-12-01 00:00:00	\N
206	2025-11-10 11:48:38.930147	\N	2025-11-19 00:00:00	\N	cancelado	12890061	f	\N	2025-11-15 00:00:00	\N
207	2025-11-10 11:48:51.015724	\N	2025-11-19 00:00:00	\N	cancelado	12890061	f	\N	2025-11-15 00:00:00	\N
203	2025-11-07 13:07:47.583387	\N	2025-11-19 00:00:00	string	rechazado	12890061	t	\N	2025-11-17 00:00:00	\N
211	2026-05-02 21:27:18.425554	\N	2026-05-04 00:00:00	\N	rechazado	12890061	f	\N	2026-05-02 00:00:00	\N
212	2026-05-02 22:00:45.178753	\N	2026-05-04 00:00:00		rechazado	12890061	f	\N	2026-05-02 00:00:00	\N
208	2025-11-10 11:49:27.144179	\N	2025-11-29 00:00:00	\N	cancelado	12890061	f	\N	2025-11-10 00:00:00	\N
217	2026-05-16 04:50:48.138462	2026-05-19 00:00:00	2026-05-19 00:00:00		cancelado	12890061	f	\N	2026-05-19 00:00:00	3
220	2026-05-16 07:11:13.307403	2026-05-19 00:00:00	2026-05-19 00:00:00		cancelado	12890061	f	\N	2026-05-19 00:00:00	\N
213	2026-05-02 22:05:27.34521	\N	2026-05-04 00:00:00		cancelado	12890061	f	\N	2026-05-02 00:00:00	1
216	2026-05-15 23:56:58.001195	2026-05-18 00:00:00	2026-05-18 00:00:00		finalizado	12890061	f	\N	2026-05-18 00:00:00	2
215	2026-05-15 23:49:10.710198	2026-05-18 00:00:00	2026-05-19 00:00:00		cancelado	12890061	t	\N	2026-05-18 00:00:00	\N
209	2025-11-10 11:49:56.021877	\N	2025-12-30 00:00:00	\N	cancelado	12890061	f	\N	2025-11-30 00:00:00	\N
219	2026-05-16 06:37:17.181998	2026-05-18 00:00:00	2026-05-19 00:00:00		aprobado	12890061	t	\N	2026-05-18 00:00:00	4
214	2026-05-15 23:43:40.651143	2026-05-18 00:00:00	2026-05-19 00:00:00		rechazado	12890061	f	\N	2026-05-18 00:00:00	\N
218	2026-05-16 06:32:25.670097	2026-05-18 00:00:00	2026-05-19 00:00:00		finalizado	12890061	f	\N	2026-05-18 00:00:00	\N
221	2026-05-16 07:12:15.944166	2026-05-30 00:00:00	2026-05-31 00:00:00		rechazado	12890061	f	\N	2026-05-30 00:00:00	5
210	2026-05-02 21:25:21.078767	\N	2026-05-11 00:00:00	\N	rechazado	12890061	f	\N	2026-05-02 00:00:00	\N
224	2026-06-01 02:35:45.339701	2026-05-31 00:00:00	2026-05-31 00:00:00		finalizado	12890061	f	\N	2026-05-31 00:00:00	\N
225	2026-06-01 02:36:27.52325	2026-06-01 00:00:00	2026-06-02 00:00:00		rechazado	12890061	f	\N	2026-06-01 00:00:00	\N
226	2026-06-01 02:36:35.293505	2026-06-01 00:00:00	2026-06-02 00:00:00	asddddddddddddczcad	finalizado	12890061	f	\N	2026-06-01 00:00:00	\N
205	2025-11-10 11:26:31.923793	\N	2025-11-13 00:00:00	\N	rechazado	12890061	t	\N	2025-11-10 00:00:00	\N
222	2026-05-25 21:06:21.005642	2026-05-25 00:00:00	2026-05-25 00:00:00		rechazado	12890061	f	\N	2026-05-25 00:00:00	\N
223	2026-05-25 21:06:55.023458	2026-05-25 00:00:00	2026-05-25 00:00:00		rechazado	12890061	f	\N	2026-05-25 00:00:00	\N
227	2026-06-01 02:36:47.408517	2026-06-01 00:00:00	2026-06-02 00:00:00		finalizado	12890061	f	\N	2026-06-01 00:00:00	\N
230	2026-06-01 02:40:10.130267	2026-06-01 00:00:00	2026-06-03 00:00:00		rechazado	12890061	f	\N	2026-06-01 00:00:00	8
229	2026-06-01 02:40:00.20504	2026-06-01 00:00:00	2026-06-03 00:00:00		rechazado	12890061	f	\N	2026-06-01 00:00:00	7
204	2025-11-07 19:27:07.468486	\N	2025-11-11 00:00:00	string	rechazado	12890061	t	\N	2025-11-08 00:00:00	\N
231	2026-06-01 02:49:29.451831	2026-06-03 00:00:00	2026-06-04 00:00:00		finalizado	12890061	f	\N	2026-06-03 00:00:00	9
228	2026-06-01 02:39:49.66722	2026-06-01 00:00:00	2026-06-02 00:00:00		rechazado	12890061	t	\N	2026-06-01 00:00:00	6
232	2026-06-03 17:38:56.927447	2026-06-03 00:00:00	2026-06-04 00:00:00	Todo salio bien	finalizado	12890061	f	\N	2026-06-03 00:00:00	\N
233	2026-06-03 21:45:36.316399	2026-06-03 00:00:00	2026-06-04 00:00:00	Esta mal	finalizado	12890061	f	\N	2026-06-03 00:00:00	\N
234	2026-06-04 00:15:32.259251	2026-06-03 00:00:00	2026-06-04 00:00:00		rechazado	12890061	f	\N	2026-06-03 00:00:00	\N
235	2026-06-04 02:46:58.864592	2026-06-03 00:00:00	2026-06-04 00:00:00		rechazado	12890061	f	\N	2026-06-03 00:00:00	\N
236	2026-06-04 12:00:25.282079	2026-06-04 00:00:00	2026-06-05 00:00:00		rechazado	12890061	f	\N	2026-06-04 00:00:00	\N
237	2026-06-05 00:53:52.718499	2026-06-04 00:00:00	2026-06-05 00:00:00		rechazado	12890061	f	\N	2026-06-04 00:00:00	\N
238	2026-06-05 00:58:47.923823	2026-06-04 00:00:00	2026-06-05 00:00:00		rechazado	12890061	f	\N	2026-06-04 00:00:00	\N
239	2026-06-05 01:01:31.325963	2026-06-05 00:00:00	2026-06-06 00:00:00		rechazado	12890061	f	\N	2026-06-05 00:00:00	\N
240	2026-06-05 02:12:02.644748	2026-06-04 00:00:00	2026-06-05 00:00:00		rechazado	12890061	f	\N	2026-06-04 00:00:00	\N
241	2026-06-05 02:57:33.604346	2026-06-05 00:00:00	2026-06-06 00:00:00		rechazado	12890061	f	\N	2026-06-05 00:00:00	\N
242	2026-06-05 03:01:55.328079	2026-06-06 00:00:00	2026-06-07 00:00:00		rechazado	12890061	f	\N	2026-06-06 00:00:00	\N
243	2026-06-05 03:02:01.771726	2026-06-06 00:00:00	2026-06-07 00:00:00		rechazado	12890061	f	\N	2026-06-06 00:00:00	\N
244	2026-06-05 17:14:18.585851	2026-06-05 00:00:00	2026-06-06 00:00:00		rechazado	12890061	f	\N	2026-06-05 00:00:00	\N
245	2026-06-05 23:29:46.245395	2026-06-07 00:00:00	2026-06-08 00:00:00	Ninguna	finalizado	12890061	f	\N	2026-06-07 00:00:00	\N
246	2026-06-06 00:02:54.374593	2026-06-05 00:00:00	2026-06-06 00:00:00	Ninguna	finalizado	12890061	f	\N	2026-06-05 00:00:00	\N
247	2026-06-06 00:07:57.641839	2026-06-08 00:00:00	2026-06-09 00:00:00	Todo mal	finalizado	12890061	f	\N	2026-06-08 00:00:00	\N
\.


--
-- TOC entry 5390 (class 0 OID 23125)
-- Dependencies: 270
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.usuarios (carnet, nombre, apellido_paterno, apellido_materno, rol, contrasena, email, telefono, telefono_referencia, nombre_referencia, email_referencia, estado_eliminado, id_carrera, imagen_frente_carnet, imagen_atras_carnet, refresh_token, refresh_token_expiry) FROM stdin;
6	Luis	Castillo	Mendoza	estudiante	password6	estudiante5@ucb.edu.bo	76862003	61329560	Valentina Rojas	referencia8718@gmail.com	t	1	\N	\N	\N	\N
5	Valentina	Vargas	Herrera	estudiante	password5	estudiante4@ucb.edu.bo	72059304	63133388	Jose	referencia1132@gmail.com	t	1	\N	\N	\N	\N
73847214	as	a	a	administrador	$2a$10$1ULn4GO7m.ci4DnZOA8O.udUVf6CHxX7h.nHnb4kLtCEACg7ExZ9i	prueba213r@ucb.edu.bo	79943071	\N	\N	\N	t	5	\N	\N	\N	\N
9333914	Lucia	Lopez	Sanchez	estudiante	$2a$10$ssT7X5khnoyoWNTun4OrI.0DUsQPls6f/X9wYZOYCw26ZiuPxZaSS	lucia.gonzalez7684@ucb.edu.bo	75182345	66342367	Lucia Ramirez	referencia6618@gmail.com	t	1	\N	\N	\N	\N
7	Daniela	Ramirez	Mendoza	estudiante	password7	estudiante6@ucb.edu.bo	76722387	67789040	Luis Terrazas	referencia1767@gmail.com	t	1	\N	\N	\N	\N
prueba	juan	prueba	prueba	estudiante	seguridad	prueba	1	\N	\N	string	t	1	\\x61	\\x61	\N	\N
0001431	Josue	Balbontin	Ugarteche	estudiante	123456789	josue.balbontin@ucb.edu.bo	1234567423	\N	\N	\N	t	2	\\x61	\\x61	\N	\N
10	Juan	Aguilara	Silva	estudiante	password10	estudiante9@ucb.edu.bo	77013996	67568548	Manuel Gonzalez	referencia4243@gmail.com	t	1	\N	\N	\N	\N
2	string	string	string	estudiante	string	t@ucb.edu.bo	88888888	777777777	string	t@ucb.edu.bo	t	2	\N	\N	\N	\N
1	Andrea	Vargas	Rojas	estudiante	password1	estudiante0@ucb.edu.bo	77327303	68834903	antoniooo	referencia1047@gmail.com	t	1	\N	\N	\N	\N
9	Valentina	Jimenez	Rojas	estudiante	password9	estudiante81@ucb.edu.bo	70470031	62972235	Ana Garciaa	referencia866@gmail.com	t	1	\N	\N	\N	\N
3	Juan	Moreno	Silva	estudiante	password3	estudiante2@ucb.edu.bo	72742431	61300599	Luciaaa	referencia7529@gmail.com	t	1	\N	\N	\N	\N
4	string	string	string	estudiante	string	f@ucb.edu.bo	756767555	877755675	string	f@ucb.edu.bo	t	2	\N	\N	\N	\N
34167236	Elena	Romero	Gutierrez	estudiante	$2a$11$qbPfIdX3zKUymKOGWqIw3.p2uPg1XCb1mco4eQ6lmHe2B.rUetC8S	elena.romero5954@ucb.edu.bo	76481291	63044318	Jose Mendoza	referencia2818@gmail.com	t	1	\N	\N	\N	\N
738472136	pruebita	p	´p	administrador	$2a$10$CnA2WqdyoGuL3eyPYcTIBuRCgdGkTIGss4ijpIVS513LKJqYTFpiG	prueba1266@ucb.edu.bo	874284823	\N		\N	f	5	\N	\N	\N	\N
81283812	nombre	ap	ap	estudiante	$2a$10$2mPRM1W9XsBLu0G19chDrOxlINmJOvQvpwBFyC.CySpSrcQVEE3aO	kkkkk@ucb.edu.bo	812838373	\N	\N	\N	f	4	\N	\N	\N	\N
1289006111	Fer	Terrazas	llanos	estudiante	1111111111	fernando.terraza11s@ucb.edu.bo	123456aaaaaaa				t	17	\N	\N	\N	\N
21914143	Lucia	Silva	Aguilar	estudiante	$2a$10$uSvWgsm.EQGmVqqfJd82LOCdpOBEPLQaHrzxwAir7.qV8SGR8MtzC	lucia.silva7462@ucb.edu.bo	78693775	62273447	Camila Martinez	referencia2035@gmail.com	f	1	\N	\N	\N	\N
2222222222	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	estudiante	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@ucb.edu.bo	1111111111	1111111111	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	111111111111111111111111111aaaaaaaaaa111@gmail.com	t	12	\N	\N	\N	\N
2333333342	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	estudiante	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1@ucb.edu.bo	1121111111	1111111111	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1@gmail.com	t	18	\N	\N	\N	\N
prueba123	string	string	string	estudiante	string	terrazasfernando@gmail.com	striasdasdng	strinadasdg	string	string	t	2	\N	\N	\N	\N
string	string	string	string	estudiante	string	terrazas@ucb.edu.bo	striasdang	strasdasding	string	string	t	26	\N	\N	\N	\N
12912838	prueba222	prueba	prueba	administrador	$2a$11$w50B9Myn/MCEFONbCjntHOYfZnPDFGoBKLULO8UW3riDLG3reMf8O	prueba12@ucb.edu.bo	12345678	\N	\N	\N	t	5	\N	\N	\N	\N
122222243	f	asasas	VALLEJOS	administrador	$2a$11$vuUBIzb3.cAfZmhCZccQwOFoW2FwXVBVuIhYgeVa64n7ixpGYZgE6	alejandrov@ucb.edu.bo	12345678	\N	\N	\N	t	19	\N	\N	\N	\N
58622735	Juan	Martinez	Chavez	estudiante	$2a$10$ipJaGT7nCFSbrxwaIuhIU.pf02R3/koKvCsVUI8tfoquHipW96h2W	juan.martinez4109@ucb.edu.bo	71765405	66168473	Diego Herrera	referencia9066@gmail.com	f	1	\N	\N	\N	\N
59319519	Francisco	Garcia	Ramirez	estudiante	$2a$10$/wEh52VqNaDpAcTK5cC/tev92eID0qAssjOjQ0gpeeu9W5JNU7KOO	francisco.garcia5791@ucb.edu.bo	74232805	62374938	Patricia Garcia	referencia1725@gmail.com	f	1	\N	\N	\N	\N
57767149	Fernando	Romero	Martinez	estudiante	$2a$10$lAdl86NrlGmB.40q5nJouOHx/hfukrRb1xckSE61ETlkjD7sRtGda	fernando.romero6116@ucb.edu.bo	75525078	62847862	Fernanda Martinez	referencia2393@gmail.com	f	1	\N	\N	\N	\N
85520089	Manuel	Chavez	Cruz	estudiante	$2a$10$XPkE6zho1WpvaSx1S9bcROhf7qObumMYSNqAWZDMhT/PrAcExGwYO	manuel.chavez7771@ucb.edu.bo	78395586	61407028	Ricardo Mendoza	referencia7485@gmail.com	f	1	\N	\N	\N	\N
71733290	Antonio	Silva	Gonzalez	estudiante	$2a$10$s/6x8gKJ1hklODeFv18rnu7UF6P4RqSgE215sIB4VHvI8oJXpu15e	antonio.silva538@ucb.edu.bo	74568607	61820001	Daniela Sanchez	referencia433@gmail.com	f	1	\N	\N	\N	\N
7777777	juanito	terrazas	llanos	estudiante	$2a$10$Re/hvHa4QUZTjW8DhP0W7u9WQ1glSwuy.DpsBjA2EiMeHoz2hcCRK	fernando@ucb.edu.bo	7777777	7777777	juanito	juanitoPrueba@gmail.com	f	2	\N	\N	\N	\N
47756422	Sofia	Silva	Castillo	estudiante	$2a$10$Ld6HjqlbjzVO6AHsPzMjXOp53hvfscgMuL58lcJcg3yVTQWpjhFdC	sofia.silva2491@ucb.edu.bo	73121078	66273068	Pedro Herrera	referencia3302@gmail.com	f	1	\N	\N	\N	\N
97360510	Ricardo	Moreno	Terrazas	estudiante	$2a$10$k2ePXfLfxuUpxfY14p/bV.0X3iuDofFEQY7s0SQiIvyZcwSv.mip.	ricardo.moreno5423@ucb.edu.bo	77468855	61299849	Diego Jimenez	referencia3961@gmail.com	f	1	\N	\N	\N	\N
5259137	Daniela	Perez	Perez	estudiante	$2a$10$e5m0atqRSiT8OEpnxOEGj.FhBHbKJWrC83tuWMamsR9jAzC.iof8m	daniela.perez9960@ucb.edu.bo	74068545	61437996	Patricia Castillo	referencia3870@gmail.com	f	1	\N	\N	\N	\N
97063100	Fernanda	Castillo	Aguilar	estudiante	$2a$10$pidrmS3lBQ7owsWSzXOGgeuTlPDAmVmd5BauD17VmCQnJDrK7Dapm	fernanda.castillo8839@ucb.edu.bo	74140692	61494383	Francisco Silva	referencia5810@gmail.com	f	1	\N	\N	\N	\N
66409515	Diego	Morales	Gutierrez	estudiante	$2a$10$Kb5Dya9laUgMLG5NtYhJguT2NqItfXzxFhVEQ1AQORgE8pG5Ifnrm	diego.morales370@ucb.edu.bo	73259229	69478832	Juan Martinez	referencia5772@gmail.com	f	1	\N	\N	\N	\N
78861502	Ricardo	Morales	Ramirez	estudiante	$2a$10$tNKc9K47WmG.AjRtvE3c7.THAozq1dQH.3r/HM7Cgg.u1DTU234uC	ricardo.morales9940@ucb.edu.bo	77889629	67036791	Patricia Rodriguez	referencia2529@gmail.com	f	1	\N	\N	\N	\N
59231293	Luis	Cruz	Moreno	estudiante	$2a$10$WL8o0PCd1U8tkNfZaF//5OZ3e5ns7p.XuFJ3T6Or2Qt9cSquwoOX.	luis.cruz3737@ucb.edu.bo	76432763	62920267	Patricia Gutierrez	referencia4472@gmail.com	f	1	\N	\N	\N	\N
36210672	Camila	Vargas	Gonzalez	estudiante	$2a$10$LwIEjDQ7Tjq.D2EXupQ1J.3620Pd4fR/Sykd4ihj2baR7MhZBGG.y	camila.vargas4739@ucb.edu.bo	77919955	64339232	Carlos Vargas	referencia5282@gmail.com	f	1	\N	\N	\N	\N
15722208	Fernanda	Flores	Gutierrez	estudiante	$2a$10$Hj7ie34SiLn/EdHLt8.YWuYVV58jSl0Al8tD3brheKN57clqP7z12	fernanda.flores9808@ucb.edu.bo	75043491	60313726	Fernando Flores	referencia7621@gmail.com	f	1	\N	\N	\N	\N
6664799	Manuel	Sanchez	Mendoza	estudiante	$2a$10$Vp1VBaM7FHcYdfXuc3JLPOBiluQQ/nsnNInXdy0uVjVMtHw4CQgQ6	manuel.sanchez1073@ucb.edu.bo	70365869	60410734	Elena Sanchez	referencia5162@gmail.com	f	1	\N	\N	\N	\N
60981713	Miguel	Garcia	Moreno	estudiante	$2a$10$MzcnrDbRIyI9RimsxYvL6.gE1oSvJH8TU9pgyxIHsKGX4jxDNFiPW	miguel.garcia2510@ucb.edu.bo	78899630	67768081	Maria Aguilar	referencia8263@gmail.com	f	1	\N	\N	\N	\N
17245667	Isabella	Garcia	Romero	estudiante	$2a$10$SjDS3xYztN5083jevCKUL.KpoUbd289URyXFB3JlKGfQRz/WkkHr2	isabella.garcia7715@ucb.edu.bo	77735326	68773302	Fernando Morales	referencia6958@gmail.com	f	1	\N	\N	\N	\N
18237579	Antonio	Martinez	Jimenez	estudiante	$2a$10$DTDKShvKrAKQppIBBJQFteiSKvSj7L3ImQ20v2TXUaClTFPWhnbb6	antonio.martinez3357@ucb.edu.bo	79457405	66809682	Carlos Gonzalez	referencia8419@gmail.com	f	1	\N	\N	\N	\N
50017573	Carmen	Herrera	Romero	estudiante	$2a$10$bF7pn9QuUDdz1Qt7Omv9V.gSo3WA1CpMrziW9iB/c2eOEwtpU1O6u	carmen.herrera6627@ucb.edu.bo	71131881	60036292	Carlos Perez	referencia7013@gmail.com	f	1	\N	\N	\N	\N
70082357	Ricardo	Perez	Flores	estudiante	$2a$10$IIvcIoG.I0zJBJIoC3KCeOY.qMjJhciTDJuNIBt1edgbt31ZqG5sa	ricardo.perez1610@ucb.edu.bo	75125192	67088420	Francisco Ramirez	referencia7610@gmail.com	f	1	\N	\N	\N	\N
60243079	Elena	Mendoza	Rodriguez	estudiante	$2a$10$qpnn4.CJnaUP4JsGotPOwOk5K3d/2PekbRxXDDiHtGeFWOBQktSBC	elena.mendoza2910@ucb.edu.bo	71325083	68091219	Lucia Cruz	referencia5654@gmail.com	f	1	\N	\N	\N	\N
1289832	Sofia	Mendoza	Castillo	estudiante	$2a$10$XFyXamzfMbUWV2dmwXy34eugW512Q3K7WBKN.BlGIfFl4lMWXFe9O	sofia.mendoza1621@ucb.edu.bo	71225633	65583885	Jose Terrazas	referencia8286@gmail.com	f	1	\N	\N	\N	\N
98030255	Valentina	Cruz	Vargas	estudiante	$2a$10$h/OM1v4cHkL6/5A5czI/l.b9KvqYKnhAN0WU2ToQrP/VtvIcOZsCK	valentina.cruz7255@ucb.edu.bo	74803695	64853610	Pedro Gutierrez	referencia9782@gmail.com	f	1	\N	\N	\N	\N
46753146	Daniela	Cruz	Romero	estudiante	$2a$10$ouZOPNTO8vjgUyEeqBfKlexHOvVUvcU6M1wKcdRDan2PuCJUV0B.2	daniela.cruz972@ucb.edu.bo	75532116	62773261	Patricia Cruz	referencia8840@gmail.com	f	1	\N	\N	\N	\N
49274282	Roberto	Ramirez	Gonzalez	estudiante	$2a$10$U3.YkXHDvfAQLHO35vcOBOkIDHYBLQcwus1rg.R6cbS9Fo3BTHUni	roberto.ramirez4969@ucb.edu.bo	78973824	60601589	Carlos Sanchez	referencia6373@gmail.com	f	1	\N	\N	\N	\N
63994794	Isabella	Sanchez	Gutierrez	estudiante	$2a$10$1x/r5L/NYHNp4KOc7NqaQOcRgi4jXpFgso1qeCeUjhABNM79DNAXq	isabella.sanchez6541@ucb.edu.bo	74838024	61179758	Ricardo Herrera	referencia6103@gmail.com	f	1	\N	\N	\N	\N
58355055	Ricardos	Silva	Lopez	estudiante	$2a$10$VXKW.1rqgFPRyhz0OegM7.T84xriScKnz77orEQARAjq5uUKzN0Ba	ricardo.silva6655@ucb.edu.bo	75883794	68546752	Carlos Jimenez	referencia9824@gmail.com	f	1	\N	\N	\N	\N
20899134	Daniela	Gutierrez	Garcia	estudiante	$2a$10$uf5OLBMsOF1qL1sx1llKNuNb3Ws46NUSuXwLsLON1IXay8ZIHeYv2	daniela.gutierrez8330@ucb.edu.bo	78301510	68420866	Elena Gonzalez	referencia7561@gmail.com	f	1	\N	\N	\N	\N
30394623	Andrea	Moreno	Cruz	estudiante	$2a$10$TpruAdQezXszZ6F2g7b6XOKYbH/3oBkN31ryhqycCVlO3sbbdM7py	andrea.moreno951@ucb.edu.bo	70629243	68326233	Carmen Morales	referencia3590@gmail.com	f	1	\N	\N	\N	\N
69883234	Miguel	Gutierrez	Aguilar	estudiante	$2a$10$8TaUrulNBJPG3xqIvrqFgeu05DU7RKRDKxG7IxGDt9fQZsKi/x9S.	miguel.gutierrez9479@ucb.edu.bo	71274101	60929043	Juan Aguilar	referencia6688@gmail.com	f	1	\N	\N	\N	\N
35875580	Carmen	Rojas	Jimenez	estudiante	$2a$10$iD3/UYbynXkz/.xj2.nfv.JVQ4BlKwtrtDoVucMWwwiiSYGVCUrMy	carmen.rojas9380@ucb.edu.bo	77423403	68733670	Lucia Garcia	referencia8355@gmail.com	f	1	\N	\N	\N	\N
76411897	Elena	Vargas	Flores	estudiante	$2a$10$ZuTTIjr5Pcfbz05dRvSrPuPh.6lVGaRflD1B4P3FaY/2iMqVUoA0q	elena.vargas5130@ucb.edu.bo	76439099	60650852	Miguel Jimenez	referencia3442@gmail.com	f	1	\N	\N	\N	\N
48608896	Maria	Terrazas	Herrera	estudiante	$2a$10$ANovtYFonkPPVnQJJYZV8uZilHjCrsgnlMjKjIuacSCt0lZy1FI8y	maria.terrazas267@ucb.edu.bo	78506702	61703136	Francisco Lopez	referencia5921@gmail.com	f	1	\N	\N	\N	\N
80932264	Ricardo	Herrera	Romero	estudiante	$2a$10$u8DX9LvGH1nPdwY0qKF16OA.d83qvudr3/3BrKduO2b7QiRTmPd/G	ricardo.herrera9561@ucb.edu.bo	73177683	64381585	Valentina Mendoza	referencia8040@gmail.com	f	1	\N	\N	\N	\N
70927752	Fernanda	Terrazas	Gonzalez	estudiante	$2a$10$Jyg9hJEaKeNsIpD9Z2DQQu4jK9T21yZRD5u7tYfcchWN31pKv1wkC	fernanda.terrazas3344@ucb.edu.bo	77810728	68052216	Luis Perez	referencia4816@gmail.com	f	1	\N	\N	\N	\N
29338797	Antonio	Romero	Silva	estudiante	$2a$10$OsdsbpzPfoDupcoZmSTQpeVYPAyGvC9Qhl4JrKKirn.4j2OoMY/7y	antonio.romero189@ucb.edu.bo	75800329	60004056	Luis Perez	referencia742@gmail.com	f	1	\N	\N	\N	\N
13308491	Juan	Jimenez	Romero	estudiante	$2a$10$G2P.exhZ/xum4qJloM273OQGmmwpxO706s4yz/ZG35DQiT/VS1eei	juan.jimenez533@ucb.edu.bo	75313693	63883662	Isabella Castillo	referencia4634@gmail.com	f	1	\N	\N	\N	\N
95979604	Elena	Rodriguez	Rodriguez	estudiante	$2a$10$r2PvwHQYadO66NfoK0tkjOwExD0UGW6ws5tmVway2TJsz0YTBhcju	elena.rodriguez4258@ucb.edu.bo	74949768	61394456	Ricardo Rojas	referencia9193@gmail.com	f	1	\N	\N	\N	\N
32277497	Francisco	Silva	Garcia	estudiante	$2a$10$xqfUJuaDtQvDg79XRTUkp.EsnIKIeqMZ/n9e6d41kIXaGsDac3kkW	francisco.silva2354@ucb.edu.bo	74652846	67771170	Francisco Rodriguez	referencia4035@gmail.com	f	1	\N	\N	\N	\N
43160228	Fernanda	Martinez	Chavez	estudiante	$2a$10$D1AfA9KSCf/HHRJHDa9D5uVXm2aYSCKXGRWwUxNZYv4L7Fz0BX4bm	fernanda.martinez4842@ucb.edu.bo	77735348	60280942	Juan Jimenez	referencia8573@gmail.com	f	1	\N	\N	\N	\N
58108879	Carlos	Mendoza	Rojas	estudiante	$2a$10$kr0xC7QT4Kk/7l1GwcTYTuvxCDZSKJzouViYOPSkGrgaos.Y2yAje	carlos.mendoza1860@ucb.edu.bo	79791653	65246867	Pedro Jimenez	referencia9672@gmail.com	f	1	\N	\N	\N	\N
31175141	Juan	Herrera	Gutierrez	estudiante	$2a$10$sX3lGGzNiP3gb73lUoA.dusdRS7dXjB4tR36qBIwf6fQqPnLcq0AK	juan.herrera4541@ucb.edu.bo	75007637	60838905	Juan Rojas	referencia9647@gmail.com	f	1	\N	\N	\N	\N
17112196	Maria	Cruz	Silva	estudiante	$2a$10$38EE2C3StqmpzTTXwBNMee5cBQPb4qs2bPG/wMZmWVn.x.1fGQMti	maria.cruz9138@ucb.edu.bo	70370545	67711092	Carlos Gutierrez	referencia4211@gmail.com	f	1	\N	\N	\N	\N
52517484	Ricardo	Morales	Ramirez	estudiante	$2a$10$q4bSbD9csyrOU/37X4UKwOh/Fu8iqk/Hv4BoGwNPcmrAtb1VDtQLe	ricardo.morales4220@ucb.edu.bo	77660522	69785024	Ana Gonzalez	referencia7269@gmail.com	f	1	\N	\N	\N	\N
65501378	Ricardo	Rodriguez	Jimenez	estudiante	$2a$10$5L1yGyeMxrAKVKs/LS8tpOpvEBtwIrm13JUsXvPGTYp5BlEvzqkqK	ricardo.rodriguez2949@ucb.edu.bo	70842198	65326948	Pedro Jimenez	referencia8124@gmail.com	f	1	\N	\N	\N	\N
80988483	Patricia	Terrazas	Jimenez	estudiante	$2a$10$KV3VW/ve4a9LcWVGiMgPhOOKv/RbnIL4GkNuOvhoZuIT9oI3z1m..	patricia.terrazas3499@ucb.edu.bo	73803240	64961411	Manuel Flores	referencia6033@gmail.com	f	1	\N	\N	\N	\N
95226109	Maria	Moreno	Gonzalez	estudiante	$2a$10$qW4XlZkLRy4LwMZKgQ0pV.czoVj/DmhlWKKSvA4dy5LQ2n6n4fVRi	maria.moreno9407@ucb.edu.bo	75117966	65115242	Luis Morales	referencia145@gmail.com	f	1	\N	\N	\N	\N
33674506	Elena	Gonzalez	Morales	estudiante	$2a$10$oX/NM/XgtnIQHCQ/s/Gos.JFu7I3cOgiq74HHj6TIj9jbvzJF5ZJy	elena.gonzalez5636@ucb.edu.bo	71009183	69176859	Juan Vargas	referencia8215@gmail.com	f	1	\N	\N	\N	\N
91853909	Fernanda	Cruz	Ramirez	estudiante	$2a$10$3OM8FcLVejEHMpwB5omNRuMp2g9YQFypi78KenpPw0VavnXoGWgz.	fernanda.cruz5857@ucb.edu.bo	70699187	61131701	Elena Rojas	referencia2205@gmail.com	f	1	\N	\N	\N	\N
4660334	Patricia	Castillo	Jimenez	estudiante	$2a$10$fwUKGlbmL75o5e/4xJPfreEl63vtUWTPukHXQjBcq7Ij8e7ksnhVK	patricia.castillo9925@ucb.edu.bo	70529138	65614116	Carmen Vargas	referencia2595@gmail.com	f	1	\N	\N	\N	\N
35536042	Pedro	Romero	Ramirez	estudiante	$2a$10$s4eTVqZSo9LnZmyCjTEwse1t9BkuntuVFqwPv1vCu9KmqYaDEuKU.	pedro.romero7848@ucb.edu.bo	73418391	63369489	Carmen Sanchez	referencia6230@gmail.com	f	1	\N	\N	\N	\N
3092709	Roberto	Gutierrez	Martinez	estudiante	$2a$10$yfH4MbixfweOHEeP9daxUuspoTIhDq846Ai2swCFBto0trPm/zuBm	roberto.gutierrez7796@ucb.edu.bo	78472769	69482762	Roberto Terrazas	referencia7609@gmail.com	f	1	\N	\N	\N	\N
74858424	Maria	Vargas	Gonzalez	estudiante	$2a$10$6RPqvsCfejVcs0CxaA8pvOb2BEmHCgCazTAN4lHSIMkEsVBjM5PTi	maria.vargas1272@ucb.edu.bo	77519642	69544059	Andrea Chavez	referencia2665@gmail.com	f	1	\N	\N	\N	\N
28366179	Daniela	Gonzalez	Garcia	estudiante	$2a$10$sRxr8NZbPSlg2p.VEGKZ9eLrTTTGk42eRJfqAhNiQzjNl1RsnOBma	daniela.gonzalez4921@ucb.edu.bo	75853304	68683442	Francisco Sanchez	referencia2637@gmail.com	f	1	\N	\N	\N	\N
38693144	Maria	Perez	Sanchez	estudiante	$2a$10$8AqLik2g9Y2spXxvZPpsxuhm8/JHZlWCA4sWmAvYOiAE6FzyKORI.	maria.perez3808@ucb.edu.bo	76467815	63903588	Camila Jimenez	referencia8543@gmail.com	f	1	\N	\N	\N	\N
56897648	Jose	Rodriguez	Chavez	estudiante	$2a$10$QXy35C27CRUG.zNTds8BBeOtN85xQyRHaENCEi4s6krgoe1R9X31q	jose.rodriguez8047@ucb.edu.bo	79382789	64466780	Lucia Morales	referencia3985@gmail.com	f	1	\N	\N	\N	\N
10489796	Antonio	Vargas	Rodriguez	estudiante	$2a$10$MpBoBS1draO81LmDA.j86eMCETr/WjDUsuu1VjMhQLq8YffKMRL4K	antonio.vargas4826@ucb.edu.bo	73022710	63434450	Pedro Sanchez	referencia873@gmail.com	f	1	\N	\N	\N	\N
444444	string	string	string	estudiante	$2a$10$hLxuWD/tGGYdB5JHje6XJuqZ5LUa93TAesdtG3MZI8.QFA5c9JYcm	fERN@ucb.edu.bo	string	string	string	f@ucb.edu.bo	f	2	\N	\N	\N	\N
1289222	Fer	Terrazas	llanos	estudiante	$2a$10$g27gmjCVbg6ygh2G0M1URulf0bCP7HJaa4.zn6E8osTqCnPsL/sSu	fernando.terrazaszz@ucb.edu.bo	123456				f	17	\N	\N	\N	\N
43603011	Ana	Vargas	Rojas	estudiante	$2a$10$G49aWgq303JVdybKPb2E7Oa16jxeodHkmpgG4qY9g3rFEUCHvy3yi	ana.vargas2762@ucb.edu.bo	72205949	66742681	Ricardo Jimenez	referencia4080@gmail.com	f	1	\N	\N	\N	\N
67078800	Maria	Mendoza	Rojas	estudiante	$2a$10$grFC4AWTw7RwX3P6bxcqceZzyvsDS1w.geVL4rD6CB2jP3XBkqLhO	maria.mendoza618@ucb.edu.bo	71044760	69273530	Fernanda Aguilar	referencia1369@gmail.com	f	1	\N	\N	\N	\N
33449593	Jose	Herrera	Jimenez	estudiante	$2a$10$YkCr1aueQJ5MUAsCQp2kpeWd/ew3IrxFuCxpgHXJjtzXkndwC692K	jose.herrera1811@ucb.edu.bo	73523095	64058976	Carmen Silva	referencia8231@gmail.com	f	1	\N	\N	\N	\N
34820332	Elena	Chavez	Garcia	estudiante	$2a$10$FVohpFixcAN0Z0AyUqcvj.bejOhncmdhrjFSR9xdMSc.J5wjgU9.K	elena.chavez9289@ucb.edu.bo	77668274	69598242	Patricia Herrera	referencia6615@gmail.com	f	1	\N	\N	\N	\N
26055162	Ana	Herrera	Martinez	estudiante	$2a$10$OPZzSVLzCqOGk9u83ga2DOl1SvCit0HXqo5XkWDTVwHkitl7tqdyi	ana.herrera3183@ucb.edu.bo	72424718	67361349	Juan Gonzalez	referencia6994@gmail.com	f	1	\N	\N	\N	\N
46893372	Fernanda	Gutierrez	Jimenez	estudiante	$2a$10$0lJxXFZ5QlMjOdellMQPNelgdKJt/wIpkhUFrRJFJ.yVV6H91lMUu	fernanda.gutierrez5089@ucb.edu.bo	79674778	62811274	Andrea Herrera	referencia6899@gmail.com	f	1	\N	\N	\N	\N
69382946	Lucia	Martinez	Gonzalez	estudiante	$2a$10$DRsbERHTpWeOxJfq1kZ/R.ObLu/S8aDqEQttKqj5Itp7MOoqXwWQa	lucia.martinez6672@ucb.edu.bo	73075957	61284608	Carmen Terrazas	referencia5668@gmail.com	f	1	\N	\N	\N	\N
44973158	Carlos	Castillo	Perez	estudiante	$2a$10$7BDRxjO4DUwyq5T51CF/XepS.nhOErvyAH6ylVBT0C9kQqB5n/mJO	carlos.castillo1355@ucb.edu.bo	71336820	60623020	Maria Perez	referencia2348@gmail.com	f	1	\N	\N	\N	\N
51445813	Diego	Sanchez	Vargas	estudiante	$2a$10$yqcmVyyxJy.27cvhJnf9LuemKnCz1xrQQV978GkLSMtb7lAvoFH1S	diego.sanchez5753@ucb.edu.bo	71348939	67687314	Roberto Flores	referencia6461@gmail.com	f	1	\N	\N	\N	\N
22866085	Francisco	Martinez	Vargas	estudiante	$2a$10$OcCmDvnw9vfgpx.odarBregiFPnF4ADU09BrysX8IqK.n.juiUq4e	francisco.martinez4323@ucb.edu.bo	72654595	62883137	Diego Cruz	referencia1485@gmail.com	f	1	\N	\N	\N	\N
91058289	Carlos	Rojas	Chavez	estudiante	$2a$10$MAqmPqGUg5wU4Hb3F4PzU.RRL9LLKsyGBECuGS.UzIRfjFmaJj9le	carlos.rojas7853@ucb.edu.bo	70952714	64997659	Elena Flores	referencia7039@gmail.com	f	1	\N	\N	\N	\N
17053415	Camila	Martinez	Castillo	estudiante	$2a$10$iWMy2GfymmcI/cH/aY8SZOI64PJk.g02KHxCa7eUV9hDSTQ2YHPEq	camila.martinez7620@ucb.edu.bo	72583484	61124595	Fernando Perez	referencia7377@gmail.com	f	1	\N	\N	\N	\N
90180117	Roberto	Ramirez	Lopez	estudiante	$2a$10$seQRVFYr6/a0kSeJznToqeuo6tQmufawkzBs9abCIe2ACAc.NJoPK	roberto.ramirez2922@ucb.edu.bo	73677231	67665334	Andrea Jimenez	referencia1251@gmail.com	f	1	\N	\N	\N	\N
29708795	Andrea	Cruz	Silva	estudiante	$2a$10$.rwfOlYLmVM/44mhPzjH7OsQPisLoCq7cOwBeYd9ARbJ0bFnn637a	andrea.cruz4578@ucb.edu.bo	79802189	66463751	Ricardo Herrera	referencia2143@gmail.com	f	1	\N	\N	\N	\N
12890062	Fernando	Terrazas	Llanos	estudiante	$2a$10$Pd8PKpTWsm6w7mgcBO9BV.UhyGy9ohbseCaaEkNBtm1zU66mPlElS	terrazasllanosfernando@ucb.edu.bo	73818234	\N	\N	\N	f	18	\N	\N	\N	\N
122221213	Alejandro	Ramírez	Vallejos	administrador	$2a$10$fIosbUwV2zHcaY1BW7efAeYuHuJ/Ps/8G6OYl9XZIMytEXWixZm.q	alejandro.r.r@ucb.edu.bo	79943071	\N	\N	\N	f	12	\N	\N	\N	\N
12890061	Fernando	Terrazas	Llanos	administrador	$2a$10$/8JV2T7ZgDGesA4Bd8J1Ne7YprDGYSOIS3vdcXZ9TBf2B4aifVe0G	fernando.terrazas@ucb.edu.bo	799430792	\N	\N	\N	f	2	\N	\N	VxzWEmIW1XLb/nSGzn4bBQp0o6wvejknTKiTeTqN3qewc7NG9RAknJjUCabLow3N9GkxjsNBpRSER9ruWbk7lQ==	2026-06-13 00:58:16.0303-04
\.


--
-- TOC entry 5419 (class 0 OID 0)
-- Dependencies: 220
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.aggregatedcounter_id_seq', 255, true);


--
-- TOC entry 5420 (class 0 OID 0)
-- Dependencies: 222
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 287, true);


--
-- TOC entry 5421 (class 0 OID 0)
-- Dependencies: 224
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 9, true);


--
-- TOC entry 5422 (class 0 OID 0)
-- Dependencies: 226
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 95, true);


--
-- TOC entry 5423 (class 0 OID 0)
-- Dependencies: 228
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 380, true);


--
-- TOC entry 5424 (class 0 OID 0)
-- Dependencies: 230
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 95, true);


--
-- TOC entry 5425 (class 0 OID 0)
-- Dependencies: 232
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- TOC entry 5426 (class 0 OID 0)
-- Dependencies: 237
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 96, true);


--
-- TOC entry 5427 (class 0 OID 0)
-- Dependencies: 239
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 285, true);


--
-- TOC entry 5428 (class 0 OID 0)
-- Dependencies: 241
-- Name: Accesorio_Id_Accesorio_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Accesorio_Id_Accesorio_seq"', 20, true);


--
-- TOC entry 5429 (class 0 OID 0)
-- Dependencies: 243
-- Name: Categoria_ID_Categoria_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Categoria_ID_Categoria_seq"', 34, true);


--
-- TOC entry 5430 (class 0 OID 0)
-- Dependencies: 245
-- Name: Componente_Id_Componente_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Componente_Id_Componente_seq"', 16, true);


--
-- TOC entry 5431 (class 0 OID 0)
-- Dependencies: 247
-- Name: Empresa_Mantenimiento_Id_Empresa_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Empresa_Mantenimiento_Id_Empresa_seq"', 14, true);


--
-- TOC entry 5432 (class 0 OID 0)
-- Dependencies: 249
-- Name: Equipo_Id_equipo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Equipo_Id_equipo_seq"', 660, true);


--
-- TOC entry 5433 (class 0 OID 0)
-- Dependencies: 251
-- Name: Gavetero_Id_Gavetero_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Gavetero_Id_Gavetero_seq"', 40, true);


--
-- TOC entry 5434 (class 0 OID 0)
-- Dependencies: 253
-- Name: Grupo_Equipo_Id_Grupo_equipo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Grupo_Equipo_Id_Grupo_equipo_seq"', 198, true);


--
-- TOC entry 5435 (class 0 OID 0)
-- Dependencies: 255
-- Name: Mantenimiento_Id_Mantenimiento_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Mantenimiento_Id_Mantenimiento_seq"', 19, true);


--
-- TOC entry 5436 (class 0 OID 0)
-- Dependencies: 257
-- Name: Mueble_Id_Mueble_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Mueble_Id_Mueble_seq"', 16, true);


--
-- TOC entry 5437 (class 0 OID 0)
-- Dependencies: 259
-- Name: Prestamo_Id_Prestamo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Prestamo_Id_Prestamo_seq"', 247, true);


--
-- TOC entry 5438 (class 0 OID 0)
-- Dependencies: 273
-- Name: audit_logs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.audit_logs_id_seq', 48, true);


--
-- TOC entry 5439 (class 0 OID 0)
-- Dependencies: 261
-- Name: carrera_id_carrera_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.carrera_id_carrera_seq', 24, true);


--
-- TOC entry 5440 (class 0 OID 0)
-- Dependencies: 262
-- Name: carreras_id_carrera_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.carreras_id_carrera_seq', 48, true);


--
-- TOC entry 5441 (class 0 OID 0)
-- Dependencies: 265
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.detalles_mantenimientos_id_detalle_mantenimiento_seq', 1, false);


--
-- TOC entry 5442 (class 0 OID 0)
-- Dependencies: 266
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq1; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.detalles_mantenimientos_id_detalle_mantenimiento_seq1', 12, true);


--
-- TOC entry 5443 (class 0 OID 0)
-- Dependencies: 268
-- Name: detalles_prestamos_id_detalle_prestamo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.detalles_prestamos_id_detalle_prestamo_seq', 203, true);


--
-- TOC entry 5444 (class 0 OID 0)
-- Dependencies: 269
-- Name: nombre_de_tu_tabla_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.nombre_de_tu_tabla_id_seq', 9, true);


--
-- TOC entry 5052 (class 2606 OID 23161)
-- Name: aggregatedcounter aggregatedcounter_key_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_key_key UNIQUE (key);


--
-- TOC entry 5054 (class 2606 OID 23163)
-- Name: aggregatedcounter aggregatedcounter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_pkey PRIMARY KEY (id);


--
-- TOC entry 5056 (class 2606 OID 23165)
-- Name: counter counter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);


--
-- TOC entry 5060 (class 2606 OID 23167)
-- Name: hash hash_key_field_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);


--
-- TOC entry 5062 (class 2606 OID 23169)
-- Name: hash hash_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);


--
-- TOC entry 5068 (class 2606 OID 23171)
-- Name: job job_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- TOC entry 5071 (class 2606 OID 23173)
-- Name: jobparameter jobparameter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);


--
-- TOC entry 5076 (class 2606 OID 23175)
-- Name: jobqueue jobqueue_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);


--
-- TOC entry 5079 (class 2606 OID 23177)
-- Name: list list_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);


--
-- TOC entry 5081 (class 2606 OID 23179)
-- Name: lock lock_resource_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);

ALTER TABLE ONLY hangfire.lock REPLICA IDENTITY USING INDEX lock_resource_key;


--
-- TOC entry 5083 (class 2606 OID 23181)
-- Name: schema schema_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);


--
-- TOC entry 5085 (class 2606 OID 23183)
-- Name: server server_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);


--
-- TOC entry 5089 (class 2606 OID 23185)
-- Name: set set_key_value_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);


--
-- TOC entry 5091 (class 2606 OID 23187)
-- Name: set set_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);


--
-- TOC entry 5094 (class 2606 OID 23189)
-- Name: state state_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);


--
-- TOC entry 5096 (class 2606 OID 23191)
-- Name: accesorios Accesorio_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accesorios
    ADD CONSTRAINT "Accesorio_pk" PRIMARY KEY (id_accesorio);


--
-- TOC entry 5099 (class 2606 OID 23193)
-- Name: categorias Categoria_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT "Categoria_pk" PRIMARY KEY (id_categoria);


--
-- TOC entry 5104 (class 2606 OID 23195)
-- Name: componentes Componente_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.componentes
    ADD CONSTRAINT "Componente_pk" PRIMARY KEY (id_componente);


--
-- TOC entry 5107 (class 2606 OID 23197)
-- Name: empresas_mantenimiento Empresa_Mantenimiento_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.empresas_mantenimiento
    ADD CONSTRAINT "Empresa_Mantenimiento_pk" PRIMARY KEY (id_empresa_mantenimiento);


--
-- TOC entry 5112 (class 2606 OID 23199)
-- Name: equipos Equipo_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT "Equipo_pk" PRIMARY KEY (id_equipo);


--
-- TOC entry 5117 (class 2606 OID 23201)
-- Name: gaveteros Gavetero_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gaveteros
    ADD CONSTRAINT "Gavetero_pk" PRIMARY KEY (id_gavetero);


--
-- TOC entry 5122 (class 2606 OID 23203)
-- Name: grupos_equipos Grupo_Equipo_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grupos_equipos
    ADD CONSTRAINT "Grupo_Equipo_pk" PRIMARY KEY (id_grupo_equipo);


--
-- TOC entry 5127 (class 2606 OID 23205)
-- Name: mantenimientos Mantenimiento_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mantenimientos
    ADD CONSTRAINT "Mantenimiento_pk" PRIMARY KEY (id_mantenimiento);


--
-- TOC entry 5130 (class 2606 OID 23207)
-- Name: muebles Mueble_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.muebles
    ADD CONSTRAINT "Mueble_pk" PRIMARY KEY (id_mueble);


--
-- TOC entry 5134 (class 2606 OID 23209)
-- Name: prestamos Prestamo_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.prestamos
    ADD CONSTRAINT "Prestamo_pk" PRIMARY KEY (id_prestamo);


--
-- TOC entry 5152 (class 2606 OID 23211)
-- Name: usuarios Usuario_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT "Usuario_pk" PRIMARY KEY (carnet);


--
-- TOC entry 5161 (class 2606 OID 23380)
-- Name: audit_logs audit_logs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.audit_logs
    ADD CONSTRAINT audit_logs_pkey PRIMARY KEY (id);


--
-- TOC entry 5138 (class 2606 OID 23213)
-- Name: carreras carrera_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.carreras
    ADD CONSTRAINT carrera_pkey PRIMARY KEY (id_carrera);


--
-- TOC entry 5143 (class 2606 OID 23215)
-- Name: contratos contrato_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.contratos
    ADD CONSTRAINT contrato_id PRIMARY KEY (id);


--
-- TOC entry 5145 (class 2606 OID 23217)
-- Name: detalles_mantenimientos detalles_mantenimientos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_mantenimientos
    ADD CONSTRAINT detalles_mantenimientos_pkey PRIMARY KEY (id_detalle_mantenimiento);


--
-- TOC entry 5148 (class 2606 OID 23219)
-- Name: detalles_prestamos detalles_prestamos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT detalles_prestamos_pkey PRIMARY KEY (id_detalle_prestamo);


--
-- TOC entry 5157 (class 2606 OID 23221)
-- Name: usuarios unique_carnet; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT unique_carnet UNIQUE (carnet);


--
-- TOC entry 5141 (class 2606 OID 23223)
-- Name: carreras unique_carreras; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.carreras
    ADD CONSTRAINT unique_carreras UNIQUE (nombre);


--
-- TOC entry 5102 (class 2606 OID 23225)
-- Name: categorias unique_categorias; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT unique_categorias UNIQUE (nombre);


--
-- TOC entry 5115 (class 2606 OID 23227)
-- Name: equipos unique_codigo_imt; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT unique_codigo_imt UNIQUE (codigo_imt);


--
-- TOC entry 5159 (class 2606 OID 23229)
-- Name: usuarios unique_email; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT unique_email UNIQUE (email);


--
-- TOC entry 5125 (class 2606 OID 23231)
-- Name: grupos_equipos unique_grupos_equipos_nombre_modelo_marca; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grupos_equipos
    ADD CONSTRAINT unique_grupos_equipos_nombre_modelo_marca UNIQUE (nombre, modelo, marca);


--
-- TOC entry 5132 (class 2606 OID 23233)
-- Name: muebles unique_nombre; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.muebles
    ADD CONSTRAINT unique_nombre UNIQUE (nombre);


--
-- TOC entry 5110 (class 2606 OID 23235)
-- Name: empresas_mantenimiento unique_nombre_empresas_mantenimiento; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.empresas_mantenimiento
    ADD CONSTRAINT unique_nombre_empresas_mantenimiento UNIQUE (nombre);


--
-- TOC entry 5120 (class 2606 OID 23237)
-- Name: gaveteros unique_nombre_gaveteros; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gaveteros
    ADD CONSTRAINT unique_nombre_gaveteros UNIQUE (nombre);


--
-- TOC entry 5057 (class 1259 OID 23238)
-- Name: ix_hangfire_counter_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_counter_expireat ON hangfire.counter USING btree (expireat);


--
-- TOC entry 5058 (class 1259 OID 23239)
-- Name: ix_hangfire_counter_key; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_counter_key ON hangfire.counter USING btree (key);


--
-- TOC entry 5063 (class 1259 OID 23240)
-- Name: ix_hangfire_hash_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_hash_expireat ON hangfire.hash USING btree (expireat);


--
-- TOC entry 5064 (class 1259 OID 23241)
-- Name: ix_hangfire_job_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_expireat ON hangfire.job USING btree (expireat);


--
-- TOC entry 5065 (class 1259 OID 23242)
-- Name: ix_hangfire_job_statename; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_statename ON hangfire.job USING btree (statename);


--
-- TOC entry 5066 (class 1259 OID 23243)
-- Name: ix_hangfire_job_statename_is_not_null; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_statename_is_not_null ON hangfire.job USING btree (statename) INCLUDE (id) WHERE (statename IS NOT NULL);


--
-- TOC entry 5069 (class 1259 OID 23244)
-- Name: ix_hangfire_jobparameter_jobidandname; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON hangfire.jobparameter USING btree (jobid, name);


--
-- TOC entry 5072 (class 1259 OID 23245)
-- Name: ix_hangfire_jobqueue_fetchedat_queue_jobid; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_fetchedat_queue_jobid ON hangfire.jobqueue USING btree (fetchedat NULLS FIRST, queue, jobid);


--
-- TOC entry 5073 (class 1259 OID 23246)
-- Name: ix_hangfire_jobqueue_jobidandqueue; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON hangfire.jobqueue USING btree (jobid, queue);


--
-- TOC entry 5074 (class 1259 OID 23247)
-- Name: ix_hangfire_jobqueue_queueandfetchedat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON hangfire.jobqueue USING btree (queue, fetchedat);


--
-- TOC entry 5077 (class 1259 OID 23248)
-- Name: ix_hangfire_list_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_list_expireat ON hangfire.list USING btree (expireat);


--
-- TOC entry 5086 (class 1259 OID 23249)
-- Name: ix_hangfire_set_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_set_expireat ON hangfire.set USING btree (expireat);


--
-- TOC entry 5087 (class 1259 OID 23250)
-- Name: ix_hangfire_set_key_score; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_set_key_score ON hangfire.set USING btree (key, score);


--
-- TOC entry 5092 (class 1259 OID 23251)
-- Name: ix_hangfire_state_jobid; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_state_jobid ON hangfire.state USING btree (jobid);


--
-- TOC entry 5097 (class 1259 OID 23252)
-- Name: idx_accesorios_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_accesorios_identificadores ON public.accesorios USING btree (nombre, id_equipo, estado_eliminado);


--
-- TOC entry 5162 (class 1259 OID 23381)
-- Name: idx_audit_admin; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_audit_admin ON public.audit_logs USING btree (admin_carnet, "timestamp" DESC);


--
-- TOC entry 5163 (class 1259 OID 23382)
-- Name: idx_audit_entidad; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_audit_entidad ON public.audit_logs USING btree (entidad, entidad_id);


--
-- TOC entry 5139 (class 1259 OID 23253)
-- Name: idx_carreras_nombre; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_carreras_nombre ON public.carreras USING btree (nombre, estado_eliminado);


--
-- TOC entry 5100 (class 1259 OID 23254)
-- Name: idx_categorias_nombre; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_categorias_nombre ON public.categorias USING btree (nombre, estado_eliminado);


--
-- TOC entry 5105 (class 1259 OID 23255)
-- Name: idx_componentes; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_componentes ON public.componentes USING btree (nombre, id_equipo, estado_eliminado);


--
-- TOC entry 5146 (class 1259 OID 23256)
-- Name: idx_detalles_mantenimientos; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_detalles_mantenimientos ON public.detalles_mantenimientos USING btree (id_mantenimiento, estado_eliminado);


--
-- TOC entry 5149 (class 1259 OID 23257)
-- Name: idx_detalles_prestamos; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_detalles_prestamos ON public.detalles_prestamos USING btree (id_prestamo, estado_eliminado);


--
-- TOC entry 5108 (class 1259 OID 23258)
-- Name: idx_empresas_mantenimiento; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_empresas_mantenimiento ON public.empresas_mantenimiento USING btree (nombre, estado_eliminado);


--
-- TOC entry 5113 (class 1259 OID 23259)
-- Name: idx_equipos_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_equipos_identificadores ON public.equipos USING btree (id_grupo_equipo, codigo_imt, estado_eliminado);


--
-- TOC entry 5118 (class 1259 OID 23260)
-- Name: idx_gaveteros_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_gaveteros_identificadores ON public.gaveteros USING btree (nombre, id_mueble, estado_eliminado);


--
-- TOC entry 5123 (class 1259 OID 23261)
-- Name: idx_grupos_equipos_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_grupos_equipos_identificadores ON public.grupos_equipos USING btree (id_categoria, nombre, modelo, marca, estado_eliminado);


--
-- TOC entry 5128 (class 1259 OID 23262)
-- Name: idx_mantenimientos_fecha_empresa; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_mantenimientos_fecha_empresa ON public.mantenimientos USING btree (fecha_mantenimiento, fecha_final_mantenimiento, id_empresa, estado_eliminado);


--
-- TOC entry 5153 (class 1259 OID 23263)
-- Name: idx_muebles_nombre; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_muebles_nombre ON public.usuarios USING btree (nombre, estado_eliminado);


--
-- TOC entry 5135 (class 1259 OID 23264)
-- Name: idx_prestamos_fechas; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_prestamos_fechas ON public.prestamos USING btree (fecha_prestamo_esperada, fecha_devolucion_esperada, carnet, estado_eliminado);


--
-- TOC entry 5154 (class 1259 OID 23265)
-- Name: idx_usuarios_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_usuarios_email ON public.usuarios USING btree (email, estado_eliminado);


--
-- TOC entry 5150 (class 1259 OID 23266)
-- Name: ix_detalles_prestamos_id_equipo; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_detalles_prestamos_id_equipo ON public.detalles_prestamos USING btree (id_equipo);


--
-- TOC entry 5136 (class 1259 OID 23267)
-- Name: ix_prestamos_carnet_estado; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_prestamos_carnet_estado ON public.prestamos USING btree (carnet, estado_prestamo, estado_eliminado);


--
-- TOC entry 5155 (class 1259 OID 23268)
-- Name: ix_usuarios_refresh_token; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_usuarios_refresh_token ON public.usuarios USING btree (refresh_token);


--
-- TOC entry 5181 (class 2620 OID 23269)
-- Name: equipos trg_actualizar_costo_promedio_delete; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_actualizar_costo_promedio_delete AFTER DELETE ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_costo_promedio_grupo();


--
-- TOC entry 5182 (class 2620 OID 23270)
-- Name: equipos trg_actualizar_costo_promedio_insert; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_actualizar_costo_promedio_insert AFTER INSERT ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_costo_promedio_grupo();


--
-- TOC entry 5183 (class 2620 OID 23271)
-- Name: equipos trg_actualizar_costo_promedio_update; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_actualizar_costo_promedio_update AFTER UPDATE OF costo_referencia, estado_eliminado, estado_equipo ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_costo_promedio_grupo();


--
-- TOC entry 5184 (class 2620 OID 23272)
-- Name: equipos trg_equipo_estado_actualiza_grupo; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_equipo_estado_actualiza_grupo AFTER UPDATE OF estado_eliminado ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_cantidad_equipo_por_estado();


--
-- TOC entry 5185 (class 2620 OID 23273)
-- Name: equipos trg_equipos_after_insert; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_equipos_after_insert AFTER INSERT ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_incrementar_cantidad_equipos();


--
-- TOC entry 5187 (class 2620 OID 23274)
-- Name: gaveteros trg_gavetero_movimiento_actualiza_numero_mueble; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_gavetero_movimiento_actualiza_numero_mueble AFTER UPDATE ON public.gaveteros FOR EACH ROW WHEN ((old.id_mueble IS DISTINCT FROM new.id_mueble)) EXECUTE FUNCTION public.fn_actualizar_gavetero_tras_update_mueble();


--
-- TOC entry 5188 (class 2620 OID 23275)
-- Name: gaveteros trg_gaveteros_estado_conteo_mueble; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_gaveteros_estado_conteo_mueble AFTER UPDATE OF estado_eliminado ON public.gaveteros FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_conteo_gaveteros_por_estado();


--
-- TOC entry 5189 (class 2620 OID 23276)
-- Name: gaveteros trg_incrementar_gaveteros; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_incrementar_gaveteros AFTER INSERT ON public.gaveteros FOR EACH ROW EXECUTE FUNCTION public.fn_incrementar_numero_gaveteros();


--
-- TOC entry 5190 (class 2620 OID 23277)
-- Name: mantenimientos trg_mantenimientos_cascade_estado_a_detalles; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_mantenimientos_cascade_estado_a_detalles AFTER UPDATE OF estado_eliminado ON public.mantenimientos FOR EACH ROW EXECUTE FUNCTION public.fn_estado_eliminado_mantenimiento_a_detalle();


--
-- TOC entry 5191 (class 2620 OID 23278)
-- Name: prestamos trg_prestamos_estado_a_detalles; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_prestamos_estado_a_detalles AFTER UPDATE OF estado_eliminado ON public.prestamos FOR EACH ROW EXECUTE FUNCTION public.fn_estado_eliminado_prestamo_a_detalle();


--
-- TOC entry 5186 (class 2620 OID 23279)
-- Name: equipos trg_update_cantidad_tras_update_equipos; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_update_cantidad_tras_update_equipos AFTER UPDATE ON public.equipos FOR EACH ROW WHEN ((old.id_grupo_equipo IS DISTINCT FROM new.id_grupo_equipo)) EXECUTE FUNCTION public.fn_actualizar_cantidad_tras_update_equipos();


--
-- TOC entry 5164 (class 2606 OID 23280)
-- Name: jobparameter jobparameter_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 5165 (class 2606 OID 23285)
-- Name: state state_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 5166 (class 2606 OID 23290)
-- Name: accesorios Accesorio_Equipo_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accesorios
    ADD CONSTRAINT "Accesorio_Equipo_fk" FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- TOC entry 5167 (class 2606 OID 23295)
-- Name: componentes Componente_Equipo_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.componentes
    ADD CONSTRAINT "Componente_Equipo_fk" FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- TOC entry 5168 (class 2606 OID 23300)
-- Name: equipos Equipo_Gavetero_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT "Equipo_Gavetero_fk" FOREIGN KEY (id_gavetero) REFERENCES public.gaveteros(id_gavetero);


--
-- TOC entry 5169 (class 2606 OID 23305)
-- Name: equipos Equipo_Grupo_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT "Equipo_Grupo_fk" FOREIGN KEY (id_grupo_equipo) REFERENCES public.grupos_equipos(id_grupo_equipo);


--
-- TOC entry 5171 (class 2606 OID 23310)
-- Name: grupos_equipos Grupo_Categoria_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grupos_equipos
    ADD CONSTRAINT "Grupo_Categoria_fk" FOREIGN KEY (id_categoria) REFERENCES public.categorias(id_categoria);


--
-- TOC entry 5172 (class 2606 OID 23315)
-- Name: mantenimientos Mantenimiento_Empresa_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mantenimientos
    ADD CONSTRAINT "Mantenimiento_Empresa_fk" FOREIGN KEY (id_empresa) REFERENCES public.empresas_mantenimiento(id_empresa_mantenimiento);


--
-- TOC entry 5173 (class 2606 OID 23320)
-- Name: prestamos Prestamo_Usuario_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.prestamos
    ADD CONSTRAINT "Prestamo_Usuario_fk" FOREIGN KEY (carnet) REFERENCES public.usuarios(carnet);


--
-- TOC entry 5174 (class 2606 OID 23325)
-- Name: prestamos Prestamo_contrato_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.prestamos
    ADD CONSTRAINT "Prestamo_contrato_fk" FOREIGN KEY (id_contrato) REFERENCES public.contratos(id) NOT VALID;


--
-- TOC entry 5177 (class 2606 OID 23330)
-- Name: detalles_prestamos detalles_prestamos_id_grupo_equipo_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT detalles_prestamos_id_grupo_equipo_fkey FOREIGN KEY (id_grupo_equipo) REFERENCES public.grupos_equipos(id_grupo_equipo);


--
-- TOC entry 5175 (class 2606 OID 23335)
-- Name: detalles_mantenimientos fk_detalle_mantenimiento_equipo; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_mantenimientos
    ADD CONSTRAINT fk_detalle_mantenimiento_equipo FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- TOC entry 5176 (class 2606 OID 23340)
-- Name: detalles_mantenimientos fk_detalles_mantenimiento; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_mantenimientos
    ADD CONSTRAINT fk_detalles_mantenimiento FOREIGN KEY (id_mantenimiento) REFERENCES public.mantenimientos(id_mantenimiento);


--
-- TOC entry 5178 (class 2606 OID 23345)
-- Name: detalles_prestamos fk_equipo; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT fk_equipo FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- TOC entry 5170 (class 2606 OID 23350)
-- Name: gaveteros fk_gaveteros_muebles; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gaveteros
    ADD CONSTRAINT fk_gaveteros_muebles FOREIGN KEY (id_mueble) REFERENCES public.muebles(id_mueble);


--
-- TOC entry 5179 (class 2606 OID 23355)
-- Name: detalles_prestamos fk_prestamo; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT fk_prestamo FOREIGN KEY (id_prestamo) REFERENCES public.prestamos(id_prestamo);


--
-- TOC entry 5180 (class 2606 OID 23360)
-- Name: usuarios fk_usuarios_carrera; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_carrera FOREIGN KEY (id_carrera) REFERENCES public.carreras(id_carrera);


--
-- TOC entry 5399 (class 0 OID 0)
-- Dependencies: 7
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2026-06-05 22:38:48

--
-- PostgreSQL database dump complete
--

