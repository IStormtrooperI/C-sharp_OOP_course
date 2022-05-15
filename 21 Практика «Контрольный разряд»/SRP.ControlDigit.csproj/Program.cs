using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRP.ControlDigit;

namespace SRP.ControlDigit
{
	class Program
	{
		static void Main(string[] args)
		{
			var test = new ControlDigitAlgo_Tests();
			test.TestUpc();
			test.TestIsbn10();
			test.TestLuhn();
		}
	}
}
