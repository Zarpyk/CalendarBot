using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CalendarBot.Commands;
using CalendarBot.Database;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SQLite;

namespace CalendarBot {
    public class Bot {
        public DiscordClient client { get; private set; }
        public CommandsNextExtension commands { get; private set; }
        public static SQLiteConnection db;

        public static readonly string greenHexColor = "#4aff7a";
        public static readonly string errorHexColor = "#ff5454";
        public static readonly string helpHexColor = "#73ceff";

        private static Timer timer;

        public async Task RunAsync() {
            string json = string.Empty;
            using (FileStream fs = File.OpenRead("config.json")) {
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false))) {
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            ConfigJson configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            DiscordConfiguration config = new DiscordConfiguration() {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Error
            };

            client = new DiscordClient(config);
            client.Ready += OnClientReady;

            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration() {
                StringPrefixes = new[] {configJson.Prefix},
                EnableMentionPrefix = true,
                EnableDms = true,
                IgnoreExtraArguments = true,
                EnableDefaultHelp = false
            };

            commands = client.UseCommandsNext(commandsConfig);

            commands.RegisterCommands<CalendarCommand>();
            commands.RegisterCommands<AddCommand>();
            commands.RegisterCommands<RemoveCommand>();
            commands.RegisterCommands<HelpCommand>();

            setUpDB();
            setUpTimer();

            await client.ConnectAsync(new DiscordActivity("!help", ActivityType.Playing));
            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e) {
            return Task.CompletedTask;
        }

        private void setUpDB() {
            string databasePath = Path.Combine(Directory.GetCurrentDirectory(), "Database.db");
            db = new SQLiteConnection(databasePath);
            db.CreateTable<CalendarTask>();
        }

        private void setUpTimer() {
            timer = new Timer(60000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e) {
            foreach (CalendarTask calendarTask in db.Table<CalendarTask>().ToList().Where(x => x.time < DateTime.Now)) {
                db.Delete<CalendarTask>(calendarTask.ID);
            }
            Console.WriteLine("Se ha actualizado el DB");
        }
    }
}