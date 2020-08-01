using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Utility
{
    public class NumberToWordsConverter
    {
        readonly Dictionary<int, string> _numberTextDictionary;
        public NumberToWordsConverter()
        {
            // Init Dictionary
            _numberTextDictionary = new Dictionary<int, string>
            {
                {0, "Zero"},
                {1, "One"},
                {2, "Two"},
                {3, "Three"},
                {4, "Four"},
                {5, "Five"},
                {6, "Six"},
                {7, "Seven"},
                {8, "Eight"},
                {9, "Nine"},
                {10, "Ten"},
                {11, "Eleven"},
                {12, "Twelve"},
                {13, "Thirteen"},
                {14, "Fourteen"},
                {15, "Fifteen"},
                {16, "Sixteen"},
                {17, "Seventeen"},
                {18, "Eighteen"},
                {19, "Nineteen"},
                {20, "Twenty"},
                {30, "Thirty"},
                {40, "Forty"},
                {50, "Fifty"},
                {60, "Sixty"},
                {70, "Seventy"},
                {80, "Eighty"},
                {90, "Ninety"},
                {100, "Hundred"},
                {1000, "Thousand"},
                {1000000, "Million"}
            };
        }
        private string ConvertNumberToWords(string inputNumber, string word = "", ConvertPart convertPart = ConvertPart.None)
        {
            double number;
            // Cents logic
            if (convertPart == ConvertPart.Cent)
            {
                //For example convert one digit cents (.,2) to 20 cents.
                if (inputNumber.Length == 1)
                    number = System.Convert.ToDouble(inputNumber) * 10;
                //Only calculate first two digit cents. 
                else
                    number = System.Convert.ToDouble(inputNumber.Substring(0, 2));
            }
            else
                number = System.Convert.ToDouble(inputNumber);

            if (number > 999999999)
                return "Maximum supported number is 999999999";

            //Read directly from dictionary.
            if (number <= 20)
            {
                return $"{word} {(number == 0 && !string.IsNullOrEmpty(word) ? string.Empty : _numberTextDictionary[(int)number])}".Trim().ToLower();
            }

            
            if (number < 100)
            {
                var numModTen = number % 10;
                //If number is multiply of 10
                if (numModTen == 0)
                    return $"{word} {_numberTextDictionary[(int)(number)]}".Trim().ToLower();
                //Convert to multiply of ten and second part directly read from dictionary.
                return $"{word} {_numberTextDictionary[(int)(number - (numModTen))]}-{_numberTextDictionary[(int)numModTen]}".Trim().ToLower();
            }

            //Recursive splitting number by 1000 until finish converting.
            var numLength = Math.Floor(Math.Log10(number));
            var floorPart1 = (int)Math.Pow(10, numLength - (numLength < 3 ? 0 : (numLength % 3)));
            var floorPart2 = (int)Math.Floor(number / floorPart1);
            return ConvertNumberToWords((number - (floorPart1 * floorPart2)).ToString(), $"{word} {ConvertNumberToWords(floorPart2.ToString())} {_numberTextDictionary[floorPart1]}").Trim().ToLower();
        }

        public string Convert(string number)
        {
            if (string.IsNullOrEmpty(number)) return string.Empty;

            try
            {
                var converter = new NumberToWordsConverter();
                //Remove space from input and also ',' char from end.
                number = number.Replace(" ", string.Empty).Trim(',');
                string result;
                //If having both dollars and cents part 
                if (number.Contains(","))
                {
                    var part = number.Split(',');
                    var dollars = converter.ConvertNumberToWords(part[0]);
                    var cents = converter.ConvertNumberToWords(part[1], convertPart: ConvertPart.Cent);
                    result = $"{dollars} {(dollars == "one" ? "dollar" : "dollars")} and {cents} {(cents == "one" ? "cent" : "cents")}";
                }
                //If having only dollars 
                else
                {
                    var dollars = converter.ConvertNumberToWords(number);
                    result = $"{dollars} {(dollars == "one" ? "dollar" : "dollars")}";
                }

                return result.ToLower();
            }
            catch (Exception)
            {
                return "Error In Converting";
            }
        }

    }
}
