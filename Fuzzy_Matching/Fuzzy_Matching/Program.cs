using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace Fuzzy_Matching
{
    class Program
    {
        static void Main(string[] args)
        {
            string stringToCompae1 = string.Empty;
            string stringToCompae2 = string.Empty;

            string stringToCompaeForAlphaChk1 = string.Empty;
            string stringToCompaeForAlphaChk2 = string.Empty;

            bool numCheckFlag = false;

            //Perform Alpha check
            var alphaCheckresult = PerformAlphaCheck(stringToCompae1, stringToCompae2);
            stringToCompaeForAlphaChk1 = alphaCheckresult.Item1;
            stringToCompaeForAlphaChk2 = alphaCheckresult.Item2;

            ///when I determine that at least one string is empty, then I must conclude that no string match exists 
            ///and I must not continue utilizing the alpha check algorithm(confidence value 0).
            ///If No Alpha check go for Litteral check
            if (string.IsNullOrEmpty(stringToCompaeForAlphaChk1) || string.IsNullOrEmpty(stringToCompaeForAlphaChk2))
            {
                //Perform litteral check
                if (numCheckFlag)
                {
                    string digitString1 = new String(stringToCompae1.Where(Char.IsDigit).ToArray());
                    string digitString2 = new String(stringToCompae2.Where(Char.IsDigit).ToArray());

                    if (digitString1 == digitString2)
                    {
                        if (stringToCompae1 == stringToCompae2)
                        {
                            Console.WriteLine("String match is determined.");
                        }
                        else
                        {
                            if (IsStringMatchFound(stringToCompaeForAlphaChk1, stringToCompaeForAlphaChk2))
                            {
                                Console.WriteLine("String match is determined.");
                            }
                            else
                            {
                                Console.WriteLine("No string match exists.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No string match exists.");
                    }
                }
            } ///If Alpha check found
            else
            {
                if (IsStringMatchFound(stringToCompaeForAlphaChk1, stringToCompaeForAlphaChk2))
                {
                    Console.WriteLine("String match is determined.");
                }
                else
                {
                    Console.WriteLine("No string match exists.");
                }
            }

            Console.ReadLine();
        }

        public static Tuple<string, string> PerformAlphaCheck(string stringToCompae1, string stringToCompae2 )
        {
            string stringToCompaeForAlphaChk1 = string.Empty;
            string stringToCompaeForAlphaChk2 = string.Empty;

            //Check if there’s an alpha char. in both strings
            bool isAlphaCharExists1 = Regex.Matches(stringToCompae1, @"[a-zA-Z]").Count > 0 ? true : false;
            bool isAlphaCharExists2 = Regex.Matches(stringToCompae2, @"[a-zA-Z]").Count > 0 ? true : false;

            //Remove leading digits that exists before the alpha chars
            if (isAlphaCharExists1)
            {
                stringToCompaeForAlphaChk1 = RemoveLeadingDigits(stringToCompae1);
            }
            else
            {
                stringToCompaeForAlphaChk1 = string.Empty;
            }

            if (isAlphaCharExists2)
            {
                stringToCompaeForAlphaChk2 = RemoveLeadingDigits(stringToCompae2);
            }
            else
            {
                stringToCompaeForAlphaChk2 = string.Empty;
            }

            return Tuple.Create<string, string>(stringToCompaeForAlphaChk1, stringToCompaeForAlphaChk2);
        }

        public static string RemoveLeadingDigits(string inputString)
        {
            int alphaIndex = 0;

            while (!Char.IsLetter(inputString[alphaIndex]))
            {
                ++alphaIndex;
            }

            //As we are finding first alpha character from string better we need to start string from that character instead of removing digits
            //string stringToCompae21 = inputString.Substring(alphaIndex);
            //inputString = Regex.Replace(stringToCompae21, @"[\d-]", string.Empty) + inputString;

            inputString = inputString.Substring(alphaIndex);

            return inputString;
        }

        public static int GetComparisionRange(string inputString)
        {
            if (inputString.Length > 15)
            {
                return 4;
            }
            else if (inputString.Length < 15 && inputString.Length > 10)
            {
                return 3;
            }
            else
            {
                return 2;
            }
        }

        public static int CalculateWeightedScore(string inputString1, string inputString2)
        {
            int weightedScore = 0;
            int noOfComparedChars = 0;
            int noOfNonMatchedChars = 0;

            int comparisionRange = GetComparisionRange(inputString1);

            char[] inputstringArray1 = inputString1.ToCharArray();
            char[] inputstringArray2 = inputString2.ToCharArray();

            for (int i = 0; i < comparisionRange; i++)
            {
                noOfComparedChars++;
                if (inputstringArray1[i] == inputstringArray2[i])
                {
                    break;
                }
                else
                {
                    noOfNonMatchedChars++;
                }
            }

            noOfNonMatchedChars = noOfNonMatchedChars / 2;

            if (noOfComparedChars < 1)
            {
                weightedScore = 0;
            }
            else if (noOfComparedChars >= 1)
            {
                weightedScore = ((300 * noOfComparedChars) / inputString1.Length) + ((300 * noOfComparedChars) / inputString2.Length) + ((300 * (noOfComparedChars - noOfNonMatchedChars)) / noOfComparedChars);
            }

            return weightedScore;
        }

        public static bool IsStringMatchFound(string inputString1, string inputString2)
        {
            int configThreshold = 10;
            bool result = false;

            int weightedScoreForString1To2 = CalculateWeightedScore(inputString1, inputString2);
            int weightedScoreForString2To1 = CalculateWeightedScore(inputString2, inputString1);

            int averageWeightedScore = ((weightedScoreForString1To2 + weightedScoreForString2To1) / 2);

            if (configThreshold >= averageWeightedScore)
            {
                result = true;
            }

            return result;
        }
    }
}

