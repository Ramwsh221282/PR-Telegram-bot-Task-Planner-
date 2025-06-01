using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
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
    private readonly IExternalChatsRepository _repository;

    /// <summary>
    /// <inheritdoc cref="IUnitOfWork"/>
    /// </summary>
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

        // проверка на существование пользователя
        var existing = await _repository.Readable.GetExternalChatOwnerById(id, ct);
        if (existing.IsSuccess)
            return Result.Failure<ExternalChatOwner>(
                $"Обладатель внешнего чата с id: {useCase.Id} уже присутствует."
            );

        var memberId = ExternalChatMemberId.Dedicated(useCase.Id);
        var memberName = ExternalChatMemberName.Create(name).Value;
        ExternalChatOwner owner = new(memberId, memberName);

        // сохранение данных зарегистрированного пользователя в хранилище
        _repository.Writable.AddChatOwner(owner, _unitOfWork, ct);
        return owner;
    }
}
