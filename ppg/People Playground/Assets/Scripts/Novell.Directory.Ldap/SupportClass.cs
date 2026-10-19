using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

public class SupportClass
{
	public class Tokenizer
	{
		private ArrayList elements;

		private string source;

		private string delimiters = " \t\n\r";

		private bool returnDelims;

		public int Count => elements.Count;

		public Tokenizer(string source)
		{
			elements = new ArrayList();
			elements.AddRange(source.Split(delimiters.ToCharArray()));
			RemoveEmptyStrings();
			this.source = source;
		}

		public Tokenizer(string source, string delimiters)
		{
			elements = new ArrayList();
			this.delimiters = delimiters;
			elements.AddRange(source.Split(this.delimiters.ToCharArray()));
			RemoveEmptyStrings();
			this.source = source;
		}

		public Tokenizer(string source, string delimiters, bool retDel)
		{
			elements = new ArrayList();
			this.delimiters = delimiters;
			this.source = source;
			returnDelims = retDel;
			if (returnDelims)
			{
				Tokenize();
			}
			else
			{
				elements.AddRange(source.Split(this.delimiters.ToCharArray()));
			}
			RemoveEmptyStrings();
		}

		private void Tokenize()
		{
			string text = source;
			string text2 = "";
			if (text.IndexOfAny(delimiters.ToCharArray()) < 0 && text.Length > 0)
			{
				elements.Add(text);
			}
			else if (text.IndexOfAny(delimiters.ToCharArray()) < 0 && text.Length <= 0)
			{
				return;
			}
			while (text.IndexOfAny(delimiters.ToCharArray()) >= 0)
			{
				if (text.IndexOfAny(delimiters.ToCharArray()) == 0)
				{
					if (text.Length > 1)
					{
						elements.Add(text.Substring(0, 1));
						text = text.Substring(1);
					}
					else
					{
						text = "";
					}
				}
				else
				{
					text2 = text.Substring(0, text.IndexOfAny(delimiters.ToCharArray()));
					elements.Add(text2);
					elements.Add(text.Substring(text2.Length, 1));
					text = ((text.Length <= text2.Length + 1) ? "" : text.Substring(text2.Length + 1));
				}
			}
			if (text.Length > 0)
			{
				elements.Add(text);
			}
		}

		public bool HasMoreTokens()
		{
			return elements.Count > 0;
		}

		public string NextToken()
		{
			if (source == "")
			{
				throw new Exception();
			}
			string result;
			if (returnDelims)
			{
				RemoveEmptyStrings();
				result = (string)elements[0];
				elements.RemoveAt(0);
				return result;
			}
			elements = new ArrayList();
			elements.AddRange(source.Split(delimiters.ToCharArray()));
			RemoveEmptyStrings();
			result = (string)elements[0];
			elements.RemoveAt(0);
			source = source.Remove(source.IndexOf(result), result.Length);
			source = source.TrimStart(delimiters.ToCharArray());
			return result;
		}

		public string NextToken(string delimiters)
		{
			this.delimiters = delimiters;
			return NextToken();
		}

		private void RemoveEmptyStrings()
		{
			for (int i = 0; i < elements.Count; i++)
			{
				if ((string)elements[i] == "")
				{
					elements.RemoveAt(i);
					i--;
				}
			}
		}
	}

	public class DateTimeFormatManager
	{
		public class DateTimeFormatHashTable : Hashtable
		{
			private class DateTimeFormatProperties
			{
				public string DateFormatPattern = "d-MMM-yy";

				public string TimeFormatPattern = "h:mm:ss tt";
			}

			public void SetDateFormatPattern(DateTimeFormatInfo format, string newPattern)
			{
				if (this[format] != null)
				{
					((DateTimeFormatProperties)this[format]).DateFormatPattern = newPattern;
					return;
				}
				DateTimeFormatProperties dateTimeFormatProperties = new DateTimeFormatProperties();
				dateTimeFormatProperties.DateFormatPattern = newPattern;
				Add(format, dateTimeFormatProperties);
			}

			public string GetDateFormatPattern(DateTimeFormatInfo format)
			{
				if (this[format] == null)
				{
					return "d-MMM-yy";
				}
				return ((DateTimeFormatProperties)this[format]).DateFormatPattern;
			}

			public void SetTimeFormatPattern(DateTimeFormatInfo format, string newPattern)
			{
				if (this[format] != null)
				{
					((DateTimeFormatProperties)this[format]).TimeFormatPattern = newPattern;
					return;
				}
				DateTimeFormatProperties dateTimeFormatProperties = new DateTimeFormatProperties();
				dateTimeFormatProperties.TimeFormatPattern = newPattern;
				Add(format, dateTimeFormatProperties);
			}

			public string GetTimeFormatPattern(DateTimeFormatInfo format)
			{
				if (this[format] == null)
				{
					return "h:mm:ss tt";
				}
				return ((DateTimeFormatProperties)this[format]).TimeFormatPattern;
			}
		}

		public static DateTimeFormatHashTable manager = new DateTimeFormatHashTable();
	}

	public class ArrayListSupport
	{
		public static object[] ToArray(ArrayList collection, object[] objects)
		{
			int num = 0;
			IEnumerator enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				objects[num++] = enumerator.Current;
			}
			return objects;
		}
	}

	public class ThreadClass : IThreadRunnable
	{
		private Thread threadField;

		public Thread Instance
		{
			get
			{
				return threadField;
			}
			set
			{
				threadField = value;
			}
		}

		public string Name
		{
			get
			{
				return threadField.Name;
			}
			set
			{
				if (threadField.Name == null)
				{
					threadField.Name = value;
				}
			}
		}

		public ThreadPriority Priority
		{
			get
			{
				return threadField.Priority;
			}
			set
			{
				threadField.Priority = value;
			}
		}

		public bool IsAlive => threadField.IsAlive;

		public bool IsBackground
		{
			get
			{
				return threadField.IsBackground;
			}
			set
			{
				threadField.IsBackground = value;
			}
		}

		public ThreadClass()
		{
			threadField = new Thread(Run);
		}

		public ThreadClass(string Name)
		{
			threadField = new Thread(Run);
			this.Name = Name;
		}

		public ThreadClass(ThreadStart Start)
		{
			threadField = new Thread(Start);
		}

		public ThreadClass(ThreadStart Start, string Name)
		{
			threadField = new Thread(Start);
			this.Name = Name;
		}

		public virtual void Run()
		{
		}

		public virtual void Start()
		{
			threadField.Start();
		}

		public virtual void Interrupt()
		{
			threadField.Interrupt();
		}

		public void Join()
		{
			threadField.Join();
		}

		public void Join(long MiliSeconds)
		{
			lock (this)
			{
				threadField.Join(new TimeSpan(MiliSeconds * 10000));
			}
		}

		public void Join(long MiliSeconds, int NanoSeconds)
		{
			lock (this)
			{
				threadField.Join(new TimeSpan(MiliSeconds * 10000 + NanoSeconds * 100));
			}
		}

		public void Resume()
		{
			threadField.Resume();
		}

		public void Abort()
		{
			threadField.Abort();
		}

		public void Abort(object stateInfo)
		{
			lock (this)
			{
				threadField.Abort(stateInfo);
			}
		}

		public void Suspend()
		{
			threadField.Suspend();
		}

		public override string ToString()
		{
			return string.Concat("Thread[" + Name + "," + Priority.ToString() + ",", "]");
		}

		public static ThreadClass Current()
		{
			return new ThreadClass
			{
				Instance = Thread.CurrentThread
			};
		}
	}

	public class CollectionSupport : CollectionBase
	{
		public virtual bool Add(object element)
		{
			return base.List.Add(element) != -1;
		}

		public virtual bool AddAll(ICollection collection)
		{
			bool result = false;
			if (collection != null)
			{
				IEnumerator enumerator = new ArrayList(collection).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						result = Add(enumerator.Current);
					}
				}
			}
			return result;
		}

		public virtual bool AddAll(CollectionSupport collection)
		{
			return AddAll((ICollection)collection);
		}

		public virtual bool Contains(object element)
		{
			return base.List.Contains(element);
		}

		public virtual bool ContainsAll(ICollection collection)
		{
			bool result = false;
			IEnumerator enumerator = new ArrayList(collection).GetEnumerator();
			while (enumerator.MoveNext() && (result = Contains(enumerator.Current)))
			{
			}
			return result;
		}

		public virtual bool ContainsAll(CollectionSupport collection)
		{
			return ContainsAll((ICollection)collection);
		}

		public virtual bool IsEmpty()
		{
			return base.Count == 0;
		}

		public virtual bool Remove(object element)
		{
			bool result = false;
			if (Contains(element))
			{
				base.List.Remove(element);
				result = true;
			}
			return result;
		}

		public virtual bool RemoveAll(ICollection collection)
		{
			bool result = false;
			IEnumerator enumerator = new ArrayList(collection).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (Contains(enumerator.Current))
				{
					result = Remove(enumerator.Current);
				}
			}
			return result;
		}

		public virtual bool RemoveAll(CollectionSupport collection)
		{
			return RemoveAll((ICollection)collection);
		}

		public virtual bool RetainAll(ICollection collection)
		{
			bool flag = false;
			IEnumerator enumerator = GetEnumerator();
			CollectionSupport collectionSupport = new CollectionSupport();
			collectionSupport.AddAll(collection);
			while (enumerator.MoveNext())
			{
				if (!collectionSupport.Contains(enumerator.Current))
				{
					flag = Remove(enumerator.Current);
					if (flag)
					{
						enumerator = GetEnumerator();
					}
				}
			}
			return flag;
		}

		public virtual bool RetainAll(CollectionSupport collection)
		{
			return RetainAll((ICollection)collection);
		}

		public virtual object[] ToArray()
		{
			int num = 0;
			object[] array = new object[base.Count];
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				array[num++] = enumerator.Current;
			}
			return array;
		}

		public virtual object[] ToArray(object[] objects)
		{
			int num = 0;
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				objects[num++] = enumerator.Current;
			}
			return objects;
		}

		public static CollectionSupport ToCollectionSupport(object[] array)
		{
			CollectionSupport collectionSupport = new CollectionSupport();
			collectionSupport.AddAll(array);
			return collectionSupport;
		}
	}

	public class ListCollectionSupport : ArrayList
	{
		public ListCollectionSupport()
		{
		}

		public ListCollectionSupport(ICollection collection)
			: base(collection)
		{
		}

		public ListCollectionSupport(int capacity)
			: base(capacity)
		{
		}

		public new virtual bool Add(object valueToInsert)
		{
			base.Insert(Count, valueToInsert);
			return true;
		}

		public virtual bool AddAll(int index, IList list)
		{
			bool result = false;
			if (list != null)
			{
				IEnumerator enumerator = new ArrayList(list).GetEnumerator();
				int num = index;
				while (enumerator.MoveNext())
				{
					base.Insert(num++, enumerator.Current);
					result = true;
				}
			}
			return result;
		}

		public virtual bool AddAll(IList collection)
		{
			return AddAll(Count, collection);
		}

		public virtual bool AddAll(CollectionSupport collection)
		{
			return AddAll(Count, collection);
		}

		public virtual bool AddAll(int index, CollectionSupport collection)
		{
			return AddAll(index, (IList)collection);
		}

		public virtual object ListCollectionClone()
		{
			return MemberwiseClone();
		}

		public virtual IEnumerator ListIterator()
		{
			return base.GetEnumerator();
		}

		public virtual bool RemoveAll(ICollection collection)
		{
			bool result = false;
			IEnumerator enumerator = new ArrayList(collection).GetEnumerator();
			while (enumerator.MoveNext())
			{
				result = true;
				if (base.Contains(enumerator.Current))
				{
					base.Remove(enumerator.Current);
				}
			}
			return result;
		}

		public virtual bool RemoveAll(CollectionSupport collection)
		{
			return RemoveAll((ICollection)collection);
		}

		public virtual object RemoveElement(int index)
		{
			object result = this[index];
			RemoveAt(index);
			return result;
		}

		public virtual bool RemoveElement(object element)
		{
			bool result = false;
			if (Contains(element))
			{
				base.Remove(element);
				result = true;
			}
			return result;
		}

		public virtual object RemoveFirst()
		{
			object result = this[0];
			RemoveAt(0);
			return result;
		}

		public virtual object RemoveLast()
		{
			object result = this[Count - 1];
			base.RemoveAt(Count - 1);
			return result;
		}

		public virtual bool RetainAll(ICollection collection)
		{
			bool flag = false;
			IEnumerator enumerator = GetEnumerator();
			ListCollectionSupport listCollectionSupport = new ListCollectionSupport(collection);
			while (enumerator.MoveNext())
			{
				if (!listCollectionSupport.Contains(enumerator.Current))
				{
					flag = RemoveElement(enumerator.Current);
					if (flag)
					{
						enumerator = GetEnumerator();
					}
				}
			}
			return flag;
		}

		public virtual bool RetainAll(CollectionSupport collection)
		{
			return RetainAll((ICollection)collection);
		}

		public virtual bool ContainsAll(ICollection collection)
		{
			bool result = false;
			IEnumerator enumerator = new ArrayList(collection).GetEnumerator();
			while (enumerator.MoveNext() && (result = Contains(enumerator.Current)))
			{
			}
			return result;
		}

		public virtual bool ContainsAll(CollectionSupport collection)
		{
			return ContainsAll((ICollection)collection);
		}

		public virtual ListCollectionSupport SubList(int startIndex, int endIndex)
		{
			int num = 0;
			GetEnumerator();
			ListCollectionSupport listCollectionSupport = new ListCollectionSupport();
			for (num = startIndex; num < endIndex; num++)
			{
				listCollectionSupport.Add(this[num]);
			}
			return listCollectionSupport;
		}

		public virtual object[] ToArray(object[] objects)
		{
			if (objects.Length < Count)
			{
				objects = new object[Count];
			}
			int num = 0;
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				objects[num++] = enumerator.Current;
			}
			return objects;
		}

		public virtual IEnumerator ListIterator(int index)
		{
			if (index < 0 || index > Count)
			{
				throw new IndexOutOfRangeException();
			}
			IEnumerator enumerator = GetEnumerator();
			if (index > 0)
			{
				int num = 0;
				while (enumerator.MoveNext() && num < index - 1)
				{
					num++;
				}
			}
			return enumerator;
		}

		public virtual object GetLast()
		{
			if (Count == 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this[Count - 1];
		}

		public virtual bool IsEmpty()
		{
			return Count == 0;
		}

		public virtual object Set(int index, object element)
		{
			object result = this[index];
			this[index] = element;
			return result;
		}

		public virtual object Get(int index)
		{
			return this[index];
		}
	}

	public class ArraysSupport
	{
		public static bool IsArrayEqual(Array array1, Array array2)
		{
			if (array1.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array1.Length; i++)
			{
				if (!array1.GetValue(i).Equals(array2.GetValue(i)))
				{
					return false;
				}
			}
			return true;
		}

		public static void FillArray(Array array, int fromindex, int toindex, object val)
		{
			object value = val;
			Type elementType = array.GetType().GetElementType();
			if (elementType != val.GetType())
			{
				value = Convert.ChangeType(val, elementType);
			}
			if (array.Length == 0)
			{
				throw new NullReferenceException();
			}
			if (fromindex > toindex)
			{
				throw new ArgumentException();
			}
			if (fromindex < 0 || array.Length < toindex)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = ((fromindex > 0) ? fromindex-- : fromindex); i < toindex; i++)
			{
				array.SetValue(value, i);
			}
		}

		public static void FillArray(Array array, object val)
		{
			FillArray(array, 0, array.Length, val);
		}
	}

	public class SetSupport : ArrayList
	{
		public SetSupport()
		{
		}

		public SetSupport(ICollection collection)
			: base(collection)
		{
		}

		public SetSupport(int capacity)
			: base(capacity)
		{
		}

		public new virtual bool Add(object objectToAdd)
		{
			if (Contains(objectToAdd))
			{
				return false;
			}
			base.Add(objectToAdd);
			return true;
		}

		public virtual bool AddAll(ICollection collection)
		{
			bool result = false;
			if (collection != null)
			{
				IEnumerator enumerator = new ArrayList(collection).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						result = Add(enumerator.Current);
					}
				}
			}
			return result;
		}

		public virtual bool AddAll(CollectionSupport collection)
		{
			return AddAll((ICollection)collection);
		}

		public virtual bool ContainsAll(ICollection collection)
		{
			bool result = false;
			IEnumerator enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext() && (result = Contains(enumerator.Current)))
			{
			}
			return result;
		}

		public virtual bool ContainsAll(CollectionSupport collection)
		{
			return ContainsAll((ICollection)collection);
		}

		public virtual bool IsEmpty()
		{
			return Count == 0;
		}

		public new virtual bool Remove(object elementToRemove)
		{
			bool result = false;
			if (Contains(elementToRemove))
			{
				result = true;
			}
			base.Remove(elementToRemove);
			return result;
		}

		public virtual bool RemoveAll(ICollection collection)
		{
			bool flag = false;
			IEnumerator enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!flag && Contains(enumerator.Current))
				{
					flag = true;
				}
				Remove(enumerator.Current);
			}
			return flag;
		}

		public virtual bool RemoveAll(CollectionSupport collection)
		{
			return RemoveAll((ICollection)collection);
		}

		public virtual bool RetainAll(ICollection collection)
		{
			bool result = false;
			IEnumerator enumerator = collection.GetEnumerator();
			SetSupport setSupport = (SetSupport)collection;
			while (enumerator.MoveNext())
			{
				if (!setSupport.Contains(enumerator.Current))
				{
					result = Remove(enumerator.Current);
					enumerator = GetEnumerator();
				}
			}
			return result;
		}

		public virtual bool RetainAll(CollectionSupport collection)
		{
			return RetainAll((ICollection)collection);
		}

		public new virtual object[] ToArray()
		{
			int num = 0;
			object[] array = new object[Count];
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				array[num++] = enumerator.Current;
			}
			return array;
		}

		public virtual object[] ToArray(object[] objects)
		{
			int num = 0;
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				objects[num++] = enumerator.Current;
			}
			return objects;
		}
	}

	public class AbstractSetSupport : SetSupport
	{
	}

	public class MessageDigestSupport
	{
		private HashAlgorithm algorithm;

		private byte[] data;

		private int position;

		private string algorithmName;

		public HashAlgorithm Algorithm
		{
			get
			{
				return algorithm;
			}
			set
			{
				algorithm = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		public string AlgorithmName => algorithmName;

		public MessageDigestSupport(string algorithm)
		{
			if (algorithm.Equals("SHA-1"))
			{
				algorithmName = "SHA";
			}
			else
			{
				algorithmName = algorithm;
			}
			Algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(algorithmName);
			position = 0;
		}

		[CLSCompliant(false)]
		public sbyte[] DigestData()
		{
			sbyte[] result = ToSByteArray(Algorithm.ComputeHash(data));
			Reset();
			return result;
		}

		[CLSCompliant(false)]
		public sbyte[] DigestData(byte[] newData)
		{
			Update(newData);
			return DigestData();
		}

		public void Update(byte[] newData)
		{
			if (position == 0)
			{
				Data = newData;
				position = Data.Length - 1;
				return;
			}
			byte[] array = Data;
			Data = new byte[newData.Length + position + 1];
			array.CopyTo(Data, 0);
			newData.CopyTo(Data, array.Length);
			position = Data.Length - 1;
		}

		public void Update(byte newData)
		{
			Update(new byte[1] { newData });
		}

		public void Update(byte[] newData, int offset, int count)
		{
			byte[] array = new byte[count];
			Array.Copy(newData, offset, array, 0, count);
			Update(array);
		}

		public void Reset()
		{
			data = null;
			position = 0;
		}

		public override string ToString()
		{
			return Algorithm.ToString();
		}

		public static MessageDigestSupport GetInstance(string algorithm)
		{
			return new MessageDigestSupport(algorithm);
		}

		[CLSCompliant(false)]
		public static bool EquivalentDigest(sbyte[] firstDigest, sbyte[] secondDigest)
		{
			bool flag = false;
			if (firstDigest.Length == secondDigest.Length)
			{
				int num = 0;
				flag = true;
				while (flag && num < firstDigest.Length)
				{
					flag = firstDigest[num] == secondDigest[num];
					num++;
				}
			}
			return flag;
		}
	}

	public class SecureRandomSupport
	{
		private RNGCryptoServiceProvider generator;

		public SecureRandomSupport()
		{
			generator = new RNGCryptoServiceProvider();
		}

		public SecureRandomSupport(byte[] seed)
		{
			generator = new RNGCryptoServiceProvider(seed);
		}

		[CLSCompliant(false)]
		public sbyte[] NextBytes(byte[] randomnumbersarray)
		{
			generator.GetBytes(randomnumbersarray);
			return ToSByteArray(randomnumbersarray);
		}

		public static byte[] GetSeed(int numberOfBytes)
		{
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			byte[] array = new byte[numberOfBytes];
			rNGCryptoServiceProvider.GetBytes(array);
			return array;
		}

		public void SetSeed(byte[] newSeed)
		{
			generator = new RNGCryptoServiceProvider(newSeed);
		}

		public void SetSeed(long newSeed)
		{
			byte[] array = new byte[8];
			for (int num = 7; num > 0; num--)
			{
				array[num] = (byte)(newSeed - (newSeed >> 8 << 8));
				newSeed >>= 8;
			}
			SetSeed(array);
		}
	}

	public interface SingleThreadModel
	{
	}

	[CLSCompliant(false)]
	public static sbyte[] ToSByteArray(byte[] byteArray)
	{
		sbyte[] array = new sbyte[byteArray.Length];
		for (int i = 0; i < byteArray.Length; i++)
		{
			array[i] = (sbyte)byteArray[i];
		}
		return array;
	}

	[CLSCompliant(false)]
	public static byte[] ToByteArray(sbyte[] sbyteArray)
	{
		byte[] array = new byte[sbyteArray.Length];
		for (int i = 0; i < sbyteArray.Length; i++)
		{
			array[i] = (byte)sbyteArray[i];
		}
		return array;
	}

	public static byte[] ToByteArray(string sourceString)
	{
		byte[] array = new byte[sourceString.Length];
		for (int i = 0; i < sourceString.Length; i++)
		{
			array[i] = (byte)sourceString[i];
		}
		return array;
	}

	public static byte[] ToByteArray(object[] tempObjectArray)
	{
		byte[] array = new byte[tempObjectArray.Length];
		for (int i = 0; i < tempObjectArray.Length; i++)
		{
			array[i] = (byte)tempObjectArray[i];
		}
		return array;
	}

	[CLSCompliant(false)]
	public static int ReadInput(Stream sourceStream, ref sbyte[] target, int start, int count)
	{
		if (target.Length == 0)
		{
			return 0;
		}
		byte[] array = new byte[target.Length];
		int num = 0;
		int num2 = start;
		int num3 = count;
		while (num3 > 0)
		{
			int num4 = sourceStream.Read(array, num2, num3);
			if (num4 == 0)
			{
				break;
			}
			num += num4;
			num2 += num4;
			num3 -= num4;
		}
		if (num == 0)
		{
			return -1;
		}
		for (int i = start; i < start + num; i++)
		{
			target[i] = (sbyte)array[i];
		}
		return num;
	}

	[CLSCompliant(false)]
	public static int ReadInput(TextReader sourceTextReader, ref sbyte[] target, int start, int count)
	{
		if (target.Length == 0)
		{
			return 0;
		}
		char[] array = new char[target.Length];
		int num = sourceTextReader.Read(array, start, count);
		if (num == 0)
		{
			return -1;
		}
		for (int i = start; i < start + num; i++)
		{
			target[i] = (sbyte)array[i];
		}
		return num;
	}

	public static long Identity(long literal)
	{
		return literal;
	}

	[CLSCompliant(false)]
	public static ulong Identity(ulong literal)
	{
		return literal;
	}

	public static float Identity(float literal)
	{
		return literal;
	}

	public static double Identity(double literal)
	{
		return literal;
	}

	public static string FormatDateTime(DateTimeFormatInfo format, DateTime date)
	{
		string timeFormatPattern = DateTimeFormatManager.manager.GetTimeFormatPattern(format);
		string dateFormatPattern = DateTimeFormatManager.manager.GetDateFormatPattern(format);
		return date.ToString(dateFormatPattern + " " + timeFormatPattern, format);
	}

	public static object PutElement(IDictionary collection, object key, object newValue)
	{
		object result = collection[key];
		collection[key] = newValue;
		return result;
	}

	public static bool VectorRemoveElement(IList arrayList, object element)
	{
		bool result = arrayList.Contains(element);
		arrayList.Remove(element);
		return result;
	}

	public static object HashtableRemove(Hashtable hashtable, object key)
	{
		object result = hashtable[key];
		hashtable.Remove(key);
		return result;
	}

	public static void SetSize(ArrayList arrayList, int newSize)
	{
		if (newSize < 0)
		{
			throw new ArgumentException();
		}
		if (newSize < arrayList.Count)
		{
			arrayList.RemoveRange(newSize, arrayList.Count - newSize);
			return;
		}
		while (newSize > arrayList.Count)
		{
			arrayList.Add(null);
		}
	}

	public static object StackPush(Stack stack, object element)
	{
		stack.Push(element);
		return element;
	}

	public static void GetCharsFromString(string sourceString, int sourceStart, int sourceEnd, ref char[] destinationArray, int destinationStart)
	{
		int num = sourceStart;
		int num2 = destinationStart;
		while (num < sourceEnd)
		{
			destinationArray[num2] = sourceString[num];
			num++;
			num2++;
		}
	}

	public static FileStream GetFileStream(string FileName, bool Append)
	{
		if (Append)
		{
			return new FileStream(FileName, FileMode.Append);
		}
		return new FileStream(FileName, FileMode.Create);
	}

	[CLSCompliant(false)]
	public static char[] ToCharArray(sbyte[] sByteArray)
	{
		char[] array = new char[sByteArray.Length];
		sByteArray.CopyTo(array, 0);
		return array;
	}

	public static char[] ToCharArray(byte[] byteArray)
	{
		char[] array = new char[byteArray.Length];
		byteArray.CopyTo(array, 0);
		return array;
	}

	public static object CreateNewInstance(Type classType)
	{
		object result = null;
		Type[] types = new Type[0];
		ConstructorInfo[] array = null;
		array = classType.GetConstructors();
		if (array.Length == 0)
		{
			throw new UnauthorizedAccessException();
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetParameters().Length == 0)
			{
				result = classType.GetConstructor(types).Invoke(new object[0]);
				break;
			}
			if (i == array.Length - 1)
			{
				throw new MethodAccessException();
			}
		}
		return result;
	}

	public static void WriteStackTrace(Exception throwable, TextWriter stream)
	{
		stream.Write(throwable.StackTrace);
		stream.Flush();
	}

	public static bool EqualsSupport(ICollection source, ICollection target)
	{
		IEnumerator enumerator = ReverseStack(source);
		IEnumerator enumerator2 = ReverseStack(target);
		if (source.Count != target.Count)
		{
			return false;
		}
		while (enumerator.MoveNext() && enumerator2.MoveNext())
		{
			if (!enumerator.Current.Equals(enumerator2.Current))
			{
				return false;
			}
		}
		return true;
	}

	public static bool EqualsSupport(ICollection source, object target)
	{
		if (target.GetType() != typeof(ICollection))
		{
			return false;
		}
		return EqualsSupport(source, (ICollection)target);
	}

	public static bool EqualsSupport(IDictionaryEnumerator source, object target)
	{
		if (target.GetType() != typeof(IDictionaryEnumerator))
		{
			return false;
		}
		return EqualsSupport(source, (IDictionaryEnumerator)target);
	}

	public static bool EqualsSupport(IDictionaryEnumerator source, IDictionaryEnumerator target)
	{
		while (source.MoveNext() && target.MoveNext())
		{
			if (source.Key.Equals(target.Key) && source.Value.Equals(target.Value))
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerator ReverseStack(ICollection collection)
	{
		if (collection.GetType() == typeof(Stack))
		{
			ArrayList arrayList = new ArrayList(collection);
			arrayList.Reverse();
			return arrayList.GetEnumerator();
		}
		return collection.GetEnumerator();
	}
}
