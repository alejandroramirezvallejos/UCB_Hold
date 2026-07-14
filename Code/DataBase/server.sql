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
    id_contrato integer,
    recordatorio_enviado boolean DEFAULT false NOT NULL
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
    "timestamp" timestamp with time zone DEFAULT now() NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
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
-- Name: avisos_disponibilidad; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.avisos_disponibilidad (
    id_aviso integer NOT NULL,
    carnet_usuario character varying(20) NOT NULL,
    id_grupo_equipo integer NOT NULL,
    fecha date NOT NULL,
    cantidad integer DEFAULT 1 NOT NULL,
    notificado boolean DEFAULT false NOT NULL,
    fecha_creacion timestamp with time zone DEFAULT now() NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.avisos_disponibilidad OWNER TO postgres;

--
-- Name: avisos_disponibilidad_id_aviso_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.avisos_disponibilidad ALTER COLUMN id_aviso ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.avisos_disponibilidad_id_aviso_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


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
-- Name: notificaciones; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.notificaciones (
    id_notificacion integer NOT NULL,
    carnet_usuario character varying(20) NOT NULL,
    tipo character varying(50) NOT NULL,
    titulo text NOT NULL,
    contenido text,
    detalle text,
    leido boolean DEFAULT false NOT NULL,
    fecha_envio timestamp with time zone DEFAULT now() NOT NULL,
    estado_eliminado boolean DEFAULT false NOT NULL
);


ALTER TABLE public.notificaciones OWNER TO postgres;

--
-- Name: notificaciones_id_notificacion_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.notificaciones ALTER COLUMN id_notificacion ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.notificaciones_id_notificacion_seq
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
    refresh_token_expiry timestamp with time zone,
    bloqueado boolean DEFAULT false NOT NULL,
    motivo_bloqueo text
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
556	stats:succeeded:2026-07-02-15	5	2026-07-03 11:50:05.010268-04
277	stats:succeeded:2026-06-07	5	2026-07-06 23:40:07.965354-04
294	stats:succeeded:2026-06-11	1	2026-07-11 19:56:32.601991-04
189	stats:succeeded:2026-06-05	22	2026-07-05 19:57:25.730095-04
398	stats:succeeded:2026-06-13	7	2026-07-12 21:40:13.066799-04
7	stats:succeeded:2026-06-03	9	2026-07-03 19:02:15.151647-04
572	stats:succeeded:2026-07-02-16	4	2026-07-03 12:30:07.822425-04
584	stats:succeeded:2026-07-02-17	1	2026-07-03 13:51:31.046399-04
587	stats:succeeded:2026-07-02-18	4	2026-07-03 14:58:18.197062-04
503	stats:succeeded:2026-07-01	17	2026-08-01 14:50:12.654047-04
3	stats:succeeded	222	\N
546	stats:succeeded:2026-07-02-04	1	2026-07-03 00:16:49.808061-04
243	stats:succeeded:2026-06-06	11	2026-07-05 23:46:25.802054-04
594	stats:succeeded:2026-07-02-19	3	2026-07-03 15:20:11.392289-04
547	stats:succeeded:2026-07-02	24	2026-08-02 15:20:10.392289-04
551	stats:succeeded:2026-07-02-05	5	2026-07-03 01:40:11.653728-04
419	stats:succeeded:2026-06-14	29	2026-07-14 14:20:00.385235-04
557	stats:succeeded:2026-07-02-06	1	2026-07-03 02:54:37.180028-04
291	stats:succeeded:2026-06-12	38	2026-07-12 19:58:49.950992-04
24	stats:succeeded:2026-06-04	56	2026-07-04 19:30:05.605239-04
\.


--
-- Data for Name: counter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.counter (id, key, value, expireat) FROM stdin;
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
8	recurring-job:estado-prestamo	LastExecution	2026-07-02T19:20:11.3045214Z	\N	0
6	recurring-job:estado-prestamo	NextExecution	2026-07-02T19:30:00.0000000Z	\N	0
9	recurring-job:estado-prestamo	LastJobId	221	\N	0
\.


--
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
202	606	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 05:38:55.419643-04	2026-07-03 01:39:04.62077-04	0
212	636	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 16:20:05.10734-04	2026-07-03 12:20:05.237954-04	0
203	609	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 05:40:11.050632-04	2026-07-03 01:40:11.653728-04	0
214	642	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 17:51:26.744468-04	2026-07-03 13:51:31.046399-04	0
204	612	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 06:54:37.089134-04	2026-07-03 02:54:37.180028-04	0
216	648	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 18:20:13.440868-04	2026-07-03 14:20:13.525004-04	0
205	615	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 15:11:56.058119-04	2026-07-03 11:11:56.431335-04	0
218	654	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 18:58:09.100019-04	2026-07-03 14:58:18.197062-04	0
206	618	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 15:20:12.795867-04	2026-07-03 11:20:12.919476-04	0
220	660	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 19:10:10.441047-04	2026-07-03 15:10:10.519907-04	0
207	621	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 15:30:14.876326-04	2026-07-03 11:30:14.997825-04	0
198	594	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 04:16:43.66472-04	2026-07-03 00:16:49.808061-04	0
208	624	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 15:40:02.524986-04	2026-07-03 11:40:02.559152-04	0
199	597	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 05:08:38.958652-04	2026-07-03 01:08:48.664866-04	0
209	627	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 15:50:04.940201-04	2026-07-03 11:50:05.010268-04	0
200	600	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 05:10:09.829402-04	2026-07-03 01:10:10.08869-04	0
201	603	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 05:20:12.506398-04	2026-07-03 01:20:13.060574-04	0
211	633	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 16:10:02.436412-04	2026-07-03 12:10:08.891821-04	0
210	630	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 16:00:07.03528-04	2026-07-03 12:00:07.104895-04	0
213	639	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 16:30:07.688386-04	2026-07-03 12:30:07.822425-04	0
215	645	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 18:18:58.189398-04	2026-07-03 14:19:01.054274-04	0
217	651	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 18:43:59.708007-04	2026-07-03 14:44:10.376817-04	0
219	657	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 19:00:09.469312-04	2026-07-03 15:00:09.657226-04	0
221	663	Succeeded	{"Type": "IMT_Reservas.Server.Infrastructure.Jobs.EstadoPrestamoJob, IMT_Reservas.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Method": "Execute", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-07-02 19:20:11.322085-04	2026-07-03 15:20:11.392289-04	0
\.


--
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
873	219	RecurringJobId	"estado-prestamo"	0
874	219	Time	1783018809	0
875	219	CurrentCulture	"es-MX"	0
876	219	CurrentUICulture	"es-MX"	0
877	220	RecurringJobId	"estado-prestamo"	0
878	220	Time	1783019410	0
879	220	CurrentCulture	"es-MX"	0
880	220	CurrentUICulture	"es-MX"	0
881	221	RecurringJobId	"estado-prestamo"	0
882	221	Time	1783020011	0
883	221	CurrentCulture	"es-MX"	0
884	221	CurrentUICulture	"es-MX"	0
789	198	RecurringJobId	"estado-prestamo"	0
790	198	Time	1782965803	0
791	198	CurrentCulture	"es-MX"	0
792	198	CurrentUICulture	"es-MX"	0
793	199	RecurringJobId	"estado-prestamo"	0
794	199	Time	1782968918	0
795	199	CurrentCulture	"es-MX"	0
796	199	CurrentUICulture	"es-MX"	0
797	200	RecurringJobId	"estado-prestamo"	0
798	200	Time	1782969009	0
799	200	CurrentCulture	"es-MX"	0
800	200	CurrentUICulture	"es-MX"	0
801	201	RecurringJobId	"estado-prestamo"	0
802	201	Time	1782969612	0
803	201	CurrentCulture	"es-MX"	0
804	201	CurrentUICulture	"es-MX"	0
805	202	RecurringJobId	"estado-prestamo"	0
806	202	Time	1782970735	0
807	202	CurrentCulture	"es-MX"	0
808	202	CurrentUICulture	"es-MX"	0
809	203	RecurringJobId	"estado-prestamo"	0
810	203	Time	1782970811	0
811	203	CurrentCulture	"es-MX"	0
812	203	CurrentUICulture	"es-MX"	0
813	204	RecurringJobId	"estado-prestamo"	0
814	204	Time	1782975277	0
815	204	CurrentCulture	"es-MX"	0
816	204	CurrentUICulture	"es-MX"	0
817	205	RecurringJobId	"estado-prestamo"	0
818	205	Time	1783005116	0
819	205	CurrentCulture	"es-MX"	0
820	205	CurrentUICulture	"es-MX"	0
821	206	RecurringJobId	"estado-prestamo"	0
822	206	Time	1783005612	0
823	206	CurrentCulture	"es-MX"	0
824	206	CurrentUICulture	"es-MX"	0
825	207	RecurringJobId	"estado-prestamo"	0
826	207	Time	1783006214	0
827	207	CurrentCulture	"es-MX"	0
828	207	CurrentUICulture	"es-MX"	0
829	208	RecurringJobId	"estado-prestamo"	0
830	208	Time	1783006802	0
831	208	CurrentCulture	"es-MX"	0
832	208	CurrentUICulture	"es-MX"	0
833	209	RecurringJobId	"estado-prestamo"	0
834	209	Time	1783007404	0
835	209	CurrentCulture	"es-MX"	0
836	209	CurrentUICulture	"es-MX"	0
837	210	RecurringJobId	"estado-prestamo"	0
838	210	Time	1783008007	0
839	210	CurrentCulture	"es-MX"	0
840	210	CurrentUICulture	"es-MX"	0
841	211	RecurringJobId	"estado-prestamo"	0
842	211	Time	1783008602	0
843	211	CurrentCulture	"es-MX"	0
844	211	CurrentUICulture	"es-MX"	0
845	212	RecurringJobId	"estado-prestamo"	0
846	212	Time	1783009205	0
847	212	CurrentCulture	"es-MX"	0
848	212	CurrentUICulture	"es-MX"	0
849	213	RecurringJobId	"estado-prestamo"	0
850	213	Time	1783009807	0
851	213	CurrentCulture	"es-MX"	0
852	213	CurrentUICulture	"es-MX"	0
853	214	RecurringJobId	"estado-prestamo"	0
854	214	Time	1783014686	0
855	214	CurrentCulture	"es-MX"	0
856	214	CurrentUICulture	"es-MX"	0
857	215	RecurringJobId	"estado-prestamo"	0
858	215	Time	1783016337	0
859	215	CurrentCulture	"es-MX"	0
860	215	CurrentUICulture	"es-MX"	0
861	216	RecurringJobId	"estado-prestamo"	0
862	216	Time	1783016413	0
863	216	CurrentCulture	"es-MX"	0
864	216	CurrentUICulture	"es-MX"	0
865	217	RecurringJobId	"estado-prestamo"	0
866	217	Time	1783017837	0
867	217	CurrentCulture	"es-MX"	0
868	217	CurrentUICulture	"es-MX"	0
869	218	RecurringJobId	"estado-prestamo"	0
870	218	Time	1783018688	0
871	218	CurrentCulture	"es-MX"	0
872	218	CurrentUICulture	"es-MX"	0
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
1	recurring-jobs	1783020600	estado-prestamo	\N	0
\.


--
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
594	198	Succeeded	\N	2026-07-02 04:16:49.826916-04	{"Latency": "281", "SucceededAt": "2026-07-02T04:16:49.7852545Z", "PerformanceDuration": "5838"}	0
597	199	Succeeded	\N	2026-07-02 05:08:48.686724-04	{"Latency": "502", "SucceededAt": "2026-07-02T05:08:48.3828784Z", "PerformanceDuration": "8919"}	0
598	200	Enqueued	Triggered by recurring job scheduler	2026-07-02 05:10:09.839864-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T05:10:09.8392210Z"}	0
601	201	Enqueued	Triggered by recurring job scheduler	2026-07-02 05:20:12.521891-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T05:20:12.5214282Z"}	0
602	201	Processing	\N	2026-07-02 05:20:12.561125-04	{"ServerId": "x:38508:4f69c811-fbe7-4bfe-8173-52ed869a9873", "WorkerId": "d494a03c-112e-4782-b860-8bcbdb2f9c20", "StartedAt": "2026-07-02T05:20:12.5560175Z"}	0
603	201	Succeeded	\N	2026-07-02 05:20:13.063715-04	{"Latency": "59", "SucceededAt": "2026-07-02T05:20:12.9866742Z", "PerformanceDuration": "420"}	0
606	202	Succeeded	\N	2026-07-02 05:39:04.630091-04	{"Latency": "482", "SucceededAt": "2026-07-02T05:39:04.5590405Z", "PerformanceDuration": "8654"}	0
609	203	Succeeded	\N	2026-07-02 05:40:11.658021-04	{"Latency": "80", "SucceededAt": "2026-07-02T05:40:11.5577374Z", "PerformanceDuration": "426"}	0
612	204	Succeeded	\N	2026-07-02 06:54:37.181724-04	{"Latency": "56", "SucceededAt": "2026-07-02T06:54:37.1701911Z", "PerformanceDuration": "24"}	0
613	205	Enqueued	Triggered by recurring job scheduler	2026-07-02 15:11:56.085346-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T15:11:56.0850026Z"}	0
619	207	Enqueued	Triggered by recurring job scheduler	2026-07-02 15:30:14.895845-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T15:30:14.8954557Z"}	0
622	208	Enqueued	Triggered by recurring job scheduler	2026-07-02 15:40:02.530931-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T15:40:02.5306337Z"}	0
624	208	Succeeded	\N	2026-07-02 15:40:02.571487-04	{"Latency": "21", "SucceededAt": "2026-07-02T15:40:02.5552502Z", "PerformanceDuration": "8"}	0
625	209	Enqueued	Triggered by recurring job scheduler	2026-07-02 15:50:04.949341-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T15:50:04.9487524Z"}	0
629	210	Processing	\N	2026-07-02 16:00:07.067573-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "85c5d056-a2ce-4b7e-bc1d-12bfffac1062", "StartedAt": "2026-07-02T16:00:07.0640379Z"}	0
631	211	Enqueued	Triggered by recurring job scheduler	2026-07-02 16:10:02.748462-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T16:10:02.6869668Z"}	0
634	212	Enqueued	Triggered by recurring job scheduler	2026-07-02 16:20:05.180283-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T16:20:05.1795739Z"}	0
635	212	Processing	\N	2026-07-02 16:20:05.205149-04	{"ServerId": "x:35052:74d12a0b-b856-4c43-ac6b-e8f70e6883f1", "WorkerId": "33880127-1b62-47bc-8c40-895d300cc116", "StartedAt": "2026-07-02T16:20:05.1991647Z"}	0
636	212	Succeeded	\N	2026-07-02 16:20:05.240621-04	{"Latency": "103", "SucceededAt": "2026-07-02T16:20:05.2297608Z", "PerformanceDuration": "19"}	0
638	213	Processing	\N	2026-07-02 16:30:07.765745-04	{"ServerId": "x:35052:74d12a0b-b856-4c43-ac6b-e8f70e6883f1", "WorkerId": "270b9d17-94a7-41f9-830d-734a6d6f8f1c", "StartedAt": "2026-07-02T16:30:07.7513938Z"}	0
641	214	Processing	\N	2026-07-02 17:51:27.259052-04	{"ServerId": "x:27004:a29913a3-b9df-4813-a3cc-35c917b2b8d9", "WorkerId": "48861a7b-c9c7-4e02-b389-2530c818ff37", "StartedAt": "2026-07-02T17:51:27.1725549Z"}	0
644	215	Processing	\N	2026-07-02 18:18:58.368052-04	{"ServerId": "x:21600:c8627ef7-39fc-4060-8a70-a35b3d2d08e2", "WorkerId": "7e58ad67-c28f-477d-bc9b-010845fadabe", "StartedAt": "2026-07-02T18:18:58.3384225Z"}	0
647	216	Processing	\N	2026-07-02 18:20:13.475159-04	{"ServerId": "x:21600:c8627ef7-39fc-4060-8a70-a35b3d2d08e2", "WorkerId": "7e58ad67-c28f-477d-bc9b-010845fadabe", "StartedAt": "2026-07-02T18:20:13.4647286Z"}	0
651	217	Succeeded	\N	2026-07-02 18:44:10.398024-04	{"Latency": "635", "SucceededAt": "2026-07-02T18:44:10.0810176Z", "PerformanceDuration": "9729"}	0
654	218	Succeeded	\N	2026-07-02 18:58:18.206757-04	{"Latency": "234", "SucceededAt": "2026-07-02T18:58:18.0620283Z", "PerformanceDuration": "8724"}	0
655	219	Enqueued	Triggered by recurring job scheduler	2026-07-02 19:00:09.537356-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T19:00:09.5367998Z"}	0
658	220	Enqueued	Triggered by recurring job scheduler	2026-07-02 19:10:10.45722-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T19:10:10.4567880Z"}	0
659	220	Processing	\N	2026-07-02 19:10:10.480893-04	{"ServerId": "x:39192:3fba42cd-17ce-4213-9e2c-d6447f87ba77", "WorkerId": "dd9645a9-0966-4c77-8da2-b49bfa47c575", "StartedAt": "2026-07-02T19:10:10.4743809Z"}	0
660	220	Succeeded	\N	2026-07-02 19:10:10.523634-04	{"Latency": "45", "SucceededAt": "2026-07-02T19:10:10.5123260Z", "PerformanceDuration": "25"}	0
592	198	Enqueued	Triggered by recurring job scheduler	2026-07-02 04:16:43.793863-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T04:16:43.7671104Z"}	0
595	199	Enqueued	Triggered by recurring job scheduler	2026-07-02 05:08:39.280366-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T05:08:39.2339611Z"}	0
599	200	Processing	\N	2026-07-02 05:10:09.85913-04	{"ServerId": "x:38508:4f69c811-fbe7-4bfe-8173-52ed869a9873", "WorkerId": "dd15c805-82f2-448d-899b-f4c313ca8ded", "StartedAt": "2026-07-02T05:10:09.8505699Z"}	0
604	202	Enqueued	Triggered by recurring job scheduler	2026-07-02 05:38:55.714734-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T05:38:55.6560123Z"}	0
607	203	Enqueued	Triggered by recurring job scheduler	2026-07-02 05:40:11.07685-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T05:40:11.0757463Z"}	0
610	204	Enqueued	Triggered by recurring job scheduler	2026-07-02 06:54:37.105275-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T06:54:37.1048963Z"}	0
614	205	Processing	\N	2026-07-02 15:11:56.122739-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "bd2387d1-e04f-4471-8780-3acdf6cffca3", "StartedAt": "2026-07-02T15:11:56.1023650Z"}	0
615	205	Succeeded	\N	2026-07-02 15:11:56.434962-04	{"Latency": "69", "SucceededAt": "2026-07-02T15:11:56.3557930Z", "PerformanceDuration": "227"}	0
620	207	Processing	\N	2026-07-02 15:30:14.962652-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "cc37134c-fe44-4bc9-ac8e-3215d2d31615", "StartedAt": "2026-07-02T15:30:14.9513494Z"}	0
621	207	Succeeded	\N	2026-07-02 15:30:15.002018-04	{"Latency": "96", "SucceededAt": "2026-07-02T15:30:14.9882268Z", "PerformanceDuration": "15"}	0
623	208	Processing	\N	2026-07-02 15:40:02.544402-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "e753781d-1ce6-4fcc-aa22-4190c3cff2ec", "StartedAt": "2026-07-02T15:40:02.5413675Z"}	0
626	209	Processing	\N	2026-07-02 15:50:04.967174-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "61082c0f-6921-4b08-bfec-6d102ee66487", "StartedAt": "2026-07-02T15:50:04.9620739Z"}	0
627	209	Succeeded	\N	2026-07-02 15:50:05.011573-04	{"Latency": "29", "SucceededAt": "2026-07-02T15:50:05.0050529Z", "PerformanceDuration": "34"}	0
632	211	Processing	\N	2026-07-02 16:10:02.943296-04	{"ServerId": "x:35052:74d12a0b-b856-4c43-ac6b-e8f70e6883f1", "WorkerId": "33880127-1b62-47bc-8c40-895d300cc116", "StartedAt": "2026-07-02T16:10:02.8980979Z"}	0
637	213	Enqueued	Triggered by recurring job scheduler	2026-07-02 16:30:07.728128-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T16:30:07.7274988Z"}	0
639	213	Succeeded	\N	2026-07-02 16:30:07.825101-04	{"Latency": "85", "SucceededAt": "2026-07-02T16:30:07.8146835Z", "PerformanceDuration": "41"}	0
642	214	Succeeded	\N	2026-07-02 17:51:31.050259-04	{"Latency": "539", "SucceededAt": "2026-07-02T17:51:31.0074068Z", "PerformanceDuration": "3722"}	0
645	215	Succeeded	\N	2026-07-02 18:19:01.059156-04	{"Latency": "189", "SucceededAt": "2026-07-02T18:19:01.0206443Z", "PerformanceDuration": "2641"}	0
649	217	Enqueued	Triggered by recurring job scheduler	2026-07-02 18:44:00.025281-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T18:43:59.9250827Z"}	0
652	218	Enqueued	Triggered by recurring job scheduler	2026-07-02 18:58:09.201842-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T18:58:09.1725331Z"}	0
656	219	Processing	\N	2026-07-02 19:00:09.601886-04	{"ServerId": "x:39192:3fba42cd-17ce-4213-9e2c-d6447f87ba77", "WorkerId": "6c905f06-6041-41a8-8058-7a6550a8e7b3", "StartedAt": "2026-07-02T19:00:09.5827653Z"}	0
661	221	Enqueued	Triggered by recurring job scheduler	2026-07-02 19:20:11.340118-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T19:20:11.3397957Z"}	0
593	198	Processing	\N	2026-07-02 04:16:43.931278-04	{"ServerId": "x:20300:38dc0033-3d41-4f4d-bc47-e41f08dcbdae", "WorkerId": "f2139875-7cf1-43e5-915f-af8483d6ff1b", "StartedAt": "2026-07-02T04:16:43.8916585Z"}	0
596	199	Processing	\N	2026-07-02 05:08:39.441676-04	{"ServerId": "x:38508:4f69c811-fbe7-4bfe-8173-52ed869a9873", "WorkerId": "dd15c805-82f2-448d-899b-f4c313ca8ded", "StartedAt": "2026-07-02T05:08:39.3902125Z"}	0
600	200	Succeeded	\N	2026-07-02 05:10:10.092688-04	{"Latency": "34", "SucceededAt": "2026-07-02T05:10:10.0024494Z", "PerformanceDuration": "138"}	0
605	202	Processing	\N	2026-07-02 05:38:55.883883-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "8ecbc6f2-e833-4855-8c35-bf2eb4a06e70", "StartedAt": "2026-07-02T05:38:55.8232034Z"}	0
608	203	Processing	\N	2026-07-02 05:40:11.121656-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "8ecbc6f2-e833-4855-8c35-bf2eb4a06e70", "StartedAt": "2026-07-02T05:40:11.1144666Z"}	0
611	204	Processing	\N	2026-07-02 06:54:37.137718-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "8ecbc6f2-e833-4855-8c35-bf2eb4a06e70", "StartedAt": "2026-07-02T06:54:37.1228426Z"}	0
616	206	Enqueued	Triggered by recurring job scheduler	2026-07-02 15:20:12.843912-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T15:20:12.8435072Z"}	0
617	206	Processing	\N	2026-07-02 15:20:12.88888-04	{"ServerId": "x:2652:7525cdbe-7f5f-4fb3-9520-732b1615d3df", "WorkerId": "164cf906-3cde-4714-b02f-1632a760c8b3", "StartedAt": "2026-07-02T15:20:12.8856747Z"}	0
618	206	Succeeded	\N	2026-07-02 15:20:12.943258-04	{"Latency": "99", "SucceededAt": "2026-07-02T15:20:12.9107121Z", "PerformanceDuration": "15"}	0
628	210	Enqueued	Triggered by recurring job scheduler	2026-07-02 16:00:07.049299-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T16:00:07.0487946Z"}	0
630	210	Succeeded	\N	2026-07-02 16:00:07.10656-04	{"Latency": "37", "SucceededAt": "2026-07-02T16:00:07.0975773Z", "PerformanceDuration": "24"}	0
633	211	Succeeded	\N	2026-07-02 16:10:08.898359-04	{"Latency": "526", "SucceededAt": "2026-07-02T16:10:08.7699750Z", "PerformanceDuration": "5805"}	0
640	214	Enqueued	Triggered by recurring job scheduler	2026-07-02 17:51:27.026166-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T17:51:26.9313665Z"}	0
643	215	Enqueued	Triggered by recurring job scheduler	2026-07-02 18:18:58.283323-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T18:18:58.2588749Z"}	0
646	216	Enqueued	Triggered by recurring job scheduler	2026-07-02 18:20:13.452216-04	{"Queue": "default", "EnqueuedAt": "2026-07-02T18:20:13.4520402Z"}	0
648	216	Succeeded	\N	2026-07-02 18:20:13.526513-04	{"Latency": "38", "SucceededAt": "2026-07-02T18:20:13.5165278Z", "PerformanceDuration": "37"}	0
650	217	Processing	\N	2026-07-02 18:44:00.317509-04	{"ServerId": "x:36520:da0c8a71-f09a-4245-94bd-c4ffb5a7ce86", "WorkerId": "8022ea5d-f35b-466d-85f9-e258f1eeb05f", "StartedAt": "2026-07-02T18:44:00.2172675Z"}	0
653	218	Processing	\N	2026-07-02 18:58:09.317927-04	{"ServerId": "x:39192:3fba42cd-17ce-4213-9e2c-d6447f87ba77", "WorkerId": "6c905f06-6041-41a8-8058-7a6550a8e7b3", "StartedAt": "2026-07-02T18:58:09.2704619Z"}	0
657	219	Succeeded	\N	2026-07-02 19:00:09.660668-04	{"Latency": "142", "SucceededAt": "2026-07-02T19:00:09.6364448Z", "PerformanceDuration": "24"}	0
662	221	Processing	\N	2026-07-02 19:20:11.364824-04	{"ServerId": "x:39192:3fba42cd-17ce-4213-9e2c-d6447f87ba77", "WorkerId": "8659dc12-a3b4-467c-b0a9-838f32e92c26", "StartedAt": "2026-07-02T19:20:11.3563829Z"}	0
663	221	Succeeded	\N	2026-07-02 19:20:11.394078-04	{"Latency": "48", "SucceededAt": "2026-07-02T19:20:11.3839703Z", "PerformanceDuration": "13"}	0
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

COPY public.audit_logs (id, admin_carnet, admin_nombre, accion, entidad, entidad_id, detalle, "timestamp", estado_eliminado) FROM stdin;
1	sistema		AtrasadoAutomatico	PrestamoEntity	230	Auto-rechazado por exceder fecha de inicio	2026-06-03 05:04:57.968706-04	f
2	sistema		AtrasadoAutomatico	PrestamoEntity	228	Auto-rechazado por exceder fecha de inicio	2026-06-03 05:04:59.092436-04	f
3	sistema		AtrasadoAutomatico	PrestamoEntity	229	Auto-rechazado por exceder fecha de inicio	2026-06-03 05:04:59.11666-04	f
4	12890061	Fernando	Recoger	PrestamoEntity	231	\N	2026-06-03 13:00:39.314541-04	f
5	12890061	Fernando	Devolver	PrestamoEntity	231	\N	2026-06-03 13:00:45.566309-04	f
6	12890061	Fernando	Crear	CarreraEntity	48	\N	2026-06-03 13:03:42.730319-04	f
7	12890061	Fernando	Eliminar	Prestamo	228	\N	2026-06-03 17:37:04.053889-04	f
8	12890061	Fernando	Eliminar	Carrera	48	\N	2026-06-03 17:38:08.003556-04	f
9	12890061	Fernando	Crear	PrestamoEntity	232	\N	2026-06-03 17:38:57.042305-04	f
10	12890061	Fernando	Aprobar	PrestamoEntity	232	\N	2026-06-03 17:39:06.976268-04	f
11	12890061	Fernando	Recoger	PrestamoEntity	232	\N	2026-06-03 17:39:10.612578-04	f
12	12890061	Fernando	Devolver	PrestamoEntity	232	\N	2026-06-03 17:39:18.705133-04	f
13	12890061	Fernando	Crear	Prestamo	233	\N	2026-06-03 21:45:36.983285-04	f
14	12890061	Fernando	Aprobar	Prestamo	233	\N	2026-06-03 21:46:02.761909-04	f
15	12890061	Fernando	Recoger	Prestamo	233	\N	2026-06-03 21:46:19.010525-04	f
16	12890061	Fernando	Devolver	Prestamo	233	\N	2026-06-04 00:11:09.650793-04	f
17	12890061	Fernando	Crear	Prestamo	234	\N	2026-06-04 00:15:32.301454-04	f
18	sistema		Rechazar	PrestamoEntity	234	Auto-rechazado por exceder fecha de inicio	2026-06-04 00:20:15.116688-04	f
19	12890061	Fernando	Crear	Prestamo	235	\N	2026-06-04 02:46:59.304736-04	f
20	sistema		Rechazar	PrestamoEntity	235	Auto-rechazado por exceder fecha de inicio	2026-06-04 02:50:08.212638-04	f
21	12890061	Fernando	Crear	Prestamo	236	\N	2026-06-04 12:00:25.755302-04	f
22	sistema		Rechazar	PrestamoEntity	236	Auto-rechazado por exceder fecha de inicio	2026-06-05 00:46:39.66447-04	f
23	12890061	Fernando	Crear	Prestamo	237	\N	2026-06-05 00:53:53.418886-04	f
24	12890061	Fernando	Editar	Equipo	125	\N	2026-06-05 00:58:28.563629-04	f
25	12890061	Fernando	Crear	Prestamo	238	\N	2026-06-05 00:58:47.950576-04	f
26	sistema		Rechazar	PrestamoEntity	237	Auto-rechazado por exceder fecha de inicio	2026-06-05 01:00:09.609708-04	f
27	sistema		Rechazar	PrestamoEntity	238	Auto-rechazado por exceder fecha de inicio	2026-06-05 01:00:09.62739-04	f
28	12890061	Fernando	Crear	Prestamo	239	\N	2026-06-05 01:01:31.412742-04	f
29	12890061	Fernando	Rechazar	Prestamo	239	\N	2026-06-05 01:01:41.984142-04	f
30	12890061	Fernando	Crear	Prestamo	240	\N	2026-06-05 02:12:03.17475-04	f
31	sistema		Rechazar	PrestamoEntity	240	Auto-rechazado por exceder fecha de inicio	2026-06-05 02:47:56.741735-04	f
32	12890061	Fernando	Crear	Prestamo	241	\N	2026-06-05 02:57:34.024847-04	f
33	12890061	Fernando	Editar	Equipo	131	\N	2026-06-05 03:01:05.48879-04	f
34	12890061	Fernando	Rechazar	Prestamo	241	\N	2026-06-05 17:14:34.372209-04	f
35	12890061	Fernando	Rechazar	Prestamo	242	\N	2026-06-05 17:14:38.436154-04	f
36	12890061	Fernando	Rechazar	Prestamo	243	\N	2026-06-05 17:14:43.640293-04	f
37	12890061	Fernando	Rechazar	Prestamo	244	\N	2026-06-05 23:30:00.415131-04	f
38	12890061	Fernando	Aprobar	Prestamo	245	\N	2026-06-05 23:30:03.865809-04	f
39	12890061	Fernando	Recoger	Prestamo	245	\N	2026-06-05 23:30:09.630514-04	f
40	12890061	Fernando	Devolver	Prestamo	245	\N	2026-06-05 23:31:53.954576-04	f
41	12890061	Fernando	Crear	Prestamo	246	\N	2026-06-06 00:02:54.910616-04	f
42	12890061	Fernando	Aprobar	Prestamo	246	\N	2026-06-06 00:03:05.913046-04	f
43	12890061	Fernando	Recoger	Prestamo	246	\N	2026-06-06 00:03:09.728353-04	f
44	12890061	Fernando	Devolver	Prestamo	246	\N	2026-06-06 00:03:18.454845-04	f
45	12890061	Fernando	Crear	Prestamo	247	\N	2026-06-06 00:07:57.664393-04	f
46	12890061	Fernando	Aprobar	Prestamo	247	\N	2026-06-06 00:08:05.194095-04	f
47	12890061	Fernando	Recoger	Prestamo	247	\N	2026-06-06 00:08:08.817232-04	f
48	12890061	Fernando	Devolver	Prestamo	247	\N	2026-06-06 00:08:15.985716-04	f
49	12890061	Fernando	Crear	Prestamo	248	\N	2026-06-06 02:44:17.292338-04	f
50	12890061	Fernando	Aprobar	Prestamo	248	\N	2026-06-06 02:44:41.926533-04	f
51	12890061	Fernando	Recoger	Prestamo	248	\N	2026-06-06 02:44:50.483044-04	f
52	12890061	Fernando	Devolver	Prestamo	248	{"observacion":"Se arruino","equipos":[{"codigo":240000040,"nombre":"Adaptador verde 250V - 20A","estado":"parcialmente_operativo"}]}	2026-06-06 02:45:04.052446-04	f
53	12890061	Fernando	Editar	GrupoEquipo	22	\N	2026-06-06 02:50:15.465299-04	f
54	12890061	Fernando	Crear	Prestamo	249	\N	2026-06-06 02:52:05.244368-04	f
55	sistema		Rechazar	PrestamoEntity	249	Auto-rechazado por exceder fecha de inicio	2026-06-06 03:20:40.552589-04	f
56	12890061	Fernando	Crear	Prestamo	250	\N	2026-06-07 03:10:18.695507-04	f
57	12890061	Fernando	Aprobar	Prestamo	250	\N	2026-06-07 03:11:06.98523-04	f
58	12890061	Fernando	Recoger	Prestamo	250	\N	2026-06-07 03:11:10.747903-04	f
59	12890061	Fernando	Devolver	Prestamo	250	{"observacion":"Todo bien","equipos":[{"codigo":290000006,"nombre":"Cargador Litio\\u2011Ion 7.2V \\u2011 12V max","estado":"operativo"}]}	2026-06-07 03:11:18.203253-04	f
60	sistema		Crear	Usuario	99988877	\N	2026-06-12 02:00:04.020858-04	f
61	12890061	Fernando	Cancelar	Prestamo	251	\N	2026-06-12 22:22:11.667735-04	f
62	12890061	Fernando	Aprobar	Prestamo	252	\N	2026-06-12 22:25:44.19893-04	f
63	12890061	Fernando	Recoger	Prestamo	252	\N	2026-06-12 22:25:49.082638-04	f
64	12890061	Fernando	Devolver	Prestamo	252	{"observacion":null,"equipos":[{"codigo":240000010,"nombre":"Mini Dron","estado":"operativo"}]}	2026-06-12 22:25:54.555319-04	f
66	12890061	Fernando	Crear	Prestamo	256	\N	2026-06-12 23:38:59.978334-04	f
67	sistema		Rechazar	PrestamoEntity	256	Auto-rechazado por exceder fecha de inicio	2026-06-14 01:04:36.871857-04	f
68	12890061	Fernando	Crear	Prestamo	257	\N	2026-06-14 16:20:24.741262-04	f
69	12890061	Fernando	Crear	Prestamo	258	\N	2026-06-14 16:38:36.582046-04	f
70	12890061	Fernando	Aprobar	Prestamo	258	\N	2026-06-14 16:39:37.488547-04	f
71	12890061	Fernando	Recoger	Prestamo	258	\N	2026-06-14 16:39:41.755296-04	f
72	12890061	Fernando	Aprobar	Prestamo	257	\N	2026-06-14 17:04:53.501214-04	f
73	sistema		AtrasadoAutomatico	PrestamoEntity	258	\N	2026-07-01 04:38:12.134118-04	f
74	sistema		Rechazar	PrestamoEntity	257	Auto-rechazado por exceder fecha de inicio	2026-07-01 04:38:12.831687-04	f
75	12890061	Fernando	Devolver	Prestamo	258	{"observacion":null,"equipos":[{"codigo":240000012,"nombre":"L\\u00E1mpara de Aumento (nueva)","estado":"operativo"}]}	2026-07-01 04:44:46.323334-04	f
\.


--
-- Data for Name: avisos_disponibilidad; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.avisos_disponibilidad (id_aviso, carnet_usuario, id_grupo_equipo, fecha, cantidad, notificado, fecha_creacion, estado_eliminado) FROM stdin;
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
12	<div _ngcontent-ng-c852924939="" class="formulario">\n<div class="contrato-container">\n  <h1>Minuta de PRÉSTAMO DE EQUIPOS DE LABORATORIO</h1>\n  \n  <p>\n    A los 12 días del mes de junio de 2026, entre el Sr. <strong>Job Angel Ledezma Pérez</strong>, Director de Carrera de Ingeniería Mecatrónica de la Universidad Católica Boliviana "San Pablo" (UCB) Regional Santa Cruz, identificado con el C.I. <strong>5268336 CB </strong>y con domicilio en la ciudad de Santa Cruz de la Sierra, quien en adelante se denominará el COMODANTE, y de otra parte el usuario <strong>Fernando </strong>, C.I. <strong>12890061</strong>, quien se domicilia en la ciudad de Santa Cruz de la Sierra, celebran por medio de este instrumento el <strong>COMODATO</strong> sobre los equipos de laboratorio y en las condiciones que a continuación se describen:\n  </p>\n  \n  <strong>Primera. Del objeto.-</strong>\n  <p>\n    El COMODANTE entrega al GRUPO COMODATARIO y este recibe, a título de comodato, equipos de laboratorio pertenecientes a la Carrera de Ingeniería Mecatrónica, localizados en el Laboratorio de Robótica y Automatización y bajo la custodia del COMODANTE. El detalle de los equipos a ser otorgados se describe a continuación:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="28">Por definirse</td>\n          <td class="ucb-code" data-grupo-id="28">Por definirse</td>\n          <td>\n          <strong>Mini Dron</strong>\n          <p>Marca:   Default </p>\n          <p>Modelo:   Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="28">Por definirse</td>\n          <td>1</td> \n        </tr>\n      \n    \n    </tbody>\n  </table>\n  \n  <strong>Segunda. Del uso autorizado.-</strong>\n  <p>\n    El GRUPO COMODATARIO podrá utilizar los bienes descritos en el punto primero, única y exclusivamente para la realización de proyectos relacionados con las disciplinas que requieran de los mismos, quedando expresamente prohibido el retiro de dichos equipos fuera de las instalaciones del Campus de la Universidad Católica Boliviana "San Pablo" - Regional Santa Cruz, ubicado en el Km. 9, Carretera al Norte.\n  </p>\n  \n  <strong>Tercera. De las obligaciones del GRUPO COMODATARIO.-</strong>\n  <p>\n    Son obligaciones del GRUPO COMODATARIO mantener en buen estado los bienes recibidos, cuidar el comodato y responder por todo daño o deterioro, salvo los derivados del uso autorizado; asimismo, deberán responder por los daños a terceros que puedan ocasionar dichos bienes y restituir los mismos al COMODANTE o a quien éste designe al finalizar el término pactado o en caso de: <br>\n    (1) Necesidad imprevista y urgente del COMODANTE de disponer de los bienes. <br>\n    (2) Terminación o inviabilidad del proyecto o participación en alguna feria científica para la cual se prestaron los bienes.\n  </p>\n  \n  <strong>Cuarta. De la duración y perfeccionamiento.-</strong>\n  <p>\n    Este Comodato tendrá una vigencia de 1 días contados a partir de la firma del presente contrato, fecha en la cual se realizará la entrega material de los bienes objeto del comodato.\n  </p>\n  \n  <strong>Quinta. Del valor de los bienes.-</strong>\n  <p>\n    A pesar de que el Comodato es un préstamo sin costo, se acuerda que el valor total de los bienes es de 3000 Bs, en caso de que el GRUPO COMODATARIO se vea obligado a reembolsar dicho valor al COMODANTE. La descripción y el valor de cada uno de los bienes se detalla en la siguiente tabla:\n  </p>\n  \n  <table>\n    <thead>\n      <tr>\n        <th>Código IMT</th>\n        <th>Código UCB</th>\n        <th>Descripción</th>\n        <th># Serie</th>\n        <th>Cantidad</th>\n        <th>Precio Unitario (Bs)</th>\n        <th>Precio Total (Bs)</th>\n      </tr>\n    </thead>\n    <tbody>\n      \n      \n        <tr>\n          <td class="imt-code" data-grupo-id="28">Por definirse</td>\n          <td class="ucb-code" data-grupo-id="28">Por definirse</td>\n          <td>\n          <strong>Mini Dron</strong>\n          <p>Marca:   Default </p>\n          <p>Modelo:   Default </p>\n          </td>\n          <td class="serial-code" data-grupo-id="28">Por definirse</td>\n          <td>1</td>\n          <td>3000</td>\n          <td>3000</td> \n        </tr>\n      \n    \n      <tr>\n        <td colspan="6" style="text-align: right;"><strong>Total:</strong></td>\n        <td><strong>3000</strong></td>\n      </tr>\n    </tbody>\n  </table>\n  \n  <strong>Sexta. Del estado de los bienes.-</strong>\n  <p>\n    Al momento de la firma del presente contrato y de la entrega material, los bienes se encuentran en perfecto estado de funcionamiento y sin daño físico visible.\n  </p>\n  \n  <strong>Séptima. De la cláusula compromisoria.-</strong>\n  <p>\n    En caso de conflicto en la ejecución o liquidación del presente contrato, se agotará previamente la vía de conciliación con el apoyo del Departamento Legal de la Universidad Católica Boliviana "San Pablo". Si dicha conciliación fracasa, las diferencias serán sometidas a un Tribunal de Arbitramento, el cual se ubicará en el domicilio del COMODATARIO o en el lugar donde se encuentren los bienes, con los costos a cargo de ambas partes.\n  </p>\n  \n  <p>\n    En la ciudad de Santa Cruz de la Sierra, a los 14 días del mes de 6 de 2026.\n  </p>\n  \n   <div class="signature">\n    <div>\n      <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAtAAAAGQCAYAAACH51dtAAAAAXNSR0IArs4c6QAAIABJREFUeF7t3T+vLUt6F+DyNyAwEgm6TIZFQkBgC6NhMjImwAGBdcdyQEDgQbJ1hYQ0jEhGsiVmIhJL9o1AIkD+BHCFJTtAcupsGAmJhMABAZk5773rnemzztp7da/+91b1s6Src2ZOr+6q562992/Xqq7+peZFgAABAgQIECBAgMBsgV+afaQDCRAgQIAAAQIECBBoArRBQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBAL8ByKAECBAgQIECAAAEB2hggQIAAAQIECBAgsEBAgF6A5VACBAgQIECAAAECArQxQIAAAQIECBAgQGCBgAC9AMuhBAgQIECAAAECBARoY4AAAQIECBAgQIDAAgEBegGWQwkQIECAAAECBAgI0MYAAQIECBAgQIAAgQUCAvQCLIcSIECAAAECBAgQEKCNAQIECBAgQIAAAQILBAToBVgOJUCAAAECBAgQICBAGwMECBAgQIAAAQIEFggI0AuwHEqAAAECBAgQIEBAgDYGCBAgQIAAAQIECCwQEKAXYDmUAAECBAgQIECAgABtDBAgQIAAAQIECBBYICBA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JBChLl6x1OM+UMf/H4E6wrRZ6nVFi8AcvvEodrujrLP0bgIECHwtIEAbCAQIVBKYzlDn36fty1Ad/1/OVguFjyuY29PFEyPjYTdeBAgQILCRgAC9EaTTECCwm0AG6fhzuo46L2im+mP66Zpz29PtNiydmACBKwsI0Feuvr4T6FcgQuI/a639vQ+zq/cz1Tkjncs/Rpl9zX4+mnGP2eZ4xa4n8YrlGm7M7Hd8azkBAsUFBOjiBdI8AgRmCUS4zP96X0+d/chQ/GjWfYqSgTofXCM4zxoyDiJAgMDrAgL063beSYBAbYE566mjBxk4f/bGTXYZUON88ff88x+01v7HhGD6b/cBN2+YzP8/Q3L871yaEn/P0Hz//mhjtO//3q6ZbbL+u/YY1DoCBAYVEKAHLaxuESDwicD9LPWjmxTPYpsuO4mgHIE5t/Y7q02uS4AAAQJvCAjQhgYBAlcXyNnh+z+fufzybTZ4Orucf//s9uYIw/GK/51/z/NmQLbk4pm0fydAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gICdO36aB0BAgQIECBAgEAxAQG6WEE0hwABAgQIECBAoLaAAF27PlpHgAABAgQIECBQTECALlYQzSFAgAABAgQIEKgtIEDXro/WESBAgAABAgQIFBMQoIsVRHMIECBAgAABAgRqCwjQteujdQQIECBAgAABAsUEBOhiBdEcAgQIECBAgACB2gL/H0fl0PpGZZTJAAAAAElFTkSuQmCC" alt="Firma del COMODANTE">\n      <p>Job Angel Ledezma Dr.Ing</p>\n      <p>Director de Carrera</p>\n      <p>Ingeniería Mecatrónica</p>\n      <p>UNIV. CATÓLICA BOLIVIANA "SAN PABLO"</p>\n      <p>COMODANTE</p>\n    </div>\n    <div>\n      <!-- Este es el espacio que actualizaremos cuando se reciba la firma -->\n      <img id="firmaUsuarioPlaceholder" src="data:image/png;base64,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" alt="Firma del COMODATARIO">\n      <p>Fernando</p>\n      <p>Estudiante de Ingeniería Mecatrónica</p>\n      <p>C.I. 12890061</p>\n      <p>COMODATARIO</p>\n    </div>\n  </div>\n</div></div>
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
207	\N	251	f	28	\N
208	131	252	f	28	operativo
212	\N	256	f	28	\N
213	132	257	f	29	\N
214	133	258	f	30	operativo
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
-- Data for Name: notificaciones; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.notificaciones (id_notificacion, carnet_usuario, tipo, titulo, contenido, detalle, leido, fecha_envio, estado_eliminado) FROM stdin;
\.


--
-- Data for Name: prestamos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.prestamos (id_prestamo, fecha_solicitud, fecha_prestamo, fecha_devolucion_esperada, observacion, estado_prestamo, carnet, estado_eliminado, fecha_devolucion, fecha_prestamo_esperada, id_contrato, recordatorio_enviado) FROM stdin;
251	2026-06-12 22:21:43.346402	2026-06-13 00:00:00	2026-06-14 00:00:00		cancelado	12890061	f	\N	2026-06-13 00:00:00	\N	f
257	2026-06-14 16:20:23.932819	2026-06-14 00:00:00	2026-06-15 00:00:00		rechazado	12890061	f	\N	2026-06-14 00:00:00	\N	f
258	2026-06-14 16:38:36.56072	2026-06-14 00:00:00	2026-06-15 00:00:00		finalizado	12890061	f	2026-07-01 04:44:45.618916	2026-06-14 00:00:00	\N	f
252	2026-06-12 22:22:49.480858	2026-06-13 00:00:00	2026-06-14 00:00:00		finalizado	12890061	f	2026-06-12 22:25:54.205555	2026-06-13 00:00:00	\N	f
256	2026-06-12 23:38:59.526079	2026-06-13 00:00:00	2026-06-14 00:00:00		rechazado	12890061	f	\N	2026-06-13 00:00:00	12	f
\.


--
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.usuarios (carnet, nombre, apellido_paterno, apellido_materno, rol, contrasena, email, telefono, telefono_referencia, nombre_referencia, email_referencia, estado_eliminado, id_carrera, imagen_frente_carnet, imagen_atras_carnet, refresh_token, refresh_token_expiry, bloqueado, motivo_bloqueo) FROM stdin;
12890061	Fernando	Terrazas	Llanos	administrador	$2a$10$/8JV2T7ZgDGesA4Bd8J1Ne7YprDGYSOIS3vdcXZ9TBf2B4aifVe0G	fernando.terrazas@ucb.edu.bo	799430792	\N	\N	\N	f	2	\N	\N	d07mtHD3Npu0x5MA5V446qFRD0H6D6hw0apMwi3sWmRCBKl+qmENF/jZ8Y3g6oI/k47qtaXvgqrMkf72rGjNwg==	2026-07-09 19:23:07.756327-04	f	\N
\.


--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.aggregatedcounter_id_seq', 602, true);


--
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 666, true);


--
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 9, true);


--
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 221, true);


--
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 884, true);


--
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 221, true);


--
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 222, true);


--
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 663, true);


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

SELECT pg_catalog.setval('public."Prestamo_Id_Prestamo_seq"', 258, true);


--
-- Name: audit_logs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.audit_logs_id_seq', 75, true);


--
-- Name: avisos_disponibilidad_id_aviso_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.avisos_disponibilidad_id_aviso_seq', 1, false);


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

SELECT pg_catalog.setval('public.detalles_prestamos_id_detalle_prestamo_seq', 214, true);


--
-- Name: nombre_de_tu_tabla_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.nombre_de_tu_tabla_id_seq', 12, true);


--
-- Name: notificaciones_id_notificacion_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.notificaciones_id_notificacion_seq', 1, false);


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
-- Name: avisos_disponibilidad avisos_disponibilidad_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.avisos_disponibilidad
    ADD CONSTRAINT avisos_disponibilidad_pkey PRIMARY KEY (id_aviso);


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
-- Name: notificaciones notificaciones_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT notificaciones_pkey PRIMARY KEY (id_notificacion);


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

CREATE INDEX idx_audit_admin ON public.audit_logs USING btree (admin_carnet, "timestamp" DESC, estado_eliminado);


--
-- Name: idx_audit_entidad; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_audit_entidad ON public.audit_logs USING btree (entidad, entidad_id, estado_eliminado);


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
-- Name: ix_avisos_disponibilidad_pendiente; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_avisos_disponibilidad_pendiente ON public.avisos_disponibilidad USING btree (notificado, estado_eliminado);


--
-- Name: ix_detalles_prestamos_id_equipo; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_detalles_prestamos_id_equipo ON public.detalles_prestamos USING btree (id_equipo);


--
-- Name: ix_notificaciones_carnet; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_notificaciones_carnet ON public.notificaciones USING btree (carnet_usuario, leido, estado_eliminado);


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
-- Name: avisos_disponibilidad avisos_disponibilidad_carnet_usuario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.avisos_disponibilidad
    ADD CONSTRAINT avisos_disponibilidad_carnet_usuario_fkey FOREIGN KEY (carnet_usuario) REFERENCES public.usuarios(carnet);


--
-- Name: avisos_disponibilidad avisos_disponibilidad_id_grupo_equipo_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.avisos_disponibilidad
    ADD CONSTRAINT avisos_disponibilidad_id_grupo_equipo_fkey FOREIGN KEY (id_grupo_equipo) REFERENCES public.grupos_equipos(id_grupo_equipo);


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
-- Name: notificaciones notificaciones_carnet_usuario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT notificaciones_carnet_usuario_fkey FOREIGN KEY (carnet_usuario) REFERENCES public.usuarios(carnet);


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

