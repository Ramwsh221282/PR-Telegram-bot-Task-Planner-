using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Application.ExternalChatsManagementContext.Features.RemoveExternalChatTheme;

public sealed class RemoveExternalChatThemeUseCaseHandler
    : IUseCaseHandler<RemoveExternalChatThemeUseCase, ExternalChat>
{
    private readonly IExternalChatsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveExternalChatThemeUseCaseHandler(
        IExternalChatsRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExternalChat>> Handle(
        RemoveExternalChatThemeUseCase useCase,
        CancellationToken ct = default
    )
    {
        var owner = await _repository.Readable.GetExternalChatOwnerById(useCase.userId, ct);
        if (owner.IsFailure)
            return Result.Failure<ExternalChat>(owner.Error);

        var themeChat = owner.Value.GetChildChat(
            ExternalChatId.Dedicated(useCase.chatId),
            ExternalChatId.Dedicated(useCase.themeId)
        );
        if (themeChat.IsFailure)
            return Result.Failure<ExternalChat>(themeChat.Error);

        Result removing = _repository.Writable.RemoveThemeChat(themeChat.Value, _unitOfWork, ct);
        return removing.IsFailure ? Result.Failure<ExternalChat>(removing.Error) : themeChat;
    }
}
