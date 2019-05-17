// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Sharpcms.Base.Library.Process;
using Sharpcms.Data.SiteTree;

namespace Sharpcms.Providers.Search
{
    public class Search
    {
        /// <summary>
        /// How many items can be showed on one page.
        /// </summary>
        private const int MaxResults = 5;

        private readonly Page _currentPage;
        private readonly string _indexDir;
        private readonly Process _process;
        private readonly string _searchPage;

        /// <summary>
        /// Time it took to make the search.
        /// </summary>
        private TimeSpan _duration;

        /// <summary>
        /// First item on page (user format).
        /// </summary>
        private int _fromItem;

        /// <summary>
        /// the SearchQuery
        /// </summary>
        private string _searchQuery;

        private int _startAt;

        /// <summary>
        /// First item on page (index format).
        /// </summary>
        private int _startFirstAt;

        /// <summary>
        /// Last item on page (user format).
        /// </summary>
        private int _toItem;

        /// <summary>
        /// Total items returned by search.
        /// </summary>
        private int _total;

        public Search(Process process)
        {
            _process = process;

            string mainVal = _process.QueryData["mainvalue"];
            if (string.IsNullOrEmpty(mainVal))
            {
                string startAt = _process.QueryData["start"];
                _startAt = string.IsNullOrEmpty(startAt) ? 0 : int.Parse(startAt);
            }
            else
            {
                _startAt = int.Parse(mainVal);
            }

            _indexDir = _process.Settings["search/index"];

            //jig: search only one section
            string[] s = _process.CurrentProcess.Split('/');
            if (s.Length >= 2)
            {
                _indexDir = Path.Combine(_indexDir, s[1]);
            }

            _searchPage = "show/";

            string pageId = _process.QueryData["pageidentifier"];
            _currentPage = new SiteTree(_process).GetPage(pageId);
        }

        public int StartAt
        {
            set { _startAt = value; }
        }

        private string SearchQuery
        {
            get { return _searchQuery; }
        }

        /// <summary>
        /// Prepares the string with seach summary information.
        /// </summary>
        private string Summary
        {
            get
            {
				return _total > 0 
					? string.Format("Results <b>{0} - {1}</b> of <b>{2}</b> for <b>{3}</b>. ({4} mili seconds)", _fromItem, _toItem, _total, SearchQuery, _duration.TotalMilliseconds) 
					: "No results found";
            }
        }

        /// <summary>
        /// How many pages are there in the results.
        /// </summary>
        private int PageCount
        {
            get { return (_total - 1) / MaxResults; } // floor 
        }

        /// <summary>
        /// First item of the last page
        /// </summary>
        private int LastPageStartsAt
        {
            get { return PageCount * MaxResults; }
        }

        /// <summary>
        /// Does the search and stores the information about the results.
        /// </summary>
        public void HandleSearch(string searchQuery)
        {
            DateTime start = DateTime.Now;
            _searchQuery = searchQuery;

            // create the searcher
            // index is placed in "index" subdirectory
            var searcher = new IndexSearcher(_indexDir);
            Analyzer analyzer = new StandardAnalyzer();

            // parse the query, "text" is the default field to search
            var query = QueryParser.Parse(_searchQuery, "text", analyzer);

            const string containerName = "content";
            Container container = _currentPage.Containers[containerName];
            const string resultElementName = "result";
            const string pagingElementName = "paging";
            const string summaryElementName = "summary";
            const string allElementNames = resultElementName + pagingElementName + summaryElementName;
            int count = container.Elements.Count;

            // Remove previous search result
            for (int i = count; i > 0; --i)
            {
                if (container.Elements[i] == null) continue;

                if (allElementNames.IndexOf(container.Elements[i].Type, StringComparison.Ordinal) > -1)
                {
                    container.Elements.Remove(i);
                }
            }

            Element queryElement = container.Elements[0];
            Element element = container.Elements.Create(summaryElementName);
            queryElement["query"] = _searchQuery;

            // search
            Hits hits = searcher.Search(query);

            _total = hits.Length();

            // create highlighter
            var highlighter = new Highlighter(new QueryScorer(query));

            // initialize startAt
            _startFirstAt = InitStartAt();

            // how many items we should show - less than defined at the end of the results
            int resultsCount = SmallerOf(_total, MaxResults + _startFirstAt);

            for (int i = _startFirstAt; i < resultsCount; i++)
            {
                // get the document from index
                Document document = hits.Doc(i);
                string path = document.Get("url");

                if (path != null)
                {
                    string plainText = document.Get("text");

                    TokenStream tokenStream = analyzer.TokenStream("text", new StringReader(plainText));
                    string text = highlighter.GetBestFragments(tokenStream, plainText, 2, "...");

                    element = container.Elements.Create(resultElementName);
                    element["title"] = document.Get("title");
                    element["path"] = _searchPage + path.Replace("\\", "/") + "/";
                    element["sample"] = string.IsNullOrEmpty(text) ? plainText : text;
                }
            }

            searcher.Close();

            _duration = DateTime.Now - start;
            _fromItem = _startFirstAt + 1;
            _toItem = SmallerOf(_startFirstAt + MaxResults, _total);

            // result information
            element.Node.InnerText = Summary;

            // paging link
            element = container.Elements.Create(pagingElementName);
            element.Node.InnerText = SetPaging();
            
            _process.SearchContext = _currentPage;

            _currentPage.Save();
        }

        /// <summary>
        /// Returns the smaller value of parameters.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static int SmallerOf(int first, int second)
        {
            return first < second ? first : second;
        }

        /// <summary>
        /// Page links. DataTable might be overhead but there used to be more fields in previous version so I'm keeping it for now.
        /// </summary>
        private string SetPaging()
        {
            // pageNumber starts at 1
            int pageNumber = (_startFirstAt + MaxResults - 1) / MaxResults;

            var htmlList = new List<string>();
            string html = PagingItemHtml(pageNumber + 1, false);
            htmlList.Add(html);

            const int previousPagesCount = 4;
            for (int i = pageNumber - 1; i >= 0 && i >= pageNumber - previousPagesCount; i--)
            {
                string htm = PagingItemHtml(i + 1, true);
                htmlList.Insert(0, htm);
            }

            const int nextPagesCount = 4;
            for (int i = pageNumber + 1; i <= PageCount && i <= pageNumber + nextPagesCount; i++)
            {
                string htm = PagingItemHtml(i + 1, true);
                htmlList.Add(htm);
            }

            var sb = new StringBuilder();
            foreach (string htm in htmlList)
            {
                sb.Append(htm);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Prepares HTML of a paging item (bold number for current page, links for others).
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <returns></returns>
        private static string PagingItemHtml(int number, bool active)
        {
            if (active)
            {
                return string.Format("<a href=\"javascript:ThrowEvent('{0}', '');\"> {1} </a>", (MaxResults * (number - 1)), number);
            }

            return "<b>" + number + "</b>";
        }

        /// <summary>
        /// Initializes startAt value. Checks for bad values.
        /// Request.Params["start"]
        /// </summary>
        /// <returns></returns>
        private int InitStartAt()
        {
            try
            {
                int startAt = Convert.ToInt32(_startAt);

                // too small starting item, return first page
                if (_startAt < 0)
                {
                    return 0;
                }

                // too big starting item, return last page
                return _startAt >= _total - 1 ? LastPageStartsAt : startAt;
            }
            catch
            {
                return 0;
            }
        }

    }
}