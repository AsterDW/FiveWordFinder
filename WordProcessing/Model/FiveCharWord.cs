using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FiveWordFinder.WordProcessing.Model
{
    public class FiveCharWord : IEquatable<FiveCharWord>, IComparable<FiveCharWord>, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        public string Word { get; private set; }

        /// <summary>
        /// 26 bits of an int32 are used to store an on/off flag for each letter that appears in the word string,
        /// This allows for bitwise comparisons between two or more words to detect if there are any shared letters.
        /// A 32 bit int maps the bits as follows:
        ///           zyxwvutsrqponmlkjihgfedcba
        ///     00000011111111111111111111111111
        /// The above would represent each letter a-z is turned on.
        /// </summary>
        public int CharBitMask { get; private set; }

        /// <summary>
        /// Returns the number of set bits representing the unique letters in the Word string.
        /// </summary>
        public int CountUniqueLetters
        {
            get { return CountSetBits(CharBitMask); }
        }

        /// <summary>
        /// 10 digit unsigned int value where each character in a word becomes a number within two of the positions in the 10 digit int.
        /// This is used to then sort the word alphabetically using integer comparisons instead of character comparisons of each string.
        /// </summary>
        private uint SortHash { get; set; }

        //Store the hash code so it isn't recalculated everytime this word is added to a set;
        private int HashCode { get; set; }

        private HashSet<FiveCharWord> _anagrams;
        public IReadOnlySet<FiveCharWord> Anagrams { get { return _anagrams; } }

        /// <summary>
        ///  HashSet to store words that do not have any letters in common with the this Word.
        /// </summary>
        private HashSet<FiveCharWord> _neighbors;
        public IReadOnlySet<FiveCharWord> Neighbors { get { return _neighbors; } }

        public FiveCharWord(string word)
        {
            if (word.Length > 5)
                throw new ArgumentException("Assigned word can not be greater than 5 characters.");

            Word = word;
            CharBitMask = EncodeWordBits(Word);
            SortHash = CalculateSortHash(Word);
            HashCode = SortHash.GetHashCode();
            _anagrams = new HashSet<FiveCharWord>();
            _neighbors = new HashSet<FiveCharWord>();
        }

        public bool AddAnagram(FiveCharWord word)
        {
            if (IsAnagramOf(word))
            {
                if (_anagrams.Add(word))
                {
                    OnPropertyChanged(nameof(Anagrams));
                    return true;
                }
            }

            return false;
        }

        public bool IsAnagramOf(FiveCharWord word)
        {
            return (this.CharBitMask ^ word.CharBitMask) == 0;
        }

        /// <summary>
        /// Add a neighbor word by comparing eah word's CharBitMask and if they do not share letters adds them to the Neighbors set.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>true if the element is added to the System.Collections.Generic.HashSet Neighbors object;<br/>
        /// false if the element is not a neighor (has shared letters) or is already present.</returns>
        public bool AddNeighbor(FiveCharWord word)
        {
            if (IsNeighborOf(word))
            {
                if (_neighbors.Add(word))
                {
                    OnPropertyChanged(nameof(Neighbors));
                    return true;
                }
            }
            return false;
        }

        public bool IsNeighborOf(FiveCharWord word)
        {
            return (this.CharBitMask & word.CharBitMask) == 0;
        }

        /// <summary>
        /// Encodes the string Word into a bitmask representing the letters in the word.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Int32 EncodeWordBits(string input)
        {
            Int32 storage = 0;
            foreach (var c in input)
            {
                storage |= 1 << ConvertCharToIndex(c);
            }

            return storage;
        }

        /// <summary>
        /// Converts a given char letter value into a 0 based index for the letter where 'a'=0 and 'z'=25
        /// </summary>
        /// <param name="inputChar"></param>
        /// <returns></returns>
        private int ConvertCharToIndex(char inputChar)
        {
            var lChar = char.ToLower(inputChar);
            return (int)lChar - 'a';
        }

        /// <summary>
        /// Returns the number of set bits representing the unique letters in the Word string.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int CountSetBits(int n)
        {
            int count = 0;
            while (n > 0)
            {
                count += n & 1;
                n >>= 1;
            }
            return count;
        }

        /// <summary>
        /// Store 5 character words as an unsigned integer value.
        /// Each letter has an index between 1-26 which is then stored across a 10 digit number. 2 digits for each position.
        /// This function converts each letter into it's numeric alphabet index and then multiplies each value by a power
        /// of 10 to place it into the position in the unsigned int.
        ///                                 t  h  r  e  e
        /// (Example: 'three' is encoded as 20|08|18|05|05)
        /// uint value                      2008180505
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private uint CalculateSortHash(string word)
        {
            uint hash = 0;
            for (int i = 0, j = word.Length - 1; i < word.Length; i++, j--)
            {
                uint iChar = (uint)(char.ToLower(word[i]) - 'a' + 1);
                //Each letter takes two digits so multiply letter place by 2 so it will be multiplied with the correct power of 10.
                double pow = Math.Pow(10, j * 2);
                hash += (uint)(iChar * pow);
            }
            return hash;
        }

        public override string ToString()
        {
            return this.Word;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        #region IEquatable methods
        public bool Equals(FiveCharWord? other)
        {
            return other != null && this.SortHash == other.SortHash;
        }
        #endregion IEquatable methods

        #region IComparable methods
        public int CompareTo(FiveCharWord? other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;
            return this.SortHash.CompareTo(other.SortHash);
        }

        // Define the is greater than operator.
        public static bool operator >(FiveCharWord operand1, FiveCharWord operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(FiveCharWord operand1, FiveCharWord operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(FiveCharWord operand1, FiveCharWord operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(FiveCharWord operand1, FiveCharWord operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
        #endregion IComparable methods
    }
}
