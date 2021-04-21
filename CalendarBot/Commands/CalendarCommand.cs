using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using CalendarBot.Database;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace CalendarBot.Commands {
    public class CalendarCommand : BaseCommandModule {
        private string calendarColor = "#fff878";

        [Command("Calendario"), Aliases("Cal", "C"), Description("Sirve para ver el calendario")]
        public async Task Calendar(CommandContext ctx, int page = 1) { //TODO Comprobar si el page = 1 esta funcioando bien
            var embed = new DiscordEmbedBuilder();
            embed.Color = new Optional<DiscordColor>(new DiscordColor(calendarColor));
            List<CalendarTask> calendarTasks = Bot.db.Table<CalendarTask>().ToList();
            calendarTasks = calendarTasks.OrderBy(x => x.time).ToList();

            if (calendarTasks.Count == 0) {
                embed.Description = "No hay ninguna tarea.";
            } else {
                if (page < 0) page = 1;
                else {
                    while (calendarTasks.Count < (page - 1) * 25 + 1) {
                        page -= 1;
                    }
                }
                for (int index = (page - 1) * 25; index < calendarTasks.Count; index++) {
                    CalendarTask calendar = calendarTasks[index];
                    embed.AddField(
                        $"{calendar.time:dd-MM-yyyy (hh:mm)}\n{calendar.course.ToString().Replace("_", " ")} ({calendar.ID})", //TODO hh se muestra 11 en vez de 23
                        $"{calendar.task}", true);
                }
            }

            int maxPage = (int) Math.Ceiling(calendarTasks.Count / 25.0f);
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter {
                Text = $"Pagina: {Math.Min(page, maxPage)}/{maxPage}"
            };
            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }

        [Command("Update"), Description("Sirve para actualizar el calendario, eliminando tareas antiguas " +
                                        "(se comprueba automaticamente cada 10 minutos)")]
        public async Task Update(CommandContext ctx) {
            var embed = new DiscordEmbedBuilder();

            foreach (CalendarTask calendarTask in Bot.db.Table<CalendarTask>().ToList()
                .Where(calendarTask => calendarTask.time < DateTime.Now)) {
                Bot.db.Delete<CalendarTask>(calendarTask.ID);
            }

            embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.greenHexColor));
            embed.Description = $"Se ha actualizado el calendario";

            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }
    }
}