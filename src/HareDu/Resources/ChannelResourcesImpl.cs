﻿// Copyright 2013-2014 Albert L. Hives, Chris Patterson, et al.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace HareDu.Resources
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using Model;

    internal class ChannelResourcesImpl :
        HareDuResourcesBase,
        ChannelResources
    {
        public ChannelResourcesImpl(HttpClient client, ILog logger) :
            base(client, logger)
        {
        }

        public Task<IEnumerable<Channel>> GetAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.RequestCanceled(LogInfo);

            LogInfo("Sent request to return all information pertaining to all channels on current RabbitMQ server.");

            string url = "api/channels";

            return base.Get(url, cancellationToken).As<IEnumerable<Channel>>(cancellationToken);
        }

        public Task<Channel> Get(string channelName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.RequestCanceled(LogInfo);

            string url = string.Format("api/channels/{0}", channelName);

            LogInfo(
                string.Format(
                    "Sent request to return all information pertaining to channel '{0}' on current RabbitMQ server.",
                    channelName));

            return base.Get(url, cancellationToken).As<Channel>(cancellationToken);
        }
    }
}