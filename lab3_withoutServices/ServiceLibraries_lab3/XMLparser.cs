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
    public static class XmlParser
    {
        public static void Parse(string xmlFilePath, List<Type> etlXmlOptions)
        {
            try
            {
                StreamReader streamReader = new StreamReader(xmlFilePath);
                string inputXml = streamReader.ReadToEnd();
                string pattern0 = @"^\s*
                                <[^>]*>\s*
                                <(?<AllConfName>[^>]*)>\s*
                                (?<Content>[\w\W]*)
                                </\k<AllConfName>>\s*$";
                Regex regexAllConfWithContent = new Regex(pattern0, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                var allConfName = regexAllConfWithContent.Match(inputXml).Groups["AllConfName"].Value;
                inputXml = regexAllConfWithContent.Match(inputXml).Groups["Content"].Value;

                string pattern1 = @"\s*<(?<ClassName>[^>]*)>\s*
                                    (?<Content>[\w\W]*)\s*
                                    </\k<ClassName>>";
                var regexClassNameWithContent = new Regex(pattern1, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

                string pattern2 = @"\s*<(?<FieldName>[^>]*)>\s*
                                    (?<Content>[\w\W]*)\s*
                                    </\k<FieldName>>";
                var regexDetailedContent = new Regex(pattern2, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                foreach (Match matchClassNameWithContent in regexClassNameWithContent.Matches(inputXml))
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
                    etlXmlOptions.Add(classBuilder.CreateClass());
                }

                ClassBuilder classBuilder1 = new ClassBuilder(allConfName);
                foreach (var e in etlXmlOptions)
                {
                    classBuilder1.AddField(e.GetType(), e.Name, e);
                }
                etlXmlOptions.Add(classBuilder1.CreateClass());
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format("Xml file has wrong format: {0}", e.Message));
            }
        }

        private static Type FigureOutType(string inputString)
        {
            if (bool.TryParse(inputString, out _))
            {
                return typeof(bool);
            }
            else if (int.TryParse(inputString, out _))
            {
                return typeof(int);
            }
            else
            {
                return typeof(string);
            }                               
        }
    }
}
