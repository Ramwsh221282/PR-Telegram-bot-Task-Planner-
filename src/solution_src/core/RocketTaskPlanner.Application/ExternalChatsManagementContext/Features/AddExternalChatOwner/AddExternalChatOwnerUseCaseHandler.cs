using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;

/// <summary>
/// Обработчик для <inheritdoc cref="AddExternalChatOwnerUseCase"/>
/// </summary>
public sealed class AddExternalChatOwnerUseCaseHandler
    : IUseCaseHandler<AddExternalChatOwnerUseCase, ExternalChatOwner>
{
    /// <summary>
    /// <inheritdoc cref="IExternalChatsReadableRepository"/>
    /// </summary>
    private readonly IExternalChatsWritableRepository _repository;

    public AddExternalChatOwnerUseCaseHandler(IExternalChatsWritableRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ExternalChatOwner>> Handle(
        AddExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    )
    {
        string name = useCase.Name;
        var memberId = ExternalChatMemberId.Dedicated(useCase.Id);
        var memberName = ExternalChatMemberName.Create(name).Value;
        ExternalChatOwner owner = new(memberId, memberName);
        var ownerResult = await _repository.AddChatOwner(owner, ct);
        
        return ownerResult.IsFailure ? Result.Failure<ExternalChatOwner>(ownerResult.Error) : owner;
    }
}
