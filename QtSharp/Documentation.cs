using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Util;
using System.Xml.Linq;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators.CSharp;
using HtmlAgilityPack.Samples;
using Mono.Data.Sqlite;
using zlib;
using Attribute = CppSharp.AST.Attribute;

namespace QtSharp
{
    public class Documentation
    {
        private readonly IDictionary<string, string> documentation;
        private readonly XDocument index;
        private readonly Regex regexArgName = new Regex(@"((unsigned\s*)?[\w<>]+)\s*(\*|&)?\s*\w*(\s*=\s*[^=,]+?)?(,|$)", RegexOptions.Compiled);
        private readonly Regex regexSpaceBetweenArgs = new Regex(@"\r?\n\s+", RegexOptions.Compiled);
        private readonly Regex regexEnumMembers = new Regex(@"\s*<div class=""table"">\s*<table class=""valuelist"">(?<member>.+?)\s*</table>\s*</div>\s*" +
                                                            @"([^\n]+?<p class=""figCaption"">(?<caption>.+?)</p>)?", RegexOptions.Singleline | RegexOptions.Compiled);

        public Documentation(string docsPath, string module)
        {
            this.documentation = Get(docsPath, module);
            var file = module.ToLowerInvariant();
            this.index = XDocument.Load(Path.Combine(docsPath, string.Format("qt{0}", file), string.Format("qt{0}.index", file)));
        }

        public void DocumentFunction(Function function)
        {
            var lineStart = function.LineNumberStart.ToString();
            var lineEnd = function.LineNumberEnd.ToString();
            var node = this.index.Descendants("function")
                .FirstOrDefault(f => f.Attribute("location").Value == function.TranslationUnit.FileName &&
                                     (f.Attribute("lineno").Value == lineStart || f.Attribute("lineno").Value == lineEnd));
            // HACK: functions in qglobal.h have weird additional definitions just for docs
            if ((node == null || node.Attribute("href") == null) && function.TranslationUnit.FileName == "qglobal.h")
            {
                node = this.index.Descendants("function")
                    .FirstOrDefault(c => c.Attribute("location").Value == function.TranslationUnit.FileName &&
                                         c.Attribute("name").Value == function.OriginalName);
            }
            // HACK: some functions are "located" in a cpp, go figure...
            var @params = function.Parameters.Where(p => p.Kind == ParameterKind.Regular).ToList();
            if ((node == null || node.Attribute("href") == null) && function.Signature != null)
            {
                var nodes = this.index.Descendants("function").Where(f => f.Attribute("fullname") != null &&
                                                                          f.Attribute("fullname").Value == function.QualifiedOriginalName &&
                                                                          f.Descendants("parameter").Count() == @params.Count).ToList();
                if (nodes.Count == 0)
                {
                    var method = function as Method;
                    if (method != null && !method.IsStatic && function.OriginalFunction == null)
                    {
                        return;
                    }
                    nodes = this.index.Descendants("function").Where(f => f.Attribute("name").Value == function.OriginalName &&
                                                                          f.Descendants("parameter").Count() == @params.Count).ToList();
                }
                if (nodes.Count == 1)
                {
                    node = nodes[0];
                }
                else
                {
                    if (function.Signature.Contains('('))
                    {
                        var startArgs = function.Signature.IndexOf('(') + 1;
                        var signature = function.Signature;
                        if (signature.Contains(": "))
                        {
                            signature = signature.Substring(0, signature.IndexOf(": ", StringComparison.Ordinal));
                        }
                        signature = signature.Substring(startArgs, signature.LastIndexOf(')') - startArgs);
                        signature = this.regexSpaceBetweenArgs.Replace(this.regexArgName.Replace(signature, "$1$3$5"), " ");
                        node = nodes.FirstOrDefault(f => string.Join(", ",
                            f.Descendants("parameter").Select(d => d.Attribute("left").Value.Replace(" *", "*").Replace(" &", "&"))) == signature);
                    }
                }
            }
            if (node != null && node.Attribute("href") != null)
            {
                var link = node.Attribute("href").Value.Split('#');
                var file = link[0];
                // TODO: work around https://bugreports.qt.io/browse/QTBUG-46153
                if (link[1] == "QDebug")
                {
                    link[1] = "QDebugx";
                }
                if (this.documentation.ContainsKey(file))
                {
                    var id = link[1].Split('-');
                    var docs = Regex.Matches(this.documentation[file],
                        string.Format(
                            @"<h3 class=""fn"" id=""{0}"">.+?{1}</h3>\s*(?<docs>.*?)\s*<!-- @@@{2} -->",
                            EscapeId(function.IsAmbiguous && node.Attribute("access").Value == "private" ? id[0] : link[1]),
                            id.Length > 1 && id[1] == "prop" ? string.Empty : @"\((?<args>.*?)\).*?",
                            Regex.Escape(function.OriginalName)),
                        RegexOptions.Singleline);
                    // HACK: work around bugs of the type of https://bugreports.qt.io/browse/QTBUG-46148
                    Match doc = null;
                    MatchCollection paramsMatches = null;
                    if (docs.Count == 1 && docs[0].Success)
                    {
                        doc = docs[0];
                        paramsMatches = Regex.Matches(doc.Groups["args"].Value, @"<i>\s*(.+?)\s*</i>");
                    }
                    else
                    {
                        foreach (var d in docs.Cast<Match>().Where(m => m.Success))
                        {
                            paramsMatches = Regex.Matches(d.Groups["args"].Value, @"<i>\s*(.+?)\s*</i>");
                            if (paramsMatches.Count == @params.Count)
                            {
                                doc = d;
                                break;
                            }
                        }
                    }
                    if (doc != null)
                    {
                        var i = 0;
                        foreach (Match match in paramsMatches)
                        {
                            // TODO: remove this in the final version; for now it's just for more easily spotting doc regressions
                            if (@params[i].Name.StartsWith("_", StringComparison.Ordinal) && char.IsDigit(@params[i].Name[1]))
                            {
                                @params[i].Name = Helpers.SafeIdentifier(match.Groups[1].Value);
                            }
                            i++;
                        }
                        // TODO: create links in the "See Also" section
                        function.Comment = new RawComment { BriefText = StripTags(doc.Groups["docs"].Value) };
                        if (node.Attribute("status").Value == "obsolete")
                        {
                            AddObsoleteAttribute(function);
                        }   
                    }
                }
            }
        }

        private static string EscapeId(string link)
        {
            var idBuilder = new StringBuilder();
            foreach (var c in link)
            {
                switch (c)
                {
                    case '<':
                        idBuilder.Append("-lt");
                        break;
                    case '>':
                        idBuilder.Append("-gt");
                        break;
                    case '=':
                        idBuilder.Append("-eq");
                        break;
                    case '&':
                        idBuilder.Append("-and");
                        break;
                    case '!':
                        idBuilder.Append("-not");
                        break;
                    case ' ':
                        idBuilder.Append("-");
                        break;
                    case '+':
                    case '*':
                    case '/':
                    case '|':
                    case '~':
                    case '^':
                    case '[':
                    case ']':
                        idBuilder.Append('-');
                        idBuilder.Append(((int) c).ToString("x"));
                        break;
                    default:
                        idBuilder.Append(c);
                        break;
                }
            }
            return Regex.Escape(idBuilder.ToString());
        }

        public void DocumentProperty(Property property)
        {
            var expansions = property.Namespace.PreprocessedEntities.OfType<MacroExpansion>();

            var properties = expansions.Where(e => e.Text.Contains("Q_PROPERTY") || e.Text.Contains("QDOC_PROPERTY"));
            string alternativeName = property.Name.Length == 1 ? property.Name :
                                     "is" + StringHelpers.UppercaseFirst(property.Name);
	        foreach (var macroExpansion in properties)
	        {
		        var name = macroExpansion.Text.Split(' ')[1];
		        if (name == property.Name || name == alternativeName)
		        {
                    property.LineNumberStart = macroExpansion.LineNumberStart;
                    property.LineNumberEnd = macroExpansion.LineNumberEnd;
                    property.Name = name;
					this.DocumentQtProperty(property);
					return;
		        }
	        }
            if (property.Field == null)
            {
                var comment = new RawComment();
                var getter = property.GetMethod;
                if (getter.Comment == null && getter.Namespace.OriginalName == property.Namespace.OriginalName)
                {
                    this.DocumentFunction(getter);
                }
                if (getter.Comment != null)
                {
                    comment.BriefText = getter.Comment.BriefText;
                }
                var setter = property.SetMethod;
                if (setter != null)
                {
                    if (setter.Comment == null && setter.Namespace.OriginalName == property.Namespace.OriginalName)
                    {
                        this.DocumentFunction(setter);
                    }
                    if (setter.Comment != null)
                    {
                        if (property.IsOverride)
                        {
                            comment.BriefText = ((Class) property.Namespace).GetRootBaseProperty(property).Comment.BriefText;
                        }
                        if (!string.IsNullOrEmpty(comment.BriefText))
                        {
                            comment.BriefText += Environment.NewLine;
                        }
                        comment.BriefText += setter.Comment.BriefText;
                    }
                }
                if (!string.IsNullOrEmpty(comment.BriefText))
                {
                    property.Comment = comment;
                }
            }
        }

        private void DocumentQtProperty(Declaration property)
        {
            var node = this.index.Descendants("property")
                .FirstOrDefault(c => c.Attribute("location").Value == property.TranslationUnit.FileName &&
                                     c.Attribute("name").Value == property.Name);
	        if (node != null && node.Attribute("href") != null)
			{
				var link = node.Attribute("href").Value.Split('#');
				var file = link[0];
		        if (this.documentation.ContainsKey(file))
		        {
			        string docs = this.documentation[file];
			        var match = Regex.Match(docs,
				        string.Format(
                            @"<h3 class=""fn"" id=""{0}-prop"">.+?</h3>\s*(?<docs>.+?)(\s*<p><b>[^:]+?:</b></p>\s*<div class=""table"">.+?</div>)+(?<notes>.*?)\s+?<!-- @@@{0} -->",
					        property.Name),
				        RegexOptions.Singleline | RegexOptions.ExplicitCapture);
			        if (match.Success)
			        {
						var docsBuilder = new StringBuilder(match.Groups["docs"].Value);
						if (!string.IsNullOrEmpty(match.Groups["notes"].Value))
						{
							docsBuilder.Append(match.Groups["notes"].Value);
						}
			            property.Comment = new RawComment { BriefText = StripTags(docsBuilder.ToString()) };
			        }
		        }
	        }
        }

        public void DocumentType(Class type)
        {
            var node = this.index.Descendants("class").FirstOrDefault(
                c => c.Attribute("location").Value == type.TranslationUnit.FileName &&
                     c.Attribute("name").Value == type.OriginalName);
            if (node != null)
            {
                var file = node.Attribute("href").Value;
                if (this.documentation.ContainsKey(file))
                {
                    var docs = this.documentation[file];
                    var match = Regex.Match(docs,
                        string.Format(@"<h2 id=""details"">Detailed Description</h2>\s*<p>(.+?)</p>\s*(.+?)\s*<!-- @@@{0} -->", type.Name),
                        RegexOptions.Singleline);
                    type.Comment = new RawComment
                    {
                        BriefText = StripTags(match.Groups[1].Value),
                        // TODO: create links in the "See Also" section; in general, convert all links
                        Text = StripTags(match.Groups[2].Value)
                    };
                }
            }
        }

        public void DocumentEnum(Enumeration @enum)
        {
            var node = this.index.Descendants("enum")
                .FirstOrDefault(c => c.Attribute("location").Value == @enum.TranslationUnit.FileName &&
                                     c.Attribute("name").Value == @enum.OriginalName);
            if (node != null)
            {
                var link = node.Attribute("href").Value.Split('#');
                var file = link[0];
                if (this.documentation.ContainsKey(file))
                {
                    var match = Regex.Match(this.documentation[file],
                        string.Format(
                            @"<h3 class=""\w+?"" id=""{0}"">.+?</h3>\s*(.+?)\s*<!-- @@@{1} -->",
                            Regex.Escape(link[1]), Regex.Escape(link[1].Split('-')[0])),
                        RegexOptions.Singleline);
                    if (match.Success)
                    {
                        @enum.Comment = new RawComment
                        {
                            BriefText = HtmlToText.ConvertHtml(this.regexEnumMembers.Replace(match.Groups[1].Value, m =>
                            {
                                foreach (var item in @enum.Items)
                                {
                                    var matchItem = Regex.Match(m.Groups["member"].Value,
                                        string.Format(
                                            @"<code>{0}</code></td><td class=""topAlign"">((<code>[^\n]+?</code>)|\?)</td>(<td class=""topAlign"">\s*(?<docs>.+?)\s*</td>)?</tr>",
                                            (@enum.Namespace is TranslationUnit ? "" : (@enum.Namespace.Name + "::")) + item.Name),
                                        RegexOptions.Singleline);
                                    if (matchItem.Success)
                                    {
                                        item.Comment = new RawComment
                                        {
                                            BriefText = StripTags(m.Groups["caption"].Success ? m.Groups["caption"].Value : matchItem.Groups["docs"].Value, true)
                                        };
                                    }
                                }
                                return string.Empty;
                            }))
                        };
                    }
                }
            }
        }

        private static IDictionary<string, string> Get(string docsPath, string module)
        {
            if (!Directory.Exists(docsPath))
            {
                return new Dictionary<string, string>();
            }
            try
            {
                IDictionary<string, string> documentation = GetFromQch(docsPath, module);
                return documentation.Count > 0 ? documentation : GetFromHtml(docsPath, module);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Documentation generation failed: {0}", ex.Message);
                return new Dictionary<string, string>();
            }
        }

        private static IDictionary<string, string> GetFromHtml(string docsPath, string module)
        {
            string docs = Path.Combine(docsPath, string.Format("qt{0}", module.ToLowerInvariant()));
            if (!Directory.Exists(docs))
            {
                return new Dictionary<string, string>();
            }
            return Directory.GetFiles(docs, "*.html").ToDictionary(Path.GetFileName, File.ReadAllText);
        }

        private static IDictionary<string, string> GetFromQch(string docsPath, string module)
        {
            var dataSource = Path.Combine(docsPath, string.Format("qt{0}.qch", module.ToLowerInvariant()));
            if (!File.Exists(dataSource))
            {
                return new Dictionary<string, string>();
            }
            var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = dataSource };
            using (var sqliteConnection = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString))
            {
                sqliteConnection.Open();
                using (var sqliteCommand = new SqliteCommand(
                    "SELECT Name, Data FROM FileNameTable INNER JOIN FileDataTable ON FileNameTable.FileId = FileDataTable.Id " +
                    "WHERE Name LIKE '%.html' " +
                    "ORDER BY Name", sqliteConnection))
                {
                    using (var sqliteDataReader = sqliteCommand.ExecuteReader())
                    {
                        var documentation = new Dictionary<string, string>();
                        while (sqliteDataReader.Read())
                        {
                            byte[] blob = new byte[ushort.MaxValue];
                            var length = (int) sqliteDataReader.GetBytes(1, 0, blob, 0, blob.Length);
                            using (var output = new MemoryStream(length - 4))
                            {
                                using (var zOutputStream = new ZOutputStream(output))
                                {
                                    zOutputStream.Write(blob, 4, length - 4);
                                    zOutputStream.Flush();
                                    documentation.Add(sqliteDataReader.GetString(0),
                                                      Encoding.UTF8.GetString(output.ToArray()));
                                }
                            }
                        }
                        return documentation;
                    }
                }
            }
        }

        private static string StripTags(string source, bool trim = false)
        {
            var array = new List<char>(source.Length);
            var tagArray = new List<char>();
            var inside = false;
            var literalBuilder = new StringBuilder();

            foreach (var character in source)
            {
                if (character == '<')
                {
                    inside = true;
                    continue;
                }
                if (character == '>')
                {
                    if (tagArray.Count < 5 || tagArray[0] != '!' || tagArray[1] != '-' || tagArray[2] != '-' ||
                        (tagArray[tagArray.Count - 2] == '-' && tagArray[tagArray.Count - 1] == '-'))
                    {
                        inside = false;
                    }
                    continue;
                }
                if (inside)
                {
                    tagArray.Add(character);
                }
                else
                {
                    string tag = new string(tagArray.ToArray());
                    if (tag.Contains("/tdtd"))
                    {
                        array.Add('\t');
                    }
                    tagArray.Clear();
                    switch (character)
                    {
                        case '&':
                            literalBuilder.Append(character);
                            break;
                        case ';':
                            literalBuilder.Append(character);
                            var literal = literalBuilder.ToString();
                            if (!string.IsNullOrEmpty(literal) && literal != "&#8203;")
                            {
                                array.AddRange(literal);
                            }
                            literalBuilder.Clear();
                            break;
                        default:
                            if (literalBuilder.Length > 0)
                            {
                                literalBuilder.Append(character);
                            }
                            else
                            {
                                array.Add(character);
                            }
                            break;
                    }
                }
            }
            var decoded = HtmlEncoder.HtmlDecode(new string(array.ToArray(), 0, array.Count));
            return trim ? decoded.Trim() : decoded;
        }

        private static void AddObsoleteAttribute(Declaration function)
        {
            var obsoleteMessageBuilder = new StringBuilder();
            obsoleteMessageBuilder.Append(HtmlEncoder.HtmlDecode(HtmlEncoder.HtmlEncode(function.Comment.BriefText).Split(
                Environment.NewLine.ToCharArray()).FirstOrDefault(line => line.Contains("instead") || line.Contains("deprecated"))));
            function.Attributes.Add(new Attribute
            {
                Type = typeof(ObsoleteAttribute),
                Value = string.Format("\"{0}\"", obsoleteMessageBuilder)
            });
        }
    }
}
