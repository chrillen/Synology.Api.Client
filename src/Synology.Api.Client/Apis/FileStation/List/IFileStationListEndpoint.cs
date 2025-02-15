﻿using System.Threading.Tasks;
using Synology.Api.Client.Apis.FileStation.List.Models;

namespace Synology.Api.Client.Apis.FileStation.List
{
    public interface IFileStationListEndpoint
    {
        Task<FileStationListResponse> ListAsync(FileStationListRequest fileStationListRequest);
        Task<FileStationListShareResponse> ListSharesAsync();
    }
}
