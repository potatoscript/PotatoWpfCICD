using PotatoWpfCICD.Services;

public class MainViewModel
{
    private readonly ICalculatorService _calculatorService;

    public MainViewModel(ICalculatorService calculatorService)
    {
        _calculatorService = calculatorService;
    }
}
