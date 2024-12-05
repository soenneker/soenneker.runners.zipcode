using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.String;
using Soenneker.Git.Util.Abstract;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Utils.Directory.Abstract;
using Soenneker.Utils.Dotnet.Abstract;
using Soenneker.Utils.Dotnet.NuGet.Abstract;
using Soenneker.Utils.Environment;
using Soenneker.Utils.File.Abstract;
using Soenneker.Utils.FileSync.Abstract;
using Soenneker.Utils.SHA3.Abstract;

namespace Soenneker.Runners.ZipCode.Utils;

///<inheritdoc cref="IFileOperationsUtil"/>
public class FileOperationsUtil : IFileOperationsUtil
{
    private readonly ILogger<FileOperationsUtil> _logger;
    private readonly IGitUtil _gitUtil;
    private readonly IDotnetUtil _dotnetUtil;
    private readonly IDotnetNuGetUtil _dotnetNuGetUtil;
    private readonly IFileUtil _fileUtil;
    private readonly IFileUtilSync _fileUtilSync;
    private readonly IDirectoryUtil _directoryUtil;
    private readonly ISha3Util _sha3Util;

    private string? _newHash;

    public FileOperationsUtil(IFileUtil fileUtil, ILogger<FileOperationsUtil> logger, IGitUtil gitUtil, IDotnetUtil dotnetUtil, IDotnetNuGetUtil dotnetNuGetUtil, IDirectoryUtil directoryUtil, 
        IFileUtilSync fileUtilSync, ISha3Util sha3Util)
    {
        _fileUtil = fileUtil;
        _logger = logger;
        _gitUtil = gitUtil;
        _dotnetUtil = dotnetUtil;
        _dotnetNuGetUtil = dotnetNuGetUtil;
        _directoryUtil = directoryUtil;
        _fileUtilSync = fileUtilSync;
        _sha3Util = sha3Util;
    }

    public async ValueTask Process(HashSet<string> hashSet)
    {
        string tempDir = _directoryUtil.CreateTempDirectory();

        string linesPath = Path.Combine(tempDir, "zipcodes.txt");

        await _fileUtil.WriteAllLines(linesPath, hashSet);

        string gitTempDirectory = _gitUtil.CloneToTempDirectory($"https://github.com/soenneker/{Constants.Library.ToLowerInvariantFast()}");
        
        string targetExePath = Path.Combine(gitTempDirectory, "src", "Resources", Constants.FileName);

        bool needToUpdate = await CheckForHashDifferences(gitTempDirectory, linesPath);

        if (!needToUpdate)
            return;

        await BuildPackAndPush(gitTempDirectory, targetExePath, linesPath);

        await SaveToGitRepo(gitTempDirectory);

        _fileUtilSync.Delete(linesPath);
        _directoryUtil.Delete(gitTempDirectory);
    }

    private async ValueTask BuildPackAndPush(string gitDirectory, string targetFilePath, string filePath)
    {
        _fileUtilSync.DeleteIfExists(targetFilePath);

        _directoryUtil.CreateIfDoesNotExist(Path.Combine(gitDirectory, "src", "Resources"));

        _fileUtilSync.Move(filePath, targetFilePath);

        string projFilePath = Path.Combine(gitDirectory, "src", $"{Constants.Library}.csproj");

        await _dotnetUtil.Restore(projFilePath);

        bool successful = await _dotnetUtil.Build(projFilePath, true, "Release", false);

        if (!successful)
        {
            _logger.LogError("Build was not successful, exiting...");
            return;
        }

        string version = EnvironmentUtil.GetVariableStrict("BUILD_VERSION");

        await _dotnetUtil.Pack(projFilePath, version, true, "Release", false, false, gitDirectory);

        string apiKey = EnvironmentUtil.GetVariableStrict("NUGET_TOKEN");

        string nuGetPackagePath = Path.Combine(gitDirectory, $"{Constants.Library}.{version}.nupkg");

        await _dotnetNuGetUtil.Push(nuGetPackagePath, apiKey);
    }

    private async ValueTask<bool> CheckForHashDifferences(string gitDirectory, string filePath)
    {
        string? oldHash = await _fileUtil.TryReadFile(Path.Combine(gitDirectory, "hash.txt"));

        if (oldHash == null)
        {
            _logger.LogDebug("Could not read hash from repository, proceeding to update...");
            return true;
        }

        _newHash = await _sha3Util.HashFile(filePath);

        _logger.LogDebug("New hash: {newHash}, old hash: {oldHash}", _newHash, oldHash);

        if (oldHash == _newHash)
        {
            _logger.LogInformation("Hashes are equal, no need to update, exiting...");
            return false;
        }

        return true;
    }

    private async ValueTask SaveToGitRepo(string gitDirectory)
    {
        string targetHashFile = Path.Combine(gitDirectory, "hash.txt");

        _fileUtilSync.DeleteIfExists(targetHashFile);

        await _fileUtil.WriteFile(targetHashFile, _newHash!);

        _gitUtil.AddIfNotExists(gitDirectory, targetHashFile);

        if (_gitUtil.IsRepositoryDirty(gitDirectory))
        {
            _logger.LogInformation("Changes have been detected in the repository, commiting and pushing...");

            string name = EnvironmentUtil.GetVariableStrict("NAME");
            string email = EnvironmentUtil.GetVariableStrict("EMAIL");
            string username = EnvironmentUtil.GetVariableStrict("USERNAME");
            string token = EnvironmentUtil.GetVariableStrict("TOKEN");

            _gitUtil.Commit(gitDirectory, "Updates hash for new version", name, email);

            await _gitUtil.Push(gitDirectory, username, token);
        }
        else
        {
            _logger.LogInformation("There are no changes to commit");
        }
    }
}
