using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using System.Linq;
using Discord.WebSocket;
using Discord.Addons.InteractiveCommands;

namespace SataniaBot.Services.EmbedHelper
{
    public static class EmbedHelper
    {
        public static async Task SendConfirmAsync(this IMessageChannel Channel, string message)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Color = new Color(111, 237, 69);
            builder.Description = message;

            await Channel.SendMessageAsync("", embed: builder);
        }

        public static async Task SendConfirmAsync(this IMessageChannel Channel, string title, string message)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Color = new Color(111, 237, 69);
            builder.Title = title;
            builder.Description = message;

            await Channel.SendMessageAsync("", embed: builder);
        }

        public static async Task SendErrorAsync(this IMessageChannel Channel, string message)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Color = new Color(222, 90, 47);
            builder.Description = message;

            await Channel.SendMessageAsync("", embed: builder);

        }

        public static async Task SendErrorAsync(this IMessageChannel Channel, string title, string message)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Color = new Color(222, 90, 47);
            builder.Title = title;
            builder.Description = message;

            await Channel.SendMessageAsync("", embed: builder);
        }

        public static async Task SendColouredEmbedAsync(this IMessageChannel Channel, string message, Color color)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Color = color;
            builder.Description = message;

            await Channel.SendMessageAsync("", embed: builder);
        }

        public static async Task SendColouredEmbedAsync(this IMessageChannel Channel, string title, string message, Color color)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Color = color;
            builder.Title = title;
            builder.Description = message;

            await Channel.SendMessageAsync("", embed: builder);
        }
    }
}
