using System.Collections;
using System.Collections.Immutable;

namespace TSharp.Compilation.Utils
{
	public sealed class ImmutableListBuilder<TItem> : IEnumerable
	{
		private readonly ImmutableList<TItem>.Builder builder = ImmutableList.CreateBuilder<TItem>();

		public ImmutableListBuilder<TItem> Add(TItem item)
		{
			builder.Add(item);
			return this;
		}

		public ImmutableList<TItem> Build()
		{
			return builder.ToImmutable();
		}

		public IEnumerator GetEnumerator()
		{
			return builder.GetEnumerator();
		}
	}
}