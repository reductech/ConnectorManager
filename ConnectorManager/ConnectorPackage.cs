﻿using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;

namespace Reductech.EDR.ConnectorManagement
{

/// <summary>
/// Stores connector metadata.
/// </summary>
public record ConnectorMetadata(string Id, string Version) { }

/// <summary>
/// Connector metadata which includes a downloaded connector package.
/// </summary>
public sealed record ConnectorPackage
    (ConnectorMetadata Metadata, PackageArchiveReader Package) : IDisposable
{
    /// <summary>
    /// Extract the connector to the destination directory.
    /// </summary>
    /// <param name="fileSystem">The file system to use.</param>
    /// <param name="destination">The destination directory.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task Extract(
        IFileSystem fileSystem,
        string destination,
        CancellationToken ct = default)
    {
        var files = await Package.GetPackageFilesAsync(PackageSaveMode.Files, ct);

        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();
            var entry       = Package.GetEntry(file);
            var extractPath = Path.Combine(destination, entry.Name);
            entry.ExtractToFile(fileSystem, extractPath);
        }
    }

    /// <inheritdoc />
    public void Dispose() => Package.Dispose();
}

}