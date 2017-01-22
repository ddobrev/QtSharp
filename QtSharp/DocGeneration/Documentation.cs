using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Util;
using System.Xml;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using HtmlAgilityPack;
using Mono.Data.Sqlite;
using zlib;
using Attribute = CppSharp.AST.Attribute;

namespace QtSharp.DocGeneration
{
    public class Documentation
    {
        public Documentation(string docsPath, IEnumerable<string> modules)
        {
            // HACK: work around https://bugreports.qt.io/browse/QTBUG-54025
            var documentationModules = new List<string>(modules);
            const string qt3d = "Qt3D";
            documentationModules.RemoveAll(d => d.StartsWith(qt3d, StringComparison.Ordinal));
            documentationModules.Add(qt3d);
            foreach (var module in documentationModules)
            {
                var entries = Get(docsPath, module);
                foreach (var entry in entries)
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(entry.Value);
                    this.CollectMembersDocumentation(htmlDocument.DocumentNode, entry.Key);
                    this.CollectTypesDocumentation(htmlDocument.DocumentNode, entry.Key);
                }
                if (entries.Any())
                {
                    this.CollectIndices(docsPath, module);
                }
            }
            this.Exists = this.membersDocumentation.Any() || this.typesDocumentation.Any();
        }

        private void CollectIndices(string docsPath, string module)
        {
            var xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
            var file = module.ToLowerInvariant();
            using (var stream = new FileStream(Path.Combine(docsPath, file, string.Format("{0}.index", file)), FileMode.Open))
            {
                using (var xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.GetAttribute("lineno") != null)
                        {
                            switch (xmlReader.Name)
                            {
                                case "function":
                                    var functionDocumentationNode = GetFunctionDocumentationNode(xmlReader);
                                    if (this.functionNodes.ContainsKey(functionDocumentationNode.Name))
                                    {
                                        this.functionNodes[functionDocumentationNode.Name].Add(functionDocumentationNode);
                                    }
                                    else
                                    {
                                        this.functionNodes.Add(functionDocumentationNode.Name, new List<FunctionDocIndexNode> { functionDocumentationNode });
                                    }
                                    break;
                                case "property":
                                    var fullNameDocumentationNode = GetFullNameDocumentationNode(xmlReader);
                                    if (this.propertyNodes.ContainsKey(fullNameDocumentationNode.Name))
                                    {
                                        this.propertyNodes[fullNameDocumentationNode.Name].Add(fullNameDocumentationNode);
                                    }
                                    else
                                    {
                                        this.propertyNodes.Add(fullNameDocumentationNode.Name, new List<FullNameDocIndexNode> { fullNameDocumentationNode });
                                    }
                                    break;
                                case "class":
                                    this.classNodes.Add(GetDocumentationNode(xmlReader));
                                    break;
                                case "enum":
                                    this.enumNodes.Add(GetDocumentationNode(xmlReader));
                                    break;
                                case "variable":
                                    this.variableNodes.Add(GetDocumentationNode(xmlReader));
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static FunctionDocIndexNode GetFunctionDocumentationNode(XmlReader xmlReader)
        {
            var functionDocumentationNode = new FunctionDocIndexNode
                                            {
                                                Name = xmlReader.GetAttribute("name"),
                                                FullName = xmlReader.GetAttribute("fullname"),
                                                Access = xmlReader.GetAttribute("access"),
                                                Location = xmlReader.GetAttribute("location"),
                                                LineNumber = int.Parse(xmlReader.GetAttribute("lineno")),
                                                HRef = xmlReader.GetAttribute("href"),
                                                IsObsolete = xmlReader.GetAttribute("status") == "obsolete"
                                            };
            using (var xmlSubReader = xmlReader.ReadSubtree())
            {
                while (xmlSubReader.Read())
                {
                    if (xmlSubReader.NodeType == XmlNodeType.Element && xmlSubReader.Name == "parameter")
                    {
                        functionDocumentationNode.ParametersModifiers.Add(xmlSubReader.GetAttribute("left"));
                    }
                }
            }
            return functionDocumentationNode;
        }

        private static FullNameDocIndexNode GetFullNameDocumentationNode(XmlReader xmlReader)
        {
            return new FullNameDocIndexNode
                   {
                       Name = xmlReader.GetAttribute("name"),
                       FullName = xmlReader.GetAttribute("fullname"),
                       Location = xmlReader.GetAttribute("location"),
                       LineNumber = int.Parse(xmlReader.GetAttribute("lineno")),
                       HRef = xmlReader.GetAttribute("href"),
                       IsObsolete = xmlReader.GetAttribute("status") == "obsolete"
                   };
        }

        private static DocIndexNode GetDocumentationNode(XmlReader xmlReader)
        {
            return new DocIndexNode
                   {
                       Name = xmlReader.GetAttribute("name"),
                       Location = xmlReader.GetAttribute("location"),
                       LineNumber = int.Parse(xmlReader.GetAttribute("lineno")),
                       HRef = xmlReader.GetAttribute("href"),
                       IsObsolete = xmlReader.GetAttribute("status") == "obsolete"
                   };
        }

        public bool Exists { get; set; }

        private void CollectMembersDocumentation(HtmlNode documentRoot, string docFile)
        {
            var members = new Dictionary<string, List<MemberDocumentationNode>>();
            this.membersDocumentation.Add(docFile, members);
            foreach (var fn in documentRoot.Descendants("h3").Where(
                n =>
                {
                    var @class = n.GetAttributeValue("class", "");
                    return @class == "fn" || @class == "flags";
                }))
            {
                var nodes = new List<MemberDocumentationNode>();
                var node = fn;
                while (node != null && node.NodeType != HtmlNodeType.Comment)
                {
                    if (!string.IsNullOrWhiteSpace(node.OuterHtml))
                    {
                        nodes.Add(GetDocumentationNode(node));
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

        private static MemberDocumentationNode GetDocumentationNode(HtmlNode node)
        {
            var memberDocumentationNode = new MemberDocumentationNode();
            memberDocumentationNode.Name = node.Name;
            memberDocumentationNode.OuterHtml = node.OuterHtml;
            memberDocumentationNode.InnerHtml = node.InnerHtml;
            memberDocumentationNode.InnerText = node.InnerText;
            if (memberDocumentationNode.ContainsEnumMembers =
                (node.Name == "div" && node.FirstChild.GetAttributeValue("class", string.Empty) == "valuelist") ||
                (node.Name == "p" && node.GetAttributeValue("class", string.Empty) == "figCaption"))
            {
                foreach (var code in node.Descendants("code"))
                {
                    var codeNode = GetDocumentationNode(code);
                    memberDocumentationNode.Codes.Add(codeNode);
                    codeNode.ParentOfParent = GetDocumentationNode(code.ParentNode.ParentNode);
                    foreach (var td in code.ParentNode.ParentNode.Descendants("td"))
                    {
                        codeNode.ParentOfParent.Td.Add(GetDocumentationNode(td));
                    }
                    if (codeNode.ParentOfParent.Td.Count <= 2)
                    {
                        codeNode.ParentOfParent.ParentOfParent = memberDocumentationNode;
                    }
                }
            }
            return memberDocumentationNode;
        }

        private void CollectTypesDocumentation(HtmlNode documentRoot, string docFile)
        {
            var typeNode = documentRoot.Descendants("h2").FirstOrDefault(n => n.GetAttributeValue("id", "") == "details");
            if (typeNode != null)
            {
                var nodes = new List<DocumentationNode>();
                var node = typeNode.NextSibling;
                while (node != null)
                {
                    if (!string.IsNullOrWhiteSpace(node.OuterHtml))
                    {
                        nodes.Add(new DocumentationNode(node.OuterHtml, node.InnerHtml));
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
                            var newLine = node.OwnerDocument.CreateTextNode();
                            nodes.Add(new DocumentationNode(newLine.OuterHtml, newLine.InnerHtml));
                            addedNewLine = true;
                        }
                        nodes.Add(new DocumentationNode(node.OuterHtml, node.InnerHtml));
                    }
                    node = node.NextSibling;
                }
                if (nodes.Count > 0)
                {
                    this.typesDocumentation.Add(docFile, nodes);
                }
            }
        }

        public void DocumentFunction(Function function)
        {
            if (!this.functionNodes.ContainsKey(function.OriginalName))
            {
                return;
            }

            var lineStart = function.LineNumberStart;
            var lineEnd = function.LineNumberEnd;
            var functions = this.functionNodes[function.OriginalName];
            var unit = function.OriginalNamespace.TranslationUnit;
            var location = unit.FileName;
            FunctionDocIndexNode node = null;
            var nodes = functions.FindAll(
                f => f.Location == location &&
                    (f.LineNumber == lineStart || f.LineNumber == lineEnd));
            // incredible but we actually have a case of different functions with the same name in headers with the same name at the same line
            if (nodes.Count > 0)
            {
                if (nodes.Count == 1)
                {
                    node = nodes[0];
                }
                else
                {
                    node = nodes.Find(n => n.FullName == function.QualifiedOriginalName);
                }
            }
            var @params = function.Parameters.Where(p => p.Kind == ParameterKind.Regular).ToList();
            int realParamsCount = @params.Count(p => !string.IsNullOrWhiteSpace(p.OriginalName) || p.DefaultArgument == null);
            // functions can have different line numbers because of #defines
            if (node == null || node.HRef == null)
            {
                nodes = functions.FindAll(
                    f => CheckLocation(f.Location, location) &&
                         (f.FullName == function.QualifiedOriginalName || f.Name == function.OriginalName) &&
                         f.Access != "private" && f.ParametersModifiers.Count == realParamsCount);
                // HACK: work around https://bugreports.qt.io/browse/QTBUG-53994
                if (nodes.Count == 1)
                {
                    node = nodes[0];
                }
                else
                {
                    Generator.CurrentOutputNamespace = unit.Module.OutputNamespace;
                    var paramTypes = @params.Select(p => p.Type.ToString()).ToList();
                    node = nodes.Find(
                        f => f.ParametersModifiers.SequenceEqual(paramTypes, new TypeInIndexEqualityComparer()));
                }
            }
            if (node != null && node.HRef != null)
            {
                var link = node.HRef.Split('#');
                var file = link[0];
                if (this.membersDocumentation.ContainsKey(file))
                {
                    var id = link[1].Split('-');
                    var key = EscapeId(function.IsAmbiguous && node.Access == "private" ? id[0] : link[1]);
                    if (this.membersDocumentation[file].ContainsKey(key))
                    {
                        var docs = this.membersDocumentation[file][key];
                        var i = 0;
                        // HACK: work around https://bugreports.qt.io/browse/QTBUG-53941
                        if (function.Namespace.Name == "QByteArray" &&
                            ((function.OriginalName == "qCompress" && @params.Count == 2) ||
                            (function.OriginalName == "qUncompress" && @params.Count == 1)))
                        {
                            docs = this.membersDocumentation[file][key + "-hack"];
                        }
                        foreach (Match match in regexParameters.Matches(docs[0].InnerHtml))
                        {
                            // variadic and void "parameters" are invalid
                            if (function.IsVariadic && @params.Count == i || match.Groups[1].Value == "void")
                                break;
                            @params[i++].Name = Helpers.SafeIdentifier(match.Groups[1].Value);
                        }
                        // TODO: create links in the "See Also" section
                        function.Comment = new RawComment
                        {
                            BriefText = StripTags(ConstructDocumentText(docs.Skip(1)))
                        };
                        if (node.IsObsolete)
                        {
                            AddObsoleteAttribute(function);
                        }
                    }
                }
            }
        }

        private static bool CheckLocation(string indexLocation, string location)
        {
            if (indexLocation == location)
            {
                return true;
            }
            // work around https://bugreports.qt.io/browse/QTBUG-53946
            if (Path.GetExtension(indexLocation) != ".h" && indexLocation.Contains('_'))
            {
                var i = indexLocation.IndexOf('_');
                if (i >= 0)
                {
                    return indexLocation.Substring(0, i) + ".h" == location;
                }
            }
            return false;
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
            string alternativeName = property.OriginalName.Length == 1 ? property.OriginalName :
                                     "is" + StringHelpers.Capitalize(property.OriginalName);
	        foreach (var macroExpansion in properties)
	        {
		        var name = macroExpansion.Text.Split(' ')[1];
		        if (name == property.OriginalName || name == alternativeName)
		        {
                    property.OriginalName = name;
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
                        if (string.IsNullOrEmpty(comment.BriefText))
                        {
                            comment.BriefText += setter.Comment.BriefText;
                        }
                        else
                        {
                            comment.BriefText += Environment.NewLine + setter.Comment.BriefText;
                        }
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
            if (!this.propertyNodes.ContainsKey(property.OriginalName))
            {
                return;
            }
            var qualifiedName = property.GetQualifiedName(decl => decl.OriginalName, decl => decl.Namespace);
            var node = this.propertyNodes[property.OriginalName].Find(c => c.FullName == qualifiedName);
	        if (node != null && node.HRef != null)
			{
				var link = node.HRef.Split('#');
				var file = link[0];
		        if (this.membersDocumentation.ContainsKey(file))
		        {
		            var typeDocs = this.membersDocumentation[file];
                    var key = string.Format("{0}-prop", property.OriginalName);
		            var containsKey = typeDocs.ContainsKey(key);
		            if (!containsKey)
		            {
		                key = property.OriginalName;
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
                c => c.Location == type.TranslationUnit.FileName && c.Name == type.OriginalName);
            if (node != null)
            {
                var file = node.HRef;
                if (this.typesDocumentation.ContainsKey(file))
                {
                    var docs = this.typesDocumentation[file];
                    var briefText = StripTags(docs[0].InnerHtml);
                    var text = StripTags(ConstructDocumentText(docs.Skip(1).Select(d => d.OuterHtml)));
                    type.Comment = new RawComment
                    {
                        BriefText = briefText,
                        // TODO: create links in the "See Also" section; in general, convert all links
                        Text = text,
                        FullComment = new FullComment()
                    };
                    var summary = new ParagraphComment();
                    summary.Content.Add(new TextComment { Text = briefText });
                    type.Comment.FullComment.Blocks.Add(summary);
                    var remarks = new ParagraphComment();
                    remarks.Content.AddRange(text.Split('\n').Select(t => new TextComment { Text = t }));
                    type.Comment.FullComment.Blocks.Add(remarks);
                }
            }
        }

        public void DocumentEnum(Enumeration @enum)
        {
            var node = this.enumNodes.Find(
                c => c.Location == @enum.TranslationUnit.FileName && c.Name == @enum.OriginalName);
            if (node != null)
            {
                var link = node.HRef.Split('#');
                var file = link[0];
                if (this.membersDocumentation.ContainsKey(file))
                {
                    var key = Regex.Escape(link[1]);
                    if (this.membersDocumentation[file].ContainsKey(key))
                    {
                        var docs = this.membersDocumentation[file][key];
                        var enumMembersDocs = new List<MemberDocumentationNode>();
                        for (var i = docs.Count - 1; i >= 0; i--)
                        {
                            var doc = docs[i];
                            if (doc.ContainsEnumMembers)
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
                                                  from code in member.Codes
                                                  where code.InnerText == itemQualifiedName
                                                  select code.ParentOfParent).FirstOrDefault();
                            if (enumMemberDocs != null)
                            {
                                if (enumMemberDocs.Td.Count > 2)
                                {
                                    item.Comment = new RawComment
                                    {
                                        BriefText = StripTags(enumMemberDocs.Td.Last().InnerText).Trim()
                                    };
                                }
                                else
                                {
                                    enumMemberDocs = enumMemberDocs.ParentOfParent;
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
            var lineStart = variable.LineNumberStart;
            var lineEnd = variable.LineNumberEnd;
            var node = this.variableNodes.Find(
                f => f.Location == variable.TranslationUnit.FileName &&
                     (f.LineNumber == lineStart || f.LineNumber == lineEnd));
            if (node != null)
            {
                var link = node.HRef.Split('#');
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
                        if (node.IsObsolete)
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
            var dataSource = Path.Combine(docsPath, string.Format("{0}.qch", module.ToLowerInvariant()));
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

        private static string ConstructDocumentText(IEnumerable<string> html)
        {
            return string.Join("\n", html);
        }

        private static string ConstructDocumentText(IEnumerable<DocumentationNode> html)
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

        private readonly Dictionary<string, List<DocumentationNode>> typesDocumentation = new Dictionary<string, List<DocumentationNode>>();
        private readonly Dictionary<string, Dictionary<string, List<MemberDocumentationNode>>> membersDocumentation = new Dictionary<string, Dictionary<string, List<MemberDocumentationNode>>>();

        private readonly Dictionary<string, List<FunctionDocIndexNode>> functionNodes = new Dictionary<string, List<FunctionDocIndexNode>>();
        private readonly Dictionary<string, List<FullNameDocIndexNode>> propertyNodes = new Dictionary<string, List<FullNameDocIndexNode>>();
        private readonly List<DocIndexNode> classNodes = new List<DocIndexNode>();
        private readonly List<DocIndexNode> enumNodes = new List<DocIndexNode>();
        private readonly List<DocIndexNode> variableNodes = new List<DocIndexNode>();

        private Regex regexParameters = new Regex(@"<i>\s*(.+?)\s*</i>", RegexOptions.Compiled);

        private class TypeInIndexEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Contains(y);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
