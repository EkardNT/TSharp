using System;
using System.Diagnostics.Contracts;

namespace TSharp.Compilation
{
	public struct Maybe<T> where T : class
	{
		private readonly T value;

		internal Maybe(T value)
		{
			this.value = value;
		}

		public bool Present { get { return value != null; } }
		public bool Absent { get { return !Present; } }

		public T Value
		{
			get
			{
				Contract.Requires(Present);
				return value;
			}
		}

		public T Or(T alternative)
		{
			return Present ? value : alternative;
		}

		public Maybe<T2> Map<T2>(Func<T, T2> transform) where T2 : class
		{
			Contract.Requires(transform != null);
			return Present ? Maybe.Of(transform(value)) : Maybe.OfNone<T2>();
		}

		public Maybe<T> If(Action<T> action)
		{
			Contract.Requires(action != null);
			if (Present)
				action(value);
			return this;
		}

		public Maybe<T> Else(Action action)
		{
			Contract.Requires(action != null);
			if (Absent)
				action();
			return this;
		}
	}

	public class Maybe
	{
		public static Maybe<E> Of<E>(E value) where E : class
		{
			Contract.Requires(value != null);
			return new Maybe<E>(value);
		}

		public static Maybe<E> OfAny<E>(E value) where E : class
		{
			return new Maybe<E>(value);
		}

		public static Maybe<E> OfNone<E>() where E : class
		{
			return new Maybe<E>(null);
		}
	}
}