﻿using System;
using JetBrains.Annotations;
using LibGit2Sharp;
using UIComponents;

namespace UnityGit.Core.Services
{
    public interface ISignatureService
    {
        [CanBeNull]
        Signature GetSignature();
    }
    
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    public sealed partial class SignatureService : Service, ISignatureService
    {
        [Provide]
        private IStatusService _statusService;

        public Signature GetSignature()
        {
            var hasProjectRepository = _statusService.HasProjectRepository();

            if (!hasProjectRepository && !_statusService.HasPackageRepositories())
                return null;
            
            if (hasProjectRepository)
            {
                var projectSignature =
                    _statusService.ProjectRepository.Config.BuildSignature(DateTimeOffset.Now);

                if (projectSignature != null)
                    return projectSignature;
            }

            foreach (var repository in _statusService.PackageRepositories)
            {
                var packageRepoSignature =
                    repository.Config.BuildSignature(DateTimeOffset.Now);

                if (packageRepoSignature != null)
                    return packageRepoSignature;
            }

            return null;
        }
    }
}
