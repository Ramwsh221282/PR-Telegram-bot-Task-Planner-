using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.AddExternalChatOwner;

/// <summary>
/// Создание обладателя внешнего чата.
/// </summary>
public sealed class AddExternalChatOwnerUseCaseHandler
    : IUseCaseHandler<AddExternalChatOwnerUseCase, ExternalChatOwner>
{
    private readonly IExternalChatsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddExternalChatOwnerUseCaseHandler(
        IExternalChatsRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExternalChatOwner>> Handle(
        AddExternalChatOwnerUseCase useCase,
        CancellationToken ct = default
    )
    {
        long id = useCase.Id;
        string name = useCase.Name;

        Result<ExternalChatOwner> existing = await _repository.Readable.GetExternalChatOwnerById(
            id,
            ct
        );
        if (existing.IsSuccess)
            return Result.Failure<ExternalChatOwner>(
                $"Обладатель внешнего чата с id: {useCase.Id} уже присутствует."
            );

        ExternalChatMemberId memberId = ExternalChatMemberId.Dedicated(useCase.Id);
        ExternalChatMemberName memberName = ExternalChatMemberName.Create(name).Value;
        ExternalChatOwner owner = new(memberId, memberName);

        _repository.Writable.AddChatOwner(owner, _unitOfWork, ct);
        return owner;
    }
}
