using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Sddc.ProgramReader
{
    /// <summary>
    /// A reader that can read the conference program from the webpage.
    /// </summary>
    public class ProgramReader
    {
        /// <summary>
        /// URI of the webpage.
        /// </summary>
        private Uri uri;

        /// <summary>
        /// The content of the webpage.
        /// </summary>
        private byte[] webContent;

        /// <summary>
        /// The list of parsed conference talks.
        /// </summary>
        private List<ConferenceTalk> conferenceTalks;

        /// <summary>
        /// The list of parsed conference talks.
        /// </summary>
        public IEnumerable<ConferenceTalk> ConferenceTalks
        {
            get
            {
                return conferenceTalks;
            }
        }

        /// <summary>
        /// Constructs a new program reader.
        /// </summary>
        /// <param name="uri">The URI of the webpage.</param>
        public ProgramReader(string uri)
        {
            this.uri = new Uri(uri);
        }

        /// <summary>
        /// Downloads the web content.
        /// </summary>
        private void DownloadWebContent()
        {
            using (WebClient webClient = new WebClient())
            {
                webContent = webClient.DownloadData(uri);
            }
        }

        /// <summary>
        /// Parses the web content to get conference talks.
        /// </summary>
        private void ParseWebContent()
        {
            conferenceTalks = new List<ConferenceTalk>();
            using (MemoryStream contentStream = new MemoryStream(webContent))
            {
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.Load(contentStream, Encoding.UTF8);
                var nodes = htmlDocument.DocumentNode.SelectNodes(
                    "//div[@id='program']//div[@class='fusion-row']/div[@class='fusion-one-half fusion-layout-column fusion-spacing-no']//div[div[@class='fusion-date-and-formats']]");
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        ParseWebContentNode(node);
                    }
                }
            }
        }

        /// <summary>
        /// Parses a node containing the talk in the webpage.
        /// </summary>
        /// <param name="node">Node that contains the talk.</param>
        private void ParseWebContentNode(HtmlNode node)
        {
            var times = ParseTimes(node);
            DateTime startDateTime = times.Item1;
            DateTime endDateTime = times.Item2;

            var title = ParseTitle(node);
            var speaker = ParseSpeaker(node);
            var description = ParseDescription(node);

            var conferenceTalk = new ConferenceTalk()
            {
                Title = title,
                Description = description,
                Speaker = speaker,
                Begin = startDateTime,
                End = endDateTime
            };

            conferenceTalks.Add(conferenceTalk);
        }

        /// <summary>
        /// Parses the times from the node.
        /// </summary>
        /// <param name="node">The node containing the talk.</param>
        /// <returns>A tuple with start time as first element and end time as second element.</returns>
        private static Tuple<DateTime,DateTime> ParseTimes(HtmlNode node)
        {
            var dateTimeNodes = node.SelectNodes(".//span[@class='fusion-month-year']");
            if (dateTimeNodes == null || dateTimeNodes.Count < 2)
                throw new FormatException("Cannot parse time information from conference talk.");
            var startDateTimeParts = dateTimeNodes[0].InnerText.Split(':');
            var endDateTimeParts = dateTimeNodes[1].InnerText.Split(':');
            var startDateTime = new DateTime(2016, 04, 18, Convert.ToInt32(startDateTimeParts[0]),
                Convert.ToInt32(startDateTimeParts[1]), 0);
            var endDateTime = new DateTime(2016, 04, 18, Convert.ToInt32(endDateTimeParts[0]),
                Convert.ToInt32(endDateTimeParts[1]), 0);
            return new Tuple<DateTime, DateTime>(startDateTime, endDateTime);
        }

        /// <summary>
        /// Parses the description from the node.
        /// </summary>
        /// <param name="node">The node containing the talk.</param>
        /// <returns>The description.</returns>
        private static string ParseDescription(HtmlNode node)
        {
            var descriptionNode = node.SelectSingleNode(".//div[@class='fusion-post-content-container']/p");
            if (descriptionNode == null)
                throw new FormatException("Cannot parse descripton of conference talk.");
            var description = WebUtility.HtmlDecode(descriptionNode.InnerText.Trim());
            return description;
        }

        /// <summary>
        /// Parses the speaker from the node.
        /// </summary>
        /// <param name="node">The node containing the talk.</param>
        /// <returns>The speakers name.</returns>
        private static string ParseSpeaker(HtmlNode node)
        {
            var speakerNode = node.SelectSingleNode(".//p/a");
            if (speakerNode == null)
                throw new FormatException("Cannot parse speaker name from the conference talk.");
            var speaker = WebUtility.HtmlDecode(speakerNode.InnerText.Trim());
            return speaker;
        }

        /// <summary>
        /// Parses the title from the node.
        /// </summary>
        /// <param name="node">The node containing the talk.</param>
        /// <returns>The title.</returns>
        private static string ParseTitle(HtmlNode node)
        {
            var titleNode = node.SelectSingleNode(".//h4/a");
            if (titleNode == null)
                throw new FormatException("Cannot parse title for conference talk.");
            var title = WebUtility.HtmlDecode(titleNode.InnerText.Trim());
            return title;
        }

        /// <summary>
        /// Downloads and parses the webcontent.
        /// </summary>
        public void Parse()
        {
            DownloadWebContent();
            ParseWebContent();
        }
    }
}
