using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarBot.Database;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace CalendarBot.Commands {
    public class AddCommand : BaseCommandModule {
        [Command("Add"), Description("Sirve para añadir una tarea (23:59)"), Priority(0)]
        public async Task Add(CommandContext ctx) {
            await HelpCommand.AddHelp(ctx);
        }

        [Command("Add+"),  Description("Sirve para añadir una tarea con hora y minuto especifico"), Priority(0)]
        public async Task AddPlus(CommandContext ctx) {
            await HelpCommand.AddHelp(ctx, true);
        }

        [Command("Add"), Description("Sirve para añadir una tarea (23:59)"), Priority(1)]
        public async Task Add(CommandContext ctx, int index, int day, int month, string task) {
            await Add(ctx, index, day, month, 23, 59, task);
        }

        [Command("Add+"), Aliases("AddP"), Description("Sirve para añadir una tarea con hora y minuto especifico"), Priority(1)]
        public async Task Add(CommandContext ctx, int index, int day, int month, int hour, int minute, string task) {
            bool add = true;
            bool indexParameter = true;
            bool dayParameter = true;
            bool monthParameter = true;
            bool taskParameter = true;
            if (index < 1 || index > Enum.GetNames(typeof(Course)).Length) {
                add = false;
                indexParameter = false;
            } else {
                if (day < 0) {
                    add = false;
                    dayParameter = false;
                } else {
                    switch (month) {
                        case 1:
                        case 3:
                        case 5:
                        case 7:
                        case 8:
                        case 10:
                        case 12: {
                            if (day > 31) {
                                add = false;
                                dayParameter = false;
                            }
                            break;
                        }
                        case 4:
                        case 6:
                        case 9:
                        case 11: {
                            if (day > 30) {
                                add = false;
                                dayParameter = false;
                            }
                            break;
                        }
                        case 2: {
                            if (DateTime.IsLeapYear(DateTime.Now.Year) && day > 29) {
                                add = false;
                                dayParameter = false;
                            } else if (!DateTime.IsLeapYear(DateTime.Now.Year) && day > 28) {
                                add = false;
                                dayParameter = false;
                            }
                            break;
                        }
                        default: {
                            add = false;
                            monthParameter = false;
                            break;
                        }
                    }
                }
            }
            if (task.Length > 1024) {
                add = false;
                taskParameter = false;
            }

            var embed = new DiscordEmbedBuilder();
            if (add) {
                CalendarTask calendar = new CalendarTask {
                    course = (Course) index,
                    time = new DateTime(DateTime.Now.Year, month, day, hour, minute, 0),
                    task = task
                };

                Bot.db.Insert(calendar);

                embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.greenHexColor));
                embed.Description = "Se ha añadido la tarea.";
            } else {
                embed.Color = new Optional<DiscordColor>(new DiscordColor(Bot.errorHexColor));
                string errorText = "El parametro \"";
                if (!indexParameter) errorText += "<Indice>";
                if (!dayParameter) errorText += "<Dia>";
                if (!monthParameter) errorText += "<Mes>";
                if (!taskParameter) errorText += "<Tarea a hacer> (max. 1024 caracteres)";
                errorText += "\" es incorrecto.";
                embed.Description = errorText;
            }

            embed.Build();
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }
    }
}