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
    /// AVIMM essage pager.
    /// </summary>
    public class AVIMMessagePager
    {
        public AVIMMessageQuery Query { get; private set; }

        public int PageSize
        {
            get
            {
                return Query.Limit;
            }

            private set
            {
                Query.Limit = value;
            }
        }

        public string CurrentMessageIdFlag
        {
            get
            {
                return Query.StartMessageId;
            }
            private set
            {
                Query.StartMessageId = value;
            }
        }
        public DateTime CurrentDateTimeFlag
        {
            get
            {
                return Query.From;
            }
            private set
            {
                Query.From = value;
            }
        }

        internal AVIMMessagePager()
        {

        }

        public AVIMMessagePager(AVIMConversation conversation)
            : this()
        {
            Query = conversation.GetMessageQuery();
            PageSize = 20;
            CurrentDateTimeFlag = DateTime.Now;
        }

        public AVIMMessagePager SetPageSize(int pageSize)
        {
            PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// Previouses the async.
        /// </summary>
        /// <returns>The async.</returns>
        public Task<IEnumerable<IAVIMMessage>> PreviousAsync()
        {
            return Query.FindAsync().OnSuccess(t =>
            {
                var headerMessage = t.Result.FirstOrDefault();
                if (headerMessage != null)
                {
                    CurrentMessageIdFlag = headerMessage.Id;
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
            return Query.ReverseFindAsync().OnSuccess(t =>
                {
                    var tailMessage = t.Result.LastOrDefault();
                    if (tailMessage != null)
                    {
                        CurrentMessageIdFlag = tailMessage.Id;
                        CurrentDateTimeFlag = tailMessage.ServerTimestamp.ToDateTime();
                    }
                    return t.Result;
                });
        }
    }

    /// <summary>
    /// history message interator.
    /// </summary>
    public class AVIMMessageQuery
    {
        /// <summary>
        /// Gets or sets the convsersation.
        /// </summary>
        /// <value>The convsersation.</value>
        public AVIMConversation Convsersation { get; set; }
        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        /// <value>The limit.</value>
        public int Limit { get; set; }
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public DateTime From { get; set; }
        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public DateTime To { get; set; }
        /// <summary>
        /// Gets or sets the end message identifier.
        /// </summary>
        /// <value>The end message identifier.</value>
        public string EndMessageId { get; set; }
        /// <summary>
        /// Gets or sets the start message identifier.
        /// </summary>
        /// <value>The start message identifier.</value>
        public string StartMessageId { get; set; }


        internal AVIMMessageQuery()
        {
            Limit = 20;
            From = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Realtime.AVIMMessageQuery"/> class.
        /// </summary>
        /// <param name="conversation">Conversation.</param>
        public AVIMMessageQuery(AVIMConversation conversation)
            : this()
        {
            Convsersation = conversation;
        }

        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <returns>The limit.</returns>
        /// <param name="limit">Limit.</param>
        public AVIMMessageQuery SetLimit(int limit)
        {
            Limit = limit;
            return this;
        }

        /// <summary>
        /// from lastest to previous.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<IAVIMMessage>> FindAsync()
        {
            return FindAsync<IAVIMMessage>();
        }

        /// <summary>
        /// from lastest to previous.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<IAVIMMessage>> ReverseFindAsync()
        {
            return ReverseFindAsync<IAVIMMessage>();
        }

        /// <summary>
        /// Finds the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="reverse">set direction to reverse,it means query direct is from old to new.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public Task<IEnumerable<T>> FindAsync<T>(bool reverse = false)
                    where T : IAVIMMessage
        {
            return Convsersation.QueryMessageAsync<T>(
                beforeTimeStampPoint: From,
                afterTimeStampPoint: To,
                limit: Limit,
                afterMessageId: EndMessageId,
                beforeMessageId: StartMessageId,
                direction: reverse ? 0 : 1);
        }

        /// <summary>
        /// from previous to lastest.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<T>> ReverseFindAsync<T>()
            where T : IAVIMMessage
        {
            return FindAsync<T>(true);
        }
    }
}
