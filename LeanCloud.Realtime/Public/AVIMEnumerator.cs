using LeanCloud.Storage.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeanCloud.Realtime.Internal;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// history message interator.
    /// </summary>
    public class HistoryMessageIterator
    {
       
        public AVIMConversation Convsersation { get; set; }
        public int Limit { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string EndMessageId { get; set; }
        public string StartMessageId { get; set; }

        internal string CurrentMessageIdFlag { get; set; }
        internal DateTime CurrentDateTimeFlag { get; set; }

        internal HistoryMessageIterator()
        {
            Limit = 20;
            From = DateTime.Now;
        }

        /// <summary>
        /// from lastest to previous.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<IAVIMMessage>> PreviousAsync()
        {
            if (CurrentDateTimeFlag == DateTime.MinValue)
            {
                CurrentDateTimeFlag = From;
            }

            return Convsersation.QueryMessageAsync(
                beforeTimeStampPoint: CurrentDateTimeFlag,
                afterTimeStampPoint: To,
                limit: Limit,
                afterMessageId: EndMessageId,
                beforeMessageId: StartMessageId).OnSuccess(t =>
                {
                    var headerMessage = t.Result.FirstOrDefault();
                    if (headerMessage != null)
                    {
                        StartMessageId = headerMessage.Id;
                        CurrentDateTimeFlag = headerMessage.ServerTimestamp.ToDateTime();
                    }
                    return t.Result;
                });
        }

        /// <summary>
        /// from previous to lastest.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<IAVIMMessage>> NextAsync()
        {
            if (CurrentDateTimeFlag == DateTime.MinValue)
            {
                CurrentDateTimeFlag = From;
            }

            return Convsersation.QueryMessageAsync(
                beforeTimeStampPoint: CurrentDateTimeFlag,
                afterTimeStampPoint: To,
                limit: Limit,
                afterMessageId: EndMessageId,
                beforeMessageId: StartMessageId,
                direction: 0).OnSuccess(t =>
                {
                    var tailMessage = t.Result.LastOrDefault();
                    if (tailMessage != null)
                    {
                        EndMessageId = tailMessage.Id;
                        CurrentDateTimeFlag = tailMessage.ServerTimestamp.ToDateTime();
                    }
                    return t.Result;
                });
        }
    }
}
