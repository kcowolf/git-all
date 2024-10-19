using Common;

namespace git_all_recursive
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var gitArgs = string.Join(' ', args);
            var directories = GitRunner.GetGitDirectories(".", true);
            var tasks = new Dictionary<string, Task<string>>();

            foreach (var directory in directories)
            {
                tasks.Add(directory, GitRunner.Run(directory, gitArgs));
            }

            foreach (var directory in directories)
            {
                if (tasks.TryGetValue(directory, out Task<string>? value))
                {
                    Console.WriteLine(await value);
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
