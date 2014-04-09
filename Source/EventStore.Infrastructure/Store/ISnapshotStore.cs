using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Store
{
    public class SnapshotVersion
    {
        public const long NoSnapshot = -1;

        public long LastEventId { get; set; }

        public SnapshotVersion()
        {
            LastEventId = NoSnapshot;
        }

        public SnapshotVersion(long version)
        {
            this.LastEventId = version;
        }
    }

    public interface ISnapshotStore
    {
        SnapshotVersion LoadSnapshot();
        void SaveSnapshot(SnapshotVersion version);
    }
}
