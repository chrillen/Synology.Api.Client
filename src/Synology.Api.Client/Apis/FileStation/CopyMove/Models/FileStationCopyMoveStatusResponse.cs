﻿using Newtonsoft.Json;

namespace Synology.Api.Client.Apis.FileStation.CopyMove.Models
{
    public class FileStationCopyMoveStatusResponse
    {
        [JsonProperty("dest_folder_path")]
        public string DestFolderPath { get; set; }

        public bool Finished { get; set; }

        public string Path { get; set; }

        [JsonProperty("processed_size")]
        public decimal ProcessedSize { get; set; }

        public decimal Progress { get; set; }

        public decimal Total { get; set; }
    }
}
