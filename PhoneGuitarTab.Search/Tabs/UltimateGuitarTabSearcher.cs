using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using PhoneGuitarTab.Search.Arts;

namespace PhoneGuitarTab.Search.Tabs
{
    public class UltimateGuitarTabSearcher : ITabSearcher
    {
        private const string RequestTemplateAll =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=200&type[]=300&type[]=400&type[]=500&type[]=700&type[]=800&version_la=&iphone=1&page={2}&tab_type_group=all";

        private const string RequestTemplateGuitarPro =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=500&version_la=&iphone=1&page={2}&tab_type_group=all";

        private const string RequestTemplateGuitar =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=200&version_la=&iphone=1&page={2}";

        private const string RequestTemplateBass =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=400&version_la=&iphone=1&page={2}";

        private const string RequestTemplateChords =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=300&version_la=&iphone=1&page={2}";

        private const string RequestTemplateDrum =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=700&version_la=&iphone=1&page={2}";

        private readonly Dictionary<string, string> _tabTypeMapping = new Dictionary<string, string>
        {
            {"tab pro", "guitar pro"}
        };

       
        public SearchTabResultSummary Summary { get; private set; }
        public List<SearchTabResultEntry> Entries { get; private set; }

        public event DownloadStringCompletedEventHandler SearchComplete;

        private void InvokeSearchComplete(DownloadStringCompletedEventArgs e)
        {
            DownloadStringCompletedEventHandler handler = SearchComplete;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        ///     Handler for WebClient that parses xml data into specific objects
        ///     throws specific exceptions if error occurs
        /// </summary>
        public void Run(string group, string song, int pageNumber, TabulatureType type, ResultsSortOrder sortBy)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    if (e.Error == null)
                    {
                        using (XmlReader reader = XmlReader.Create(new StringReader(e.Result)))
                        {
                            while (reader.Read())
                            {
                                if ((reader.Name == "results") && (reader.IsStartElement()))
                                    Summary = CreateResultSummary(reader);

                                if ((reader.Name == "result") && (reader.IsStartElement()))
                                    Entries.Add(CreateResultEntry(reader));
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    InvokeSearchComplete(e);
                }
            };
            Summary = new SearchTabResultSummary();
            Entries = new List<SearchTabResultEntry>();
            client.DownloadStringAsync(new Uri(String.Format(GetRequestTemplate(type, sortBy), group, song, pageNumber)));
        }

        #region Factory methods

        private SearchTabResultEntry CreateResultEntry(XmlReader reader)
        {
            var type = reader["type"] ?? "";
            return new SearchTabResultEntry
            {
                Id = reader["id"],
                Name = reader["name"],
                Artist = reader["artist"],
                Version = reader["version"],
                Url = reader["url"],
                Rating = reader["rating"],
                Votes = Int32.Parse(reader["votes"] ?? "0"),
                Type = _tabTypeMapping.ContainsKey(type) ? _tabTypeMapping[type] : type
            };
        }

        private SearchTabResultSummary CreateResultSummary(XmlReader reader)
        {
            return new SearchTabResultSummary
            {
                TotalResults = Int32.Parse(reader["count"] ?? "0"),
                PageCount = Int32.Parse(reader["pages"] ?? "0"),
                TotalSongs = Int32.Parse(reader["total"] ?? "0"),
                TotalSongsFound = Int32.Parse(reader["total_found"] ?? "0"),
                BandsFound = Int32.Parse(reader["bands_found"] ?? "0")
            };
        }

        #endregion

        private string GetRequestTemplate(TabulatureType tabType, ResultsSortOrder sortBy)
        {
            string requestTemplate;
            string alphabeticalSortParams = "&order_mode=ASC&order=title_srt";
            switch (tabType)
            {
                case TabulatureType.All:
                    if (sortBy == ResultsSortOrder.Alphabetical)
                        requestTemplate = RequestTemplateAll + alphabeticalSortParams;
                    else
                        requestTemplate = RequestTemplateAll;
                    break;
                case TabulatureType.Guitar:
                    if (sortBy == ResultsSortOrder.Alphabetical)
                        requestTemplate = RequestTemplateGuitar + alphabeticalSortParams;
                    else
                        requestTemplate = RequestTemplateAll;
                    break;
                case TabulatureType.Bass:
                    if (sortBy == ResultsSortOrder.Alphabetical)
                        requestTemplate = RequestTemplateBass + alphabeticalSortParams;
                    else
                        requestTemplate = RequestTemplateAll;
                    break;
                case TabulatureType.Chords:
                    if (sortBy == ResultsSortOrder.Alphabetical)
                        requestTemplate = RequestTemplateChords + alphabeticalSortParams;
                    else
                        requestTemplate = RequestTemplateAll;
                    break;
                case TabulatureType.Drum:
                    if (sortBy == ResultsSortOrder.Alphabetical)
                        requestTemplate = RequestTemplateDrum + alphabeticalSortParams;
                    else
                        requestTemplate = RequestTemplateAll;
                    break;
                case TabulatureType.GuitarPro:
                    if (sortBy == ResultsSortOrder.Alphabetical)
                        requestTemplate = RequestTemplateGuitarPro + alphabeticalSortParams;
                    else
                        requestTemplate = RequestTemplateAll;
                    break;
                default:
                    requestTemplate = RequestTemplateAll;
                    break;
            }

            return requestTemplate;
        }
    }
}