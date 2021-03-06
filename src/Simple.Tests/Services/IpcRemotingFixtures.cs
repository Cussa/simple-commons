﻿using System;
using NUnit.Framework;
using SharpTestsEx;

namespace Simple.Tests.Services
{
    public class IpcConstants
    {
        public const string Scheme = "ipc";
        public const string StartPort = "localserver";
    }

    [TestFixture]
    [Explicit("not stable")]
    public class IpcRemotingFactoryFixture : BaseRemotingFactoryFixture
    {
        public override Uri Uri
        {
            get { return Helper.MakeUri(IpcConstants.Scheme, IpcConstants.StartPort + 1); }
        }

    }

    [TestFixture]
    [Explicit("not stable")]
    public class IpcSelfRemotingFactoryFixture : BaseSelfRemotingFactoryFixture
    {
        public override Uri Uri
        {
            get { return Helper.MakeUri(IpcConstants.Scheme, IpcConstants.StartPort + 2); }
        }

    }

    [TestFixture]
    [Explicit("not stable")]
    public class IpcRemotingServerInterceptorFixture : BaseRemotingServerInterceptorFixture
    {
        public override Uri Uri
        {
            get { return Helper.MakeUri(IpcConstants.Scheme, IpcConstants.StartPort + 3); }
        }
    }
    [TestFixture]
    [Explicit("not stable")]
    public class IpcRemotingClientInterceptorFixture : BaseRemotingClientInterceptorFixture
    {
        public override Uri Uri
        {
            get { return Helper.MakeUri(IpcConstants.Scheme, IpcConstants.StartPort + 4); }
        }
    }

    [TestFixture]
    [Explicit("not stable")]
    public class IpcSelfRemotingServerInterceptorFixture : BaseSelfRemotingServerInterceptorFixture
    {
        public override Uri Uri
        {
            get { return Helper.MakeUri(IpcConstants.Scheme, IpcConstants.StartPort + 5); }
        }

    }

    [TestFixture]
    [Explicit("not stable")]
    public class IpcSelfRemotingClientInterceptorFixture : BaseSelfRemotingClientInterceptorFixture
    {
        public override Uri Uri
        {
            get { return Helper.MakeUri(IpcConstants.Scheme, IpcConstants.StartPort + 6); }
        }

    }

}
