using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Util;
using CppSharp.AST;
using Mono.Data.Sqlite;
using zlib;

namespace QtSharp
{
    public class Documentation
    {
        private IDictionary<string, string> docs;

        private readonly Dictionary<Class, List<string>> memberDocumentation = new Dictionary<Class, List<string>>();
        private readonly List<string> staticDocumentation = new List<string>();

        private static readonly Regex regexTypeName =
            new Regex(@"^(const +)?(?<name>((un)?signed +)?.+?(__\*)?)( *(&|((\*|(\[\]))+)) *)?$",
                RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static readonly Regex regexArg = new Regex(@"^(.+?\s+)(?<name>\w+)(\s*=\s*[^\(,\s]+(\(\s*\))?)?\s*$",
            RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static readonly Regex regexStaticDocs = new Regex("Related Non-Members(?<static>.+)",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        public Documentation(string docsPath, string module)
        {
            this.docs = Get(docsPath, module);
        }

        //public Documentation(GeneratorData data, Translator translator)
        //{
        //    this.data = data;
        //    this.translator = translator;
        //    this.GatherDocs();
        //}

        //public void DocumentEnumMember(Smoke* smoke, Smoke.Method* smokeMethod, CodeMemberField cmm, CodeTypeDeclaration type)
        //{
        //    CodeTypeDeclaration parentType = this.memberDocumentation.Keys.FirstOrDefault(t => t.Name == (string) type.UserData["parent"]);
        //    IList<string> docs;
        //    string typeName;
        //    string parentName;
        //    if (parentType == null)
        //    {
        //        docs = this.staticDocumentation;
        //        typeName = type.Name;
        //        parentName = string.Empty;
        //    }
        //    else
        //    {
        //        docs = this.memberDocumentation[parentType];
        //        typeName = Regex.Escape(parentType.Name) + "::" + Regex.Escape(type.Name);
        //        parentName = parentType.Name;
        //    }
        //    if (type.Comments.Count == 0)
        //    {
        //        for (int i = 0; i < docs.Count; i++)
        //        {
        //            const string enumDoc = @"enum {0}(\s*flags {1}::\w+\s+)?(?<docsStart>.*?)(\n){{3}}";
        //            Match matchEnum = Regex.Match(docs[i], string.Format(enumDoc, typeName, parentName),
        //                                          RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        //            if (matchEnum.Success)
        //            {
        //                string doc = (matchEnum.Groups["docsStart"].Value + matchEnum.Groups["docsEnd1"].Value).Trim();
        //                doc = Regex.Replace(doc,
        //                                    @"(The \S+ type is a typedef for QFlags<\S+>\. It stores an OR combination of \S+ values\.)",
        //                                    string.Empty);
        //                doc = Regex.Replace(doc,
        //                                    @"ConstantValue(Description)?.*?(((\n){2})|$)",
        //                                    string.Empty, RegexOptions.Singleline | RegexOptions.ExplicitCapture).Trim();
        //                if (!string.IsNullOrEmpty(doc))
        //                {
        //                    FormatComment(doc, type, i > 0 && parentType != null);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    string memberName = (parentType == null ? parentName : Regex.Escape(parentName) + "::") +
        //                        Regex.Escape(ByteArrayManager.GetString(smoke->methodNames[smokeMethod->name]));
        //    const string memberDoc = @"enum {0}.*{1}\t[^\t\n]+\t(?<docs>.*?)(&\w+;)?(\n)";
        //    for (int i = 0; i < docs.Count; i++)
        //    {
        //        Match match = Regex.Match(docs[i], string.Format(memberDoc, typeName, memberName),
        //                                  RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        //        if (match.Success)
        //        {
        //            string doc = match.Groups["docs"].Value.Trim();
        //            if (!string.IsNullOrEmpty(doc))
        //            {
        //                FormatComment(Char.ToUpper(doc[0]) + doc.Substring(1), cmm, i > 0 && parentType != null);
        //                break;
        //            }
        //        }
        //    }
        //}

        //public void DocumentProperty(CodeTypeDeclaration type, string propertyName, string propertyType, CodeMemberProperty cmp)
        //{
        //    if (this.memberDocumentation.ContainsKey(type))
        //    {
        //        IList<string> docs = this.memberDocumentation[type];
        //        for (int i = 0; i < docs.Count; i++)
        //        {
        //            Match match = Regex.Match(docs[i],
        //                                      propertyName + " : (const )?" + propertyType.Replace("*", @"\s*\*") +
        //                                      @"\n(?<docs>.*?)\nAccess functions:",
        //                                      RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        //            if (match.Success)
        //            {
        //                FormatComment(match.Groups["docs"].Value, cmp, i > 0);
        //                break;
        //            }
        //        }
        //    }
        //}

        //public void DocumentAttributeProperty(CodeTypeMember cmm, CodeTypeDeclaration type)
        //{
        //    if (this.memberDocumentation.ContainsKey(type))
        //    {
        //        IList<string> docs = this.memberDocumentation[type];
        //        string typeName = Regex.Escape(type.Name);
        //        string originalName = Char.ToLowerInvariant(cmm.Name[0]) + cmm.Name.Substring(1);
        //        const string memberDoc = @"{0}::{1}\n\W*(?<docs>.*?)(\n\s*){{3}}";
        //        for (int i = 0; i < docs.Count; i++)
        //        {
        //            Match match = Regex.Match(docs[i], string.Format(memberDoc, typeName, originalName),
        //                                      RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        //            if (match.Success)
        //            {
        //                FormatComment(match.Groups["docs"].Value, cmm, i > 0);
        //                break;
        //            }
        //        }
        //    }
        //}

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

        //private bool TryMatch(string type, string methodName, CodeTypeMember cmm, string docs, bool markObsolete,
        //                      IEnumerable<string> argTypes, bool completeSignature = true)
        //{
        //    const string memberDoc = @"(^|( --)|\n)\n([\w :*&<>,]+)?(({0}(\s*&)?::)| ){1}(const)?( \[(\w+\s*)+\])?\n\W*(?<docs>.*?)(\n\s*){{1,2}}((&?\S* --)|((\n\s*){{2}}))";
        //    const string separator = @",\s*";
        //    StringBuilder signatureRegex = new StringBuilder(methodName).Append(@"\s*\(\s*(");
        //    bool anyArgs = false;
        //    foreach (string argType in argTypes)
        //    {
        //        if (!anyArgs)
        //        {
        //            signatureRegex.Append("?<args>");
        //            anyArgs = true;
        //        }
        //        signatureRegex.Append(this.GetTypeRegex(argType, completeSignature)).Append(@"(\s+\w+(\s*=\s*[^,\r\n]+(\(\s*\))?)?)?");
        //        signatureRegex.Append(separator);
        //    }
        //    if (anyArgs)
        //    {
        //        signatureRegex.Insert(signatureRegex.Length - separator.Length, '(');
        //    }
        //    else
        //    {
        //        signatureRegex.Append('(');
        //    }
        //    signatureRegex.Append(@"[\w :*&<>]+\s*=\s*[^,\r\n]+(\(\s*\))?(,\s*)?)*)\s*\)\s*");
        //    Match match = Regex.Match(docs, string.Format(memberDoc, type, signatureRegex),
        //                              RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        //    if (match.Success)
        //    {
        //        FormatComment(match.Groups["docs"].Value, cmm, markObsolete);
        //        FillMissingParameterNames(cmm, match.Groups["args"].Value);
        //        return true;
        //    }
        //    return false;
        //}

        //private string GetTypeRegex(string argType, bool completeSignature = true)
        //{
        //    string typeName = regexTypeName.Match(argType).Groups["name"].Value;
        //    StringBuilder typeBuilder = new StringBuilder(typeName);
        //    this.FormatType(typeBuilder);
        //    typeBuilder.Insert(0, "(const +)?((");
        //    typeBuilder.Append(')');
        //    if (this.data.TypeDefsPerType.ContainsKey(typeName))
        //    {
        //        foreach (StringBuilder typeDefBuilder in from typedef in this.data.TypeDefsPerType[typeName]
        //                                                 select new StringBuilder(typedef))
        //        {
        //            this.FormatType(typeDefBuilder);
        //            typeBuilder.Append("|(").Append(typeDefBuilder).Append(')');
        //        }
        //    }
        //    typeBuilder.Append(@")");
        //    if (completeSignature)
        //    {
        //        if (argType.EndsWith("&", StringComparison.Ordinal) && !typeName.EndsWith("&", StringComparison.Ordinal))
        //        {
        //            typeBuilder.Append(@" *& *");
        //        }
        //        else
        //        {
        //            if (argType.EndsWith("*", StringComparison.Ordinal) && !typeName.EndsWith("*", StringComparison.Ordinal))
        //            {
        //                typeBuilder.Append(@" *(\*|(\[\]))+ *");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        typeBuilder.Append(@"( *(&|((\*|(\[\]))+)) *)?");	
        //    }
        //    return typeBuilder.ToString();
        //}

        //private void FormatType(StringBuilder typeBuilder)
        //{
        //    int indexOfLt = Int32.MinValue;
        //    int indexOfGt = Int32.MinValue;
        //    int indexOfColon = Int32.MinValue;
        //    int firstColonIndex = Int32.MinValue;
        //    List<int> commas = new List<int>();
        //    List<char> templateType = new List<char>(typeBuilder.Length);
        //    for (int i = typeBuilder.Length - 1; i >= 0; i--)
        //    {
        //        char @char = typeBuilder[i];
        //        switch (@char)
        //        {
        //            case '<':
        //                indexOfLt = i;
        //                break;
        //            case '>':
        //                indexOfGt = i;
        //                break;
        //            case ':':
        //                if (firstColonIndex < 0)
        //                {
        //                    firstColonIndex = i;
        //                }
        //                else
        //                {
        //                    if (i == firstColonIndex - 1)
        //                    {
        //                        indexOfColon = firstColonIndex;
        //                        firstColonIndex = Int32.MinValue;
        //                    }
        //                }
        //                break;
        //            case ',':
        //                commas.Add(i);
        //                break;
        //        }
        //        if (i > indexOfLt && i < indexOfGt)
        //        {
        //            typeBuilder.Remove(i, 1);
        //            templateType.Insert(0, @char);
        //        }
        //    }
        //    if (indexOfGt > indexOfLt)
        //    {
        //        typeBuilder.Replace("(", @"\(").Replace(")", @"\)");
        //        typeBuilder.Replace(@"*", @"\s*(\*|(\[\]))").Replace(@"&", @"\s*&").Replace(",", @",\s*");
        //        typeBuilder.Insert(indexOfLt + 1, this.GetTypeRegex(new string(templateType.ToArray())));
        //    }
        //    else
        //    {
        //        if (indexOfColon > 0)
        //        {
        //            int comma = int.MinValue;
        //            IEnumerable<int> last = commas.Where(i => i < indexOfColon).ToList();
        //            if (last.Any())
        //            {
        //                comma = last.Last();
        //            }
        //            int parentTypeStart = Math.Max(Math.Max(indexOfLt + 1, 0), comma + 1);
        //            typeBuilder.Remove(parentTypeStart, indexOfColon + 1 - parentTypeStart);
        //            typeBuilder.Insert(parentTypeStart, @"(\w+::)?");
        //            typeBuilder.Replace(@"*", @"\s*(\*|(\[\]))").Replace(@"&", @"\s*&").Replace(",", @",\s*");
        //        }
        //        else
        //        {
        //            typeBuilder.Replace("(", @"\(").Replace(")", @"\)");
        //            typeBuilder.Replace(@"*", @"\s*(\*|(\[\]))").Replace(@"&", @"\s*&").Replace(",", @",\s*");
        //        }
        //    }
        //}

        //private static void FillMissingParameterNames(CodeTypeMember cmm, string signature)
        //{
        //    CodeMemberMethod method = cmm as CodeMemberMethod;
        //    if (method == null)
        //    {
        //        return;
        //    }
        //    List<string> args = new List<string>(signature.Split(','));
        //    if (args.Count < method.Parameters.Count)
        //    {
        //        // operator
        //        args.Insert(0, "one");
        //    }
        //    MethodsGenerator.RenameParameters(method, (from arg in args
        //                                               select regexArg.Match(arg).Groups["name"].Value).ToList());
        //}

        //private void GatherDocs()
        //{
        //    IDictionary<string, string> documentation = Get(this.data.Docs);
        //    foreach (CodeTypeDeclaration type in from smokeType in this.data.SmokeTypeMap
        //                                         where string.IsNullOrEmpty((string) smokeType.Value.UserData["parent"])
        //                                         select smokeType.Value)
        //    {
        //        foreach (CodeTypeDeclaration nestedType in type.Members.OfType<CodeTypeDeclaration>().Where(t => !t.IsEnum))
        //        {
        //            this.GetClassDocs(nestedType, string.Format("{0}::{1}", type.Name, nestedType.Name),
        //                              string.Format("{0}-{1}", type.Name, nestedType.Name), documentation);
        //        }
        //        this.GetClassDocs(type, type.Name, type.Name, documentation);
        //    }
        //    this.staticDocumentation.AddRange(from k in this.translator.TypeStringMap.Keys.SelectMany(k => new[] { k + ".html", k + "-obsolete.html", k + "-qt3.html" })
        //                                      let key = k.ToLowerInvariant()
        //                                      where documentation.ContainsKey(key)
        //                                      select StripTags(documentation[key]));
        //    this.staticDocumentation.AddRange(from pair in documentation
        //                                      where (pair.Key.StartsWith("q", StringComparison.Ordinal) &&
        //                                             pair.Key.EndsWith("-h.html", StringComparison.Ordinal)) ||
        //                                            pair.Key == "qtglobal.html"
        //                                      select StripTags(pair.Value));
        //}

        //private void GetClassDocs(Class type, string typeName, string fileName, IDictionary<string, string> documentation)
        //{
        //    List<string> docs = new List<string>();
        //    CodeTypeDeclaration @interface = null;
        //    if (this.data.InterfaceTypeMap.ContainsKey(typeName))
        //    {
        //        @interface = this.data.InterfaceTypeMap[typeName];
        //    }
        //    foreach (string docFile in new[] { fileName + ".html", fileName + "-obsolete.html", fileName + "-qt3.html" })
        //    {
        //        if (documentation.ContainsKey(docFile.ToLowerInvariant()))
        //        {
        //            string classDocs = StripTags(documentation[docFile.ToLowerInvariant()]);
        //            Match match = Regex.Match(classDocs, string.Format(@"(?<class>((The {0})|(This class)).+?)More\.\.\..*?\n" +
        //                                                               @"Detailed Description\s+(?<detailed>.*?)(\n){{3,}}" +
        //                                                               @"((\w+ )*\w+ Documentation\n(?<members>.+))", typeName),
        //                                      RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        //            if (match.Success)
        //            {
        //                string members = CommentType(type, match);
        //                docs.Add(members);
        //                if (@interface != null)
        //                {
        //                    CommentType(@interface, match);
        //                }
        //                Match matchStatic = regexStaticDocs.Match(members);
        //                if (matchStatic.Success)
        //                {
        //                    this.staticDocumentation.Add(matchStatic.Groups["static"].Value);
        //                }
        //            }
        //            else
        //            {
        //                docs.Add(classDocs);
        //            }
        //        }
        //    }
        //    this.memberDocumentation[type] = docs;
        //    if (@interface != null)
        //    {
        //        this.memberDocumentation[@interface] = docs;
        //    }
        //}

        public void CommentType(Class type)
        {
            StringBuilder fileNameBuilder = new StringBuilder(type.Name.ToLowerInvariant());
            if (type.IsInterface)
            {
                fileNameBuilder.Remove(0, 1);
            }
            Class parentClass = type.Namespace as Class;
            if (parentClass != null)
            {
                fileNameBuilder.Insert(0, string.Format("{0}-", parentClass.Name.ToLowerInvariant()));
            }
            fileNameBuilder.Append(".html");
            string fileName = fileNameBuilder.ToString();
            if (this.docs.ContainsKey(fileName))
            {
                string classDocs = StripTags(this.docs[fileName]);
                Match match = Regex.Match(classDocs, string.Format(@"(?<class>((The {0})|(This class)).+?)More\.\.\..*?\n" +
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
                f => new StringBuilder(File.ReadAllText(f)).Replace("\r", string.Empty).Replace(@"\", @"\\").ToString());
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
                                    documentation.Add(sqliteDataReader.GetString(0), Encoding.UTF8.GetString(output.ToArray()).Replace(@"\", @"\\"));
                                }
                            }
                        }
                        return documentation;
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
