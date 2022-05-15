namespace SRP.ControlDigit
{
    public static class Extensions
    {
        public static int SumOfEvenNumbers(this long numeric)
        {
            int sumOfEvenNumbers = 0; int index = 1;
            while (numeric >= 1)
            {
                var number = (int)(numeric % 10);
                sumOfEvenNumbers += index % 2 == 0 ? number : 0;
                numeric /= 10; index++;
            }
            return sumOfEvenNumbers;
        }

        public static int SumOfOddNumbers(this long numeric)
        {
            int sumOfOddNumbers = 0; int index = 1;
            while (numeric >= 1)
            {
                var number = (int)(numeric % 10);
                sumOfOddNumbers += index % 2 == 1 ? number : 0;
                numeric /= 10; index++;
            }
            return sumOfOddNumbers; 
        }

        public static int SumOfOddNumbersWithThemMultiplicationsOnTwo(this long numeric)
        {
            int index = 1; int sumOfNumbers = 0;
            while (numeric > 0)
            {
                var n = (int)(numeric % 10) * 2;
                sumOfNumbers = index % 2 == 1 ? sumOfNumbers += n > 9 ? (n / 10) + (n % 10) : n : sumOfNumbers;
                numeric /= 10; index++;
            }
            return sumOfNumbers;
        }

        public static int SumOfNumbersByDescPositionMultiplications(this long numeric)
        {
            int sumOfNumbersByDescPositionMultiplications = 0; int index = 2;
            while (numeric >= 1)
            {
                var number = (int)(numeric % 10);
                sumOfNumbersByDescPositionMultiplications += number * index;
                numeric /= 10; index++;
            }
            return sumOfNumbersByDescPositionMultiplications;
        }
    }

    public static class ControlDigitAlgo
    {
        public static int Upc(long numeric)
        {
            var upcNumber = numeric.SumOfEvenNumbers() + numeric.SumOfOddNumbers() * 3;
            var reminder = upcNumber % 10;
            return reminder == 0 ? 0 : 10 - reminder;
        }

        public static int Isbn10(long numeric)
        {
            var isbn10Number = (11 - (numeric.SumOfNumbersByDescPositionMultiplications() % 11)) % 11;
            return isbn10Number == 10 ? 'X' : isbn10Number.ToString()[0];
        }

        public static int Luhn(long numeric)
        {
            var luhnNumber = numeric.SumOfEvenNumbers() + numeric.SumOfOddNumbersWithThemMultiplicationsOnTwo();
            return 10 - luhnNumber % 10 > 9 ? luhnNumber % 10 : 10 - luhnNumber % 10;
        }
    }
}
