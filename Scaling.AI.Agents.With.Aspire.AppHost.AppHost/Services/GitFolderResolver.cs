namespace Scaling.AI.Agents.With.Aspire.AppHost.AppHost.Services
{
    public static class GitFolderResolver
    {
        public static string GetGitFolderName(string? workingDirectory = null)
        {
            try
            {
                var directory = workingDirectory ?? Directory.GetCurrentDirectory();
                var gitPath = FindGitPath(directory);

                if (gitPath is null)
                    return "default";

                if (Directory.Exists(gitPath))
                {
                    var repoDirectory = Path.GetDirectoryName(gitPath);
                    return Path.GetFileName(repoDirectory).Replace(".", "") ?? "default";
                }

                if (File.Exists(gitPath))
                {
                    var gitFileContent = File.ReadAllText(gitPath);

                    if (gitFileContent.StartsWith("gitdir:"))
                    {
                        var gitDirPath = gitFileContent.Substring("gitdir:".Length).Trim();
                        var worktreeSegments = gitDirPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < worktreeSegments.Length - 1; i++)
                            if (worktreeSegments[i] == "worktrees")
                                return worktreeSegments[i + 1];

                        return Path.GetFileName(directory).Replace(".", "") ?? "default";
                    }
                }

                return "default";
            }
            catch
            {
                return "default";
            }

            static string? FindGitPath(string directory)
            {
                var currentDirectory = directory;

                while (!string.IsNullOrEmpty(currentDirectory))
                {
                    var gitPath = Path.Combine(currentDirectory, ".git");

                    if (Directory.Exists(gitPath) || File.Exists(gitPath))
                        return gitPath;

                    var parentDirectory = Path.GetDirectoryName(currentDirectory);
                    if (parentDirectory == currentDirectory)
                        break;

                    currentDirectory = parentDirectory;
                }

                return null;
            }
        }
    }
}