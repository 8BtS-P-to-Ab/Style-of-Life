using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mass_Renamer
{
    public static class Splitter
    {
        //splitter is for storing static functions
        //many of the functions here are unused but may be used if additional functions are added to the translator/a new translator is added
        //if you update anything here, make sure you update the debugs for the time being. Will get around to an auto-updater sometime.

        //from https://stackoverflow.com/a/13368402
        /// <summary>
        /// string.split but as a function. (Style of Life function).
        /// returns null on error.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="count"></param>
        /// <param name="wordDelimiter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] GetWords(
        this string input,
        int count = -1,
        string[] wordDelimiter = null,
        StringSplitOptions options = StringSplitOptions.None)
        {
            if (string.IsNullOrEmpty(input))
            {
                System.Diagnostics.Debug.WriteLine("from getWords method, paramater is not set: "
                    + String.IsNullOrEmpty(input) + " (@line 34, Splitter.cs)");
                return null;
            };

            if (count < 0)
                return input.Split(wordDelimiter, options);

            string[] words = input.Split(wordDelimiter, count + 1, options);
            if (words.Length <= count)
                return words;   // not so many words found

            // remove last "word" since that contains the rest of the string
            Array.Resize(ref words, words.Length - 1);

            return words;
        }

        /// <summary>
        /// Gets and returns the index of the left most letter of the letters to seach for.
        /// e.g. word = "test" and letters = "te" would return [0]=1, [1]=0.
        /// returns an empty array of length of 1 on error.
        /// </summary>
        /// <param name="letters"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static int[] FindLetterPos(string letters, string word)
        {
            //initialisers                                                                                  
            int[] pos = new int[1];                                                                         //initialize a new array to store the index of each occurance of the string of letters
                                                                                                            //
            if (!String.IsNullOrEmpty(letters) && !String.IsNullOrEmpty(word))                              //
            {                                                                                               //Tif the string of letters to search for and the input word to search through are not null
                String[] listOfChars = GetWords(letters);                                                   //|--split each letter to search for into an array called "listOfChars"
                int length = listOfChars.Length;                                                            //|--get how many characters are in the list of characters to search for
                Array.Resize(ref pos, length);                                                              //|--resize the result array to the length of 'length'
                                                                                                            //|
                for (int i = 0; i < length; i++)                                                            //|
                {                                                                                           //|-Tloop for 'length' times 
                    pos[i] = word.IndexOf(listOfChars[i], StringComparison.OrdinalIgnoreCase);              //|-|----find the leftest letter to search and retrun its possition as pos
                }                                                                                           //|-|L)
            }
            else
            {                                                                                        //\c)otherwise if either letters or words are null
                System.Diagnostics.Debug.WriteLine("from findLetterPos method, paramater is not set: "      //|
                    + !String.IsNullOrEmpty(letters) + " or " + !String.IsNullOrEmpty(word)                 //|
                    + " is false! (@line 62, Splitter.cs)");                                                //|--log an error
            }                                                                                               //|e)
                                                                                                            //
            return pos;                                                                                     //retrurn pos
        }

        /// <summary>
        /// Get the possition of the left most occuring letter of a word.
        /// Same as "word.indexOf(letter, StringComparison.OrdinalIgnoreCase)", style of life function.
        /// Returns -1 on error.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static int FindSingleLetterPos(string letter, string word)
        {//shouldn't need to comment this
            int pos = 0;

            if (!String.IsNullOrEmpty(letter) && !String.IsNullOrEmpty(word))
            {
                pos = word.IndexOf(letter, StringComparison.OrdinalIgnoreCase);//find the leftest letter to search and retrun its possition as letterPos

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("from findLetterPos method, paramater is not set: "
                    + !String.IsNullOrEmpty(letter) + " or " + !String.IsNullOrEmpty(word) +
                    " is false! (@line 87, Splitter.cs)");
                return -1;
            }
            return pos;
        }

        /// <summary>
        /// Counts the amount of occurances of the selected characters in a word.
        /// E.g. when searching for vowels (a e i o u) in the word 'tosteesotia', it will return in an int array: [0]=1, [1]=2, [2]=1, [3]=2, [4]=0.
        /// Returns nothing on error.
        /// </summary>
        /// <param name="letters"></param>
        /// <param name="word"></param>
        /// <returns></returns>
		public static int[] CountLetterReturnAmount(string letters, string word)
        {
            int count = 0;                                                                                              //initialize var to hold the amount of times an occurance match is found
            int test = -1;                                                                                              //might be replaceable with already existing variable 'i' in for loop
            int charReturn = 0;                                                                                         //initialize var to hold the index of each occurance match 
            int[] charCount = new int[1];                                                                               //
            int wordLength = word.Length;                                                                               //get how large the word is
                                                                                                                        //
            if (!String.IsNullOrEmpty(letters) && !String.IsNullOrEmpty(word))                                          //
            {                                                                                                           //Tif the string of letters to search for and the input word to search through are not null
                String[] listOfChars = GetWords(letters);                                                               //|--split the letters to a list of characters
                int length = listOfChars.Length;                                                                        //|--get how many characters there are to search
                Array.Resize(ref charCount, length);                                                                    //|--set the charCount array to the said length which is where all of the counts will end for each character searched
                                                                                                                        //|
                for (int charI = 0; charI < length; charI++)                                                            //|
                {                                                                                                       //|-Tloop untill length 'length' is reached, the loop number is an id to which character to search (i.e. listOfChars[charI])
                    count = 0;                                                                                          //|-|----reset count
                    charReturn = -1;                                                                                    //|-|----reset charReturn
                    test = -1;                                                                                          //|-|----reset test
                    string letter = listOfChars[charI];                                                                 //|-|----get current letter to count
                                                                                                                        //| |
                    for (int i = 0; i < wordLength; i++)                                                                //| |
                    {                                                                                                   //|-|---Tloop untill the itteration is at the last word
                        test++;                                                                                         //|-|---|----itterate test
                        charReturn = word.IndexOf(letter, i, StringComparison.OrdinalIgnoreCase);                       //|-|---|----get the index of the lest-st letter starting from the i-th possition as charReturn
                        if (test == charReturn)                                                                         //| |   |
                        {                                                                                               //|-|---|---Tif the current character being checked returns a match
                            count++;                                                                                    //|-|---|---|----itterate the amount of times this character has matched
                            charReturn = -1;                                                                            //|-|---|---|----reset charReturn
                            continue;//may be unnecessary                                                               //| |   |   |
                        }                                                                                               //|-|---|---|e)
                    }                                                                                                   //|-|---|L)
                    charCount[charI] = count;                                                                           //|-|----return the amount of times a match was found for this current letter check
                                                                                                                        //|-|
                }                                                                                                       //|-|L)
            }
            else
            {                                                                                                    //\c)otherwise if the string of letters to search for OR the input word to search through are null
                System.Diagnostics.Debug.WriteLine("from countLetterReturnAmount method, paramater is not set: "        //|
                    + !String.IsNullOrEmpty(letters) + " or " + !String.IsNullOrEmpty(word)                             //|
                    + " is false! (@line 139, Splitter.cs)");                                                           //|--log the error
            }                                                                                                           //|e)
                                                                                                                        //
            return charCount;                                                                                           //retrun the arrat
        }

        /// <summary>
        /// Returns which character, that is searched for, is at the start of the word, otherwise returns "none" if found none or "error" if an error occured.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string FindGroupOfCharacters(String chars, String word)
        {
            if (!String.IsNullOrEmpty(chars) || !String.IsNullOrEmpty(word))
            {                                                                                                       //Tif the string of chars to search for and the input word to search through are not null
                int i = -1;                                                                                         //|--initialize a variable for tracking foreach loop amount
                int length = chars.Length;                                                                          //|--get the amount of characters to search for
                string[] charStart = new string[length];                                                            //|--initialize an array of 'length' to store each occurance count
                string[] listOfChars = GetWords(chars);                                                             //|--get the list of characters to search for
                                                                                                                    //|
                foreach (var myChar in listOfChars)                                                                 //|
                {                                                                                                   //|-Tforeach character from the list of characters to search by
                    i++;                                                                                            //|-|---itterate i
                                                                                                                    //|-|
                    if (CharAtIndex(myChar, word, 0, "==") == true)                                                 //|-|
                    {                                                                                               //|-|--Tif the currnet character is at the start of the word
                        charStart[i] = "true";                                                                      //|-|--|----set that the i-th character is the first character in the word
                    }
                    else
                    {                                                                                        //|-|--\c)otherwise, if the current character is not at the start of the word
                        charStart[i] = "false";                                                                     //|-|--|----set that the i-th character is not the first character in the word
                    }                                                                                               //|-|--|e)
                }                                                                                                   //|-|L)
                                                                                                                    //|
                for (int iter = 0; iter < length; iter++)                                                           //|
                {                                                                                                   //|-Tloop until length of 'length' is reached
                    if (charStart[iter] != null)                                                                    //|-|
                    {                                                                                               //|-|--Tif the current character has any result
                        if (charStart[iter].Equals("true"))                                                         //|-|--|
                        {                                                                                           //|-|--|---Tif the current character has the result of true
                            return listOfChars[iter];                                                               //|-|--|---|-----return the character which returned true
                        }                                                                                           //|-|--|---|e)
                                                                                                                    //|-|--|
                    }
                    else
                    {                                                                                        //|-|--\c)otherwise, if the current character has no result
                        System.Diagnostics.Debug.WriteLine("from FindGroupOfCharacters:" +                          //| |  |
                            "never got a reult for character " + iter + " (@line 183, Splitter.cs)");               //|-|--|----report an error occured
                        return "error";                                                                             //|-|--|----return error for error handling
                                                                                                                    //|-|--|
                    }                                                                                               //|-|--|e)
                }                                                                                                   //|-|L)
                System.Diagnostics.Debug.WriteLine("from FindGroupOfCharacters:" +                                  //|
                    "could not find any matches (@line 189, Splitter.cs)");                                         //|--return that a minor error occured
                return "none";                                                                                      //|--if all data points in the array are "false" then return that the first character is not any of the searched characters
                                                                                                                    //|
            }
            else
            {                                                                                                //\c)otherwise, if the string of chars to search for OR the input word to search through are null
                System.Diagnostics.Debug.WriteLine("from findGroupOfCharacters method, paramater is not set: "      //|
                    + !String.IsNullOrEmpty(chars) + " or " + !String.IsNullOrEmpty(word)                           //|
                    + " is false! (@line 195, Splitter.cs)");                                                       //|--report an error occured
                return "error";                                                                                     //|--return that an error occured
            }                                                                                                       //|e)
        }

        /// <summary>
        /// Checks if the character is at (==), before, after (>) at the index to check.
        /// </summary>
        /// <param name="aChar"></param>
        /// <param name="word"></param>
        /// <param name="indexToCheck"></param>
        /// <param name="operatorType"></param>
        /// <returns></returns>
		public static bool CharAtIndex(String aChar, String word, int indexToCheck, string operatorType)
        {
            if (!String.IsNullOrEmpty(aChar) && !String.IsNullOrEmpty(word) && (!indexToCheck.Equals(null)) && !String.IsNullOrEmpty(operatorType))
            {                                                                               //Tif any of the parameters are not null
                int index = word.IndexOf(aChar, StringComparison.OrdinalIgnoreCase);        //|--get the lefter most character of the word
                                                                                            //|
                if (operatorType == "==")                                                   //|
                {                                                                           //|-Tif operator type is "=="
                    if (index == indexToCheck && index != -1)                               //|-|--Tif the left most character match is == the index to check and the index != -1
                    { return true; }                                                        //|-|--|----return true
                    else                                                                    //|-|--\c)otherwise
                    { return false; }                                                       //|-|--|e)--return false
                }
                else if (operatorType == "<")
                {                                           //|-\c)otherwise, if the operator type is <
                    if (index < indexToCheck)                                               //|-|--Tif the left most character match is < the index to check
                    { return true; }                                                        //|-|--|----return true
                    else                                                                    //|-|--\c)otherwise
                    { return false; }                                                       //|-|--|e)--return false
                }
                else if (operatorType == ">")
                {                                           //|-\c)otherwise, if the operator type is <
                    if (index > indexToCheck && index != -1)                                //|-|--Tif the left most character match is < the index to check
                    { return true; }                                                        //|-|--|----return true
                    else                                                                    //|-|--\c)otherwise
                    { return false; }                                                       //|-|--|e)--return false
                }
                else
                {                                                                    //|-\c)otherwise
                    System.Diagnostics.Debug.WriteLine("from charAtIndex method:" +         //| |
                        " could not read operator (@line246, Splitter.cs)");                //|-|---report error
                    return false;                                                           //|-|---return false
                }                                                                           //|-|e)
            }
            else
            {                                                                        //\c)otherwise if any of the parameters are null
                System.Diagnostics.Debug.WriteLine("from charAtIndex method," +             //|
                    " paramater is not set: " + !String.IsNullOrEmpty(aChar) +              //|
                    " or " + !String.IsNullOrEmpty(word) + " or " +                         //|
                    !indexToCheck.Equals(null) + " or " +                                   //|
                    !String.IsNullOrEmpty(operatorType) + " is false and not" +             //|
                    " properly set! (@line 256, Splitter.cs)");                             //|--report error
                return false;                                                               //|--retirm false
            }                                                                               //|e)
        }

        /// <summary>
        /// Returns a booleen result of if the number in question is in the int array.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool NumberInArray(int number, int[] array)
        {
            if (!number.Equals(null) && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {//simply; itterate through the array fully, checking each number
                    if (array[i] == number)
                    {
                        return true;
                    }

                }

                return false;
            }
            else
            {
                if (number.Equals(null) && array.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("from NumberInArray: 'number'" +
                        "paramater is not optional (@line 285, Splitter.cs)");
                }
                else if (!number.Equals(null) && array.Length <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("from NumberInArray: 'array'" +
                        "paramater is either missing or its length = 0 (@line 290, " +
                        "Splitter.cs)");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("from NumberInArray: both" +
                    " paramaters are invalid. Array length must be > 0 and number is" +
                    "not optional. (@line 295, Splitter.cs)");
                }

                return false;
            }
        }

        /// <summary>
        /// Returs how frequently the number in question appears in the array in question.
        /// Returns -1 if an error occured.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int CountIntAmount(int number, int[] array)
        {
            if (!number.Equals(null) && array.Length > 0)
            {
                int count = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if (number == array[i])
                    {
                        count++;
                    }

                }

                return count;
            }
            else
            {
                if (number.Equals(null) && array.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("from CountIntAmount: 'number'" +
                        "paramater is not optional (@line 330, Splitter.cs)");
                }
                else if (!number.Equals(null) && array.Length <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("from CountIntAmount: 'array'" +
                        "paramater is either missing or its length = 0 (@line 335, " +
                        "Splitter.cs)");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("from CountIntAmount: both" +
                    " paramaters are invalid. Array length must be > 0 and number is" +
                    "not optional. (@line 342, Splitter.cs)");
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns the most frequent number (return[0]) and how frequent it is (return[1]).
        /// Returns null if an error occured.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int[] MostFrequent(int[] num)
        {
            if (num.Length > 1)
            {
                int[] freqC = new int[2];                       //stores the most frequent number and how frequent it is (the reutrned array)
                int[] list = new int[num.Length];               //stores the every number in the array but without duplicates
                int[] freq = new int[num.Length];               //stores the amount of times each number appers in the array
                freqC[0] = 0;                                   //set default returns to 0
                freqC[1] = 0;                                   //
                                                                //
                for (int i = 0; i < num.Length; i++)        //T-build a list of all diffent numbers in the array (copy the array but exclude duplicates)
                {                                           //| //Tloop for length 'num.length'
                    if (!NumberInArray(num[i], list))       //| //|
                    {                                       //| //|-Tif the numer of num[i] is not already existant in the list
                        list[i] = num[i];                   //| //|-|---add it to the list
                    }                                       //| //|-|e)
                }                                           //|-//|L)
                                                            //
                for (int i = 0; i < list.Length; i++)       //T-build a list of how often each number appears in the array
                {                                           //| //Tloop for length 'list.Length'
                    freq[i] = CountIntAmount(list[i], num); //| //|--count how often list[i] number appears in the array
                                                            //| //|
                }                                           //|-//|L)
                                                            //
                freqC[1] = freq.Max();                          //get the most frequent number
                for (int i = 0; i < freq.Length; i++)           //
                {                                               //Tloop for length 'freq.Length'
                    if (freq[i] == freqC[1])                    //|
                    {                                           //|-Tgo to the most frequent number
                        freqC[0] = i;                           //|-|---update [0] to the most frequent number
                                                                //| |
                    }                                           //| |e)
                }                                               //|L)
                return freqC;                                   //return the result
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("from " +
                    "MostFrequent: the array must be at" +
                    "least 2 large. (@line 392, Splitter.cs)");
                return null;
            }

        }

        /// <summary>
        /// Returns the amount of groups of numbers that appear in an array that are not concurrent
        /// e.g. 1,1,1,3,1,3,1,1 will return 5, as there's a group of three 1s, a group of one 3s, a group of one 1s, an OTHER group of one 3s and a group of two 1s.
        /// returns -1 on error.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static int CountGroupIntAmount(int[] arr)
        {
            //read first number and second, if first is same as second then itterate
            //if second is same as third then itterate, e.c.t.
            //e.g.  1,1,1,3,1,3,1,1    or    1,1,1,1,1,1,2,1     or      1,2,2,1,3,5,1,1
            //poss: 0,1,2,3,4,5,6,7          0,1,2,3,4,5,6,7             0,1,2,3,4,5,6,7
            //so if arr = 1,1,1,3,1,3,1,1
            //if possiton 0 = possition 1 then i++, if p0 = p2 then i++, if 0!=3 count++, 3!=4 count++, 4!=5 count++, 5!=6 count++, 6=7 count++

            if (arr.Length > 1)
            {
                int i = 0;                          //create a global itterator
                int count = 0;                      //create a counter
                int previous = 0;                   //create a global variable for storing the previous array possition (really it's the current)
                int next = 0;                       //create a global variable for storing the next array possition
                                                    //
                while (true)                        //
                {                                   //Tloop INFINITE
                    previous = arr[i];              //|--get the current item
                    next = arr[i + 1];              //|--get the next item in the array
                                                    //|
                    if (previous == next)           //|
                    {                               //|-Tif the next item is the same as the current item
                        i++;                        //|-|---don't count, just go to the next item
                    }
                    else
                    {                        //|-\c)otherwise, if the current item is not the same as the next (e.g. 1 != 3)
                        count++;                    //|-|---count the previous set, but don't count the current set - yet
                        i++;                        //|-|---itterate to the next item
                                                    //|-|
                    }                               //|-|e)
                                                    //|
                    if (i == arr.Length - 1)        //|
                    {                               //|-Tif the current itteration is at the end of the array
                        count++;                    //|-|---count the CURRENT set, as the previous set has already been counted
                        break;                      //|>|>>>END;
                    }                               //|-|e)
                }                                   //|INF)
                                                    //
                return count;                       //return the result
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("" +
                    "from CountGroupIntAmount: the" +
                    "array must be at least 2 large." +
                    " (@line 451, Splitter.cs)");
                return -1;
            }
        }

        //from: https://stackoverflow.com/questions/11052095/how-can-i-sort-a-string-of-text-followed-by-a-number-using-linq/11052176#11052176
        /// <summary>
        /// Sorts an array (e.g. of files) numerically (rather than alphabetically [default])
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<string> CustomSort(this IEnumerable<string> list)
        {
            int maxLen = list.Select(s => s.Length).Max();

            return list.Select(s => new
            {
                OrgStr = s,
                SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr);
        }

        /// <summary>
        /// Returns an enumerable with a string, depending on the state input - 0=alphabeticall, 1=alphanumericall, 2=custom set.
        /// Returns an enumerable with a length of one, with the string "An error occured during processing".
        /// Limit has no error checking.
        /// </summary>
        /// <param name="state">The type of counting to be used.</param>
        /// <param name="limit">The how large the resultant enumerable will be. States 0 and 1 require soley int based characters.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetAdditionType(ushort state, string limit)
        {
            //the 'limit' variable should remain a string for the sake of modability with the custom set state.

            //STATE 0 has a logic test logged in the "programming logs" file (line 22)
            //STATE 0 has an alternative version logged in the "programming logs" file (line 39)
            //  Please reference to this file to quickly understand the reasoning and structuring of this
            //  method before attempting any upgrades
            //STATE 0 has a mathmaticall reasoning log in the "programming logs" file (line 128)

            if (state == 0)
            {                                                                               //Tif working with an alphabet addition type
                                                                                            //|
                ushort[] LA = new ushort[1];                                                //|--initialize an array to hold the lettering code
                LA[0] = 1;                                                                  //|--set first letter to non-errornous code
                int.TryParse(limit, out int locLim);                                        //|--get the limit as an int
                string conc = "";                                                           //|--initialize a string to hold the resultant code
                                                                                            //|
                for (int c = 0; c < locLim; c++)                                            //|-Tloop untill the specified limit is reached
                {                                                                           //|-|----for the first 26 loops, only single letters are returned (A-Z)
                    for (int ovi = 1; ovi <= LA.Length; ovi++)                              //|-|----meaning this for loop (ovi) is ignored for 26 of the above loop's loops
                    {                                                                       //|-|---Tloop for the amount of columns (letters) are currently in use
                        if (LA[0 + (ovi - 1)] == 27)                                        //|-|   |
                        {                                                                   //|-|---|---Tif the currently tested letter is equal to 27 (past the letter "Z")
                            if (LA.Length == 1 * ovi) { LA = new ushort[1 + ovi]; }         //|-|---|---|---Te)if the number of letters that are currently under use needs to be increased by 1, do so
                            LA[1 + (ovi - 1)]++;                                            //|-|   |---|----itterate the newly created letter to a valid state (1)
                            LA[0 + (ovi - 1)] = 1;                                          //|-|   |---|----reset the first current letter
                                                                                            //|-|   |   |
                        }                                                                   //|-|   |   |e)
                    }                                                                       //|-|   |L)
                                                                                            //|-|  
                                                                                            //returns                                                               //|-|----start returning the results
                    for (int ovi = LA.Length; ovi > 0; ovi--)                               //|-|  
                    {                                                                       //|-|---Tdeiterative loop from the amount of letters currently in use down to the first letter
                        conc += char.ConvertFromUtf32((LA[0 + (ovi - 1)] - 1) + 65);        //|-|---|----add all the letters to the final resultant string
                    }                                                                       //|-|   |L)
                                                                                            //|-|  
                    yield return conc;                                                      //|-|----yield return the final resultant
                    conc = "";                                                              //|-|----reset the final resultant string
                    LA[0]++;                                                                //|-|----itterate the first letter's possition (i.e. change from "A" to "B")
                                                                                            //|-|
                }                                                                           //|-|L)
            }
            else if (state == 1)
            {                                                        //\c)if working with a alphanumeric addition type
                int.TryParse(limit, out int l);                                             //|--get the limit as an int
                                                                                            //|
                for (int i = 1; i <= l; i++)                                                //|
                {                                                                           //|-Tloop for until the input limit is reached
                    yield return i.ToString();                                              //|-|----simply return the loop number
                }                                                                           //|-|L)
                                                                                            //|
            }
            else if (state == 2)
            {                                                        //\cif working with a custom addition type
                                                                     //|--unused space! Make a mod!
                                                                     //|
            }
            else if (state == 3)
            {                                                        //\cif additon type must return nothing (disabled)
                int.TryParse(limit, out int l);                                             //|--get the limit as an int
                                                                                            //|
                for (int i = 1; i <= l; i++)                                                //|
                {                                                                           //|-Tloop for until the input limit is reached
                    yield return "";                                                        //|-|----return nothing
                }                                                                           //|-|L)
                                                                                            //|
            }
            else
            {                                                                        //\c)otherwise if the state is an invalid state
                for (int f = 1; f <= 1; f++)                                                //|
                {                                                                           //|
                    System.Diagnostics.Debug.WriteLine("from GetAdditionType: the" +        //|
                        "state must be either 0, 1 or 2 (@line 538, Splitter.cs)");         //|--report that an error occured
                    yield return "An error occured during processing";                      //|--return that an error occured
                }                                                                           //|
                                                                                            //|
            }                                                                               //|e)
        }

    }   //END
}   //END OF DOC