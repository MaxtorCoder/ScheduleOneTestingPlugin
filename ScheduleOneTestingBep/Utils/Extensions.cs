using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ScheduleOneTestingBep.Utils;

public static class Extensions
{
    public static ushort Get16BitHash(this AssetBundle assetBundle)
    {
        if (assetBundle == null)
            throw new ArgumentNullException(nameof(assetBundle));

        var hash = 0;
        unchecked
        {
            hash = assetBundle.GetAllAssetNames().Aggregate(hash, (current, assetName) => current * 31 * assetName.GetHashCode());
            hash *= 31 * assetBundle.name.GetHashCode();
        }

        return (ushort)(ushort.MaxValue - (ushort)(hash % ushort.MaxValue));
    }

    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var lowerCased = input.ToLower();
        var pascalCased = Regex.Replace(lowerCased, @"(^|_)(\w)", match => match.Groups[2].Value.ToUpper());

        return pascalCased;
    }
}
