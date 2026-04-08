using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Estragonia;

internal sealed class BclStorageFile : IStorageBookmarkFile
{
    private Uri? _path;

    public BclStorageFile(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    public FileInfo FileInfo { get; }

    public string Name
        => FileInfo.Name;

    public bool CanBookmark
        => true;

    public Uri Path
        => _path ??= BuildPath();

    public Task<StorageItemProperties> GetBasicPropertiesAsync()
    {
        if (FileInfo.Exists)
            return Task.FromResult(new StorageItemProperties(
                (ulong)FileInfo.Length,
                FileInfo.CreationTimeUtc,
                FileInfo.LastAccessTimeUtc
            ));

        return Task.FromResult(new StorageItemProperties());
    }

    public Task<IStorageFolder?> GetParentAsync()
    {
        var storageFolder = FileInfo.Directory is { } directory ? new BclStorageFolder(directory) : null;
        return Task.FromResult<IStorageFolder?>(storageFolder);
    }

    public Task<Stream> OpenReadAsync()
    {
        return Task.FromResult<Stream>(FileInfo.OpenRead());
    }

    public Task<Stream> OpenWriteAsync()
    {
        var stream = new FileStream(FileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Write);
        return Task.FromResult<Stream>(stream);
    }

    public Task<string?> SaveBookmarkAsync()
    {
        return Task.FromResult<string?>(FileInfo.FullName);
    }

    public Task ReleaseBookmarkAsync()
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync()
    {
        FileInfo.Delete();
        return Task.CompletedTask;
    }

    public Task<IStorageItem?> MoveAsync(IStorageFolder destination)
    {
        if (destination is BclStorageFolder storageFolder)
        {
            var newPath = System.IO.Path.Combine(storageFolder.DirectoryInfo.FullName, FileInfo.Name);
            FileInfo.MoveTo(newPath);

            return Task.FromResult<IStorageItem?>(new BclStorageFile(new FileInfo(newPath)));
        }

        return Task.FromResult<IStorageItem?>(null);
    }

    public void Dispose()
    {
    }

    private Uri BuildPath()
    {
        try
        {
            if (FileInfo.Directory is not null)
            {
                var builder = new UriBuilder
                {
                    Scheme = Uri.UriSchemeFile,
                    Host = string.Empty,
                    Path = FileInfo.FullName
                };
                return builder.Uri;
            }
        }
        catch (SecurityException)
        {
        }

        return new Uri(FileInfo.Name, UriKind.Relative);
    }
}
