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
using HtmlAgilityPack;
using Mono.Data.Sqlite;
using zlib;
using Attribute = CppSharp.AST.Attribute;

namespace QtSharp
{
    public class Documentation
    {
        public Documentation(string docsPath, string module)
        {
            foreach (var entry in Get(docsPath, module))
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(entry.Value);
                this.CollectMembersDocumentation(htmlDocument.DocumentNode, entry.Key);
                this.CollectTypesDocumentation(htmlDocument.DocumentNode, entry.Key);
            }
            this.Exists = this.membersDocumentation.Any() || this.typesDocumentation.Any();
            if (!this.Exists)
            {
                return;
            }
            var file = module.ToLowerInvariant();
            var index = XDocument.Load(Path.Combine(docsPath, string.Format("qt{0}", file), string.Format("qt{0}.index", file)));
            this.functionNodes = index.Descendants("function").GroupBy(f => f.Attribute("name").Value).ToDictionary(g => g.Key, g => g.ToList());
            this.propertyNodes = index.Descendants("property").GroupBy(f => f.Attribute("name").Value).ToDictionary(g => g.Key, g => g.ToList());
            this.classNodes = index.Descendants("class").ToList();
            this.enumNodes = index.Descendants("enum").ToList();
            this.variableNodes = index.Descendants("variable").ToList();
        }

        public bool Exists { get; set; }

        private void CollectMembersDocumentation(HtmlNode documentRoot, string docFile)
        {
            var members = new Dictionary<string, List<HtmlNode>>();
            this.membersDocumentation.Add(docFile, members);
            foreach (var fn in documentRoot.Descendants("h3").Where(
                n =>
                {
                    var @class = n.GetAttributeValue("class", "");
                    return @class == "fn" || @class == "flags";
                }))
            {
                var nodes = new List<HtmlNode>();
                var node = fn;
                while (node != null && node.NodeType != HtmlNodeType.Comment)
                {
                    if (!string.IsNullOrWhiteSpace(node.OuterHtml))
                    {
                        nodes.Add(node);
                    }
                    node = node.NextSibling;
                }
                var id = fn.GetAttributeValue("id", "");
                // HACK: work around bugs of the type of https://bugreports.qt.io/browse/QTBUG-46148
                if (members.ContainsKey(id))
                {
                    members[id + "-hack"] = nodes;
                }
                else
                {
                    members[id] = nodes;
                }
            }
        }

        private void CollectTypesDocumentation(HtmlNode documentRoot, string docFile)
        {
            var typeNode = documentRoot.Descendants("h2").FirstOrDefault(n => n.GetAttributeValue("id", "") == "details");
            if (typeNode != null)
            {
                var nodes = new List<HtmlNode>();
                var node = typeNode.NextSibling;
                while (node != null)
                {
                    if (!string.IsNullOrWhiteSpace(node.OuterHtml))
                    {
                        nodes.Add(node);                        
                    }
                    node = node.NextSibling;
                }
                node = typeNode.ParentNode.NextSibling;
                var addedNewLine = false;
                while (node != null && node.NodeType != HtmlNodeType.Comment)
                {
                    if (!string.IsNullOrWhiteSpace(node.OuterHtml))
                    {
                        if (!addedNewLine)
                        {
                            nodes.Add(node.OwnerDocument.CreateTextNode());
                            addedNewLine = true;
                        }
                        nodes.Add(node);
                    }
                    node = node.NextSibling;
                }
                this.typesDocumentation.Add(docFile, nodes);
            }
        }

        public void DocumentFunction(Function function)
        {
            if (!this.functionNodes.ContainsKey(function.OriginalName))
            {
                return;
            }

            var lineStart = function.LineNumberStart.ToString();
            var lineEnd = function.LineNumberEnd.ToString();
            var functions = this.functionNodes[function.OriginalName];
            var node = functions.Find(
                f => f.Attribute("location").Value == function.TranslationUnit.FileName &&
                     (f.Attribute("lineno").Value == lineStart || f.Attribute("lineno").Value == lineEnd));
            // HACK: functions in qglobal.h have weird additional definitions just for docs
            if ((node == null || node.Attribute("href") == null) && function.TranslationUnit.FileName == "qglobal.h")
            {
                node = functions.Find(
                    c => c.Attribute("location").Value == function.TranslationUnit.FileName &&
                         c.Attribute("name").Value == function.OriginalName);
            }
            // HACK: some functions are "located" in a cpp, go figure...
            var @params = function.Parameters.Where(p => p.Kind == ParameterKind.Regular).ToList();
            if ((node == null || node.Attribute("href") == null) && function.Signature != null)
            {
                var qualifiedOriginalName = function.GetQualifiedName(decl => decl.OriginalName,
                    decl =>
                    {
                        var @class = decl.OriginalNamespace as Class;
                        return @class != null ? (@class.OriginalClass ?? @class) : decl.OriginalNamespace;
                    });
                var nodes = functions.Where(f => f.Attribute("fullname") != null &&
                                                 f.Attribute("fullname").Value == qualifiedOriginalName &&
                                                 f.Descendants("parameter").Count() == @params.Count).ToList();
                if (nodes.Count == 0)
                {
                    @params.RemoveAll(p => string.IsNullOrEmpty(p.OriginalName) && p.DefaultArgument != null);
                }
                nodes = functions.Where(f => f.Attribute("fullname") != null &&
                                             f.Attribute("fullname").Value == qualifiedOriginalName &&
                                             f.Descendants("parameter").Count() == @params.Count).ToList();
                if (nodes.Count == 0)
                {
                    var method = function as Method;
                    if (method != null && !method.IsStatic && function.OriginalFunction == null)
                    {
                        return;
                    }
                    nodes = functions.Where(f => f.Attribute("name").Value == function.OriginalName &&
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
                        node = nodes.Find(f => string.Join(", ",
                            f.Descendants("parameter").Select(d => d.Attribute("left").Value.Replace(" *", "*").Replace(" &", "&"))) == signature);
                    }
                }
            }
            if (node != null && node.Attribute("href") != null)
            {
                var link = node.Attribute("href").Value.Split('#');
                var file = link[0];
                if (this.membersDocumentation.ContainsKey(file))
                {
                    var id = link[1].Split('-');
                    var key = EscapeId(function.IsAmbiguous && node.Attribute("access").Value == "private" ? id[0] : link[1]);
                    if (this.membersDocumentation[file].ContainsKey(key))
                    {
                        var docs = this.membersDocumentation[file][key];
                        var i = 0;
                        // HACK: work around bugs of the type of https://bugreports.qt.io/browse/QTBUG-46148
                        if ((function.OperatorKind == CXXOperatorKind.Greater && function.Namespace.Name == "QLatin1String" &&
                            @params[0].Type.ToString() == "QLatin1String") ||
                            ((function.OriginalName == "flush" || function.OriginalName == "reset") &&
                            function.Namespace.Name == "QTextStream" && @params.Count > 0))
                        {
                            docs = this.membersDocumentation[file][key + "-hack"];
                        }
                        foreach (Match match in Regex.Matches(docs[0].InnerHtml, @"<i>\s*(.+?)\s*</i>"))
                        {
                            @params[i].Name = Helpers.SafeIdentifier(match.Groups[1].Value);
                            i++;
                        }
                        // TODO: create links in the "See Also" section
                        function.Comment = new RawComment
                        {
                            BriefText = StripTags(ConstructDocumentText(docs.Skip(1)))
                        };
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
                            comment.BriefText = ((Class) property.Namespace).GetBaseProperty(property).Comment.BriefText;
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
            else
            {
                property.LineNumberStart = property.Field.LineNumberStart;
                property.LineNumberEnd = property.Field.LineNumberEnd;
                this.DocumentVariable(property);
            }
        }

        private void DocumentQtProperty(Declaration property)
        {
            if (!this.propertyNodes.ContainsKey(property.Name))
            {
                return;
            }
            var qualifiedName = property.GetQualifiedName(decl => decl.OriginalName, decl => decl.Namespace);
            var node = this.propertyNodes[property.Name].Find(
                c => c.Attribute("fullname").Value == qualifiedName);
	        if (node != null && node.Attribute("href") != null)
			{
				var link = node.Attribute("href").Value.Split('#');
				var file = link[0];
		        if (this.membersDocumentation.ContainsKey(file))
		        {
		            var typeDocs = this.membersDocumentation[file];
                    var key = string.Format("{0}-prop", property.Name);
		            var containsKey = typeDocs.ContainsKey(key);
		            if (!containsKey)
		            {
		                key = property.Name;
		                containsKey = typeDocs.ContainsKey(key);
		            }
                    if (containsKey)
		            {
		                var docs = typeDocs[key];
		                var start = docs.FindIndex(n => n.InnerText == "Access functions:");
                        start = start >= 0 ? start : docs.FindIndex(n => n.InnerText == "Notifier signal:");
		                var end = docs.FindLastIndex(n => n.Name == "div");
		                if (start >= 0 && end >= 0)
		                {
                            for (var i = end; i >= start; i--)
                            {
                                docs.RemoveAt(i);
                            }
                            if (string.IsNullOrWhiteSpace(docs[start - 1].OuterHtml))
                            {
                                docs.RemoveAt(start - 1);
                            }   
		                }
		                var text = ConstructDocumentText(docs.Skip(1));
		                property.Comment = new RawComment { BriefText = StripTags(text) };
		            }
		        }
	        }
        }

        public void DocumentType(Class type)
        {
            var node = this.classNodes.Find(
                c => c.Attribute("location").Value == type.TranslationUnit.FileName &&
                     c.Attribute("name").Value == type.OriginalName);
            if (node != null)
            {
                var file = node.Attribute("href").Value;
                if (this.typesDocumentation.ContainsKey(file))
                {
                    var docs = this.typesDocumentation[file];
                    var briefText = StripTags(docs[0].InnerHtml);
                    var text = StripTags(ConstructDocumentText(docs.Skip(1)));
                    type.Comment = new RawComment
                    {
                        BriefText = briefText,
                        // TODO: create links in the "See Also" section; in general, convert all links
                        Text = text,
                        FullComment = new FullComment()
                    };
                    var paragraphComment = new ParagraphComment();
                    paragraphComment.Content.Add(new TextComment { Text = briefText });
                    paragraphComment.Content.AddRange(text.Split('\n').Select(t => new TextComment { Text = t }));
                    type.Comment.FullComment.Blocks.Add(paragraphComment);
                }
            }
        }

        public void DocumentEnum(Enumeration @enum)
        {
            var node = this.enumNodes.Find(
                c => c.Attribute("location").Value == @enum.TranslationUnit.FileName &&
                     c.Attribute("name").Value == @enum.OriginalName);
            if (node != null)
            {
                var link = node.Attribute("href").Value.Split('#');
                var file = link[0];
                if (this.membersDocumentation.ContainsKey(file))
                {
                    var key = Regex.Escape(link[1]);
                    if (this.membersDocumentation[file].ContainsKey(key))
                    {
                        var docs = this.membersDocumentation[file][key];
                        var enumMembersDocs = new List<HtmlNode>();
                        for (var i = docs.Count - 1; i >= 0; i--)
                        {
                            var doc = docs[i];
                            if ((doc.Name == "div" && doc.FirstChild.GetAttributeValue("class", "") == "valuelist") ||
                                (doc.Name == "p" && doc.GetAttributeValue("class", "") == "figCaption"))
                            {
                                enumMembersDocs.Add(doc);
                                docs.RemoveAt(i);
                                // TODO: handle images
                                if (string.IsNullOrWhiteSpace(docs[i - 1].OuterHtml) || string.IsNullOrWhiteSpace(docs[i - 1].InnerText))
                                {
                                    docs.RemoveAt(i-- - 1);
                                }
                            }
                        }
                        @enum.Comment = new RawComment { BriefText = StripTags(ConstructDocumentText(docs.Skip(1))) };
                        var enumPrefix = @enum.Namespace is TranslationUnit ? "" : (@enum.Namespace.Name + "::");
                        foreach (var item in @enum.Items)
                        {
                            var itemQualifiedName = enumPrefix + item.Name;
                            var enumMemberDocs = (from member in enumMembersDocs
                                                  from code in member.Descendants("code")
                                                  where code.InnerText == itemQualifiedName
                                                  select code.ParentNode.ParentNode).FirstOrDefault();
                            if (enumMemberDocs != null)
                            {
                                if (enumMemberDocs.Descendants("td").Count() > 2)
                                {
                                    item.Comment = new RawComment
                                    {
                                        BriefText = StripTags(enumMemberDocs.Descendants("td").Last().InnerText).Trim()
                                    };
                                }
                                else
                                {
                                    enumMemberDocs = enumMemberDocs.ParentNode.ParentNode;
                                    enumMemberDocs = enumMembersDocs.SkipWhile(n => n != enumMemberDocs).FirstOrDefault(n => n.Name == "p");
                                    if (enumMemberDocs != null)
                                    {
                                        item.Comment = new RawComment { BriefText = enumMemberDocs.InnerText };                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DocumentVariable(Declaration variable)
        {
            var lineStart = variable.LineNumberStart.ToString();
            var lineEnd = variable.LineNumberEnd.ToString();
            var node = this.variableNodes.Find(
                f => f.Attribute("location").Value == variable.TranslationUnit.FileName &&
                     (f.Attribute("lineno").Value == lineStart || f.Attribute("lineno").Value == lineEnd));
            if (node != null)
            {
                var link = node.Attribute("href").Value.Split('#');
                var file = link[0];
                if (this.membersDocumentation.ContainsKey(file))
                {
                    var key = link[1];
                    if (this.membersDocumentation[file].ContainsKey(key))
                    {
                        var docs = this.membersDocumentation[file][key];
                        // TODO: create links in the "See Also" section
                        variable.Comment = new RawComment
                        {
                            BriefText = StripTags(ConstructDocumentText(docs.Skip(1)))
                        };
                        if (node.Attribute("status").Value == "obsolete")
                        {
                            AddObsoleteAttribute(variable);
                        }
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

        private static string ConstructDocumentText(IEnumerable<HtmlNode> html)
        {
            return string.Join("\n", html.Select(n => n.OuterHtml));
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

        private readonly Dictionary<string, List<HtmlNode>> typesDocumentation = new Dictionary<string, List<HtmlNode>>();
        private readonly Dictionary<string, Dictionary<string, List<HtmlNode>>> membersDocumentation = new Dictionary<string, Dictionary<string, List<HtmlNode>>>();
        private readonly Regex regexArgName = new Regex(@"((unsigned\s*)?[\w<>]+)\s*(\*|&)?\s*\w*(\s*=\s*[^=,]+?)?(,|$)", RegexOptions.Compiled);
        private readonly Regex regexSpaceBetweenArgs = new Regex(@"\r?\n\s+", RegexOptions.Compiled);

        private readonly Dictionary<string, List<XElement>> functionNodes;
        private readonly Dictionary<string, List<XElement>> propertyNodes;
        private readonly List<XElement> classNodes;
        private readonly List<XElement> enumNodes;
        private readonly List<XElement> variableNodes;

    }
}
