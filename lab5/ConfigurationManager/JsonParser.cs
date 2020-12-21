using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfigurationManager
{
    public class JsonParser
    {
        private Utils _utils = new Utils();
        public void Parse(string jsonFilePath, List<Type> etlJsonOptions)
        {
            try
            {
                var streamReader = new StreamReader(jsonFilePath);
                //Cannot use here async read because firstly it is has no sense because config files not that big and secondly programm cannot work before system configuration not readen
                var inputJson = streamReader.ReadToEnd();
                streamReader.Close();
                var pattern0 = @"^{\s*
                                 ""(?<AllConfName>[^""]*)""\s*:\s*
                                 {\s*(?<Content>[\w\W]*)}\s*}$";
                var regexAllConfWithContent = new Regex(pattern0, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                var allConfName = regexAllConfWithContent.Match(inputJson).Groups["AllConfName"].Value;
                inputJson = regexAllConfWithContent.Match(inputJson).Groups["Content"].Value;

                var pattern1 = @"""(?<ClassName>[^""]*)""\s*:\s*
                                 \{(?<Content>[^}]*)\}";
                var regexClassNameWithContent = new Regex(pattern1, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

                var pattern2 = @"""(?<FieldName>[^""]*)""\s*:\s*[@""]*
                                    (?<Content>[^,""]*)\s*[@""]*";
                var regexDetailedContent = new Regex(pattern2, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                foreach (Match matchClassNameWithContent in regexClassNameWithContent.Matches(inputJson))
                {
                    var groupsClassNameWithContent = matchClassNameWithContent.Groups;
                    var classBuilder = new ClassBuilder(groupsClassNameWithContent["ClassName"].Value);
                    var content = groupsClassNameWithContent["Content"].Value;
                    foreach (Match matchDetailedContent in regexDetailedContent.Matches(content))
                    {
                        var groupDetailedContent = matchDetailedContent.Groups;
                        var actualType = _utils.FigureOutType(groupDetailedContent["Content"].Value);
                        classBuilder.AddField(actualType,
                                              groupDetailedContent["FieldName"].Value,
                                              Convert.ChangeType(groupDetailedContent["Content"].Value, actualType));
                    }
                    etlJsonOptions.Add(classBuilder.CreateClass());
                }

                var classBuilder1 = new ClassBuilder(allConfName);
                foreach (var e in etlJsonOptions)
                {
                    classBuilder1.AddField(e.GetType(), e.Name, e);
                }
                etlJsonOptions.Add(classBuilder1.CreateClass());
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format("Json file has wrong format: {0}", e.Message));
            }
        }        
    }
}
