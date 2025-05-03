namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognitions;

/// <summary>
/// Dto модель для работы с распознаванием
/// </summary>
public abstract record TimeRecognitionTicket;

/// <summary>
/// Dto модель периодичного распознавания
/// </summary>
/// <param name="Input">Текст</param>
public sealed record PeriodicTimeRecognitionTicket(string Input) : TimeRecognitionTicket;

/// <summary>
/// Dto модель не периодичного распознавания
/// </summary>
/// <param name="Input">Текст</param>
public sealed record SingleTimeRecognitionTicket(string Input) : TimeRecognitionTicket;

/// <summary>
/// Неизвестное распознавание (при ошибках в распознавании)
/// </summary>
public sealed record UnknownTimeRecognitionTicket : TimeRecognitionTicket;
