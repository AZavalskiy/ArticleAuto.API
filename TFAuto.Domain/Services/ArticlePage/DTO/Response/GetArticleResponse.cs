﻿using Newtonsoft.Json;
using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetArticleResponse
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Text { get; set; }

    [JsonProperty("author")]
    public string UserName { get; set; }

    public string LastUserWhoUpdated { get; set; }

    public List<TagResponse> Tags { get; set; } = new();

    public GetFileResponse Image { get; set; }

    public string CreatedTimeUtc { get; set; }

    public string LastUpdatedTimeUtc { get; set; }
}
