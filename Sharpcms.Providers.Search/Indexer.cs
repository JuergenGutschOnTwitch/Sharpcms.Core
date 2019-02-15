// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Sharpcms.Base.Library.Http;

namespace Sharpcms.Providers.Search
{
    internal class Rule
    {
        public readonly string Name;
        public readonly string Type;
        public readonly string XPath;

        public Rule(string name, string type, string xPath)
        {
            Name = name;
            Type = type;
            XPath = xPath;
        }

        public override string ToString()
        {
            string result = String.Format("Rule '{0}', Type '{1}', XPath '{2}'", Name, Type, XPath);

            return result;
        }
    }

    internal class RuleValue
    {
        public readonly Rule Rule;
        public readonly string Value;

        public RuleValue(Rule rule, string value)
        {
            Rule = rule;
            Value = value;
        }
    }

    internal class DocItem
    {
        private readonly string _key;
        private readonly ArrayList _rules = new ArrayList();
        private readonly string _xPath;

        public DocItem(string xPath, string key)
        {
            _xPath = xPath;
            _key = key;
        }

        public ArrayList Rules
        {
            get
            {
                return _rules;
            }
        }

        public string XPath
        {
            get
            {
                return _xPath;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        private void AddRule(Rule rule)
        {
            _rules.Add(rule);
        }

        public void AddRule(string name, string type, string xPath)
        {
            Rule newRule = new Rule(name, type, xPath);

            AddRule(newRule);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("DocItem: Key '{0}' XPath '{1}'" + Environment.NewLine, Key, XPath);
            stringBuilder.Append("Rules:" + Environment.NewLine);

            foreach (Rule rule in Rules)
            {
                stringBuilder.Append(rule + Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
    }

    internal class RuleSet
    {
        private readonly ArrayList _docItems = new ArrayList();
        private readonly ArrayList _globalRules = new ArrayList();
        private readonly string _name;
        private readonly string _namespace;

        public RuleSet(string name, string nameSpace)
        {
            _name = name;
            _namespace = nameSpace;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        private string Namespace
        {
            get
            {
                return _namespace;
            }
        }

        public ArrayList GlobalRules
        {
            get
            {
                return _globalRules;
            }
        }

        public ArrayList DocItems
        {
            get
            {
                return _docItems;
            }
        }

        private void AddGlobalRule(Rule rule)
        {
            _globalRules.Add(rule);
        }

        public void AddGlobalRule(string name, string type, string xpath)
        {
            Rule newGlobalRule = new Rule(name, type, xpath);

            AddGlobalRule(newGlobalRule);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("RuleSet for document type '{0}', namespace '{1}'" + Environment.NewLine, Name, Namespace);
            stringBuilder.Append("Global rules:" + Environment.NewLine);

            foreach (Rule rule in _globalRules)
            {
                stringBuilder.Append(rule + Environment.NewLine);
            }

            stringBuilder.Append("Document items:" + Environment.NewLine);
            foreach (DocItem docItem in _docItems)
            {
                stringBuilder.Append(docItem);
            }

            return stringBuilder.ToString();
        }

        public DocItem AddDocItem(string xp, string key)
        {
            DocItem addDocItem = new DocItem(xp, key);

            _docItems.Add(addDocItem);

            return addDocItem;
        }
    }

    public class Indexer
    {
        private readonly ArrayList _fileList = new ArrayList();
        private readonly string _indexName;
        private readonly ArrayList _ruleSets;
        private readonly StringBuilder _messageStringBuilder;
        private readonly bool _verbose;
        private string _docRootDirectory;
        private string _pattern;

        public Indexer(string name, bool verbose = false)
        {
            _ruleSets = new ArrayList();
            _verbose = verbose;
            _indexName = name;
            _messageStringBuilder = new StringBuilder();
        }

        public string DocRootDirectory
        {
            get
            {
                return _docRootDirectory;
            }
        }

        public string ProcMessage
        {
            get
            {
                return _messageStringBuilder.ToString();
            }
        }

        /// <summary>
        /// Add HTML files from <c>directory</c> and its subdirectories that match <c>pattern</c>.
        /// </summary>
        /// <param name="directory">Directory with the HTML files.</param>
        /// <param name="pattern">Search pattern, e.g. <c>"*.html"</c></param>
        public void AddDirectory(DirectoryInfo directory, string pattern)
        {
            _docRootDirectory = directory.FullName;
            _pattern = pattern;

            AddSubDirectory(directory);
        }

        public void AddDirectory(string filePath, string pattern)
        {
            AddDirectory(new DirectoryInfo(filePath), pattern);
        }

        private void AddSubDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo fileInfo in directory.GetFiles(_pattern))
            {
                if (_fileList.Contains(fileInfo.FullName))
                {
                    _messageStringBuilder.AppendFormat("Error : {0}\n", fileInfo.FullName);
                }
                else
                {
                    _fileList.Add(fileInfo.FullName);
                }
            }

            foreach (DirectoryInfo directoryInfo in directory.GetDirectories())
            {
                AddSubDirectory(directoryInfo);
            }
        }

        public void LoadRules(string filename)
        {
            XPathDocument pathDocument = new XPathDocument(new StreamReader(filename));
            XPathNavigator pathNavigator = pathDocument.CreateNavigator();
            XPathNodeIterator pathNodeIterator = pathNavigator.Select("/rules/doctype");

            while (pathNodeIterator.MoveNext())
            {
                if (pathNodeIterator.Current != null)
                {
                    XPathNavigator clone = pathNodeIterator.Current.Clone();
                    string root = clone.GetAttribute("root", string.Empty);
                    string nameSpace = clone.GetAttribute("namespace", string.Empty);
                    RuleSet ruleSet = new RuleSet(root, nameSpace);
                    XPathNodeIterator riter = clone.Select("rule");

                    while (riter.MoveNext())
                    {
                        if (riter.Current != null)
                        {
                            ruleSet.AddGlobalRule(riter.Current.GetAttribute("field", string.Empty), riter.Current.GetAttribute("type", string.Empty), riter.Current.GetAttribute("xpath", string.Empty));
                        }
                    }

                    riter = clone.Select("document");

                    while (riter.MoveNext())
                    {
                        if (riter.Current != null)
                        {
                            XPathNavigator xPathNavigator = riter.Current.Clone();
                            string xpath = xPathNavigator.GetAttribute("xpath", string.Empty);
                            string key = xPathNavigator.GetAttribute("key", string.Empty);
                            DocItem docItem = ruleSet.AddDocItem(xpath, key);
                            XPathNodeIterator diter = xPathNavigator.Select("rule");
                            while (diter.MoveNext())
                            {
                                if (diter.Current != null)
                                {
                                    docItem.AddRule(diter.Current.GetAttribute("field", string.Empty), diter.Current.GetAttribute("type", string.Empty), diter.Current.GetAttribute("xpath", string.Empty));
                                }
                            }
                        }
                    }

                    if (_verbose)
                    {
                        _messageStringBuilder.Append(ruleSet + "\n");
                    }

                    _ruleSets.Add(ruleSet);
                }
            }
        }

        private void AddField(Document ludoc, Rule rule, string val)
        {
            switch (rule.Type)
            {
                case "text":
                    ludoc.Add(Field.Text(rule.Name, ParseHtml(val)));
                    break;
                case "keyword":
                    ludoc.Add(Field.Keyword(rule.Name, val));
                    break;
                default:
                    _messageStringBuilder.AppendFormat("Ignoring unknown rule type: {0}\n", rule);
                    break;
            }
        }

        private static void AddKeyField(Document ludoc, string val)
        {
            ludoc.Add(Field.Keyword("_key", val));
        }

        private static void AddNameField(Document ludoc, string val)
        {
            ludoc.Add(Field.Keyword("_name", val));
        }

        private static void DeleteDocument(IndexModifier modifier, string val)
        {
            modifier.Delete(new Term("_key", val));
        }

        public void IndexDocument(string fname)
        {
            ArrayList arrayList = new ArrayList { fname };
            IndexDocuments(arrayList);
        }

        public ArrayList IndexDocuments()
        {
            ArrayList arrayList = IndexDocuments(_fileList);

            return arrayList;
        }

        public ArrayList IndexDocuments(ArrayList fnames)
        {
            ArrayList failures = new ArrayList();
 
            // create index if it doesn't exist
             IndexModifier writer = File.Exists(Path.Combine(_indexName, "segments")) 
                ? new IndexModifier(_indexName, new StandardAnalyzer(), false) 
                : new IndexModifier(_indexName, new StandardAnalyzer(), true);

            foreach (string fname in fnames)
            {
                try
                {
                    if (_verbose)
                    {
                        _messageStringBuilder.AppendFormat("Indexing file: {0}\n", fname);
                    }

                    XPathDocument pathDocument = new XPathDocument(new StreamReader(fname));
                    XPathNavigator pathNavigator = pathDocument.CreateNavigator();
                    pathNavigator.MoveToFirstChild();

                    string docname = pathNavigator.Name;

                    if (_verbose)
                    {
                        _messageStringBuilder.AppendFormat("Found document type: {0}\n", docname);
                    }

                    foreach (RuleSet ruleSet in _ruleSets)
                    {
                        if (ruleSet.Name == docname)
                        {
                            if (_verbose)
                            {
                                _messageStringBuilder.Append("Found matching ruleset, indexing.\n");
                            }

                            pathNavigator.MoveToRoot();

                            // first thing is to read and cache global fields
                            // these are duped for each document found
                            ArrayList globals = new ArrayList();
                            foreach (Rule rule in ruleSet.GlobalRules)
                            {
                                XPathExpression pathExpression = pathNavigator.Compile(rule.XPath);
                                XPathNodeIterator pathNodeIterator = pathNavigator.Select(pathExpression);
                                while (pathNodeIterator.MoveNext())
                                {
                                    if (_verbose)
                                    {
                                        _messageStringBuilder.AppendFormat("Found field '{0}' value '{1}'\n", rule.Name, pathNodeIterator.Current.Value);
                                    }

                                    globals.Add(new RuleValue(rule, pathNodeIterator.Current.Value));
                                }
                            }

                            // ok, all globals cached. now to index documents

                            pathNavigator.MoveToRoot();
                            foreach (DocItem docItem in ruleSet.DocItems)
                            {
                                XPathExpression pathExpression = pathNavigator.Compile(docItem.XPath);
                                XPathNodeIterator pathNodeIterator = pathNavigator.Select(pathExpression);

                                while (pathNodeIterator.MoveNext())
                                {
                                    XPathNavigator clone = pathNodeIterator.Current.Clone();

                                    if (_verbose)
                                    {
                                        _messageStringBuilder.Append("Found document\n");
                                    }

                                    // now compute key
                                    string key = string.Empty;
                                    if (docItem.Key != string.Empty)
                                    {
                                        //key = dn.Evaluate(doc.Key).ToString();

                                        object result = clone.Evaluate(docItem.Key);
                                        XPathNodeIterator nodeIterator = result as XPathNodeIterator;
                                        if (nodeIterator != null)
                                        {
                                            while (nodeIterator.MoveNext())
                                            {
                                                key = nodeIterator.Current.ToString();
                                            }
                                        }
                                        else
                                        {
                                            key = result.ToString();
                                        }
                                    }
                                    else
                                    {
                                        key = fname;
                                    }

                                    if (_verbose)
                                    {
                                        _messageStringBuilder.AppendFormat("Key is {0}\n", key);
                                    }

                                    // delete if document exists
                                    DeleteDocument(writer, key);

                                    Document ludoc = new Document();

                                    AddKeyField(ludoc, key);
                                    AddNameField(ludoc, ruleSet.Name);

                                    string textField = string.Empty;
                                    Rule ruleText = null;
                                    foreach (Rule rule in docItem.Rules)
                                    {
                                        XPathExpression expr = clone.Compile(rule.XPath);
                                        XPathNodeIterator iter = clone.Select(expr);
                                        while (iter.MoveNext())
                                        {
                                            string textVal = ParseHtml(iter.Current.Value);

                                            if (_verbose)
                                            {
                                                _messageStringBuilder.AppendFormat("Found field '{0}' value '{1}'\n", rule.Name, textVal);
                                            }

                                            // index away!
                                            if (rule.Name == "text")
                                            {
                                                if (ruleText == null)
                                                {
                                                    ruleText = rule;
                                                    textField = textVal;
                                                }
                                                else
                                                {
                                                    textField += " ... " + textVal;
                                                }
                                            }
                                            else
                                            {
                                                AddField(ludoc, rule, iter.Current.Value);
                                            }
                                        }
                                    }

                                    if (textField.Length > 0)
                                    {
                                        AddField(ludoc, ruleText, textField);
                                    }

                                    // now add the globals into this document
                                    foreach (RuleValue ruleValue in globals)
                                    {
                                        AddField(ludoc, ruleValue.Rule, ruleValue.Value);
                                    }

                                    writer.AddDocument(ludoc);
                                }
                            }
                        }
                    }
                    // end of fnames loop
                }
                catch (Exception e)
                {
                    failures.Add(String.Format("Failed to index file {0}, exception: {1}", fname, e));
                    if (_verbose)
                    {
                        _messageStringBuilder.AppendFormat("Failed to index {0}\n", fname);
                        _messageStringBuilder.Append(e.StackTrace + "\n");
                    }
                }
            }

            writer.Optimize();
            writer.Close();

            return failures;
        }

        /// <summary>
        /// Very simple, inefficient, and memory consuming HTML parser. Take a look at Demo/HtmlParser in DotLucene package for a better HTML parser.
        /// </summary>
        /// <param name="html">HTML document</param>
        /// <returns>Plain tesxt.</returns>
        private static string ParseHtml(string html)
        {
            html = HttpContext.Current.Server.HtmlDecode(html);
            html = HttpContext.Current.Server.HtmlDecode(html);
            html = Regex.Replace(html, @"<([^>]|\s)*>", " "); //replace all HTML 

            return html;
        }
    }
}