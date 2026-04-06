# POS Events API

ASP.NET Core 9 Web API for ingesting and querying point-of-sale events. In-memory storage, structured logging, fire-and-forget background processing.

## Run

```bash
dotnet run --project src/PosEvents.Api
```

## Endpoints

### `POST /events`

```json
{
  "eventType": "sale",
  "timestamp": "2025-01-15T10:00:00Z",
  "payload": { "amount": 42.50 }
}
```

- `200` on success
- `400` with per-field validation errors on failure

### `GET /events`

Returns all stored events in insertion order.

## Test

```bash
dotnet test
```
