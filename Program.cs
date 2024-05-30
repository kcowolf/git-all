using CliWrap;
using CliWrap.EventStream;
using System.Text;

namespace git_all
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var gitArgs = string.Join(' ', args);
            var directories = Directory.GetDirectories(".");
            var tasks = new Dictionary<string, Task<string>>();

            foreach (var directory in directories)
            {
                var gitPath = Path.Combine(directory, ".git");

                if (!Directory.Exists(gitPath) && !File.Exists(gitPath))
                {
                    continue;
                }

                tasks.Add(directory, Run(directory, gitArgs));
            }

            foreach (var directory in directories)
            {
                if (tasks.ContainsKey(directory))
                {
                    Console.WriteLine(await tasks[directory]);
                }
            }

            Console.WriteLine("Done!");
        }

        private static async Task<string> Run(string directory, string gitArgs)
        {
            var output = new StringBuilder();
            output.AppendLine("\n--------------------------------------------------------------------------------");
            output.AppendLine(directory);
            output.AppendLine();

            var cmd = Cli.Wrap("git.exe")
                .WithArguments(gitArgs)
                .WithWorkingDirectory(directory)
                .WithValidation(CommandResultValidation.None);

            await foreach (var cmdEvent in cmd.ListenAsync())
            {
                switch (cmdEvent)
                {
                    case StandardOutputCommandEvent stdOut:
                        output.AppendLine($"{stdOut.Text}");
                        break;
                    case StandardErrorCommandEvent stdErr:
                        output.AppendLine($"{stdErr.Text}");
                        break;
                }
            }

            return output.ToString();
        }
    }
}
