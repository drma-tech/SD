﻿namespace SD.Shared.Core.Models
{
    public abstract class LogDocument : CosmosDocument
    {
        protected LogDocument(string id, string key) : base(id, key)
        {
        }
    }
}