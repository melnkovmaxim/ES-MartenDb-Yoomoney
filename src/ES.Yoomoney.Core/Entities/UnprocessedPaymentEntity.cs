using ES.Yoomoney.Core.Abstractions;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Entities;

public sealed record UnprocessedPaymentEntity(string Id, Payment Payment) : IEntity;