using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain
{
    public class CompetenceDocument
    {
        public Guid CompetenceId { get; set; }

        public override int GetHashCode()
        {
            return this.CompetenceId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as CompetenceDocument);
        }

        public virtual bool Equals(CompetenceDocument other)
        {
            return null != other && other.CompetenceId == this.CompetenceId;
        }
    }
}
