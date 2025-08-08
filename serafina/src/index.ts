import 'dotenv/config';
import { Client, GatewayIntentBits, REST, Routes } from 'discord.js';
import { scheduleNightlyCouncilReport, generateCouncilReport } from './nightlyReport.js';
import { performHandshake } from './handshake.js';

const token = process.env.DISCORD_TOKEN!;
const guildId = process.env.GUILD_ID!;
const clientId = process.env.OWNER_ID!; // using owner ID as application ID placeholder

const client = new Client({ intents: [GatewayIntentBits.Guilds] });

async function registerCommands() {
  const rest = new REST({ version: '10' }).setToken(token);
  await rest.put(Routes.applicationGuildCommands(clientId, guildId), {
    body: [
      {
        name: 'councilreport',
        description: 'Generate the council report immediately.'
      }
    ]
  });
}

client.once('ready', async () => {
  console.log(`[serafina] Logged in as ${client.user?.tag}`);
  scheduleNightlyCouncilReport(client);
  performHandshake();
});

client.on('interactionCreate', async (interaction) => {
  if (!interaction.isChatInputCommand()) return;
  if (interaction.commandName === 'councilreport') {
    await interaction.deferReply({ ephemeral: true });
    await generateCouncilReport(client);
    await interaction.editReply('Council report dispatched.');
  }
});

registerCommands().then(() => client.login(token));
