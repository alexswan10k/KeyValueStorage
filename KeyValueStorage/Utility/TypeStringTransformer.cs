using System;
using System.Collections.Generic;

namespace KeyValueStorage.Utility
{
	public class TypeStringTransformer : ITypeStringTransformer
	{
		private readonly string _delimiter;
		private readonly Dictionary<Type, string> _typeAliases;

		public TypeStringTransformer(Dictionary<Type, string> typePrefixMaps = null, string delimiter = ".")
		{
			_delimiter = delimiter;
			_typeAliases = typePrefixMaps ?? new Dictionary<Type, string>();
		}


		static TypeStringTransformer()
		{
			Default = new TypeStringTransformer();
		}
		public static ITypeStringTransformer Default { get; set; }

		public string TransformFor<T>(Key keyT)
		{
			return _GetTypeAliasFor<T>() + _delimiter + keyT;
		}

		public string TransformFor<T, U>(Key keyT)
		{
			return _GetTypeAliasFor<T>() + _delimiter + keyT + _delimiter + _GetTypeAliasFor<U>();
		}

		private string _GetTypeAliasFor<T>()
		{
			string typeAlias;
			if (_typeAliases.TryGetValue(typeof(T), out typeAlias))
				return typeAlias;

			return typeof(T).Name;
		}
	}


	public class ForeignKeyTypeStringTransformer : ITypeStringTransformer
	{
		private readonly string _delimiter;
		private readonly Dictionary<Type, string> _typeAliases;

		public ForeignKeyTypeStringTransformer(Dictionary<Type, string> typePrefixMaps = null, string delimiter = ".")
		{
			_delimiter = delimiter;
			_typeAliases = typePrefixMaps ?? new Dictionary<Type, string>();
		}

		public string TransformFor<T, U>(Key keyT)
		{
			return keyT + _delimiter + _GetTypeAliasFor<U>();
		}

		public string TransformFor<T>(Key keyT)
		{
			return keyT + _delimiter + _GetTypeAliasFor<T>();
		}

		private string _GetTypeAliasFor<T>()
		{
			string typeAlias;
			if (_typeAliases.TryGetValue(typeof(T), out typeAlias))
				return typeAlias;

			return typeof(T).Name;
		}
	}

	public interface ITypeStringTransformer
	{
		string TransformFor<T, U>(Key keyT);
		string TransformFor<T>(Key keyT);
	}
}