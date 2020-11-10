using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.IO;

namespace ServiceLibraries_lab3
{
    public static class JsonParser
    {
        //    ClassBuilder classBuilder = new ClassBuilder("TestClass");
        //    classBuilder.AddField(typeof(string), "someText", "abracadabra");
        //    classBuilder.AddField(typeof(bool), "areUCrazy", true);
        //    var testClass = classBuilder.CreateClass();
        //    Console.WriteLine($"testClass.Assembly={testClass.Assembly}");
        //    Console.WriteLine($"testClass.FullName={testClass.FullName}");
        //    foreach (var e in testClass.GetFields())
        //    {
        //        Console.WriteLine($"{e.FieldType} {e.Name} {e.GetValue(null)}"); 
        //    }        
        public static void Parse(string jsonFilePath, List<Type> etlJsonOptions)
        {
            StreamReader streamReader = new StreamReader(jsonFilePath);
            string inputJson = streamReader.ReadToEnd();
            string pattern0 = @"^{\s*""(?<AllConfName>[^""]*)""\s*:\s*{\s*(?<content>[\w\W]*)}\s*}$";
            var regexAllConfWithContent = new Regex(pattern0, RegexOptions.Compiled);
            var allConfName = regexAllConfWithContent.Match(inputJson).Groups["AllConfName"].Value;
            inputJson = regexAllConfWithContent.Match(inputJson).Groups["content"].Value;
            string pattern1 = @"""(?<ClassName>[^""]*)""\s*:\s*\{(?<Content>[^}]*)\}";
            var regexClassNameWithContent = new Regex(pattern1, RegexOptions.Compiled);
            string pattern2 = @"""(?<FieldName>[^""]*)""\s*:\s*(?<Content>[^,]*)";
            var regexDetailedContent = new Regex(pattern2, RegexOptions.Compiled);
            foreach (Match matchClassNameWithContent in regexClassNameWithContent.Matches(inputJson))
            {
                GroupCollection groupsClassNameWithContent = matchClassNameWithContent.Groups;
                ClassBuilder classBuilder = new ClassBuilder(groupsClassNameWithContent["ClassName"].Value);
                string content = groupsClassNameWithContent["Content"].Value;
                foreach (Match matchDetailedContent in regexDetailedContent.Matches(content))
                {
                    GroupCollection groupDetailedContent = matchDetailedContent.Groups;
                    Type actualType = FigureOutType(groupDetailedContent["Content"].Value);
                    classBuilder.AddField(actualType,
                                          groupDetailedContent["FieldName"].Value,
                                          Convert.ChangeType(groupDetailedContent["Content"].Value, actualType));
                }
                etlJsonOptions.Add(classBuilder.CreateClass());
            }

            ClassBuilder classBuilder1 = new ClassBuilder(allConfName);
            foreach (var e in etlJsonOptions)
            {
                classBuilder1.AddField(e.GetType(), e.Name, e);
            }
            etlJsonOptions.Add(classBuilder1.CreateClass());
        }

        private static Type FigureOutType(string inputString)
        {
            if (inputString[0] == '\"')
            {
                return typeof(string);
            }
            else
            {
                if (inputString.ToLower()[0] == 't' || inputString.ToLower()[0] == 'f')
                {
                    return typeof(bool);
                }
                else
                {
                    return typeof(int);
                }
            }
        }
    }
}
