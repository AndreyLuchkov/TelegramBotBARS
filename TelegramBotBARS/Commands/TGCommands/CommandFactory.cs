namespace TelegramBotBARS.Commands
{
    public class CommandFactory
    {
        private static readonly Dictionary<string, Func<ICommand>> _commands = new();

        public CommandFactory()
        {
            _commands["/start"] = () => new StartCommand();
            _commands["/km"] = () => new KmCommand();
            _commands[";statement"] = () => new StatementCommand();
            _commands[";semester"] = () => new SemesterCommand();
            _commands["/miss"] = () => new MissCommand();
            _commands["/results"] = () => new ResultsCommand();
        }
        public bool IsCommandExist(string commandName) => _commands.ContainsKey(commandName);
        public ICommand GetCommand(string commandName)
        {
            Func<ICommand>? command;
            if (_commands.TryGetValue(commandName, out command))
            {
                return command();
            }
            else
            {
                throw new ArgumentException("The command does not exist.");
            }
        }
        public string[] GetCommandNames() => _commands.Keys.Where(name => name[0] != ';').ToArray();
    }
}
