using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Util;
using CppSharp.AST;
using CppSharp.Types;
using Mono.Data.Sqlite;
using zlib;
using Type = CppSharp.AST.Type;

namespace QtSharp
{
    public class Documentation
    {
        private readonly IDictionary<string, string> documentation;
        private readonly Dictionary<string, List<TypedefDecl>> typeDefsPerType;

        private static readonly Regex regexTypeName =
            new Regex(@"^(const +)?(?<name>((un)?signed +)?.+?(__\*)?)( *(&|((\*|(\[\]))+)) *)?$",
                RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static readonly Regex regexArg = new Regex(@"^(.+?\s+)(?<name>\w+)(\s*=\s*[^\(,\s]+(\(\s*\))?)?\s*$",
            RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static readonly Regex regexStaticDocs = new Regex("Related Non-Members(?<static>.+)",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        public Documentation(string docsPath, string module, Dictionary<Type, List<TypedefDecl>> typeDefsPerType)
        {
            this.documentation = Get(docsPath, module);
            CppTypePrinter cppTypePrinter = new CppTypePrinter(new TypeMapDatabase());
            cppTypePrinter.PrintLocalName = true;
            this.typeDefsPerType = new Dictionary<string, List<TypedefDecl>>();
            foreach (KeyValuePair<Type, List<TypedefDecl>> typeTypeDefs in typeDefsPerType)
            {
                if (!(typeTypeDefs.Key is DependentNameType) && !(typeTypeDefs.Key is InjectedClassNameType))
                {
                    string typeName = typeTypeDefs.Key.Visit(cppTypePrinter);
                    if (!this.typeDefsPerType.ContainsKey(typeName))
                    {
                        this.typeDefsPerType.Add(typeName, typeTypeDefs.Value);                        
                    }
                }
            }
        }

        //public void DocumentMember(Smoke* smoke, Smoke.Method* smokeMethod, CodeTypeMember cmm, CodeTypeDeclaration type)
        //{
        //    if (type.Name == this.data.GlobalSpaceClassName || this.translator.NamespacesAsClasses.Contains(type.Name))
        //    {
        //        this.DocumentMember(smoke, smokeMethod, cmm, @"\w+", this.staticDocumentation, false);
        //    }
        //    else
        //    {
        //        if (this.memberDocumentation.ContainsKey(type))
        //        {
        //            this.DocumentMember(smoke, smokeMethod, cmm, type.Name.Substring(type.IsInterface ? 1 : 0),
        //                                this.memberDocumentation[type]);
        //        }
        //    }
        //}

        //public void DocumentMember(string signature, CodeTypeMember cmm, CodeTypeDeclaration type)
        //{
        //    if (this.memberDocumentation.ContainsKey(type))
        //    {
        //        int index = signature.IndexOf('(');
        //        string methodName = signature.Substring(0, signature.IndexOf('('));
        //        string[] argTypes = signature.Substring(index + 1, signature.Length - index - 2)
        //                                     .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //        this.memberDocumentation[type].Where((t, i) => this.TryMatch(type.Name, methodName, cmm, t, i > 0, argTypes, false)).Any();
        //    }
        //}

        //private void DocumentMember(Smoke* smoke, Smoke.Method* smokeMethod, CodeTypeMember cmm, string type, IEnumerable<string> docs, bool markObsolete = true)
        //{
        //    string methodName = Regex.Escape(ByteArrayManager.GetString(smoke->methodNames[smokeMethod->name]));
        //    string[] argTypes = smoke->GetArgs(smokeMethod).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        //    docs.Where((t, i) => this.TryMatch(type, methodName, cmm, t, i > 0 && markObsolete, argTypes)).Any();
        //}

        public void DocumentFunction(Function function)
        {
            string file = GetFileForDeclarationContext(function.Namespace);
            if (this.documentation.ContainsKey(file) && this.TryMatch(function, this.documentation[file], false))
            {
                return;
            }
            file = file.Replace(".html", "-obsolete.html");
            if (this.documentation.ContainsKey(file))
            {
                this.TryMatch(function, this.documentation[file], true);
            }
        }

        public void DocumentProperty(Property property)
        {
            var expansions = property.Namespace.PreprocessedEntities.OfType<MacroExpansion>();

            var properties = expansions.Where(e => e.Text.Contains("Q_PROPERTY") || e.Text.Contains("QDOC_PROPERTY"));
            string alternativeName = property.Name.Length == 1 ? property.Name :
                                     "is" + char.ToUpperInvariant(property.Name[0]) + property.Name.Substring(1);
            foreach (string name in from macroExpansion in properties
                                    let name = macroExpansion.Text.Split(new[] { ' ' })[1]
                                    where name == property.Name || name == alternativeName
                                    select name)
            {
                property.Name = name;
                this.DocumentQtProperty(property);
                return;
            }
            if (property.Field == null)
            {
                Method getter = property.GetMethod;
                if (getter.Comment == null)
                {
                    this.DocumentFunction(getter);
                }
                var comment = new RawComment();
                if (getter.Comment != null)
                {
                    comment.BriefText = getter.Comment.BriefText;
                }
                Method setter = property.SetMethod;
                if (setter != null)
                {
                    if (setter.Comment == null)
                    {
                        this.DocumentFunction(setter);
                    }
                    if (setter.Comment != null)
                    {
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

        private void DocumentQtProperty(Property property)
        {
            string file = GetFileForDeclarationContext(property.Namespace);
            if (this.documentation.ContainsKey(file))
            {
                string docs = this.documentation[file];
                CppTypePrinter cppTypePrinter = new CppTypePrinter(new TypeMapDatabase());
                cppTypePrinter.PrintLocalName = true;
                string type = property.Type.Visit(cppTypePrinter);
                Match match = Regex.Match(docs, "Property Documentation.*" + property.Name + @" : (const )?(\w+::)?" + type.Replace("*", @"\s*\*") +
                                          @"(\s+const)?\n(?<docs>.*?)\nAccess functions:", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                if (match.Success)
                {
                    property.Comment = new RawComment();
                    property.Comment.BriefText = match.Groups["docs"].Value;
                }
            }
        }

        public void DocumentType(Class type)
        {
            string file = GetFileForDeclarationContext(type);
            if (this.documentation.ContainsKey(file))
            {
                string typeDocs = this.documentation[file];
                Match match = Regex.Match(typeDocs, string.Format(@"(?<class>((The {0})|(This class)).+?)More\.\.\..*?\n" +
                                                                   @"Detailed Description\s+(?<detailed>.*?)(\n){{3,}}", type.Name),
                                          RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                if (match.Success)
                {
                    type.Comment = new RawComment();
                    type.Comment.BriefText = match.Groups["class"].Value.Trim();
                    type.Comment.Text = match.Groups["detailed"].Value.Replace(match.Groups["class"].Value.Trim(), string.Empty);
                }
            }
        }

        public void DocumentEnum(Enumeration @enum)
        {
            string file = GetFileForDeclarationContext(@enum.Namespace);
            if (this.documentation.ContainsKey(file))
            {
                string typeDocs = this.documentation[file];
                const string enumDoc = @"enum {0}(\s*flags {1}::\w+\s+)?(?<docsStart>.*?)(\n){{3}}";
                string enumName = string.IsNullOrEmpty(@enum.Namespace.Name) ? @enum.Name : (@enum.Namespace.Name + "::" + @enum.Name);
                Match matchEnum = Regex.Match(typeDocs, string.Format(enumDoc, enumName, @enum.Namespace.Name),
                                              RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                if (matchEnum.Success)
                {
                    string doc = matchEnum.Groups["docsStart"].Value.Trim();
                    if (!string.IsNullOrEmpty(doc))
                    {
                        doc = Regex.Replace(doc,
                                            @"(The \S+ type is a typedef for QFlags<\S+>\. It stores an OR combination of \S+ values\.)",
                                            string.Empty);
                        doc = Regex.Replace(doc, @"ConstantValue(Description)?.*?(((\n){2})|$)", string.Empty,
                                            RegexOptions.Singleline | RegexOptions.ExplicitCapture).Trim();
                        @enum.Comment = new RawComment();
                        @enum.Comment.BriefText = doc;
                    }
                }
            }
        }

        public void DocumentEnumItem(Enumeration @enum, Enumeration.Item enumItem)
        {
            string file = GetFileForDeclarationContext(@enum.Namespace);
            if (this.documentation.ContainsKey(file))
            {
                string typeDocs = this.documentation[file];
                string typeName;
                string memberName;
                if (string.IsNullOrEmpty(@enum.Namespace.Name))
                {
                    typeName = @enum.Name;
                    memberName = enumItem.Name;
                }
                else
                {
                    typeName = @enum.Namespace.Name + "::" + @enum.Name;
                    memberName = @enum.Namespace.Name + "::" + enumItem.Name;
                }
                const string memberDoc = @"enum {0}.*?{1}\t[^\t\n]+\t(?<docs>.*?)(&\w+;)?(\n)";
                Match match = Regex.Match(typeDocs, string.Format(memberDoc, typeName, memberName),
                                          RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                if (match.Success)
                {
                    string doc = match.Groups["docs"].Value.Trim();
                    if (!string.IsNullOrEmpty(doc))
                    {
                        enumItem.Comment = doc;
                    }
                }
            }
        }

        private string GetFileForDeclarationContext(DeclarationContext declarationContext)
        {
            if (string.IsNullOrEmpty(declarationContext.Name))
            {
                TranslationUnit unit = declarationContext as TranslationUnit;
                if (unit != null)
                {
                    string file = unit.FileNameWithoutExtension + ".html";
                    if (this.documentation.ContainsKey(file))
                    {
                        return file;
                    }
                }
                return "qtglobal.html";
            }
            StringBuilder fileNameBuilder = new StringBuilder(declarationContext.Name.ToLowerInvariant());
            if (declarationContext is Class && ((Class) declarationContext).IsInterface)
            {
                fileNameBuilder.Remove(0, 1);
            }
            Class parentClass = declarationContext.Namespace as Class;
            if (parentClass != null)
            {
                fileNameBuilder.Insert(0, string.Format("{0}-", parentClass.Name.ToLowerInvariant()));
            }
            fileNameBuilder.Append(".html");
            return fileNameBuilder.ToString();
        }

        private static IDictionary<string, string> Get(string docsPath, string module)
        {
            if (!Directory.Exists(docsPath))
            {
                return new Dictionary<string, string>();
            }
            try
            {
                IDictionary<string, string> documentation = GetFromHtml(docsPath, module);
                return documentation.Count > 0 ? documentation : GetFromQch(docsPath, module);
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
            return Directory.GetFiles(docs, "*.html").ToDictionary(Path.GetFileName,
                f => StripTags(new StringBuilder(File.ReadAllText(f)).Replace("\r", string.Empty).Replace(@"\", @"\\").ToString()));
        }

        private static IDictionary<string, string> GetFromQch(string docsPath, string module)
        {
            string dataSource = Path.Combine(docsPath, string.Format("qt{0}.qch", module.ToLowerInvariant()));
            if (!File.Exists(dataSource))
            {
                return new Dictionary<string, string>();
            }
            SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder();
            sqliteConnectionStringBuilder.DataSource = dataSource;
            using (SqliteConnection sqliteConnection = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString))
            {
                sqliteConnection.Open();
                using (SqliteCommand sqliteCommand = new SqliteCommand(
                    "SELECT Name, Data FROM FileNameTable INNER JOIN FileDataTable ON FileNameTable.FileId = FileDataTable.Id " +
                    "WHERE Name LIKE '%.html' " +
                    "ORDER BY Name", sqliteConnection))
                {
                    using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
                    {
                        Dictionary<string, string> documentation = new Dictionary<string, string>();
                        while (sqliteDataReader.Read())
                        {
                            byte[] blob = new byte[ushort.MaxValue];
                            int length = (int) sqliteDataReader.GetBytes(1, 0, blob, 0, blob.Length);
                            using (MemoryStream output = new MemoryStream(length - 4))
                            {
                                using (ZOutputStream zOutputStream = new ZOutputStream(output))
                                {
                                    zOutputStream.Write(blob, 4, length - 4);
                                    zOutputStream.Flush();
                                    documentation.Add(sqliteDataReader.GetString(0),
                                                      StripTags(Encoding.UTF8.GetString(output.ToArray()).Replace(@"\", @"\\")));
                                }
                            }
                        }
                        return documentation;
                    }
                }
            }
        }

        private static string StripTags(string source)
        {
            char[] array = new char[source.Length];
            List<char> tagArray = new List<char>();
            int arrayIndex = 0;
            bool inside = false;

            foreach (char @let in source)
            {
                if (@let == '<')
                {
                    inside = true;
                    continue;
                }
                if (@let == '>')
                {
                    inside = false;
                    continue;
                }
                if (inside)
                {
                    tagArray.Add(@let);
                }
                else
                {
                    string tag = new string(tagArray.ToArray());
                    if (tag.Contains("/tdtd"))
                    {
                        array[arrayIndex++] = '\t';
                    }
                    tagArray.Clear();
                    array[arrayIndex++] = @let;
                }
            }
            return HtmlEncoder.HtmlDecode(new string(array, 0, arrayIndex));
        }

        private bool TryMatch(Function function, string docs, bool markObsolete, bool completeSignature = true)
        {
            const string memberDoc = @"(^|( --)|\n)\n([\w :*&<>,]+)?(({0}(\s*&)?::)| ){1}(const)?( \[(\w+\s*)+\])?\n\W*(?<docs>.*?)(\n\s*){{1,2}}((&?\S* --)|((\n\s*){{2}}))";
            const string separator = @",\s*";
            StringBuilder signatureRegex = new StringBuilder(Regex.Escape(function.OriginalName)).Append(@"\s*\(\s*(");
            bool anyArgs = false;
            CppTypePrinter cppTypePrinter = new CppTypePrinter(new TypeMapDatabase());
            cppTypePrinter.PrintLocalName = true;
            foreach (string argType in function.Parameters.Where(p => p.Kind == ParameterKind.Regular).Select(p => p.Type.Visit(cppTypePrinter)))
            {
                if (!anyArgs)
                {
                    signatureRegex.Append("?<args>");
                    anyArgs = true;
                }
                signatureRegex.Append(this.GetTypeRegex(argType, completeSignature)).Append(@"(\s+\w+(\s*=\s*[^,\r\n]+(\(\s*\))?)?)?");
                signatureRegex.Append(separator);
            }
            if (anyArgs)
            {
                signatureRegex.Insert(signatureRegex.Length - separator.Length, '(');
            }
            else
            {
                signatureRegex.Append('(');
            }
            signatureRegex.Append(@"[\w :*&<>]+\s*=\s*[^,\r\n]+(\(\s*\))?(,\s*)?)*)\s*\)\s*");
            Match match = Regex.Match(docs, string.Format(memberDoc, function.Namespace.Name, signatureRegex),
                                      RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            if (match.Success)
            {
                function.Comment = new RawComment();
                function.Comment.BriefText = match.Groups["docs"].Value;
                FillMissingParameterNames(function, match.Groups["args"].Value);
                return true;
            }
            return false;
        }

        private string GetTypeRegex(string argType, bool completeSignature = true)
        {
            string typeName = regexTypeName.Match(argType).Groups["name"].Value;
            StringBuilder typeBuilder = new StringBuilder(typeName);
            this.FormatType(typeBuilder);
            typeBuilder.Insert(0, @"(const +)?(((\w+::)?");
            typeBuilder.Append(')');
            if (this.typeDefsPerType.ContainsKey(typeName))
            {
                foreach (string typeDef in (from typedef in this.typeDefsPerType[typeName]
                                            select typedef.OriginalName).Distinct())
                {
                    StringBuilder typeDefBuilder = new StringBuilder(typeDef);
                    this.FormatType(typeDefBuilder);
                    typeBuilder.Append("|(").Append(typeDefBuilder).Append(')');
                }
            }
            // C++ long is mapped to int
            switch (argType)
            {
                case "int":
                case "int*":
                    typeBuilder.Append(@"|(long)");
                    break;
                case "unsigned int":
                case "unsigned int*":
                    typeBuilder.Append(@"|(unsigned\s+long)");
                    break;
            }
            typeBuilder.Append(@")");
            if (completeSignature)
            {
                if (argType.EndsWith("&", StringComparison.Ordinal) && !typeName.EndsWith("&", StringComparison.Ordinal))
                {
                    typeBuilder.Append(@" *& *");
                }
                else
                {
                    if (argType.EndsWith("*", StringComparison.Ordinal) && !typeName.EndsWith("*", StringComparison.Ordinal))
                    {
                        typeBuilder.Append(@" *(\*|(\[\]))+ *");
                    }
                }
            }
            else
            {
                typeBuilder.Append(@"( *(&|((\*|(\[\]))+)) *)?");
            }
            return typeBuilder.ToString();
        }

        private void FormatType(StringBuilder typeBuilder)
        {
            int indexOfLt = Int32.MinValue;
            int indexOfGt = Int32.MinValue;
            int indexOfColon = Int32.MinValue;
            int firstColonIndex = Int32.MinValue;
            List<int> commas = new List<int>();
            List<char> templateType = new List<char>(typeBuilder.Length);
            for (int i = typeBuilder.Length - 1; i >= 0; i--)
            {
                char @char = typeBuilder[i];
                switch (@char)
                {
                    case '<':
                        indexOfLt = i;
                        break;
                    case '>':
                        indexOfGt = i;
                        break;
                    case ':':
                        if (firstColonIndex < 0)
                        {
                            firstColonIndex = i;
                        }
                        else
                        {
                            if (i == firstColonIndex - 1)
                            {
                                indexOfColon = firstColonIndex;
                                firstColonIndex = Int32.MinValue;
                            }
                        }
                        break;
                    case ',':
                        commas.Add(i);
                        break;
                }
                if (i > indexOfLt && i < indexOfGt)
                {
                    typeBuilder.Remove(i, 1);
                    templateType.Insert(0, @char);
                }
            }
            if (indexOfGt > indexOfLt)
            {
                typeBuilder.Replace("(", @"\(").Replace(")", @"\)");
                typeBuilder.Replace(@"*", @"\s*(\*|(\[\]))").Replace(@"&", @"\s*&").Replace(",", @",\s*");
                typeBuilder.Insert(indexOfLt + 1, this.GetTypeRegex(new string(templateType.ToArray())));
            }
            else
            {
                if (indexOfColon > 0)
                {
                    int comma = int.MinValue;
                    IEnumerable<int> last = commas.Where(i => i < indexOfColon).ToList();
                    if (last.Any())
                    {
                        comma = last.Last();
                    }
                    int parentTypeStart = Math.Max(Math.Max(indexOfLt + 1, 0), comma + 1);
                    typeBuilder.Remove(parentTypeStart, indexOfColon + 1 - parentTypeStart);
                    typeBuilder.Insert(parentTypeStart, @"(\w+::)?");
                    typeBuilder.Replace(@"*", @"\s*(\*|(\[\]))").Replace(@"&", @"\s*&").Replace(",", @",\s*");
                }
                else
                {
                    typeBuilder.Replace("(", @"\(").Replace(")", @"\)");
                    typeBuilder.Replace(@"*", @"\s*(\*|(\[\]))").Replace(@"&", @"\s*&").Replace(",", @",\s*");
                }
            }
        }

        private static void FillMissingParameterNames(Function function, string signature)
        {
            List<string> args = new List<string>(signature.Split(','));
            if (args.Count < function.Parameters.Count)
            {
                // operator
                args.Insert(0, "one");
            }
            List<string> argNames = (from arg in args
                                     select regexArg.Match(arg).Groups["name"].Value).ToList();
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                Parameter parameter = function.Parameters[i];
                string oldArgName = parameter.Name;
                int index = oldArgName.IndexOf(" = ", StringComparison.Ordinal);
                string nameOnly = index > 0 ? oldArgName.Substring(0, index) : oldArgName;
                if (nameOnly.StartsWith("_", StringComparison.Ordinal) && char.IsDigit(nameOnly[1]))
                {
                    string name = argNames[i];
                    if (!string.IsNullOrEmpty(name))
                    {
                        parameter.Name = name + (index > 0 ? oldArgName.Substring(index) : string.Empty);
                    }
                }
            }
        }

        //private static void FormatComment(string docs, CodeTypeMember cmp, bool obsolete = false, string tag = "summary")
        //{
        //    StringBuilder obsoleteMessageBuilder = new StringBuilder();
        //    cmp.Comments.Add(new CodeCommentStatement(string.Format("<{0}>", tag), true));
        //    foreach (string line in HtmlEncoder.HtmlEncode(docs).Split(Environment.NewLine.ToCharArray()))
        //    {
        //        cmp.Comments.Add(new CodeCommentStatement(string.Format("<para>{0}</para>", line), true));
        //        if (obsolete && (line.Contains("instead") || line.Contains("deprecated")))
        //        {
        //            obsoleteMessageBuilder.Append(HtmlEncoder.HtmlDecode(line));
        //            obsoleteMessageBuilder.Append(' ');
        //        }
        //    }
        //    cmp.Comments.Add(new CodeCommentStatement(string.Format("</{0}>", tag), true));
        //    if (obsolete)
        //    {
        //        if (obsoleteMessageBuilder.Length > 0)
        //        {
        //            obsoleteMessageBuilder.Remove(obsoleteMessageBuilder.Length - 1, 1);
        //        }
        //        CodeSnippetTypeMember snippet = cmp as CodeSnippetTypeMember;
        //        if (snippet != null)
        //        {
        //            const string template = "        [System.ObsoleteAttribute(\"{0}\")]{1}";
        //            snippet.Text = snippet.Text.Insert(0, string.Format(template, obsoleteMessageBuilder, Environment.NewLine));
        //        }
        //        else
        //        {
        //            CodeTypeReference obsoleteAttribute = new CodeTypeReference(typeof(ObsoleteAttribute));
        //            CodePrimitiveExpression obsoleteMessage = new CodePrimitiveExpression(obsoleteMessageBuilder.ToString());
        //            cmp.CustomAttributes.Add(new CodeAttributeDeclaration(obsoleteAttribute, new CodeAttributeArgument(obsoleteMessage)));
        //        }
        //    }
        //}
    }
}
