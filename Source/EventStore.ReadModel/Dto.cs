using System;

namespace Documently.ReadModel
{
	public abstract class Dto
	{
		public string Id
		{
			get { return GetDtoIdOf(AggregateRootId, GetType()); }
		}

		public Guid AggregateRootId { get; set; }

		public static string GetDtoIdOf<T>(Guid id) where T : Dto
		{
			return GetDtoIdOf(id, typeof (T));
		}

		public static string GetDtoIdOf(Guid id, Type type)
		{
			return type.Name + "/" + id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Dto);
        }

        public virtual bool Equals(Dto other)
        {
            return null != other && other.Id == this.Id;
        }
	}
}