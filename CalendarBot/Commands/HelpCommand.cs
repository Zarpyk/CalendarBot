using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarBot.Database;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace CalendarBot {
    public class HelpCommand : BaseCommandModule {
        [Command("Help"), Description("Sirve para ver el Help")]
        public async Task Help(CommandContext ctx, string command = "default") {
            var embed = new DiscordEmbedBuilder();
            switch (command.ToLower()) {
                case "calendario":
                case "cal":
                case "c": {
                    await CalendarHelp(ctx);
                    break;
                }
                case "add": {
                    await AddHelp(ctx);
                    break;
                }
                case "add+": {
                    await AddHelp(ctx, true);
                    break;
                }
                case "remove": {
                    await RemoveHelp(ctx);
                    break;
                }
                default: {
                    embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.helpHexColor));

                    //TODO AddField Help

                    embed.AddField("!Calendario, !Cal, !C",
                        "!Calendario [Pagina] - Muestra todas las tareas");
                    embed.AddField("!Update", "!Update - Elimina las tareas antiguas " + //TODO Help Update
                                              "(existe un auto-update cada minuto)");
                    embed.AddField("!Add", "!Add <Indice> <Dia> <Mes> <Tarea a hacer>\n" +
                                           "Añade una tarea (a las 23:59)");
                    embed.AddField("!Add+, !AddP", "!Add+ <Indice> <Dia> <Mes> <Hora> <Minuto> <Tarea a hacer>\n" +
                                            "Añade una tarea a una hora especifica");
                    embed.AddField("!Remove, !Delete, !Del", "!Remove <ID> - Elimina una tarea");
                    break;
                }
            }

            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }

        public static async Task CalendarHelp(CommandContext ctx) {
            var embed = new DiscordEmbedBuilder();
            embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.helpHexColor));
            embed.AddField("Comando", "!Calendario [Pagina]");
            embed.AddField("[Pagina]",
                "Parametro opcional para indicar la pagina del calendario (paginas de 25 tareas)");
            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }

        public static async Task AddHelp(CommandContext ctx, bool plus = false) {
            var embed = new DiscordEmbedBuilder();
            embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.helpHexColor));
            embed.AddField("Comando",
                "!Add" + (plus ? "+" : "") + " <Indice> <Dia> <Mes> " + (plus ? "<Hora> <Minuto>" : "") +
                " <Tarea a hacer>");

            string index = string.Empty;
            List<string> values = Enum.GetNames(typeof(Course)).ToList();
            for (int i = 0; i < values.Count; i++) {
                index += $"{i + 1} - {values[i].Replace("_", " ")}{(i == values.Count - 1 ? "" : "\n")}";
            }
            embed.AddField("<Indice>", index);

            if (!plus) embed.AddField("<Dia> <Mes>", "Indicar el dia y el mes en que termina la tarea");
            else {
                embed.AddField("<Dia> <Mes> <Hora> <Minuto>",
                    "Indicar el dia, mes, hora y minuto en que termina la tarea");
            }

            embed.AddField("<Tarea a hacer>", "Indicar cual es la tarea");

            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }

        public static async Task RemoveHelp(CommandContext ctx) {
            var embed = new DiscordEmbedBuilder();
            embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.helpHexColor));
            embed.AddField("Comando", "!Remove <ID>");
            embed.AddField("<ID>", "Se muestra entre parentesis en la parte derecha de las tareas en !cal");

            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }
    }
}