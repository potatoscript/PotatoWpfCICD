using PotatoWpfCICD.Services;

public class CalculatorService : ICalculatorService
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Multiply(int a, int b) => a * b;

    public int Divide(int a, int b)
    {
        if (b == 0)
            throw new ArgumentException("Divider cannot be zero");
        return a / b;
    }
}
