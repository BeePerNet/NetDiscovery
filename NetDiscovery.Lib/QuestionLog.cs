using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public class QuestionLog
    {
        public DateTime Timestamp => Question.CreationTime;

        public IPEndPoint From { get; }
        public Question Question { get; }

        internal QuestionLog(IPEndPoint from, Question question)
        {
            From = from;
            Question = question;
        }
    }
}
