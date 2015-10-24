using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TSharp.Compilation.Utils
{
	public sealed class ImmutableDictionaryBuilder<TKey, TValue> : IEnumerable
	{
		private readonly ImmutableDictionary<TKey, TValue>.Builder builder;

		public ImmutableDictionaryBuilder()
		{
			builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
		}

		public ImmutableDictionaryBuilder(IEqualityComparer<TKey> keyComparer)
		{
			builder = ImmutableDictionary.CreateBuilder<TKey, TValue>(keyComparer);
		}

		public ImmutableDictionaryBuilder(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
		{
			builder = ImmutableDictionary.CreateBuilder<TKey, TValue>(keyComparer, valueComparer);
		}

		public ImmutableDictionaryBuilder<TKey, TValue> Add(TKey key, TValue value)
		{
			builder.Add(key, value);
			return this;
		}

		public ImmutableDictionary<TKey, TValue> Build()
		{
			return builder.ToImmutable();
		}

		public IEnumerator GetEnumerator()
		{
			return builder.GetEnumerator();
		}
	}
}