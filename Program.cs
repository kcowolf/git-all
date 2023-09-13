using CliWrap;
using CliWrap.EventStream;

namespace git_all
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var gitArgs = string.Join(' ', args);

            foreach (var directory in Directory.GetDirectories("."))
            {
                if (!Directory.Exists(Path.Combine(directory, ".git")))
                {
                    continue;
                }

                Console.WriteLine("\n--------------------------------------------------------------------------------");
                Console.WriteLine(directory);
                Console.WriteLine();

                var cmd = Cli.Wrap("git.exe") 
                    .WithArguments(gitArgs)
                    .WithWorkingDirectory(directory)
                    .WithValidation(CommandResultValidation.None);

                await foreach (var cmdEvent in cmd.ListenAsync())
                {
                    switch (cmdEvent)
                    {
                        case StandardOutputCommandEvent stdOut:
                            Console.WriteLine($"{stdOut.Text}");
                            break;
                        case StandardErrorCommandEvent stdErr:
                            Console.WriteLine($"{stdErr.Text}");
                            break;
                    }
                }
            }

            Console.WriteLine("Done!");
        }
    }
}