using System;

public enum MembershipFunctionType
{
    Triangular,
    Trapezoidal,
    Parabolic,
    Gaussian
}

public class FuzzySet
{
    public string Name { get; set; }
    public MembershipFunctionType Type { get; set; }
    public double[] Parameters { get; set; }

    public FuzzySet(string name, MembershipFunctionType type, double[] parameters)
    {
        Name = name;
        Type = type;
        Parameters = parameters;
    }

    public double GetMembership(double x)
    {
        return Type switch
        {
            MembershipFunctionType.Triangular => Triangular(x, Parameters[0], Parameters[1], Parameters[2]),
            MembershipFunctionType.Trapezoidal => Trapezoidal(x, Parameters[0], Parameters[1], Parameters[2], Parameters[3]),
            MembershipFunctionType.Parabolic => Parabolic(x, Parameters[0], Parameters[1]),
            MembershipFunctionType.Gaussian => Gaussian(x, Parameters[0], Parameters[1]),
        };
    }

    private static double Triangular(double x, double a, double b, double c) =>
        (x <= a || x >= c) ? 0 : (x == b ? 1 : (x < b ? (x - a) / (b - a) : (c - x) / (c - b)));

    private static double Trapezoidal(double x, double a, double b, double c, double d) =>
        (x <= a || x >= d) ? 0 : (x >= b && x <= c ? 1 : (x > a && x < b ? (x - a) / (b - a) : (d - x) / (d - c)));

    private static double Parabolic(double x, double a, double b)
    {
        if (x < a || x > b) return 0;
        double mid = (a + b) / 2;
        return 1 - Math.Pow((x - mid) / (mid - a), 2);
    }

    private static double Gaussian(double x, double mean, double sigma) =>
        Math.Exp(-Math.Pow(x - mean, 2) / (2 * Math.Pow(sigma, 2)));
}
