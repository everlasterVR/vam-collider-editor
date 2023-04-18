using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ColliderEditor
{
    sealed class MirrorRegexReplace
    {
        public Regex regex;
        public string replacement;
    }

    static class Mirrors
    {
        public static MirrorRegexReplace[] GetMirrorRegexes()
        {
            return new[]
            {
                new MirrorRegexReplace { regex = new Regex(@"l(?=[A-Z])", RegexOptions.Compiled), replacement = "r" },
                new MirrorRegexReplace { regex = new Regex(@"Left", RegexOptions.Compiled), replacement = "Right" },
                new MirrorRegexReplace { regex = new Regex(@"(?<!a)l$", RegexOptions.Compiled), replacement = "r" },
                new MirrorRegexReplace { regex = new Regex(@"(?<!a)l(?=\.)", RegexOptions.Compiled), replacement = "r" },
                new MirrorRegexReplace { regex = new Regex(@"L$", RegexOptions.Compiled), replacement = "R" },
                new MirrorRegexReplace { regex = new Regex(@"(?<!_Collider|pelvisB)L(?=[0-9A-Z\.])", RegexOptions.Compiled), replacement = "R" },
            };
        }
    }
}
