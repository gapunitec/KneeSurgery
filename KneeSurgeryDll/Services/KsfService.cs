using System.Text.RegularExpressions;

namespace KneeSurgeryDll.Services
{
    public static class KsfService
    {
        private static Dictionary<string, string> _ksfs = new Dictionary<string, string>
        {
            { "Test1()", "print(\"This is a Knee Surgery Function (ksf)!\")" },
        };

        public static string KsfConverter(string ksf)
        {
            string pattern = @"ksf\.(" + string.Join("|", GetEscapedKeys(_ksfs.Keys)) + ")";
            Regex regex = new Regex(pattern);

            return regex.Replace(ksf, match =>
            {
                if (_ksfs.TryGetValue(match.Groups[1].Value, out string output))
                {
                    return output;
                }
                else
                {
                    return match.Value;
                }
            });
        }

        static IEnumerable<string> GetEscapedKeys(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                yield return Regex.Escape(key);
            }
        }
    }
}
