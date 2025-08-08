import 'dotenv/config';
import { Client, EmbedBuilder, TextChannel } from 'discord.js';
import cron from 'cron';

// Relies on Node 18+ global `fetch` for HTTP calls.

const MCP = process.env.MCP_URL!; // MCP endpoint for status queries
const COUNCIL_CH = process.env.CHN_COUNCIL!; // Discord channel ID
const LILY_WEBHOOK = process.env.WH_LILYBEAR; // optional prettier sender
const GH_REPOS = (process.env.NAV_REPOS || '')
  .split(',')
  .map((s) => s.trim())
  .filter(Boolean);

async function getMcpStatus(): Promise<string> {
  try {
    const r = await fetch(`${MCP}/ask-gemini`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ prompt: 'Summarize system health in one sentence.' })
    });
    // Cast to any because the Gemini response schema may vary
    const j: any = await r.json().catch(() => ({ response: '(no data)' }));
    return (j.response as string) || '(no data)';
  } catch {
    return '(MCP unreachable)';
  }
}

async function getRepoDigest(repo: string): Promise<string> {
  const since = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString();
  const url = `https://api.github.com/repos/${repo}/commits?since=${encodeURIComponent(
    since
  )}&per_page=5`;
  try {
    const r = await fetch(url, {
      headers: { Accept: 'application/vnd.github+json' }
    });
    if (!r.ok) return `• ${repo}: no recent commits`;
    const commits = (await r.json()) as any[];
    if (!commits.length) return `• ${repo}: 0 commits in last 24h`;
    return commits
      .map(
        (c) => `• ${repo}@${(c.sha || '').slice(0, 7)} — ${c.commit.message.split('\n')[0]}`
      )
      .join('\n');
  } catch {
    return `• ${repo}: (error fetching commits)`;
  }
}

export async function generateCouncilReport(client: Client): Promise<void> {
  const mcp = await getMcpStatus();
  const repoLines = GH_REPOS.length
    ? (await Promise.all(GH_REPOS.map(getRepoDigest))).join('\n')
    : '—';

  const emb = new EmbedBuilder()
    .setTitle('🌙 Nightly Council Report')
    .setDescription('Summary of the last 24h across our realm.')
    .setColor(0x9b59b6)
    .addFields(
      { name: 'System Health (MCP)', value: mcp.slice(0, 1024) || '—' },
      { name: 'Recent Commits', value: repoLines.slice(0, 1024) || '—' }
    )
    .setFooter({ text: 'Reported by Lilybear' })
    .setTimestamp(new Date());

  if (LILY_WEBHOOK) {
    await fetch(LILY_WEBHOOK, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ embeds: [emb.toJSON()] })
    });
  } else {
    const ch = (await client.channels.fetch(COUNCIL_CH)) as TextChannel;
    await ch.send({ embeds: [emb] });
  }
}

export function scheduleNightlyCouncilReport(client: Client): void {
  // 08:00 UTC daily
  const job = new cron.CronJob('0 8 * * *', () => generateCouncilReport(client), null, true, 'UTC');
  job.start();
}
