﻿namespace Akros.Authorizer.Domain.Settings;

public interface IMongoDbSettings
{
    string? DatabaseName { get; set; }
    string? ConnectionString { get; set; }
}
