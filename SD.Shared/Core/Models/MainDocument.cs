﻿namespace SD.Shared.Core.Models;

public enum DocumentType
{
    Principal = 1,
    MyProvider = 3,
    WishList = 4,
    WatchedList = 5,
    WatchingList = 9,
    Login = 10,
    MySuggestions = 11
}

public abstract class MainDocument : CosmosDocument
{
    protected MainDocument(DocumentType type)
    {
        Type = type;
    }

    protected MainDocument(string id, DocumentType type) : base($"{type}:{id}")
    {
        Type = type;
    }

    public DocumentType Type { get; set; }
}

/// <summary>
///     Public read and private write
/// </summary>
public abstract class ProtectedMainDocument : MainDocument
{
    private readonly DocumentType _type;

    protected ProtectedMainDocument(DocumentType type) : base(type)
    {
        this._type = type;
    }

    protected ProtectedMainDocument(string id, DocumentType type) : base($"{type}:{id}", type)
    {
        this._type = type;
    }

    public virtual void Initialize(string id)
    {
        SetIds($"{_type}:{id}");
    }
}

/// <summary>
///     Private read and write
/// </summary>
public abstract class PrivateMainDocument(DocumentType type) : MainDocument(type)
{
    private readonly DocumentType _type = type;

    public virtual void Initialize(string userId)
    {
        SetIds($"{_type}:{userId}");
    }
}