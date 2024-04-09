namespace GFlat.Loggers;

public interface ILogger
{
    string Id { get; }


    void Write(LogKind logKind, string text);

    void WriteLine();

    void WriteLine(LogKind logKind, string text);

    void Flush();
}