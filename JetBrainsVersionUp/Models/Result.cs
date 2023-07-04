namespace JetBrainsVersionUp.Models;

internal class Result
{
    public bool Success { get; set; }
    public List<JetBrainApp> ResultApp { get; set; }
    public string Message { get; set; }

    public Result() : this(string.Empty)
    {
    }

    public Result(string errorMessage)
    {
        Success = false;
        Message = errorMessage;
        ResultApp = new List<JetBrainApp>();
    }

    public Result(List<JetBrainApp> jetBrainApp)
    {
        Success = true;
        Message = string.Empty;
        ResultApp = jetBrainApp;
    }
}
