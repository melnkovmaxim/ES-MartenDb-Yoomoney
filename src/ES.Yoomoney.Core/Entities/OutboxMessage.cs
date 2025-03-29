using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.Entities;

public sealed record OutboxMessage(
    Guid Id,
    string Type,
    string Content,
    DateTime CreatedAt,
    DateTime? ProcessedAt);