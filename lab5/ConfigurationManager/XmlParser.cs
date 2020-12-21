using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ConfigurationManager
{
    public class XmlParser
    {
        private Utils _utils = new Utils();
        public void Parse(string xmlFilePath, List<Type> etlXmlOptions)
        {
            try
            {
                var streamReader = new StreamReader(xmlFilePath);
                //Cannot use here async read because firstly it is has no sense because config files not that big and secondly programm cannot work before system configuration not readen
                var inputXml = streamReader.ReadToEnd();
                streamReader.Close();
                var pattern0 = @"^\s*
                                 <[^>]*>\s*
                                 <(?<AllConfName>[^>]*)>\s*
                                 (?<Content>[\w\W]*)
                                 </\k<AllConfName>>\s*$";
                var regexAllConfWithContent = new Regex(pattern0, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                var allConfName = regexAllConfWithContent.Match(inputXml).Groups["AllConfName"].Value;
                inputXml = regexAllConfWithContent.Match(inputXml).Groups["Content"].Value;

                var pattern1 = @"\s*<(?<ClassName>[^>]*)>\s*
                                 (?<Content>[\w\W]*)\s*
                                 </\k<ClassName>>";
                var regexClassNameWithContent = new Regex(pattern1, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

                var pattern2 = @"\s*<(?<FieldName>[^>]*)>\s*
                                 (?<Content>[\w\W]*)\s*
                                 </\k<FieldName>>";
                var regexDetailedContent = new Regex(pattern2, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                foreach (Match matchClassNameWithContent in regexClassNameWithContent.Matches(inputXml))
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
                    etlXmlOptions.Add(classBuilder.CreateClass());
                }

                var classBuilder1 = new ClassBuilder(allConfName);
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
    }
}
