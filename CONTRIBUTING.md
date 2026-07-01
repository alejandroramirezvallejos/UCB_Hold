<div align="center">

# Contributing

Guía para contribuir a UCB Hold con cambios pequeños, revisables y alineados con la arquitectura del proyecto.

[README](README.md) · [Setup](Docs/SETUP.md) · [Security](SECURITY.md) · [Code of Conduct](CODE_OF_CONDUCT.md)

</div>

---

## <img height="22" src="Images/readme-icons/contributing.svg" alt="" /> Flujo de Trabajo

1. Revisar issues o abrir una propuesta si el cambio es grande.
2. Crear una rama corta y descriptiva.
3. Mantener el cambio enfocado en un solo objetivo.
4. Actualizar documentación cuando cambien setup, API, arquitectura o comportamiento visible.
5. Enviar PR con contexto, pasos de prueba y capturas si cambia la UI.

---

## Convenciones

| Área      | Regla                                                                                         |
| --------- | --------------------------------------------------------------------------------------------- |
| Commits   | Conventional Commits: `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `ci:`.                  |
| Ramas     | `feature/<descripcion>`, `fix/<descripcion>`, `refactor/<descripcion>`, `docs/<descripcion>`. |
| Frontend  | Feature-Sliced Design, BEM, archivos en `kebab-case`, TypeScript estricto.                    |
| Backend   | Servicios con reglas de negocio, DTOs explícitos, validación centralizada y tests.            |
| Seguridad | No versionar secretos; usar `dotnet user-secrets` o variables de entorno.                     |

---

## Checklist Local

Backend:

```bash
dotnet build Code/Server/IMT_Reservas.Server.csproj
dotnet test Code/Tests/IMT_Reservas.Tests.csproj
```

Frontend:

```bash
cd Code/Client
npm run format:check
npx tsc -p tsconfig.app.json --noEmit
npx tsc -p tsconfig.spec.json --noEmit
npm run test:coverage
npm run build
```

---

## Pull Requests

Un buen PR debe incluir:

| Campo       | Qué incluir                                                |
| ----------- | ---------------------------------------------------------- |
| Qué cambió  | Resumen breve del cambio.                                  |
| Por qué     | Problema, issue o deuda que resuelve.                      |
| Cómo probar | Comandos ejecutados y escenarios manuales.                 |
| Riesgo      | Migraciones, cambios visuales, seguridad o compatibilidad. |

Evitar dependencias nuevas salvo que el beneficio, tamaño e impacto de seguridad estén justificados.
