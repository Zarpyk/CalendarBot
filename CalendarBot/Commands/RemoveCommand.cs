using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarBot.Database;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace CalendarBot.Commands {
    public class RemoveCommand : BaseCommandModule {
        [Command("Remove"), Description("Sirve para eliminar una tarea"), Priority(0)]
        public async Task Remove(CommandContext ctx) {
            await HelpCommand.RemoveHelp(ctx);
        }

        [Command("Remove"), Aliases("Delete", "Del"), Description("Sirve para eliminar una tarea"), Priority(1)]
        public async Task Remove(CommandContext ctx, int index) {
            var embed = new DiscordEmbedBuilder();

            if (Bot.db.Table<CalendarTask>().FirstOrDefault(x => x.ID == index) != null) {
                Bot.db.Delete<CalendarTask>(index);
                embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.greenHexColor));
                embed.Description = $"Se ha borrado la tarea de ID {index}.";
            } else {
                embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.errorHexColor));
                embed.Description = $"El ID {index} no existe.";
            }

            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }
    }
}