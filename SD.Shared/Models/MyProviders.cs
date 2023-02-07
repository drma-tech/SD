﻿using SD.Shared.Core.Models;

namespace SD.Shared.Models
{
    public class MyProviders : PrivateMainDocument
    {
        public MyProviders() : base(DocumentType.MyProvider)
        {
        }

        public List<MyProvidersItem> Items { get; set; } = new();

        public override bool HasValidData()
        {
            return Items.Any();
        }
    }

    public sealed class MyProvidersItem
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? logo { get; set; }
    }
}