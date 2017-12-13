using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud.Realtime.Internal
{
    internal class ReadCommand : AVIMCommand
    {
        internal class ConvRead
        {
            internal string ConvId { get; set; }
            internal string MessageId { get; set; }
            internal float Timestamp { get; set; }
            public override bool Equals(object obj)
            {
                ConvRead cr = obj as ConvRead;
                return cr.ConvId == this.ConvId;
            }
            public override int GetHashCode()
            {
                return this.ConvId.GetHashCode() ^ this.MessageId.GetHashCode() ^ this.Timestamp.GetHashCode();
            }
        }

        public ReadCommand()
            : base(cmd: "read")
        {

        }

        public ReadCommand(AVIMCommand source)
            : base(source)
        {

        }

        public ReadCommand ConvId(string convId)
        {
            return new ReadCommand(this.Argument("cid", convId));
        }

        public ReadCommand ConvIds(IEnumerable<string> convIds)
        {
            if (convIds != null)
            {
                if (convIds.Count() > 0)
                {
                    return new ReadCommand(this.Argument("cids", convIds.ToList()));
                }
            }
            return this;

        }

        internal ReadCommand Convs(IEnumerable<ConvRead> convReads)
        {
            if (convReads != null)
            {
                if (convReads.Count() > 0)
                {
                    var payload = convReads.Select(convRead => new Dictionary<string, object>()
                    {
                        { "cid", convRead.ConvId},
                        { "mid",string.IsNullOrEmpty(convRead.MessageId)?convRead.MessageId:""},
                        { "timestamp", convRead.Timestamp!=0? convRead.Timestamp:DateTime.Now.UnixTimeStampSeconds()},
                    }).ToList();
                    return new ReadCommand(this.Argument("convs", payload));
                }
            }
            return this;
        }
    }
}
