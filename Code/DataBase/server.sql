--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4
-- Dumped by pg_dump version 17.4

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
-- Name: hangfire; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA hangfire;


ALTER SCHEMA hangfire OWNER TO postgres;

--
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO postgres;

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS '';


--
-- Name: pgcrypto; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;


--
-- Name: EXTENSION pgcrypto; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION pgcrypto IS 'cryptographic functions';


--
-- Name: estado_disponibilidad; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.estado_disponibilidad AS ENUM (
    'disponible',
    'mantenimiento',
    'ocupado'
);


ALTER TYPE public.estado_disponibilidad OWNER TO postgres;

--
-- Name: estado_equipo; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.estado_equipo AS ENUM (
    'operativo',
    'parcialmente_operativo',
    'inoperativo'
);


ALTER TYPE public.estado_equipo OWNER TO postgres;

--
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
-- Name: tipo_mantenimiento; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.tipo_mantenimiento AS ENUM (
    'correctivo',
    'preventivo'
);


ALTER TYPE public.tipo_mantenimiento OWNER TO postgres;

--
-- Name: tipo_usuario; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.tipo_usuario AS ENUM (
    'docente',
    'administrador',
    'estudiante'
);


ALTER TYPE public.tipo_usuario OWNER TO postgres;

--
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
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.aggregatedcounter_id_seq OWNED BY hangfire.aggregatedcounter.id;


--
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
-- Name: counter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.counter_id_seq OWNED BY hangfire.counter.id;


--
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
-- Name: hash_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.hash_id_seq OWNED BY hangfire.hash.id;


--
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
-- Name: job_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.job_id_seq OWNED BY hangfire.job.id;


--
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
-- Name: jobparameter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.jobparameter_id_seq OWNED BY hangfire.jobparameter.id;


--
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
-- Name: jobqueue_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.jobqueue_id_seq OWNED BY hangfire.jobqueue.id;


--
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
-- Name: list_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.list_id_seq OWNED BY hangfire.list.id;


--
-- Name: lock; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.lock (
    resource text NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL,
    acquired timestamp with time zone
);


ALTER TABLE hangfire.lock OWNER TO postgres;

--
-- Name: schema; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.schema (
    version integer NOT NULL
);


ALTER TABLE hangfire.schema OWNER TO postgres;

--
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
-- Name: set_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.set_id_seq OWNED BY hangfire.set.id;


--
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
-- Name: state_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.state_id_seq OWNED BY hangfire.state.id;


--
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
-- Name: COLUMN accesorios.id_accesorio; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.accesorios.id_accesorio IS 'Código del accesorio';


--
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
-- Name: categorias; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.categorias (
    id_categoria integer NOT NULL,
    nombre character varying(255) NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.categorias OWNER TO postgres;

--
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
-- Name: COLUMN componentes.id_componente; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.componentes.id_componente IS 'Código del componente';


--
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
-- Name: COLUMN empresas_mantenimiento.id_empresa_mantenimiento; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.empresas_mantenimiento.id_empresa_mantenimiento IS 'Código empresa';


--
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
-- Name: COLUMN grupos_equipos.descripcion; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.grupos_equipos.descripcion IS 'Esto se mostrar en la pagina web';


--
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
-- Name: COLUMN muebles.id_mueble; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.muebles.id_mueble IS 'Código del mueble';


--
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
-- Name: COLUMN prestamos.id_prestamo; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.prestamos.id_prestamo IS 'Código del préstamo';


--
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
-- Name: audit_logs_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.audit_logs_id_seq OWNED BY public.audit_logs.id;


--
-- Name: carreras; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.carreras (
    id_carrera integer NOT NULL,
    nombre character varying(255) NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.carreras OWNER TO postgres;

--
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
-- Name: carrera_id_carrera_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.carrera_id_carrera_seq OWNED BY public.carreras.id_carrera;


--
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
-- Name: contratos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.contratos (
    id integer NOT NULL,
    contrato text
);


ALTER TABLE public.contratos OWNER TO postgres;

--
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
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.detalles_mantenimientos_id_detalle_mantenimiento_seq OWNED BY public.detalles_mantenimientos.id_detalle_mantenimiento;


--
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
-- Name: aggregatedcounter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter ALTER COLUMN id SET DEFAULT nextval('hangfire.aggregatedcounter_id_seq'::regclass);


--
-- Name: counter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.counter ALTER COLUMN id SET DEFAULT nextval('hangfire.counter_id_seq'::regclass);


--
-- Name: hash id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash ALTER COLUMN id SET DEFAULT nextval('hangfire.hash_id_seq'::regclass);


--
-- Name: job id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.job ALTER COLUMN id SET DEFAULT nextval('hangfire.job_id_seq'::regclass);


--
-- Name: jobparameter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter ALTER COLUMN id SET DEFAULT nextval('hangfire.jobparameter_id_seq'::regclass);


--
-- Name: jobqueue id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobqueue ALTER COLUMN id SET DEFAULT nextval('hangfire.jobqueue_id_seq'::regclass);


--
-- Name: list id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.list ALTER COLUMN id SET DEFAULT nextval('hangfire.list_id_seq'::regclass);


--
-- Name: set id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set ALTER COLUMN id SET DEFAULT nextval('hangfire.set_id_seq'::regclass);


--
-- Name: state id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state ALTER COLUMN id SET DEFAULT nextval('hangfire.state_id_seq'::regclass);


--
-- Name: audit_logs id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.audit_logs ALTER COLUMN id SET DEFAULT nextval('public.audit_logs_id_seq'::regclass);


--
-- Data for Name: aggregatedcounter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.aggregatedcounter (id, key, value, expireat) FROM stdin;
1	stats:succeeded:2026-06-01	2	2026-06-30 22:50:00.66743-04
277	stats:succeeded:2026-06-07	5	2026-07-06 23:40:07.965354-04
292	stats:succeeded:2026-06-11-23	1	2026-06-12 19:56:33.601991-04
294	stats:succeeded:2026-06-11	1	2026-07-11 19:56:32.601991-04
189	stats:succeeded:2026-06-05	22	2026-07-05 19:57:25.730095-04
7	stats:succeeded:2026-06-03	9	2026-07-03 19:02:15.151647-04
338	stats:succeeded:2026-06-12-03	6	2026-06-12 23:50:04.624147-04
290	stats:succeeded:2026-06-12-00	6	2026-06-12 20:50:09.095883-04
312	stats:succeeded:2026-06-12-01	4	2026-06-12 21:50:06.817735-04
356	stats:succeeded:2026-06-12-04	6	2026-06-13 00:50:13.979423-04
374	stats:succeeded:2026-06-12-05	1	2026-06-13 01:00:01.143367-04
377	stats:succeeded:2026-06-12-14	1	2026-06-13 10:54:38.987424-04
243	stats:succeeded:2026-06-06	11	2026-07-05 23:46:25.802054-04
379	stats:succeeded:2026-06-12-15	2	2026-06-13 11:50:06.611433-04
291	stats:succeeded:2026-06-12	32	2026-07-12 11:50:05.611433-04
3	stats:succeeded	138	\N
320	stats:succeeded:2026-06-12-02	6	2026-06-12 22:50:05.854412-04
24	stats:succeeded:2026-06-04	56	2026-07-04 19:30:05.605239-04
\.


--
-- Data for Name: counter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.counter (id, key, value, expireat) FROM stdin;
417	stats:succeeded:2026-06-12	1	2026-07-12 12:00:07.952546-04
418	stats:succeeded:2026-06-12-16	1	2026-06-13 12:00:08.952546-04
419	stats:succeeded	1	\N
\.


--
-- Data for Name: hash; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.hash (id, key, field, value, expireat, updatecount) FROM stdin;
1	recurring-job:estado-prestamo	Queue	default	\N	0
2	recurring-job:estado-prestamo	Cron	*/10 * * * *	\N	0
3	recurring-job:estado-prestamo	TimeZoneId	UTC	\N	0
4	recurring-job:estado-prestamo	Job	{"Type":"IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Method":"Execute","ParameterTypes":"[]","Arguments":"[]"}	\N	0
5	recurring-job:estado-prestamo	CreatedAt	2026-06-01T02:32:59.4235310Z	\N	0
7	recurring-job:estado-prestamo	V	2	\N	0
8	recurring-job:estado-prestamo	LastExecution	2026-06-12T16:00:08.8978866Z	\N	0
6	recurring-job:estado-prestamo	NextExecution	2026-06-12T16:10:00.0000000Z	\N	0
9	recurring-job:estado-prestamo	LastJobId	139	\N	0
\.


--
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
127	381	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 03:40:02.939591-04	2026-06-12 23:40:02.97561-04	0
112	336	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 00:50:08.792415-04	2026-06-12 20:50:09.095883-04	0
121	363	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 02:40:03.386129-04	2026-06-12 22:40:03.440899-04	0
131	393	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 04:20:06.860424-04	2026-06-13 00:20:06.934628-04	0
115	345	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 01:49:36.19304-04	2026-06-12 21:49:38.514756-04	0
106	318	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-11 23:56:28.800863-04	2026-06-12 19:56:33.601991-04	0
125	375	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 03:20:13.293545-04	2026-06-12 23:20:13.342271-04	0
132	396	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 04:30:09.044989-04	2026-06-13 00:30:09.113134-04	0
118	354	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 02:10:11.394298-04	2026-06-12 22:10:11.446674-04	0
109	327	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 00:20:04.290662-04	2026-06-12 20:20:04.336025-04	0
138	414	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 15:50:06.515694-04	2026-06-13 11:50:06.611433-04	0
135	405	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 05:00:01.099271-04	2026-06-13 01:00:01.143367-04	0
129	387	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 04:00:05.288808-04	2026-06-13 00:00:05.343648-04	0
130	390	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 04:10:05.854865-04	2026-06-13 00:10:05.898217-04	0
126	378	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 03:30:00.478733-04	2026-06-12 23:30:00.538407-04	0
113	339	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 01:00:09.630912-04	2026-06-12 21:00:09.737352-04	0
122	366	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 02:50:05.820926-04	2026-06-12 22:50:05.854412-04	0
133	399	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 04:40:11.417108-04	2026-06-13 00:40:11.465659-04	0
116	348	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 01:50:06.738766-04	2026-06-12 21:50:06.817735-04	0
107	321	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 00:00:15.071701-04	2026-06-12 20:00:15.319504-04	0
124	372	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 03:10:10.600715-04	2026-06-12 23:10:10.662014-04	0
136	408	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 14:54:32.830518-04	2026-06-13 10:54:38.987424-04	0
110	330	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 00:30:05.751526-04	2026-06-12 20:30:06.117073-04	0
119	357	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 02:20:13.901039-04	2026-06-12 22:20:13.956114-04	0
139	417	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 16:00:08.907334-04	2026-06-13 12:00:08.952546-04	0
114	342	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 01:35:09.370951-04	2026-06-12 21:35:12.700623-04	0
108	324	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 00:10:02.061014-04	2026-06-12 20:10:02.237775-04	0
123	369	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 03:00:08.270679-04	2026-06-12 23:00:08.336041-04	0
134	402	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 04:50:13.93315-04	2026-06-13 00:50:13.979423-04	0
111	333	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 00:40:06.662386-04	2026-06-12 20:40:06.72763-04	0
117	351	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 02:00:09.333159-04	2026-06-12 22:00:09.407291-04	0
128	384	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 03:50:04.588983-04	2026-06-12 23:50:04.624147-04	0
137	411	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 15:48:50.858356-04	2026-06-13 11:48:55.876126-04	0
120	360	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-06-12 02:30:01.295451-04	2026-06-12 22:30:01.340576-04	0
\.


--
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
421	106	RecurringJobId	"estado-prestamo"	0
422	106	Time	1781222188	0
423	106	CurrentCulture	"es-MX"	0
424	106	CurrentUICulture	"es-MX"	0
433	109	RecurringJobId	"estado-prestamo"	0
434	109	Time	1781223604	0
435	109	CurrentCulture	"es-MX"	0
436	109	CurrentUICulture	"es-MX"	0
445	112	RecurringJobId	"estado-prestamo"	0
446	112	Time	1781225408	0
447	112	CurrentCulture	"es-MX"	0
448	112	CurrentUICulture	"es-MX"	0
457	115	RecurringJobId	"estado-prestamo"	0
458	115	Time	1781228975	0
459	115	CurrentCulture	"es-MX"	0
460	115	CurrentUICulture	"es-MX"	0
469	118	RecurringJobId	"estado-prestamo"	0
470	118	Time	1781230211	0
471	118	CurrentCulture	"es-MX"	0
472	118	CurrentUICulture	"es-MX"	0
481	121	RecurringJobId	"estado-prestamo"	0
482	121	Time	1781232003	0
483	121	CurrentCulture	"es-MX"	0
484	121	CurrentUICulture	"es-MX"	0
509	128	RecurringJobId	"estado-prestamo"	0
510	128	Time	1781236204	0
511	128	CurrentCulture	"es-MX"	0
512	128	CurrentUICulture	"es-MX"	0
533	134	RecurringJobId	"estado-prestamo"	0
534	134	Time	1781239813	0
535	134	CurrentCulture	"es-MX"	0
536	134	CurrentUICulture	"es-MX"	0
545	137	RecurringJobId	"estado-prestamo"	0
546	137	Time	1781279330	0
547	137	CurrentCulture	"es-MX"	0
548	137	CurrentUICulture	"es-MX"	0
425	107	RecurringJobId	"estado-prestamo"	0
426	107	Time	1781222415	0
427	107	CurrentCulture	"es-MX"	0
428	107	CurrentUICulture	"es-MX"	0
437	110	RecurringJobId	"estado-prestamo"	0
438	110	Time	1781224205	0
439	110	CurrentCulture	"es-MX"	0
440	110	CurrentUICulture	"es-MX"	0
449	113	RecurringJobId	"estado-prestamo"	0
450	113	Time	1781226009	0
451	113	CurrentCulture	"es-MX"	0
452	113	CurrentUICulture	"es-MX"	0
461	116	RecurringJobId	"estado-prestamo"	0
462	116	Time	1781229006	0
463	116	CurrentCulture	"es-MX"	0
464	116	CurrentUICulture	"es-MX"	0
473	119	RecurringJobId	"estado-prestamo"	0
474	119	Time	1781230813	0
475	119	CurrentCulture	"es-MX"	0
476	119	CurrentUICulture	"es-MX"	0
489	123	RecurringJobId	"estado-prestamo"	0
490	123	Time	1781233208	0
491	123	CurrentCulture	"es-MX"	0
492	123	CurrentUICulture	"es-MX"	0
497	125	RecurringJobId	"estado-prestamo"	0
498	125	Time	1781234413	0
499	125	CurrentCulture	"es-MX"	0
500	125	CurrentUICulture	"es-MX"	0
505	127	RecurringJobId	"estado-prestamo"	0
506	127	Time	1781235602	0
507	127	CurrentCulture	"es-MX"	0
508	127	CurrentUICulture	"es-MX"	0
513	129	RecurringJobId	"estado-prestamo"	0
514	129	Time	1781236805	0
515	129	CurrentCulture	"es-MX"	0
516	129	CurrentUICulture	"es-MX"	0
517	130	RecurringJobId	"estado-prestamo"	0
518	130	Time	1781237405	0
519	130	CurrentCulture	"es-MX"	0
520	130	CurrentUICulture	"es-MX"	0
537	135	RecurringJobId	"estado-prestamo"	0
538	135	Time	1781240401	0
539	135	CurrentCulture	"es-MX"	0
540	135	CurrentUICulture	"es-MX"	0
549	138	RecurringJobId	"estado-prestamo"	0
550	138	Time	1781279406	0
551	138	CurrentCulture	"es-MX"	0
552	138	CurrentUICulture	"es-MX"	0
429	108	RecurringJobId	"estado-prestamo"	0
430	108	Time	1781223002	0
431	108	CurrentCulture	"es-MX"	0
432	108	CurrentUICulture	"es-MX"	0
441	111	RecurringJobId	"estado-prestamo"	0
442	111	Time	1781224806	0
443	111	CurrentCulture	"es-MX"	0
444	111	CurrentUICulture	"es-MX"	0
453	114	RecurringJobId	"estado-prestamo"	0
454	114	Time	1781228109	0
455	114	CurrentCulture	"es-MX"	0
456	114	CurrentUICulture	"es-MX"	0
465	117	RecurringJobId	"estado-prestamo"	0
466	117	Time	1781229609	0
467	117	CurrentCulture	"es-MX"	0
468	117	CurrentUICulture	"es-MX"	0
477	120	RecurringJobId	"estado-prestamo"	0
478	120	Time	1781231401	0
479	120	CurrentCulture	"es-MX"	0
480	120	CurrentUICulture	"es-MX"	0
485	122	RecurringJobId	"estado-prestamo"	0
486	122	Time	1781232605	0
487	122	CurrentCulture	"es-MX"	0
488	122	CurrentUICulture	"es-MX"	0
493	124	RecurringJobId	"estado-prestamo"	0
494	124	Time	1781233810	0
495	124	CurrentCulture	"es-MX"	0
496	124	CurrentUICulture	"es-MX"	0
501	126	RecurringJobId	"estado-prestamo"	0
502	126	Time	1781235000	0
503	126	CurrentCulture	"es-MX"	0
504	126	CurrentUICulture	"es-MX"	0
521	131	RecurringJobId	"estado-prestamo"	0
522	131	Time	1781238006	0
523	131	CurrentCulture	"es-MX"	0
524	131	CurrentUICulture	"es-MX"	0
525	132	RecurringJobId	"estado-prestamo"	0
526	132	Time	1781238609	0
527	132	CurrentCulture	"es-MX"	0
528	132	CurrentUICulture	"es-MX"	0
529	133	RecurringJobId	"estado-prestamo"	0
530	133	Time	1781239211	0
531	133	CurrentCulture	"es-MX"	0
532	133	CurrentUICulture	"es-MX"	0
541	136	RecurringJobId	"estado-prestamo"	0
542	136	Time	1781276072	0
543	136	CurrentCulture	"es-MX"	0
544	136	CurrentUICulture	"es-MX"	0
553	139	RecurringJobId	"estado-prestamo"	0
554	139	Time	1781280008	0
555	139	CurrentCulture	"es-MX"	0
556	139	CurrentUICulture	"es-MX"	0
\.


--
-- Data for Name: jobqueue; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.jobqueue (id, jobid, queue, fetchedat, updatecount) FROM stdin;
\.


--
-- Data for Name: list; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.list (id, key, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: lock; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.lock (resource, updatecount, acquired) FROM stdin;
\.


--
-- Data for Name: schema; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.schema (version) FROM stdin;
23
\.


--
-- Data for Name: server; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.server (id, data, lastheartbeat, updatecount) FROM stdin;
\.


--
-- Data for Name: set; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.set (id, key, score, value, expireat, updatecount) FROM stdin;
1	recurring-jobs	1781280600	estado-prestamo	\N	0
\.


--
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
321	107	Succeeded	\N	2026-06-12 00:00:15.322751-04	{"Latency": "171", "SucceededAt": "2026-06-12T00:00:15.2974402Z", "PerformanceDuration": "54"}	0
330	110	Succeeded	\N	2026-06-12 00:30:06.139365-04	{"Latency": "256", "SucceededAt": "2026-06-12T00:30:06.0819050Z", "PerformanceDuration": "72"}	0
336	112	Succeeded	\N	2026-06-12 00:50:09.101446-04	{"Latency": "190", "SucceededAt": "2026-06-12T00:50:09.0594112Z", "PerformanceDuration": "75"}	0
344	115	Processing	\N	2026-06-12 01:49:36.555345-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "6976f811-0e88-4af3-a182-55637ef8383f", "StartedAt": "2026-06-12T01:49:36.5265361Z"}	0
351	117	Succeeded	\N	2026-06-12 02:00:09.408934-04	{"Latency": "45", "SucceededAt": "2026-06-12T02:00:09.3993280Z", "PerformanceDuration": "21"}	0
376	126	Enqueued	Triggered by recurring job scheduler	2026-06-12 03:30:00.488966-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T03:30:00.4884263Z"}	0
389	130	Processing	\N	2026-06-12 04:10:05.880602-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "fceeaa19-f73a-481b-8791-fc66838f1cd5", "StartedAt": "2026-06-12T04:10:05.8723762Z"}	0
392	131	Processing	\N	2026-06-12 04:20:06.924098-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "f47d9775-0bc5-4b32-98ca-ebb99906aa56", "StartedAt": "2026-06-12T04:20:06.9165420Z"}	0
395	132	Processing	\N	2026-06-12 04:30:09.081738-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "0347da5f-0937-414d-91f3-044e817e0525", "StartedAt": "2026-06-12T04:30:09.0749450Z"}	0
398	133	Processing	\N	2026-06-12 04:40:11.457063-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "69a4c2e5-9793-4757-b656-09656158fbea", "StartedAt": "2026-06-12T04:40:11.4524202Z"}	0
402	134	Succeeded	\N	2026-06-12 04:50:13.980837-04	{"Latency": "32", "SucceededAt": "2026-06-12T04:50:13.9742525Z", "PerformanceDuration": "9"}	0
408	136	Succeeded	\N	2026-06-12 14:54:39.015562-04	{"Latency": "382", "SucceededAt": "2026-06-12T14:54:38.8724860Z", "PerformanceDuration": "5660"}	0
415	139	Enqueued	Triggered by recurring job scheduler	2026-06-12 16:00:08.915539-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T16:00:08.9152594Z"}	0
416	139	Processing	\N	2026-06-12 16:00:08.928265-04	{"ServerId": "x:21768:5f8a7391-72d8-4c56-8b02-9e31eee73ebc", "WorkerId": "90e16590-7d8b-405c-b9a4-8724f9e82a71", "StartedAt": "2026-06-12T16:00:08.9254987Z"}	0
417	139	Succeeded	\N	2026-06-12 16:00:08.976648-04	{"Latency": "24", "SucceededAt": "2026-06-12T16:00:08.9450498Z", "PerformanceDuration": "13"}	0
316	106	Enqueued	Triggered by recurring job scheduler	2026-06-11 23:56:29.051585-04	{"Queue": "default", "EnqueuedAt": "2026-06-11T23:56:29.0256813Z"}	0
322	108	Enqueued	Triggered by recurring job scheduler	2026-06-12 00:10:02.074065-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T00:10:02.0737456Z"}	0
323	108	Processing	\N	2026-06-12 00:10:02.091123-04	{"ServerId": "x:7152:49468b21-b203-4ba3-adaf-63e5b009efcd", "WorkerId": "b8f2f8f3-bc1c-4443-885b-7e3f0215ca6a", "StartedAt": "2026-06-12T00:10:02.0876265Z"}	0
324	108	Succeeded	\N	2026-06-12 00:10:02.238963-04	{"Latency": "34", "SucceededAt": "2026-06-12T00:10:02.2328223Z", "PerformanceDuration": "137"}	0
331	111	Enqueued	Triggered by recurring job scheduler	2026-06-12 00:40:06.672641-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T00:40:06.6720768Z"}	0
337	113	Enqueued	Triggered by recurring job scheduler	2026-06-12 01:00:09.671568-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T01:00:09.6710912Z"}	0
338	113	Processing	\N	2026-06-12 01:00:09.697907-04	{"ServerId": "x:6856:4ebcee2b-800a-4c8b-8a3f-0a0e183acf84", "WorkerId": "8105bdf2-fd45-401e-bc30-eff94802c557", "StartedAt": "2026-06-12T01:00:09.6919752Z"}	0
339	113	Succeeded	\N	2026-06-12 01:00:09.74108-04	{"Latency": "73", "SucceededAt": "2026-06-12T01:00:09.7257141Z", "PerformanceDuration": "20"}	0
345	115	Succeeded	\N	2026-06-12 01:49:38.51859-04	{"Latency": "374", "SucceededAt": "2026-06-12T01:49:38.4390319Z", "PerformanceDuration": "1871"}	0
352	118	Enqueued	Triggered by recurring job scheduler	2026-06-12 02:10:11.40815-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T02:10:11.4077703Z"}	0
354	118	Succeeded	\N	2026-06-12 02:10:11.447749-04	{"Latency": "35", "SucceededAt": "2026-06-12T02:10:11.4423410Z", "PerformanceDuration": "12"}	0
377	126	Processing	\N	2026-06-12 03:30:00.511895-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "3312d301-9f6e-49a6-babc-59f640203c40", "StartedAt": "2026-06-12T03:30:00.5034621Z"}	0
390	130	Succeeded	\N	2026-06-12 04:10:05.899182-04	{"Latency": "29", "SucceededAt": "2026-06-12T04:10:05.8926046Z", "PerformanceDuration": "8"}	0
391	131	Enqueued	Triggered by recurring job scheduler	2026-06-12 04:20:06.907617-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T04:20:06.9073818Z"}	0
393	131	Succeeded	\N	2026-06-12 04:20:06.935222-04	{"Latency": "67", "SucceededAt": "2026-06-12T04:20:06.9316709Z", "PerformanceDuration": "4"}	0
394	132	Enqueued	Triggered by recurring job scheduler	2026-06-12 04:30:09.05631-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T04:30:09.0560023Z"}	0
396	132	Succeeded	\N	2026-06-12 04:30:09.115444-04	{"Latency": "44", "SucceededAt": "2026-06-12T04:30:09.1037810Z", "PerformanceDuration": "14"}	0
397	133	Enqueued	Triggered by recurring job scheduler	2026-06-12 04:40:11.428211-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T04:40:11.4279990Z"}	0
399	133	Succeeded	\N	2026-06-12 04:40:11.466213-04	{"Latency": "42", "SucceededAt": "2026-06-12T04:40:11.4628419Z", "PerformanceDuration": "3"}	0
401	134	Processing	\N	2026-06-12 04:50:13.962606-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "86b80cc4-3c43-432e-9102-0c141a0b0e7a", "StartedAt": "2026-06-12T04:50:13.9601569Z"}	0
409	137	Enqueued	Triggered by recurring job scheduler	2026-06-12 15:48:50.970336-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T15:48:50.9427026Z"}	0
413	138	Processing	\N	2026-06-12 15:50:06.55514-04	{"ServerId": "x:21768:5f8a7391-72d8-4c56-8b02-9e31eee73ebc", "WorkerId": "504d969f-49ab-45a2-8641-3ec5da54474a", "StartedAt": "2026-06-12T15:50:06.5483421Z"}	0
317	106	Processing	\N	2026-06-11 23:56:29.215203-04	{"ServerId": "x:7152:49468b21-b203-4ba3-adaf-63e5b009efcd", "WorkerId": "17805fde-ebb3-41f3-8ddf-e9b4420cdf99", "StartedAt": "2026-06-11T23:56:29.1751758Z"}	0
325	109	Enqueued	Triggered by recurring job scheduler	2026-06-12 00:20:04.303651-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T00:20:04.3033568Z"}	0
332	111	Processing	\N	2026-06-12 00:40:06.697826-04	{"ServerId": "x:28772:7ad13435-85c7-4ec2-90bc-140c8ff0caa3", "WorkerId": "2a5a5fc6-8fa8-4142-8eee-6ba74270d369", "StartedAt": "2026-06-12T00:40:06.6917249Z"}	0
340	114	Enqueued	Triggered by recurring job scheduler	2026-06-12 01:35:09.493453-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T01:35:09.4541845Z"}	0
346	116	Enqueued	Triggered by recurring job scheduler	2026-06-12 01:50:06.75281-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T01:50:06.7520642Z"}	0
355	119	Enqueued	Triggered by recurring job scheduler	2026-06-12 02:20:13.912965-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T02:20:13.9126038Z"}	0
357	119	Succeeded	\N	2026-06-12 02:20:13.958629-04	{"Latency": "38", "SucceededAt": "2026-06-12T02:20:13.9514566Z", "PerformanceDuration": "12"}	0
378	126	Succeeded	\N	2026-06-12 03:30:00.540464-04	{"Latency": "39", "SucceededAt": "2026-06-12T03:30:00.5257178Z", "PerformanceDuration": "6"}	0
400	134	Enqueued	Triggered by recurring job scheduler	2026-06-12 04:50:13.951419-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T04:50:13.9511764Z"}	0
404	135	Processing	\N	2026-06-12 05:00:01.134138-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "9fa3572e-2a93-4100-a41e-8070788d665f", "StartedAt": "2026-06-12T05:00:01.1313647Z"}	0
410	137	Processing	\N	2026-06-12 15:48:51.054358-04	{"ServerId": "x:21768:5f8a7391-72d8-4c56-8b02-9e31eee73ebc", "WorkerId": "504d969f-49ab-45a2-8641-3ec5da54474a", "StartedAt": "2026-06-12T15:48:51.0245712Z"}	0
318	106	Succeeded	\N	2026-06-11 23:56:33.620609-04	{"Latency": "426", "SucceededAt": "2026-06-11T23:56:33.5820806Z", "PerformanceDuration": "4355"}	0
326	109	Processing	\N	2026-06-12 00:20:04.320933-04	{"ServerId": "x:7152:49468b21-b203-4ba3-adaf-63e5b009efcd", "WorkerId": "0c75b1f9-67c0-4de5-9670-0bd19bf17cf5", "StartedAt": "2026-06-12T00:20:04.3152961Z"}	0
327	109	Succeeded	\N	2026-06-12 00:20:04.33683-04	{"Latency": "33", "SucceededAt": "2026-06-12T00:20:04.3330328Z", "PerformanceDuration": "8"}	0
333	111	Succeeded	\N	2026-06-12 00:40:06.730032-04	{"Latency": "40", "SucceededAt": "2026-06-12T00:40:06.7157062Z", "PerformanceDuration": "13"}	0
341	114	Processing	\N	2026-06-12 01:35:09.59191-04	{"ServerId": "x:31768:f87a1240-2f03-4eeb-ad7c-327430ad76f6", "WorkerId": "6dcf34dd-898c-4489-89f4-fdd5af5a927b", "StartedAt": "2026-06-12T01:35:09.5632346Z"}	0
347	116	Processing	\N	2026-06-12 01:50:06.780219-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "6976f811-0e88-4af3-a182-55637ef8383f", "StartedAt": "2026-06-12T01:50:06.7681880Z"}	0
356	119	Processing	\N	2026-06-12 02:20:13.933185-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "3312d301-9f6e-49a6-babc-59f640203c40", "StartedAt": "2026-06-12T02:20:13.9247074Z"}	0
358	120	Enqueued	Triggered by recurring job scheduler	2026-06-12 02:30:01.301779-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T02:30:01.3015136Z"}	0
360	120	Succeeded	\N	2026-06-12 02:30:01.341878-04	{"Latency": "29", "SucceededAt": "2026-06-12T02:30:01.3359627Z", "PerformanceDuration": "11"}	0
362	121	Processing	\N	2026-06-12 02:40:03.43235-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "558481b3-77e0-4b9b-a859-40fd899f9a1a", "StartedAt": "2026-06-12T02:40:03.4269547Z"}	0
364	122	Enqueued	Triggered by recurring job scheduler	2026-06-12 02:50:05.825497-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T02:50:05.8252133Z"}	0
368	123	Processing	\N	2026-06-12 03:00:08.322773-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "fceeaa19-f73a-481b-8791-fc66838f1cd5", "StartedAt": "2026-06-12T03:00:08.3200425Z"}	0
370	124	Enqueued	Triggered by recurring job scheduler	2026-06-12 03:10:10.610794-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T03:10:10.6105521Z"}	0
371	124	Processing	\N	2026-06-12 03:10:10.6327-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "86b80cc4-3c43-432e-9102-0c141a0b0e7a", "StartedAt": "2026-06-12T03:10:10.6277353Z"}	0
372	124	Succeeded	\N	2026-06-12 03:10:10.664299-04	{"Latency": "39", "SucceededAt": "2026-06-12T03:10:10.6569955Z", "PerformanceDuration": "17"}	0
382	128	Enqueued	Triggered by recurring job scheduler	2026-06-12 03:50:04.592716-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T03:50:04.5925699Z"}	0
383	128	Processing	\N	2026-06-12 03:50:04.607537-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "558481b3-77e0-4b9b-a859-40fd899f9a1a", "StartedAt": "2026-06-12T03:50:04.6027039Z"}	0
384	128	Succeeded	\N	2026-06-12 03:50:04.625861-04	{"Latency": "20", "SucceededAt": "2026-06-12T03:50:04.6183169Z", "PerformanceDuration": "8"}	0
403	135	Enqueued	Triggered by recurring job scheduler	2026-06-12 05:00:01.117507-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T05:00:01.1170563Z"}	0
405	135	Succeeded	\N	2026-06-12 05:00:01.144558-04	{"Latency": "36", "SucceededAt": "2026-06-12T05:00:01.1410843Z", "PerformanceDuration": "4"}	0
411	137	Succeeded	\N	2026-06-12 15:48:55.87981-04	{"Latency": "207", "SucceededAt": "2026-06-12T15:48:55.7811535Z", "PerformanceDuration": "4714"}	0
319	107	Enqueued	Triggered by recurring job scheduler	2026-06-12 00:00:15.110746-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T00:00:15.1102881Z"}	0
328	110	Enqueued	Triggered by recurring job scheduler	2026-06-12 00:30:05.871419-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T00:30:05.8296394Z"}	0
334	112	Enqueued	Triggered by recurring job scheduler	2026-06-12 00:50:08.871051-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T00:50:08.8385505Z"}	0
342	114	Succeeded	\N	2026-06-12 01:35:12.706546-04	{"Latency": "233", "SucceededAt": "2026-06-12T01:35:12.6593984Z", "PerformanceDuration": "3055"}	0
349	117	Enqueued	Triggered by recurring job scheduler	2026-06-12 02:00:09.34942-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T02:00:09.3488143Z"}	0
359	120	Processing	\N	2026-06-12 02:30:01.319188-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "880ad0d1-0896-4193-a19b-092c00624057", "StartedAt": "2026-06-12T02:30:01.3105431Z"}	0
361	121	Enqueued	Triggered by recurring job scheduler	2026-06-12 02:40:03.42076-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T02:40:03.4205390Z"}	0
363	121	Succeeded	\N	2026-06-12 02:40:03.453109-04	{"Latency": "48", "SucceededAt": "2026-06-12T02:40:03.4380137Z", "PerformanceDuration": "3"}	0
365	122	Processing	\N	2026-06-12 02:50:05.839184-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "28cbb8d0-e357-4d2b-96a8-03695ca75648", "StartedAt": "2026-06-12T02:50:05.8355729Z"}	0
366	122	Succeeded	\N	2026-06-12 02:50:05.855225-04	{"Latency": "21", "SucceededAt": "2026-06-12T02:50:05.8503820Z", "PerformanceDuration": "8"}	0
385	129	Enqueued	Triggered by recurring job scheduler	2026-06-12 04:00:05.296993-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T04:00:05.2967822Z"}	0
386	129	Processing	\N	2026-06-12 04:00:05.328848-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "28cbb8d0-e357-4d2b-96a8-03695ca75648", "StartedAt": "2026-06-12T04:00:05.3243287Z"}	0
387	129	Succeeded	\N	2026-06-12 04:00:05.34565-04	{"Latency": "42", "SucceededAt": "2026-06-12T04:00:05.3383536Z", "PerformanceDuration": "7"}	0
406	136	Enqueued	Triggered by recurring job scheduler	2026-06-12 14:54:33.022585-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T14:54:32.9878633Z"}	0
412	138	Enqueued	Triggered by recurring job scheduler	2026-06-12 15:50:06.531868-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T15:50:06.5312412Z"}	0
320	107	Processing	\N	2026-06-12 00:00:15.236501-04	{"ServerId": "x:7152:49468b21-b203-4ba3-adaf-63e5b009efcd", "WorkerId": "17805fde-ebb3-41f3-8ddf-e9b4420cdf99", "StartedAt": "2026-06-12T00:00:15.2241447Z"}	0
329	110	Processing	\N	2026-06-12 00:30:05.996433-04	{"ServerId": "x:28772:7ad13435-85c7-4ec2-90bc-140c8ff0caa3", "WorkerId": "d6977b51-eedc-492a-95aa-3411cdf20b84", "StartedAt": "2026-06-12T00:30:05.9561040Z"}	0
335	112	Processing	\N	2026-06-12 00:50:08.972762-04	{"ServerId": "x:6856:4ebcee2b-800a-4c8b-8a3f-0a0e183acf84", "WorkerId": "f91e5bfe-1cb3-4aca-ad41-88f0a818b513", "StartedAt": "2026-06-12T00:50:08.9307288Z"}	0
343	115	Enqueued	Triggered by recurring job scheduler	2026-06-12 01:49:36.468623-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T01:49:36.4476093Z"}	0
348	116	Succeeded	\N	2026-06-12 01:50:06.819942-04	{"Latency": "47", "SucceededAt": "2026-06-12T01:50:06.8125408Z", "PerformanceDuration": "26"}	0
350	117	Processing	\N	2026-06-12 02:00:09.372439-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "86b80cc4-3c43-432e-9102-0c141a0b0e7a", "StartedAt": "2026-06-12T02:00:09.3629997Z"}	0
353	118	Processing	\N	2026-06-12 02:10:11.426504-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "9fa3572e-2a93-4100-a41e-8070788d665f", "StartedAt": "2026-06-12T02:10:11.4224170Z"}	0
367	123	Enqueued	Triggered by recurring job scheduler	2026-06-12 03:00:08.307649-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T03:00:08.3070374Z"}	0
369	123	Succeeded	\N	2026-06-12 03:00:08.33699-04	{"Latency": "55", "SucceededAt": "2026-06-12T03:00:08.3325337Z", "PerformanceDuration": "6"}	0
373	125	Enqueued	Triggered by recurring job scheduler	2026-06-12 03:20:13.306281-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T03:20:13.3059358Z"}	0
374	125	Processing	\N	2026-06-12 03:20:13.320406-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "9fa3572e-2a93-4100-a41e-8070788d665f", "StartedAt": "2026-06-12T03:20:13.3162870Z"}	0
375	125	Succeeded	\N	2026-06-12 03:20:13.342857-04	{"Latency": "31", "SucceededAt": "2026-06-12T03:20:13.3377408Z", "PerformanceDuration": "12"}	0
379	127	Enqueued	Triggered by recurring job scheduler	2026-06-12 03:40:02.94428-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T03:40:02.9440827Z"}	0
380	127	Processing	\N	2026-06-12 03:40:02.957764-04	{"ServerId": "x:14740:3b7026c2-5033-4ef1-b6d4-010a6bad9ae5", "WorkerId": "880ad0d1-0896-4193-a19b-092c00624057", "StartedAt": "2026-06-12T03:40:02.9537270Z"}	0
381	127	Succeeded	\N	2026-06-12 03:40:02.976471-04	{"Latency": "22", "SucceededAt": "2026-06-12T03:40:02.9714694Z", "PerformanceDuration": "9"}	0
388	130	Enqueued	Triggered by recurring job scheduler	2026-06-12 04:10:05.86364-04	{"Queue": "default", "EnqueuedAt": "2026-06-12T04:10:05.8634610Z"}	0
407	136	Processing	\N	2026-06-12 14:54:33.200842-04	{"ServerId": "x:27084:a7a69097-9eaf-49b8-aabb-7cae67f57dbd", "WorkerId": "394299af-ff3b-4c3a-9299-d43857193a50", "StartedAt": "2026-06-12T14:54:33.1574864Z"}	0
414	138	Succeeded	\N	2026-06-12 15:50:06.613844-04	{"Latency": "45", "SucceededAt": "2026-06-12T15:50:06.5935609Z", "PerformanceDuration": "32"}	0
\.


--
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
49	12890061	Fernando	Crear	Prestamo	248	\N	2026-06-06 02:44:17.292338-04
50	12890061	Fernando	Aprobar	Prestamo	248	\N	2026-06-06 02:44:41.926533-04
51	12890061	Fernando	Recoger	Prestamo	248	\N	2026-06-06 02:44:50.483044-04
52	12890061	Fernando	Devolver	Prestamo	248	{"observacion":"Se arruino","equipos":[{"codigo":240000040,"nombre":"Adaptador verde 250V - 20A","estado":"parcialmente_operativo"}]}	2026-06-06 02:45:04.052446-04
53	12890061	Fernando	Editar	GrupoEquipo	22	\N	2026-06-06 02:50:15.465299-04
54	12890061	Fernando	Crear	Prestamo	249	\N	2026-06-06 02:52:05.244368-04
55	sistema		Rechazar	PrestamoEntity	249	Auto-rechazado por exceder fecha de inicio	2026-06-06 03:20:40.552589-04
56	12890061	Fernando	Crear	Prestamo	250	\N	2026-06-07 03:10:18.695507-04
57	12890061	Fernando	Aprobar	Prestamo	250	\N	2026-06-07 03:11:06.98523-04
58	12890061	Fernando	Recoger	Prestamo	250	\N	2026-06-07 03:11:10.747903-04
59	12890061	Fernando	Devolver	Prestamo	250	{"observacion":"Todo bien","equipos":[{"codigo":290000006,"nombre":"Cargador Litio\\u2011Ion 7.2V \\u2011 12V max","estado":"operativo"}]}	2026-06-07 03:11:18.203253-04
60	sistema		Crear	Usuario	99988877	\N	2026-06-12 02:00:04.020858-04
\.


--
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
-- Data for Name: contratos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.contratos (id, contrato) FROM stdin;
\.


--
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
-- Data for Name: detalles_prestamos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.detalles_prestamos (id_detalle_prestamo, id_equipo, id_prestamo, estado_eliminado, id_grupo_equipo, estado_equipo_retorno) FROM stdin;
\.


--
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
169	59	240000040	Adaptador verde 250V - 20A, diseñado para conexiones eléctricas seguras en aplicaciones de alta potencia. Robusto y fácil de instalar.	parcialmente_operativo	0000	Mueble Ventana - Part. Superior	0	9999	\N	9	f	2025-07-14	0000
\.


--
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
-- Data for Name: grupos_equipos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.grupos_equipos (id_grupo_equipo, nombre, modelo, url_data_sheet, cantidad, marca, id_categoria, estado_eliminado, url_imagen, descripcion, costo_promedio) FROM stdin;
17	string	string	string	0	string	5	t	string	string	0.00
18	prueba	TC-2022	http:prueba	0	DELL	3	t	http:prueba	LOREM	0.00
19	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	https://aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaasssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaa	0	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	1	t	https://dsdasddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd	aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa	0.00
5	prueba	prueba	https::prueba	0	prueba	4	t	https://th.bing.com/th/id/OIP.u6bg7Q6XQdd5ZCfumbYt9AHaD4?cb=iwp1&rs=1&pid=ImgDetMain	prueba	0.00
3	Fuente de alimentación DC	   prueba		0	   prueba	3	t	https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR9iF-fVeQ04UmJkJrtRgqMR5PZ8g_BqFwDwg&s	Aqui entra un texto descriptivo del equipo	0.00
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
59	Adaptador verde 250V - 20A	Default	\N	2	Default	24	f	https://m.media-amazon.com/images/I/61-u5zY-BcL._UF894,1000_QL80_.jpg	Adaptador verde 250V - 20A, diseñado para conexiones eléctricas seguras en aplicaciones de alta potencia. Robusto y fácil de instalar.	0.00
22	Maletín con cajones	 Default	https://www.makita.es/product/194884-7.html	0	 Default	24	f	https://fi.makitamedia.com/images/3_Makita/304_accessories_GS1/30410_JPG_zoom/194884-7_C1C0.jpg	Maletín con cajones, funcional y práctico para organizar herramientas. Incluye brocas, aunque el juego está incompleto.	\N
\.


--
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
-- Data for Name: prestamos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.prestamos (id_prestamo, fecha_solicitud, fecha_prestamo, fecha_devolucion_esperada, observacion, estado_prestamo, carnet, estado_eliminado, fecha_devolucion, fecha_prestamo_esperada, id_contrato) FROM stdin;
\.


--
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.usuarios (carnet, nombre, apellido_paterno, apellido_materno, rol, contrasena, email, telefono, telefono_referencia, nombre_referencia, email_referencia, estado_eliminado, id_carrera, imagen_frente_carnet, imagen_atras_carnet, refresh_token, refresh_token_expiry) FROM stdin;
12890061	Fernando	Terrazas	Llanos	administrador	$2a$10$/8JV2T7ZgDGesA4Bd8J1Ne7YprDGYSOIS3vdcXZ9TBf2B4aifVe0G	fernando.terrazas@ucb.edu.bo	799430792	\N	\N	\N	f	2	\N	\N	Nr6GtiKGhYD3plxAAO/53wnGJUir3zwN/EVSQtG3M87pag9RV0lB/iNi0UxGqH6t3QYXj0aX2rNDtCLVcBKmrw==	2026-06-19 15:50:03.630424-04
\.


--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.aggregatedcounter_id_seq', 381, true);


--
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 419, true);


--
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 9, true);


--
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 139, true);


--
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 556, true);


--
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 139, true);


--
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 140, true);


--
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 417, true);


--
-- Name: Accesorio_Id_Accesorio_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Accesorio_Id_Accesorio_seq"', 20, true);


--
-- Name: Categoria_ID_Categoria_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Categoria_ID_Categoria_seq"', 34, true);


--
-- Name: Componente_Id_Componente_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Componente_Id_Componente_seq"', 16, true);


--
-- Name: Empresa_Mantenimiento_Id_Empresa_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Empresa_Mantenimiento_Id_Empresa_seq"', 14, true);


--
-- Name: Equipo_Id_equipo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Equipo_Id_equipo_seq"', 660, true);


--
-- Name: Gavetero_Id_Gavetero_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Gavetero_Id_Gavetero_seq"', 40, true);


--
-- Name: Grupo_Equipo_Id_Grupo_equipo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Grupo_Equipo_Id_Grupo_equipo_seq"', 198, true);


--
-- Name: Mantenimiento_Id_Mantenimiento_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Mantenimiento_Id_Mantenimiento_seq"', 19, true);


--
-- Name: Mueble_Id_Mueble_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Mueble_Id_Mueble_seq"', 16, true);


--
-- Name: Prestamo_Id_Prestamo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Prestamo_Id_Prestamo_seq"', 250, true);


--
-- Name: audit_logs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.audit_logs_id_seq', 60, true);


--
-- Name: carrera_id_carrera_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.carrera_id_carrera_seq', 24, true);


--
-- Name: carreras_id_carrera_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.carreras_id_carrera_seq', 48, true);


--
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.detalles_mantenimientos_id_detalle_mantenimiento_seq', 1, false);


--
-- Name: detalles_mantenimientos_id_detalle_mantenimiento_seq1; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.detalles_mantenimientos_id_detalle_mantenimiento_seq1', 12, true);


--
-- Name: detalles_prestamos_id_detalle_prestamo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.detalles_prestamos_id_detalle_prestamo_seq', 206, true);


--
-- Name: nombre_de_tu_tabla_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.nombre_de_tu_tabla_id_seq', 9, true);


--
-- Name: aggregatedcounter aggregatedcounter_key_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_key_key UNIQUE (key);


--
-- Name: aggregatedcounter aggregatedcounter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_pkey PRIMARY KEY (id);


--
-- Name: counter counter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);


--
-- Name: hash hash_key_field_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);


--
-- Name: hash hash_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);


--
-- Name: job job_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- Name: jobparameter jobparameter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);


--
-- Name: jobqueue jobqueue_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);


--
-- Name: list list_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);


--
-- Name: lock lock_resource_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);

ALTER TABLE ONLY hangfire.lock REPLICA IDENTITY USING INDEX lock_resource_key;


--
-- Name: schema schema_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);


--
-- Name: server server_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);


--
-- Name: set set_key_value_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);


--
-- Name: set set_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);


--
-- Name: state state_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);


--
-- Name: accesorios Accesorio_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accesorios
    ADD CONSTRAINT "Accesorio_pk" PRIMARY KEY (id_accesorio);


--
-- Name: categorias Categoria_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT "Categoria_pk" PRIMARY KEY (id_categoria);


--
-- Name: componentes Componente_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.componentes
    ADD CONSTRAINT "Componente_pk" PRIMARY KEY (id_componente);


--
-- Name: empresas_mantenimiento Empresa_Mantenimiento_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.empresas_mantenimiento
    ADD CONSTRAINT "Empresa_Mantenimiento_pk" PRIMARY KEY (id_empresa_mantenimiento);


--
-- Name: equipos Equipo_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT "Equipo_pk" PRIMARY KEY (id_equipo);


--
-- Name: gaveteros Gavetero_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gaveteros
    ADD CONSTRAINT "Gavetero_pk" PRIMARY KEY (id_gavetero);


--
-- Name: grupos_equipos Grupo_Equipo_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grupos_equipos
    ADD CONSTRAINT "Grupo_Equipo_pk" PRIMARY KEY (id_grupo_equipo);


--
-- Name: mantenimientos Mantenimiento_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mantenimientos
    ADD CONSTRAINT "Mantenimiento_pk" PRIMARY KEY (id_mantenimiento);


--
-- Name: muebles Mueble_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.muebles
    ADD CONSTRAINT "Mueble_pk" PRIMARY KEY (id_mueble);


--
-- Name: prestamos Prestamo_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.prestamos
    ADD CONSTRAINT "Prestamo_pk" PRIMARY KEY (id_prestamo);


--
-- Name: usuarios Usuario_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT "Usuario_pk" PRIMARY KEY (carnet);


--
-- Name: audit_logs audit_logs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.audit_logs
    ADD CONSTRAINT audit_logs_pkey PRIMARY KEY (id);


--
-- Name: carreras carrera_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.carreras
    ADD CONSTRAINT carrera_pkey PRIMARY KEY (id_carrera);


--
-- Name: contratos contrato_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.contratos
    ADD CONSTRAINT contrato_id PRIMARY KEY (id);


--
-- Name: detalles_mantenimientos detalles_mantenimientos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_mantenimientos
    ADD CONSTRAINT detalles_mantenimientos_pkey PRIMARY KEY (id_detalle_mantenimiento);


--
-- Name: detalles_prestamos detalles_prestamos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT detalles_prestamos_pkey PRIMARY KEY (id_detalle_prestamo);


--
-- Name: usuarios unique_carnet; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT unique_carnet UNIQUE (carnet);


--
-- Name: carreras unique_carreras; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.carreras
    ADD CONSTRAINT unique_carreras UNIQUE (nombre);


--
-- Name: categorias unique_categorias; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT unique_categorias UNIQUE (nombre);


--
-- Name: equipos unique_codigo_imt; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT unique_codigo_imt UNIQUE (codigo_imt);


--
-- Name: usuarios unique_email; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT unique_email UNIQUE (email);


--
-- Name: grupos_equipos unique_grupos_equipos_nombre_modelo_marca; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grupos_equipos
    ADD CONSTRAINT unique_grupos_equipos_nombre_modelo_marca UNIQUE (nombre, modelo, marca);


--
-- Name: muebles unique_nombre; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.muebles
    ADD CONSTRAINT unique_nombre UNIQUE (nombre);


--
-- Name: empresas_mantenimiento unique_nombre_empresas_mantenimiento; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.empresas_mantenimiento
    ADD CONSTRAINT unique_nombre_empresas_mantenimiento UNIQUE (nombre);


--
-- Name: gaveteros unique_nombre_gaveteros; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gaveteros
    ADD CONSTRAINT unique_nombre_gaveteros UNIQUE (nombre);


--
-- Name: ix_hangfire_counter_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_counter_expireat ON hangfire.counter USING btree (expireat);


--
-- Name: ix_hangfire_counter_key; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_counter_key ON hangfire.counter USING btree (key);


--
-- Name: ix_hangfire_hash_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_hash_expireat ON hangfire.hash USING btree (expireat);


--
-- Name: ix_hangfire_job_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_expireat ON hangfire.job USING btree (expireat);


--
-- Name: ix_hangfire_job_statename; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_statename ON hangfire.job USING btree (statename);


--
-- Name: ix_hangfire_job_statename_is_not_null; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_statename_is_not_null ON hangfire.job USING btree (statename) INCLUDE (id) WHERE (statename IS NOT NULL);


--
-- Name: ix_hangfire_jobparameter_jobidandname; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON hangfire.jobparameter USING btree (jobid, name);


--
-- Name: ix_hangfire_jobqueue_fetchedat_queue_jobid; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_fetchedat_queue_jobid ON hangfire.jobqueue USING btree (fetchedat NULLS FIRST, queue, jobid);


--
-- Name: ix_hangfire_jobqueue_jobidandqueue; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON hangfire.jobqueue USING btree (jobid, queue);


--
-- Name: ix_hangfire_jobqueue_queueandfetchedat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON hangfire.jobqueue USING btree (queue, fetchedat);


--
-- Name: ix_hangfire_list_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_list_expireat ON hangfire.list USING btree (expireat);


--
-- Name: ix_hangfire_set_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_set_expireat ON hangfire.set USING btree (expireat);


--
-- Name: ix_hangfire_set_key_score; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_set_key_score ON hangfire.set USING btree (key, score);


--
-- Name: ix_hangfire_state_jobid; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_state_jobid ON hangfire.state USING btree (jobid);


--
-- Name: idx_accesorios_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_accesorios_identificadores ON public.accesorios USING btree (nombre, id_equipo, estado_eliminado);


--
-- Name: idx_audit_admin; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_audit_admin ON public.audit_logs USING btree (admin_carnet, "timestamp" DESC);


--
-- Name: idx_audit_entidad; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_audit_entidad ON public.audit_logs USING btree (entidad, entidad_id);


--
-- Name: idx_carreras_nombre; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_carreras_nombre ON public.carreras USING btree (nombre, estado_eliminado);


--
-- Name: idx_categorias_nombre; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_categorias_nombre ON public.categorias USING btree (nombre, estado_eliminado);


--
-- Name: idx_componentes; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_componentes ON public.componentes USING btree (nombre, id_equipo, estado_eliminado);


--
-- Name: idx_detalles_mantenimientos; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_detalles_mantenimientos ON public.detalles_mantenimientos USING btree (id_mantenimiento, estado_eliminado);


--
-- Name: idx_detalles_prestamos; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_detalles_prestamos ON public.detalles_prestamos USING btree (id_prestamo, estado_eliminado);


--
-- Name: idx_empresas_mantenimiento; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_empresas_mantenimiento ON public.empresas_mantenimiento USING btree (nombre, estado_eliminado);


--
-- Name: idx_equipos_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_equipos_identificadores ON public.equipos USING btree (id_grupo_equipo, codigo_imt, estado_eliminado);


--
-- Name: idx_gaveteros_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_gaveteros_identificadores ON public.gaveteros USING btree (nombre, id_mueble, estado_eliminado);


--
-- Name: idx_grupos_equipos_identificadores; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_grupos_equipos_identificadores ON public.grupos_equipos USING btree (id_categoria, nombre, modelo, marca, estado_eliminado);


--
-- Name: idx_mantenimientos_fecha_empresa; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_mantenimientos_fecha_empresa ON public.mantenimientos USING btree (fecha_mantenimiento, fecha_final_mantenimiento, id_empresa, estado_eliminado);


--
-- Name: idx_muebles_nombre; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_muebles_nombre ON public.usuarios USING btree (nombre, estado_eliminado);


--
-- Name: idx_prestamos_fechas; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_prestamos_fechas ON public.prestamos USING btree (fecha_prestamo_esperada, fecha_devolucion_esperada, carnet, estado_eliminado);


--
-- Name: idx_usuarios_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_usuarios_email ON public.usuarios USING btree (email, estado_eliminado);


--
-- Name: ix_detalles_prestamos_id_equipo; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_detalles_prestamos_id_equipo ON public.detalles_prestamos USING btree (id_equipo);


--
-- Name: ix_prestamos_carnet_estado; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_prestamos_carnet_estado ON public.prestamos USING btree (carnet, estado_prestamo, estado_eliminado);


--
-- Name: ix_usuarios_refresh_token; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_usuarios_refresh_token ON public.usuarios USING btree (refresh_token);


--
-- Name: equipos trg_actualizar_costo_promedio_delete; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_actualizar_costo_promedio_delete AFTER DELETE ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_costo_promedio_grupo();


--
-- Name: equipos trg_actualizar_costo_promedio_insert; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_actualizar_costo_promedio_insert AFTER INSERT ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_costo_promedio_grupo();


--
-- Name: equipos trg_actualizar_costo_promedio_update; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_actualizar_costo_promedio_update AFTER UPDATE OF costo_referencia, estado_eliminado, estado_equipo ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_costo_promedio_grupo();


--
-- Name: equipos trg_equipo_estado_actualiza_grupo; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_equipo_estado_actualiza_grupo AFTER UPDATE OF estado_eliminado ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_cantidad_equipo_por_estado();


--
-- Name: equipos trg_equipos_after_insert; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_equipos_after_insert AFTER INSERT ON public.equipos FOR EACH ROW EXECUTE FUNCTION public.fn_incrementar_cantidad_equipos();


--
-- Name: gaveteros trg_gavetero_movimiento_actualiza_numero_mueble; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_gavetero_movimiento_actualiza_numero_mueble AFTER UPDATE ON public.gaveteros FOR EACH ROW WHEN ((old.id_mueble IS DISTINCT FROM new.id_mueble)) EXECUTE FUNCTION public.fn_actualizar_gavetero_tras_update_mueble();


--
-- Name: gaveteros trg_gaveteros_estado_conteo_mueble; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_gaveteros_estado_conteo_mueble AFTER UPDATE OF estado_eliminado ON public.gaveteros FOR EACH ROW EXECUTE FUNCTION public.fn_actualizar_conteo_gaveteros_por_estado();


--
-- Name: gaveteros trg_incrementar_gaveteros; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_incrementar_gaveteros AFTER INSERT ON public.gaveteros FOR EACH ROW EXECUTE FUNCTION public.fn_incrementar_numero_gaveteros();


--
-- Name: mantenimientos trg_mantenimientos_cascade_estado_a_detalles; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_mantenimientos_cascade_estado_a_detalles AFTER UPDATE OF estado_eliminado ON public.mantenimientos FOR EACH ROW EXECUTE FUNCTION public.fn_estado_eliminado_mantenimiento_a_detalle();


--
-- Name: prestamos trg_prestamos_estado_a_detalles; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_prestamos_estado_a_detalles AFTER UPDATE OF estado_eliminado ON public.prestamos FOR EACH ROW EXECUTE FUNCTION public.fn_estado_eliminado_prestamo_a_detalle();


--
-- Name: equipos trg_update_cantidad_tras_update_equipos; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_update_cantidad_tras_update_equipos AFTER UPDATE ON public.equipos FOR EACH ROW WHEN ((old.id_grupo_equipo IS DISTINCT FROM new.id_grupo_equipo)) EXECUTE FUNCTION public.fn_actualizar_cantidad_tras_update_equipos();


--
-- Name: jobparameter jobparameter_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: state state_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: accesorios Accesorio_Equipo_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accesorios
    ADD CONSTRAINT "Accesorio_Equipo_fk" FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- Name: componentes Componente_Equipo_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.componentes
    ADD CONSTRAINT "Componente_Equipo_fk" FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- Name: equipos Equipo_Gavetero_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT "Equipo_Gavetero_fk" FOREIGN KEY (id_gavetero) REFERENCES public.gaveteros(id_gavetero);


--
-- Name: equipos Equipo_Grupo_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipos
    ADD CONSTRAINT "Equipo_Grupo_fk" FOREIGN KEY (id_grupo_equipo) REFERENCES public.grupos_equipos(id_grupo_equipo);


--
-- Name: grupos_equipos Grupo_Categoria_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grupos_equipos
    ADD CONSTRAINT "Grupo_Categoria_fk" FOREIGN KEY (id_categoria) REFERENCES public.categorias(id_categoria);


--
-- Name: mantenimientos Mantenimiento_Empresa_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mantenimientos
    ADD CONSTRAINT "Mantenimiento_Empresa_fk" FOREIGN KEY (id_empresa) REFERENCES public.empresas_mantenimiento(id_empresa_mantenimiento);


--
-- Name: prestamos Prestamo_Usuario_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.prestamos
    ADD CONSTRAINT "Prestamo_Usuario_fk" FOREIGN KEY (carnet) REFERENCES public.usuarios(carnet);


--
-- Name: prestamos Prestamo_contrato_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.prestamos
    ADD CONSTRAINT "Prestamo_contrato_fk" FOREIGN KEY (id_contrato) REFERENCES public.contratos(id) NOT VALID;


--
-- Name: detalles_prestamos detalles_prestamos_id_grupo_equipo_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT detalles_prestamos_id_grupo_equipo_fkey FOREIGN KEY (id_grupo_equipo) REFERENCES public.grupos_equipos(id_grupo_equipo);


--
-- Name: detalles_mantenimientos fk_detalle_mantenimiento_equipo; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_mantenimientos
    ADD CONSTRAINT fk_detalle_mantenimiento_equipo FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- Name: detalles_mantenimientos fk_detalles_mantenimiento; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_mantenimientos
    ADD CONSTRAINT fk_detalles_mantenimiento FOREIGN KEY (id_mantenimiento) REFERENCES public.mantenimientos(id_mantenimiento);


--
-- Name: detalles_prestamos fk_equipo; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT fk_equipo FOREIGN KEY (id_equipo) REFERENCES public.equipos(id_equipo);


--
-- Name: gaveteros fk_gaveteros_muebles; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gaveteros
    ADD CONSTRAINT fk_gaveteros_muebles FOREIGN KEY (id_mueble) REFERENCES public.muebles(id_mueble);


--
-- Name: detalles_prestamos fk_prestamo; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detalles_prestamos
    ADD CONSTRAINT fk_prestamo FOREIGN KEY (id_prestamo) REFERENCES public.prestamos(id_prestamo);


--
-- Name: usuarios fk_usuarios_carrera; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_carrera FOREIGN KEY (id_carrera) REFERENCES public.carreras(id_carrera);


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

