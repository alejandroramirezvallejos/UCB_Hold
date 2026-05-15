# API Reference

Backend `.NET 8` — REST endpoints under `/api/{Controller}`.
Interactive documentation available at `/swagger` (Development only).

---

## ✦ Response Format

All responses use the [Ardalis.Result](https://github.com/ardalis/Result) envelope.

**Success `2xx`**

```json
{
  "Status": 200,
  "Value": { "Id": 1, "Nombre": "Ejemplo" },
  "Errors": [],
  "ValidationErrors": []
}
```

**Created `201`**

```json
{
  "Status": 201,
  "Value": { "Id": 42 }
}
```

**Validation error `400`**

```json
{
  "Status": 400,
  "Value": null,
  "Errors": [],
  "ValidationErrors": [
    { "Identifier": "Email", "ErrorMessage": "Formato inválido" }
  ]
}
```

**Not found `404`**

```json
{
  "Status": 404,
  "Value": null,
  "Errors": ["Not Found"]
}
```

**Unauthorized `401`**

```json
{
  "Status": 401,
  "Value": null,
  "Errors": ["Credenciales inválidas"]
}
```

---
