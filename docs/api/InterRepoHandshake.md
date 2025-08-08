# Inter-Repo Handshake Protocol

This protocol lets sibling repositories announce their presence and confirm API compatibility during startup.

## Goals
- Discover active services across the MKWW/GameDin network.
- Share version and capability metadata for cross-repo calls.
- Gracefully handle offline siblings without blocking.

## HTTP Contract
- **Method:** `POST`
- **Path:** `/handshake`
- **Request Body:**
```json
{
  "repo": "GameDinVR",
  "version": "1.0.0",
  "timestamp": "2025-01-14T00:00:00Z"
}
```
- **Response Body:**
```json
{
  "status": "ok",
  "repo": "Serafina",
  "capabilities": ["discord", "mcp"]
}
```

Repositories should cache successful handshakes and retry unreachable peers with exponential backoff.

## Environment Configuration
Define sibling endpoints in `.env`:
```
SIBLING_ENDPOINTS=https://serafina.example.com,https://lilybear.example.com
```

## Reference Implementation
See `serafina/src/handshake.ts` for a Node.js helper that iterates endpoints and logs results.

