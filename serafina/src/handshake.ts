// performHandshake pings sibling repositories defined in SIBLING_ENDPOINTS
// to exchange version and capability metadata. Uses the Node 18+ global `fetch`.
export async function performHandshake(): Promise<void> {
  const endpoints = (process.env.SIBLING_ENDPOINTS || '')
    .split(',')
    .map((s: string) => s.trim())
    .filter(Boolean);

  for (const url of endpoints) {
    try {
      const res = await fetch(`${url}/handshake`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          repo: 'GameDinVR',
          version: process.env.npm_package_version || '0.0.0',
          timestamp: new Date().toISOString()
        })
      });
      console.log(`[handshake] ${url} -> ${res.status}`);
    } catch (err) {
      console.error(`[handshake] failed to reach ${url}`, err);
    }
  }
}
