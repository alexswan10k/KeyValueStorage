using System;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Cryptography.Hashers;
using KeyValueStorage.Tools.Stores;
using KeyValueStorage.Tools.Utility.CharGenerators;
using KeyValueStorage.Tools.Utility.Strings;

namespace KeyValueStorage.Tools
{
	public class KvLoginProvider
	{
		private readonly IRandomCharacterGenerator _characterGen;
		private readonly IStringVerifier _usernameVerifier;
		private readonly IStringVerifier _passwordVerifier;
		private readonly IKVStore _store;
		private readonly IHasher _hasher;

		public KvLoginProvider(IKVStore store, string namespacePrefix = "ULP:", IRandomCharacterGenerator characterGen = null, IHasher hasher = null, IStringVerifier usernameVerifier= null, IStringVerifier passwordVerifier = null)
		{
            _store = new KeyTransformKVStore(store, new PrefixTransformer(namespacePrefix));
			_usernameVerifier = usernameVerifier ?? new NullStringVerifier();
			_passwordVerifier = passwordVerifier ?? new NullStringVerifier();
			_characterGen = characterGen ?? new SimpleRandomCharacterGenerator();
			_hasher = hasher ?? new Md5HasherWithSalt();
		}

		public UserCreationResult CreateUser(string username, string password)
		{
			var user = _GetUser(username);
			if(user != null)
				return new UserCreationResult(UserCreationResultCode.UsernameAlreadyExists);

			if(!_usernameVerifier.Verify(username))
				return new UserCreationResult(UserCreationResultCode.UsernameInvalid);
			if(_passwordVerifier.Verify(password))
				return new UserCreationResult(UserCreationResultCode.PasswordInvalid);

			_SetUser(username, new UserLoginDetailsInt(){HashedData = _hasher.ComputeHash(password), DateCreated = DateTime.UtcNow});
			return new UserCreationResult(UserCreationResultCode.Success);
		}

		public bool UserExists(string username)
		{
			return _GetUser(username) != null;
		}

		public AutherizationRequest Authorize(string username, string password)
		{
			var user = _GetUser(username);

			IHasher hasherLocal;

			if(_hasher.GetType().FullName == user.HashedData.EncrType)
				hasherLocal = _hasher;
			else
			{
				var hashAlgoType = Type.GetType(user.HashedData.EncrType, false);
				if(hashAlgoType == null)
					return AutherizationRequest.CannotFindIHashAlgorithmImplementation;

				hasherLocal = Activator.CreateInstance(hashAlgoType) as IHasher;
			}

			if(hasherLocal.Verify(password, user.HashedData))
			{
				user.LastLogin = DateTime.UtcNow;
				user.FailedLogins = 0;
				_SetUser(username, user);
				return AutherizationRequest.Success;
			}

			user.FailedLogins++;
			_SetUser(username, user);

			return AutherizationRequest.PasswordInvalid;
		}

		public bool UpdatePassword(string username, string newPassword)
		{
			var user = _GetUser(username);

			if(user == null)
				return false;

			user.HashedData= _hasher.ComputeHash(newPassword);

			_SetUser(username, user);
            return true;
		}

		public string GenerateAndSetNewPassword(string username)
		{
			var user = _GetUser(username);
			if(user == null)
				throw new ArgumentException(username +" does note exist");

			var newPassword= _characterGen.Generate();

			user.HashedData= _hasher.ComputeHash(newPassword);
			_SetUser(username, user);

			return newPassword;
		}

		public int GetFailedAuthorizationAttempts(string username)
		{
			var user = _GetUser(username);

			if(user == null)
				return -1;

			return user.FailedLogins;
		}

		private UserLoginDetailsInt _GetUser(string username)
		{
			return _store.Get<UserLoginDetailsInt>(username);
		}

		private void _SetUser(string username, UserLoginDetailsInt details)
		{
			_store.Set(username, details);
		}

        class UserLoginDetailsInt
        {
            public HashedData HashedData { get; set; }
            public DateTime LastLogin { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DatePasswordChanged { get; set; }
            public int FailedLogins { get; set; }
        }
	}

    public class UserCreationResult
	{
		public UserCreationResultCode Code{get;set;}
		public UserCreationResult(UserCreationResultCode code)
		{
			Code = code;
		}
	}

	public enum UserCreationResultCode
	{
		Success,
		UsernameAlreadyExists,
		UnknownError,
		UsernameInvalid,
		PasswordInvalid
	}

	public enum AutherizationRequest
	{
		Success,
		PasswordInvalid,
		ExceededMaxLoginAttempts,
		CannotFindIHashAlgorithmImplementation
	}
}
