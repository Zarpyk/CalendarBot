namespace CalendarBot {
    internal class Program {
        public static void Main(string[] args) {
            Bot bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}