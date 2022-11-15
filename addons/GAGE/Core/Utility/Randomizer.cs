/*Randomizer.cs
This is a collection of functions I've either hacked together or found that produce psuedorandom results
This is provided as-is under the do-what-you-want license
(c)SG 2017

*/

using Godot;
using System;
using System.Text;

public static class Randomizer
{
    private static Random _rando = new Random();

    public static void Seed(int seed)
    {
        _rando = new Random(seed);
    }

    public static void Reseed()
    {
        _rando = new Random();
    }

    public static void Reseed(int seed)
    {
        _rando = new Random(seed);
    }

    public static float Range(float min, float maxExclusive)
    {
        return min + (float)_rando.NextDouble() * (maxExclusive - min);
    }

    public static int Range(int min, int maxExclusive)
    {
        return _rando.Next(min, maxExclusive);
    }

    public static float Between(float min, float maxInclusive)
    {
        return Range(min, maxInclusive + 1);
    }

    public static int Between(int min, int maxInclusive)
    {
        return Range(min, maxInclusive + 1);
    }

    public static int Max(int max)
    {
        return _rando.Next(max);
    }

    public static float Max(float max)
    {
        return (float)_rando.NextDouble() * max;
    }

    public static int Next()
    {
        return _rando.Next();
    }

    public static float NextFloat()
    {
        return (float)_rando.NextDouble();
    }

    public static int RollDie(int numSides)
    {
        return Range(1, numSides + 1);
    }

    public static int CoinFlip()
    {
        return _rando.Next(2);
    }

    public static int CoinFlipOneNegOne()
    {
        if (_rando.Next(2) > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public static bool BooleanCoinFlip()
    {
        if (_rando.Next(2) > 0)
        {
            return true;
        }

        return false;
    }

    public static byte RandomByte()
    {
        return (byte)Range(0, 256);
    }

    public static int RandomByteAsInt()
    {
        return Range(0, 256);
    }

    public static string RandomString(int size = 38)
    {
        var builder = new StringBuilder();

        char ch;
        for (var i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * NextFloat() + 65)));
            builder.Append(ch);
        }

        return builder.ToString();
    }

    public static Vector2 RandomVec2()
    {
        var v = new Vector2();
        v.x = Range(-1f, 1f);
        v.y = Range(-1f, 1f);
        return v;
    }

    public static Vector2 RandomPositiveVec2()
    {
        var v = new Vector2();
        v.x = NextFloat();
        v.y = NextFloat();
        return v;
    }

    public static Vector3 RandomVec3()
    {
        var v = new Vector3();
        v.x = Range(-1f, 1f);
        v.y = Range(-1f, 1f);
        v.z = Range(-1f, 1f);
        return v;
    }

    public static Vector3 RandomPositiveVec3()
    {
        var v = new Vector3();
        v.x = NextFloat();
        v.y = NextFloat();
        v.z = NextFloat();
        return v;
    }

    public static Color RandomColor(bool randomizeAlpha)
    {
        var r = NextFloat();
        var g = NextFloat();
        var b = NextFloat();
        var a = randomizeAlpha ? NextFloat() : 1f;
        var c = new Color(r,g,b,a);

        return c;

    }

    public static int RandomInt()
    {
        return Range(0, int.MaxValue);
    }

    /// <summary>
    /// Get a random english-ish sounding word
    /// </summary>
    /// <param name="minLength">min length of the name, must be at least 2</param>
    /// <param name="maxLength">max length of the name, must be greater than or equal to min</param>
    /// <returns>a name-ish sound string, between the length of min-max</returns>
    public static string NextWord(int minLength, int maxLength)
    {
        //make sure the min is long enough (must be at least 2 letters)
        if (minLength < 2)
        {
            throw new ArgumentException("minLength must be at least 2");
        }

        //Get the length of the name we are gonna generate
        int nameLength = _rando.Next(minLength, maxLength);

        //create an empty word
        StringBuilder name = new StringBuilder();

        // The letters to choose from.  we add more of some so that we are more likely to get those letters
        string[] consonants = new string[]
        {
                "b", "b", "c", "d", "d", "f","g","g","h","h","j","k","l","m","m","n","n","p","p","qu","r","r","s","s","s","t","t","t","v","w","x","z",
                "ck", "ck", "sh", "ch", "nn", "gh", "ll", "st", "st", "mn", "sp", "sp","ss", "tt"
        };
        string[] vowels = new string[]
        {
                "a","a","a","e","e","e","i","i","o","o","u","y",
                "ou", "ea", "oo"
        };

        //Should this name start with a consonant?
        bool startConsonant = 0 == _rando.Next(0, 2);

        //first fill the name with random consonants
        while (name.Length < nameLength)
        {
            //do we want a consonant or a vowel?
            if (startConsonant)
            {
                name.Append(consonants[_rando.Next(0, consonants.Length)]);
            }
            else
            {
                name.Append(vowels[_rando.Next(0, vowels.Length)]);
            }
            startConsonant = !startConsonant;
        }

        return name.ToString();
    }

    /// <summary>
    /// Get a random english-ish sounding name... same as a word, but the first letter is uppercase
    /// </summary>
    /// <param name="minLength">min length of the name, must be at least 2</param>
    /// <param name="maxLength">max length of the name, must be greater than or equal to min</param>
    /// <returns>a name-ish sound string, between the length of min-max</returns>
    public static string NextName(int minLength, int maxLength)
    {
        //get a random word
        StringBuilder name = new StringBuilder(NextWord(minLength, maxLength));

        //the first letter should be uppercase!
        name[0] = (Char.ToUpper(name[0]));

        return name.ToString();
    }

    /// <summary>
    /// Get a random swear word
    /// </summary>
    /// <returns>a four letter swear word</returns>
    public static string NextSwearWord()
    {
        return NextWord(4, 4);
    }
}
