﻿using System.Net.Http;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
