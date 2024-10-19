using CliWrap;
using CliWrap.EventStream;
using System.Text;

namespace Common
{
    public static class GitRunner
    {
        public static List<string> GetGitDirectories(string baseDirectory, bool recursive)
        {
            var results = new List<string>();
            var subDirectories = Directory.GetDirectories(baseDirectory);

            foreach (var subDirectory in subDirectories)
            {
                var gitPath = Path.Combine(subDirectory, ".git");
                if (Directory.Exists(gitPath) || File.Exists(gitPath))
                {
                    results.Add(subDirectory);
                }

                if (recursive)
                {
                    results.AddRange(GetGitDirectories(subDirectory, true));
                }
            }

            return results;
        }

        public static async Task<string> Run(string directory, string gitArgs)
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
