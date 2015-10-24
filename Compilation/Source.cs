using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;

namespace TSharp.Compilation
{
	public sealed class SourceCode
	{
		public static SourceCode Empty { get; } = new SourceCode(new string[] { }, 0, 0);

		private readonly string[] graphemes;
		private readonly int start, length;

		private SourceCode(string[] graphemes, int start, int length)
		{
			Contract.Requires(graphemes != null);
			Contract.Requires(start >= 0);
			Contract.Requires(start + length <= graphemes.Length);
			this.graphemes = graphemes;
			this.start = start;
			this.length = length;
		}
		
		/// <summary>
		/// Gets the source code grapheme at the given zero-based index.
		/// </summary>
		/// <param name="index">The zero-based index of the grapheme to retrieve.</param>
		/// <returns>The grapheme at the given index.</returns>
		public string this[int index] { get { return index >= 0 && index < graphemes.Length ? graphemes[index] : string.Empty; } }

		/// <summary>
		/// The number of graphemes in the <see cref="SourceCode"/>.
		/// </summary>
		public int Length { get { return graphemes.Length; } }

		public SourceCode Subrange(int start, int length)
		{
			Contract.Requires(start >= 0);
			Contract.Requires(start + length <= Length);
			return new SourceCode(graphemes, start, length);
		}

		public SourcePosition Convert(SourceOffset offset)
		{
			Contract.Requires(offset.Offset >= 0);
			Contract.Requires(offset.Offset < Length);

			// TODO: a data structure to make this efficient.
			int row = 1; int col = 1;
			for (int i = 0; i < offset.Offset; i++)
			{
				switch (graphemes[i])
				{
					case "\r":
					case "\n":
					case "\r\n":
						row++;
						col = 1;
						break;
					default:
						col++;
						break;
				}
			}

			return new SourcePosition(row, col);
		}
		
		public static SourceCode FromText(string text)
		{
			Contract.Requires(text != null);
			text = text.Normalize(NormalizationForm.FormD);
			var graphemesBuilder = new List<string>(text.Length);
			var enumerator = StringInfo.GetTextElementEnumerator(text);
			while (enumerator.MoveNext())
			{
				graphemesBuilder.Add(enumerator.GetTextElement());
			}
			var graphemes = graphemesBuilder.ToArray();
			return new SourceCode(graphemes, 0, graphemes.Length);
		}

		public static SourceCode FromFile(string path, Encoding encoding)
		{
			Contract.Requires(!string.IsNullOrEmpty(path));
			Contract.Requires(encoding != null);
			return FromText(File.ReadAllText(path, encoding));
		}

		public static implicit operator SourceCode(string text)
		{
			return FromText(text);
		}
	}

	public struct SourcePosition
	{
		public int Row { get; private set; }
		public int Column { get; private set; }

		public SourcePosition(int row, int column)
		{
			Contract.Requires(row > 0);
			Contract.Requires(column > 0);
			Row = row;
			Column = column;
		}
	}

	public struct SourceOffset
	{
		internal int Offset { get; private set; }

		public SourceOffset(int offset)
		{
			Contract.Requires(offset >= 0);
			Offset = offset;
		}

		public override string ToString()
		{
			return string.Format("SourceLocation(Offset={0})", Offset);
		}

		public override bool Equals(object obj)
		{
			return obj is SourceOffset && this == (SourceOffset)obj;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = (int)2166136261;
				hash = hash * 16777619 ^ Offset;
				return hash;
			}
		}

		public static bool operator ==(SourceOffset lhs, SourceOffset rhs)
		{
			return lhs.Offset == rhs.Offset;
		}

		public static bool operator !=(SourceOffset lhs, SourceOffset rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator <(SourceOffset lhs, SourceOffset rhs)
		{
			return lhs.Offset < rhs.Offset;
		}

		public static bool operator >(SourceOffset lhs, SourceOffset rhs)
		{
			return rhs < lhs;
		}

		public static bool operator <=(SourceOffset lhs, SourceOffset rhs)
		{
			return lhs < rhs || lhs == rhs;
		}

		public static bool operator >=(SourceOffset lhs, SourceOffset rhs)
		{
			return lhs > rhs || lhs == rhs;
		}
	}

	public struct SourceSpan
	{
		public static SourceSpan Zero => new SourceSpan(0);
		public static SourceSpan One => new SourceSpan(1);

		internal int Size { get; private set; }

		public SourceSpan(int size)
		{
			Contract.Requires(size >= 0);
			this.Size = size;
		}

		public override bool Equals(object obj)
		{
			return obj is SourceSpan && this == (SourceSpan)obj;
		}

		public override int GetHashCode()
		{
			return Size.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("SourceSpan(Size={0})", Size);
		}

		public static bool operator ==(SourceSpan lhs, SourceSpan rhs)
		{
			return lhs.Size == rhs.Size;
		}

		public static bool operator !=(SourceSpan lhs, SourceSpan rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator SourceSpan(int size)
		{
			Contract.Requires(size >= 0);
			return new SourceSpan(size);
		}
	}

	public struct SourceRange
	{
		/// <summary>
		/// Inclusive lower bound.
		/// </summary>
		public SourceOffset Start { get; set; }
		/// <summary>
		/// Exclusive upper bound.
		/// </summary>
		public SourceOffset End { get; set; }

		public SourceRange(SourceOffset start, SourceOffset end)
		{
			Contract.Requires(start <= end);
			Start = start;
			End = end;
		}

		public override bool Equals(object obj)
		{
			return obj is SourceRange && this == (SourceRange)obj;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = (int)2166136261;
				hash = hash * 16777619 ^ Start.GetHashCode();
				hash = hash * 16777619 ^ Start.GetHashCode();
				return hash;
			}
		}

		public override string ToString()
		{
			return string.Format("SourceRange(Start={0}, End={1})", Start, End);
		}

		public static bool operator==(SourceRange lhs, SourceRange rhs)
		{
			return lhs.Start == rhs.Start && lhs.End == rhs.End;
		}

		public static bool operator!=(SourceRange lhs, SourceRange rhs)
		{
			return !(lhs == rhs);
		}
	}
}