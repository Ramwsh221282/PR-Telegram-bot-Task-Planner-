using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.RecognitionStrategies;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Recognizers;

namespace RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade.Services;

public sealed class TimeRecognitionRecognizersService
{
    private readonly Dictionary<Type, ITimeRecognizer> _recognizers = [];
    private readonly Dictionary<Type, IRecognitionStrategy> _strategies = [];

    public TimeRecognitionRecognizersService()
    {
        _recognizers.Add(typeof(DayOfWeekRecognizer), new DayOfWeekRecognizer());
        _recognizers.Add(typeof(MonthRecognizer), new MonthRecognizer());
        _recognizers.Add(typeof(PeriodicTimeRecognizer), new PeriodicTimeRecognizer());
        _recognizers.Add(typeof(RelativeDateRecognizer), new RelativeDateRecognizer());
        _recognizers.Add(typeof(TimeRecognizer), new TimeRecognizer());
        _strategies.Add(typeof(RawStringRecognitionStrategy), new RawStringRecognitionStrategy());
        _strategies.Add(typeof(ChunkRecognitionStrategy), new ChunkRecognitionStrategy());
    }

    public ITimeRecognizer GetRecognizer<TRecognizer>()
        where TRecognizer : ITimeRecognizer => _recognizers[typeof(TRecognizer)];

    public IRecognitionStrategy GetStrategy<TStrategy>()
        where TStrategy : IRecognitionStrategy => _strategies[typeof(TStrategy)];
}
