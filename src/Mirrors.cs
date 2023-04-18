using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ColliderEditor
{
    public static class Mirrors
    {
        readonly static List<KeyValuePair<string, string>> _map = new List<KeyValuePair<string, string>>();

        class MirrorRegexReplace
        {
            public Regex regex;
            public string replacement;
        }

        readonly static MirrorRegexReplace[] _mirrorRegexes =
        {
            new MirrorRegexReplace { regex = new Regex(@"l(?=[A-Z])", RegexOptions.Compiled), replacement = "r" },
            new MirrorRegexReplace { regex = new Regex(@"Left", RegexOptions.Compiled), replacement = "Right" },
            new MirrorRegexReplace { regex = new Regex(@"(?<!a)l$", RegexOptions.Compiled), replacement = "r" },
            new MirrorRegexReplace { regex = new Regex(@"(?<!a)l(?=\.)", RegexOptions.Compiled), replacement = "r" },
            new MirrorRegexReplace { regex = new Regex(@"L$", RegexOptions.Compiled), replacement = "R" },
            new MirrorRegexReplace { regex = new Regex(@"(?<!_Collider|pelvisB)L(?=[0-9A-Z\.])", RegexOptions.Compiled), replacement = "R" },
        };

        public static string Find(string name)
        {
            bool matched = false;
            string s = name;

            foreach(var entry in _map)
            {
                if(s.EndsWith(entry.Key))
                {
                    s = s.Substring(0, s.Length - entry.Key.Length) + entry.Value;
                    matched = true;
                    break;
                }
            }

            foreach(var r in _mirrorRegexes)
            {
                if(!r.regex.IsMatch(s))
                {
                    continue;
                }

                s = r.regex.Replace(s, r.replacement);
                matched = true;
            }

            return matched ? s : null;
        }

        static Mirrors()
        {
            // ReSharper disable StringLiteralTypo

            // tongue
            Add(
                "tongue03.StandardCollidersTongue03._Collider2._Collider2",
                "tongue03.StandardCollidersTongue03._Collider3._Collider3");
            Add(
                "tongue04.StandardCollidersTongue04._Collider2._Collider2",
                "tongue04.StandardCollidersTongue04._Collider3._Collider3");
            Add(
                "tongue04.StandardCollidersTongue04._Collider4._Collider4",
                "tongue04.StandardCollidersTongue04._Collider5._Collider5");
            Add(
                "tongue05.StandardCollidersTongue05._Collider2._Collider2",
                "tongue05.StandardCollidersTongue05._Collider3._Collider3");
            Add(
                "tongue05.StandardCollidersTongue05._Collider4._Collider4",
                "tongue05.StandardCollidersTongue05._Collider5._Collider5");
            Add(
                "tongueTip.StandardCollidersTongueTip._Collider2._Collider2",
                "tongueTip.StandardCollidersTongueTip._Collider3._Collider3");
            Add(
                "tongueTip.StandardCollidersTongueTip._Collider4._Collider4",
                "tongueTip.StandardCollidersTongueTip._Collider5._Collider5");

            // Chest
            Add(
                "chest.FemaleAutoColliderschest.AutoColliderFemaleAutoColliderschest6 (1).AutoColliderFemaleAutoColliderschest6 (1)",
                "chest.FemaleAutoColliderschest.AutoColliderFemaleAutoColliderschest6 (2).AutoColliderFemaleAutoColliderschest6 (2)");
            Add(
                "chest.FemaleAutoColliderschest.AutoColliderFemaleAutoColliderschest6 (3).AutoColliderFemaleAutoColliderschest6 (3)",
                "chest.FemaleAutoColliderschest.AutoColliderFemaleAutoColliderschest6 (4).AutoColliderFemaleAutoColliderschest6 (4)");
            Add(
                "chest.FemaleAutoColliderschest.AutoColliderFemaleAutoColliderschest6 (5).AutoColliderFemaleAutoColliderschest6 (5)",
                "chest.FemaleAutoColliderschest.AutoColliderFemaleAutoColliderschest6 (6).AutoColliderFemaleAutoColliderschest6 (6)");

            // abdomen
            Add(
                "abdomen2.FemaleAutoCollidersabdomen2_.AutoColliderFemaleAutoCollidersabdomen2_7.AutoColliderFemaleAutoCollidersabdomen2_7",
                "abdomen2.FemaleAutoCollidersabdomen2_.AutoColliderFemaleAutoCollidersabdomen2_8.AutoColliderFemaleAutoCollidersabdomen2_8");
            Add(
                "abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen5.AutoColliderFemaleAutoCollidersabdomen5",
                "abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen6.AutoColliderFemaleAutoCollidersabdomen6");
            for(int i = 1; i <= 5; ++i)
            {
                Add(
                    $"abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen{6 + i}.AutoColliderFemaleAutoCollidersabdomen{6 + i}",
                    $"abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen{6 + i + 5}.AutoColliderFemaleAutoCollidersabdomen{6 + i + 5}");
            }

            Add(
                "abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen21.AutoColliderFemaleAutoCollidersabdomen21",
                "abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen22.AutoColliderFemaleAutoCollidersabdomen22");
            Add(
                "abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen23.AutoColliderFemaleAutoCollidersabdomen23",
                "abdomen.FemaleAutoCollidersabdomen.AutoColliderFemaleAutoCollidersabdomen24.AutoColliderFemaleAutoCollidersabdomen24");

            // shin
            for(int i = 1; i <= 16; ++i)
            {
                Add(
                    $"lShin.FemaleAutoColliderslShin.AutoColliderFemaleAutoColliderslShin{i}.AutoColliderFemaleAutoColliderslShin{i}",
                    $"rShin.FemaleAutoCollidersrShin.AutoColliderrShin{i}.AutoColliderrShin{i}");
            }

            // ReSharper restore StringLiteralTypo
        }

        static void Add(string left, string right)
        {
            _map.Add(new KeyValuePair<string, string>(left, right));
        }
    }
}
