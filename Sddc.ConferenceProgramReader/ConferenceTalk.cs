using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sddc.ProgramReader
{
    /// <summary>
    ///  A talk on a conference.
    /// </summary>
    public class ConferenceTalk
    {
        /// <summary>
        /// Begin time of the talk.
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// End time of the talk.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Title of the talk.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the talk.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Speakers name.
        /// </summary>
        public string Speaker { get; set; }

    }
}
