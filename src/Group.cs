using System.Text.RegularExpressions;

namespace ColliderEditor
{
    public class Group
    {
        public string Name { get; }
        Regex Pattern { get; }

        public Group(string name, string pattern)
        {
            Name = name;
            Pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        }

        public bool Test(string name)
        {
            return Pattern.IsMatch(name);
        }
    }
}
