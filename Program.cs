using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobInfo.ApplicationConfiguration;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
    .Build();

AccountConfiguration accountConfig = new();
config.Bind("AccountConfiguration", accountConfig);

ShowAllBlobs(accountConfig.Accounts.First(), accountConfig.Accounts.First().Containers.First());

void ShowAllBlobs(StorageAccount account, Container container)
{
    BlobContainerClient containerClient = new(account.ConnectionString, container.Name);

    List<BlobCsvRecord> blobCsvRecords = new();

    foreach (BlobItem blob in containerClient.GetBlobs().OrderBy(x => x.Name))
    {
        blobCsvRecords.Add(new()
        {
            Name = blob.Name,
            StorageClass = blob.Properties.BlobType?.ToString() ?? "[unknown type]",
            Size = blob.Properties.ContentLength ?? -1,
            UploadDate = blob.Properties.CreatedOn?.LocalDateTime ?? default
        });
    }

    using (StreamWriter writer = new("blobs.csv"))
    using (CsvWriter csv = new(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecords(blobCsvRecords);
    }
}

class BlobCsvRecord
{
    public string Name { get; set; } = default!;
    public string StorageClass { get; set; } = default!;
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }
}

class BlobCsvRecordMap : ClassMap<BlobCsvRecord>
{
    public BlobCsvRecordMap()
    {
        Map(x => x.Name);
        Map(x => x.StorageClass);
        Map(x => x.Size);
        Map(x => x.UploadDate);
    }
}