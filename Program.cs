using System;
using System.Collections.Generic;
using System.Globalization;

namespace LinearAndPolynomial
{
	
	interface IFunction
	{
		void SetCoefficientsFromConsole(Func<string, double> reader);
		void DisplayCoefficients();
		double Calculate(double x);
	}

	
	abstract class FunctionBase : IFunction
	{
		protected double _a1;
		protected double _a0;

		protected double A1 => _a1;
		protected double A0 => _a0;

		public FunctionBase() { }
		public FunctionBase(double a1, double a0)
		{
			_a1 = a1;
			_a0 = a0;
		}

		public abstract void SetCoefficientsFromConsole(Func<string, double> reader);
		public abstract void DisplayCoefficients();
		public abstract double Calculate(double x);

		protected string FormatDouble(double v)
		{
			return v.ToString("N2", CultureInfo.InvariantCulture);
		}
	}

	
	class LinearFunction : FunctionBase
	{
		public LinearFunction() { }
		public LinearFunction(double a1, double a0) : base(a1, a0) { }

		public override void SetCoefficientsFromConsole(Func<string, double> reader)
		{
			_a1 = reader("Введіть коефіцієнт a1: ");
			_a0 = reader("Введіть коефіцієнт a0: ");
		}

		public override void DisplayCoefficients()
		{
			string result = string.Empty;
			bool first = true;

			double[] coeffs = { _a1, _a0 };
			int[] exps = { 1, 0 };
			for (int i = 0; i < coeffs.Length; i++)
			{
				double c = coeffs[i];
				if (Math.Abs(c) < 1e-12) continue;

				string sign = first ? (c < 0 ? "-" : string.Empty) : (c < 0 ? " - " : " + ");
				double abs = Math.Abs(c);
				string term = exps[i] == 0 ? FormatDouble(abs) : (Math.Abs(abs - 1.0) < 1e-12 ? "x" : FormatDouble(abs) + "x");
				result += sign + term;
				first = false;
			}

			if (string.IsNullOrEmpty(result)) result = "0";
			Console.WriteLine($"Лінійна функція: f(x) = {result}");
		}

		public override double Calculate(double x)
		{
			return _a1 * x + _a0;
		}
	}

	
	class Polynomial : FunctionBase
	{
		private double _a2;
		private double _a3;
		private double _a4;

		public Polynomial() { }
		public Polynomial(double a4, double a3, double a2, double a1, double a0) : base(a1, a0)
		{
			_a4 = a4; _a3 = a3; _a2 = a2;
		}

		public override void SetCoefficientsFromConsole(Func<string, double> reader)
		{
			_a4 = reader("Введіть коефіцієнт a4: ");
			_a3 = reader("Введіть коефіцієнт a3: ");
			_a2 = reader("Введіть коефіцієнт a2: ");
			_a1 = reader("Введіть коефіцієнт a1: ");
			_a0 = reader("Введіть коефіцієнт a0: ");
		}

		public override void DisplayCoefficients()
		{
			double[] coeffs = { _a4, _a3, _a2, _a1, _a0 };
			int[] exps = { 4, 3, 2, 1, 0 };

			string result = string.Empty;
			bool first = true;
			for (int i = 0; i < coeffs.Length; i++)
			{
				double c = coeffs[i];
				if (Math.Abs(c) < 1e-12) continue;

				string sign = first ? (c < 0 ? "-" : string.Empty) : (c < 0 ? " - " : " + ");
				double abs = Math.Abs(c);
				string term;
				if (exps[i] == 0) term = FormatDouble(abs);
				else if (exps[i] == 1) term = (Math.Abs(abs - 1.0) < 1e-12) ? "x" : FormatDouble(abs) + "x";
				else term = FormatDouble(abs) + $"x^{exps[i]}";

				result += sign + term;
				first = false;
			}

			if (string.IsNullOrEmpty(result)) result = "0";
			Console.WriteLine($"Многочлен: f(x) = {result}");
		}

		public override double Calculate(double x)
		{
			double x2 = x * x;
			double x3 = x2 * x;
			double x4 = x3 * x;
			return _a4 * x4 + _a3 * x3 + _a2 * x2 + _a1 * x + _a0;
		}
	}

	internal static class Program
	{
		private static double ReadDouble(string prompt)
		{
			double value;
			Console.Write(prompt);
			while (!double.TryParse(Console.ReadLine(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value))
			{
				Console.WriteLine("Неправильний формат. Спробуйте ще раз (використовуйте крапку як десятковий роздільник)." );
				Console.Write(prompt);
			}
			return value;
		}

		static void Main()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			Console.WriteLine("=== ЛІНІЙНА ФУНКЦІЯ (введіть коефіцієнти) ===");
			IFunction linear = new LinearFunction();
			linear.SetCoefficientsFromConsole(ReadDouble);
			linear.DisplayCoefficients();

			Console.WriteLine("\n=== МНОГОЧЛЕН (введіть коефіцієнти) ===");
			IFunction poly = new Polynomial();
			poly.SetCoefficientsFromConsole(ReadDouble);
			poly.DisplayCoefficients();

			double x = ReadDouble("\nВведіть значення x: ");

			Console.WriteLine($"\nЗначення лінійної функції в точці x={x}: {linear.Calculate(x):N4}");
			Console.WriteLine($"Значення многочлена в точці x={x}: {poly.Calculate(x):N4}");

			Console.WriteLine("\n=== Поліморфізм: виклик через інтерфейс IFunction ===");
			List<IFunction> funcs = new List<IFunction> { linear, poly };
			for (int i = 0; i < funcs.Count; i++)
			{
				Console.WriteLine($"\nФункція #{i + 1} (через IFunction):");
				funcs[i].DisplayCoefficients();
				Console.WriteLine($"f({x}) = {funcs[i].Calculate(x):N4}");
			}
		}
	}
}
