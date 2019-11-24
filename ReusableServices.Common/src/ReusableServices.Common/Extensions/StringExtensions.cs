using System.Linq;

namespace ReusableServices.Common.Extensions
{
  public static class StringExtensions
  {
    public static string Underscore(this string str)
    {
      return string.Concat(
        str.Select((letter, index) => index > 0 && char.IsUpper(letter) ? $"_{letter}" : letter.ToString()));
    }
  }
}