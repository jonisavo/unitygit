﻿using System.Collections.Generic;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Credentials = UnityGit.Core.Data.Credentials;

namespace UnityGit.Core.Services
{
    public interface ICredentialsService
    {
        Credentials GetCredentialsForRepository(IRepository repository);

        bool HasCredentialsForRepository(IRepository repository);

        void SetCredentialsForRepository(IRepository repository, Credentials credentials);
        
        CredentialsHandler GetCredentialsHandlerForRepository(IRepository repository);
    }
    
    public sealed class CredentialsService : ICredentialsService
    {
        private readonly Dictionary<IRepository, Credentials> _credentials =
            new Dictionary<IRepository, Credentials>();
        
        public Credentials GetCredentialsForRepository(IRepository repository)
        {
            if (!HasCredentialsForRepository(repository))
                return Credentials.InvalidCredentials;
            
            return _credentials[repository];
        }

        public bool HasCredentialsForRepository(IRepository repository)
        {
            return _credentials.ContainsKey(repository);
        }

        public void SetCredentialsForRepository(IRepository repository, Credentials credentials)
        {
            _credentials[repository] = credentials;
        }
        
        public CredentialsHandler GetCredentialsHandlerForRepository(IRepository repository)
        {
            return (_url, _userName, _cred) =>
            {
                var credentials = GetCredentialsForRepository(repository);
                return new UsernamePasswordCredentials
                {
                    Username = credentials.Username,
                    Password = credentials.Password
                };
            };
        }
    }
}
